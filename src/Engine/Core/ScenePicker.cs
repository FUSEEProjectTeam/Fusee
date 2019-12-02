using System.Collections.Generic;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

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
        public SceneNodeContainer Node;

        /// <summary>
        /// The mesh.
        /// </summary>
        public Mesh Mesh;

        /// <summary>
        /// The index of the triangle that was picked.
        /// </summary>
        public int Triangle;

        /// <summary>
        /// The u, v coordinates.
        /// </summary>
        public float U, V;

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

        // Convenience
        /// <summary>
        /// Gets the triangles of the picked mesh.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public void GetTriangle(out float3 a, out float3 b, out float3 c)
        {
            a = Mesh.Vertices[Mesh.Triangles[Triangle + 0]];
            b = Mesh.Vertices[Mesh.Triangles[Triangle + 1]];
            c = Mesh.Vertices[Mesh.Triangles[Triangle + 2]];
        }
        /// <summary>
        /// Returns the center of the picked triangle.
        /// </summary>
        public float3 TriangleCenter
        {
            get
            {
                float3 a, b, c;
                GetTriangle(out a, out b, out c);
                return (a + b + c) / 3;
            }
        }
        /// <summary>
        /// Returns the barycentric tiangel coordinates.
        /// </summary>
        public float3 TriangleBarycentric
        {
            get
            {
                float3 a, b, c;
                GetTriangle(out a, out b, out c);
                return float3.Barycentric(a, b, c, U, V);
            }
        }
        /// <summary>
        /// Gets the normals at the picked triangle.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public void GetNormals(out float3 a, out float3 b, out float3 c)
        {
            a = Mesh.Normals[Mesh.Triangles[Triangle + 0]];
            b = Mesh.Normals[Mesh.Triangles[Triangle + 1]];
            c = Mesh.Normals[Mesh.Triangles[Triangle + 2]];
        }
        /// <summary>
        /// Returns the normal at the center of the picked triangle.
        /// </summary>
        public float3 NormalCenter
        {
            get
            {
                float3 a, b, c;
                GetNormals(out a, out b, out c);
                return (a + b + c) / 3;
            }
        }
        /// <summary>
        /// Returns the barycentric normal coordinates.
        /// </summary>
        public float3 NormalBarycentric
        {
            get
            {
                float3 a, b, c;
                GetNormals(out a, out b, out c);
                return float3.Barycentric(a, b, c, U, V);
            }
        }
        /// <summary>
        /// Returns the model position.
        /// </summary>
        public float3 ModelPos
        {
            get
            {
                return TriangleBarycentric;
            }
        }
        /// <summary>
        /// Returns the clipping position of the model.
        /// </summary>
        public float3 ClipPos
        {
            get
            {
                var mat = Projection * View * Model;
                return float4x4.TransformPerspective(mat, ModelPos);
            }
        }
        /// <summary>
        /// Returns the world position of the model.
        /// </summary>
        public float3 WorldPos
        {
            get
            {
                return float4x4.TransformPerspective(Model, ModelPos);
            }
        }
        /// <summary>
        /// Returns the camera position.
        /// </summary>
        public float3 CameraPos
        {
            get
            {
                var mat = View * Model;
                return float4x4.TransformPerspective(mat, ModelPos);
            }
        }
    }

    /// <summary>
    /// Implements the scene picker.
    /// </summary>
    public class ScenePicker : Viserator<PickResult, ScenePicker.PickerState>
    {
        private CanvasTransformComponent _ctc;
        private RenderContext _rc;

        #region State
        public class PickerState : VisitorState
        {
            private CollapsingStateStack<float4x4> _canvasXForm = new CollapsingStateStack<float4x4>();
            private CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();
            private CollapsingStateStack<MinMaxRect> _uiRect = new CollapsingStateStack<MinMaxRect>();

            public float4x4 Model
            {
                set { _model.Tos = value; }
                get { return _model.Tos; }
            }


            public MinMaxRect UiRect
            {
                set { _uiRect.Tos = value; }
                get { return _uiRect.Tos; }
            }

            public float4x4 CanvasXForm
            {
                get => _canvasXForm.Tos;
                set => _canvasXForm.Tos = value;
            }


            public PickerState()
            {
                RegisterState(_model);
                RegisterState(_uiRect);
                RegisterState(_canvasXForm);
            }
        };

        public float4x4 View { get; private set; }
        public float4x4 Projection { get; private set; }
        #endregion

        public ScenePicker(SceneContainer scene)
            : base(scene.Children.GetEnumerator())
        {
            View = float4x4.Identity;
            Projection = float4x4.Identity;
        }

        protected override void InitState()
        {
            base.InitState();
            State.Model = float4x4.Identity;
            State.CanvasXForm = float4x4.Identity;
        }


        public IEnumerable<PickResult> Pick(RenderContext rc, float2 pickPos)
        {
            _rc = rc;
            PickPosClip = pickPos;
            View = _rc.View;
            Projection = _rc.Projection;
            return Viserate();            
        }


        #region Visitors

        /// <summary>
        /// If a Projection Component is visited, the projection matrix is set.
        /// </summary>
        /// <param name="cam">The visited <see cref="CameraComponent"/>.</param>
        [VisitMethod]
        public void PickCamera(CameraComponent cam)
        {
            if (cam.CustomCameraUpdate != null)
            {
                cam.CustomCameraUpdate(out float4x4 proj, out float4 viewport);

                _rc.Projection = proj;
                //_rc.Viewport((int)viewport.x, (int)viewport.y, (int)viewport.z, (int)viewport.w);

                return;
            }

            var startX = (int)(_rc.ViewportWidth * (cam.Viewport.x / 100));
            var startY = (int)(_rc.ViewportHeight * (cam.Viewport.y / 100));
            var width = (int)(_rc.ViewportWidth * (cam.Viewport.z / 100));
            var height = (int)(_rc.ViewportHeight * (cam.Viewport.w / 100));

            //_rc.Viewport(startX, startY, width, height);

            switch (cam.ProjectionMethod)
            {
                default:
                case ProjectionMethod.PERSPECTIVE:
                    _rc.Projection = float4x4.CreatePerspectiveFieldOfView(cam.Fov, (float)width / height, cam.ClippingPlanes.x, cam.ClippingPlanes.y);
                    break;
                case ProjectionMethod.ORTHOGRAPHIC:
                    _rc.Projection = float4x4.CreateOrthographic(width, height, cam.ClippingPlanes.x, cam.ClippingPlanes.y);
                    break;
            }

            _rc.View = float4x4.Invert(State.Model);
        }

        /// <summary>
        /// If a TransformComponent is visited the model matrix of the <see cref="RenderContext"/> and <see cref="RendererState"/> is updated.
        /// It additionally updates the view matrix of the RenderContext.
        /// </summary> 
        /// <param name="transform">The TransformComponent.</param>
        [VisitMethod]
        public void PickTransform(TransformComponent transform)
        {
            State.Model *= transform.Matrix();
            _rc.Model = State.Model;            
        }
        private bool isCtcInitialized = false;

        [VisitMethod]
        public void PickCanvasTransform(CanvasTransformComponent ctc)
        {
            _ctc = ctc;

            if (ctc.CanvasRenderMode == CanvasRenderMode.WORLD)
            {
                var newRect = new MinMaxRect
                {
                    Min = ctc.Size.Min,
                    Max = ctc.Size.Max
                };

                State.CanvasXForm *= float4x4.CreateTranslation(newRect.Center.x, newRect.Center.y, 0) * float4x4.CreateScale(newRect.Size.x, newRect.Size.y, 1);
                State.Model *= State.CanvasXForm;
                State.UiRect = newRect;
            }

            if (ctc.CanvasRenderMode == CanvasRenderMode.SCREEN)
            {
                var projection = Projection;
                var zNear = System.Math.Abs(projection.M34 / (projection.M33 + 1));

                var fov = 2f * System.Math.Atan(1f / projection.M22);
                var aspect = projection.M22 / projection.M11;

                var invView = float4x4.Invert(View);
                var canvasPos = new float3(invView.M14, invView.M24, invView.M34 + zNear);

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
                State.CanvasXForm *= invView * float4x4.CreateTranslation(0, 0, zNear + 0.00001f) * float4x4.CreateScale(newRect.Size.x, newRect.Size.y, 1);
                State.Model *= State.CanvasXForm;
                State.UiRect = newRect;
            }
        }

        [VisitMethod]
        public void PickRectTransform(RectTransformComponent rtc)
        {
            MinMaxRect newRect;
            if (_ctc.CanvasRenderMode == CanvasRenderMode.SCREEN)
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
            var scaleX = newRect.Size.x / State.UiRect.Size.x;
            var scaleY = newRect.Size.y / State.UiRect.Size.y;

            State.UiRect = newRect;
            State.Model *= float4x4.CreateTranslation(translationX, translationY, 0) * float4x4.CreateScale(scaleX, scaleY, 1);
        }

        [VisitMethod]
        public void PickMesh(Mesh mesh)
        {
            if (!mesh.Active) return;
            var mvp = Projection * View * State.Model;
            for (int i = 0; i < mesh.Triangles.Length; i += 3)
            {
                // a, b c: current triangle's vertices in clip coordinates
                float4 a = new float4(mesh.Vertices[mesh.Triangles[i + 0]], 1);
                a = float4x4.TransformPerspective(mvp, a);

                float4 b = new float4(mesh.Vertices[mesh.Triangles[i + 1]], 1);
                b = float4x4.TransformPerspective(mvp, b);

                float4 c = new float4(mesh.Vertices[mesh.Triangles[i + 2]], 1);
                c = float4x4.TransformPerspective(mvp, c);

                float u, v;
                // Point-in-Triangle-Test
                if (float2.PointInTriangle(a.xy, b.xy, c.xy, PickPosClip, out u, out v))
                {

                    YieldItem(new PickResult
                    {
                        Mesh = mesh,
                        Node = CurrentNode,
                        Triangle = i,
                        Model = State.Model,
                        View = this.View,
                        Projection = this.Projection,
                        U = u,
                        V = v
                    });
                }
            }
        }

        public float2 PickPosClip { get; set; }

        #endregion

    }
}

