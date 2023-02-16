using CommunityToolkit.Diagnostics;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// This class contains information about the scene of the picked point.
    /// </summary>
    public class PickResult
    {
        // Data
        /// <summary>
        /// The scene code container.
        /// </summary>
        public SceneNode? Node;

        /// <summary>
        /// The mesh.
        /// </summary>
        public Mesh? Mesh;

        /// <summary>
        /// The model matrix.
        /// </summary>
        public float4x4 Model;

        /// <summary>
        /// The view matrix
        /// </summary>
        public float4x4 View;

        /// <summary>
        /// The projection matrix.
        /// </summary>
        public float4x4 Projection;

        /// <summary>
        /// The clip position
        /// </summary>
        public float3 ClipPos;
    }

    public class MeshPickResult : PickResult
    {
        public int Triangle;
        public float U;
        public float V;
        public float DistanceFromOrigin;
    }

    /// <summary>
    /// Implements the scene picker.
    /// </summary>
    public class ScenePicker : Viserator<PickResult, ScenePicker.PickerState, SceneNode, SceneComponent>
    {
        private CanvasTransform? _ctc;

        private bool isCtcInitialized = false;
        private MinMaxRect _parentRect;

        private int _canvasWidth;
        private int _canvasHeight;

        public class MousePos
        {
            /// <summary>
            /// Current mouse position (in clip space)
            /// </summary>
            public int PickPosClip { get; set; }
        }

        #region State
        /// <summary>
        /// The picker state upon scene traversal.
        /// </summary>
        public class PickerState : VisitorState
        {
            private readonly CollapsingStateStack<float4x4> _canvasXForm = new();
            private readonly CollapsingStateStack<float4x4> _model = new();
            private readonly CollapsingStateStack<MinMaxRect> _uiRect = new();
            private readonly CollapsingStateStack<Cull> _cullMode = new();
            private readonly CollapsingStateStack<ShaderEffect> _shaderFX = new();

            public float2 PickPosClip { get; set; }

            /// <summary>
            /// The registered model.
            /// </summary>
            public float4x4 Model
            {
                set => _model.Tos = value;
                get => _model.Tos;
            }

            /// <summary>
            /// The registered UI rectangle.
            /// </summary>
            public MinMaxRect UiRect
            {
                set => _uiRect.Tos = value;
                get => _uiRect.Tos;
            }

            /// <summary>
            /// The registered canvas transform.
            /// </summary>
            public float4x4 CanvasXForm
            {
                get => _canvasXForm.Tos;
                set => _canvasXForm.Tos = value;
            }

            public ShaderEffect ShaderEffect
            {
                get => _shaderFX.Tos;
                set => _shaderFX.Tos = value;
            }

            /// <summary>
            /// The registered cull mode.
            /// </summary>
            public Cull CullMode
            {
                get => _cullMode.Tos;
                set => _cullMode.Tos = value;
            }

            public PickComponent? CurrentPickComp;

            /// <summary>
            /// The default constructor for the <see cref="PickerState"/> class, which registers state stacks for model, UI rectangle, and canvas transform, as well as cull mode.
            /// </summary>
            // TODO: Ask @CMl why this is being called after every Iteration/Viseration
            public PickerState()
            {
                RegisterState(_model);
                RegisterState(_uiRect);
                RegisterState(_canvasXForm);
                RegisterState(_cullMode);
                RegisterState(_shaderFX);
            }

            /// <summary>
            /// The current view matrix.
            /// </summary>
            public float4x4 View { get; set; }

            /// <summary>
            /// The current projection matrix.
            /// </summary>
            public float4x4 Projection { get; set; }
        }

        /// <summary>
        /// The pick position on the screen.
        /// </summary>
        public float2 PickPosClip { get; set; }

        public float4x4 InvView => State.View.Invert();

        public float4x4 InvProjection => State.Projection.Invert();

        public Camera? CurrentCamera { get; private set; }

        #endregion

        // deferred renderer for RGB FBO picking
        private SceneRendererForward _sceneRenderer;
        private WritableTexture _writableTexture;
        private Transform _cameraPosition = new();
        private RenderContext _rc;

        /// <summary>
        /// The constructor to initialize a new ScenePicker.
        /// </summary>
        /// <param name="cullMode"></param>
        /// <param name="scene">The <see cref="SceneContainer"/> to pick from.</param>
        public ScenePicker(SceneContainer scene, RenderContext rc = null, Cull cullMode = Cull.None, IEnumerable<IPickerModule> customPickModule = null)
            : base(scene.Children, customPickModule)
        {
            IgnoreInactiveComponents = true;
            State.CullMode = cullMode;
            _rc = rc;
        }

        /// <summary>
        /// This method is called when traversal starts to initialize the traversal state.
        /// </summary>
        protected override void InitState()
        {
            base.InitState();
            State.Model = float4x4.Identity;
            State.View = float4x4.Identity;
            State.Projection = float4x4.Identity;
            State.CanvasXForm = float4x4.Identity;

        }

        /// <summary>
        /// Returns a collection of objects that fall in the area of the pick position and that can be iterated over.
        /// </summary>
        /// <param name="pickPos">The pick position.</param>
        /// <param name="canvasWidth">The width of the current canvas, gets overwrite if a <see cref="Camera.RenderTexture"/> is bound</param>
        /// <param name="canvasHeight">The height of the current canvas, gets overwrite if a <see cref="Camera.RenderTexture"/> is bound</param>
        /// <returns></returns>
        public IEnumerable<PickResult> Pick(float2 pickPos, int canvasWidth, int canvasHeight)
        {

            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;

            PickPosClip = pickPos;
            State.PickPosClip = pickPos;

            SetState();
            var res = Viserate().ToList();
            res.AddRange(CheckVisitorModuleResults());

            return res;
        }

        private IEnumerable<PickResult> CheckVisitorModuleResults()
        {
            foreach (var module in VisitorModules)
            {
                var m = (IPickerModule)module;
                if (m.PickResult != null)
                    yield return m.PickResult;
            }
        }

        /// <summary>
        /// Wire state (call by ref) to visitor module
        /// </summary>
        private void SetState()
        {
            foreach (var module in VisitorModules)
            {
                var m = (IPickerModule)module;
                m.SetState(State);
            }
        }

        #region Visitors

        /// <summary>
        /// Set the current camera, update View and Projection matrices
        /// </summary>
        /// <param name="cam"></param>
        [VisitMethod]
        public void UpdateCamera(Camera cam)
        {
            if (!cam.Active) return;

            CurrentCamera = cam;

            var view = State.Model;
            var scale = float4x4.GetScale(State.View);

            if (scale.x != 1)
            {
                view.M11 /= scale.x;
                view.M21 /= scale.x;
                view.M31 /= scale.x;
            }

            if (scale.y != 1)
            {
                view.M12 /= scale.y;
                view.M22 /= scale.y;
                view.M32 /= scale.y;
            }

            if (scale.z != 1)
            {
                view.M13 /= scale.z;
                view.M23 /= scale.z;
                view.M33 /= scale.z;
            }

            State.View = view.Invert();
            // TODO(mr): TEST Renderlayer
            State.Projection = CurrentCamera.RenderTexture != null
            ? CurrentCamera.GetProjectionMat(CurrentCamera.RenderTexture.Width, CurrentCamera.RenderTexture.Height, out var _)
            : CurrentCamera.GetProjectionMat(_canvasWidth, _canvasHeight, out var _);

        }

        /// <summary>
        /// Sets the state of the model matrices and UiRects.
        /// </summary>
        /// <param name="ctc">The CanvasTransformComponent.</param>
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

                State.CanvasXForm *= float4x4.CreateTranslation(newRect.Center.x, newRect.Center.y, 0);
                State.Model *= State.CanvasXForm;

                _parentRect = newRect;
                State.UiRect = newRect;
            }
            else if (ctc.CanvasRenderMode == CanvasRenderMode.Screen)
            {
                var invProj = float4x4.Invert(State.Projection);

                var frustumCorners = new float4[4];

                frustumCorners[0] = invProj * new float4(-1, -1, -1, 1); //nbl
                frustumCorners[1] = invProj * new float4(1, -1, -1, 1); //nbr
                frustumCorners[2] = invProj * new float4(-1, 1, -1, 1); //ntl
                frustumCorners[3] = invProj * new float4(1, 1, -1, 1); //ntr

                for (var i = 0; i < frustumCorners.Length; i++)
                {
                    var corner = frustumCorners[i];
                    corner /= corner.w; //world space frustum corners
                    frustumCorners[i] = corner;
                }

                var width = (frustumCorners[0] - frustumCorners[1]).Length;
                var height = (frustumCorners[0] - frustumCorners[2]).Length;

                var zNear = frustumCorners[0].z;
                var canvasPos = new float3(InvView.M14, InvView.M24, InvView.M34 + zNear);

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
                State.CanvasXForm *= State.Model.Invert() * InvView * float4x4.CreateTranslation(0, 0, zNear + (zNear * 0.01f));
                State.Model *= State.CanvasXForm;

                _parentRect = newRect;
                State.UiRect = newRect;
            }
        }

        /// <summary>
        /// If a RectTransformComponent is visited the model matrix and MinMaxRect get updated in the <see cref="RendererState"/>.
        /// </summary>
        /// <param name="rtc">The XFormComponent.</param>
        [VisitMethod]
        public void RenderRectTransform(RectTransform rtc)
        {
            MinMaxRect newRect;
            if (_ctc?.CanvasRenderMode == CanvasRenderMode.Screen)
            {
                newRect = new MinMaxRect
                {
                    Min = State.UiRect.Min + State.UiRect.Size * rtc.Anchors.Min + (rtc.Offsets.Min / _ctc.Scale.x),
                    Max = State.UiRect.Min + State.UiRect.Size * rtc.Anchors.Max + (rtc.Offsets.Max / _ctc.Scale.y)
                };
            }
            else
            {
                // The Heart of the UiRect calculation: Set anchor points relative to parent
                // rectangle and add absolute offsets
                newRect = new MinMaxRect
                {
                    Min = State.UiRect.Min + State.UiRect.Size * rtc.Anchors.Min + rtc.Offsets.Min,
                    Max = State.UiRect.Min + State.UiRect.Size * rtc.Anchors.Max + rtc.Offsets.Max
                };
            }

            var translationDelta = newRect.Center - State.UiRect.Center;
            var translationX = translationDelta.x / State.UiRect.Size.x;
            var translationY = translationDelta.y / State.UiRect.Size.y;

            _parentRect = State.UiRect;
            State.UiRect = newRect;

            State.Model *= float4x4.CreateTranslation(translationX, translationY, 0);
        }

        /// <summary>
        /// If a XFormComponent is visited the model matrix gets updated in the <see cref="RendererState"/> and set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="xfc">The XFormComponent.</param>
        [VisitMethod]
        public void RenderXForm(XForm xfc)
        {
            float4x4 scale;

            if (State.UiRect.Size != _parentRect.Size)
            {
                var scaleX = State.UiRect.Size.x / _parentRect.Size.x;
                var scaleY = State.UiRect.Size.y / _parentRect.Size.y;
                scale = float4x4.CreateScale(scaleX, scaleY, 1);
            }
            else
            {
                scale = State.UiRect.Size == _parentRect.Size && xfc.Name.Contains("Canvas")
                    ? float4x4.CreateScale(State.UiRect.Size.x, State.UiRect.Size.y, 1)
                    : float4x4.CreateScale(1, 1, 1);
            }

            State.Model *= scale;
        }

        /// <summary>
        /// If a XFormTextComponent is visited the model matrix gets updated in the <see cref="RendererState"/> and set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="xfc">The XFormTextComponent.</param>
        [VisitMethod]
        public void RenderXFormText(XFormText xfc)
        {
            var zNear = (InvProjection * new float4(-1, -1, -1, 1)).z;
            var scaleFactor = zNear / 100;
            var invScaleFactor = 1 / scaleFactor;

            float translationY;
            float translationX;

            float scaleX;
            float scaleY;

            if (_ctc?.CanvasRenderMode == CanvasRenderMode.Screen)
            {
                //Undo parent scale
                scaleX = 1 / State.UiRect.Size.x;
                scaleY = 1 / State.UiRect.Size.y;

                //Calculate translation according to alignment
                translationX = xfc.HorizontalAlignment switch
                {
                    HorizontalTextAlignment.Left => -State.UiRect.Size.x / 2,
                    HorizontalTextAlignment.Center => -xfc.Width / 2,
                    HorizontalTextAlignment.Right => State.UiRect.Size.x / 2 - xfc.Width,
                    _ => throw new ArgumentException("Invalid Horizontal Alignment"),
                };
                translationY = xfc.VerticalAlignment switch
                {
                    VerticalTextAlignment.Top => State.UiRect.Size.y / 2,
                    VerticalTextAlignment.Center => xfc.Height / 2,
                    VerticalTextAlignment.Bottom => xfc.Height - (State.UiRect.Size.y / 2),
                    _ => throw new ArgumentException("Invalid Horizontal Alignment"),
                };
            }
            else
            {
                //Undo parent scale, scale by distance
                scaleX = 1 / State.UiRect.Size.x * scaleFactor;
                scaleY = 1 / State.UiRect.Size.y * scaleFactor;

                //Calculate translation according to alignment by scaling the rectangle size
                translationX = xfc.HorizontalAlignment switch
                {
                    HorizontalTextAlignment.Left => -State.UiRect.Size.x * invScaleFactor / 2,
                    HorizontalTextAlignment.Center => -xfc.Width / 2,
                    HorizontalTextAlignment.Right => State.UiRect.Size.x * invScaleFactor / 2 - xfc.Width,
                    _ => throw new ArgumentException("Invalid Horizontal Alignment"),
                };
                translationY = xfc.VerticalAlignment switch
                {
                    VerticalTextAlignment.Top => State.UiRect.Size.y * invScaleFactor / 2,
                    VerticalTextAlignment.Center => xfc.Height / 2,
                    VerticalTextAlignment.Bottom => xfc.Height - (State.UiRect.Size.y * invScaleFactor / 2),
                    _ => throw new ArgumentException("Invalid Horizontal Alignment"),
                };
            }

            var translation = float4x4.CreateTranslation(translationX, translationY, 0);
            var scale = float4x4.CreateScale(scaleX, scaleY, 1);

            State.Model *= scale;
            State.Model *= translation;
        }

        /// <summary>
        /// If a TransformComponent is visited the model matrix of the <see cref="RenderContext"/> and <see cref="RendererState"/> is updated.
        /// It additionally updates the view matrix of the RenderContext.
        /// </summary>
        /// <param name="transform">The TransformComponent.</param>
        [VisitMethod]
        public void RenderTransform(Transform transform)
        {
            State.Model *= transform.Matrix;
        }

        /// <summary>
        /// Save the current shader effect
        /// Later we can check if we have a geometry shader source and use FBO picking
        /// </summary>
        /// <param name="effect"></param>
        [VisitMethod]
        public void RenderShaderEffect(ShaderEffect effect)
        {
            State.ShaderEffect = effect;
        }

        /// <summary>
        /// Handles custom pick component with pick layer and custom picking methods.
        /// If <see cref="PickComponent"/> is not active, the picking is being skipped
        /// </summary>
        /// <param name="comp"></param>
        [VisitMethod]
        public void HandlePickComponent(PickComponent comp)
        {
            State.CurrentPickComp = comp;
        }

        /// <summary>
        /// Creates pick results from a given mesh if it is within the pick position.
        /// </summary>
        /// <param name="mesh">The given Mesh.</param>
        [VisitMethod]
        public void HandleMesh(Mesh mesh)
        {
            if (State.CurrentPickComp?.Active == false) return;

            if (!mesh.Active) return;
            if (mesh == null) return;

            if (State?.CurrentPickComp != null)
            {
                if (State?.CurrentPickComp?.CustomPickMethod != null)
                {
                    var res = State?.CurrentPickComp?.CustomPickMethod(mesh, CurrentNode, State.Model, State.View, State.Projection, PickPosClip);
                    if (res != null)
                    {
                        YieldItem(res);
                        return;
                    }
                }
            }

            switch (mesh.MeshType)
            {
                // we should be able to pick all Triangle* with Raycast
                case PrimitiveType.Triangles:
                case PrimitiveType.TriangleFan:
                case PrimitiveType.TriangleStrip:
                    if (State != null)
                        PickTriangleGeometry(mesh, State.Projection, State.View);
                    break;
                // everything else should be pickable with an color coded FBO at mouse position
                // point cloud will be picked via PointCloudPicker Module
                case PrimitiveType.Points:
                    Diagnostics.Warn($"Unknown primitive type {mesh.MeshType}, picking not possible!");
                    break;
                default:
                    // check if we have a geometry shader --> use FBO picking
                    if (State == null) return;
                    if (State.ShaderEffect.GeometryShaderSrc == null) return;

                    // non-triangle picking is only possible with camera
                    Guard.IsNotNull(CurrentCamera, nameof(CurrentCamera));
                    Guard.IsNotNull(State, nameof(State));

                    var cam = new Camera(CurrentCamera.ProjectionMethod, 1, 1000, CurrentCamera.Fov);

                    // if scene renderer is empty, generate new with camera and custom shader stuff
                    // attach current scene node to shader
                    var container = new SceneContainer
                    {
                        Children = new List<SceneNode>
                       {
                            new SceneNode
                            {
                                Components = new List<SceneComponent>
                                {
                                    _cameraPosition,
                                    cam
                                }
                            }
                       }
                    };
                    var w = _canvasWidth;
                    var h = _canvasHeight;
                    _writableTexture ??= WritableTexture.CreateAlbedoTex(w, h, new ImagePixelFormat(ColorFormat.Intensity));
                    _cameraPosition.Matrix = State.View.Invert();
                    cam.RenderTexture = _writableTexture;
                    cam.FrustumCullingOn = false;
                    container.Children.Insert(1, CurrentNode);

                    _sceneRenderer = new SceneRendererForward(container);
                    _sceneRenderer.Render(_rc);

                    // convert mouse from ClipSpace back to world coordinates
                    // reverse pickPosClip calculation
                    // var pickPosClip = (mousePos * new float2(2.0f / canvasWidth, -2.0f / canvasHeight)) + new float2(-1, 1);
                    var mouseModelPos = PickPosClip - new float2(-1, 1);
                    mouseModelPos = new float2(mouseModelPos.x / (2.0f / w), mouseModelPos.y / (-2.0f / h));

                    // read pixel value at mouse coordinates/position
                    var pixels = _rc.ReadPixels((int)mouseModelPos.x, (int)mouseModelPos.y, new ImagePixelFormat(ColorFormat.Intensity), 1, 1);

                    if (pixels[0] != 0)
                        YieldItem(new PickResult
                        {
                            Mesh = mesh,
                            Node = CurrentNode,
                            Model = State.Model
                        });
                    break;
            }
        }

        private void PickLineGeometry(Mesh mesh)
        {
            var mvp = State.Projection * State.View * State.Model;
            var matOfNode = CurrentNode.GetComponent<ShaderEffect>();
            if (matOfNode == null)
            {
                Diagnostics.Debug("No shader effect for line renderer found!");
                return;
            }
            var thicknessFromShader = matOfNode.GetFxParam<float>("Thickness");

            if (mesh.Triangles == null) return;
            if (mesh.Vertices == null) return;
            if (CurrentCamera == null)
            {
                Diagnostics.Warn("No camera found in SceneGraph, no picking possible!");
                return;
            }

            for (var i = 0; i < mesh.Triangles.Length; i += 2)
            {
                var viewportHeight = CurrentCamera.Viewport.w;
                var thickness = (thicknessFromShader / viewportHeight);

                var pt1 = float4x4.TransformPerspective(mvp, mesh.Vertices[(int)mesh.Triangles[i + 0]]).xy;
                var pt2 = float4x4.TransformPerspective(mvp, mesh.Vertices[(int)mesh.Triangles[i + 1]]).xy;
                var pt0 = State.PickPosClip;

                // Line Eq = ax + by + c = 0
                // A = (y1 - y2)
                // B = (x2 - x1)
                // C = (x1 * y2 - x2 * y1)

                // dist(line, pt) = |Ax + By + C| / A² + B²
                var a = pt1.y - pt2.y;
                var b = pt2.x - pt1.x;
                var c = (pt1.x * pt2.y) - (pt2.x * pt1.y);

                var d = MathF.Abs((a * pt0.x) + (b * pt0.y) + c) / ((a * a) + (b * b));

                if (d <= thickness)
                {
                    YieldItem(new PickResult
                    {
                        Mesh = mesh,
                        Node = CurrentNode,
                        Model = State.Model,
                        ClipPos = float4x4.TransformPerspective(State.Projection * State.View, CurrentNode.GetTransform().Translation),
                        View = State.View,
                        Projection = State.Projection
                    });
                }
            }
        }


        private void PickTriangleGeometry(Mesh mesh, float4x4 projectionMatrix, float4x4 viewMatrix)
        {
            if (mesh == null) return;
            if (mesh.Triangles == null) return;
            if (mesh.Vertices == null) return;

            if (mesh.BoundingBox == default)
            {
                Diagnostics.Warn($"Current bounding box of {mesh} is default while mesh is being picked. Generating box ...");
                mesh.BoundingBox = new(mesh.Vertices.AsReadOnlySpan);
            }

            if (mesh.BoundingBox.Size.x <= 0 || mesh.BoundingBox.Size.y <= 0 || mesh.BoundingBox.Size.z <= 0)
            {
                Diagnostics.Warn($"Current bounding box of {mesh} is smaller or equal to zero. Forcing a thickness in zero direction of >= float.Epsilon");
                var maxX = mesh.BoundingBox.Size.x <= 0 ? float.Epsilon : mesh.BoundingBox.max.x;
                var maxY = mesh.BoundingBox.Size.y <= 0 ? float.Epsilon : mesh.BoundingBox.max.y;
                var maxZ = mesh.BoundingBox.Size.z <= 0 ? float.Epsilon : mesh.BoundingBox.max.z;

                var minX = mesh.BoundingBox.Size.x <= 0 ? 0 : mesh.BoundingBox.min.x;
                var minY = mesh.BoundingBox.Size.y <= 0 ? 0 : mesh.BoundingBox.min.y;
                var minZ = mesh.BoundingBox.Size.z <= 0 ? 0 : mesh.BoundingBox.min.z;

                mesh.BoundingBox = new AABBf(new float3(minX, minY, minZ), new float3(maxX, maxY, maxZ));
            }

            var ray = new RayF(PickPosClip, viewMatrix, projectionMatrix);
            var box = State.Model * mesh.BoundingBox;
            // does not work for Planes or Ortographic Cameras!
            if (!box.IntersectRay(ray))
                return;

            for (int i = 0; i < mesh.Triangles.Length; i += 3)
            {
                // Vertices of the picked triangle in world space
                var a = new float3(mesh.Vertices[(int)mesh.Triangles[i + 0]]);
                a = float4x4.Transform(State.Model, a);

                var b = new float3(mesh.Vertices[(int)mesh.Triangles[i + 1]]);
                b = float4x4.Transform(State.Model, b);

                var c = new float3(mesh.Vertices[(int)mesh.Triangles[i + 2]]);
                c = float4x4.Transform(State.Model, c);

                // Normal of the plane defined by a, b, and c.
                var n = float3.Normalize(float3.Cross(a - c, b - c));


                // Distance between "Origin" and the plane abc when following the Direction.
                var distance = -float3.Dot(ray.Origin - a, n) / float3.Dot(ray.Direction, n);

                // does not work for Planes or Ortographic Cameras!
                if (distance < 0)
                    continue;

                // Position of the intersection point between ray and plane.
                var point = ray.Origin + (ray.Direction * distance);

                if (float3.PointInTriangle(a, b, c, point, out float u, out float v))
                {
                    if (State.CullMode == Cull.None || (State.CullMode == Cull.Clockwise) == (float3.Dot(n, ray.Direction) < 0))
                    {
                        YieldItem(new MeshPickResult
                        {
                            Mesh = mesh,
                            Node = CurrentNode,
                            Triangle = i,
                            Model = State.Model,
                            U = u,
                            V = v,
                            DistanceFromOrigin = distance
                        });
                    }
                }
            }
        }

        #endregion

    }
}