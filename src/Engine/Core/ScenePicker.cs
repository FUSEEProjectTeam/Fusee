using System;
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

        private bool isCtcInitialized = false;
        private MinMaxRect _parentRect;

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
        /// The current view matrix.
        /// </summary>
        public float4x4 View { get; private set; }

        /// <summary>
        /// The current projection matrix.
        /// </summary>
        public float4x4 Projection { get; private set; }

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
        /// <param name="rc"></param>
        /// <param name="pickPos">The pick position.</param>
        /// <returns></returns>
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

                State.CanvasXForm *= float4x4.CreateTranslation(newRect.Center.x, newRect.Center.y, 0);
                State.Model *= State.CanvasXForm;

                _parentRect = newRect;
                State.UiRect = newRect;
            }

            if (ctc.CanvasRenderMode == CanvasRenderMode.SCREEN)
            {
                var invProj = float4x4.Invert(_rc.Projection);

                var frustumCorners = new float4[4];

                frustumCorners[0] = invProj * new float4(-1, -1, -1, 1); //nbl
                frustumCorners[1] = invProj * new float4(1, -1, -1, 1); //nbr 
                frustumCorners[2] = invProj * new float4(-1, 1, -1, 1); //ntl  
                frustumCorners[3] = invProj * new float4(1, 1, -1, 1); //ntr                

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
                State.CanvasXForm *= _rc.InvView * float4x4.CreateTranslation(0, 0, zNear + (zNear * 0.01f));
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
        public void RenderRectTransform(RectTransformComponent rtc)
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

            _parentRect = State.UiRect;
            State.UiRect = newRect;

            State.Model *= float4x4.CreateTranslation(translationX, translationY, 0);
        }

        /// <summary>
        /// If a XFormComponent is visited the model matrix gets updated in the <see cref="RendererState"/> and set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="xfc">The XFormComponent.</param>
        [VisitMethod]
        public void RenderXForm(XFormComponent xfc)
        {
            float4x4 scale;

            if (State.UiRect.Size != _parentRect.Size)
            {
                var scaleX = State.UiRect.Size.x / _parentRect.Size.x;
                var scaleY = State.UiRect.Size.y / _parentRect.Size.y;
                scale = float4x4.CreateScale(scaleX, scaleY, 1);
            }
            else if (State.UiRect.Size == _parentRect.Size && xfc.Name.Contains("Canvas"))
                scale = float4x4.CreateScale(State.UiRect.Size.x, State.UiRect.Size.y, 1);
            else
                scale = float4x4.CreateScale(1, 1, 1);

            State.Model *= scale;
            _rc.Model = State.Model;
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
                scaleX = 1 / State.UiRect.Size.x;
                scaleY = 1 / State.UiRect.Size.y;

                //Calculate translation according to alignment
                switch (xfc.HorizontalAlignment)
                {
                    case HorizontalTextAlignment.LEFT:
                        translationX = -State.UiRect.Size.x / 2;
                        break;
                    case HorizontalTextAlignment.CENTER:
                        translationX = -xfc.Width / 2;
                        break;
                    case HorizontalTextAlignment.RIGHT:
                        translationX = State.UiRect.Size.x / 2 - xfc.Width;
                        break;
                    default:
                        throw new ArgumentException("Invalid Horizontal Alignment");
                }

                switch (xfc.VerticalAlignment)
                {
                    case VerticalTextAlignment.TOP:
                        translationY = State.UiRect.Size.y / 2;
                        break;
                    case VerticalTextAlignment.CENTER:
                        translationY = xfc.Height / 2;
                        break;
                    case VerticalTextAlignment.BOTTOM:
                        translationY = xfc.Height - (State.UiRect.Size.y / 2);
                        break;
                    default:
                        throw new ArgumentException("Invalid Horizontal Alignment");
                }
            }
            else
            {
                //Undo parent scale, scale by distance
                scaleX = 1 / State.UiRect.Size.x * scaleFactor;
                scaleY = 1 / State.UiRect.Size.y * scaleFactor;

                //Calculate translation according to alignment by scaling the rectangle size
                switch (xfc.HorizontalAlignment)
                {
                    case HorizontalTextAlignment.LEFT:
                        translationX = -State.UiRect.Size.x * invScaleFactor / 2;
                        break;
                    case HorizontalTextAlignment.CENTER:
                        translationX = -xfc.Width / 2;
                        break;
                    case HorizontalTextAlignment.RIGHT:
                        translationX = State.UiRect.Size.x * invScaleFactor / 2 - xfc.Width;
                        break;
                    default:
                        throw new ArgumentException("Invalid Horizontal Alignment");
                }

                switch (xfc.VerticalAlignment)
                {
                    case VerticalTextAlignment.TOP:
                        translationY = State.UiRect.Size.y * invScaleFactor / 2;
                        break;
                    case VerticalTextAlignment.CENTER:
                        translationY = xfc.Height / 2;
                        break;
                    case VerticalTextAlignment.BOTTOM:
                        translationY = xfc.Height - (State.UiRect.Size.y * invScaleFactor / 2);
                        break;
                    default:
                        throw new ArgumentException("Invalid Horizontal Alignment");
                }
            }

            var translation = float4x4.CreateTranslation(translationX, translationY, 0);
            var scale = float4x4.CreateScale(scaleX, scaleY, 1);

            State.Model *= scale;
            State.Model *= translation;
            _rc.Model = State.Model;
        }

        /// <summary>
        /// If a TransformComponent is visited the model matrix of the <see cref="RenderContext"/> and <see cref="RendererState"/> is updated.
        /// It additionally updates the view matrix of the RenderContext.
        /// </summary> 
        /// <param name="transform">The TransformComponent.</param>
        [VisitMethod]
        public void RenderTransform(TransformComponent transform)
        {
            State.Model *= transform.Matrix();
            _rc.Model = State.Model;
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

