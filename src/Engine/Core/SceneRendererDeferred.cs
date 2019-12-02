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
    /// <summary>
    /// Use a Scene Renderer to traverse a scene hierarchy (made out of scene nodes and components) in order
    /// to have each visited element contribute to the result rendered against a given render context.
    /// This particular SceneRenderer uses deferred rendering.
    /// </summary>
    public class SceneRendererDeferred : SceneRendererForward
    {
        /// <summary>
        /// Sets the GL.ClearColor
        /// </summary>
        public float4 BackgroundColor { get; private set; }

        /// <summary>
        /// Sets the Shadow Map resolution.
        /// </summary>
        public TexRes ShadowMapRes { get; private set; } = TexRes.MID_RES;

        /// <summary>
        /// Sets the G-Buffer texture resolution.
        /// </summary>
        public TexRes TexRes { get; private set; } = TexRes.MID_RES;

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

        /// <summary>
        /// This value is used for cascaded shadow mapping.
        /// The viewing frustum is split according to the Parallel Split Shadow Maps algorithm. 
        /// With this, the lambda value specifies a weight that adapts the logarithmic view frustum split according to the far planes of a uniform split.
        /// A value in the range [0;1] is expected. If it falls outside this range the value is clamped.
        /// </summary>
        public float PssmLambda = 0.4f;

        /// <summary>
        /// The number of shadow maps, generated when using cascaded shadow mapping for parallel lights.
        /// If set to 1 standard shadow mapping is used.
        /// </summary>
        public int NumberOfCascades = 3;

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
        private ShaderEffect _lightingPassEffectCascaded; //needed when a parallel light is rendered with cascaded shadow mapping;           

        private readonly RenderTarget _gBufferRenderTarget;

        //This texture caches the SSAO texture object when detaching it from the fbo on the gpu in order to turn SAO off at runtime.
        private readonly WritableTexture _ssaoRenderTexture;
        private readonly WritableTexture _blurRenderTex;
        private readonly WritableTexture _lightedSceneTex; //Do post-pro effects like FXAA on this texture.

        private readonly Dictionary<Tuple<SceneNodeContainer, LightComponent>, ShadowParams> _shadowparams; //One per Light       

        private RenderPasses _currentPass;
        private bool _canUseGeometryShaders;

        /// <summary>
        /// Creates a new instance of type SceneRendererDeferred.
        /// </summary>
        /// <param name="sc">The SceneContainer, containing the scene that gets rendered.</param>
        /// <param name="texRes">The g-buffer texture resolution.</param>
        /// <param name="shadowMapRes">The shadow map resolution.</param>       
        public SceneRendererDeferred(SceneContainer sc, TexRes texRes = TexRes.MID_RES, TexRes shadowMapRes = TexRes.MID_RES) : base(sc)
        {
            TexRes = texRes;
            ShadowMapRes = shadowMapRes;
            _gBufferRenderTarget = new RenderTarget(TexRes);
            _gBufferRenderTarget.SetPositionTex();
            _gBufferRenderTarget.SetAlbedoSpecularTex();
            _gBufferRenderTarget.SetNormalTex();
            _gBufferRenderTarget.SetDepthTex(TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, TextureCompareFunc.GL_LEQUAL);
            _gBufferRenderTarget.SetSpecularTex();

            _ssaoRenderTexture = new WritableTexture(RenderTargetTextureTypes.G_SSAO, new ImagePixelFormat(ColorFormat.fRGB32), (int)TexRes, (int)TexRes, false, TextureFilterMode.NEAREST);
            _blurRenderTex = new WritableTexture(RenderTargetTextureTypes.G_SSAO, new ImagePixelFormat(ColorFormat.fRGB32), (int)TexRes, (int)TexRes, false, TextureFilterMode.NEAREST);
            _lightedSceneTex = new WritableTexture(RenderTargetTextureTypes.G_ALBEDO, new ImagePixelFormat(ColorFormat.fRGB32), (int)TexRes, (int)TexRes, false, TextureFilterMode.LINEAR);

            _shadowparams = new Dictionary<Tuple<SceneNodeContainer, LightComponent>, ShadowParams>();

            _gBufferRenderTarget.DeleteBuffers += DeleteBuffers;

            _shadowEffect = ShaderCodeBuilder.ShadowMapEffect();

            //Pre-pass to build the geometry pass shader effect from
            var buildFrag = new ProtoToFrag(_sc, false);
            buildFrag.BuildFragmentShaders();

            _quadScene = new SceneContainer()
            {
                Children = new List<SceneNodeContainer>()
                {
                    new SceneNodeContainer()
                    {
                        Components = new List<SceneComponentContainer>()
                        {
                            new ShaderEffectComponent()
                            {
                                Effect = _lightingPassEffect
                            },
                            new Plane()

                        }
                    }
                }
            };

            _quadShaderEffectComp = (ShaderEffectComponent)_quadScene.Children[0].Components[0];
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

            if (_currentPass != RenderPasses.SHADOW)
            {
                _rc.SetShaderEffect(shaderComponent.Effect);
                _state.Effect = shaderComponent.Effect;
            }
        }

        /// <summary>
        /// If a Projection Component is visited, the projection matrix is set.
        /// </summary>
        /// <param name="cam">The visited <see cref="CameraComponent"/>.</param>
        [VisitMethod]
        public new void RenderCamera(CameraComponent cam)
        {
            if (cam.CustomCameraUpdate != null)
            {
                cam.CustomCameraUpdate(out float4x4 proj, out float4 viewport);

                _rc.Projection = proj;

                if (_currentPass == RenderPasses.LIGHTING)
                    _rc.Viewport((int)viewport.x, (int)viewport.y, (int)viewport.z, (int)viewport.w);

                return;
            }

            var startX = (int)(_rc.ViewportWidth * (cam.Viewport.x / 100));
            var startY = (int)(_rc.ViewportHeight * (cam.Viewport.y / 100));
            var width = (int)(_rc.ViewportWidth * (cam.Viewport.z / 100));
            var height = (int)(_rc.ViewportHeight * (cam.Viewport.w / 100));

            if (_currentPass == RenderPasses.LIGHTING)
                _rc.Viewport(startX, startY, width, height);

            switch (cam.ProjectionMethod)
            {
                default:
                case ProjectionMethod.PERSPECTIVE:
                    _rc.Projection = float4x4.CreatePerspectiveFieldOfView(cam.Fov, (float)width / height, cam.ClippingPlanes.x, cam.ClippingPlanes.y);
                    break;
                case ProjectionMethod.ORTHOGRAPHIC:
                    _rc.Projection = float4x4.CreateOrthographic(width, height, cam.ClippingPlanes.x, cam.ClippingPlanes.y);
                    break;
            }

            _rc.View = float4x4.Invert(_state.Model);

        }

        #endregion

        #region Shadow mapping

        private ShadowParams CreateShadowParams(LightResult lr, Tuple<SceneNodeContainer, LightComponent> key)
        {
            float4x4[] lightSpaceMatrices;
            var shadowParamClipPlanes = new float2[NumberOfCascades];

            //1. Calculate light space matrices and clip planes
            switch (lr.Light.Type)
            {
                case LightType.Legacy:
                case LightType.Parallel:
                    {
                        lightSpaceMatrices = new float4x4[NumberOfCascades];

                        var lightDir = float3.Normalize((lr.Rotation * float4.UnitZ).xyz);

                        var lightPos = lr.WorldSpacePos;
                        var target = lightPos + lightDir;
                        var lightView = float4x4.LookAt(lightPos, target, float3.Normalize(lr.Rotation * float3.UnitY));

                        //if (CurrentProjection != null)
                        //{
                        float tmpLambda;
                        if (PssmLambda > 1 || PssmLambda < 0)
                        {
                            tmpLambda = M.Clamp(PssmLambda, 0, 1);
                            Diagnostics.Warn("Lambda is > 1 or < 0 and is therefor camped between 0 and 1.");
                        }
                        else
                            tmpLambda = PssmLambda;

                        var zNear = _rc.DefaultState.ZNearDefautlt;
                        var zFar = _rc.DefaultState.ZFarDefault;
                        var fov = _rc.DefaultState.FovDefault;
                        var width = _rc.DefaultState.ViewportWidth;
                        var height = _rc.DefaultState.ViewportHeight;
                        var cascades = ShadowMapping.ParallelSplitCascades(NumberOfCascades, lightView, tmpLambda, zNear, zFar, width, height, fov, _rc.View).ToList();

                        if (cascades.Count <= 1)
                        {
                            var lightProjection = ShadowMapping.CreateOrthographic(cascades[0].Aabb);
                            shadowParamClipPlanes[0] = cascades[0].ClippingPlanes;
                            var completeFrustumLightMat = lightProjection * lightView;
                            lightSpaceMatrices[0] = completeFrustumLightMat;
                        }
                        else
                        {
                            for (int i = 0; i < cascades.Count; i++)
                            {
                                shadowParamClipPlanes[i] = cascades[i].ClippingPlanes;
                                var aabbLightSpace = cascades[i].Aabb;
                                lightSpaceMatrices[i] = ShadowMapping.CreateOrthographic(aabbLightSpace) * lightView;
                            }
                        }
                        //}

                        break;
                    }
                case LightType.Point:
                    {
                        lightSpaceMatrices = new float4x4[6];
                        var lightPos = lr.WorldSpacePos;
                        shadowParamClipPlanes = new float2[] { new float2(1, 1 + lr.Light.MaxDistance) };

                        var lightProjection = float4x4.CreatePerspectiveFieldOfView(M.DegreesToRadians(90), 1f, shadowParamClipPlanes[0].x, shadowParamClipPlanes[0].y);

                        float4x4 lightView;

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
                    }
                case LightType.Spot:
                    {
                        lightSpaceMatrices = new float4x4[1];
                        var lightPos = lr.WorldSpacePos;
                        shadowParamClipPlanes = new float2[] { new float2(0.1f, 0.1f + lr.Light.MaxDistance) };
                        var lightProjection = float4x4.CreatePerspectiveFieldOfView(M.DegreesToRadians(lr.Light.OuterConeAngle) * 2, 1f, shadowParamClipPlanes[0].x, shadowParamClipPlanes[0].y);
                        var lightView = float4x4.LookAt(lightPos, lightPos + float3.Normalize(lr.Rotation * float3.UnitZ), lr.Rotation * float3.UnitY);
                        lightSpaceMatrices[0] = lightProjection * lightView;
                        break;
                    }
                default:
                    throw new ArgumentException("No Light Space Matrix created, light type not supported!");
            }

            //2. If we haven't created the shadow params for this light yet, do so,
            if (!_shadowparams.TryGetValue(key, out ShadowParams outParams))
            {
                switch (lr.Light.Type)
                {
                    case LightType.Point:
                        {
                            var shadowMap = new WritableCubeMap(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), (int)ShadowMapRes, (int)ShadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER, TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, TextureCompareFunc.GL_LESS);
                            outParams = new ShadowParams() { ClipPlanesForLightMat = shadowParamClipPlanes, LightSpaceMatrices = lightSpaceMatrices, ShadowMaps = new IWritableTexture[1] { shadowMap } };
                            break;
                        }
                    case LightType.Legacy:
                    case LightType.Parallel:
                        {
                            var shadowMaps = new IWritableTexture[NumberOfCascades];
                            for (int i = 0; i < NumberOfCascades; i++)
                            {
                                var shadowMap = new WritableTexture(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), (int)ShadowMapRes, (int)ShadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER, TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, TextureCompareFunc.GL_LESS);
                                shadowMaps[i] = shadowMap;
                            }
                            outParams = new ShadowParams() { ClipPlanesForLightMat = shadowParamClipPlanes, LightSpaceMatrices = lightSpaceMatrices, ShadowMaps = shadowMaps };
                            break;
                        }
                    case LightType.Spot:

                        {
                            var shadowMap = new WritableTexture(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), (int)ShadowMapRes, (int)ShadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER, TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, TextureCompareFunc.GL_LESS);
                            outParams = new ShadowParams() { ClipPlanesForLightMat = shadowParamClipPlanes, LightSpaceMatrices = lightSpaceMatrices, ShadowMaps = new IWritableTexture[1] { shadowMap } };
                            break;
                        }
                    default:
                        throw new ArgumentException("Invalid light type.");
                }

                _shadowparams.Add(key, outParams);
            }
            //else update light space matrices and clip planes.
            else
            {
                outParams.LightSpaceMatrices = lightSpaceMatrices;
                outParams.ClipPlanesForLightMat = shadowParamClipPlanes;
            }

            return outParams;
        }

        #endregion

        #region Rendering

        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="rc">The <see cref="RenderContext"/>.</param>
        /// <param name="renderTex">If the render texture isn't null, the last pass of the deferred pipeline will render into it, else it will render to the screen.</param>        
        public new void Render(RenderContext rc, WritableTexture renderTex = null)
        {
            SetContext(rc);
            AccumulateLight();
            _rc.EnableDepthClamp();

            _canUseGeometryShaders = _rc.GetHardwareCapabilities(HardwareCapability.CAN_USE_GEOMETRY_SHADERS) == 1U ? true : false;

            if (_rc.ClearColor != _texClearColor)
                BackgroundColor = _rc.ClearColor;

            _rc.ClearColor = _texClearColor;

            var screenWidth = _rc.ViewportWidth;
            var screenHeight = _rc.ViewportHeight;

            //Shadow Map Passes: Renders the scene for each light that is casting shadows and creates the shadow map for it.
            _rc.Viewport(0, 0, (int)ShadowMapRes, (int)ShadowMapRes, false);
            RenderShadowMaps(rc);

            //Pass 1: Geometry pass
            _rc.Viewport(0, 0, (int)_gBufferRenderTarget.TextureResolution, (int)_gBufferRenderTarget.TextureResolution, false);
            RenderGeometryPass(rc);

            if (_ssaoOn)
                RenderSSAO(rc);

            //Pass 4 & 5: FXAA and Lighting
            _currentPass = RenderPasses.LIGHTING;

            int width = renderTex == null ? screenWidth : renderTex.Width;
            int height = renderTex == null ? screenHeight : renderTex.Height;

            if (!FxaaOn)
            {
                _rc.Viewport(0, 0, width, height);
                RenderLightPasses(rc, renderTex);
            }
            else
            {
                _rc.Viewport(0, 0, _lightedSceneTex.Width, _lightedSceneTex.Height);
                RenderLightPasses(rc, _lightedSceneTex);
                //Post-Effect: FXAA

                _rc.Viewport(0, 0, width, height);
                RenderFXAA(rc, renderTex);
            }  

            _rc.ResetToDefaultState();
        }

        /// <summary>
        /// Renders one (lighting calculation) pass for each light and blends the results together. 
        /// Alternatively it would be possible to iterate the lights in the shader, but this would create a more complex shader. Additionally it would be more difficult to implement a dynamic number of lights.
        /// The iteration here should not prove critical, due to the scene only consisting of a single quad.
        /// </summary>
        private void RenderLightPasses(RenderContext rc, WritableTexture renderTex = null)
        {
            rc.SetRenderTarget(renderTex);

            var lightPassCnt = 0;

            for (int i = 0; i < LightViseratorResults.Count; i++)
            {
                var isCastingShadows = false; //needed because Android and Web doesn't support geometry shaders.
                var lightVisRes = LightViseratorResults[i];

                if (!lightVisRes.Item2.Light.Active) continue;

                if (lightVisRes.Item2.Light.IsCastingShadows)
                {
                    isCastingShadows = true;
                    var shadowParams = _shadowparams[new Tuple<SceneNodeContainer, LightComponent>(lightVisRes.Item1, lightVisRes.Item2.Light)];

                    //Create and/or choose correct shader effect
                    switch (lightVisRes.Item2.Light.Type)
                    {
                        case LightType.Point:
                            {
                                if (_canUseGeometryShaders)
                                {
                                    if ((_lightingPassEffectPoint == null))
                                        _lightingPassEffectPoint = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, _texClearColor, (WritableCubeMap)shadowParams.ShadowMaps[0]);

                                    _lightingPassEffect = _lightingPassEffectPoint;
                                }
                                else //use no shadows material
                                {
                                    isCastingShadows = false;

                                    if (_lightingPassEffectNoShadow == null)
                                        _lightingPassEffectNoShadow = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, _texClearColor);

                                    _lightingPassEffect = _lightingPassEffectNoShadow;
                                }
                                break;
                            }
                        case LightType.Legacy:
                        case LightType.Parallel:
                            {
                                if (NumberOfCascades > 1)
                                {
                                    if (_lightingPassEffectCascaded == null)
                                    {
                                        var shadowMaps = Array.ConvertAll(shadowParams.ShadowMaps, tex => (WritableTexture)tex);
                                        _lightingPassEffectCascaded = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, shadowMaps, shadowParams.ClipPlanesForLightMat, shadowMaps.Length, _texClearColor);
                                    }

                                    _lightingPassEffect = _lightingPassEffectCascaded;
                                }
                                else
                                {
                                    if (_lightingPassEffectOther == null)
                                        _lightingPassEffectOther = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, _texClearColor, (WritableTexture)shadowParams.ShadowMaps[0]);

                                    _lightingPassEffect = _lightingPassEffectOther;
                                }
                                break;
                            }
                        case LightType.Spot:
                            {
                                if (_lightingPassEffectOther == null)
                                    _lightingPassEffectOther = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, _texClearColor, (WritableTexture)shadowParams.ShadowMaps[0]);

                                _lightingPassEffect = _lightingPassEffectOther;
                                break;
                            }
                        default:
                            break;
                    }
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

        private void RenderShadowMaps(RenderContext rc)
        {                        
            _currentPass = RenderPasses.SHADOW;

            foreach (var lightVisRes in LightViseratorResults)
            {
                if (!lightVisRes.Item2.Light.IsCastingShadows || !lightVisRes.Item2.Light.Active || (lightVisRes.Item2.Light.Type == LightType.Point && !_canUseGeometryShaders)) continue;

                var key = new Tuple<SceneNodeContainer, LightComponent>(lightVisRes.Item1, lightVisRes.Item2.Light);
                var shadowParams = CreateShadowParams(lightVisRes.Item2, key);

                switch (lightVisRes.Item2.Light.Type)
                {
                    case LightType.Point:
                        {
                            if (_shadowCubeMapEffect == null)
                                _shadowCubeMapEffect = ShaderCodeBuilder.ShadowCubeMapEffect(shadowParams.LightSpaceMatrices);
                            else
                                _shadowCubeMapEffect.SetEffectParam($"LightSpaceMatrices[0]", shadowParams.LightSpaceMatrices);

                            _shadowCubeMapEffect.SetEffectParam("LightMatClipPlanes", shadowParams.ClipPlanesForLightMat[0]);
                            _shadowCubeMapEffect.SetEffectParam("LightPos", lightVisRes.Item2.WorldSpacePos);
                            _rc.SetShaderEffect(_shadowCubeMapEffect);

                            rc.SetRenderTarget((IWritableCubeMap)shadowParams.ShadowMaps[0]);
                            Traverse(_sc.Children);
                            break;
                        }
                    case LightType.Legacy:
                    case LightType.Parallel:
                        {
                            for (int i = 0; i < shadowParams.LightSpaceMatrices.Length; i++)
                            {
                                _shadowEffect.SetEffectParam("LightSpaceMatrix", shadowParams.LightSpaceMatrices[i]);
                                _shadowEffect.SetEffectParam("LightType", (int)lightVisRes.Item2.Light.Type);
                                _rc.SetShaderEffect(_shadowEffect);

                                rc.SetRenderTarget(shadowParams.ShadowMaps[i]);
                                Traverse(_sc.Children);
                            }

                            break;
                        }
                    case LightType.Spot:
                        {
                            _shadowEffect.SetEffectParam("LightSpaceMatrix", shadowParams.LightSpaceMatrices[0]);
                            _shadowEffect.SetEffectParam("LightType", (int)lightVisRes.Item2.Light.Type);
                            _rc.SetShaderEffect(_shadowEffect);

                            rc.SetRenderTarget(shadowParams.ShadowMaps[0]);
                            Traverse(_sc.Children);
                            break;
                        }

                    default:
                        break;
                }
            }
        }

        private void RenderGeometryPass(RenderContext rc)
        {            
            _currentPass = RenderPasses.GEOMETRY;
            rc.SetRenderTarget(_gBufferRenderTarget);
            Traverse(_sc.Children);
        }

        private void RenderSSAO(RenderContext rc)
        {
            //Pass 2: SSAO
            _currentPass = RenderPasses.SSAO;
            if (_ssaoTexEffect == null)
                _ssaoTexEffect = ShaderCodeBuilder.SSAORenderTargetTextureEffect(_gBufferRenderTarget, 64, new float2((float)TexRes, (float)TexRes));
            _quadShaderEffectComp.Effect = _ssaoTexEffect;
            rc.SetRenderTarget(_ssaoRenderTexture);
            Traverse(_quadScene.Children);

            //Pass 3: Blur SSAO Texture
            _currentPass = RenderPasses.SSAO_BLUR;
            if (_blurEffect == null)
                _blurEffect = ShaderCodeBuilder.SSAORenderTargetBlurEffect(_ssaoRenderTexture);
            _quadShaderEffectComp.Effect = _blurEffect;
            rc.SetRenderTarget(_blurRenderTex);
            Traverse(_quadScene.Children);

            //Set blurred SSAO Texture as SSAO Texture in gBuffer
            _gBufferRenderTarget.SetTexture(_blurRenderTex, RenderTargetTextureTypes.G_SSAO);
        }

        private void RenderFXAA(RenderContext rc, WritableTexture renderTex = null)
        {
            _currentPass = RenderPasses.FXAA;
            if (_fxaaEffect == null)
                _fxaaEffect = ShaderCodeBuilder.FXAARenderTargetEffect(_lightedSceneTex, new float2((float)TexRes, (float)TexRes));
            _quadShaderEffectComp.Effect = _fxaaEffect;

            if (renderTex == null)
                rc.SetRenderTarget();
            else
                rc.SetRenderTarget(renderTex);

            Traverse(_quadScene.Children);
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
                Diagnostics.Warn("Strength of the light will be clamped between 0 and 1.");
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
                    case LightType.Legacy:
                    case LightType.Parallel:
                        if (NumberOfCascades > 1)
                        {
                            var shadowMaps = Array.ConvertAll(shadowParams.ShadowMaps, tex => (WritableTexture)tex);
                            effect.SetEffectParam("ShadowMaps[0]", shadowMaps);
                            effect.SetEffectParam("ClipPlanes[0]", shadowParams.ClipPlanesForLightMat);
                            effect.SetEffectParam("LightSpaceMatrices[0]", shadowParams.LightSpaceMatrices);
                        }
                        else
                        {
                            effect.SetEffectParam("LightSpaceMatrix", shadowParams.LightSpaceMatrices[0]);
                            effect.SetEffectParam("ShadowMap", (WritableTexture)shadowParams.ShadowMaps[0]);
                        }
                        break;
                    case LightType.Spot:
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
