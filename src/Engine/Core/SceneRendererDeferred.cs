using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core
{
    enum DeferredPasses
    {
        GEOMETRY,
        SSAO,
        SSAO_BLUR,
        SHADOW,
        FXAA,
        LIGHTING
    }

    /// <summary>
    /// Use a Scene Renderer to traverse a scene hierarchy (made out of scene nodes and components) in order
    /// to have each visited element contribute to the result rendered against a given render context.
    /// </summary>
    public class SceneRendererDeferred : SceneRendererForward
    {
        /// <summary>
        /// The distance to the near clipping plane of the current projection matrix.
        /// </summary>
        public float ZNear { get; private set; }

        /// <summary>
        /// The distance to the far clipping plane of the current projection matrix.
        /// </summary>
        public float ZFar { get; private set; }

        /// <summary>
        /// The vertical field of view of the current projection matrix.
        /// </summary>
        public float Fov { get; private set; }

        /// <summary>
        /// Sets the GL.ClearColor
        /// </summary>
        public float4 BackgroundColor;

        /// <summary>
        /// Sets the Shadow Map resolution.
        /// </summary>
        public TexRes ShadowMapRes = TexRes.MID_RES;

        /// <summary>
        /// Determines if the scene gets rendered with Fast Approximate Anti Aliasing.
        /// </summary>
        public bool FxaaOn { get { return _fxaaOn; } set { _needToRecreateLightingMatOther = true; _needToRecreateLightingMatPoint = true; _needToRecreateLightingMatNoShadow = true; _fxaaOn = value; } }
        private bool _fxaaOn = true;

        private bool _needToRecreateLightingMatOther = false;
        private bool _needToRecreateLightingMatPoint = false;
        private bool _needToRecreateLightingMatNoShadow = false;

        private float4 _texClearColor = new float4(1, 1, 1, 1);
        private readonly SceneContainer _quadScene = new SceneContainer();
        private readonly ShaderEffectComponent _quadShaderEffectComp;
        private bool _isBackgroundColorSet = false;

        private ShaderEffect _ssaoTexEffect;
        private ShaderEffect _lightingPassEffect;       
        private ShaderEffect _blurEffect;
        private ShaderEffect _fxaaEffect;

        private ShaderEffect _lightingPassEffectPoint; //needed when a point light is rendered;
        private ShaderEffect _lightingPassEffectOther; //needed when a light of another type is rendered;
        private ShaderEffect _lightingPassEffectNoShadow; //needed when a light of another type is rendered;

        private DeferredPasses _currentPass;

        private readonly RenderTarget _gBufferRenderTarget;
        
        private readonly RenderTarget _blurRenderTarget;
        private readonly RenderTarget _ambientLightedSceneRenderTarget;
        private readonly RenderTarget _lightedSceneRenderTarget;

        private readonly WritableTexture _ssaoRenderTexture;

        private readonly Dictionary<Tuple<SceneNodeContainer, LightComponent>, ShadowParams> _shadowRenderTargets; //One per Light 

        private readonly TexRes _texRes;
        private readonly ProjectionComponent _projectionComponent;
        private readonly ShaderEffect _shadowEffect;
        private ShaderEffect _shadowCubeMapEffect;

        internal class ShadowParams
        {
            // For omni directional shadow mapping there will be six LightSpaceMatrices
            // to allow the creation of the cube map in one pass.
            public float4x4[] LightSpaceMats;
            //TODO: not cool - think of some other way to distinguish between cube map and normal texture.
            public IWritableCubeMap CubeShadowMap;
            public IWritableTexture ShadowMap;
            public float2 ClipPlanesForLightMat;
        }

        private float4x4 _thisScenesProjection = float4x4.Identity;

        /// <summary>
        /// Creates a new instance of type SceneRendererDeferred.
        /// </summary>
        /// <param name="sc">The SceneContainer, containing the scene that gets rendered.</param>
        /// <param name="texRes">Resolution of the render textures.</param>
        /// <param name="projComp">The projection component, that is used to render the scene.</param>
        public SceneRendererDeferred(SceneContainer sc, TexRes texRes, ProjectionComponent projComp) : base(sc)
        {
            _texRes = texRes;
            _projectionComponent = projComp;
            _gBufferRenderTarget = new RenderTarget(_texRes);
            _gBufferRenderTarget.SetPositionTex();
            _gBufferRenderTarget.SetAlbedoSpecularTex();
            _gBufferRenderTarget.SetNormalTex();
            _gBufferRenderTarget.SetDepthTex();

            _ssaoRenderTexture = new WritableTexture(RenderTargetTextureTypes.G_SSAO, new ImagePixelFormat(ColorFormat.fRGB32), (int)texRes, (int)texRes, false, TextureFilterMode.NEAREST);


            _blurRenderTarget = new RenderTarget(_texRes);
            _blurRenderTarget.SetSSAOTex();

            _lightedSceneRenderTarget = new RenderTarget(_texRes);
            _lightedSceneRenderTarget.SetAlbedoSpecularTex();

            _ambientLightedSceneRenderTarget = new RenderTarget(_texRes);

            _shadowRenderTargets = new Dictionary<Tuple<SceneNodeContainer, LightComponent>, ShadowParams>();

            _gBufferRenderTarget.DeleteBuffers += DeleteBuffers;            
            _blurRenderTarget.DeleteBuffers += DeleteBuffers;
            _lightedSceneRenderTarget.DeleteBuffers += DeleteBuffers;
            _ambientLightedSceneRenderTarget.DeleteBuffers += DeleteBuffers;

            _shadowEffect = ShaderCodeBuilder.ShadowMapEffect();            

            //Create ShaderEffect for deferred rendering ...TODO: add support for textures, create shader effect in Scene Converter to avoid another traversal.
            foreach (var child in sc.Children)
            {
                var oldEffectComp = child.GetComponent<ShaderEffectComponent>();
                if (oldEffectComp == null) continue;
                var oldEffect = oldEffectComp.Effect;

                var col = (float4)oldEffect.GetEffectParam("DiffuseColor");
                var specStrength = (float)oldEffect.GetEffectParam("SpecularIntensity");
                col.a = specStrength;

                oldEffect.GetEffectParam("DiffuseTexture", out object tex);
                oldEffect.GetEffectParam("DiffuseMix", out object mix);

                var renderTargetMat = new ShaderEffectComponent();
                if (tex != null)
                {
                    renderTargetMat.Effect = ShaderCodeBuilder.GBufferTextureEffect(_gBufferRenderTarget, (float)mix, (Texture)tex);
                }
                else
                {
                    renderTargetMat.Effect = ShaderCodeBuilder.GBufferTextureEffect(_gBufferRenderTarget, 1f);
                }

                child.RemoveComponent<ShaderEffectComponent>();

                child.Components.Insert(1, renderTargetMat);
                renderTargetMat.Effect.SetEffectParam("DiffuseColor", col);
            }

            _quadScene = new SceneContainer()
            {
                Children = new List<SceneNodeContainer>()
                {
                    new SceneNodeContainer()
                    {
                        Components = new List<SceneComponentContainer>()
                        {
                            _projectionComponent,

                            new ShaderEffectComponent()
                            {
                                Effect = _lightingPassEffect
                            },
                            new Plane()

                        }
                    }
                }
            };

            _quadShaderEffectComp = (ShaderEffectComponent)_quadScene.Children[0].Components[1];
        }

        /// <summary>
        /// If a Mesh is visited and it has a <see cref="WeightComponent"/> the BoneIndices and  BoneWeights get set, 
        /// the shader parameters for all lights in the scene are updated according to the <see cref="LightViserator"/>
        /// and the geometry is passed to be pushed through the rendering pipeline.        
        /// </summary>
        /// <param name="mesh">The Mesh.</param>
        [VisitMethod]
        public new void RenderMesh(Mesh mesh)
        {
            if (!mesh.Active) return;

            WeightComponent wc = CurrentNode.GetWeights();
            if (wc != null)
                AddWeightComponentToMesh(mesh, wc);

            _rc.Render(mesh);
        }

        /// <summary>
        /// If a ShaderEffectComponent is visited the ShaderEffect of the <see cref="RendererState"/> is updated and the effect is set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="shaderComponent">The ShaderEffectComponent</param>
        [VisitMethod]
        public new void RenderShaderEffect(ShaderEffectComponent shaderComponent)
        {
            if (HasNumberOfLightsChanged)
            {
                //change #define MAX_LIGHTS... or rebuild shader effect?
                HasNumberOfLightsChanged = false;
            }

            if (_currentPass != DeferredPasses.SHADOW)
            {
                _rc.SetShaderEffect(shaderComponent.Effect);
                _state.Effect = shaderComponent.Effect;
            }
            
        }

        /// <summary>
        /// If a Projection Component is visited, the projection matrix is set.
        /// </summary>
        /// <param name="pc">The visited ProjectionComponent.</param>
        [VisitMethod]
        public new void RenderProjection(ProjectionComponent pc)
        {
            Fov = pc.Fov;
            ZNear = pc.ZNear;
            ZFar = pc.ZFar;
            base.RenderProjection(pc);
            _thisScenesProjection = _rc.Projection;
        }

        //TODO: PROBLEM Without cascaded shadow maps and a large frustum we get a large AABB, possibly spanning the whole scene and therefore imprecise shadows.
        private float4x4 GetProjectionForParallelLight(float4x4 lightView, out float2 clipPlanes)
        {
            //1. Calculate the 8 corners of the view frustum in world space. This can be done by using the inverse view-projection matrix to transform the 8 corners of the NDC cube (which in OpenGL is [‒1, 1] along each axis).
            //2. Transform the frustum corners to a space aligned with the shadow map axes.This would commonly be the directional light object's local space. 
            //In fact, steps 1 and 2 can be done in one step by combining the inverse view-projection matrix of the camera with the inverse world matrix of the light.
            var invViewProjection = float4x4.Invert(_thisScenesProjection * _rc.View);            

            var frustumCorners = new float4[8];

            frustumCorners[0] = invViewProjection * new float4(-1, -1, -1, 1); //nbl
            frustumCorners[1] = invViewProjection * new float4(1, -1, -1, 1); //nbr 
            frustumCorners[2] = invViewProjection * new float4(-1, 1, -1, 1); //ntl  
            frustumCorners[3] = invViewProjection * new float4(1, 1, -1, 1); //ntr  
            frustumCorners[4] = invViewProjection * new float4(-1, -1, 1, 1); //fbl 
            frustumCorners[5] = invViewProjection * new float4(1, -1, 1, 1); //fbr 
            frustumCorners[6] = invViewProjection * new float4(-1, 1, 1, 1); //ftl  
            frustumCorners[7] = invViewProjection * new float4(1, 1, 1, 1); //ftr           

            for (int i = 0; i < frustumCorners.Length; i++)
            {
                var corner = frustumCorners[i];
                corner /= corner.w; //world space frustum corners
                corner = lightView * corner; //light space frustum corners

                frustumCorners[i] = corner;
            }

            var lightSpaceFrustumAABB = new AABBf(frustumCorners[0].xyz, frustumCorners[0].xyz);
            foreach (var p in frustumCorners)
            {
                lightSpaceFrustumAABB |= p.xyz;
            }

            clipPlanes = new float2(lightSpaceFrustumAABB.min.z, lightSpaceFrustumAABB.min.z + lightSpaceFrustumAABB.Size.z);
            return float4x4.CreateOrthographic(lightSpaceFrustumAABB.Size.x, lightSpaceFrustumAABB.Size.y, lightSpaceFrustumAABB.min.z, lightSpaceFrustumAABB.min.z + lightSpaceFrustumAABB.Size.z);
        }

        private ShadowParams CreateShadowParams(LightResult lr, TexRes shadowMapRes, Tuple<SceneNodeContainer, LightComponent> key)
        {
            float3 lightPos;
            float4x4 lightView;
            float4x4 lightProjection;
            
            float4x4[] lightSpaceMatrices;
            if (lr.Light.Type != LightType.Point)
                lightSpaceMatrices = new float4x4[1];
            else
                lightSpaceMatrices = new float4x4[6];

            float zNear;
            float zFar;

            switch (lr.Light.Type)
            {
                case LightType.Parallel:                
                    // TODO: implement cascaded shadow maps.
                    lightPos = lr.WorldSpacePos;                                               
                    lightView = float4x4.LookAt(lightPos, lightPos + float3.Normalize(lr.Rotation * float3.UnitZ), lr.Rotation * float3.UnitY);                    
                    lightProjection = GetProjectionForParallelLight(lightView, out var clipPlanes);
                    zNear = clipPlanes.x;
                    zFar = clipPlanes.y;
                    lightSpaceMatrices[0] = lightProjection * lightView;
                    break;
                case LightType.Point:
                    //TODO: implement cube shadow maps
                    lightPos = lr.WorldSpacePos;
                    zNear = 1f;
                    zFar = zNear + lr.Light.MaxDistance;

                    lightProjection = float4x4.CreatePerspectiveFieldOfView(M.DegreesToRadians(90), 1f, zNear, zFar);

                    lightView = float4x4.LookAt(lightPos, lightPos + float3.UnitX, float3.UnitY); //right face
                    lightSpaceMatrices[1] = lightProjection * lightView;
                    
                    lightView = float4x4.LookAt(lightPos, lightPos - float3.UnitX, float3.UnitY); //left face
                    lightSpaceMatrices[0] = lightProjection * lightView;
                    
                    lightView = float4x4.LookAt(lightPos, lightPos + float3.UnitY, float3.UnitZ); //upper face
                    lightSpaceMatrices[3] = lightProjection * lightView;
                    
                    lightView = float4x4.LookAt(lightPos, lightPos -float3.UnitY, -float3.UnitZ); //lower face
                    lightSpaceMatrices[2] = lightProjection * lightView;
                    
                    lightView = float4x4.LookAt(lightPos, lightPos + float3.UnitZ, float3.UnitY); //back face
                    lightSpaceMatrices[5] = lightProjection * lightView;
                    
                    lightView = float4x4.LookAt(lightPos, lightPos  -float3.UnitZ, float3.UnitY); //front face
                    lightSpaceMatrices[4] = lightProjection * lightView;
                    break;
                case LightType.Spot:
                    lightPos = lr.WorldSpacePos;
                    zNear = 0.1f;
                    zFar = 0.1f + lr.Light.MaxDistance;
                    lightProjection = float4x4.CreatePerspectiveFieldOfView(M.DegreesToRadians(lr.Light.OuterConeAngle)*2, 1f, zNear, zFar);
                    lightView = float4x4.LookAt(lightPos, lightPos + float3.Normalize(lr.Rotation * float3.UnitZ), lr.Rotation * float3.UnitY);
                    lightSpaceMatrices[0] = lightProjection * lightView;
                    break;
                case LightType.Legacy:                    
                    lightView = _rc.View;
                    lightProjection = GetProjectionForParallelLight(lightView, out var clipPlanesLegacy);
                    zNear = clipPlanesLegacy.x;
                    zFar = clipPlanesLegacy.y;
                    lightSpaceMatrices[0] = lightProjection * lightView;
                    break;
                default:
                    throw new ArgumentException("No Light Space Matrix created, light type not supported!");
            }

            if (!_shadowRenderTargets.TryGetValue(key, out ShadowParams outParams))
            {
                if (lr.Light.Type == LightType.Point)
                {
                    var shadowMap = new WritableCubeMap(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), (int)ShadowMapRes, (int)ShadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_EDGE);
                    outParams = new ShadowParams() { ClipPlanesForLightMat = new float2(zNear, zFar), LightSpaceMats = lightSpaceMatrices, CubeShadowMap = shadowMap };
                }
                else
                {
                    var shadowMap = new WritableTexture(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), (int)ShadowMapRes, (int)ShadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER);
                    outParams = new ShadowParams() { ClipPlanesForLightMat = new float2(zNear, zFar), LightSpaceMats = lightSpaceMatrices, ShadowMap = shadowMap };
                }
               
                _shadowRenderTargets.Add(key, outParams);
            }
            else
            {
                outParams.LightSpaceMats = lightSpaceMatrices;
                outParams.ClipPlanesForLightMat = new float2(zNear, zFar);
            }

            return outParams;
        }

        private void UpdateLightAndShadowParams(Tuple<SceneNodeContainer, LightResult> lightVisRes, ShaderEffect effect)
        {
            var lightRes = lightVisRes.Item2;
            var light = lightRes.Light;

            var dirWorldSpace = float3.Normalize((lightRes.Rotation * float4.UnitZ).xyz);
            var dirViewSpace = float3.Normalize((_rc.View * new float4(dirWorldSpace)).xyz);
            var strength = light.Strength;

            if (strength > 1.0 || strength < 0.0)
            {
                strength = M.Clamp(light.Strength, 0.0f, 1.0f);
                Diagnostics.Log("WARNING: strength of the light will be clamped between 0 and 1.");
            }

            // Set params in modelview space since the lightning calculation is in modelview space
            effect.SetEffectParam("light.position", _rc.View * lightRes.WorldSpacePos);
            effect.SetEffectParam("light.positionWorldSpace", lightRes.WorldSpacePos);
            effect.SetEffectParam("light.intensities", light.Color);
            effect.SetEffectParam("light.maxDistance", light.MaxDistance);
            effect.SetEffectParam("light.strength", strength);
            effect.SetEffectParam("light.outerConeAngle", M.DegreesToRadians(light.OuterConeAngle));
            effect.SetEffectParam("light.innerConeAngle", M.DegreesToRadians(light.InnerConeAngle));
            effect.SetEffectParam("light.direction", dirViewSpace);
            effect.SetEffectParam("light.directionWorldSpace", dirWorldSpace);
            effect.SetEffectParam("light.lightType", (int)light.Type);
            effect.SetEffectParam("light.isActive", light.Active ? 1 : 0);
            effect.SetEffectParam("light.isCastingShadows", light.IsCastingShadows ? 1 : 0);

            if (light.IsCastingShadows)
            {
                var shadowParams = _shadowRenderTargets[new Tuple<SceneNodeContainer, LightComponent>(lightVisRes.Item1, lightVisRes.Item2.Light)];
                if (shadowParams.ShadowMap != null)
                {
                    effect.SetEffectParam("ShadowMap", shadowParams.ShadowMap);
                    effect.SetEffectParam("LightSpaceMatrix", shadowParams.LightSpaceMats[0]);
                }
                else
                {
                    if (light.Type == LightType.Point && shadowParams.CubeShadowMap != null)
                        effect.SetEffectParam("ShadowCubeMap", shadowParams.CubeShadowMap);
                    else
                        effect.SetEffectParam("ShadowCubeMap", null);
                }
            }
        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="rc">The <see cref="RenderContext"/>.</param>        
        public void Render(RenderContext rc)
        {  
            SetContext(rc);
            AccumulateLight();
            _rc.EnableDepthClamp();

            if (!_isBackgroundColorSet)
            {
                BackgroundColor = _rc.ClearColor;
                _isBackgroundColorSet = true;
            }

            _rc.ClearColor = _texClearColor;

            //TODO: if platform != Desktop ignore point lights - because a geometry shader is used to create the (cube) shadow map in one pass.
            //Create shadow textures in GBuffer RenderTarget and one RenderTarget for each shadow map.
            _rc.Viewport(0, 0, (int)ShadowMapRes, (int)ShadowMapRes);
            _currentPass = DeferredPasses.SHADOW;
            _rc.SetShaderEffect(_shadowEffect);
            foreach (var lightVisRes in LightViseratorResults)
            {
                if (!lightVisRes.Item2.Light.IsCastingShadows || !lightVisRes.Item2.Light.Active) continue;

                var key = new Tuple<SceneNodeContainer, LightComponent>(lightVisRes.Item1, lightVisRes.Item2.Light);
                var shadowParams = CreateShadowParams(lightVisRes.Item2, ShadowMapRes, key);

                if (lightVisRes.Item2.Light.Type == LightType.Point)
                {
                    if (_shadowCubeMapEffect == null)
                        _shadowCubeMapEffect = ShaderCodeBuilder.ShadowCubeMapEffect(shadowParams.LightSpaceMats);
                    else                    
                        _shadowCubeMapEffect.SetEffectParam($"LightSpaceMatrices[0]", shadowParams.LightSpaceMats);                    

                    _shadowCubeMapEffect.SetEffectParam("LightMatClipPlanes", shadowParams.ClipPlanesForLightMat);
                    _shadowCubeMapEffect.SetEffectParam("LightPos", lightVisRes.Item2.WorldSpacePos);
                    _rc.SetShaderEffect(_shadowCubeMapEffect);

                    rc.SetRenderTarget(shadowParams.CubeShadowMap);
                }
                else
                {
                    _shadowEffect.SetEffectParam("LightSpaceMatrix", shadowParams.LightSpaceMats[0]);
                    _shadowEffect.SetEffectParam("LightMatClipPlanes", _shadowRenderTargets[key].ClipPlanesForLightMat);
                    _shadowEffect.SetEffectParam("LightType", (int)lightVisRes.Item2.Light.Type);

                    rc.SetRenderTarget(shadowParams.ShadowMap);
                }
                
                Traverse(_sc.Children);
            }

            //Pass 1: Geometry pass
            _rc.Viewport(0, 0, (int)_gBufferRenderTarget.TextureResolution, (int)_gBufferRenderTarget.TextureResolution);
            _currentPass = DeferredPasses.GEOMETRY;
            rc.SetRenderTarget(_gBufferRenderTarget);
            Traverse(_sc.Children);

            //Pass 2: SSAO
            _currentPass = DeferredPasses.SSAO;
            if (_ssaoTexEffect == null)
                _ssaoTexEffect = ShaderCodeBuilder.SSAORenderTargetTextureEffect(_gBufferRenderTarget, 64, new float2((float)_texRes, (float)_texRes), new float2(_projectionComponent.ZNear, _projectionComponent.ZNear));
            _quadShaderEffectComp.Effect = _ssaoTexEffect;
            rc.SetRenderTarget(_ssaoRenderTexture);
            Traverse(_quadScene.Children);

            //Pass 3: Blur SSAO Texture
            _currentPass = DeferredPasses.SSAO_BLUR;
            if (_blurEffect == null)
                _blurEffect = ShaderCodeBuilder.SSAORenderTargetBlurEffect(_ssaoRenderTexture);
            _quadShaderEffectComp.Effect = _blurEffect;
            rc.SetRenderTarget(_blurRenderTarget);
            Traverse(_quadScene.Children);

            //Set blurred SSAO Texture as SSAO Texture in gBuffer
            _gBufferRenderTarget.SetTextureFromRenderTarget(_blurRenderTarget, RenderTargetTextureTypes.G_SSAO);

            _currentPass = DeferredPasses.LIGHTING;

            //Pass 4 & 5: FXAA and Lighting
            if (!FxaaOn)
            {
                _rc.Viewport(0, 0, _projectionComponent.Width, _projectionComponent.Height);
                _rc.ClearColor = float4.Zero;
                
                // ----FXAA OFF----               

                var lightPassCnt = 0;
                rc.SetRenderTarget();

                for (int i = 0; i < LightViseratorResults.Count; i++)
                {
                    var lightVisRes = LightViseratorResults[i];
                    if (!lightVisRes.Item2.Light.Active) continue;

                    if (lightVisRes.Item2.Light.IsCastingShadows)
                    {
                        var shadowParams = _shadowRenderTargets[new Tuple<SceneNodeContainer, LightComponent>(lightVisRes.Item1, lightVisRes.Item2.Light)];

                        if ((_lightingPassEffectOther == null || _needToRecreateLightingMatOther) && lightVisRes.Item2.Light.Type != LightType.Point)
                        {
                            _needToRecreateLightingMatOther = false;
                            _lightingPassEffectOther = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, (WritableTexture)shadowParams.ShadowMap);
                        }
                        if ((_lightingPassEffectPoint == null || _needToRecreateLightingMatPoint) && lightVisRes.Item2.Light.Type == LightType.Point)
                        {
                            _needToRecreateLightingMatPoint = false;
                            _lightingPassEffectPoint = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, (WritableCubeMap)shadowParams.CubeShadowMap);
                        }

                        if (lightVisRes.Item2.Light.Type != LightType.Point)
                            _lightingPassEffect = _lightingPassEffectOther;
                        else
                            _lightingPassEffect = _lightingPassEffectPoint;
                    }
                    else
                    {
                        if (_lightingPassEffectNoShadow == null || _needToRecreateLightingMatNoShadow)
                        {
                            _needToRecreateLightingMatNoShadow = false;
                            _lightingPassEffectNoShadow = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light);
                        }

                        _lightingPassEffect = _lightingPassEffectNoShadow;
                    }

                    UpdateLightAndShadowParams(lightVisRes, _lightingPassEffect);
                    _lightingPassEffect.SetEffectParam("PassNo", lightPassCnt);
                    _quadShaderEffectComp.Effect = _lightingPassEffect;
                    Traverse(_quadScene.Children);
                    lightPassCnt++;
                }
            }
            else
            {
                _rc.ClearColor = float4.Zero;
                // ---- FXAA ON ----

                //TODO: if platform != Desktop ignore point lights - because a geometry shader is used to create the (cube) shadow map in one pass.
                var lightPassCnt = 0;
                rc.SetRenderTarget(_lightedSceneRenderTarget);
                for (int i = 0; i < LightViseratorResults.Count; i++)
                {
                    var lightVisRes = LightViseratorResults[i];
                    if (!lightVisRes.Item2.Light.Active) continue;

                    if (lightVisRes.Item2.Light.IsCastingShadows)
                    {
                        var shadowParams = _shadowRenderTargets[new Tuple<SceneNodeContainer, LightComponent>(lightVisRes.Item1, lightVisRes.Item2.Light)];

                        if ((_lightingPassEffectOther == null || _needToRecreateLightingMatOther) && lightVisRes.Item2.Light.Type != LightType.Point)
                        {
                            _needToRecreateLightingMatOther = false;
                            _lightingPassEffectOther = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, (WritableTexture)shadowParams.ShadowMap);
                        }
                        if ((_lightingPassEffectPoint == null || _needToRecreateLightingMatPoint) && lightVisRes.Item2.Light.Type == LightType.Point)
                        {
                            _needToRecreateLightingMatPoint = false;
                            _lightingPassEffectPoint = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, (WritableCubeMap)shadowParams.CubeShadowMap);
                        }

                        if (lightVisRes.Item2.Light.Type != LightType.Point)
                            _lightingPassEffect = _lightingPassEffectOther;
                        else
                            _lightingPassEffect = _lightingPassEffectPoint;
                    }
                    else
                    {
                        if(_lightingPassEffectNoShadow == null || _needToRecreateLightingMatNoShadow)
                        {
                            _needToRecreateLightingMatNoShadow = false;
                            _lightingPassEffectNoShadow = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light);
                        }

                        _lightingPassEffect = _lightingPassEffectNoShadow;
                    }

                    UpdateLightAndShadowParams(lightVisRes, _lightingPassEffect);
                    _lightingPassEffect.SetEffectParam("PassNo", lightPassCnt);
                    _quadShaderEffectComp.Effect = _lightingPassEffect;
                    Traverse(_quadScene.Children);
                    lightPassCnt++;
                }

                _currentPass = DeferredPasses.FXAA;
                _rc.Viewport(0, 0, _projectionComponent.Width, _projectionComponent.Height);
                if (_fxaaEffect == null)
                    _fxaaEffect = ShaderCodeBuilder.FXAARenderTargetEffect(_lightedSceneRenderTarget, new float2((float)_texRes, (float)_texRes));
                _quadShaderEffectComp.Effect = _fxaaEffect;
                rc.SetRenderTarget();
                Traverse(_quadScene.Children);
            }
        }

        private void DeleteBuffers(object sender, EventArgs e)
        {
            var rt = (RenderTarget)sender;
            if (rt.GBufferHandle != null)
                _rc.DeleteFrameBuffer(rt.GBufferHandle);
            if (rt.DepthBufferHandle != null)
                _rc.DeleteRenderBuffer(rt.DepthBufferHandle);
        }
    }
}
