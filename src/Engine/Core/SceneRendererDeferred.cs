using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
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
        /// If possible set this in the "Init" method to avoid the creation of an SSAO texture if you don't need one.
        /// </summary>
        public bool SsaoOn
        {
            get
            {
                return _ssaoOn;
            }
            set
            {
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

        private readonly Plane _quad = new Plane();

        private ShaderEffect _ssaoTexEffect;
        private ShaderEffect _lightingPassEffect;
        private ShaderEffect _blurEffect;
        private ShaderEffect _fxaaEffect;
        private ShaderEffect _shadowEffect;
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

        private Frustum _lightFrustum;

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
            _gBufferRenderTarget.SetDepthTex(TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, Compare.LessEqual);
            _gBufferRenderTarget.SetSpecularTex();

            _ssaoRenderTexture = new WritableTexture(RenderTargetTextureTypes.G_SSAO, new ImagePixelFormat(ColorFormat.fRGB16), (int)TexRes, (int)TexRes, false, TextureFilterMode.NEAREST);
            _blurRenderTex = new WritableTexture(RenderTargetTextureTypes.G_SSAO, new ImagePixelFormat(ColorFormat.fRGB16), (int)TexRes, (int)TexRes, false, TextureFilterMode.NEAREST);
            _lightedSceneTex = new WritableTexture(RenderTargetTextureTypes.G_ALBEDO, new ImagePixelFormat(ColorFormat.fRGB32), (int)TexRes, (int)TexRes, false, TextureFilterMode.LINEAR);

            _shadowparams = new Dictionary<Tuple<SceneNodeContainer, LightComponent>, ShadowParams>();

            _gBufferRenderTarget.DeleteBuffers += DeleteBuffers;
        }

        # region Deferred specific visitors

        /// <summary>
        /// If a ShaderEffectComponent is visited the ShaderEffect of the <see cref="RendererState"/> is updated and the effect is set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="effectComponent">The effect component.</param>
        [VisitMethod]
        public new void RenderShaderEffect(EffectComponent effectComponent)
        {
            if (HasNumberOfLightsChanged)
                HasNumberOfLightsChanged = false;

            if (_currentPass != RenderPasses.SHADOW)
            {
                _rc.SetEffect(effectComponent.Effect);
                _state.Effect = effectComponent.Effect;
            }
        }

        /// <summary>
        /// If a Mesh is visited and it has a <see cref="WeightComponent"/> the BoneIndices and  BoneWeights get set, 
        /// the shader parameters for all lights in the scene are updated and the geometry is passed to be pushed through the rendering pipeline.        
        /// </summary>
        /// <param name="mesh">The Mesh.</param>
        [VisitMethod]
        public new void RenderMesh(Mesh mesh)
        {
            if (!mesh.Active) return;

            if (DoFrumstumCulling)
            {
                Frustum frustum;
                if (_currentPass == RenderPasses.SHADOW)
                    frustum = _lightFrustum;
                else
                    frustum = _rc.RenderFrustum;

                //If the bounding box is zero in size, it is not initialized and we cannot perform the culling test.
                if (mesh.BoundingBox.Size != float3.Zero)
                {
                    var worldSpaceBoundingBox = _state.Model * mesh.BoundingBox;
                    if (!worldSpaceBoundingBox.InsideOrIntersectingFrustum(frustum))
                        return;
                }
            }

            WeightComponent wc = CurrentNode.GetWeights();
            if (wc != null)
                AddWeightComponentToMesh(mesh, wc);

            var renderStatesBefore = _rc.CurrentRenderState.Copy();
            _rc.Render(mesh);
            var renderStatesAfter = _rc.CurrentRenderState.Copy();

            _state.RenderUndoStates = renderStatesBefore.Delta(renderStatesAfter);
        }

        #endregion

        #region HierarchyLevel

        /// <summary>
        /// Pops from the RenderState and sets the Model and View matrices in the RenderContext.
        /// </summary>
        protected override void PopState()
        {
            _rc.SetRenderStateSet(_state.RenderUndoStates);
            _state.Pop();
            _rc.Model = _state.Model;

            //If we render the shadow pass: ignore ShaderEffects of the SceneNodes and use the ones that are needed to render the shadow maps.
            if (_currentPass != RenderPasses.SHADOW)
                _rc.SetEffect(_state.Effect);

        }
        #endregion

        #region Shadow mapping

        private ShadowParams CreateShadowParams(LightResult lr, Tuple<SceneNodeContainer, LightComponent> key)
        {
            float4x4[] lightSpaceMatrices;
            List<Frustum> frustums;
            var shadowParamClipPlanes = new float2[NumberOfCascades];

            //1. Calculate light space matrices and clip planes
            switch (lr.Light.Type)
            {
                case LightType.Legacy:
                case LightType.Parallel:
                    {
                        lightSpaceMatrices = new float4x4[NumberOfCascades];
                        frustums = new List<Frustum>();

                        var lightDir = float3.Normalize((lr.Rotation * float4.UnitZ).xyz);

                        var lightPos = lr.WorldSpacePos;
                        var target = lightPos + lightDir;
                        var lightView = float4x4.LookAt(lightPos, target, float3.Normalize(lr.Rotation * float3.UnitY));

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
                        var width = _rc.DefaultState.CanvasWidth;
                        var height = _rc.DefaultState.CanvasHeight;
                        var cascades = ShadowMapping.ParallelSplitCascades(NumberOfCascades, lightView, tmpLambda, zNear, zFar, width, height, fov, _rc.View).ToList();

                        if (cascades.Count <= 1)
                        {
                            var lightProjection = ShadowMapping.CreateOrthographic(cascades[0].Aabb);
                            shadowParamClipPlanes[0] = cascades[0].ClippingPlanes;
                            var completeFrustumLightMat = lightProjection * lightView;
                            lightSpaceMatrices[0] = completeFrustumLightMat;
                            var frustum = new Frustum();
                            frustum.CalculateFrustumPlanes(completeFrustumLightMat);
                            frustums.Add(frustum);

                        }
                        else
                        {
                            for (int i = 0; i < cascades.Count; i++)
                            {
                                shadowParamClipPlanes[i] = cascades[i].ClippingPlanes;
                                var aabbLightSpace = cascades[i].Aabb;
                                lightSpaceMatrices[i] = ShadowMapping.CreateOrthographic(aabbLightSpace) * lightView;
                                var frustum = new Frustum();
                                frustum.CalculateFrustumPlanes(lightSpaceMatrices[i]);
                                frustums.Add(frustum);
                            }
                        }

                        break;
                    }
                case LightType.Point:
                    {
                        lightSpaceMatrices = new float4x4[6];
                        frustums = new List<Frustum>(6);

                        var lightPos = lr.WorldSpacePos;
                        shadowParamClipPlanes = new float2[] { new float2(1, 1 + lr.Light.MaxDistance) };

                        var lightProjection = float4x4.CreatePerspectiveFieldOfView(M.DegreesToRadians(90), 1f, shadowParamClipPlanes[0].x, shadowParamClipPlanes[0].y);

                        float4x4 lightView;

                        lightView = float4x4.LookAt(lightPos, lightPos - float3.UnitX, float3.UnitY); //left face
                        lightSpaceMatrices[0] = lightProjection * lightView;
                        var frustumLeft = new Frustum();
                        frustumLeft.CalculateFrustumPlanes(lightSpaceMatrices[0]);
                        frustums.Add(frustumLeft);

                        lightView = float4x4.LookAt(lightPos, lightPos + float3.UnitX, float3.UnitY); //right face
                        lightSpaceMatrices[1] = lightProjection * lightView;
                        var frustumRight = new Frustum();
                        frustumRight.CalculateFrustumPlanes(lightSpaceMatrices[1]);
                        frustums.Add(frustumRight);

                        lightView = float4x4.LookAt(lightPos, lightPos - float3.UnitY, -float3.UnitZ); //lower face
                        lightSpaceMatrices[2] = lightProjection * lightView;
                        var frustumBottom = new Frustum();
                        frustumBottom.CalculateFrustumPlanes(lightSpaceMatrices[2]);
                        frustums.Add(frustumBottom);

                        lightView = float4x4.LookAt(lightPos, lightPos + float3.UnitY, float3.UnitZ); //upper face
                        lightSpaceMatrices[3] = lightProjection * lightView;
                        var frustumTop = new Frustum();
                        frustumTop.CalculateFrustumPlanes(lightSpaceMatrices[3]);
                        frustums.Add(frustumTop);

                        lightView = float4x4.LookAt(lightPos, lightPos - float3.UnitZ, float3.UnitY); //front face
                        lightSpaceMatrices[4] = lightProjection * lightView;
                        var frustumFront = new Frustum();
                        frustumFront.CalculateFrustumPlanes(lightSpaceMatrices[4]);
                        frustums.Add(frustumFront);

                        lightView = float4x4.LookAt(lightPos, lightPos + float3.UnitZ, float3.UnitY); //back face
                        lightSpaceMatrices[5] = lightProjection * lightView;
                        var frustumBack = new Frustum();
                        frustumBack.CalculateFrustumPlanes(lightSpaceMatrices[5]);
                        frustums.Add(frustumBack);

                        break;
                    }
                case LightType.Spot:
                    {
                        lightSpaceMatrices = new float4x4[1];
                        frustums = new List<Frustum>(1);

                        var lightPos = lr.WorldSpacePos;
                        shadowParamClipPlanes = new float2[] { new float2(0.1f, 0.1f + lr.Light.MaxDistance) };
                        var lightProjection = float4x4.CreatePerspectiveFieldOfView(M.DegreesToRadians(lr.Light.OuterConeAngle) * 2, 1f, shadowParamClipPlanes[0].x, shadowParamClipPlanes[0].y);
                        var lightView = float4x4.LookAt(lightPos, lightPos + float3.Normalize(lr.Rotation * float3.UnitZ), lr.Rotation * float3.UnitY);
                        lightSpaceMatrices[0] = lightProjection * lightView;
                        var frustum = new Frustum();
                        frustum.CalculateFrustumPlanes(lightSpaceMatrices[0]);
                        frustums.Add(frustum);
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
                            var shadowMap = new WritableCubeMap(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth16), (int)ShadowMapRes, (int)ShadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER, TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, Compare.Less);
                            outParams = new ShadowParams() { ClipPlanesForLightMat = shadowParamClipPlanes, LightSpaceMatrices = lightSpaceMatrices, ShadowMap = shadowMap, Frustums = frustums };
                            break;
                        }
                    case LightType.Legacy:
                    case LightType.Parallel:
                        {
                            IWritableTexture shadowMap;
                            if (NumberOfCascades == 1)
                                shadowMap = new WritableTexture(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth24), (int)ShadowMapRes, (int)ShadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER, TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, Compare.Less);
                            else if (NumberOfCascades > 1)
                                shadowMap = new WritableArrayTexture(NumberOfCascades, RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth24), (int)ShadowMapRes, (int)ShadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER, TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, Compare.Less);
                            else
                                throw new ArgumentException($"Number of shadow cascades is {NumberOfCascades} but must be greater or equal 1.");

                            outParams = new ShadowParams() { ClipPlanesForLightMat = shadowParamClipPlanes, LightSpaceMatrices = lightSpaceMatrices, ShadowMap = shadowMap, Frustums = frustums };
                            break;
                        }
                    case LightType.Spot:
                        {
                            var shadowMap = new WritableTexture(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth16), (int)ShadowMapRes, (int)ShadowMapRes, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER, TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE, Compare.Less);
                            outParams = new ShadowParams() { ClipPlanesForLightMat = shadowParamClipPlanes, LightSpaceMatrices = lightSpaceMatrices, ShadowMap = shadowMap, Frustums = frustums };
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
                outParams.Frustums = frustums;
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
        public void Render(RenderContext rc, WritableTexture renderTex = null)
        {
            SetContext(rc);

            PrePassVisitor.PrePassTraverse(_sc, _rc, false);
            AccumulateLight();

            _rc.EnableDepthClamp();

            _canUseGeometryShaders = _rc.GetHardwareCapabilities(HardwareCapability.CAN_USE_GEOMETRY_SHADERS) == 1U ? true : false;

            if (PrePassVisitor.CameraPrepassResults.Count != 0)
            {
                var cams = PrePassVisitor.CameraPrepassResults.OrderBy(cam => cam.Item2.Camera.Layer);
                foreach (var cam in cams)
                {
                    if (cam.Item2.Camera.Active)
                    {
                        DoFrumstumCulling = cam.Item2.Camera.FrustumCullingOn;
                        PerCamRender(cam, renderTex);
                        //Reset Viewport and frustum culling bool in case we have another scene, rendered without a camera
                        _rc.Viewport(0, 0, rc.DefaultState.CanvasWidth, rc.DefaultState.CanvasHeight);
                        DoFrumstumCulling = true;
                    }
                }
            }
            else
                RenderAllPasses(new float4(0, 0, _rc.ViewportWidth, _rc.ViewportHeight), renderTex);
        }

        private void PerCamRender(Tuple<SceneNodeContainer, CameraResult> cam, WritableTexture renderTex = null)
        {
            var tex = cam.Item2.Camera.RenderTexture;

            if (tex != null)
                _rc.SetRenderTarget(cam.Item2.Camera.RenderTexture);
            else
                _rc.SetRenderTarget();

            _rc.Projection = cam.Item2.Camera.GetProjectionMat(_rc.ViewportWidth, _rc.ViewportHeight, out float4 viewport);
            _rc.Viewport((int)viewport.x, (int)viewport.y, (int)viewport.z, (int)viewport.w);

            _rc.ClearColor = cam.Item2.Camera.BackgroundColor;

            if (cam.Item2.Camera.ClearColor)
                _rc.Clear(ClearFlags.Color);

            if (cam.Item2.Camera.ClearDepth)
                _rc.Clear(ClearFlags.Depth);

            _rc.View = cam.Item2.View;

            RenderAllPasses(viewport, renderTex);
        }

        private void RenderAllPasses(float4 lightingPassViewport, WritableTexture renderTex = null)
        {
            var preRenderStateSet = _rc.CurrentRenderState.Copy(); //"Snapshot" of the current render states as they came from the user code.
            var preRenderLockedStates = new Dictionary<RenderState, KeyValuePair<bool, uint>>(_rc.LockedStates);

            if (_rc.ClearColor != _texClearColor)
                BackgroundColor = _rc.ClearColor;

            _rc.ClearColor = _texClearColor;

            //Pass 1: Geometry pass - render with current render states.
            _rc.Viewport(0, 0, (int)_gBufferRenderTarget.TextureResolution, (int)_gBufferRenderTarget.TextureResolution);
            RenderGeometryPass();

            //Shadow Map Passes: Renders the scene for each light that is casting shadows and creates the shadow map for it.
            _rc.Viewport(0, 0, (int)ShadowMapRes, (int)ShadowMapRes);

            //Cache state because rendering the shadow maps will change the state eventually.
            var doCulling = DoFrumstumCulling;
            RenderShadowMaps();
            DoFrumstumCulling = doCulling;

            //Undo all user-made and shadow pass related render state changes to be able to work on a "white sheet" from here on.
            _rc.UnlockAllRenderStates();
            _rc.SetRenderStateSet(new RenderStateSet());

            if (_ssaoOn)
                RenderSSAO();

            //Pass 4 & 5: FXAA and Lighting
            _currentPass = RenderPasses.LIGHTING;

            int width = renderTex == null ? (int)lightingPassViewport.z : renderTex.Width;
            int height = renderTex == null ? (int)lightingPassViewport.w : renderTex.Height;

            if (!FxaaOn)
            {
                _rc.Viewport((int)lightingPassViewport.x, (int)lightingPassViewport.y, width, height);
                RenderLightPasses(renderTex);
            }
            else
            {
                _rc.Viewport(0, 0, _lightedSceneTex.Width, _lightedSceneTex.Height);
                RenderLightPasses(_lightedSceneTex);

                //Post-Effect: FXAA
                _rc.Viewport((int)lightingPassViewport.x, (int)lightingPassViewport.y, width, height);
                RenderFXAA(renderTex);
            }

            //Reset states and locks to the values they had before rendering the deferred passes (after shadow and geometry pass).
            foreach (var state in preRenderStateSet.States)
            {
                if (preRenderLockedStates.TryGetValue(state.Key, out var lockState))
                    _rc.SetRenderState(state.Key, state.Value, lockState.Key);
                else
                    _rc.SetRenderState(state.Key, state.Value);
            }
        }

        /// <summary>
        /// Renders one (lighting calculation) pass for each light and blends the results together. 
        /// Alternatively it would be possible to iterate the lights in the shader, but this would create a more complex shader. Additionally it would be more difficult to implement a dynamic number of lights.
        /// The iteration here should not prove critical, due to the scene only consisting of a single quad.
        /// </summary>
        private void RenderLightPasses(WritableTexture renderTex = null)
        {
            if (renderTex != null)
                _rc.SetRenderTarget(renderTex);
            else
                _rc.SetRenderTarget();

            _rc.Clear(ClearFlags.Depth | ClearFlags.Color);

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
                                    if (_lightingPassEffectPoint == null)
                                    {
                                        _lightingPassEffectPoint = MakeShaderEffect.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, _texClearColor, (WritableCubeMap)shadowParams.ShadowMap);
                                        _rc.CreateEffect(true, _lightingPassEffectPoint);
                                    }
                                    _lightingPassEffect = _lightingPassEffectPoint;
                                }
                                else //use no shadows material
                                {
                                    isCastingShadows = false;

                                    if (_lightingPassEffectNoShadow == null)
                                    {
                                        _lightingPassEffectNoShadow = MakeShaderEffect.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, _texClearColor);
                                        _rc.CreateEffect(true, _lightingPassEffectNoShadow);
                                    }
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
                                        _lightingPassEffectCascaded = MakeShaderEffect.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, (WritableArrayTexture)shadowParams.ShadowMap, shadowParams.ClipPlanesForLightMat, NumberOfCascades, _texClearColor);
                                        _rc.CreateEffect(true, _lightingPassEffectCascaded);
                                    }
                                    _lightingPassEffect = _lightingPassEffectCascaded;
                                }
                                else
                                {
                                    if (_lightingPassEffectOther == null)
                                    {
                                        _lightingPassEffectOther = MakeShaderEffect.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, _texClearColor, (WritableTexture)shadowParams.ShadowMap);
                                        _rc.CreateEffect(true, _lightingPassEffectOther);
                                    }
                                    _lightingPassEffect = _lightingPassEffectOther;
                                }
                                break;
                            }
                        case LightType.Spot:
                            {
                                if (_lightingPassEffectOther == null)
                                {
                                    _lightingPassEffectOther = MakeShaderEffect.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, _texClearColor, (WritableTexture)shadowParams.ShadowMap);
                                    _rc.CreateEffect(true, _lightingPassEffectOther);
                                }
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
                    {
                        _lightingPassEffectNoShadow = MakeShaderEffect.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Item2.Light, _texClearColor);
                        _rc.CreateEffect(true, _lightingPassEffectNoShadow);
                    }
                    _lightingPassEffect = _lightingPassEffectNoShadow;
                }

                UpdateLightAndShadowParams(lightVisRes, _lightingPassEffect, isCastingShadows);
                _lightingPassEffect.SetFxParam(ShaderShards.UniformNameDeclarations.RenderPassNo, lightPassCnt);

                if (_needToSetSSAOTex)
                {
                    _lightingPassEffect.SetFxParam(ShaderShards.UniformNameDeclarations.SsaoOn, _ssaoOn ? 1 : 0);
                    _needToSetSSAOTex = false;
                }

                //Set background color only in last light pass to NOT blend the color (additive).
                if (i == LightViseratorResults.Count - 1)
                    _lightingPassEffect.SetFxParam(ShaderShards.UniformNameDeclarations.BackgroundColor, BackgroundColor);
                else
                    _lightingPassEffect.SetFxParam(ShaderShards.UniformNameDeclarations.BackgroundColor, _texClearColor);

                _rc.SetEffect(_lightingPassEffect);
                _rc.Render(_quad);
                lightPassCnt++;
            }
        }

        private void RenderShadowMaps()
        {
            _currentPass = RenderPasses.SHADOW;
            if (_shadowEffect == null)
            {
                _shadowEffect = MakeShaderEffect.ShadowMapEffect();
                _rc.CreateEffect(true, _shadowEffect);
            }

            foreach (var lightVisRes in LightViseratorResults)
            {
                if (!lightVisRes.Item2.Light.IsCastingShadows || !lightVisRes.Item2.Light.Active || (lightVisRes.Item2.Light.Type == LightType.Point && !_canUseGeometryShaders)) continue;

                var key = new Tuple<SceneNodeContainer, LightComponent>(lightVisRes.Item1, lightVisRes.Item2.Light);
                var shadowParams = CreateShadowParams(lightVisRes.Item2, key);

                switch (lightVisRes.Item2.Light.Type)
                {
                    case LightType.Point:
                        {
                            DoFrumstumCulling = false;

                            if (_shadowCubeMapEffect == null)
                            {
                                _shadowCubeMapEffect = MakeShaderEffect.ShadowCubeMapEffect(shadowParams.LightSpaceMatrices);
                                _rc.CreateEffect(true, _shadowCubeMapEffect);
                            }
                            else
                                _shadowCubeMapEffect.SetFxParam($"LightSpaceMatrices[0]", shadowParams.LightSpaceMatrices);

                            _shadowCubeMapEffect.SetFxParam("LightMatClipPlanes", shadowParams.ClipPlanesForLightMat[0]);
                            _shadowCubeMapEffect.SetFxParam("LightPos", lightVisRes.Item2.WorldSpacePos);
                            _rc.SetEffect(_shadowCubeMapEffect);

                            _rc.SetRenderTarget((IWritableCubeMap)shadowParams.ShadowMap);

                            //No culling here because much of the work is done in the geometry shader.

                            Traverse(_sc.Children);
                            break;
                        }
                    case LightType.Legacy:
                    case LightType.Parallel:
                        {
                            if (NumberOfCascades == 1)
                            {
                                _shadowEffect.SetFxParam(ShaderShards.UniformNameDeclarations.LightSpaceMatrix, shadowParams.LightSpaceMatrices[0]);
                                _rc.SetEffect(_shadowEffect);
                                _rc.SetRenderTarget((IWritableTexture)shadowParams.ShadowMap);

                                _lightFrustum = shadowParams.Frustums[0];

                                Traverse(_sc.Children);
                            }
                            else if (NumberOfCascades > 1)
                            {
                                for (int i = 0; i < NumberOfCascades; i++)
                                {
                                    _shadowEffect.SetFxParam(ShaderShards.UniformNameDeclarations.LightSpaceMatrix, shadowParams.LightSpaceMatrices[i]);
                                    _rc.SetEffect(_shadowEffect);
                                    _rc.SetRenderTarget((IWritableArrayTexture)shadowParams.ShadowMap, i);

                                    _lightFrustum = shadowParams.Frustums[i];

                                    Traverse(_sc.Children);
                                }
                            }
                            else
                                throw new ArgumentException($"Number of cascades must be greater or equal 1 but is  {NumberOfCascades}.");


                            break;
                        }
                    case LightType.Spot:
                        {
                            DoFrumstumCulling = true;
                            _shadowEffect.SetFxParam(ShaderShards.UniformNameDeclarations.LightSpaceMatrix, shadowParams.LightSpaceMatrices[0]);
                            _rc.SetEffect(_shadowEffect);
                            _rc.SetRenderTarget(shadowParams.ShadowMap);

                            _lightFrustum = shadowParams.Frustums[0];
                            Traverse(_sc.Children);
                            break;
                        }

                    default:
                        break;
                }
            }
        }

        private void RenderGeometryPass()
        {
            _currentPass = RenderPasses.GEOMETRY;
            _rc.SetRenderTarget(_gBufferRenderTarget);
            Traverse(_sc.Children);
        }

        private void RenderSSAO()
        {
            //Pass 2: SSAO
            _currentPass = RenderPasses.SSAO;
            if (_ssaoTexEffect == null)
            {
                _ssaoTexEffect = MakeShaderEffect.SSAORenderTargetTextureEffect(_gBufferRenderTarget, 64, new float2((float)TexRes, (float)TexRes), 4);
                _rc.CreateEffect(true, _ssaoTexEffect);
            }
            _rc.SetEffect(_ssaoTexEffect);
            _rc.SetRenderTarget(_ssaoRenderTexture);
            _rc.Render(_quad);

            //Pass 3: Blur SSAO Texture
            _currentPass = RenderPasses.SSAO_BLUR;
            if (_blurEffect == null)
            {
                _blurEffect = MakeShaderEffect.SSAORenderTargetBlurEffect(_ssaoRenderTexture);
                _rc.CreateEffect(true, _blurEffect);
            }
            _rc.SetEffect(_blurEffect);
            _rc.SetRenderTarget(_blurRenderTex);
            _rc.Render(_quad);

            //Set blurred SSAO Texture as SSAO Texture in gBuffer
            _gBufferRenderTarget.SetTexture(_blurRenderTex, RenderTargetTextureTypes.G_SSAO);
        }

        private void RenderFXAA(WritableTexture renderTex = null)
        {
            _currentPass = RenderPasses.FXAA;
            if (_fxaaEffect == null)
            {
                _fxaaEffect = MakeShaderEffect.FXAARenderTargetEffect(_lightedSceneTex, new float2((float)TexRes, (float)TexRes));
                _rc.CreateEffect(true, _fxaaEffect);
            }
            _rc.SetEffect(_fxaaEffect);

            if (renderTex == null)
                _rc.SetRenderTarget();
            else
                _rc.SetRenderTarget(renderTex);

            _rc.Render(_quad);
        }

        private void UpdateLightAndShadowParams(Tuple<SceneNodeContainer, LightResult> lightVisRes, ShaderEffect effect, bool isCastingShadows)
        {
            var lightRes = lightVisRes.Item2;
            var light = lightRes.Light;

            if (light.Type == LightType.Legacy)
            {
                lightRes.Rotation = new float4x4
                (
                    new float4(_rc.InvView.Row0.xyz, 0),
                    new float4(_rc.InvView.Row1.xyz, 0),
                    new float4(_rc.InvView.Row2.xyz, 0),
                    float4.UnitW
                 );
            }

            var dirWorldSpace = float3.Normalize((lightRes.Rotation * float4.UnitZ).xyz);
            var dirViewSpace = float3.Normalize((_rc.View * new float4(dirWorldSpace)).xyz);
            var strength = light.Strength;

            if (strength > 1.0 || strength < 0.0)
            {
                strength = M.Clamp(light.Strength, 0.0f, 1.0f);
                Diagnostics.Warn("Strength of the light will be clamped between 0 and 1.");
            }

            // Set params in modelview space since the lightning calculation is in modelview space
            switch (lightVisRes.Item2.Light.Type)
            {
                case LightType.Point:
                    effect.SetFxParam("light.position", _rc.View * lightRes.WorldSpacePos);
                    effect.SetFxParam("light.intensities", light.Color);
                    effect.SetFxParam("light.maxDistance", light.MaxDistance);
                    effect.SetFxParam("light.strength", strength);
                    effect.SetFxParam("light.isActive", light.Active ? 1 : 0);
                    break;
                case LightType.Legacy:
                case LightType.Parallel:
                    effect.SetFxParam("light.intensities", light.Color);
                    effect.SetFxParam("light.strength", strength);
                    effect.SetFxParam("light.direction", dirViewSpace);
                    effect.SetFxParam("light.isActive", light.Active ? 1 : 0);
                    break;
                case LightType.Spot:
                    effect.SetFxParam("light.position", _rc.View * lightRes.WorldSpacePos);
                    effect.SetFxParam("light.intensities", light.Color);
                    effect.SetFxParam("light.maxDistance", light.MaxDistance);
                    effect.SetFxParam("light.strength", strength);
                    effect.SetFxParam("light.outerConeAngle", M.DegreesToRadians(light.OuterConeAngle));
                    effect.SetFxParam("light.innerConeAngle", M.DegreesToRadians(light.InnerConeAngle));
                    effect.SetFxParam("light.direction", dirViewSpace);
                    effect.SetFxParam("light.isActive", light.Active ? 1 : 0);
                    break;
                default:
                    break;
            }

            if (isCastingShadows) //we don't use light.IsCastingShadows because we could need to skip the shadow calculation because of hardware capabilities.
            {
                effect.SetFxParam("light.isCastingShadows", light.IsCastingShadows ? 1 : 0);
                effect.SetFxParam("light.bias", light.Bias);
                var shadowParams = _shadowparams[new Tuple<SceneNodeContainer, LightComponent>(lightVisRes.Item1, lightVisRes.Item2.Light)];

                switch (lightVisRes.Item2.Light.Type)
                {
                    case LightType.Point:
                        effect.SetFxParam(ShaderShards.UniformNameDeclarations.ShadowCubeMap, (WritableCubeMap)shadowParams.ShadowMap);
                        break;
                    case LightType.Legacy:
                    case LightType.Parallel:
                        if (NumberOfCascades > 1)
                        {
                            effect.SetFxParam("ShadowMap", shadowParams.ShadowMap);
                            effect.SetFxParam("ClipPlanes[0]", shadowParams.ClipPlanesForLightMat);
                            effect.SetFxParam("LightSpaceMatrices[0]", shadowParams.LightSpaceMatrices);
                        }
                        else
                        {
                            effect.SetFxParam(ShaderShards.UniformNameDeclarations.LightSpaceMatrix, shadowParams.LightSpaceMatrices[0]);
                            effect.SetFxParam(ShaderShards.UniformNameDeclarations.ShadowMap, (WritableTexture)shadowParams.ShadowMap);
                        }
                        break;
                    case LightType.Spot:
                        effect.SetFxParam(ShaderShards.UniformNameDeclarations.LightSpaceMatrix, shadowParams.LightSpaceMatrices[0]);
                        effect.SetFxParam(ShaderShards.UniformNameDeclarations.ShadowMap, (WritableTexture)shadowParams.ShadowMap);
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
