using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
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
        /// Enables or disables Frustum Culling.
        /// If we render with one or more cameras this value will be overwritten by <see cref="Camera.FrustumCullingOn"/>.
        /// </summary>
        public bool DoFrumstumCulling = true;

        /// <summary>
        /// The RenderLayer this renderer should render.
        /// </summary>
        public RenderLayers RenderLayer
        {
            get => _renderLayer;
            set
            {
                _renderLayer = value;
                foreach (var module in VisitorModules)
                {
                    ((IRendererModule)module).UpdateRenderLayer(_renderLayer);
                }
            }
        }
        private RenderLayers _renderLayer;

        /// <summary>
        /// Returns currently visited <see cref="InstanceData"/> during a traversal.
        /// </summary>
        protected InstanceData CurrentInstanceData;

        /// <summary>
        /// Light results, collected from the scene in the <see cref="Core.PrePassVisitor"/>.
        /// </summary>
        internal List<LightResult> LightViseratorResults
        {
            get => _lightResults;
            private set
            {
                _lightResults = value;

                if (_numberOfLights != _lightResults.Count)
                {
                    _numberOfLights = _lightResults.Count;
                }
            }
        }
        private List<LightResult> _lightResults = new();

        #region Traversal information

        private CanvasTransform _ctc;
        private MinMaxRect _parentRect;
        private int _numberOfLights;

        internal PrePassVisitor PrePassVisitor { get; private set; }

        /// <summary>
        /// Caches SceneNodes and their model matrices. Used when visiting a <see cref="Bone"/>.
        /// </summary>
        protected Dictionary<SceneNode, float4x4> _boneMap = new();

        /// <summary>
        /// Manages animations.
        /// </summary>
        protected Animation _animation;

        /// <summary>
        /// The <see cref="SceneContainer"/>, containing the scene that gets rendered.
        /// </summary>
        protected SceneContainer _sc;

        /// <summary>
        /// The <see cref="SceneContainer"/>, containing the scene that gets rendered.
        /// </summary>
        public SceneContainer SC { get => _sc; }

        /// <summary>
        /// The <see cref="RenderContext"/>, used to render the scene. This will be ignored if cameras are used.
        /// </summary>
        protected RenderContext _rc;

        /// <summary>
        /// Holds the status of the model matrices and other information we need while traversing up and down the scene graph.
        /// </summary>
        protected RendererState _state;

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
                    Color = new float4(1f, 1f, 1f, 1f),
                    OuterConeAngle = 45f,
                    InnerConeAngle = 35f,
                    Type = LightType.Legacy,
                    IsCastingShadows = false
                };
            }
            // if there is no light in scene then add one (legacyMode)
            _lightResults.Add(new LightResult(_legacyLight)
            {
                Rotation = new float4x4
                (
                    new float4(_rc.InvView.Row1.xyz, 0),
                    new float4(_rc.InvView.Row2.xyz, 0),
                    new float4(_rc.InvView.Row3.xyz, 0),
                    float4.UnitW
                 ),
                WorldSpacePos = _rc.InvView.Column4.xyz
            });
        }

        /// <summary>
        /// Creates a new instance of type SceneRendererForward.
        /// This scene renderer is used for forward rendering.
        /// </summary>
        /// <param name="sc">The <see cref="Scene"/> containing the scene that is rendered.</param>
        /// <param name="renderLayer"></param>
        public SceneRendererForward(SceneContainer sc, RenderLayers renderLayer = RenderLayers.Default)
        {
            _sc = sc;
            PrePassVisitor = new PrePassVisitor();
            IgnoreInactiveComponents = true;
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
                                    Channel<float3>.LerpFunc lerpFunc = animTrackContainer.LerpType switch
                                    {
                                        LerpType.Lerp => Lerp.Float3Lerp,
                                        LerpType.Slerp => Lerp.Float3QuaternionSlerp,
                                        _ => throw new InvalidOperationException(
             "Unknown lerp type: animTrackContainer.LerpType: " +
             (int)animTrackContainer.LerpType),// C# 6throw new InvalidEnumArgumentException(nameOf(animTrackContainer.LerpType), (int)animTrackContainer.LerpType, typeof(LerpType));
                                               // throw new InvalidEnumArgumentException("animTrackContainer.LerpType", (int)animTrackContainer.LerpType, typeof(LerpType));
                                    };
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
        public virtual void SetContext(RenderContext rc)
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
                InitState();
            }
        }

        /// <summary>
        /// Updates the state in each registered <see cref="IRendererModule"/>.
        /// </summary>
        protected void NotifyStateChanges()
        {
            foreach (var module in VisitorModules)
            {
                ((IRendererModule)module).UpdateState(_state);
            }
        }

        /// <summary>
        /// Updates the camera in each registered <see cref="IRendererModule"/>.
        /// </summary>
        protected void NotifyCameraChanges(Camera cam)
        {
            foreach (var module in VisitorModules)
            {
                ((IRendererModule)module).UpdateCamera(cam);
            }
        }

        #endregion

        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="rc"></param>
        public virtual void Render(RenderContext rc)
        {
            SetContext(rc);
            NotifyStateChanges();

            PrePassVisitor.PrePassTraverse(_sc);

            AccumulateLight();

            if (PrePassVisitor.CameraPrepassResults.Count != 0)
            {
                var cams = PrePassVisitor.CameraPrepassResults.OrderBy(cam => cam.Camera.Layer);

                //Render for all cameras
                foreach (var cam in cams)
                {
                    if (cam.Camera.Active)
                    {
                        PerCamClear(cam);
                        NotifyCameraChanges(cam.Camera);
                        DoFrumstumCulling = cam.Camera.FrustumCullingOn;
                        PerCamRender(cam);
                    }
                }

                //Reset Viewport and frustum culling bool in case we have another scene, rendered without a camera
                _rc.Viewport(0, 0, rc.DefaultState.CanvasWidth, rc.DefaultState.CanvasHeight);
                //Standard value: frustum culling is on.
                DoFrumstumCulling = true;
            }
            else
            {
                UpdateShaderParamsForAllLights();
                Traverse(_sc.Children);
            }
        }

        internal void PerCamClear(CameraResult cam)
        {
            var tex = cam.Camera.RenderTexture;
            RenderLayer = cam.Camera.RenderLayer;

            float4 viewport = tex != null
                ? cam.Camera.GetViewportInPx(tex.Width, tex.Height)
                : cam.Camera.GetViewportInPx(_rc.GetWindowWidth(), _rc.GetWindowHeight());

            _rc.Viewport((int)viewport.x, (int)viewport.y, (int)viewport.z, (int)viewport.w);
            _rc.SetRenderTarget(tex);

            _rc.ClearColor = cam.Camera.BackgroundColor;
            if (cam.Camera.ClearColor)
                _rc.Clear(ClearFlags.Color);

            if (cam.Camera.ClearDepth)
                _rc.Clear(ClearFlags.Depth);
        }

        private void PerCamRender(CameraResult cam)
        {
            RenderLayer = cam.Camera.RenderLayer;
            _rc.View = cam.View;

            var tex = cam.Camera.RenderTexture;

            _rc.SetRenderTarget(tex);

            _rc.Projection = tex != null
                ? cam.Camera.GetProjectionMat(cam.Camera.RenderTexture.Width, cam.Camera.RenderTexture.Height, out float4 viewport)
                : cam.Camera.GetProjectionMat(_rc.GetWindowWidth(), _rc.GetWindowHeight(), out viewport);

            _rc.Viewport((int)viewport.x, (int)viewport.y, (int)viewport.z, (int)viewport.w);

            UpdateShaderParamsForAllLights();

            Traverse(_sc.Children);

            // if we have a multisample texture we need to blt the result of our rendering to the result texture
            if (tex is WritableMultisampleTexture wmt)
            {
                _rc.BlitMultisample2DTextureToTexture(wmt, wmt.InternalResultTexture);
            }

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
        /// Renders the RenderLayer.
        /// </summary>
        /// <param name="renderLayer"></param>
        [VisitMethod]
        public void RenderRenderLayer(RenderLayer renderLayer)
        {
            _state.RenderLayer = renderLayer;
        }

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
            _ = float4x4.CreateTranslation(trans) * rot;

            if (!_boneMap.TryGetValue(boneContainer, out _))
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
            var boneArray = new float4x4[weight.Joints.Count];
            for (var i = 0; i < weight.Joints.Count; i++)
            {
                var tmp = weight.BindingMatrices[i];
                boneArray[i] = _boneMap[weight.Joints[i]] * tmp;
            }
            //TODO: find a way to NOT push the bones into the RC because they are not "global"
        }

        /// <summary>
        /// Sets <see cref="CurrentInstanceData"/>.
        /// </summary>
        /// <param name="instanceData"></param>
        [VisitMethod]
        public void RenderInstances(InstanceData instanceData)
        {
            CurrentInstanceData = instanceData;
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

                var currentScale = _state.Model.Scale();
                _state.CanvasXForm *= float4x4.CreateScale(new float3(1 / currentScale.x, 1 / currentScale.y, 1 / currentScale.z)) * float4x4.CreateTranslation(newRect.Center.x, newRect.Center.y, 0);
                _state.Model *= _state.CanvasXForm;

                _parentRect = newRect;
                _state.UiRect = newRect;
            }
            else if (ctc.CanvasRenderMode == CanvasRenderMode.Screen)
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
                _state.CanvasXForm *= _rc.InvModel * _rc.InvView * float4x4.CreateTranslation(0, 0, zNear + (zNear * 0.01f));
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
                translationX = xfc.HorizontalAlignment switch
                {
                    HorizontalTextAlignment.Left => -_state.UiRect.Size.x / 2,
                    HorizontalTextAlignment.Center => -xfc.Width / 2,
                    HorizontalTextAlignment.Right => _state.UiRect.Size.x / 2 - xfc.Width,
                    _ => throw new ArgumentException("Invalid Horizontal Alignment"),
                };
                translationY = xfc.VerticalAlignment switch
                {
                    VerticalTextAlignment.Top => _state.UiRect.Size.y / 2,
                    VerticalTextAlignment.Center => xfc.Height / 2,
                    VerticalTextAlignment.Bottom => xfc.Height - (_state.UiRect.Size.y / 2),
                    _ => throw new ArgumentException("Invalid Horizontal Alignment"),
                };
            }
            else
            {
                //Undo parent scale, scale by distance
                scaleX = 1 / _state.UiRect.Size.x * scaleFactor;
                scaleY = 1 / _state.UiRect.Size.y * scaleFactor;

                //Calculate translation according to alignment by scaling the rectangle size
                translationX = xfc.HorizontalAlignment switch
                {
                    HorizontalTextAlignment.Left => -_state.UiRect.Size.x * invScaleFactor / 2,
                    HorizontalTextAlignment.Center => -xfc.Width / 2,
                    HorizontalTextAlignment.Right => _state.UiRect.Size.x * invScaleFactor / 2 - xfc.Width,
                    _ => throw new ArgumentException("Invalid Horizontal Alignment"),
                };
                translationY = xfc.VerticalAlignment switch
                {
                    VerticalTextAlignment.Top => _state.UiRect.Size.y * invScaleFactor / 2,
                    VerticalTextAlignment.Center => xfc.Height / 2,
                    VerticalTextAlignment.Bottom => xfc.Height - (_state.UiRect.Size.y * invScaleFactor / 2),
                    _ => throw new ArgumentException("Invalid Horizontal Alignment"),
                };
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
            _state.Model *= transform.Matrix;
            _rc.Model = _state.Model;
        }

        /// <summary>
        /// If a ShaderEffect is visited the ShaderEffect of the <see cref="RendererState"/> is updated and the effect is set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="effect">The <see cref="Effect"/></param>
        [VisitMethod]
        public void RenderEffect(Effect effect)
        {
            _state.Effect = effect;
            _rc.SetEffect(_state.Effect, true);
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
            if (!RenderLayer.HasFlag(_state.RenderLayer.Layer) && !_state.RenderLayer.Layer.HasFlag(RenderLayer) || _state.RenderLayer.Layer.HasFlag(RenderLayers.None))
                return;

            if (DoFrumstumCulling)
            {
                //If the bounding box is zero in size, it is not initialized and we cannot perform the culling test.
                if (mesh.BoundingBox.Size.x > 0 && mesh.BoundingBox.Size.y > 0 && mesh.BoundingBox.Size.z > 0)
                {
                    var worldSpaceBoundingBox = _state.Model * mesh.BoundingBox;
                    if (!worldSpaceBoundingBox.InsideOrIntersectingFrustum(_rc.RenderFrustum))
                        return;
                }
            }

            //var wc = CurrentNode.GetWeights();
            //if (wc != null)
            //    AddWeightToMesh(mesh, wc);

            _rc.Render(mesh, CurrentInstanceData, true);
            CurrentInstanceData = null;
        }

        /// <summary>
        /// If a Mesh is visited the shader parameters for all lights in the scene are updated and the geometry is passed to be pushed through the rendering pipeline.
        /// </summary>
        /// <param name="mesh">The Mesh.</param>
        [VisitMethod]
        public void RenderMesh(GpuMesh mesh)
        {
            if (!mesh.Active) return;
            if (!RenderLayer.HasFlag(_state.RenderLayer.Layer) && !_state.RenderLayer.Layer.HasFlag(RenderLayer) || _state.RenderLayer.Layer.HasFlag(RenderLayers.None))
                return;

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

            _rc.Render(mesh, true);
        }

        /// <summary>
        /// Adds bone indices and bone weights from a <see cref="Weight"/> to a mesh.
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

            mesh.BoneIndices = new MeshAttributes<float4>(boneIndices);
            mesh.BoneWeights = new MeshAttributes<float4>(boneWeights);
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
            _state.Effect = _rc.DefaultEffect;
            _rc.CreateShaderProgram(_state.Effect);
            _state.RenderLayer = new RenderLayer();
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
            _rc.SetEffect(_state.Effect, true);
        }

        #endregion

        private void UpdateShaderParamsForAllLights()
        {
            if (_lightResults.Count > ModuleExtensionPoint.NumberOfLightsForward)
                Diagnostics.Warn($"Number of lights in the scene exceeds the maximal allowed number. Lights above {ModuleExtensionPoint.NumberOfLightsForward} will be ignored!");

            for (var i = 0; i < _rc.ForwardLights.Length; i++)
            {
                if (i < _lightResults.Count)
                    UpdateShaderParamForLight(i, _lightResults[i]);
                else
                    _rc.ForwardLights[i].Light.Active = false;
            }
        }

        private void UpdateShaderParamForLight(int position, LightResult lightRes)
        {
            var light = lightRes.Light;
            var strength = light.Strength;

            if (strength > 1.0 || strength < 0.0)
            {
                strength = M.Clamp(light.Strength, 0.0f, 1.0f);
                Diagnostics.Warn("Strength of the light will be clamped between 0 and 1.");
                light.Strength = strength;
            }

            _rc.ForwardLights[position] = lightRes;
        }
    }
}