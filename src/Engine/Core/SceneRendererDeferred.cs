using CommunityToolkit.Diagnostics;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
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
        /// The texture resolution in pixel, that is used to create the shadow maps for light sources that cast shadows.
        /// Note that shadow casting is only available with deferred rendering.
        /// </summary>
        public TexRes ShadowMapRes { get; private set; } = TexRes.Middle;

        /// <summary>
        /// The texture resolution in pixel, that is used for the G-Buffer textures.
        /// </summary>
        public TexRes TexRes { get; private set; } = TexRes.Middle;

        /// <summary>
        /// Sets the distance to the far plane for the cascaded shadow mapping calculation.
        /// For smaller scenes we need to choose smaller values to get shadow maps with decent resolution.
        /// </summary>
        public int CascadeFarPlane = 500;

        /// <summary>
        /// Controls whether the render output is anti aliased using FXAA.
        /// This is done in an additional pass that is turned off if this is set to false.
        /// </summary>
        public bool FxaaOn { get; set; } = true;

        /// <summary>
        /// Determines if the scene gets rendered with Screen Space Ambient Occlusion.
        /// This is done in an additional pass that is turned of if this is set to false.
        /// In this case the ambient component of the lighting is a static value.
        /// If possible set this in the "Init" method to avoid the creation of an SSAO texture if you don't need one.
        /// </summary>
        public bool SsaoOn
        {
            get => _ssaoOn;
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

        private float4 _texClearColor = new(0, 0, 0, 0);

        private readonly Plane _quad = new();

        private ShaderEffect? _ssaoTexEffect;
        private ShaderEffect? _lightingPassEffect;
        private ShaderEffect? _blurEffect;
        private ShaderEffect? _fxaaEffect;
        private ShaderEffect? _shadowEffect;
        private ShaderEffect? _shadowCubeMapEffect;
        private ShaderEffect? _shadowCubeMapEffectPointPrimitives;

        //The following ShaderEffects cache all possible ShaderEffects, needed to render the lighting passes.
        private ShaderEffect? _lightingPassEffectPoint; //needed when a point light is rendered;
        private ShaderEffect? _lightingPassEffectOther; //needed when a light of another type is rendered;
        private ShaderEffect? _lightingPassEffectNoShadow; //needed when a light of another type is rendered without shadows;
        private ShaderEffect? _lightingPassEffectCascaded; //needed when a parallel light is rendered with cascaded shadow mapping;
        private ShaderEffect? _lightingPassEffectNoCascades; //needed when a parallel light is rendered without cascaded shadow mapping;

        private IRenderTarget? _gBufferRenderTarget;

        //This texture caches the SSAO texture object when detaching it from the fbo on the gpu in order to turn SAO off at runtime.
        private WritableTexture? _ssaoRenderTexture;
        private WritableTexture? _blurRenderTex;
        private WritableTexture? _lightedSceneTex; //Do post-pro effects like FXAA on this texture.

        private readonly Dictionary<Light, ShadowParams>? _shadowparams; //One per Light

        private RenderPasses _currentPass;
        private bool _canUseGeometryShaders;

        private FrustumF? _lightFrustum;

        private LightType _currentLightType;

        /// <summary>
        /// Creates a new instance of type SceneRendererDeferred.
        /// </summary>
        /// <param name="sc">The SceneContainer, containing the scene that gets rendered.</param>
        /// <param name="renderLayer"></param>
        /// <param name="texRes">The g-buffer texture resolution.</param>
        /// <param name="shadowMapRes">The shadow map resolution.</param>
        public SceneRendererDeferred(SceneContainer sc, RenderLayers renderLayer = RenderLayers.Default, TexRes texRes = TexRes.Middle, TexRes shadowMapRes = TexRes.Middle) : base(sc, renderLayer)
        {
            Diagnostics.Warn($"Alpha blend is disabled for deferred rendering for now - {RenderState.AlphaBlendEnable} is locked (see SceneRendererDeferred.RenderAllPasses()).");

            IgnoreInactiveComponents = true;
            TexRes = texRes;
            ShadowMapRes = shadowMapRes;

            _shadowparams = new Dictionary<Light, ShadowParams>();
        }

        # region Deferred specific visitors

        /// <summary>
        /// If a ShaderEffectComponent is visited the ShaderEffect of the <see cref="RendererState"/> is updated and the effect is set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="effect">The effect.</param>
        [VisitMethod]
        public void RenderShaderEffect(Effect effect)
        {
            if (_currentPass != RenderPasses.Shadow)
            {
                _rc.SetEffect(effect, false);
                _state.Effect = effect;
            }
        }

        /// <summary>
        /// If a Mesh is visited and it has a <see cref="Weight"/> the BoneIndices and  BoneWeights get set,
        /// the shader parameters for all lights in the scene are updated and the geometry is passed to be pushed through the rendering pipeline.
        /// </summary>
        /// <param name="mesh">The Mesh.</param>
        [VisitMethod]
        public new void RenderMesh(Mesh mesh)
        {
            if (!mesh.Active) return;
            if (!RenderLayer.HasFlag(_state.RenderLayer.Layer) && !_state.RenderLayer.Layer.HasFlag(RenderLayer) || _state.RenderLayer.Layer.HasFlag(RenderLayers.None))
                return;

            Guard.IsNotNull(_rc);

            if (DoFrumstumCulling)
            {
                Guard.IsNotNull(_lightFrustum);

                FrustumF frustum;
                if (_currentPass == RenderPasses.Shadow)
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

            if (_currentPass == RenderPasses.Shadow && _currentLightType == LightType.Point)
            {
                Guard.IsNotNull(_shadowCubeMapEffectPointPrimitives);
                Guard.IsNotNull(_shadowCubeMapEffect);

                if (mesh.MeshType == PrimitiveType.Points)
                    _rc.SetEffect(_shadowCubeMapEffectPointPrimitives);
                else
                    _rc.SetEffect(_shadowCubeMapEffect);
            }
            _rc.Render(mesh, CurrentInstanceData, _currentPass == RenderPasses.Shadow);
            CurrentInstanceData = null;
        }

        /// <summary>
        /// If a Mesh is visited the shader parameters for all lights in the scene are updated and the geometry is passed to be pushed through the rendering pipeline.
        /// </summary>
        /// <param name="mesh">The Mesh.</param>
        [VisitMethod]
        public new void RenderMesh(GpuMesh mesh)
        {
            if (!mesh.Active) return;
            if (!RenderLayer.HasFlag(_state.RenderLayer.Layer) && !_state.RenderLayer.Layer.HasFlag(RenderLayer) || _state.RenderLayer.Layer.HasFlag(RenderLayers.None))
                return;

            Guard.IsNotNull(_rc);

            if (DoFrumstumCulling)
            {
                Guard.IsNotNull(_lightFrustum);
                FrustumF frustum;
                if (_currentPass == RenderPasses.Shadow)
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

            if (_currentPass == RenderPasses.Shadow && _currentLightType == LightType.Point)
            {
                if (mesh.MeshType == PrimitiveType.Points)
                {
                    Guard.IsNotNull(_shadowCubeMapEffectPointPrimitives);
                    _rc.SetEffect(_shadowCubeMapEffectPointPrimitives);
                }
                else
                {

                    Guard.IsNotNull(_shadowCubeMapEffect);
                    _rc.SetEffect(_shadowCubeMapEffect);
                }
            }

            var renderStatesBefore = _rc.CurrentRenderState.Copy();
            _rc.Render(mesh, _currentPass == RenderPasses.Shadow);
        }

        #endregion

        #region HierarchyLevel

        /// <summary>
        /// Pops from the RenderState and sets the Model and View matrices in the RenderContext.
        /// </summary>
        protected override void PopState()
        {
            _state.Pop();
            _rc.Model = _state.Model;

            //If we render the shadow pass: ignore ShaderEffects of the SceneNodes and use the ones that are needed to render the shadow maps.
            if (_currentPass != RenderPasses.Shadow)
                _rc.SetEffect(_state.Effect);

        }
        #endregion

        #region Shadow mapping

        private ShadowParams CreateShadowParams(LightResult lr)
        {
            float4x4[] lightSpaceMatrices;
            List<FrustumF> frustums;
            var shadowParamClipPlanes = new float2[NumberOfCascades];

            //1. Calculate light space matrices and clip planes
            switch (lr.Light.Type)
            {
                case LightType.Legacy:
                case LightType.Parallel:
                    {
                        lightSpaceMatrices = new float4x4[NumberOfCascades];
                        frustums = new List<FrustumF>();

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
                        {
                            tmpLambda = PssmLambda;
                        }

                        var zNear = _rc.DefaultState.ZNearDefautlt;
                        var zFar = CascadeFarPlane == 0 ? _rc.DefaultState.ZFarDefault : CascadeFarPlane;
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
                            var frustum = new FrustumF();
                            frustum.CalculateFrustumPlanes(completeFrustumLightMat);
                            frustums.Add(frustum);

                        }
                        else
                        {
                            for (var i = 0; i < cascades.Count; i++)
                            {
                                shadowParamClipPlanes[i] = cascades[i].ClippingPlanes;
                                var aabbLightSpace = cascades[i].Aabb;
                                lightSpaceMatrices[i] = ShadowMapping.CreateOrthographic(aabbLightSpace) * lightView;
                                var frustum = new FrustumF();
                                frustum.CalculateFrustumPlanes(lightSpaceMatrices[i]);
                                frustums.Add(frustum);
                            }
                        }

                        break;
                    }
                case LightType.Point:
                    {
                        lightSpaceMatrices = new float4x4[6];
                        frustums = new List<FrustumF>(6);

                        var lightPos = lr.WorldSpacePos;
                        shadowParamClipPlanes = new float2[] { new float2(1, 1 + lr.Light.MaxDistance) };

                        var lightProjection = float4x4.CreatePerspectiveFieldOfView(M.DegreesToRadians(90), 1f, shadowParamClipPlanes[0].x, shadowParamClipPlanes[0].y);

                        float4x4 lightView;

                        lightView = float4x4.LookAt(lightPos, lightPos - float3.UnitX, float3.UnitY); //left face
                        lightSpaceMatrices[0] = lightProjection * lightView;
                        var frustumLeft = new FrustumF();
                        frustumLeft.CalculateFrustumPlanes(lightSpaceMatrices[0]);
                        frustums.Add(frustumLeft);

                        lightView = float4x4.LookAt(lightPos, lightPos + float3.UnitX, float3.UnitY); //right face
                        lightSpaceMatrices[1] = lightProjection * lightView;
                        var frustumRight = new FrustumF();
                        frustumRight.CalculateFrustumPlanes(lightSpaceMatrices[1]);
                        frustums.Add(frustumRight);

                        lightView = float4x4.LookAt(lightPos, lightPos - float3.UnitY, -float3.UnitZ); //lower face
                        lightSpaceMatrices[2] = lightProjection * lightView;
                        var frustumBottom = new FrustumF();
                        frustumBottom.CalculateFrustumPlanes(lightSpaceMatrices[2]);
                        frustums.Add(frustumBottom);

                        lightView = float4x4.LookAt(lightPos, lightPos + float3.UnitY, float3.UnitZ); //upper face
                        lightSpaceMatrices[3] = lightProjection * lightView;
                        var frustumTop = new FrustumF();
                        frustumTop.CalculateFrustumPlanes(lightSpaceMatrices[3]);
                        frustums.Add(frustumTop);

                        lightView = float4x4.LookAt(lightPos, lightPos - float3.UnitZ, float3.UnitY); //front face
                        lightSpaceMatrices[4] = lightProjection * lightView;
                        var frustumFront = new FrustumF();
                        frustumFront.CalculateFrustumPlanes(lightSpaceMatrices[4]);
                        frustums.Add(frustumFront);

                        lightView = float4x4.LookAt(lightPos, lightPos + float3.UnitZ, float3.UnitY); //back face
                        lightSpaceMatrices[5] = lightProjection * lightView;
                        var frustumBack = new FrustumF();
                        frustumBack.CalculateFrustumPlanes(lightSpaceMatrices[5]);
                        frustums.Add(frustumBack);

                        break;
                    }
                case LightType.Spot:
                    {
                        lightSpaceMatrices = new float4x4[1];
                        frustums = new List<FrustumF>(1);

                        var lightPos = lr.WorldSpacePos;
                        shadowParamClipPlanes = new float2[] { new float2(0.1f, 0.1f + lr.Light.MaxDistance) };
                        var lightProjection = float4x4.CreatePerspectiveFieldOfView(M.DegreesToRadians(lr.Light.OuterConeAngle) * 2, 1f, shadowParamClipPlanes[0].x, shadowParamClipPlanes[0].y);
                        var lightView = float4x4.LookAt(lightPos, lightPos + float3.Normalize(lr.Rotation * float3.UnitZ), lr.Rotation * float3.UnitY);
                        lightSpaceMatrices[0] = lightProjection * lightView;
                        var frustum = new FrustumF();
                        frustum.CalculateFrustumPlanes(lightSpaceMatrices[0]);
                        frustums.Add(frustum);
                        break;
                    }
                default:
                    throw new ArgumentException("No Light Space Matrix created, light type not supported!");
            }

            //2. If we haven't created the shadow parameters for this light yet, do so,
            if (!_shadowparams.TryGetValue(lr.Light, out var outParams))
            {
                switch (lr.Light.Type)
                {
                    case LightType.Point:
                        {
                            //Use TextureCompareMode.CompareRefToTexture with SamplerCubeShadow to enable hardware anti aliasing
                            var shadowMap = new WritableCubeMap(RenderTargetTextureTypes.Depth, new ImagePixelFormat(ColorFormat.Depth16), (int)ShadowMapRes, (int)ShadowMapRes, false, TextureFilterMode.Nearest, TextureWrapMode.ClampToBorder, TextureCompareMode.None, Compare.Less);
                            outParams = new ShadowParams() { ClipPlanesForLightMat = shadowParamClipPlanes, LightSpaceMatrices = lightSpaceMatrices, ShadowMap = shadowMap, Frustums = frustums };
                            break;
                        }
                    case LightType.Legacy:
                    case LightType.Parallel:
                        {
                            IWritableTexture shadowMap;
                            if (NumberOfCascades == 1)
                                shadowMap = new WritableTexture(RenderTargetTextureTypes.Depth, new ImagePixelFormat(ColorFormat.Depth24), (int)ShadowMapRes, (int)ShadowMapRes, false, TextureFilterMode.Nearest, TextureWrapMode.ClampToBorder, TextureCompareMode.CompareRefToTexture, Compare.Less);
                            else if (NumberOfCascades > 1)
                                shadowMap = new WritableArrayTexture(NumberOfCascades, RenderTargetTextureTypes.Depth, new ImagePixelFormat(ColorFormat.Depth16), (int)ShadowMapRes, (int)ShadowMapRes, false, TextureFilterMode.Nearest, TextureWrapMode.ClampToBorder, TextureCompareMode.CompareRefToTexture, Compare.Less);
                            else
                                throw new ArgumentException($"Number of shadow cascades is {NumberOfCascades} but must be greater or equal 1.");

                            outParams = new ShadowParams() { ClipPlanesForLightMat = shadowParamClipPlanes, LightSpaceMatrices = lightSpaceMatrices, ShadowMap = shadowMap, Frustums = frustums };
                            break;
                        }
                    case LightType.Spot:
                        {
                            var shadowMap = new WritableTexture(RenderTargetTextureTypes.Depth, new ImagePixelFormat(ColorFormat.Depth16), (int)ShadowMapRes, (int)ShadowMapRes, false, TextureFilterMode.Nearest, TextureWrapMode.ClampToBorder, TextureCompareMode.CompareRefToTexture, Compare.Less);
                            outParams = new ShadowParams() { ClipPlanesForLightMat = shadowParamClipPlanes, LightSpaceMatrices = lightSpaceMatrices, ShadowMap = shadowMap, Frustums = frustums };
                            break;
                        }
                    default:
                        throw new ArgumentException("Invalid light type.");
                }

                _shadowparams.Add(lr.Light, outParams);
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
        public override void Render(RenderContext rc)
        {
            SetContext(rc);
            NotifyStateChanges();

            PrePassVisitor.PrePassTraverse(_sc);
            AccumulateLight();

            _rc.EnableDepthClamp();

            _canUseGeometryShaders = _rc.GetHardwareCapabilities(HardwareCapability.CanUseGeometryShaders) == 1U;

            if (PrePassVisitor.CameraPrepassResults.Count != 0)
            {
                var cams = PrePassVisitor.CameraPrepassResults.OrderBy(cam => cam.Camera.Layer);

                foreach (var cam in cams)
                {
                    if (cam.Camera.Active)
                    {
                        PerCamClear(cam);
                        NotifyCameraChanges(cam.Camera);
                        DoFrustumCulling = cam.Camera.FrustumCullingOn;
                        PerCamRender(cam, cam.Camera.RenderTexture);
                    }
                }

                //Reset Viewport and frustum culling bool in case we have another scene, rendered without a camera
                _rc.Viewport(0, 0, rc.DefaultState.CanvasWidth, rc.DefaultState.CanvasHeight);
                DoFrustumCulling = true;
            }
            else
            {
                RenderAllPasses(new float4(0, 0, _rc.ViewportWidth, _rc.ViewportHeight));
            }
        }

        private void PerCamRender(CameraResult cam, IWritableTexture? renderTex = null)
        {
            RenderLayer = cam.Camera.RenderLayer;
            _rc.View = cam.View;

            float4 viewport;

            if (renderTex != null)
            {
                _rc.Projection = cam.Camera.GetProjectionMat(renderTex.Width, renderTex.Height, out viewport);
            }
            else
            {
                var w = renderTex == null ? _rc.GetWindowWidth() : renderTex.Width;
                var h = renderTex == null ? _rc.GetWindowHeight() : renderTex.Height;
                _rc.Projection = cam.Camera.GetProjectionMat(w, h, out viewport);
            }

            RenderAllPasses(viewport, renderTex);


            // if we have a multisample texture we need to blt the result of our rendering to the result texture
            if (cam.Camera.RenderTexture is WritableMultisampleTexture wmt)
            {
                _rc.BlitMultisample2DTextureToTexture(wmt, wmt.InternalResultTexture);
            }
        }

        private void RenderAllPasses(float4 lightingPassViewport, IWritableTexture? renderTex = null)
        {
            var preRenderStateSet = _rc.CurrentRenderState.Copy(); //"Snapshot" of the current render states as they came from the user code.
            var preRenderLockedStates = new Dictionary<RenderState, KeyValuePair<bool, uint>>(_rc.LockedStates);
            _rc.SetRenderState(RenderState.AlphaBlendEnable, 0, true);

            if (_rc.ClearColor != _texClearColor)
                BackgroundColor = _rc.ClearColor;

            _rc.ClearColor = _texClearColor;

            //Pass 1: Geometry pass - render with current render states.
            _rc.Viewport(0, 0, (int)_gBufferRenderTarget.TextureResolution, (int)_gBufferRenderTarget.TextureResolution);
            RenderGeometryPass();

            //Shadow Map Passes: Renders the scene for each light that is casting shadows and creates the shadow map for it.
            _rc.Viewport(0, 0, (int)ShadowMapRes, (int)ShadowMapRes);

            //Cache state because rendering the shadow maps will change the state eventually.
            var doCulling = DoFrustumCulling;
            RenderShadowMaps();
            DoFrustumCulling = doCulling;

            //Undo all user-made and shadow pass related render state changes to be able to work on a "white sheet" from here on.
            _rc.UnlockAllRenderStates();
            _rc.SetRenderStateSet(new RenderStateSet());

            if (_ssaoOn)
                RenderSSAO();

            //Pass 4 & 5: FXAA and Lighting
            _currentPass = RenderPasses.Lighting;

            if (!FxaaOn)
            {
                _rc.Viewport((int)lightingPassViewport.x, (int)lightingPassViewport.y, (int)lightingPassViewport.z, (int)lightingPassViewport.w);
                RenderLightPasses(renderTex);
            }
            else
            {
                _rc.Viewport(0, 0, _lightedSceneTex.Width, _lightedSceneTex.Height);
                RenderLightPasses(_lightedSceneTex);

                //Post-Effect: FXAA
                _rc.Viewport((int)lightingPassViewport.x, (int)lightingPassViewport.y, (int)lightingPassViewport.z, (int)lightingPassViewport.w);
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
        private void RenderLightPasses(IWritableTexture? renderTex = null)
        {
            Guard.IsNotNull(_rc);

            _rc.SetRenderTarget(renderTex);
            _rc.Clear(ClearFlags.Color | ClearFlags.Depth);

            var lightPassCnt = 0;

            for (var i = 0; i < LightViseratorResults.Count; i++)
            {
                var isCastingShadows = false; //needed because Android and Web doesn't support geometry shaders.
                var lightVisRes = LightViseratorResults[i];

                if (!lightVisRes.Light.Active) continue;

                if (lightVisRes.Light.IsCastingShadows)
                {
                    isCastingShadows = true;

                    Guard.IsNotNull(_shadowparams);
                    if (!_shadowparams.TryGetValue(lightVisRes.Light, out var shadowParams)) continue;

                    //Create and/or choose correct shader effect
                    switch (lightVisRes.Light.Type)
                    {
                        case LightType.Point:
                            {
                                if (_canUseGeometryShaders)
                                {
                                    if (_lightingPassEffectPoint == null)
                                    {
                                        Guard.IsNotNull(_gBufferRenderTarget);
                                        _lightingPassEffectPoint = MakeEffect.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Light, _texClearColor, (WritableCubeMap?)shadowParams.ShadowMap);
                                        _rc.CreateShaderProgram(_lightingPassEffectPoint);
                                    }
                                    _lightingPassEffect = _lightingPassEffectPoint;
                                }
                                else //use no shadows material
                                {
                                    isCastingShadows = false;

                                    if (_lightingPassEffectNoShadow == null)
                                    {
                                        _lightingPassEffectNoShadow = MakeEffect.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Light, _texClearColor);
                                        _rc.CreateShaderProgram(_lightingPassEffectNoShadow);
                                    }
                                    _lightingPassEffect = _lightingPassEffectNoShadow;
                                }
                                break;
                            }
                        case LightType.Legacy:
                        case LightType.Parallel:
                            {
                                Guard.IsNotNull(_gBufferRenderTarget);

                                if (NumberOfCascades > 1)
                                {
                                    if (_lightingPassEffectCascaded == null)
                                    {
                                        _lightingPassEffectCascaded = MakeEffect.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Light, (WritableArrayTexture?)shadowParams?.ShadowMap, shadowParams?.ClipPlanesForLightMat, NumberOfCascades, _texClearColor);
                                        _rc.CreateShaderProgram(_lightingPassEffectCascaded);
                                    }
                                    _lightingPassEffect = _lightingPassEffectCascaded;
                                }
                                else
                                {
                                    if (_lightingPassEffectNoCascades == null)
                                    {
                                        _lightingPassEffectNoCascades = MakeEffect.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Light, _texClearColor, (WritableTexture?)shadowParams?.ShadowMap);
                                        _rc.CreateShaderProgram(_lightingPassEffectNoCascades);
                                    }
                                    _lightingPassEffect = _lightingPassEffectNoCascades;
                                }
                                break;
                            }
                        case LightType.Spot:
                            {
                                if (_lightingPassEffectOther == null)
                                {
                                    _lightingPassEffectOther = MakeEffect.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Light, _texClearColor, shadowParams.ShadowMap as WritableTexture);
                                    _rc.CreateShaderProgram(_lightingPassEffectOther);
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
                        _lightingPassEffectNoShadow = MakeEffect.DeferredLightingPassEffect(_gBufferRenderTarget, lightVisRes.Light, _texClearColor);
                        _rc.CreateShaderProgram(_lightingPassEffectNoShadow);
                    }
                    _lightingPassEffect = _lightingPassEffectNoShadow;
                }

                UpdateLightAndShadowParams(lightVisRes, _lightingPassEffect, isCastingShadows);
                _lightingPassEffect.SetFxParam(UniformNameDeclarations.RenderPassNoHash, lightPassCnt);

                if (_needToSetSSAOTex)
                {
                    _lightingPassEffect.SetFxParam(UniformNameDeclarations.SsaoOnHash, _ssaoOn ? 1 : 0);
                    _needToSetSSAOTex = false;
                }

                //Set background color only in last light pass to NOT blend the color (additive).
                var isNextLightLast = i + 1 == LightViseratorResults.Count - 1;
                if (i == LightViseratorResults.Count - 1 ||
                   (isNextLightLast && !LightViseratorResults[i + 1].Light.Active) ||
                   (isNextLightLast && !LightViseratorResults[i + 1].Light.IsCastingShadows) ||
                   (isNextLightLast && lightVisRes.Light.Type == LightType.Point && !_canUseGeometryShaders))
                    _lightingPassEffect.SetFxParam(UniformNameDeclarations.BackgroundColorHash, BackgroundColor);
                else
                    _lightingPassEffect.SetFxParam(UniformNameDeclarations.BackgroundColorHash, _texClearColor);
                _rc.SetEffect(_lightingPassEffect);
                _rc.Render(_quad);
                lightPassCnt++;
            }
        }

        private void RenderShadowMaps()
        {
            Guard.IsNotNull(_rc);

            _currentPass = RenderPasses.Shadow;
            if (_shadowEffect == null)
            {
                _shadowEffect = MakeEffect.ShadowMapEffect();
                _rc.CreateShaderProgram(_shadowEffect);
            }

            foreach (var lightVisRes in LightViseratorResults)
            {
                if (!lightVisRes.ReRenderShadowMap ||
                    !lightVisRes.Light.IsCastingShadows ||
                    !lightVisRes.Light.Active ||
                    (lightVisRes.Light.Type == LightType.Point && !_canUseGeometryShaders)) continue;

                var shadowParams = CreateShadowParams(lightVisRes);
                _currentLightType = lightVisRes.Light.Type;

                switch (_currentLightType)
                {
                    case LightType.Point:
                        {
                            DoFrustumCulling = false;

                            if (_shadowCubeMapEffect == null)
                            {
                                _shadowCubeMapEffect = MakeEffect.ShadowCubeMapEffect(shadowParams.LightSpaceMatrices);
                                _rc.CreateShaderProgram(_shadowCubeMapEffect);
                            }
                            else
                                _shadowCubeMapEffect.SetFxParam(UniformNameDeclarations.LightSpaceMatricesHash, shadowParams.LightSpaceMatrices);

                            if (_shadowCubeMapEffectPointPrimitives == null)
                            {
                                _shadowCubeMapEffectPointPrimitives = MakeEffect.ShadowCubeMapEffectPointPrimitives(shadowParams.LightSpaceMatrices);
                                _rc.CreateShaderProgram(_shadowCubeMapEffectPointPrimitives);
                            }
                            else
                                _shadowCubeMapEffectPointPrimitives.SetFxParam(UniformNameDeclarations.LightSpaceMatricesHash, shadowParams.LightSpaceMatrices);

                            _shadowCubeMapEffect.SetFxParam(UniformNameDeclarations.LightMatClipPlanesHash, shadowParams.ClipPlanesForLightMat[0]);
                            _shadowCubeMapEffect.SetFxParam(UniformNameDeclarations.LightPosHash, lightVisRes.WorldSpacePos);

                            _shadowCubeMapEffectPointPrimitives.SetFxParam(UniformNameDeclarations.LightMatClipPlanesHash, shadowParams.ClipPlanesForLightMat[0]);
                            _shadowCubeMapEffectPointPrimitives.SetFxParam(UniformNameDeclarations.LightPosHash, lightVisRes.WorldSpacePos);
                            _rc.SetEffect(_shadowCubeMapEffect);

                            Guard.IsNotNull(shadowParams.ShadowMap);
                            _rc.SetRenderTarget((IWritableCubeMap)shadowParams.ShadowMap);
                            _rc.Clear(ClearFlags.Color | ClearFlags.Depth);
                            //No culling here because much of the work is done in the geometry shader.

                            Traverse(_sc.Children);
                            break;
                        }
                    case LightType.Legacy:
                    case LightType.Parallel:
                        {
                            if (NumberOfCascades == 1)
                            {
                                _shadowEffect.SetFxParam(UniformNameDeclarations.LightSpaceMatrixHash, shadowParams.LightSpaceMatrices[0]);
                                _rc.SetEffect(_shadowEffect);
                                _rc.SetRenderTarget(shadowParams.ShadowMap);
                                _rc.Clear(ClearFlags.Color | ClearFlags.Depth);

                                Guard.IsNotNull(shadowParams);
                                _lightFrustum = shadowParams.Frustums?[0];

                                Traverse(_sc.Children);
                            }
                            else if (NumberOfCascades > 1)
                            {
                                for (int i = 0; i < NumberOfCascades; i++)
                                {
                                    _shadowEffect.SetFxParam(UniformNameDeclarations.LightSpaceMatrixHash, shadowParams.LightSpaceMatrices[i]);
                                    _rc.SetEffect(_shadowEffect);
                                    _rc.SetRenderTarget((IWritableArrayTexture)shadowParams.ShadowMap, i);
                                    _rc.Clear(ClearFlags.Color | ClearFlags.Depth);

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
                            DoFrustumCulling = true;
                            _shadowEffect.SetFxParam(UniformNameDeclarations.LightSpaceMatrixHash, shadowParams.LightSpaceMatrices[0]);
                            _rc.SetEffect(_shadowEffect);
                            _rc.SetRenderTarget(shadowParams.ShadowMap);
                            _rc.Clear(ClearFlags.Color | ClearFlags.Depth);

                            _lightFrustum = shadowParams.Frustums[0];
                            Traverse(_sc.Children);
                            break;
                        }

                    default:
                        break;
                }
                if (lightVisRes.Light.Type != LightType.Legacy && lightVisRes.Light.Type != LightType.Parallel && NumberOfCascades > 1)
                    lightVisRes.ReRenderShadowMap = false;
            }
        }

        private void RenderGeometryPass()
        {
            _currentPass = RenderPasses.Geometry;
            _rc.SetRenderTarget(_gBufferRenderTarget);
            _rc.Clear(ClearFlags.Color | ClearFlags.Depth);
            Traverse(_sc.Children);
        }

        private void RenderSSAO()
        {
            //Pass 2: SSAO
            _currentPass = RenderPasses.Ssao;
            if (_ssaoTexEffect == null)
            {
                _ssaoTexEffect = MakeEffect.SSAORenderTargetTextureEffect(_gBufferRenderTarget, 64, new int2((int)TexRes, (int)TexRes), 4);
                _rc.CreateShaderProgram(_ssaoTexEffect);
            }
            _rc.SetEffect(_ssaoTexEffect);
            _rc.SetRenderTarget(_ssaoRenderTexture);
            _rc.Clear(ClearFlags.Color | ClearFlags.Depth);
            _rc.Render(_quad);

            //Pass 3: Blur SSAO Texture
            _currentPass = RenderPasses.SsaoBlur;
            if (_blurEffect == null)
            {
                _blurEffect = MakeEffect.SSAORenderTargetBlurEffect(_ssaoRenderTexture);
                _rc.CreateShaderProgram(_blurEffect);
            }
            _rc.SetEffect(_blurEffect);
            _rc.SetRenderTarget(_blurRenderTex);
            _rc.Clear(ClearFlags.Color | ClearFlags.Depth);
            _rc.Render(_quad);

            //Set blurred SSAO Texture as SSAO Texture in gBuffer
            _gBufferRenderTarget.SetTexture(_blurRenderTex, RenderTargetTextureTypes.Ssao);
        }

        private void RenderFXAA(IWritableTexture? renderTex = null)
        {
            _currentPass = RenderPasses.Fxaa;
            if (_fxaaEffect == null)
            {
                _fxaaEffect = MakeEffect.FXAARenderTargetEffect(_lightedSceneTex, new int2((int)TexRes, (int)TexRes));
                _rc.CreateShaderProgram(_fxaaEffect);
            }
            _rc.SetEffect(_fxaaEffect);

            if (renderTex == null)
                _rc.SetRenderTarget();
            else
                _rc.SetRenderTarget(renderTex);
            _rc.Clear(ClearFlags.Color | ClearFlags.Depth);
            _rc.Render(_quad);
        }

        private void UpdateLightAndShadowParams(LightResult lightVisRes, ShaderEffect effect, bool isCastingShadows)
        {
            var light = lightVisRes.Light;

            if (light.Type == LightType.Legacy)
            {
                lightVisRes.Rotation = new float4x4
                (
                    new float4(_rc.InvView.Row1.xyz, 0),
                    new float4(_rc.InvView.Row2.xyz, 0),
                    new float4(_rc.InvView.Row3.xyz, 0),
                    float4.UnitW
                 );
            }

            var dirWorldSpace = float3.Normalize((lightVisRes.Rotation * float4.UnitZ).xyz);
            var dirViewSpace = float3.Normalize((_rc.View * new float4(dirWorldSpace)).xyz);
            var strength = light.Strength;

            if (strength > 1.0 || strength < 0.0)
            {
                strength = M.Clamp(light.Strength, 0.0f, 1.0f);
                Diagnostics.Warn("Strength of the light will be clamped between 0 and 1.");
            }

            // Set params in modelview space since the lightning calculation is in modelview space
            switch (lightVisRes.Light.Type)
            {
                case LightType.Point:
                    effect.SetFxParam("light.position", _rc.View * lightVisRes.WorldSpacePos);
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
                    effect.SetFxParam("light.position", _rc.View * lightVisRes.WorldSpacePos);
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
                var shadowParams = _shadowparams[lightVisRes.Light];

                switch (lightVisRes.Light.Type)
                {
                    case LightType.Point:
                        effect.SetFxParam(UniformNameDeclarations.ShadowCubeMapHash, (WritableCubeMap)shadowParams.ShadowMap);
                        break;
                    case LightType.Legacy:
                    case LightType.Parallel:
                        if (NumberOfCascades > 1)
                        {
                            effect.SetFxParam(UniformNameDeclarations.ShadowMapHash, shadowParams.ShadowMap);
                            effect.SetFxParam($"{UniformNameDeclarations.LightMatClipPlanes}[0]", shadowParams.ClipPlanesForLightMat);
                            effect.SetFxParam($"{UniformNameDeclarations.LightSpaceMatrices}[0]", shadowParams.LightSpaceMatrices);
                        }
                        else
                        {
                            effect.SetFxParam(UniformNameDeclarations.LightSpaceMatrixHash, shadowParams.LightSpaceMatrices[0]);
                            effect.SetFxParam(UniformNameDeclarations.ShadowMapHash, (WritableTexture)shadowParams.ShadowMap);
                        }
                        break;
                    case LightType.Spot:
                        effect.SetFxParam(UniformNameDeclarations.LightSpaceMatrixHash, shadowParams.LightSpaceMatrices[0]);
                        effect.SetFxParam(UniformNameDeclarations.ShadowMapHash, (WritableTexture)shadowParams.ShadowMap);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        /// <summary>
        /// Sets the initial values in the <see cref="RendererState"/>.
        /// </summary>
        protected override void InitState()
        {
            _state.Clear();
            _state.Model = float4x4.Identity;
            _state.CanvasXForm = float4x4.Identity;
            _state.UiRect = new MinMaxRect { Min = -float2.One, Max = float2.One };
            _state.Effect = _rc.DefaultEffect;
            _rc.CreateShaderProgram(_state.Effect, false);
            _state.RenderLayer = new RenderLayer();
        }

        /// <summary>
        /// Sets the render context for the given scene.
        /// </summary>
        /// <param name="rc"></param>
        public override void SetContext(RenderContext rc)
        {
            if (rc == null)
                throw new ArgumentNullException(nameof(rc));

            if (rc != _rc)
            {
                _rc = rc;

                foreach (var module in VisitorModules)
                {
                    ((IRendererModule)module).UpdateContext(_rc);
                }

                InitRenderTextures();
                InitState();
            }
        }

        private void InitRenderTextures()
        {
            _gBufferRenderTarget = _rc.CreateGBufferTarget(TexRes);

            _ssaoRenderTexture = new WritableTexture(RenderTargetTextureTypes.Ssao, new ImagePixelFormat(ColorFormat.RGB/*fRGB16*/), (int)TexRes, (int)TexRes, false, TextureFilterMode.Nearest);
            _blurRenderTex = new WritableTexture(RenderTargetTextureTypes.Ssao, new ImagePixelFormat(ColorFormat.RGB/*fRGB16*/), (int)TexRes, (int)TexRes, false, TextureFilterMode.Nearest);
            _lightedSceneTex = new WritableTexture(RenderTargetTextureTypes.Albedo, new ImagePixelFormat(ColorFormat.RGB), (int)TexRes, (int)TexRes, false, TextureFilterMode.Linear);

            _gBufferRenderTarget.DeleteBuffers += DeleteBuffers;
        }

        private void DeleteBuffers(object? sender, EventArgs e)
        {
            Guard.IsNotNull(_rc);
            Guard.IsNotNull(sender);
            var rt = (IRenderTarget)sender;
            if (rt.GBufferHandle != null)
                _rc.DeleteFrameBuffer(rt.GBufferHandle);
            if (rt.DepthBufferHandle != null)
                _rc.DeleteRenderBuffer(rt.DepthBufferHandle);
        }
    }
}