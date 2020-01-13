using System.Collections.Generic;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Implements the pick result and relevant information about it.
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
        /// The triangle.
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

        #region State
        /// <summary>
        /// The picker state upon scene traversal.
        /// </summary>
        public class PickerState : VisitorState
        {
            private CollapsingStateStack<float4x4> _canvasXForm = new CollapsingStateStack<float4x4>();
            private CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();
            private CollapsingStateStack<MinMaxRect> _uiRect = new CollapsingStateStack<MinMaxRect>();

            /// <summary>
            /// The registered model.
            /// </summary>
            public float4x4 Model
            {
                set { _model.Tos = value; }
                get { return _model.Tos; }
            }

            /// <summary>
            /// The registered ui rectangle.
            /// </summary>
            public MinMaxRect UiRect
            {
                set { _uiRect.Tos = value; }
                get { return _uiRect.Tos; }
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
            /// The default constructor for the <see cref="PickerState"/> class, which registers state stacks for mode, ui rectangle, and canvas transform.
            /// </summary>
            public PickerState()
            {
                RegisterState(_model);
                RegisterState(_uiRect);
                RegisterState(_canvasXForm);
            }
        };

        /// <summary>
        /// The view and projection matrices.
        /// </summary>
        public float4x4 View, Projection;
        #endregion

        /// <summary>
        /// The constructor to initialize a new ScenePicker.
        /// </summary>
        /// <param name="scene">The <see cref="SceneContainer"/> to pick from.</param>
        public ScenePicker(SceneContainer scene)
            : base(scene.Children.GetEnumerator())
        {
            View = float4x4.Identity;
            Projection = float4x4.Identity;
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
        /// <param name="pickPos">The pick position.</param>
        /// <returns></returns>
        public IEnumerable<PickResult> Pick(float2 pickPos)
        {
            PickPosClip = pickPos;
            return Viserate();
        }


        #region Visitors

        /// <summary>
        /// Sets the Projection matrix according to the projection method of a given ProjectionComponent. (Can be perspective or orthographic).
        /// </summary>
        /// <param name="pc">The given ProjectionComponent.</param>
        [VisitMethod]
        public void PickProjection(ProjectionComponent pc)
        {
            switch (pc.ProjectionMethod)
            {
                case ProjectionMethod.PERSPECTIVE:
                    var aspect = pc.Width / (float)pc.Height;
                    Projection = float4x4.CreatePerspectiveFieldOfView(pc.Fov, aspect, pc.ZNear, pc.ZFar);                    
                    break;
                case ProjectionMethod.ORTHOGRAPHIC:
                    Projection = float4x4.CreateOrthographic(pc.Width, pc.Height, pc.ZNear, pc.ZFar);                    
                    break;
            }
        }

        /// <summary>
        /// Transforms the Model matrix of the PickerState by multiplying it with the matrix of the given TransformComponent.
        /// </summary>
        /// <param name="transform"> The given TransformComponent.</param>
        [VisitMethod]
        public void PickTransform(TransformComponent transform)
        {
            State.Model *= transform.Matrix();
        }

        private bool isCtcInitialized = false;
        /// <summary>
        /// Transforms the CanvasXForm, Model, and UiRect matrices according to the canvas render mode of a given CanvasTransformComponent. (Can be "WORLD" or "SCREEN").
        /// </summary>
        /// <param name="ctc">The given CanvasTransformComponent.</param>
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

        /// <summary>
        /// Sets the UiRect accordingly to the canvas render mode and a given RectTransformComponent's anchors and offsets.
        /// </summary>
        /// <param name="rtc">The given RectTransformComponent.</param>
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

        /// <summary>
        /// Creates pick results from a given mesh if it is within the pick position.
        /// </summary>
        /// <param name="mesh">The given Mesh.</param>
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

        /// <summary>
        /// The pick position on the screen.
        /// </summary>
        public float2 PickPosClip { get; set; }

        #endregion

    }
}



//public class ScenePicker : Viserator<PickResult, ScenePicker.PickingState>
//{
//    public class PickingState : VisitorState
//    {
//        private CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();
//        private CollapsingStateStack<float4x4> _view = new CollapsingStateStack<float4x4>();
//        private CollapsingStateStack<float4x4> _projection = new CollapsingStateStack<float4x4>();

//        public float4x4 Model
//        {
//            set { _model.Tos = value; }
//            get { return _model.Tos; }
//        }
//        public float4x4 View
//        {
//            set { _view.Tos = value; }
//            get { return _view.Tos; }
//        }

//        public float4x4 Projection
//        {
//            set { _projection.Tos =  value; }
//            get { return _projection.Tos; }
//        }

//        public PickingState()
//        {
//            RegisterState(_model);
//            RegisterState(_view);
//            RegisterState(_projection);
//        }
//    }


//    #region Visitors
//    [VisitMethod]
//    public void PickTransform(TransformComponent transform)
//    {
//        State.Model *= transform.Matrix();
//    }

//    [VisitMethod]
//    public void PickMesh(MeshComponent mesh)
//    {
//        float4x4 mvp = State.Projection * State.View * State.Model;
//        for (int i = 0; i < mesh.Triangles.Length; i += 3)
//        {
//            // a, b c: current triangle's vertices in clip coordinates
//            float4 a = new float4(mesh.Vertices[mesh.Triangles[i + 0]], 1).TransformPerspective(mvp);
//            float4 b = new float4(mesh.Vertices[mesh.Triangles[i + 1]], 1).TransformPerspective(mvp);
//            float4 c = new float4(mesh.Vertices[mesh.Triangles[i + 2]], 1).TransformPerspective(mvp);

//            float u, v;
//            // Point-in-Triangle-Test
//            if (float2.PointInTriangle(a.xy, b.xy, c.xy, PickPosClip, out u, out v))
//            {
//                YieldItem(new PickResult
//                     {
//                         Mesh = mesh,
//                         Node = CurrentNode,
//                         Triangle = i,
//                         Model = State.Model,
//                         View = State.View,
//                         Projection = State.Projection,
//                         U = u,
//                         V = v
//                     });
//            }
//        }
//    }

//    public float2 PickPosClip { get; set; }

//    #endregion

//}
