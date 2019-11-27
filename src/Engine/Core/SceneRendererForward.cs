using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using Fusee.Xirkit;
using Fusee.Base.Common;
using Fusee.Engine.Core.ShaderShards;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use a Scene Renderer to traverse a scene hierarchy (made out of scene nodes and components) in order
    /// to have each visited element contribute to the result rendered against a given render context.
    /// </summary>
    public partial class SceneRendererForward : SceneVisitor
    {
        private int _numberOfLights;

        ///Is set to true if a light was added or removed from the scene.
        protected bool HasNumberOfLightsChanged;

        /// <summary>
        /// Light results, collected from the scene in the Viserator.
        /// </summary>
        public List<Tuple<SceneNodeContainer, LightResult>> LightViseratorResults
        {            
            get
            {
                return _lightResults;
            }
            private set
            {
                _lightResults = value;

                if (_numberOfLights != _lightResults.Count)
                {
                    _lightPararamStringsAllLights = new Dictionary<int, LightParamStrings>();
                    HasNumberOfLightsChanged = true;
                    _numberOfLights = _lightResults.Count;
                }
            }
        }

        private CanvasTransformComponent _ctc;
        private MinMaxRect _parentRect;

        #region Traversal information

        /// <summary>
        /// Caches SceneNodeContainers and their model matrices. Used when visiting a <see cref="BoneComponent"/>.
        /// </summary>
        protected Dictionary<SceneNodeContainer, float4x4> _boneMap;

        /// <summary>
        /// Manages animations.
        /// </summary>
        protected Animation _animation;

        /// <summary>
        /// The SceneContainer, containing the scene that gets rendered.
        /// </summary>
        protected SceneContainer _sc;

        /// <summary>
        /// The RenderContext, used to render the scene.
        /// </summary>
        protected RenderContext _rc;

        /// <summary>
        /// The ShaderEffect, used if no other effect is found while traversing the scene.
        /// </summary>
        protected ShaderEffect _defaultEffect;

        /// <summary>
        /// Holds the status of the model matrices and other information we need while traversing up and down the scene graph.
        /// </summary>
        protected RendererState _state;

        /// <summary>
        /// List of <see cref="LightResult"/>, created by the <see cref="LightViserator"/>.
        /// </summary>
        protected List<Tuple<SceneNodeContainer, LightResult>> _lightResults = new List<Tuple<SceneNodeContainer, LightResult>>();

        #endregion

        #region Initialization Construction Startup      

        
        private LightComponent _legacyLight;

        private void SetDefaultLight()
        {
            if(_legacyLight == null)
            {
                _legacyLight = new LightComponent()
                {
                    Active = true,
                    Strength = 1.0f,
                    MaxDistance = 0.0f,
                    Color = new float4(1.0f, 1.0f, 1.0f, 1f),
                    OuterConeAngle = 45f,
                    InnerConeAngle = 35f,
                    Type = LightType.Legacy,
                    IsCastingShadows = false
                };
            }
            // if there is no light in scene then add one (legacyMode)
            _lightResults.Add(new Tuple<SceneNodeContainer, LightResult>(CurrentNode, new LightResult(_legacyLight)
            {
                Rotation = new float4x4
                (
                    new float4(_rc.InvView.Row0.xyz, 0),
                    new float4(_rc.InvView.Row1.xyz, 0),
                    new float4(_rc.InvView.Row2.xyz, 0),
                    float4.UnitW
                 ),
                WorldSpacePos = _rc.InvView.Column3.xyz
            }));
        }

        /// <summary>
        /// Creates a new instance of type SceneRendererForward.
        /// This scene renderer is used for forward rendering.
        /// </summary>
        /// <param name="sc">The <see cref="SceneContainer"/> containing the scene that is rendered.</param>
        public SceneRendererForward(SceneContainer sc)
        {
            _sc = sc;

            var buildFrag = new ProtoToFrag(_sc, true);
            buildFrag.BuildFragmentShaders();

            _state = new RendererState();
            InitAnimations(_sc);
        }

        /// <summary>
        /// Initializes animations, given as <see cref="AnimationComponent"/>.
        /// </summary>
        /// <param name="sc">The SceneContainer, containing the AnimationComponents.</param>
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

        /// <summary>
        /// Handles animations.
        /// </summary>
        public void Animate()
        {
            if (_animation.ChannelBaseList.Count != 0)
            {
                // Set the animation time here!
                _animation.Animate(Time.DeltaTime);
            }
        }

        /// <summary>
        /// Sets the render context for the given scene.
        /// </summary>
        /// <param name="rc"></param>
        public void SetContext(RenderContext rc)
        {
            if (rc == null)
                throw new ArgumentNullException("rc");

            if (rc != _rc)
            {
                _rc = rc;
                _boneMap = new Dictionary<SceneNodeContainer, float4x4>();

                var defaultMat = new MaterialComponent
                {
                    Diffuse = new MatChannelContainer
                    {
                        Color = new float4(0.5f, 0.5f, 0.5f, 1.0f)
                    },
                    Specular = new SpecularChannelContainer
                    {
                        Color = new float4(1, 1, 1, 1),
                        Intensity = 0.5f,
                        Shininess = 22
                    }
                };

                _defaultEffect = ShaderCodeBuilder.MakeShaderEffectFromMatComp(defaultMat);
                _rc.SetShaderEffect(_defaultEffect);
            }
        }
        #endregion


        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="renderTarget">Optional parameter: set this if you want to render to a g-buffer.</param>
        public void Render(RenderContext rc, RenderTarget renderTarget = null)
        {
            SetContext(rc);
            AccumulateLight();
            UpdateShaderParamsForAllLights();
            rc.SetRenderTarget(renderTarget);

            Traverse(_sc.Children);
        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="renderTexture">Optional parameter: set this if you want to render to a texture.</param>
        public void Render(RenderContext rc, WritableTexture renderTexture = null)
        {
            SetContext(rc);
            AccumulateLight();
            UpdateShaderParamsForAllLights();
            rc.SetRenderTarget(renderTexture);

            Traverse(_sc.Children);
        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="rc"></param>       
        public void Render(RenderContext rc)
        {
            SetContext(rc);
            AccumulateLight();
            UpdateShaderParamsForAllLights();
            rc.SetRenderTarget();

            Traverse(_sc.Children);
        }

        #region Visitors

        /// <summary>
        /// If a Projection Component is visited, the projection matrix is set.
        /// </summary>
        /// <param name="pc">The visited ProjectionComponent.</param>
        [VisitMethod]
        public void RenderProjection(ProjectionComponent pc)
        {
            pc.Width = _rc.ViewportWidth;
            pc.Height = _rc.ViewportHeight;
            _rc.Projection = pc.Matrix();
        }

        /// <summary>
        /// Renders the Bone.
        /// </summary>
        /// <param name="bone">The bone.</param>
        [VisitMethod]
        public void RenderBone(BoneComponent bone)
        {
            SceneNodeContainer boneContainer = CurrentNode;

            var trans = boneContainer.GetGlobalTranslation();
            var rot = boneContainer.GetGlobalRotation();

            var currentModel = float4x4.CreateTranslation(trans) * rot; //TODO: ???

            float4x4 transform;
            if (!_boneMap.TryGetValue(boneContainer, out transform))
                _boneMap.Add(boneContainer, _rc.Model);
            else
                _boneMap[boneContainer] = _rc.Model;
        }

        /// <summary>
        /// Renders the weight.
        /// </summary>
        /// <param name="weight"></param>
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

        private bool isCtcInitialized = false;

        /// <summary>
        /// Sets the state of the model matrices and UiRects.
        /// </summary>
        /// <param name="ctc">The CanvasTransformComponent.</param>
        [VisitMethod]
        public void RenderCanvasTransform(CanvasTransformComponent ctc)
        {
            _ctc = ctc;

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

                var newRect = new MinMaxRect
                {
                    Min = ctc.ScreenSpaceSize.Min,
                    Max = ctc.ScreenSpaceSize.Max
                };

                if (!isCtcInitialized)
                {
                    ctc.Scale = new float2(ctc.Size.Size.x / ctc.ScreenSpaceSize.Size.x,
                        ctc.Size.Size.y / ctc.ScreenSpaceSize.Size.y);

                    _ctc = ctc;
                    isCtcInitialized = true;

                }
                _state.CanvasXForm *= _rc.InvView * float4x4.CreateTranslation(0, 0, zNear + (zNear * 0.01f));
                _state.Model *= _state.CanvasXForm;

                _parentRect = newRect;
                _state.UiRect = newRect;
            }
        }

        /// <summary>
        /// If a RectTransformComponent is visited the model matrix and MinMaxRect get updated in the <see cref="RendererState"/>.
        /// </summary>
        /// <param name="rtc">The XFormComponent.</param>
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

        /// <summary>
        /// If a XFormComponent is visited the model matrix gets updated in the <see cref="RendererState"/> and set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="xfc">The XFormComponent.</param>
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
            else if (_state.UiRect.Size == _parentRect.Size && xfc.Name.Contains("Canvas"))
                scale = float4x4.CreateScale(_state.UiRect.Size.x, _state.UiRect.Size.y, 1);
            else
                scale = float4x4.CreateScale(1, 1, 1);

            _state.Model *= scale;
            _rc.Model = _state.Model;
        }

        /// <summary>
        /// If a XFormTextComponent is visited the model matrix gets updated in the <see cref="RendererState"/> and set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="xfc">The XFormTextComponent.</param>
        [VisitMethod]
        public void RenderXFormText(XFormTextComponent xfc)
        {
            var scaleX = 1 / _state.UiRect.Size.x * xfc.TextScaleFactor;
            var scaleY = 1 / _state.UiRect.Size.y * xfc.TextScaleFactor;
            var scale = float4x4.CreateScale(scaleX, scaleY, 1);

            _state.Model *= scale;
            _rc.Model = _state.Model;
        }

        /// <summary>
        /// If a TransformComponent is visited the model matrix of the <see cref="RenderContext"/> and <see cref="RendererState"/> is updated.
        /// It additionally updates the view matrix of the RenderContext.
        /// </summary> 
        /// <param name="transform">The TransformComponent.</param>
        [VisitMethod]
        public void RenderTransform(TransformComponent transform)
        {
            _state.Model *= transform.Matrix();
            _rc.Model = _state.Model;
        }

        /// <summary>
        /// If a PtOctantComponent is visited the level of this octant is set in the shader.
        /// </summary>
        /// <param name="ptOctant"></param>
        [VisitMethod]
        public void RenderPtOctantComponent(PtOctantComponent ptOctant)
        {
            _state.Effect.SetEffectParam("OctantLevel", ptOctant.Level);
        }


        /// <summary>
        /// If a ShaderEffectComponent is visited the ShaderEffect of the <see cref="RendererState"/> is updated and the effect is set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="shaderComponent">The ShaderEffectComponent</param>
        [VisitMethod]
        public void RenderShaderEffect(ShaderEffectComponent shaderComponent)
        {
            if (HasNumberOfLightsChanged)
            {
                //change #define MAX_LIGHTS... or rebuild shader effect?
                HasNumberOfLightsChanged = false;
            }
            _state.Effect = shaderComponent.Effect;
            _rc.SetShaderEffect(shaderComponent.Effect);

        }

        /// <summary>
        /// If a Mesh is visited and it has a <see cref="WeightComponent"/> the BoneIndices and  BoneWeights get set, 
        /// the shader parameters for all lights in the scene are updated according to the <see cref="LightViserator"/>
        /// and the geometry is passed to be pushed through the rendering pipeline.        
        /// </summary>
        /// <param name="mesh">The Mesh.</param>
        [VisitMethod]
        public void RenderMesh(Mesh mesh)
        {
            if (!mesh.Active) return;

            WeightComponent wc = CurrentNode.GetWeights();
            if (wc != null)
                AddWeightComponentToMesh(mesh, wc);

            _rc.Render(mesh);
        }

        /// <summary>
        /// Viserates the LightComponent and caches them in a dedicated field.
        /// </summary>
        protected void AccumulateLight()
        {
           LightViseratorResults = _sc.Children.Viserate<LightViserator, Tuple<SceneNodeContainer, LightResult>>().ToList();
            
            if (LightViseratorResults.Count == 0)
                SetDefaultLight();
        }

        protected void AddWeightComponentToMesh(Mesh mesh, WeightComponent wc)
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

        #endregion

        #region HierarchyLevel

        /// <summary>
        /// Sets the initial values in the <see cref="RendererState"/>.
        /// </summary>
        protected override void InitState()
        {
            _state.Clear();
            _state.Model = float4x4.Identity;
            _state.CanvasXForm = float4x4.Identity;
            _state.UiRect = new MinMaxRect { Min = -float2.One, Max = float2.One };
            _state.Effect = _defaultEffect;
        }

        /// <summary>
        /// Pushes into the RenderState.
        /// </summary>
        protected override void PushState()
        {
            _state.Push();
        }

        /// <summary>
        /// Pops from the RenderState and sets the Model and View matrices in the RenderContext.
        /// </summary>
        protected override void PopState()
        {
            _state.Pop();
            _rc.Model = _state.Model;
        }

        #endregion

        private Dictionary<int, LightParamStrings> _lightPararamStringsAllLights = new Dictionary<int, LightParamStrings>();

        private void UpdateShaderParamsForAllLights()
        {
            for (var i = 0; i < _lightResults.Count; i++)
            {
                if (!_lightPararamStringsAllLights.ContainsKey(i))
                {
                    _lightPararamStringsAllLights.Add(i, new LightParamStrings(i));
                }

                UpdateShaderParamForLight(i, _lightResults[i].Item2);
            }
        }

        private void UpdateShaderParamForLight(int position, LightResult lightRes)
        {
            var light = lightRes.Light;

            var dirWorldSpace = float3.Normalize((lightRes.Rotation * float4.UnitZ).xyz);
            var dirViewSpace = float3.Normalize((_rc.View * new float4(dirWorldSpace)).xyz);
            var strength = light.Strength;

            if (strength > 1.0 || strength < 0.0)
            {
                strength = M.Clamp(light.Strength, 0.0f, 1.0f);
                Diagnostics.Warn("Strength of the light will be clamped between 0 and 1.");
            }

            var lightParamStrings = _lightPararamStringsAllLights[position];

            // Set params in modelview space since the lightning calculation is in modelview space
            _rc.SetFXParam(lightParamStrings.PositionViewSpace, _rc.View * lightRes.WorldSpacePos);
            _rc.SetFXParam(lightParamStrings.PositionWorldSpace, lightRes.WorldSpacePos);
            _rc.SetFXParam(lightParamStrings.Intensities, light.Color);
            _rc.SetFXParam(lightParamStrings.MaxDistance, light.MaxDistance);
            _rc.SetFXParam(lightParamStrings.Strength, strength);
            _rc.SetFXParam(lightParamStrings.OuterAngle, M.DegreesToRadians(light.OuterConeAngle));
            _rc.SetFXParam(lightParamStrings.InnerAngle, M.DegreesToRadians(light.InnerConeAngle));
            _rc.SetFXParam(lightParamStrings.Direction, dirViewSpace);
            _rc.SetFXParam(lightParamStrings.DirectionWorldSpace, dirWorldSpace);
            _rc.SetFXParam(lightParamStrings.LightType, (int)light.Type);
            _rc.SetFXParam(lightParamStrings.IsActive, light.Active ? 1 : 0);
            _rc.SetFXParam(lightParamStrings.IsCastingShadows, light.IsCastingShadows ? 1 : 0);
            _rc.SetFXParam(lightParamStrings.Bias, light.Bias);
        }

        #region RenderContext/Asset Setup

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
            }

            if (returnEffectParameterDeclaration.Value == null)
                throw new InvalidDataException($"EffectParameterDeclaration:{effectParameter.Name}, value is null");

            return returnEffectParameterDeclaration;
        }

        #endregion
    }

    internal struct LightParamStrings
    {
        public string PositionViewSpace;
        public string PositionWorldSpace;
        public string Intensities;
        public string MaxDistance;
        public string Strength;
        public string OuterAngle;
        public string InnerAngle;
        public string Direction;
        public string DirectionWorldSpace;
        public string LightType;
        public string IsActive;
        public string IsCastingShadows;
        public string Bias;

        public LightParamStrings(int arrayPos)
        {
            PositionViewSpace = $"allLights[{arrayPos}].position";
            PositionWorldSpace = $"allLights[{arrayPos}].positionWorldSpace";
            Intensities = $"allLights[{arrayPos}].intensities";
            MaxDistance = $"allLights[{arrayPos}].maxDistance";
            Strength = $"allLights[{arrayPos}].strength";
            OuterAngle = $"allLights[{arrayPos}].outerConeAngle";
            InnerAngle = $"allLights[{arrayPos}].innerConeAngle";
            Direction = $"allLights[{arrayPos}].direction";
            DirectionWorldSpace = $"allLights[{arrayPos}].directionWorldSpace";
            LightType = $"allLights[{arrayPos}].lightType";
            IsActive = $"allLights[{arrayPos}].isActive";
            IsCastingShadows = $"allLights[{arrayPos}].isCastingShadows";
            Bias = $"allLights[{arrayPos}].bias";
        }

    }
}