using CommunityToolkit.Diagnostics;
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

    /// <summary>
    /// A possible line <see cref="PickResult"/>.
    /// Contains specific information for a line geometry.
    /// </summary>
    public class LinePickResult : PickResult
    {
        /// <summary>
        /// The mesh.
        /// </summary>
        public Mesh? Mesh;
    }

    /// <summary>
    /// A possible mesh <see cref="PickResult"/>.
    /// Contains specific information for a triangle geometry.
    /// </summary>
    public class MeshPickResult : PickResult
    {
        /// <summary>
        /// The mesh.
        /// </summary>
        public Mesh? Mesh;
        /// <summary>
        /// The hit triangle.
        /// </summary>
        public int Triangle;
        /// <summary>
        /// U coordinate.
        /// </summary>
        public float U;
        /// <summary>
        /// V coordinate.
        /// </summary>
        public float V;
        /// <summary>
        /// The distance from the <see cref="RayF"/> (mouse position) to the <see cref="Mesh"/>.
        /// </summary>
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

            /// <summary>
            /// The current pick position in clip coordinate space.
            /// </summary>
            public float2 PickPosClip { get; internal set; }

            /// <summary>
            /// The current camera used for picking
            /// </summary>
            public CameraResult CurrentCameraResult { get; internal set; }

            /// <summary>
            /// The current canvas screen size
            /// </summary>
            public int2 ScreenSize { get; internal set; }

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

            /// <summary>
            /// The registered cull mode.
            /// </summary>
            public Cull CullMode
            {
                get => _cullMode.Tos;
                set => _cullMode.Tos = value;
            }

            /// <summary>
            /// The currently bound optional <see cref="PickComponent"/> which can be used for storing custom pick methods as well as a <see cref="PickComponent.PickLayer"/>.
            /// </summary>
            public PickComponent? CurrentPickComp;

            /// <summary>
            /// The default constructor for the <see cref="PickerState"/> class, which registers state stacks for model, UI rectangle, and canvas transform, as well as cull mode.
            /// </summary>
            public PickerState()
            {
                RegisterState(_model);
                RegisterState(_uiRect);
                RegisterState(_canvasXForm);
                RegisterState(_cullMode);
                RegisterState(_shaderFX);
            }
        }

        /// <summary>
        /// The pick position on the screen.
        /// </summary>
        public float2 PickPosClip { get; set; }

        private float4x4 _view;
        private float4x4 _invView;
        private float4x4 _projection;
        private float4x4 _invProj;

        private CameraResult _currentCameraResult;

        internal CameraResult CurrentCameraResult
        {
            get => _currentCameraResult;
            private set
            {
                Guard.IsGreaterThan(_canvasWidth, 0);
                Guard.IsGreaterThan(_canvasHeight, 0);

                _currentCameraResult = value;
                _projection = _currentCameraResult.Camera == null ? float4x4.Identity : _currentCameraResult.Camera.GetProjectionMat(_canvasWidth, _canvasHeight, out _);
                _view = _currentCameraResult.View;
                _invView = _view.Invert();
                _invProj = _projection.Invert();
            }
        }

        private readonly IEnumerable<CameraResult> _prePassResults;

        #endregion


        /// <summary>
        /// The constructor to initialize a new ScenePicker.
        /// </summary>
        /// <param name="scene">The <see cref="SceneContainer"/> to pick from.</param>
        /// <param name="prePassCameraResults">The collected <see cref="IEnumerable{CameraResult}"/> from the <see cref="PrePassVisitor.PrePassTraverse(SceneContainer)"/> functionality.</param>
        /// <param name="cullMode">The <see cref="Mesh"/>'s <see cref="Cull"/> mode.</param>
        /// <param name="customPickModule">Any custom <see cref="IPickerModule"/>s, e. g. a point cloud picker module. Has to be registered from "external" like Desktop.Core.</param>
        public ScenePicker(SceneContainer scene, IEnumerable<CameraResult> prePassCameraResults, Cull cullMode = Cull.None, IEnumerable<IPickerModule>? customPickModule = null)
            : base(scene.Children, customPickModule)
        {
            IgnoreInactiveComponents = true;
            State.CullMode = cullMode;
            _prePassResults = prePassCameraResults;

        }

        /// <summary>
        /// This method is called when traversal starts to initialize the traversal state.
        /// </summary>
        protected override void InitState()
        {
            base.InitState();
            State.Model = float4x4.Identity;
            State.CanvasXForm = float4x4.Identity;
        }

        /// <summary>
        /// Returns a collection of objects that fall in the area of the pick position and that can be iterated over.
        /// </summary>
        /// <param name="pickPos">The pick position in canvas coordinates (e.g. [1270x720]), usually <see cref="Input.Mouse"/>.Position.</param>
        /// <param name="canvasWidth">The width of the current canvas, gets overwrite if a <see cref="Camera.RenderTexture"/> is bound</param>
        /// <param name="canvasHeight">The height of the current canvas, gets overwrite if a <see cref="Camera.RenderTexture"/> is bound</param>
        /// <returns></returns>
        public IEnumerable<PickResult>? Pick(float2 pickPos, int canvasWidth, int canvasHeight)
        {
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;

            float2 pickPosClip;
            if (_prePassResults.Count() == 0)
            {
                Diagnostics.Error("No camera from a PrePassVisitor found. Picking not possible!");
                return null;
            }

            CameraResult pickCam = default;
            Rectangle pickCamRect = new();

            foreach (var camRes in _prePassResults)
            {
                Rectangle camRect = new()
                {
                    Left = (int)(camRes.Camera.Viewport.x * _canvasWidth / 100),
                    Top = (int)(camRes.Camera.Viewport.y * _canvasHeight / 100)
                };
                camRect.Right = ((int)(camRes.Camera.Viewport.z * _canvasWidth) / 100) + camRect.Left;
                camRect.Bottom = ((int)(camRes.Camera.Viewport.w * _canvasHeight) / 100) + camRect.Top;

                if (!float2.PointInRectangle(new float2(camRect.Left, camRect.Top), new float2(camRect.Right, camRect.Bottom), pickPos))
                    continue;

                if (pickCam == default || camRes.Camera.Layer > pickCam.Camera.Layer)
                {
                    pickCam = camRes;
                    pickCamRect = camRect;
                }
            }

            CurrentCameraResult = pickCam;

            pickPosClip = ((pickPos - new float2(pickCamRect.Left, pickCamRect.Top)) * new float2(2.0f / pickCamRect.Width, -2.0f / pickCamRect.Height)) + new float2(-1, 1);
            PickPosClip = pickPosClip;
            State.PickPosClip = pickPosClip;
            State.CurrentCameraResult = pickCam;
            State.ScreenSize = new int2(pickCamRect.Width, pickCamRect.Height);

            SetState();
            var res = Viserate().ToList();
            res.AddRange(CheckVisitorModuleResults());
            return res;
        }

        private IEnumerable<PickResult> CheckVisitorModuleResults()
        {
            var res = new List<PickResult>();
            foreach (var module in VisitorModules)
            {
                var m = (IPickerModule)module;
                if (m.PickResults != null)
                    res.AddRange(m.PickResults);
            }

            return res;
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
                var frustumCorners = new float4[4];

                frustumCorners[0] = _invProj * new float4(-1, -1, -1, 1); //nbl
                frustumCorners[1] = _invProj * new float4(1, -1, -1, 1); //nbr
                frustumCorners[2] = _invProj * new float4(-1, 1, -1, 1); //ntl
                frustumCorners[3] = _invProj * new float4(1, 1, -1, 1); //ntr

                for (var i = 0; i < frustumCorners.Length; i++)
                {
                    var corner = frustumCorners[i];
                    corner /= corner.w; //world space frustum corners
                    frustumCorners[i] = corner;
                }

                var width = (frustumCorners[0] - frustumCorners[1]).Length;
                var height = (frustumCorners[0] - frustumCorners[2]).Length;

                var zNear = frustumCorners[0].z;
                var canvasPos = new float3(_invView.M14, _invView.M24, _invView.M34 + zNear);

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
                State.CanvasXForm *= State.Model.Invert() * _invView * float4x4.CreateTranslation(0, 0, zNear + (zNear * 0.01f));
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
            var zNear = (_invProj * new float4(-1, -1, -1, 1)).z;
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
                    var res = State?.CurrentPickComp?.CustomPickMethod(mesh, CurrentNode, State.Model, _view, _projection, PickPosClip);
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
                        PickTriangleGeometry(mesh);
                    break;
                case PrimitiveType.Lines:
                    PickLineGeometry(mesh);
                    break;
                case PrimitiveType.LineAdjacency:
                    PickLineAdjacencyGeometry(mesh);
                    break;
                // point cloud will be picked via PointCloudPicker Module
                case PrimitiveType.Points:
                    Diagnostics.Warn($"Picking of points not possible! Use the PointCloudPicker module!");
                    break;
                default:
                    Diagnostics.Warn($"Picking failed, unknown primitive type {mesh.MeshType}. Use PickComponent.CustomPickMethod!");
                    break;
            }
        }

        private void PickLineAdjacencyGeometry(Mesh mesh)
        {

            var mvp = _projection * _view * State.Model;
            var matOfNode = CurrentNode.GetComponent<ShaderEffect>();
            if (matOfNode == null)
            {
                Diagnostics.Debug("No shader effect for line renderer found!");
                return;
            }
            var thicknessFromShader = matOfNode.GetFxParam<float>("Thickness");

            if (mesh.Triangles == null) return;
            if (mesh.Vertices == null) return;
            if (CurrentCameraResult == null)
            {
                Diagnostics.Warn("No camera found in SceneGraph, no picking possible!");
                return;
            }

            if (CurrentCameraResult.Camera == default)
                return;

            var size = CurrentCameraResult.Camera.GetViewportInPx(_canvasWidth, _canvasHeight);
            var viewportHeight = size.w;
            var viewportWidth = size.z;
            var aspect = viewportHeight / viewportWidth;
            var line_width = M.Max(1.0f, thicknessFromShader);

            for (var i = 1; i < mesh.Triangles.Length - 1; i += 2)
            {
                // recreate the mesh of the geometry shader, pt in triangle check for three vertices
                // not very perfomant, however this is currently the only way to yield the resulting vertices.
                // The GLSL.TransformFeedback is cluttered with problems and GLSL performance warnings
                var ndc0 = float4x4.TransformPerspective(mvp, mesh.Vertices[(int)mesh.Triangles[i - 1]]);
                var ndc1 = float4x4.TransformPerspective(mvp, mesh.Vertices[(int)mesh.Triangles[i + 0]]);
                var ndc2 = float4x4.TransformPerspective(mvp, mesh.Vertices[(int)mesh.Triangles[i + 1]]);
                var ndc3 = float4x4.TransformPerspective(mvp, mesh.Vertices[(int)mesh.Triangles[i + 2]]);

                //direction of the three segments (previous, current, next) */
                var line_vector0 = ndc1 - ndc0;
                var line_vector1 = ndc2 - ndc1;
                var line_vector2 = ndc3 - ndc2;
                var dir0 = new float2(line_vector0.x, line_vector0.y * aspect).Normalize();
                var dir1 = new float2(line_vector1.x, line_vector1.y * aspect).Normalize();
                var dir2 = new float2(line_vector2.x, line_vector2.y * aspect).Normalize();

                //normals of the three segments (previous, current, next)
                var n0 = new float2(-dir0.y, dir0.x);
                var n1 = new float2(-dir1.y, dir1.x);
                var n2 = new float2(-dir2.y, dir2.x);

                // determine miter lines by averaging the normals of the 2 segments
                var miter_a = (n0 + n1).Normalize();// miter at start of current segment
                var miter_b = (n1 + n2).Normalize();// miter at end of current segment

                // determine the length of the miter by projecting it onto normal and then inverse it
                float an1 = float2.Dot(miter_a, n1);
                float bn1 = float2.Dot(miter_b, n2);
                if (an1 == 0) an1 = 1;
                if (bn1 == 0) bn1 = 1;

                float length_a = line_width / an1;
                if (float2.Dot(dir0, dir1) < -0.1)
                {
                    miter_a = n1;
                    length_a = line_width;
                }

                float length_b = line_width / bn1;
                if (float2.Dot(dir1, dir2) < -0.1)
                {
                    miter_b = n1;
                    length_b = line_width;
                }

                miter_a = new float2(length_a / viewportWidth, length_a / viewportHeight) * miter_a;
                miter_b = new float2(length_b / viewportWidth, length_b / viewportHeight) * miter_b;

                var vert0 = new float3(ndc1.x + miter_a.x, ndc1.y + miter_a.y, ndc1.z);
                var vert1 = new float3(ndc1.x - miter_a.x, ndc1.y - miter_a.y, ndc1.z);
                var vert2 = new float3(ndc2.x + miter_b.x, ndc2.y + miter_b.y, ndc2.z);
                var vert3 = new float3(ndc2.x - miter_b.x, ndc2.y - miter_b.y, ndc2.z);

                if (float2.PointInTriangle(vert0.xy, vert1.xy, vert2.xy, PickPosClip, out _, out _) ||
                    float2.PointInTriangle(vert2.xy, vert1.xy, vert3.xy, PickPosClip, out _, out _))
                {
                    YieldItem(new LinePickResult
                    {
                        Mesh = mesh,
                        Node = CurrentNode,
                        Model = State.Model,
                        ClipPos = float4x4.TransformPerspective(_projection * _view, State.Model.Translation()),
                        View = _view,
                        Projection = _projection
                    });
                }
            }
        }

        private void PickLineGeometry(Mesh mesh)
        {

            var mvp = _projection * _view * State.Model;

            var matOfNode = CurrentNode.GetComponent<ShaderEffect>();
            if (matOfNode == null)
            {
                Diagnostics.Debug("No shader effect for line renderer found!");
                return;
            }
            var thicknessFromShader = matOfNode.GetFxParam<float>("Thickness");

            if (mesh.Triangles == null) return;
            if (mesh.Vertices == null) return;
            if (CurrentCameraResult == null)
            {
                Diagnostics.Warn("No camera found in SceneGraph, no picking possible!");
                return;
            }

            if (CurrentCameraResult.Camera == default)
                return;

            var size = CurrentCameraResult.Camera.GetViewportInPx(_canvasWidth, _canvasHeight);
            var viewportHeight = size.w;
            var viewportWidth = size.z;
            var aspect = viewportHeight / viewportWidth;
            var line_width = M.Max(1.0f, thicknessFromShader);

            for (var i = 0; i < mesh.Triangles.Length; i += 2)
            {
                var pt0 = float4x4.TransformPerspective(mvp, mesh.Vertices[(int)mesh.Triangles[i + 0]]).xy;
                var pt1 = float4x4.TransformPerspective(mvp, mesh.Vertices[(int)mesh.Triangles[i + 1]]).xy;

                var lineVector = pt1 - pt0;
                var dir = new float2(lineVector.x, lineVector.y * aspect).Normalize();

                var normal = new float2(-dir.y, dir.x);
                var normal_a = new float2(line_width / viewportWidth, line_width / viewportHeight) * normal;
                var normal_b = new float2(line_width / viewportWidth, line_width / viewportHeight) * normal;

                var vert0 = pt0 + normal_a;
                var vert1 = pt0 - normal_a;
                var vert2 = pt1 + normal_b;
                var vert3 = pt1 - normal_b;

                if (float2.PointInTriangle(vert0.xy, vert1.xy, vert2.xy, PickPosClip, out _, out _) ||
                   float2.PointInTriangle(vert2.xy, vert1.xy, vert3.xy, PickPosClip, out _, out _))
                {
                    YieldItem(new LinePickResult
                    {
                        Mesh = mesh,
                        Node = CurrentNode,
                        Model = State.Model,
                        ClipPos = float4x4.TransformPerspective(_projection * _view, State.Model.Translation()),
                        View = _view,
                        Projection = _projection
                    });
                }
            }
        }

        /// <summary>
        /// Pick triangle geometry via ray cast.
        /// </summary>
        /// <param name="mesh"></param>
        private void PickTriangleGeometry(Mesh mesh)
        {
            if (mesh == null) return;
            if (mesh.Triangles == null) return;
            if (mesh.Vertices == null) return;

            if (mesh.BoundingBox == default)
            {
                //Diagnostics.Warn($"Current bounding box of {mesh} is default while mesh is being picked. Generating box ...");
                mesh.BoundingBox = new(mesh.Vertices.AsReadOnlySpan);
            }

            if (mesh.GetType() != typeof(Primitives.Plane) && (mesh.BoundingBox.Size.x <= 0f || mesh.BoundingBox.Size.y <= 0f || mesh.BoundingBox.Size.z <= 0f))
            {
                Diagnostics.Warn($"Size of current bounding box is 0 for one or more dimensions. Picking not possible.");
                return;
            }

            var ray = new RayF(PickPosClip, _view, _projection);

            var box = State.Model * mesh.BoundingBox;
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
                            ClipPos = float4x4.TransformPerspective(_projection * _view, State.Model.Translation()),
                            Projection = _projection,
                            View = _view,
                            DistanceFromOrigin = distance
                        });
                    }
                }
            }
        }

        #endregion

    }
}