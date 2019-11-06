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
        private readonly RenderTarget _blurRenderTarget;
        private readonly RenderTarget _ambientLightedSceneRenderTarget;
        private readonly RenderTarget _lightedSceneRenderTarget;

        //This texture caches the SSAO texture object when detaching it from the fbo on the gpu in order to turn SAO off at runtime.
        private readonly WritableTexture _ssaoRenderTexture;

        private readonly Dictionary<Tuple<SceneNodeContainer, LightComponent>, ShadowParams> _shadowparams; //One per Light

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
            
            _thisScenesProjection = pc;
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
                                                
                        if (_thisScenesProjection != null)
                        {
                            float tmpLambda;
                            if (PssmLambda > 1 || PssmLambda < 0)
                            {
                                tmpLambda = M.Clamp(PssmLambda, 0, 1);
                                Diagnostics.Log("#WARNING: lambda is > 1 or < 0 and is therefor camped between 0 and 1.");
                            }
                            else                            
                                tmpLambda = PssmLambda;                            

                            var cascades = ShadowMapping.ParallelSplitCascades(NumberOfCascades, lightView, tmpLambda, _thisScenesProjection.ZNear, _thisScenesProjection.ZFar, _thisScenesProjection.Width, _thisScenesProjection.Height, _thisScenesProjection.Fov, _rc.View).ToList();
                            
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
                        }

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
                            var shadowMap = new WritableCubeMap(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), (int)_shadowMapRes, (int)_shadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER, TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, TextureCompareFunc.GL_LESS);
                            outParams = new ShadowParams() { ClipPlanesForLightMat = shadowParamClipPlanes, LightSpaceMatrices = lightSpaceMatrices, ShadowMaps = new IWritableTexture[1] { shadowMap } };
                            break;
                        }
                    case LightType.Legacy:
                    case LightType.Parallel:
                        {
                            var shadowMaps = new IWritableTexture[NumberOfCascades];
                            for (int i = 0; i < NumberOfCascades; i++)
                            {
                                var shadowMap = new WritableTexture(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), (int)_shadowMapRes, (int)_shadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER, TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, TextureCompareFunc.GL_LESS);
                                shadowMaps[i] = shadowMap;
                            }
                            outParams = new ShadowParams() { ClipPlanesForLightMat = shadowParamClipPlanes, LightSpaceMatrices = lightSpaceMatrices, ShadowMaps = shadowMaps };
                            break;
                        }
                    case LightType.Spot:
                    
                        {
                            var shadowMap = new WritableTexture(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), (int)_shadowMapRes, (int)_shadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER, TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, TextureCompareFunc.GL_LESS);
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

                                rc.SetRenderTarget(shadowParams.ShadowMaps[i]);
                                Traverse(_sc.Children);
                            }

                            break;
                        }
                    case LightType.Spot:                    
                        {
                            _shadowEffect.SetEffectParam("LightSpaceMatrix", shadowParams.LightSpaceMatrices[0]);
                            _shadowEffect.SetEffectParam("LightType", (int)lightVisRes.Item2.Light.Type);

                            rc.SetRenderTarget(shadowParams.ShadowMaps[0]);
                            Traverse(_sc.Children);
                            break;
                        }

                    default:
                        break;
                }
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
                                        _lightingPassEffectPoint = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, (WritableCubeMap)shadowParams.ShadowMaps[0], _texClearColor);
                                    
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
                                        _lightingPassEffectOther = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, (WritableTexture)shadowParams.ShadowMaps[0], _texClearColor);
                                    
                                    _lightingPassEffect = _lightingPassEffectOther;
                                }
                                break;
                            }
                        case LightType.Spot:
                            {
                                if (_lightingPassEffectOther == null)
                                    _lightingPassEffectOther = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, (WritableTexture)shadowParams.ShadowMaps[0], _texClearColor);

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
