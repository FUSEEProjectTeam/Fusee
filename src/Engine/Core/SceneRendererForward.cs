using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Math.Core;
using Fusee.Xene;
using Fusee.Xirkit;
using System;
using System.Collections.Generic;
using System.Linq;
using Animation = Fusee.Xirkit.Animation;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use a Scene Renderer to traverse a scene hierarchy (made out of scene nodes and s) in order
    /// to have each visited element contribute to the result rendered against a given render context.
    /// </summary>
    public class SceneRendererForward : Visitor<SceneNode, SceneComponent>
    {
        /// <summary>
        ///Is set to true if a light was added or removed from the scene.
        /// /// </summary>
        protected bool HasNumberOfLightsChanged;

        /// <summary>
        /// Enables or disables Frustum Culling.
        /// If we render with one or more cameras this value will be overwritten by <see cref="Scene.Camera.FrustumCullingOn"/>.
        /// </summary>
        public bool DoFrumstumCulling = true;

        /// <summary>
        /// Light results, collected from the scene in the <see cref="Core.PrePassVisitor"/>.
        /// </summary>
        public List<Tuple<SceneNode, LightResult>> LightViseratorResults
        {
            get => _lightResults;
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

        private CanvasTransform _ctc;
        private MinMaxRect _parentRect;
        private int _numberOfLights;

        internal PrePassVisitor PrePassVisitor { get; private set; }

        /// <summary>
        /// Caches SceneNodes and their model matrices. Used when visiting a <see cref="Bone"/>.
        /// </summary>
        protected Dictionary<SceneNode, float4x4> _boneMap;

        /// <summary>
        /// Manages animations.
        /// </summary>
        protected Animation _animation;

        /// <summary>
        /// The Scene, containing the scene that gets rendered.
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
        protected List<Tuple<SceneNode, LightResult>> _lightResults = new List<Tuple<SceneNode, LightResult>>();

        #endregion

        #region Initialization Construction Startup      


        private Light _legacyLight;

        private void SetDefaultLight()
        {
            if (_legacyLight == null)
            {
                _legacyLight = new Light()
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
            _lightResults.Add(new Tuple<SceneNode, LightResult>(CurrentNode, new LightResult(_legacyLight)
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
        /// <param name="sc">The <see cref="Scene"/> containing the scene that is rendered.</param>
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
        /// Initializes animations, given as <see cref="Animation"/>.
        /// </summary>
        /// <param name="sc">The Scene, containing the Animations.</param>
        public void InitAnimations(SceneContainer sc)
        {
            _animation = new Animation();

            foreach (var a in sc.Children.FindComponents(t => t.GetType() == typeof(Scene.Animation)))
            {
                var ac = (Scene.Animation)a;
                if (ac.AnimationTracks != null)
                {
                    foreach (var animTrackContainer in ac.AnimationTracks)
                    {
                        // Type t = animTrackContainer.TypeId;
                        switch (animTrackContainer.TypeId)
                        {
                            // if (typeof(int).IsAssignableFrom(t))
                            case TypeId.Int:
                                {
                                    var channel = new Channel<int>(Lerp.IntLerp);
                                    foreach (AnimationKeyInt key in animTrackContainer.KeyFrames)
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
                                    var channel = new Channel<float>(Lerp.FloatLerp);
                                    foreach (AnimationKeyFloat key in animTrackContainer.KeyFrames)
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
                                    var channel = new Channel<float2>(Lerp.Float2Lerp);
                                    foreach (AnimationKeyFloat2 key in animTrackContainer.KeyFrames)
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
                                    var channel = new Channel<float3>(lerpFunc);
                                    foreach (AnimationKeyFloat3 key in animTrackContainer.KeyFrames)
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
                                    var channel = new Channel<float4>(Lerp.Float4Lerp);
                                    foreach (AnimationKeyFloat4 key in animTrackContainer.KeyFrames)
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
                _boneMap = new Dictionary<SceneNode, float4x4>();
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
                        DoFrumstumCulling = cam.Item2.Camera.FrustumCullingOn;
                        PerCamRender(cam);
                        //Reset Viewport and frustum culling bool in case we have another scene, rendered without a camera
                        _rc.Viewport(0, 0, rc.DefaultState.CanvasWidth, rc.DefaultState.CanvasHeight);
                        //Standard value: frustum culling is on.
                        DoFrumstumCulling = true;
                    }
                }
            }
            else
            {
                UpdateShaderParamsForAllLights();
                Traverse(_sc.Children);
            }
        }

        private void PerCamRender(Tuple<SceneNode, CameraResult> cam)
        {
            var tex = cam.Item2.Camera.RenderTexture;

            if (tex != null)
                _rc.SetRenderTarget(cam.Item2.Camera.RenderTexture);
            else
                _rc.SetRenderTarget();

            _rc.Projection = cam.Item2.Camera.GetProjectionMat(_rc.ViewportWidth, _rc.ViewportHeight, out var viewport);
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
        /// Viserates the Light and caches them in a dedicated field.
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
        public void RenderBone(Bone bone)
        {
            var boneContainer = CurrentNode;

            var trans = boneContainer.GetGlobalTranslation();
            var rot = boneContainer.GetGlobalRotation();

            var currentModel = float4x4.CreateTranslation(trans) * rot; //TODO: ???

            if (!_boneMap.TryGetValue(boneContainer, out var transform))
                _boneMap.Add(boneContainer, _rc.Model);
            else
                _boneMap[boneContainer] = _rc.Model;
        }

        /// <summary>
        /// Renders the weight.
        /// </summary>
        /// <param name="weight"></param>
        [VisitMethod]
        public void RenderWeight(Weight weight)
        {
            var boneArray = new float4x4[weight.Joints.Count()];
            for (var i = 0; i < weight.Joints.Count(); i++)
            {
                var tmp = weight.BindingMatrices[i];
                boneArray[i] = _boneMap[weight.Joints[i]] * tmp;
            }
            _rc.Bones = boneArray;
        }

        private bool isCtcInitialized = false;

        /// <summary>
        /// Sets the state of the model matrices and UiRects.
        /// </summary>
        /// <param name="ctc">The CanvasTransform.</param>
        [VisitMethod]
        public void RenderCanvasTransform(CanvasTransform ctc)
        {
            _ctc = ctc;

            if (ctc.CanvasRenderMode == CanvasRenderMode.World)
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

            if (ctc.CanvasRenderMode == CanvasRenderMode.Screen)
            {
                var frustumCorners = new float4[4];

                frustumCorners[0] = _rc.InvProjection * new float4(-1, -1, -1, 1); //nbl
                frustumCorners[1] = _rc.InvProjection * new float4(1, -1, -1, 1); //nbr 
                frustumCorners[2] = _rc.InvProjection * new float4(-1, 1, -1, 1); //ntl  
                frustumCorners[3] = _rc.InvProjection * new float4(1, 1, -1, 1); //ntr                

                for (var i = 0; i < frustumCorners.Length; i++)
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
        /// If a RectTransform is visited the model matrix and MinMaxRect get updated in the <see cref="RendererState"/>.
        /// </summary>
        /// <param name="rtc">The XForm.</param>
        [VisitMethod]
        public void RenderRectTransform(RectTransform rtc)
        {
            MinMaxRect newRect;
            if (_ctc.CanvasRenderMode == CanvasRenderMode.Screen)
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
        /// If a XForm is visited the model matrix gets updated in the <see cref="RendererState"/> and set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="xfc">The XForm.</param>
        [VisitMethod]
        public void RenderXForm(XForm xfc)
        {
            float4x4 scale;

            if (_state.UiRect.Size != _parentRect.Size)
            {
                var scaleX = _state.UiRect.Size.x / _parentRect.Size.x;
                var scaleY = _state.UiRect.Size.y / _parentRect.Size.y;
                scale = float4x4.CreateScale(scaleX, scaleY, 1);
            }
            else if (_state.UiRect.Size == _parentRect.Size && xfc.Name.Contains("Canvas"))
            {
                scale = float4x4.CreateScale(_state.UiRect.Size.x, _state.UiRect.Size.y, 1);
            }
            else
            {
                scale = float4x4.CreateScale(1, 1, 1);
            }

            _state.Model *= scale;
            _rc.Model = _state.Model;
        }

        /// <summary>
        /// If a XFormText is visited the model matrix gets updated in the <see cref="RendererState"/> and set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="xfc">The XFormText.</param>
        [VisitMethod]
        public void RenderXFormText(XFormText xfc)
        {
            var zNear = (_rc.InvProjection * new float4(-1, -1, -1, 1)).z;
            var scaleFactor = zNear / 100;
            var invScaleFactor = 1 / scaleFactor;

            float translationY;
            float translationX;

            float scaleX;
            float scaleY;

            if (_ctc.CanvasRenderMode == CanvasRenderMode.Screen)
            {
                //Undo parent scale
                scaleX = 1 / _state.UiRect.Size.x;
                scaleY = 1 / _state.UiRect.Size.y;

                //Calculate translation according to alignment
                switch (xfc.HorizontalAlignment)
                {
                    case HorizontalTextAlignment.Left:
                        translationX = -_state.UiRect.Size.x / 2;
                        break;
                    case HorizontalTextAlignment.Center:
                        translationX = -xfc.Width / 2;
                        break;
                    case HorizontalTextAlignment.Right:
                        translationX = _state.UiRect.Size.x / 2 - xfc.Width;
                        break;
                    default:
                        throw new ArgumentException("Invalid Horizontal Alignment");
                }

                switch (xfc.VerticalAlignment)
                {
                    case VerticalTextAlignment.Top:
                        translationY = _state.UiRect.Size.y / 2;
                        break;
                    case VerticalTextAlignment.Center:
                        translationY = xfc.Height / 2;
                        break;
                    case VerticalTextAlignment.Bottom:
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
                    case HorizontalTextAlignment.Left:
                        translationX = -_state.UiRect.Size.x * invScaleFactor / 2;
                        break;
                    case HorizontalTextAlignment.Center:
                        translationX = -xfc.Width / 2;
                        break;
                    case HorizontalTextAlignment.Right:
                        translationX = _state.UiRect.Size.x * invScaleFactor / 2 - xfc.Width;
                        break;
                    default:
                        throw new ArgumentException("Invalid Horizontal Alignment");
                }

                switch (xfc.VerticalAlignment)
                {
                    case VerticalTextAlignment.Top:
                        translationY = _state.UiRect.Size.y * invScaleFactor / 2;
                        break;
                    case VerticalTextAlignment.Center:
                        translationY = xfc.Height / 2;
                        break;
                    case VerticalTextAlignment.Bottom:
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
        /// If a Transform is visited the model matrix of the <see cref="RenderContext"/> and <see cref="RendererState"/> is updated.
        /// It additionally updates the view matrix of the RenderContext.
        /// </summary> 
        /// <param name="transform">The Transform.</param>
        [VisitMethod]
        public void RenderTransform(Transform transform)
        {
            _state.Model *= transform.Matrix();
            _rc.Model = _state.Model;
        }

        /// <summary>
        /// If a PtOctant is visited the level of this octant is set in the shader.
        /// </summary>
        /// <param name="ptOctant"></param>
        [VisitMethod]
        public void RenderPtOctant(Octant ptOctant)
        {
            _state.Effect.SetEffectParam("OctantLevel", ptOctant.Level);
        }


        /// <summary>
        /// If a ShaderEffect is visited the ShaderEffect of the <see cref="RendererState"/> is updated and the effect is set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="shader">The ShaderEffect</param>
        [VisitMethod]
        public void RenderShaderEffect(ShaderEffect shader)
        {
            if (HasNumberOfLightsChanged)
            {
                //change #define MAX_LIGHTS... or rebuild shader effect?
                HasNumberOfLightsChanged = false;
            }
            _state.Effect = shader;
            _rc.SetShaderEffect(_state.Effect);
        }

        /// <summary>
        /// If a Mesh is visited and it has a <see cref="Weight"/> the BoneIndices and  BoneWeights get set, 
        /// the shader parameters for all lights in the scene are updated and the geometry is passed to be pushed through the rendering pipeline.        
        /// </summary>
        /// <param name="mesh">The Mesh.</param>
        [VisitMethod]
        public void RenderMesh(Mesh mesh)
        {
            if (!mesh.Active) return;

            if (DoFrumstumCulling)
            {
                //If the bounding box is zero in size, it is not initialized and we cannot perform the culling test.
                if (mesh.BoundingBox.Size != float3.Zero)
                {
                    var worldSpaceBoundingBox = _state.Model * mesh.BoundingBox;
                    if (!worldSpaceBoundingBox.InsideOrIntersectingFrustum(_rc.RenderFrustum))
                        return;
                }
            }

            var wc = CurrentNode.GetWeights();
            if (wc != null)
                AddWeightToMesh(mesh, wc);

            var renderStatesBefore = _rc.CurrentRenderState.Copy();
            _rc.Render(mesh);
            var renderStatesAfter = _rc.CurrentRenderState.Copy();

            _state.RenderUndoStates = renderStatesBefore.Delta(renderStatesAfter);
        }

        /// <summary>
        /// Adds weight to a given mesh
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="wc"></param>
        protected void AddWeightToMesh(Mesh mesh, Weight wc)
        {
            var boneWeights = new float4[wc.WeightMap.Count];
            var boneIndices = new float4[wc.WeightMap.Count];

            // Iterate over the vertices
            for (var iVert = 0; iVert < wc.WeightMap.Count; iVert++)
            {
                var vwl = wc.WeightMap[iVert];

                // Security guard. Sometimes a vertex has no weight. This should be fixed in the model. But
                // let's just not crash here. Instead of having a completely unweighted vertex, bind it to
                // the root bone (index 0).
                if (vwl == null)
                    vwl = new VertexWeightList();
                if (vwl.VertexWeights == null)
                {
                    vwl.VertexWeights =
                        new List<VertexWeight>(new[] { new VertexWeight { JointIndex = 0, Weight = 1.0f } });
                }

                var nJoints = System.Math.Min(4, vwl.VertexWeights.Count);
                for (var iJoint = 0; iJoint < nJoints; iJoint++)
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

            // Set parameters in modelview space since the lightning calculation is in modelview space
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