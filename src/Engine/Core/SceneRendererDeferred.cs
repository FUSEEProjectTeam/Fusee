using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System;
using System.Collections.Generic;

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
        internal class ShadowParams
        {
            // For omni directional shadow mapping there will be six LightSpaceMatrices
            // to allow the creation of the cube map in one pass.
            public float4x4[] LightSpaceMats;
            public IWritableTexture ShadowMap;
            public float2 ClipPlanesForLightMat;
        }

        /// <summary>
        /// Sets the GL.ClearColor
        /// </summary>
        public float4 BackgroundColor { get; private set; }

        /// <summary>
        /// Sets the Shadow Map resolution.
        /// </summary>
        public TexRes ShadowMapRes = TexRes.MID_RES;
        private TexRes _shadowMapRes = TexRes.MID_RES;

        /// <summary>
        /// Sets the G-Buffer texture resolution.
        /// </summary>
        public TexRes TexRes = TexRes.MID_RES;
        private TexRes _texRes = TexRes.MID_RES;

        /// <summary>
        /// Determines if the scene gets rendered with Fast Approximate Anti Aliasing.
        /// </summary>
        public bool FxaaOn
        {
            get
            {
                return _fxaaOn;
            }
            set
            {
                _needToRecreateLightingMatOther = _needToRecreateLightingMatPoint = _needToRecreateLightingMatNoShadow = _needToSetSSAOTex = true;
                _fxaaOn = value;
            }
        }
        private bool _fxaaOn = true;

        /// <summary>
        /// Determines if the scene gets rendered with Screen Space Ambient Occlusion.
        /// </summary>
        public bool SsaoOn
        {
            get
            {
                return _ssaoOn;
            }
            set
            {
                if (value == false)
                    _rc.DetachTextureFromFbo(_gBufferRenderTarget, RenderTargetTextureTypes.G_SSAO);
                else
                    _rc.ReattachTextureFromFbo(_gBufferRenderTarget, RenderTargetTextureTypes.G_SSAO);
                _ssaoOn = value;
                _needToSetSSAOTex = true;
            }
        }
        private bool _ssaoOn = true;

        private bool _needToRecreateLightingMatOther = false;
        private bool _needToRecreateLightingMatPoint = false;
        private bool _needToRecreateLightingMatNoShadow = false;

        private bool _needToSetSSAOTex = false;

        private float4 _texClearColor = new float4(0, 0, 0, 0);
        private readonly SceneContainer _quadScene = new SceneContainer();
        private readonly ShaderEffectComponent _quadShaderEffectComp;

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

        private readonly Dictionary<Tuple<SceneNodeContainer, LightComponent>, ShadowParams> _shadowparams; //One per Light         
        private readonly ShaderEffect _shadowEffect;
        private ShaderEffect _shadowCubeMapEffect;

        private float4x4 _thisScenesProjection = float4x4.Identity;

        private bool _canUseGeometryShaders;

        /// <summary>
        /// Creates a new instance of type SceneRendererDeferred.
        /// </summary>
        /// <param name="sc">The SceneContainer, containing the scene that gets rendered.</param>       
        public SceneRendererDeferred(SceneContainer sc) : base(sc)
        {
            _gBufferRenderTarget = new RenderTarget(_texRes);
            _gBufferRenderTarget.SetPositionTex();
            _gBufferRenderTarget.SetAlbedoSpecularTex();
            _gBufferRenderTarget.SetNormalTex();
            _gBufferRenderTarget.SetDepthTex(TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, TextureCompareFunc.GL_LEQUAL);

            _ssaoRenderTexture = new WritableTexture(RenderTargetTextureTypes.G_SSAO, new ImagePixelFormat(ColorFormat.fRGB32), (int)_texRes, (int)_texRes, false, TextureFilterMode.NEAREST);

            _blurRenderTarget = new RenderTarget(_texRes);
            _blurRenderTarget.SetSSAOTex();

            _lightedSceneRenderTarget = new RenderTarget(_texRes);
            _lightedSceneRenderTarget.SetAlbedoSpecularTex();

            _ambientLightedSceneRenderTarget = new RenderTarget(_texRes);

            _shadowparams = new Dictionary<Tuple<SceneNodeContainer, LightComponent>, ShadowParams>();

            _gBufferRenderTarget.DeleteBuffers += DeleteBuffers;
            _blurRenderTarget.DeleteBuffers += DeleteBuffers;
            _lightedSceneRenderTarget.DeleteBuffers += DeleteBuffers;
            _ambientLightedSceneRenderTarget.DeleteBuffers += DeleteBuffers;

            _shadowEffect = ShaderCodeBuilder.ShadowMapEffect();

            // TODO: For deferred rendering we need a other ShaderEffect as we would in forward rendering. At the moment this conversion is done in the SceneRendererDeferred as a first step.
            // Can we avoid this? Maybe if we (implement) and use the upcoming "shader shard" system?
            // 
            // Current situation:
            // Case 1 Scene is loaded from fus file: if we'd know in the SceneConverter whether we render deferred or forward, we could create the correct ShaderEffect there.
            // Case 2 The user does create a scene programmatically: at the moment he needs to know that he has to use a GBufferTextureEffect for deferred rendering. In this case this additional traversal is obsolete.
            // When and how do we want to decide if we render forward or deferred?
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
                    renderTargetMat.Effect = ShaderCodeBuilder.GBufferTextureEffect(_gBufferRenderTarget, (float)mix, (Texture)tex);
                else
                    renderTargetMat.Effect = ShaderCodeBuilder.GBufferTextureEffect(_gBufferRenderTarget, 1f);

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
                            new ProjectionComponent(ProjectionMethod.PERSPECTIVE,1,1,1),

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
            base.RenderProjection(pc);

            if (_currentPass == DeferredPasses.GEOMETRY)
                _quadScene.Children[0].Components[0] = pc;

            _thisScenesProjection = _rc.Projection;
        }

        private float4[] GetWorldSpaceFrustumCorners()
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
                frustumCorners[i] = corner;
            }

            return frustumCorners;
        }

        //TODO: PROBLEM Without cascaded shadow maps and a large frustum we get a large AABB, possibly spanning the whole scene and therefore imprecise shadows.
        private float4x4 GetProjectionForParallelLight(float4x4 lightView, float4[] frustumCorners, out float2 clipPlanes)
        {
            for (int i = 0; i < frustumCorners.Length; i++)
            {
                var corner = frustumCorners[i];
                corner = lightView * corner; //light space frustum corners
                frustumCorners[i] = corner;
            }

            var lightSpaceFrustumAABB = new AABBf(frustumCorners[0].xyz, frustumCorners[0].xyz);
            foreach (var p in frustumCorners)
            {
                lightSpaceFrustumAABB |= p.xyz;
            }

            clipPlanes = new float2(lightSpaceFrustumAABB.min.z, lightSpaceFrustumAABB.min.z + lightSpaceFrustumAABB.Size.z);
            return float4x4.CreateOrthographic(lightSpaceFrustumAABB.Size.x, lightSpaceFrustumAABB.Size.y, lightSpaceFrustumAABB.min.z, lightSpaceFrustumAABB.max.z + lightSpaceFrustumAABB.min.z);
        }

        private ShadowParams CreateShadowParams(LightResult lr, Tuple<SceneNodeContainer, LightComponent> key)
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

                    var n = System.Math.Abs(_thisScenesProjection.M34 / (_thisScenesProjection.M33 + 1.0f));
                    var f = System.Math.Abs(_thisScenesProjection.M34 / (_thisScenesProjection.M33 - 1.0f));

                    var frustumCorners = GetWorldSpaceFrustumCorners();

                    float3 frustumCenter = float3.Zero;

                    foreach (var corner in frustumCorners)
                        frustumCenter += corner.xyz;

                    frustumCenter /= 8;

                    var lightDir = float3.Normalize((lr.Rotation * float4.UnitZ).xyz);

                    lightPos = frustumCenter + (-lightDir * (f - n));//lr.WorldSpacePos;    
                    var target = lightPos + float3.Normalize(lr.Rotation * float3.UnitZ);
                    lightView = float4x4.LookAt(lightPos, target, float3.Normalize(lr.Rotation * float3.UnitY));
                    lightProjection = GetProjectionForParallelLight(lightView, frustumCorners, out var clipPlanes);
                    zNear = clipPlanes.x;
                    zFar = clipPlanes.y;
                    lightSpaceMatrices[0] = lightProjection * lightView;
                    break;

                case LightType.Point:
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

                    lightView = float4x4.LookAt(lightPos, lightPos - float3.UnitY, -float3.UnitZ); //lower face
                    lightSpaceMatrices[2] = lightProjection * lightView;

                    lightView = float4x4.LookAt(lightPos, lightPos + float3.UnitZ, float3.UnitY); //back face
                    lightSpaceMatrices[5] = lightProjection * lightView;

                    lightView = float4x4.LookAt(lightPos, lightPos - float3.UnitZ, float3.UnitY); //front face
                    lightSpaceMatrices[4] = lightProjection * lightView;
                    break;
                case LightType.Spot:
                    lightPos = lr.WorldSpacePos;
                    zNear = 0.1f;
                    zFar = 0.1f + lr.Light.MaxDistance;
                    lightProjection = float4x4.CreatePerspectiveFieldOfView(M.DegreesToRadians(lr.Light.OuterConeAngle) * 2, 1f, zNear, zFar);
                    lightView = float4x4.LookAt(lightPos, lightPos + float3.Normalize(lr.Rotation * float3.UnitZ), lr.Rotation * float3.UnitY);
                    lightSpaceMatrices[0] = lightProjection * lightView;
                    break;
                case LightType.Legacy:
                    lightView = _rc.View;
                    lightProjection = GetProjectionForParallelLight(lightView, GetWorldSpaceFrustumCorners(), out var clipPlanesLegacy); ;
                    zNear = clipPlanesLegacy.x;
                    zFar = clipPlanesLegacy.y;
                    lightSpaceMatrices[0] = lightProjection * lightView;
                    break;
                default:
                    throw new ArgumentException("No Light Space Matrix created, light type not supported!");
            }

            if (!_shadowparams.TryGetValue(key, out ShadowParams outParams))
            {
                if (lr.Light.Type == LightType.Point)
                {
                    var shadowMap = new WritableCubeMap(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), (int)_shadowMapRes, (int)_shadowMapRes, false, TextureFilterMode.LINEAR, TextureWrapMode.CLAMP_TO_EDGE);
                    outParams = new ShadowParams() { ClipPlanesForLightMat = new float2(zNear, zFar), LightSpaceMats = lightSpaceMatrices, ShadowMap = shadowMap };
                }
                else
                {
                    var shadowMap = new WritableTexture(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), (int)_shadowMapRes, (int)_shadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER, TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, TextureCompareFunc.GL_LESS);
                    outParams = new ShadowParams() { ClipPlanesForLightMat = new float2(zNear, zFar), LightSpaceMats = lightSpaceMatrices, ShadowMap = shadowMap };
                }

                _shadowparams.Add(key, outParams);
            }
            else
            {
                outParams.LightSpaceMats = lightSpaceMatrices;
                outParams.ClipPlanesForLightMat = new float2(zNear, zFar);
            }

            return outParams;
        }

        private void UpdateLightAndShadowParams(Tuple<SceneNodeContainer, LightResult> lightVisRes, ShaderEffect effect, bool isCastingShadows)
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
            effect.SetEffectParam("light.bias", light.Bias);

            if (isCastingShadows) //we don't use light.IsCastingShadows because we could need to skip the shadow calculation because of hardware capabilities.
            {                
                var shadowParams = _shadowparams[new Tuple<SceneNodeContainer, LightComponent>(lightVisRes.Item1, lightVisRes.Item2.Light)];

                if (lightVisRes.Item2.Light.Type != LightType.Point)
                {
                    effect.SetEffectParam("LightSpaceMatrix", shadowParams.LightSpaceMats[0]);
                    effect.SetEffectParam("ShadowMap", shadowParams.ShadowMap);
                }
                else
                    effect.SetEffectParam("ShadowCubeMap", shadowParams.ShadowMap);
            }
        }

        /// <summary>
        /// Renders one pass for each light and blends the results together. 
        /// Alternatively it would be possible to iterate the lights in the shader, but this would create a more complex shader. Additionally it would be more difficult to implementation a dynamic number of lights.
        /// The iteration here should not prove critical, due to the scene only consisting of one quad.
        /// </summary>
        private void RenderLightPasses()
        {
            var lightPassCnt = 0;
            
            for (int i = 0; i < LightViseratorResults.Count; i++)
            {
                var isCastingShadows = false;
                var lightVisRes = LightViseratorResults[i];

                if (!lightVisRes.Item2.Light.Active)  continue;

                if ((lightVisRes.Item2.Light.IsCastingShadows && lightVisRes.Item2.Light.Type != LightType.Point) || (lightVisRes.Item2.Light.IsCastingShadows && lightVisRes.Item2.Light.Type == LightType.Point && _canUseGeometryShaders))
                {
                    var shadowParams = _shadowparams[new Tuple<SceneNodeContainer, LightComponent>(lightVisRes.Item1, lightVisRes.Item2.Light)];

                    if ((_lightingPassEffectOther == null || _needToRecreateLightingMatOther) && lightVisRes.Item2.Light.Type != LightType.Point)
                    {
                        _needToRecreateLightingMatOther = false;
                        _lightingPassEffectOther = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, (WritableTexture)shadowParams.ShadowMap, _texClearColor);
                    }
                    if ((_lightingPassEffectPoint == null || _needToRecreateLightingMatPoint) && lightVisRes.Item2.Light.Type == LightType.Point)
                    {
                        _needToRecreateLightingMatPoint = false;
                        _lightingPassEffectPoint = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, (WritableCubeMap)shadowParams.ShadowMap, _texClearColor);
                    }

                    if (lightVisRes.Item2.Light.Type != LightType.Point)
                        _lightingPassEffect = _lightingPassEffectOther;
                    else
                        _lightingPassEffect = _lightingPassEffectPoint;

                    isCastingShadows = true;
                }
                else
                {
                    if (_lightingPassEffectNoShadow == null || _needToRecreateLightingMatNoShadow)
                    {
                        _needToRecreateLightingMatNoShadow = false;
                        _lightingPassEffectNoShadow = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, _texClearColor);
                    }

                    _lightingPassEffect = _lightingPassEffectNoShadow;
                }

                UpdateLightAndShadowParams(lightVisRes, _lightingPassEffect, isCastingShadows);
                _lightingPassEffect.SetEffectParam("PassNo", lightPassCnt);

                if (_needToSetSSAOTex)
                {
                    _lightingPassEffect.SetEffectParam("SsaoOn", _ssaoOn ? 1 : 0);
                    _needToSetSSAOTex = false;
                }

                //Set background color only in last light pass to NOT blend the color (additive).
                if (i == LightViseratorResults.Count - 1)
                    _lightingPassEffect.SetEffectParam("BackgroundColor", BackgroundColor);
                else                
                    _lightingPassEffect.SetEffectParam("BackgroundColor", _texClearColor);
                

                _quadShaderEffectComp.Effect = _lightingPassEffect;
                Traverse(_quadScene.Children);
                lightPassCnt++;
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

            _canUseGeometryShaders = _rc.GetHardwareCapabilities(HardwareCapability.CAN_USE_GEOMETRY_SHADERS) == 1U ? true : false;

            BackgroundColor = _rc.ClearColor;
            _rc.ClearColor = _texClearColor;

            var screenWidth = _rc.ViewportWidth;
            var screenHeight = _rc.ViewportHeight;
           
            //Create shadow textures in GBuffer RenderTarget and one RenderTarget for each shadow map.
            _rc.Viewport(0, 0, (int)_shadowMapRes, (int)_shadowMapRes, false);
            _currentPass = DeferredPasses.SHADOW;
            _rc.SetShaderEffect(_shadowEffect);
            foreach (var lightVisRes in LightViseratorResults)
            {
                if (!lightVisRes.Item2.Light.IsCastingShadows || !lightVisRes.Item2.Light.Active || (lightVisRes.Item2.Light.Type == LightType.Point && !_canUseGeometryShaders)) continue;

                var key = new Tuple<SceneNodeContainer, LightComponent>(lightVisRes.Item1, lightVisRes.Item2.Light);
                var shadowParams = CreateShadowParams(lightVisRes.Item2, key);

                if (lightVisRes.Item2.Light.Type == LightType.Point)
                {
                    if (_shadowCubeMapEffect == null)
                        _shadowCubeMapEffect = ShaderCodeBuilder.ShadowCubeMapEffect(shadowParams.LightSpaceMats);
                    else
                        _shadowCubeMapEffect.SetEffectParam($"LightSpaceMatrices[0]", shadowParams.LightSpaceMats);

                    _shadowCubeMapEffect.SetEffectParam("LightMatClipPlanes", shadowParams.ClipPlanesForLightMat);
                    _shadowCubeMapEffect.SetEffectParam("LightPos", lightVisRes.Item2.WorldSpacePos);
                    _rc.SetShaderEffect(_shadowCubeMapEffect);

                    rc.SetRenderTarget((IWritableCubeMap)shadowParams.ShadowMap);
                }
                else
                {
                    _shadowEffect.SetEffectParam("LightSpaceMatrix", shadowParams.LightSpaceMats[0]);
                    _shadowEffect.SetEffectParam("LightMatClipPlanes", _shadowparams[key].ClipPlanesForLightMat);
                    _shadowEffect.SetEffectParam("LightType", (int)lightVisRes.Item2.Light.Type);

                    rc.SetRenderTarget(shadowParams.ShadowMap);
                }

                Traverse(_sc.Children);
            }

            //Pass 1: Geometry pass
            _rc.Viewport(0, 0, (int)_gBufferRenderTarget.TextureResolution, (int)_gBufferRenderTarget.TextureResolution, false);
            _currentPass = DeferredPasses.GEOMETRY;
            rc.SetRenderTarget(_gBufferRenderTarget);
            Traverse(_sc.Children);

            if (_ssaoOn)
            {
                //Pass 2: SSAO
                _currentPass = DeferredPasses.SSAO;
                if (_ssaoTexEffect == null)
                    _ssaoTexEffect = ShaderCodeBuilder.SSAORenderTargetTextureEffect(_gBufferRenderTarget, 64, new float2((float)_texRes, (float)_texRes));
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
            }

            _currentPass = DeferredPasses.LIGHTING;

            //Pass 4 & 5: FXAA and Lighting
            if (!FxaaOn)
            {
                _rc.Viewport(0, 0, screenWidth, screenHeight);

                // ----FXAA OFF----
                rc.SetRenderTarget();
                RenderLightPasses();
            }
            else
            {
                // ---- FXAA ON ----
                rc.SetRenderTarget(_lightedSceneRenderTarget);
                RenderLightPasses();

                _currentPass = DeferredPasses.FXAA;
                _rc.Viewport(0, 0, screenWidth, screenHeight);
                if (_fxaaEffect == null)
                    _fxaaEffect = ShaderCodeBuilder.FXAARenderTargetEffect(_lightedSceneRenderTarget, new float2((float)_texRes, (float)_texRes));
                _quadShaderEffectComp.Effect = _fxaaEffect;

                rc.SetRenderTarget();

                Traverse(_quadScene.Children);
            }

            _texRes = TexRes;
            _shadowMapRes = ShadowMapRes;
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
