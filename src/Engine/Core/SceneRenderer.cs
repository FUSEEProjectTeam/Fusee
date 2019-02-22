using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using Fusee.Xirkit;


namespace Fusee.Engine.Core
{
    /// <summary>
    /// Axis-Aligned Bounding Box Calculator. Use instances of this class to calculate axis-aligned bounding boxes
    /// on scenes, list of scene nodes or individual scene nodes. Calculations always include any child nodes.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class AABBCalculator : SceneVisitor
    {
        // ReSharper disable once InconsistentNaming
        public class AABBState : VisitorState
        {
            private readonly CollapsingStateStack<float4x4> _modelView = new CollapsingStateStack<float4x4>();

            public float4x4 ModelView
            {
                set { _modelView.Tos = value; }
                get { return _modelView.Tos; }
            }

            public AABBState()
            {
                RegisterState(_modelView);
            }
        }

        //private SceneContainer _sc;
        private IEnumerable<SceneNodeContainer> _sncList;
        private AABBState _state = new AABBState();
        private bool _boxValid;
        private AABBf _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="AABBCalculator"/> class.
        /// </summary>
        /// <param name="sc">The scene container to calculate an axis-aligned bounding box for.</param>
        public AABBCalculator(SceneContainer sc)
        {
            _sncList = sc.Children;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AABBCalculator"/> class.
        /// </summary>
        /// <param name="sncList">The list of scene nodes to calculate an axis-aligned bounding box for.</param>
        public AABBCalculator(IEnumerable<SceneNodeContainer> sncList)
        {
            _sncList = sncList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AABBCalculator"/> class.
        /// </summary>
        /// <param name="snc">A single scene node to calculate an axis-aligned bounding box for.</param>
        public AABBCalculator(SceneNodeContainer snc)
        {
            _sncList = SceneVisitorHelpers.SingleRootEnumerable(snc);
        }

        /// <summary>
        /// Performs the calculation and returns the resulting box on the object(s) passed in the constructor. Any calculation
        /// always includes a full traversal over all child nodes.
        /// </summary>
        /// <returns>The resulting axis-aligned bounding box.</returns>
        public AABBf? GetBox()
        {
            Traverse(_sncList);
            if (_boxValid)
                return _result;
            return null;
        }

        #region Visitors

        /// <summary>
        /// Do not call. Used for internal traversal purposes only
        /// </summary>
        /// <param name="transform">The transform component.</param>
        [VisitMethod]
        public void OnTransform(TransformComponent transform)
        {
            _state.ModelView *= transform.Matrix();
        }

        /// <summary>
        /// Do not call. Used for internal traversal purposes only
        /// </summary>
        /// <param name="mesh">The mesh component.</param>
        [VisitMethod]
        public void OnMesh(Mesh mesh)
        {
            var box = _state.ModelView * mesh.BoundingBox;
            if (!_boxValid)
            {
                _result = box;
                _boxValid = true;
            }
            else
            {
                _result = AABBf.Union(_result, box);
            }
        }

        #endregion

        #region HierarchyLevel

        protected override void InitState()
        {
            _boxValid = false;
            _state.Clear();
            _state.ModelView = float4x4.Identity;
        }

        protected override void PushState()
        {
            _state.Push();
        }

        protected override void PopState()
        {
            _state.Pop();
        }

        #endregion
    }

    /// <summary>
    /// All supported lightning calculation methods LegacyShaderCodeBuilder.cs supports.
    /// </summary>
    // ReSharper disable InconsistentNaming
    public enum LightingCalculationMethod
    {
        /// <summary> 
        /// Simple Blinn Phong Shading without fresnel & distribution function
        /// </summary>
        SIMPLE,

        /// <summary>
        /// Physical based shading
        /// </summary>
        ADVANCED,

        /// <summary>
        /// Physical based shading with environment cube map algorithm
        /// </summary>
        ADVANCEDwENVMAP
    }

    /// <summary>
    /// Use a Scene Renderer to traverse a scene hierarchy (made out of scene nodes and components) in order
    /// to have each visited element contribute to the result rendered against a given render context.
    /// </summary>
    public class SceneRenderer : SceneVisitor
    {
        // All lights
        public static Dictionary<LightComponent, LightResult> AllLightResults = new Dictionary<LightComponent, LightResult>();
        // Multipass
        private bool _renderWithShadows;
        private bool _renderDeferred;
        private bool _renderEnvMap;
        private readonly bool _wantToRenderWithShadows;
        private readonly bool _wantToRenderDeferred;
        private readonly bool _wantToRenderEnvMap;
        public float2 ShadowMapSize { set; get; } = new float2(1024, 1024);

        private CanvasTransformComponent _ctc;
        private MinMaxRect _parentRect;

        /// <summary>
        /// Try to render with Shadows. If not possible, fallback to false.
        /// </summary>
        [Obsolete("Will be migrated to seperate SceneRenderer")]
        public bool DoRenderWithShadows
        {
            private set { _renderWithShadows = _rc.GetHardwareCapabilities(HardwareCapability.DefferedPossible) == 1U && value; }
            get { return _renderWithShadows; }
        }

        /// <summary>
        /// Try to render deferred. If not possible, fallback to false.
        /// </summary>
        [Obsolete("Will be migrated to seperate SceneRenderer")]
        public bool DoRenderDeferred
        {
            private set { _renderDeferred = _rc.GetHardwareCapabilities(HardwareCapability.DefferedPossible) == 1U && value; }
            get { return _renderDeferred; }
        }

        /// <summary>
        /// Try to render with EM. If not possible, fallback to false.
        /// </summary>
        [Obsolete("Will be migrated to seperate SceneRenderer")]
        public bool DoRenderEnvMap
        {
            private set { _renderEnvMap = _rc.GetHardwareCapabilities(HardwareCapability.DefferedPossible) == 1U && value; }
            get { return _renderEnvMap; }
        }

        #region Traversal information


        private Dictionary<SceneNodeContainer, float4x4> _boneMap;
        private Dictionary<ShaderComponent, ShaderEffect> _shaderEffectMap;
        private Animation _animation;
        private readonly SceneContainer _sc;

        private RenderContext _rc;

        private Dictionary<LightComponent, LightResult> _lightComponents = new Dictionary<LightComponent, LightResult>();

        private string _scenePathDirectory;
        private ShaderEffect _defaultEffect;

        #endregion

        #region State

        public class RendererState : VisitorState
        {
            private CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();
            private CollapsingStateStack<MinMaxRect> _uiRect = new CollapsingStateStack<MinMaxRect>();
            private CollapsingStateStack<float4x4> _canvasXForm = new CollapsingStateStack<float4x4>();

            public float4x4 Model
            {
                get { return _model.Tos; }
                set { _model.Tos = value; }
            }

            public MinMaxRect UiRect
            {
                get { return _uiRect.Tos; }
                set { _uiRect.Tos = value; }
            }

            public float4x4 CanvasXForm
            {
                get => _canvasXForm.Tos;
                set => _canvasXForm.Tos = value;
            }

            private StateStack<ShaderEffect> _effect = new StateStack<ShaderEffect>();

            public ShaderEffect Effect
            {
                set { _effect.Tos = value; }
                get { return _effect.Tos; }
            }

            public RendererState()
            {
                RegisterState(_model);
                RegisterState(_canvasXForm);
                RegisterState(_effect);
                RegisterState(_uiRect);
            }
        }

        private RendererState _state;
        private float4x4 _view;

        #endregion

        #region Initialization Construction Startup

        public SceneRenderer(SceneContainer sc, bool RenderDeferred = false, bool RenderShadows = false)
             : this(sc)
        {
            if (RenderShadows)
                _wantToRenderWithShadows = true;

            if (RenderDeferred)
                _wantToRenderDeferred = true;
        }

        public SceneRenderer(SceneContainer sc /*, string scenePathDirectory*/)
        {
            // accumulate all lights and...
            // NEEDED FOR JSIL; do not use .toDictonary(x => x.Values, x => x.Keys)
            var results = sc.Children.Viserate<LightSetup, KeyValuePair<LightComponent, LightResult>>();
            LightResult result;
            foreach (var keyValuePair in results)
            {
                if (_lightComponents.TryGetValue(keyValuePair.Key, out result)) continue;
                _lightComponents.Add(keyValuePair.Key, keyValuePair.Value);
            }
            // ...set them
            AllLightResults = _lightComponents;

            if (AllLightResults.Count == 0)
            {
                // if there is no light in scene then add one (legacyMode)
                AllLightResults.Add(new LightComponent(), new LightResult
                {
                    PositionWorldSpace = float3.UnitZ,
                    Position = float3.UnitZ,
                    Active = true,
                    AmbientCoefficient = 0.0f,
                    Attenuation = 0.0f,
                    Color = new float3(1.0f, 1.0f, 1.0f),
                    ConeAngle = 45f,
                    ConeDirection = float3.UnitZ,
                    ModelMatrix = float4x4.Identity,
                    Type = (int)LightType.Legacy
                });
            }

            _sc = sc;
            // _scenePathDirectory = scenePathDirectory;
            _state = new RendererState();
            InitAnimations(_sc);
        }

        public void InitAnimations(SceneContainer sc)
        {
            _animation = new Animation();

            foreach (AnimationComponent ac in sc.Children.FindComponents<AnimationComponent>(c => true))
            {
                if (ac.AnimationTracks != null)
                {
                    foreach (AnimationTrackContainer animTrackContainer in ac.AnimationTracks)
                    {
                        // Type t = animTrackContainer.TypeId;
                        switch (animTrackContainer.TypeId)
                        {
                            // if (typeof(int).IsAssignableFrom(t))
                            case TypeId.Int:
                                {
                                    Channel<int> channel = new Channel<int>(Lerp.IntLerp);
                                    foreach (AnimationKeyContainerInt key in animTrackContainer.KeyFrames)
                                    {
                                        channel.AddKeyframe(new Keyframe<int>(key.Time, key.Value));
                                    }
                                    _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                        animTrackContainer.Property);
                                }
                                break;
                            //else if (typeof(float).IsAssignableFrom(t))
                            case TypeId.Float:
                                {
                                    Channel<float> channel = new Channel<float>(Lerp.FloatLerp);
                                    foreach (AnimationKeyContainerFloat key in animTrackContainer.KeyFrames)
                                    {
                                        channel.AddKeyframe(new Keyframe<float>(key.Time, key.Value));
                                    }
                                    _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                        animTrackContainer.Property);
                                }
                                break;

                            // else if (typeof(float2).IsAssignableFrom(t))
                            case TypeId.Float2:
                                {
                                    Channel<float2> channel = new Channel<float2>(Lerp.Float2Lerp);
                                    foreach (AnimationKeyContainerFloat2 key in animTrackContainer.KeyFrames)
                                    {
                                        channel.AddKeyframe(new Keyframe<float2>(key.Time, key.Value));
                                    }
                                    _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                        animTrackContainer.Property);
                                }
                                break;
                            // else if (typeof(float3).IsAssignableFrom(t))
                            case TypeId.Float3:
                                {
                                    Channel<float3>.LerpFunc lerpFunc;
                                    switch (animTrackContainer.LerpType)
                                    {
                                        case LerpType.Lerp:
                                            lerpFunc = Lerp.Float3Lerp;
                                            break;
                                        case LerpType.Slerp:
                                            lerpFunc = Lerp.Float3QuaternionSlerp;
                                            break;
                                        default:
                                            // C# 6throw new InvalidEnumArgumentException(nameof(animTrackContainer.LerpType), (int)animTrackContainer.LerpType, typeof(LerpType));
                                            // throw new InvalidEnumArgumentException("animTrackContainer.LerpType", (int)animTrackContainer.LerpType, typeof(LerpType));
                                            throw new InvalidOperationException(
                                                "Unknown lerp type: animTrackContainer.LerpType: " +
                                                (int)animTrackContainer.LerpType);
                                    }
                                    Channel<float3> channel = new Channel<float3>(lerpFunc);
                                    foreach (AnimationKeyContainerFloat3 key in animTrackContainer.KeyFrames)
                                    {
                                        channel.AddKeyframe(new Keyframe<float3>(key.Time, key.Value));
                                    }
                                    _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                        animTrackContainer.Property);
                                }
                                break;
                            // else if (typeof(float4).IsAssignableFrom(t))
                            case TypeId.Float4:
                                {
                                    Channel<float4> channel = new Channel<float4>(Lerp.Float4Lerp);
                                    foreach (AnimationKeyContainerFloat4 key in animTrackContainer.KeyFrames)
                                    {
                                        channel.AddKeyframe(new Keyframe<float4>(key.Time, key.Value));
                                    }
                                    _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                        animTrackContainer.Property);
                                }
                                break;
                                //TODO : Add cases for each type
                        }
                    }
                }
            }
        }

        public void Animate()
        {
            if (_animation.ChannelBaseList.Count != 0)
            {
                // Set the animation time here!
                _animation.Animate(Time.DeltaTime);
            }
        }


        public void SetContext(RenderContext rc)
        {
            if (rc == null)
                throw new ArgumentNullException("rc");

            if (rc != _rc)
            {
                _rc = rc;
                _boneMap = new Dictionary<SceneNodeContainer, float4x4>();
                _shaderEffectMap = new Dictionary<ShaderComponent, ShaderEffect>();
                var defaultMat = new MaterialComponent
                {
                    Diffuse = new MatChannelContainer
                    {
                        Color = new float3(0.5f, 0.5f, 0.5f)
                    },
                    Specular = new SpecularChannelContainer
                    {
                        Color = new float3(1, 1, 1),
                        Intensity = 0.5f,
                        Shininess = 22
                    }
                };
                _defaultEffect = ShaderCodeBuilder.MakeShaderEffectFromMatComp(defaultMat);

                //_defaultEffect.AttachToContext(_rc);
                _rc.SetShaderEffect(_defaultEffect);

                // Check for hardware capabilities:
                DoRenderDeferred = _wantToRenderDeferred;
                DoRenderWithShadows = _wantToRenderWithShadows;
                DoRenderEnvMap = _wantToRenderEnvMap;
            }
        }
        #endregion

        public void Render(RenderContext rc)
        {
            SetContext(rc);

            rc.SetRenderTarget(null);
            Traverse(_sc.Children);
        }


        #region Visitors

        [VisitMethod]
        public void RenderBone(BoneComponent bone)
        {
            SceneNodeContainer boneContainer = CurrentNode;
            float4x4 transform;
            if (!_boneMap.TryGetValue(boneContainer, out transform))
                _boneMap.Add(boneContainer, _rc.Model);
            else
                _boneMap[boneContainer] = _rc.Model;
        }

        [VisitMethod]
        public void RenderWeight(WeightComponent weight)
        {
            float4x4[] boneArray = new float4x4[weight.Joints.Count()];
            for (int i = 0; i < weight.Joints.Count(); i++)
            {
                float4x4 tmp = weight.BindingMatrices[i];
                boneArray[i] = _boneMap[weight.Joints[i]] * tmp;
            }
            _rc.Bones = boneArray;
        }

        [VisitMethod]
        public void RenderCanvasTransform(CanvasTransformComponent ctc)
        {
            _ctc = ctc;
            _rc.View = _view;

            if (ctc.CanvasRenderMode == CanvasRenderMode.WORLD)
            {
                var newRect = new MinMaxRect
                {
                    Min = ctc.Size.Min,
                    Max = ctc.Size.Max
                };

                _state.CanvasXForm *= float4x4.CreateTranslation(newRect.Center.x, newRect.Center.y, 0);
                _state.Model *= _state.CanvasXForm;

                _parentRect = newRect;
                _state.UiRect = newRect;                
            }

            if (ctc.CanvasRenderMode == CanvasRenderMode.SCREEN)
            {
                var projection = _rc.Projection;
                var zNear = System.Math.Abs(projection.M34 / (projection.M33 + 1));

                var fov = 2f * System.Math.Atan(1f / projection.M22);
                var aspect = projection.M22 / projection.M11;

                var canvasPos = new float3(_rc.InvView.M14, _rc.InvView.M24, _rc.InvView.M34 + zNear);

                var height = (float)(2f * System.Math.Tan(fov / 2f) * zNear);
                var width = height * aspect;

                ctc.ScreenSpaceSize = new MinMaxRect
                {
                    Min = new float2(canvasPos.x - width / 2, canvasPos.y - height / 2),
                    Max = new float2(canvasPos.x + width / 2, canvasPos.y + height / 2)
                };

                ctc.Scale = new float2(ctc.Size.Size.x / ctc.ScreenSpaceSize.Size.x, ctc.Size.Size.y / ctc.ScreenSpaceSize.Size.y);                

                var newRect = new MinMaxRect
                {
                    Min = ctc.ScreenSpaceSize.Min,
                    Max = ctc.ScreenSpaceSize.Max
                };

                _ctc = ctc;
                _state.CanvasXForm *= _rc.InvView * float4x4.CreateTranslation(0, 0, zNear + (zNear*0.01f));
                _state.Model *= _state.CanvasXForm;

                _parentRect = newRect;
                _state.UiRect = newRect;
            }
        }

        [VisitMethod]
        public void RenderRectTransform(RectTransformComponent rtc)
        {
            MinMaxRect newRect;
            if (_ctc.CanvasRenderMode == CanvasRenderMode.SCREEN)
            {
                newRect = new MinMaxRect
                {
                    Min = _state.UiRect.Min + _state.UiRect.Size * rtc.Anchors.Min + (rtc.Offsets.Min / _ctc.Scale.x),
                    Max = _state.UiRect.Min + _state.UiRect.Size * rtc.Anchors.Max + (rtc.Offsets.Max / _ctc.Scale.y)
                };
            }
            else
            {
                // The Heart of the UiRect calculation: Set anchor points relative to parent
                // rectangle and add absolute offsets
                newRect = new MinMaxRect
                {
                    Min = _state.UiRect.Min + _state.UiRect.Size * rtc.Anchors.Min + rtc.Offsets.Min,
                    Max = _state.UiRect.Min + _state.UiRect.Size * rtc.Anchors.Max + rtc.Offsets.Max
                };
            }

            var translationDelta = newRect.Center - _state.UiRect.Center;
            var translationX = translationDelta.x / _state.UiRect.Size.x;
            var translationY = translationDelta.y / _state.UiRect.Size.y;
            
            _parentRect = _state.UiRect;
            _state.UiRect = newRect;

            _state.Model *= float4x4.CreateTranslation(translationX, translationY, 0); 
        }

        [VisitMethod]
        public void RenderXForm(XFormComponent xfc)
        {
            float4x4 scale;

            if (_state.UiRect.Size != _parentRect.Size)
            {
                var scaleX = _state.UiRect.Size.x / _parentRect.Size.x;
                var scaleY = _state.UiRect.Size.y / _parentRect.Size.y;
                scale = float4x4.CreateScale(scaleX, scaleY, 1);
            }
            else
                scale = float4x4.CreateScale(_state.UiRect.Size.x, _state.UiRect.Size.y, 1);

            _state.Model *= scale;
            _rc.Model = _state.Model;
            _rc.View = _view;
        }

        [VisitMethod]
        public void RenderXFormText(XFormTextComponent xfc)
        {
            var scaleX = 1 / _state.UiRect.Size.x * xfc.TextScaleFactor;
            var scaleY = 1/ _state.UiRect.Size.y * xfc.TextScaleFactor;
            var scale = float4x4.CreateScale(scaleX, scaleY, 1);

            _state.Model *= scale;
            _rc.Model = _state.Model;
            _rc.View = _view;
        }

        [VisitMethod]
        public void RenderTransform(TransformComponent transform)
        {
            _state.Model *= transform.Matrix();
            _rc.Model = _state.Model;
            _rc.View = _view;
            // CM 3.5.17 _rc.ModelView = _view * _state.Model; // Changed from Model to ModelView
        }

        [VisitMethod]
        public void RenderShader(ShaderComponent shaderComponent)
        {
            var effect = BuildMaterialFromShaderComponent(shaderComponent);
            _state.Effect = effect;
        }


        [VisitMethod]
        public void RenderShaderEffect(ShaderEffectComponent shaderComponent)
        {
            _state.Effect = shaderComponent.Effect;
            _rc.SetShaderEffect(shaderComponent.Effect);
        }

        [VisitMethod]
        public void RenderMesh(Mesh mesh)
        {
            Mesh rm = mesh;
            WeightComponent wc = CurrentNode.GetWeights();
            if (wc != null)
                AddWeightComponentToMesh(mesh, wc);

            RenderCurrentPass(rm, _state.Effect);
        }

        [VisitMethod]
        public void AccumulateLight(LightComponent lightComponent)
        {
            LightResult result;
            if (AllLightResults.TryGetValue(lightComponent, out result)) return;

            // chache miss
            // accumulate all lights and...
            // NEEDED FOR JSIL; do not use .toDictonary(x => x.Values, x => x.Keys)
            var results = _sc.Children.Viserate<LightSetup, KeyValuePair<LightComponent, LightResult>>();

            foreach (var keyValuePair in results)
            {
                if (_lightComponents.TryGetValue(keyValuePair.Key, out result)) continue;
                _lightComponents.Add(keyValuePair.Key, keyValuePair.Value);
            }
            // _lightComponents = _sc.Children.Viserate<LightSetup, KeyValuePair<LightComponent, LightResult>>().ToDictionary(result => result.Key, result => result.Value);
            // ...set them
            AllLightResults = _lightComponents;
            // and multiply them with current modelview matrix
            // normalize etc.
            LightsToModelViewSpace();

        }

        private void AddWeightComponentToMesh(Mesh mesh, WeightComponent wc)
        {
            float4[] boneWeights = new float4[wc.WeightMap.Count];
            float4[] boneIndices = new float4[wc.WeightMap.Count];

            // Iterate over the vertices
            for (int iVert = 0; iVert < wc.WeightMap.Count; iVert++)
            {
                VertexWeightList vwl = wc.WeightMap[iVert];

                // Security guard. Sometimes a vertex has no weight. This should be fixed in the model. But
                // let's just not crash here. Instead of having a completely unweighted vertex, bind it to
                // the root bone (index 0).
                if (vwl == null)
                    vwl = new VertexWeightList();
                if (vwl.VertexWeights == null)
                    vwl.VertexWeights =
                        new List<VertexWeight>(new[] { new VertexWeight { JointIndex = 0, Weight = 1.0f } });
                int nJoints = System.Math.Min(4, vwl.VertexWeights.Count);
                for (int iJoint = 0; iJoint < nJoints; iJoint++)
                {
                    // boneWeights[iVert][iJoint] = vwl.VertexWeights[iJoint].Weight;
                    // boneIndices[iVert][iJoint] = vwl.VertexWeights[iJoint].JointIndex;
                    // JSIL cannot handle float4 indexer. Map [0..3] to [x..z] by hand
                    switch (iJoint)
                    {
                        case 0:
                            boneWeights[iVert].x = vwl.VertexWeights[iJoint].Weight;
                            boneIndices[iVert].x = vwl.VertexWeights[iJoint].JointIndex;
                            break;
                        case 1:
                            boneWeights[iVert].y = vwl.VertexWeights[iJoint].Weight;
                            boneIndices[iVert].y = vwl.VertexWeights[iJoint].JointIndex;
                            break;
                        case 2:
                            boneWeights[iVert].z = vwl.VertexWeights[iJoint].Weight;
                            boneIndices[iVert].z = vwl.VertexWeights[iJoint].JointIndex;
                            break;
                        case 3:
                            boneWeights[iVert].w = vwl.VertexWeights[iJoint].Weight;
                            boneIndices[iVert].w = vwl.VertexWeights[iJoint].JointIndex;
                            break;
                    }
                }
                boneWeights[iVert].Normalize1();
            }

            mesh.BoneIndices = boneIndices;
            mesh.BoneWeights = boneWeights;
        }

        private void LightsToModelViewSpace()
        {
            // Add ModelView Matrix to all lights
            foreach (var key in AllLightResults.Keys.ToList())
            {
                var light = AllLightResults[key];

                // Multiply LightPosition with modelview
                light.PositionModelViewSpace = _rc.ModelView * light.PositionWorldSpace;

                // float4 is really needed
                var lightConeDirectionFloat4 = new float4(light.ConeDirection.x, light.ConeDirection.y, light.ConeDirection.z,
                                          0.0f);
                lightConeDirectionFloat4 = _rc.ModelView * lightConeDirectionFloat4;
                lightConeDirectionFloat4.Normalize();
                light.ConeDirectionModelViewSpace = new float3(lightConeDirectionFloat4.x, lightConeDirectionFloat4.y, lightConeDirectionFloat4.z);

                // convert spotlight angle from degrees to radians
                light.ConeAngle = M.DegreesToRadians(light.ConeAngle);
                AllLightResults[key] = light;
            }
        }

        #endregion

        #region HierarchyLevel

        protected override void InitState()
        {
            _state.Clear();
            _state.Model = float4x4.Identity;
            _state.CanvasXForm = float4x4.Identity;
            _state.UiRect = new MinMaxRect { Min = -float2.One, Max = float2.One };
            _state.Effect = _defaultEffect;

            _view = _rc.ModelView;
        }

        protected override void PushState()
        {
            _state.Push();
        }

        protected override void PopState()
        {
            _state.Pop();
            _rc.Model = _state.Model;
            _rc.View = _view;
            //CM 3.5.17 _rc.ModelView = _view * _state.Model;
        }

        #endregion

        public void RenderCurrentPass(Mesh rm, ShaderEffect effect)
        {
            for (var i = 0; i < _lightComponents.Keys.Count; i++)
            {
                _rc.SetShaderEffect(effect);
                UpdateLightParamsInPixelShader(i, _lightComponents[_lightComponents.Keys.ElementAt(i)], effect);
                _rc.Render(rm);
            }

        }

        private void UpdateLightParamsInPixelShader(int position, LightResult light, ShaderEffect effect)
        {
            if (!light.Active) return;

            // Set params in modelview space since the lightning calculation is in modelview space

            _rc.SetFXParam($"allLights[{position}].position", light.PositionModelViewSpace);
            _rc.SetFXParam($"allLights[{position}].intensities", light.Color);
            _rc.SetFXParam($"allLights[{position}].attenuation", light.Attenuation);
            _rc.SetFXParam($"allLights[{position}].ambientCoefficient", light.AmbientCoefficient);
            _rc.SetFXParam($"allLights[{position}].coneAngle", light.ConeAngle);
            _rc.SetFXParam($"allLights[{position}].coneDirection", light.ConeDirectionModelViewSpace);
            _rc.SetFXParam($"allLights[{position}].lightType", light.Type);
        }

        #region RenderContext/Asset Setup

        private ShaderEffect BuildMaterialFromShaderComponent(ShaderComponent shaderComponent)
        {
            ShaderEffect shaderEffect;
            if (_shaderEffectMap.TryGetValue(shaderComponent, out shaderEffect)) return shaderEffect;

            shaderEffect = MakeShader(shaderComponent);
            _rc.SetShaderEffect(shaderEffect);
            //shaderEffect.AttachToContext(_rc);
            _shaderEffectMap.Add(shaderComponent, shaderEffect);
            return shaderEffect;
        }


        // Creates Shader from given shaderComponent
        private static ShaderEffect MakeShader(ShaderComponent shaderComponent)
        {
            var effectParametersFromShaderComponent = new List<EffectParameterDeclaration>();
            var renderStateSet = new RenderStateSet();

            if (shaderComponent.EffectParameter != null)
            {
                // BUG: JSIL crashes with:
                // BUG: effectParametersFromShaderComponent.AddRange(shaderComponent.EffectParameter.Select(CreateEffectParameterDeclaration));

                var allEffectParameterDeclaration = new List<EffectParameterDeclaration>();

                foreach (var effectParam in shaderComponent.EffectParameter) // DO NOT CONVERT TO LINQ!
                {
                    allEffectParameterDeclaration.Add(CreateEffectParameterDeclaration(effectParam));
                }
                effectParametersFromShaderComponent.AddRange(allEffectParameterDeclaration);

            }

            // no Effectpasses
            if (shaderComponent.EffectPasses == null)
                throw new InvalidDataException("No EffectPasses in Shader Component! Please specify at least one pass");

            var effectPasses = new EffectPassDeclaration[shaderComponent.EffectPasses.Count];

            for (var i = 0; i < shaderComponent.EffectPasses.Count; i++)
            {
                var newEffectPass = new EffectPassDeclaration();
                var effectPass = shaderComponent.EffectPasses[i];

                if (effectPass.RenderStateContainer != null)
                {
                    renderStateSet = new RenderStateSet();
                    renderStateSet.SetRenderStates(effectPass.RenderStateContainer);
                }


                newEffectPass.VS = effectPass.VS;
                newEffectPass.PS = effectPass.PS;
                newEffectPass.StateSet = renderStateSet;

                effectPasses[i] = newEffectPass;
            }

            return new ShaderEffect(effectPasses, effectParametersFromShaderComponent);
        }

        private static EffectParameterDeclaration CreateEffectParameterDeclaration(TypeContainer effectParameter)
        {
            if (effectParameter.Name == null)
                throw new InvalidDataException("EffectParameterDeclaration: Name is empty!");

            var returnEffectParameterDeclaration = new EffectParameterDeclaration { Name = effectParameter.Name };

            var t = effectParameter.TypeId;

            switch (t)
            {
                case TypeId.Int:
                    if (effectParameter is TypeContainerInt effectParameterInt)
                        returnEffectParameterDeclaration.Value = effectParameterInt.Value;
                    break;
                case TypeId.Double:
                    if (effectParameter is TypeContainerDouble effectParameterDouble)
                        returnEffectParameterDeclaration.Value = effectParameterDouble.Value;
                    break;
                case TypeId.Float:
                    if (effectParameter is TypeContainerFloat effectParameterFloat)
                        returnEffectParameterDeclaration.Value = effectParameterFloat.Value;
                    break;
                case TypeId.Float2:
                    if (effectParameter is TypeContainerFloat2 effectParameterFloat2)
                        returnEffectParameterDeclaration.Value = effectParameterFloat2.Value;
                    break;
                case TypeId.Float3:
                    if (effectParameter is TypeContainerFloat3 effectParameterFloat3)
                        returnEffectParameterDeclaration.Value = effectParameterFloat3.Value;
                    break;
                case TypeId.Float4:
                    if (effectParameter is TypeContainerFloat4 effectParameterFloat4)
                        returnEffectParameterDeclaration.Value = effectParameterFloat4.Value;
                    break;
                case TypeId.Bool:
                    if (effectParameter is TypeContainerBool effectParameterBool)
                        returnEffectParameterDeclaration.Value = effectParameterBool.Value;
                    break;
                default:
                    throw new InvalidDataException($"EffectParameterDeclaration:{effectParameter.Name} is of unhandled type {t.ToString()}!");
                    break;
            }


            if (returnEffectParameterDeclaration.Value == null)
                throw new InvalidDataException($"EffectParameterDeclaration:{effectParameter.Name}, value is null");

            return returnEffectParameterDeclaration;
        }




        #endregion

    }

    #region LightViserator

    /// <summary>
    /// This struct saves a light found by a Viserator with all parameters
    /// </summary>
    public struct LightResult
    {
        /// <summary>
        /// Represents the light status.
        /// </summary>
        public bool Active;
        /// <summary>
        /// Represents the position of the light.
        /// </summary>
        public float3 Position;
        /// <summary>
        /// Represents the color.
        /// </summary>
        public float3 Color;
        /// <summary>
        /// Represents the attenuation of the light.
        /// </summary>
        public float Attenuation;
        /// <summary>
        /// Represents the ambient coefficient of the light.
        /// </summary>
        public float AmbientCoefficient;
        /// <summary>
        /// Represents the type of the light.
        /// </summary>
        public int Type;
        /// <summary>
        /// Represents the spot angle of the light.
        /// </summary>
        public float ConeAngle;
        /// <summary>
        /// Represents the cone direction of the light.
        /// </summary>
        public float3 ConeDirection;
        /// <summary>
        /// The ModelMatrix of the light
        /// </summary>
        public float4x4 ModelMatrix;
        /// <summary>
        /// The light's position in World Coordiantes.
        /// </summary>
        public float3 PositionWorldSpace;
        /// <summary>
        /// The cone's direction in WorldSpace
        /// </summary>
        public float3 ConeDirectionWorldSpace;
        /// <summary>
        /// The lights's position in ModelView Coordinates.
        /// </summary>
        public float3 PositionModelViewSpace;
        /// <summary>
        /// The cone's position in ModelViewCoordinates
        /// </summary>
        public float3 ConeDirectionModelViewSpace;
    }



    public class LightSetupState : VisitorState
    {
        private readonly CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();

        /// <summary>
        /// Gets or sets the top of the Model matrix stack. The Model matrix transforms model coordinates into world coordinates.
        /// </summary>
        /// <value>
        /// The Model matrix.
        /// </value>
        public float4x4 Model
        {
            set { _model.Tos = value; }
            get { return _model.Tos; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightSetupState"/> class.
        /// </summary>
        public LightSetupState()
        {
            RegisterState(_model);
        }
    }

    public class LightSetup : Viserator<KeyValuePair<LightComponent, LightResult>, LightSetupState>
    {
        public Dictionary<LightComponent, LightResult> FoundLightResults = new Dictionary<LightComponent, LightResult>();

        protected override void InitState()
        {
            base.InitState();
            State.Model = float4x4.Identity;
        }


        [VisitMethod]
        public void OnTransform(TransformComponent xform)
        {
            State.Model *= xform.Matrix();
        }

        [VisitMethod]
        public void OnLight(LightComponent lightComponent)
        {
            var lightResult = new LightResult
            {
                Type = (int)lightComponent.Type,
                Color = lightComponent.Color,
                ConeAngle = lightComponent.ConeAngle,
                ConeDirection = lightComponent.ConeDirection,
                AmbientCoefficient = lightComponent.AmbientCoefficient,
                ModelMatrix = State.Model,
                Position = lightComponent.Position,
                PositionWorldSpace = State.Model * lightComponent.Position,
                ConeDirectionWorldSpace = State.Model * lightComponent.ConeDirection,
                Active = lightComponent.Active,
                Attenuation = lightComponent.Attenuation
            };
            YieldItem(new KeyValuePair<LightComponent, LightResult>(lightComponent, lightResult));
        }
    }
    #endregion
}