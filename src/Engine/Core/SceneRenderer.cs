using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
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
        // Choose Lightning Method
        public static LightingCalculationMethod LightingCalculationMethod;
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

        private Dictionary<MaterialComponent, ShaderEffect> _matMap;
        private Dictionary<MaterialLightComponent, ShaderEffect> _lightMatMap;
        private Dictionary<MaterialPBRComponent, ShaderEffect> _pbrComponent;
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

            private StateStack<ShaderEffect> _effect = new StateStack<ShaderEffect>();

            public ShaderEffect Effect
            {
                set { _effect.Tos = value; }
                get { return _effect.Tos; }
            }

            public RendererState()
            {
                RegisterState(_model);
                RegisterState(_effect);
                RegisterState(_uiRect);
            }
        };

        private RendererState _state;
        private float4x4 _view;

        #endregion

        #region Initialization Construction Startup

        public SceneRenderer(SceneContainer sc, LightingCalculationMethod lightCalcMethod, bool RenderDeferred = false, bool RenderShadows = false)
             : this(sc)
        {
            LightingCalculationMethod = lightCalcMethod;

            if (RenderShadows)
                _wantToRenderWithShadows = true;

            if (RenderDeferred)
                _wantToRenderDeferred = true;

            if (lightCalcMethod == LightingCalculationMethod.ADVANCEDwENVMAP)
                _wantToRenderEnvMap = true;
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
                    Attenuation = 0,
                    Color = new float3(1.0f, 1.0f, 1.0f),
                    ConeAngle = 45f,
                    ConeDirection = float3.UnitZ,
                    ModelMatrix = float4x4.Identity,
                    Type = LightType.Legacy
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

        private float2 _rcViewportOriginalSize;

        public void SetContext(RenderContext rc)
        {
            if (rc == null)
                throw new ArgumentNullException("rc");

            if (rc != _rc)
            {
                _rc = rc;
                _rcViewportOriginalSize = new float2(_rc.ViewportWidth, _rc.ViewportHeight);
                _matMap = new Dictionary<MaterialComponent, ShaderEffect>();
                _lightMatMap = new Dictionary<MaterialLightComponent, ShaderEffect>();
                _pbrComponent = new Dictionary<MaterialPBRComponent, ShaderEffect>();
                _boneMap = new Dictionary<SceneNodeContainer, float4x4>();
                _shaderEffectMap = new Dictionary<ShaderComponent, ShaderEffect>();
                _defaultEffect = MakeMaterial(new MaterialComponent
                {
                    Diffuse = new MatChannelContainer()
                    {
                        Color = new float3(0.5f, 0.5f, 0.5f)
                    },
                    Specular = new SpecularChannelContainer()
                    {
                        Color = new float3(1, 1, 1),
                        Intensity = 0.5f,
                        Shininess = 22
                    }
                });
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
                _boneMap.Add(boneContainer, _rc.ModelView); // Changed from Model to ModelView
            else
                _boneMap[boneContainer] = _rc.ModelView; // Changed from Model to ModelView
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
        public void RenderRectTransform(RectTransformComponent rtc)
        {
            // The Heart of the UiRect calculation: Set anchor points relative to parent
            // rectangle and add absolute offsets
            MinMaxRect newRect = new MinMaxRect();
            newRect.Min = _state.UiRect.Min + _state.UiRect.Size * rtc.Anchors.Min + rtc.Offsets.Min;
            newRect.Max = _state.UiRect.Min + _state.UiRect.Size * rtc.Anchors.Max + rtc.Offsets.Max;

            var trans = newRect.Center - _state.UiRect.Center;
            var scale = new float2(newRect.Size.x / _state.UiRect.Size.x, newRect.Size.y / _state.UiRect.Size.y);
            var model = float4x4.CreateTranslation(trans.x, trans.y, 0) * float4x4.Scale(scale.x, scale.y, 1.0f);
            // var model = float4x4.Invert(_state.Model) *  float4x4.CreateTranslation(newRectCenter.x, newRectCenter.y, 0) * float4x4.Scale(0.5f * newRect.Size.x, 0.5f * newRect.Size.y, 1.0f);


            _state.UiRect = newRect;

            _state.Model *= float4x4.Identity;
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
        public void RenderMaterial(MaterialComponent matComp)
        {
            var effect = LookupMaterial(matComp);
            UpdateEffectParameters(matComp, effect);
            _state.Effect = effect;
        }

        [VisitMethod]
        public void RenderMaterial(MaterialLightComponent matComp)
        {
            var effect = LookupMaterial(matComp);
            _state.Effect = effect;
        }

        [VisitMethod]
        public void RenderMaterial(MaterialPBRComponent matComp)
        {
            var effect = LookupMaterial(matComp);
            _state.Effect = effect;
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
        }

        [VisitMethod]
        public void RenderMesh(Mesh mesh)
        {
            Mesh rm = mesh;
            //if (!_meshMap.TryGetValue(mesh, out rm))
            //{
            //    rm = MakeMesh(mesh);
            //    _meshMap.Add(mesh, rm);
            //}

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

        private static void UpdateLightParamsInPixelShader(int position, LightResult light, ShaderEffect effect)
        {
            if (!light.Active) return;

            // Set params in modelview space since the lightning calculation is in modelview space
            effect.SetEffectParam($"allLights[{position}].position", light.PositionModelViewSpace);
            effect.SetEffectParam($"allLights[{position}].intensities", light.Color);
            effect.SetEffectParam($"allLights[{position}].attenuation", light.Attenuation);
            effect.SetEffectParam($"allLights[{position}].ambientCoefficient", light.AmbientCoefficient);
            effect.SetEffectParam($"allLights[{position}].coneAngle", light.ConeAngle);
            effect.SetEffectParam($"allLights[{position}].coneDirection", light.ConeDirectionModelViewSpace);
            effect.SetEffectParam($"allLights[{position}].lightType", light.Type);
        }

        #region RenderContext/Asset Setup


        private ShaderEffect LookupMaterial(MaterialComponent mc)
        {
            ShaderEffect mat;
            if (_matMap.TryGetValue(mc, out mat)) return mat;

            mat = MakeMaterial(mc);
            _rc.SetShaderEffect(mat);
            // mat.AttachToContext(_rc);
            _matMap.Add(mc, mat);
            return mat;
        }
        private ShaderEffect LookupMaterial(MaterialLightComponent mc)
        {
            ShaderEffect mat;
            if (_lightMatMap.TryGetValue(mc, out mat)) return mat;

            mat = MakeMaterial(mc);
            _rc.SetShaderEffect(mat);
            //mat.AttachToContext(_rc);
            _lightMatMap.Add(mc, mat);
            return mat;
        }

        private ShaderEffect LookupMaterial(MaterialPBRComponent mc)
        {
            ShaderEffect mat;
            if (_pbrComponent.TryGetValue(mc, out mat)) return mat;

            mat = MakeMaterial(mc);
            _rc.SetShaderEffect(mat);
            // mat.AttachToContext(_rc);
            _pbrComponent.Add(mc, mat);
            return mat;
        }



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

        public Mesh MakeMesh(Mesh mc)
        {
            WeightComponent wc = CurrentNode.GetWeights();
            Mesh rm;
            if (wc == null)
            {
                rm = new Mesh()
                {
                    Colors = null,
                    Normals = mc.Normals,
                    UVs = mc.UVs,
                    Vertices = mc.Vertices,
                    Triangles = mc.Triangles
                };
            }
            else // Create Mesh with weightdata
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

                rm = new Mesh()
                {
                    Colors = null,
                    Normals = mc.Normals,
                    UVs = mc.UVs,
                    BoneIndices = boneIndices,
                    BoneWeights = boneWeights,
                    Vertices = mc.Vertices,
                    Triangles = mc.Triangles
                };


                /*
                // invert weightmap to handle it easier
                float[,] invertedWeightMap = new float[wc.WeightMap[0].JointWeights.Count, wc.Joints.Count];
                for (int i = 0; i < wc.WeightMap.Count; i++)
                {
                    for (int j = 0; j < wc.WeightMap[i].JointWeights.Count; j++)
                    {
                        invertedWeightMap[j, i] = (float) wc.WeightMap[i].JointWeights[j];
                    }
                }

                float4[] boneWeights = new float4[invertedWeightMap.GetLength(0)];
                float4[] boneIndices = new float4[invertedWeightMap.GetLength(0)];

                // Contents of the invertedWeightMap:
                // ----------------------------------
                // Imagine the weight table as seen in 3d modelling programs, i.e. cinema4d;
                // wij are values in the range between 0..1 and specify to which percentage 
                // the vertex (i) is controlled by the bone (j).
                //
                //            bone 0   bone 1   bone 2   bone 3   ....  -> indexed by j
                // vertex 0:   w00      w01      w02      w03
                // vertex 1:   w10      w11      w12      w13
                // vertex 2:   w20      w21      w22      w23
                // vertex 3:   w30      w31      w32      w33
                //   ...
                //  indexed 
                //   by i

                // Iterate over the vertices
                for (int iVert = 0; iVert < invertedWeightMap.GetLength(0); iVert++)
                {
                    boneWeights[iVert] = new float4(0, 0, 0, 0);
                    boneIndices[iVert] = new float4(0, 0, 0, 0);

                    var tempDictionary = new Dictionary<int, float>();

                    // For the given vertex i, see which bones control us
                    for (int j = 0; j < invertedWeightMap.GetLength(1); j++)
                    {
                        if (j < 4)
                        {
                            tempDictionary.Add(j, invertedWeightMap[iVert, j]);
                        }
                        else
                        {
                            float tmpWeight = invertedWeightMap[iVert, j];
                            var keyAndValue = tempDictionary.OrderBy(kvp => kvp.Value).First();
                            if (tmpWeight > keyAndValue.Value)
                            {
                                tempDictionary.Remove(keyAndValue.Key);
                                tempDictionary.Add(j, tmpWeight);
                            }
                        }
                    }

                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[iVert].x = keyValuePair.Key;
                        boneWeights[iVert].x = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[iVert].y = keyValuePair.Key;
                        boneWeights[iVert].y = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[iVert].z = keyValuePair.Key;
                        boneWeights[iVert].z = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[iVert].w = keyValuePair.Key;
                        boneWeights[iVert].w = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }

                    boneWeights[iVert].Normalize1();
                }

                rm = new Mesh()
                {
                    Colors = null,
                    Normals = mc.Normals,
                    UVs = mc.UVs,
                    BoneIndices = boneIndices,
                    BoneWeights = boneWeights,
                    Vertices = mc.Vertices,
                    Triangles = mc.Triangles
                };
                */
            }

            return rm;
        }

        private ITexture LoadTexture(string path)
        {
            // string texturePath = Path.Combine(_scenePathDirectory, path);
            var image = AssetStorage.Get<ImageData>(path);
            return _rc.CreateTexture(image);
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

        private ShaderEffect MakeMaterial(MaterialComponent mc)
        {
            WeightComponent wc = CurrentNode.GetWeights();

            ShaderCodeBuilder scb = null;

            // If MaterialLightComponent is found call the LegacyShaderCodeBuilder with the MaterialLight
            // The LegacyShaderCodeBuilder is intelligent enough to handle all the necessary compilations needed for the VS & PS
            if (mc.GetType() == typeof(MaterialLightComponent))
            {
                if (mc is MaterialLightComponent lightMat) scb = new ShaderCodeBuilder(lightMat, null, wc, _renderWithShadows);
            }
            else if (mc.GetType() == typeof(MaterialPBRComponent))
            {
                if (mc is MaterialPBRComponent pbrMaterial) scb = new ShaderCodeBuilder(pbrMaterial, null, LightingCalculationMethod, wc, _renderWithShadows);
            }
            else
            {
                scb = new ShaderCodeBuilder(mc, null, wc, _renderWithShadows); // TODO, CurrentNode.GetWeights() != null);
            }

            var effectParameters = AssembleEffectParamers(mc, scb);

            if (scb != null)
            {
                var ret = new ShaderEffect(new[]
                {
                    new EffectPassDeclaration()
                    {
                        VS = scb.VS,
                        //VS = VsBones,
                        PS = scb.PS,
                        StateSet = new RenderStateSet()
                        {
                            ZEnable = true,
                            AlphaBlendEnable = false
                        }
                    }
                },
                    effectParameters
                    );
                return ret;
            }

            throw new Exception("Material could not be evaluated or be built!");
        }

        private IEnumerable<EffectParameterDeclaration> AssembleEffectParamers(MaterialComponent mc, ShaderCodeBuilder scb)
        {
            var effectParameters = new List<EffectParameterDeclaration>();

            if (mc.HasDiffuse)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.DiffuseColorName,
                    Value = (object)mc.Diffuse.Color
                });
                if (mc.Diffuse.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = ShaderCodeBuilder.DiffuseMixName,
                        Value = mc.Diffuse.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = ShaderCodeBuilder.DiffuseTextureName,
                        Value = LoadTexture(mc.Diffuse.Texture)
                    });
                }
            }

            if (mc.HasSpecular)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.SpecularColorName,
                    Value = (object)mc.Specular.Color
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.SpecularShininessName,
                    Value = (object)mc.Specular.Shininess
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.SpecularIntensityName,
                    Value = (object)mc.Specular.Intensity
                });
                if (mc.Specular.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = ShaderCodeBuilder.SpecularMixName,
                        Value = mc.Specular.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = ShaderCodeBuilder.SpecularTextureName,
                        Value = LoadTexture(mc.Specular.Texture)
                    });
                }
            }

            if (mc.HasEmissive)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.EmissiveColorName,
                    Value = (object)mc.Emissive.Color
                });
                if (mc.Emissive.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = ShaderCodeBuilder.EmissiveMixName,
                        Value = mc.Emissive.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = ShaderCodeBuilder.EmissiveTextureName,
                        Value = LoadTexture(mc.Emissive.Texture)
                    });
                }
            }

            if (mc.HasBump)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.BumpIntensityName,
                    Value = mc.Bump.Intensity
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.BumpTextureName,
                    Value = LoadTexture(mc.Bump.Texture)
                });
            }

            SetLightEffectParameters(ref effectParameters);

            return effectParameters;
        }

        private static void SetLightEffectParameters(ref List<EffectParameterDeclaration> effectParameters)
        {
            for (var i = 0; i < AllLightResults.Keys.Count; i++)
            {
                if (!AllLightResults[AllLightResults.Keys.ElementAt(i)].Active)
                    continue;

                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].position",
                    Value = AllLightResults[AllLightResults.Keys.ElementAt(i)].PositionWorldSpace
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].intensities",
                    Value = AllLightResults[AllLightResults.Keys.ElementAt(i)].Color
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].attenuation",
                    Value = AllLightResults[AllLightResults.Keys.ElementAt(i)].Attenuation
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].ambientCoefficient",
                    Value = AllLightResults[AllLightResults.Keys.ElementAt(i)].AmbientCoefficient
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].coneAngle",
                    Value = AllLightResults[AllLightResults.Keys.ElementAt(i)].ConeAngle
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].coneDirection",
                    Value = AllLightResults[AllLightResults.Keys.ElementAt(i)].ConeDirection
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].lightType",
                    Value = (int)AllLightResults[AllLightResults.Keys.ElementAt(i)].Type
                });
            }
        }


        private void UpdateEffectParameters(MaterialComponent mc, ShaderEffect fx)
        {
            if (mc.HasDiffuse)
            {
                fx.SetEffectParam(ShaderCodeBuilder.DiffuseColorName, mc.Diffuse.Color);
                if (mc.Diffuse.Texture != null)
                {
                    fx.SetEffectParam(ShaderCodeBuilder.DiffuseMixName, mc.Diffuse.Mix);
                    // TODO: fx.SetEffectParam(scb.DiffuseTextureName, LookupTexture(mc.Diffuse.Texture));
                }
            }

            if (mc.HasSpecular)
            {
                fx.SetEffectParam(ShaderCodeBuilder.SpecularColorName, mc.Specular.Color);
                fx.SetEffectParam(ShaderCodeBuilder.SpecularShininessName, mc.Specular.Shininess);
                fx.SetEffectParam(ShaderCodeBuilder.SpecularIntensityName, mc.Specular.Intensity);
                if (mc.Specular.Texture != null)
                {
                    fx.SetEffectParam(ShaderCodeBuilder.SpecularMixName, mc.Specular.Mix);
                    // TODO: fx.SetEffectParam(scb.SpecularTextureName, LookupTexture(mc.Specular.Texture));
                }
            }

            if (mc.HasEmissive)
            {
                fx.SetEffectParam(ShaderCodeBuilder.EmissiveColorName, mc.Emissive.Color);
                if (mc.Emissive.Texture != null)
                {
                    fx.SetEffectParam(ShaderCodeBuilder.EmissiveMixName, mc.Emissive.Mix);
                    // TODO: fx.SetEffectParam(scb.EmissiveTextureName, LookupTexture(mc.Emissive.Texture));
                }
            }

            if (mc.HasBump)
            {
                fx.SetEffectParam(ShaderCodeBuilder.BumpIntensityName, mc.Bump.Intensity);
                // TODO: fx.SetEffectParam(scb.BumpTextureName, LookupTexture(mc.Bump.Texture));
            }

            /*
            // Any light calculation needed at all?
            if (mc.HasDiffuse || mc.HasSpecular)
            {
                // Light calculation parameters
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.LightColorName,
                    Value = new float3(1, 1, 1)
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.LightIntensityName,
                    Value = (float)1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.LightDirectionName,
                    Value = new float3(0, 0, 1)
                });
            }
            */

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
        public LightType Type;
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
                Type = lightComponent.Type,
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