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
    /// This particular SceneRenderer uses deferred rendering.
    /// </summary>
    public class SceneRendererDeferred : SceneRendererForward
    {
        internal class ShadowParams
        {
            // For omni directional shadow mapping there will be six LightSpaceMatrices
            // to allow the creation of the cube map in one pass.
            public float4x4[] LightSpaceMatrices;
            public IWritableTexture[] ShadowMaps;
            public float2[] ClipPlanesForLightMat;
        }

        /// <summary>
        /// Sets the GL.ClearColor
        /// </summary>
        public float4 BackgroundColor { get; private set; }

        /// <summary>
        /// Sets the Shadow Map resolution.
        /// </summary>
        public TexRes ShadowMapRes = TexRes.MID_RES;

        /// <summary>
        /// Sets the G-Buffer texture resolution.
        /// </summary>
        public TexRes TexRes = TexRes.MID_RES;

        /// <summary>
        /// Determines if the scene gets rendered with Fast Approximate Anti Aliasing.
        /// </summary>
        public bool FxaaOn { get; set; } = true;

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

        private bool _needToSetSSAOTex = false;

        private float4 _texClearColor = new float4(0, 0, 0, 0);
        private readonly SceneContainer _quadScene = new SceneContainer();
        private readonly ShaderEffectComponent _quadShaderEffectComp;

        private ShaderEffect _ssaoTexEffect;
        private ShaderEffect _lightingPassEffect;
        private ShaderEffect _blurEffect;
        private ShaderEffect _fxaaEffect;
        private readonly ShaderEffect _shadowEffect;
        private ShaderEffect _shadowCubeMapEffect;

        //The following ShaderEffects cache all possible ShaderEffects, needed to render the lighting passes.
        private ShaderEffect _lightingPassEffectPoint; //needed when a point light is rendered;
        private ShaderEffect _lightingPassEffectOther; //needed when a light of another type is rendered;
        private ShaderEffect _lightingPassEffectNoShadow; //needed when a light of another type is rendered without shadows;              

        private readonly RenderTarget _gBufferRenderTarget;
        private readonly RenderTarget _blurRenderTarget;
        private readonly RenderTarget _ambientLightedSceneRenderTarget;
        private readonly RenderTarget _lightedSceneRenderTarget;

        //This texture caches the SSAO texture object when detaching it from the fbo on the gpu in order to turn SAO off at runtime.
        private readonly WritableTexture _ssaoRenderTexture;

        private readonly Dictionary<Tuple<SceneNodeContainer, LightComponent>, ShadowParams> _shadowparams; //One per Light         

        private float4x4 _thisScenesProjectionMat = float4x4.Identity;
        private ProjectionComponent _thisScenesProjection;

        private DeferredPasses _currentPass;

        //_shadowMapRes is set to ShadowMapRes at the end of each Render call to allow resolution changes at runtime.
        private TexRes _shadowMapRes = TexRes.MID_RES;
        //_texRes is set to TexRes at the end of each Render call to allow resolution changes at runtime.
        private TexRes _texRes = TexRes.MID_RES;

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

        # region Deferred specific visitors

        /// <summary>
        /// If a ShaderEffectComponent is visited the ShaderEffect of the <see cref="RendererState"/> is updated and the effect is set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="shaderComponent">The ShaderEffectComponent</param>
        [VisitMethod]
        public new void RenderShaderEffect(ShaderEffectComponent shaderComponent)
        {
            if (HasNumberOfLightsChanged)
                HasNumberOfLightsChanged = false;

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

            _thisScenesProjectionMat = _rc.Projection;
            _thisScenesProjection = pc;
        }

        #endregion

        #region Shadow mapping

        private IEnumerable<float> GetClippingPlanesOfSplitFrustums(float zNear, float zFar, int numberOfSplits, float lambda = 0.5f)
        {
            for (int i = 0; i < numberOfSplits + 1; i++)
            {
                var splitOverNoOfSplits = i / (float)numberOfSplits;
                yield return SplitClipPlane(zNear, zFar, splitOverNoOfSplits, lambda);
            }
        }

        private float SplitClipPlaneUniform(float zNear, float zFar, float splitOverNoOfSplits)
        {
            return zNear + (zFar - zNear) * splitOverNoOfSplits;
        }

        private float SplitClipPlaneLog(float zNear, float zFar, float splitOverNoOfSplits)
        {
            return zNear * (float)System.Math.Pow((zFar / zNear), splitOverNoOfSplits);
        }

        //0 > lambda > 1
        private float SplitClipPlane(float zNear, float zFar, float splitOverNoOfSplits, float lambda)
        {
            return lambda * SplitClipPlaneLog(zNear, zFar, splitOverNoOfSplits) + (1 - lambda) * SplitClipPlaneUniform(zNear, zFar, splitOverNoOfSplits);
        }

        private AABBf FrustumAABBLightSpace(float4x4 lightView, float4[] frustumCorners)
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

            return lightSpaceFrustumAABB;
        }

        private float4[] GetWorldSpaceFrustumCorners(float4x4 projectionMatrix)
        {
            //1. Calculate the 8 corners of the view frustum in world space. This can be done by using the inverse view-projection matrix to transform the 8 corners of the NDC cube (which in OpenGL is [‒1, 1] along each axis).
            //2. Transform the frustum corners to a space aligned with the shadow map axes.This would commonly be the directional light object's local space. 
            //In fact, steps 1 and 2 can be done in one step by combining the inverse view-projection matrix of the camera with the inverse world matrix of the light.
            var invViewProjection = float4x4.Invert(projectionMatrix * _rc.View);

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

        private IEnumerable<float4x4> AllFrustumSplitLightProjectionMatrices(int numberOfSplits)
        {
            var clipPlanes = GetClippingPlanesOfSplitFrustums(_thisScenesProjection.ZNear, _thisScenesProjection.ZFar, numberOfSplits).ToList();

            for (int i = 0; i < clipPlanes.Count - 1; i++)
            {
                var zNear = clipPlanes[i];
                var zFar = clipPlanes[i + 1];
                var fov = _thisScenesProjection.Fov;
                var aspect = _thisScenesProjection.Width / _thisScenesProjection.Height;
                yield return float4x4.CreatePerspectiveFieldOfView(fov, aspect, zNear, zFar);
            }
        }

        private IEnumerable<float4[]> AllFrustumSplitCornersWorldSpace(int numberOfSplits)
        {
            var allSplitProjectionMatrices = AllFrustumSplitLightProjectionMatrices(numberOfSplits).ToList();

            for (int i = 0; i < allSplitProjectionMatrices.Count; i++)
            {
                yield return GetWorldSpaceFrustumCorners(allSplitProjectionMatrices[i]);
            }
        }

        private IEnumerable<AABBf> AllFrustumSplitAABBsLightSpace(int numberOfSplits, float4x4 lightView)
        {
            var frustumCorners = AllFrustumSplitCornersWorldSpace(numberOfSplits).ToList();
            for (int i = 0; i < frustumCorners.Count; i++)
            {
                yield return FrustumAABBLightSpace(lightView, frustumCorners[i]);
            }
        }

        private float4x4 CropMatrix(AABBf lightSpaceAABB)
        {
            var scaleX = 2 / (lightSpaceAABB.max.x - lightSpaceAABB.min.x);
            var scaleY = 2 / (lightSpaceAABB.max.y - lightSpaceAABB.min.y);

            var offsetX = -0.5f * (lightSpaceAABB.max.x + lightSpaceAABB.min.x) * scaleX;
            var offsetY = -0.5f * (lightSpaceAABB.max.y + lightSpaceAABB.min.y) * scaleY;

            var scaleZ = 1.0f / (lightSpaceAABB.max.z - lightSpaceAABB.min.z);
            var offsetZ = lightSpaceAABB.min.z * scaleZ;

            return new float4x4(scaleX, 0, 0, offsetX,
                                0, scaleY, 0, offsetY,
                                0, 0, scaleZ, offsetZ,
                                0, 0, 0, 1);
        }

        //TODO: PROBLEM Without cascaded shadow maps and a large frustum we get a large AABB, possibly spanning the whole scene and therefore imprecise shadows.
        private float4x4 GetProjectionForParallelLight(AABBf frustumAABBLightSpace, out float2 clipPlanes)
        {
            clipPlanes = new float2(frustumAABBLightSpace.min.z, frustumAABBLightSpace.min.z + frustumAABBLightSpace.Size.z);
            return float4x4.CreateOrthographic(frustumAABBLightSpace.Size.x, frustumAABBLightSpace.Size.y, frustumAABBLightSpace.min.z, frustumAABBLightSpace.max.z + frustumAABBLightSpace.min.z);
        }

        private ShadowParams CreateShadowParams(LightResult lr, Tuple<SceneNodeContainer, LightComponent> key)
        {
            float3 lightPos;
            float4x4 lightView;
            float4x4 lightProjection;

            float4x4[] lightSpaceMatrices;

            const int numberOfSplitFrustums = 4;

            if (lr.Light.Type == LightType.Parallel)
                lightSpaceMatrices = new float4x4[numberOfSplitFrustums];
            else if (lr.Light.Type != LightType.Point)
                lightSpaceMatrices = new float4x4[1];
            else
                lightSpaceMatrices = new float4x4[6];

            float zNear;
            float zFar;

            switch (lr.Light.Type)
            {
                case LightType.Parallel:

                    var frustumCorners = GetWorldSpaceFrustumCorners(_thisScenesProjectionMat);

                    lightPos = lr.WorldSpacePos;
                    var target = lightPos + float3.Normalize(lr.Rotation * float3.UnitZ);
                    lightView = float4x4.LookAt(lightPos, target, float3.Normalize(lr.Rotation * float3.UnitY));

                    //see: https://developer.nvidia.com/gpugems/GPUGems3/gpugems3_ch10.html and https://developer.download.nvidia.com/SDK/10.5/opengl/src/cascaded_shadow_maps/doc/cascaded_shadow_maps.pdf
                    if (_thisScenesProjection != null)
                    {
                        var allSplitFrustumAABBs = AllFrustumSplitAABBsLightSpace(numberOfSplitFrustums, lightView).ToList();
                        for (int i = 0; i < allSplitFrustumAABBs.Count; i++)
                        {
                            var aabb = allSplitFrustumAABBs[i];
                            //aabb.min.z = 0f;
                            var mat = CropMatrix(aabb);
                        }
                    }

                    lightProjection = GetProjectionForParallelLight(FrustumAABBLightSpace(lightView, frustumCorners), out var clipPlanes);
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
                    lightProjection = GetProjectionForParallelLight(FrustumAABBLightSpace(lightView, GetWorldSpaceFrustumCorners(_thisScenesProjectionMat)), out var clipPlanesLegacy); ;
                    zNear = clipPlanesLegacy.x;
                    zFar = clipPlanesLegacy.y;
                    lightSpaceMatrices[0] = lightProjection * lightView;
                    break;
                default:
                    throw new ArgumentException("No Light Space Matrix created, light type not supported!");
            }

            if (!_shadowparams.TryGetValue(key, out ShadowParams outParams))
            {

                switch (lr.Light.Type)
                {
                    case LightType.Point:
                        {
                            var shadowMap = new WritableCubeMap(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), (int)_shadowMapRes, (int)_shadowMapRes, false, TextureFilterMode.LINEAR, TextureWrapMode.CLAMP_TO_EDGE);
                            outParams = new ShadowParams() { ClipPlanesForLightMat = new float2[] { new float2(zNear, zFar) }, LightSpaceMatrices = lightSpaceMatrices, ShadowMaps = new IWritableTexture[1] { shadowMap } };
                            break;
                        }
                    case LightType.Parallel:
                    case LightType.Spot:
                    case LightType.Legacy:
                        {
                            var shadowMap = new WritableTexture(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), (int)_shadowMapRes, (int)_shadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER, TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, TextureCompareFunc.GL_LESS);
                            outParams = new ShadowParams() { ClipPlanesForLightMat = new float2[] { new float2(zNear, zFar) }, LightSpaceMatrices = lightSpaceMatrices, ShadowMaps = new IWritableTexture[1] { shadowMap } };
                            break;
                        }
                    default:
                        throw new ArgumentException("Invalid light type");
                }

                _shadowparams.Add(key, outParams);
            }
            else
            {
                outParams.LightSpaceMatrices = lightSpaceMatrices;
                outParams.ClipPlanesForLightMat = new float2[] { new float2(zNear, zFar) };
            }

            return outParams;
        }


        #endregion

        #region Rendering

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

            //Shadow Map Passes: Renders the scene for each light that is casting shadows and creates the shadow map for it.           
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
                        _shadowCubeMapEffect = ShaderCodeBuilder.ShadowCubeMapEffect(shadowParams.LightSpaceMatrices);
                    else
                        _shadowCubeMapEffect.SetEffectParam($"LightSpaceMatrices[0]", shadowParams.LightSpaceMatrices);

                    _shadowCubeMapEffect.SetEffectParam("LightMatClipPlanes", shadowParams.ClipPlanesForLightMat[0]);
                    _shadowCubeMapEffect.SetEffectParam("LightPos", lightVisRes.Item2.WorldSpacePos);
                    _rc.SetShaderEffect(_shadowCubeMapEffect);

                    rc.SetRenderTarget((IWritableCubeMap)shadowParams.ShadowMaps[0]);
                }
                else
                {
                    _shadowEffect.SetEffectParam("LightSpaceMatrix", shadowParams.LightSpaceMatrices[0]);                    
                    _shadowEffect.SetEffectParam("LightType", (int)lightVisRes.Item2.Light.Type);

                    rc.SetRenderTarget(shadowParams.ShadowMaps[0]);
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

                rc.SetRenderTarget();
                RenderLightPasses();
            }
            else
            {
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

        /// <summary>
        /// Renders one (lighting calculation) pass for each light and blends the results together. 
        /// Alternatively it would be possible to iterate the lights in the shader, but this would create a more complex shader. Additionally it would be more difficult to implement a dynamic number of lights.
        /// The iteration here should not prove critical, due to the scene only consisting of a single quad.
        /// </summary>
        private void RenderLightPasses()
        {
            var lightPassCnt = 0;

            for (int i = 0; i < LightViseratorResults.Count; i++)
            {
                var isCastingShadows = false;
                var lightVisRes = LightViseratorResults[i];

                if (!lightVisRes.Item2.Light.Active) continue;

                if ((lightVisRes.Item2.Light.IsCastingShadows && lightVisRes.Item2.Light.Type != LightType.Point) || (lightVisRes.Item2.Light.IsCastingShadows && lightVisRes.Item2.Light.Type == LightType.Point && _canUseGeometryShaders))
                {
                    var shadowParams = _shadowparams[new Tuple<SceneNodeContainer, LightComponent>(lightVisRes.Item1, lightVisRes.Item2.Light)];

                    if ((_lightingPassEffectOther == null) && lightVisRes.Item2.Light.Type != LightType.Point)
                        _lightingPassEffectOther = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, (WritableTexture)shadowParams.ShadowMaps[0], _texClearColor);

                    if ((_lightingPassEffectPoint == null) && lightVisRes.Item2.Light.Type == LightType.Point)
                        _lightingPassEffectPoint = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, (WritableCubeMap)shadowParams.ShadowMaps[0], _texClearColor);

                    if (lightVisRes.Item2.Light.Type != LightType.Point)
                        _lightingPassEffect = _lightingPassEffectOther;
                    else
                        _lightingPassEffect = _lightingPassEffectPoint;

                    isCastingShadows = true;
                }
                else
                {
                    if (_lightingPassEffectNoShadow == null)
                        _lightingPassEffectNoShadow = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, _texClearColor);

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

                switch (lightVisRes.Item2.Light.Type)
                {
                    case LightType.Point:
                        effect.SetEffectParam("ShadowCubeMap", (WritableCubeMap)shadowParams.ShadowMaps[0]);
                        break;
                    case LightType.Parallel:
                    case LightType.Spot:
                    case LightType.Legacy:
                        effect.SetEffectParam("LightSpaceMatrix", shadowParams.LightSpaceMatrices[0]);
                        effect.SetEffectParam("ShadowMap", (WritableTexture)shadowParams.ShadowMaps[0]);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

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
