using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public SceneNode Node;

        /// <summary>
        /// The mesh.
        /// </summary>
        public Mesh Mesh;

        /// <summary>
        /// The index of the triangle that was picked.
        /// </summary>
        public int Triangle;

        /// <summary>
        /// The barycentric u, v coordinates within the picked triangle.
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
                GetTriangle(out var a, out var b, out var c);
                return (a + b + c) / 3;
            }
        }
        /// <summary>
        /// Returns the barycentric triangle coordinates.
        /// </summary>
        public float3 TriangleBarycentric
        {
            get
            {
                GetTriangle(out var a, out var b, out var c);
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
                GetNormals(out var a, out var b, out var c);
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
                GetNormals(out var a, out var b, out var c);
                return float3.Barycentric(a, b, c, U, V);
            }
        }
        /// <summary>
        /// Returns the model position.
        /// </summary>
        public float3 ModelPos => TriangleBarycentric;
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
        public float3 WorldPos => float4x4.TransformPerspective(Model, ModelPos);
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
        /// <summary>
        /// 
        /// </summary>
        public float2 UV
        {
            get
            {
                float2 uva = Mesh.UVs[Mesh.Triangles[Triangle]];
                float2 uvb = Mesh.UVs[Mesh.Triangles[Triangle + 1]];
                float2 uvc = Mesh.UVs[Mesh.Triangles[Triangle + 2]];

                return float2.Barycentric(uva, uvb, uvc, U, V);
            }
        }
    }

    /// <summary>
    /// Implements the scene picker.
    /// </summary>
    public class ScenePicker : Viserator<PickResult, ScenePicker.PickerState, SceneNode, SceneComponent>
    {
        private CanvasTransform _ctc;
        private RenderContext _rc;

        private bool isCtcInitialized = false;
        private MinMaxRect _parentRect;

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
            /// The default constructor for the <see cref="PickerState"/> class, which registers state stacks for model, UI rectangle, and canvas transform, as well as cull mode.
            /// </summary>
            public PickerState()
            {
                RegisterState(_model);
                RegisterState(_uiRect);
                RegisterState(_canvasXForm);
                RegisterState(_cullMode);
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
            : base(scene.Children)
        {
            IgnoreInactiveComponents = true;
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
            State.CullMode = _rc != null ? (Cull)_rc.GetRenderState(RenderState.CullMode) : Cull.None;
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
                var invProj = float4x4.Invert(_rc.Projection);

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
                State.CanvasXForm *= _rc.InvModel * _rc.InvView * float4x4.CreateTranslation(0, 0, zNear + (zNear * 0.01f));
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
            if (_ctc.CanvasRenderMode == CanvasRenderMode.Screen)
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
            else if (State.UiRect.Size == _parentRect.Size && xfc.Name.Contains("Canvas"))
            {
                scale = float4x4.CreateScale(State.UiRect.Size.x, State.UiRect.Size.y, 1);
            }
            else
            {
                scale = float4x4.CreateScale(1, 1, 1);
            }

            State.Model *= scale;
            _rc.Model = State.Model;
        }

        /// <summary>
        /// If a XFormTextComponent is visited the model matrix gets updated in the <see cref="RendererState"/> and set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="xfc">The XFormTextComponent.</param>
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
            _rc.Model = State.Model;
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
            _rc.Model = State.Model;
        }

        /// <summary>
        /// Creates pick results from a given mesh if it is within the pick position.
        /// </summary>
        /// <param name="mesh">The given Mesh.</param>
        [VisitMethod]
        public void PickMesh(Mesh mesh)
        {
            if (!mesh.Active ||
                (mesh.MeshType != (int)OpenGLPrimitiveType.Triangles &&
                mesh.MeshType != (int)OpenGLPrimitiveType.TriangleFan &&
                mesh.MeshType != (int)OpenGLPrimitiveType.TriangleStrip)) return;

            var mvp = Projection * View * State.Model;
            for (var i = 0; i < mesh.Triangles.Length; i += 3)
            {
                // a, b c: current triangle's vertices in clip coordinates
                var a = new float4(mesh.Vertices[mesh.Triangles[i + 0]], 1);
                a = float4x4.TransformPerspective(mvp, a);

                var b = new float4(mesh.Vertices[mesh.Triangles[i + 1]], 1);
                b = float4x4.TransformPerspective(mvp, b);

                var c = new float4(mesh.Vertices[mesh.Triangles[i + 2]], 1);
                c = float4x4.TransformPerspective(mvp, c);

                // Point-in-Triangle-Test
                if (float2.PointInTriangle(a.xy, b.xy, c.xy, PickPosClip, out var u, out var v))
                {
                    var pickPos = float3.Barycentric(a.xyz, b.xyz, c.xyz, u, v);

                    if (pickPos.z >= -1 && pickPos.z <= 1)
                    {
                        if (State.CullMode == Cull.None || float2.IsTriangleCW(a.xy, b.xy, c.xy) == (State.CullMode == Cull.Clockwise))
                        {
                            YieldItem(new PickResult
                            {
                                Mesh = mesh,
                                Node = CurrentNode,
                                Triangle = i,
                                Model = State.Model,
                                View = View,
                                Projection = Projection,
                                U = u,
                                V = v
                            });
                        }
                    }
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