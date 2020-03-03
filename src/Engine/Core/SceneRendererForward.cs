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
using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards.Fragment;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use a Scene Renderer to traverse a scene hierarchy (made out of scene nodes and components) in order
    /// to have each visited element contribute to the result rendered against a given render context.
    /// </summary>
    public class SceneRendererForward : SceneVisitor
    {
        /// <summary>
        ///Is set to true if a light was added or removed from the scene.
        /// /// </summary>
        protected bool HasNumberOfLightsChanged;

        /// <summary>
        /// Light results, collected from the scene in the <see cref="Core.PrePassVisitor"/>.
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
                    LightingShard.LightPararamStringsAllLights = new Dictionary<int, LightParamStrings>();
                    HasNumberOfLightsChanged = true;
                    _numberOfLights = _lightResults.Count;
                }
            }
        }

        #region Traversal information

        private CanvasTransformComponent _ctc;
        private MinMaxRect _parentRect;
        private int _numberOfLights;

        internal PrePassVisitor PrePassVisitor { get; private set; }

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
        /// Holds the status of the model matrices and other information we need while traversing up and down the scene graph.
        /// </summary>
        protected RendererState _state;

        /// <summary>
        /// List of <see cref="LightResult"/>, created by the <see cref="Core.PrePassVisitor"/>.
        /// </summary>
        protected List<Tuple<SceneNodeContainer, LightResult>> _lightResults = new List<Tuple<SceneNodeContainer, LightResult>>();

        #endregion

        #region Initialization Construction Startup      


        private LightComponent _legacyLight;

        private void SetDefaultLight()
        {
            if (_legacyLight == null)
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
            PrePassVisitor = new PrePassVisitor();
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
                _rc.SetShaderEffect(ShaderCodeBuilder.Default);
            }
        }
        #endregion


        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="rc"></param>       
        public void Render(RenderContext rc)
        {
            SetContext(rc);

            PrePassVisitor.PrePassTraverse(_sc, _rc);

            AccumulateLight();

            if (PrePassVisitor.CameraPrepassResults.Count != 0)
            {
                var cams = PrePassVisitor.CameraPrepassResults.OrderBy(cam => cam.Item2.Camera.Layer);
                foreach (var cam in cams)
                {
                    if (cam.Item2.Camera.Active)
                    {
                        PerCamRender(cam);
                        //Reset Viewport in case we have another scene, rendered without a camera 
                        _rc.Viewport(0, 0, rc.DefaultState.CanvasWidth, rc.DefaultState.CanvasHeight);
                    }
                }
            }
            else
            {
                UpdateShaderParamsForAllLights();
                Traverse(_sc.Children);
            }
        }

        private void PerCamRender(Tuple<SceneNodeContainer, CameraResult> cam)
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

            UpdateShaderParamsForAllLights();

            Traverse(_sc.Children);
        }


        /// <summary>
        /// Viserates the LightComponent and caches them in a dedicated field.
        /// </summary>
        protected void AccumulateLight()
        {
            LightViseratorResults = PrePassVisitor.LightPrepassResuls;

            if (LightViseratorResults.Count == 0)
                SetDefaultLight();
        }

        #region Visitors

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
                var frustumCorners = new float4[4];

                frustumCorners[0] = _rc.InvProjection * new float4(-1, -1, -1, 1); //nbl
                frustumCorners[1] = _rc.InvProjection * new float4(1, -1, -1, 1); //nbr 
                frustumCorners[2] = _rc.InvProjection * new float4(-1, 1, -1, 1); //ntl  
                frustumCorners[3] = _rc.InvProjection * new float4(1, 1, -1, 1); //ntr                

                for (int i = 0; i < frustumCorners.Length; i++)
                {
                    var corner = frustumCorners[i];
                    corner /= corner.w; //world space frustum corners               
                    frustumCorners[i] = corner;
                }

                var width = (frustumCorners[0] - frustumCorners[1]).Length;
                var height = (frustumCorners[0] - frustumCorners[2]).Length;

                var zNear = frustumCorners[0].z;
                var canvasPos = new float3(_rc.InvView.M14, _rc.InvView.M24, _rc.InvView.M34 + zNear);

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
            var zNear = (_rc.InvProjection * new float4(-1, -1, -1, 1)).z;
            var scaleFactor = zNear / 100;
            var invScaleFactor = 1 / scaleFactor;

            float translationY; 
            float translationX;

            float scaleX;
            float scaleY;
            
            if (_ctc.CanvasRenderMode == CanvasRenderMode.SCREEN)
            {
                //Undo parent scale
                scaleX = 1 / _state.UiRect.Size.x;
                scaleY = 1 / _state.UiRect.Size.y;
                

                //Calculate translation according to alignment
                switch (xfc.HorizontalAlignment)
                {
                    case HorizontalTextAlignment.LEFT:
                        translationX = -_state.UiRect.Size.x / 2;
                        break;
                    case HorizontalTextAlignment.MIDDLE:
                        translationX = -xfc.Width / 2;
                        break;
                    case HorizontalTextAlignment.RIGHT:
                        translationX = _state.UiRect.Size.x  / 2 - xfc.Width;
                        break;
                    default:
                        throw new ArgumentException("Invalid Horizontal Alignment");
                }

                switch (xfc.VerticalAlignment)
                {
                    case VerticalTextAlignment.TOP:
                        translationY = _state.UiRect.Size.y / 2;
                        break;
                    case VerticalTextAlignment.MIDDLE:
                        translationY = xfc.Height / 2;
                        break;
                    case VerticalTextAlignment.BOTTOM:
                        translationY = xfc.Height - (_state.UiRect.Size.y / 2);
                        break;
                    default:
                        throw new ArgumentException("Invalid Horizontal Alignment");
                }
            }
            else
            {
                //Undo parent scale, scale by distance
                scaleX = 1 / _state.UiRect.Size.x * scaleFactor;
                scaleY = 1 / _state.UiRect.Size.y * scaleFactor;                

                //Calculate translation according to alignment by scaling the rectangle size
                switch (xfc.HorizontalAlignment)
                {
                    case HorizontalTextAlignment.LEFT:
                        translationX = -_state.UiRect.Size.x * invScaleFactor / 2;
                        break;
                    case HorizontalTextAlignment.MIDDLE:
                        translationX = -xfc.Width / 2;
                        break;
                    case HorizontalTextAlignment.RIGHT:
                        translationX = _state.UiRect.Size.x * invScaleFactor / 2 - xfc.Width;
                        break;
                    default:
                        throw new ArgumentException("Invalid Horizontal Alignment");
                }

                switch (xfc.VerticalAlignment)
                {
                    case VerticalTextAlignment.TOP:
                        translationY = _state.UiRect.Size.y * invScaleFactor / 2;
                        break;
                    case VerticalTextAlignment.MIDDLE:
                        translationY = xfc.Height / 2;
                        break;
                    case VerticalTextAlignment.BOTTOM:
                        translationY = xfc.Height - (_state.UiRect.Size.y * invScaleFactor / 2);
                        break;
                    default:
                        throw new ArgumentException("Invalid Horizontal Alignment");
                }
            }

            var translation = float4x4.CreateTranslation(translationX, translationY, 0);
            var scale = float4x4.CreateScale(scaleX, scaleY, 1);

            _state.Model *= scale;
            _state.Model *= translation;
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
            _rc.SetShaderEffect(_state.Effect);
        }

        /// <summary>
        /// If a Mesh is visited and it has a <see cref="WeightComponent"/> the BoneIndices and  BoneWeights get set, 
        /// the shader parameters for all lights in the scene are updated and the geometry is passed to be pushed through the rendering pipeline.        
        /// </summary>
        /// <param name="mesh">The Mesh.</param>
        [VisitMethod]
        public void RenderMesh(Mesh mesh)
        {
            if (!mesh.Active) return;

            WeightComponent wc = CurrentNode.GetWeights();
            if (wc != null)
                AddWeightComponentToMesh(mesh, wc);

            var renderStatesBefore = _rc.CurrentRenderState.Copy();
            _rc.Render(mesh);
            var renderStatesAfter = _rc.CurrentRenderState.Copy();

            _state.RenderUndoStates = renderStatesBefore.Delta(renderStatesAfter);
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
            _state.Effect = ShaderCodeBuilder.Default;
            _state.RenderUndoStates = new RenderStateSet();
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
            _rc.SetRenderStateSet(_state.RenderUndoStates);
            _state.Pop();
            _rc.Model = _state.Model;
            _rc.SetShaderEffect(_state.Effect);

        }

        #endregion        

        private void UpdateShaderParamsForAllLights()
        {
            for (var i = 0; i < _lightResults.Count; i++)
            {
                if (!LightingShard.LightPararamStringsAllLights.ContainsKey(i))
                {
                    LightingShard.LightPararamStringsAllLights.Add(i, new LightParamStrings(i));
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

            var lightParamStrings = LightingShard.LightPararamStringsAllLights[position];

            // Set params in modelview space since the lightning calculation is in modelview space
            _rc.SetGlobalEffectParam(lightParamStrings.PositionViewSpace, _rc.View * lightRes.WorldSpacePos);
            _rc.SetGlobalEffectParam(lightParamStrings.Intensities, light.Color);
            _rc.SetGlobalEffectParam(lightParamStrings.MaxDistance, light.MaxDistance);
            _rc.SetGlobalEffectParam(lightParamStrings.Strength, strength);
            _rc.SetGlobalEffectParam(lightParamStrings.OuterAngle, M.DegreesToRadians(light.OuterConeAngle));
            _rc.SetGlobalEffectParam(lightParamStrings.InnerAngle, M.DegreesToRadians(light.InnerConeAngle));
            _rc.SetGlobalEffectParam(lightParamStrings.Direction, dirViewSpace);
            _rc.SetGlobalEffectParam(lightParamStrings.LightType, (int)light.Type);
            _rc.SetGlobalEffectParam(lightParamStrings.IsActive, light.Active ? 1 : 0);
            _rc.SetGlobalEffectParam(lightParamStrings.IsCastingShadows, light.IsCastingShadows ? 1 : 0);
            _rc.SetGlobalEffectParam(lightParamStrings.Bias, light.Bias);
        }
    }
}