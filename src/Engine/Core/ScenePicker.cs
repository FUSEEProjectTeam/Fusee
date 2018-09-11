using System;
using System.Collections.Generic;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    public class PickResult
    {
        // Data
        public SceneNodeContainer Node;
        public Mesh Mesh;
        public int Triangle;
        public float U, V;
        public float4x4 Model;
        public float4x4 View;
        public float4x4 Projection;


        // Convenience
        public void GetTriangle(out float3 a, out float3 b, out float3 c)
        {
            a = Mesh.Vertices[Mesh.Triangles[Triangle + 0]];
            b = Mesh.Vertices[Mesh.Triangles[Triangle + 1]];
            c = Mesh.Vertices[Mesh.Triangles[Triangle + 2]];
        }

        public float3 TriangleCenter
        {
            get
            {
                float3 a, b, c;
                GetTriangle(out a, out b, out c);
                return (a + b + c) / 3;
            }
        }

        public float3 TriangleBarycentric
        {
            get
            {
                float3 a, b, c;
                GetTriangle(out a, out b, out c);
                return float3.Barycentric(a, b, c, U, V);
            }
        }

        public void GetNormals(out float3 a, out float3 b, out float3 c)
        {
            a = Mesh.Normals[Mesh.Triangles[Triangle + 0]];
            b = Mesh.Normals[Mesh.Triangles[Triangle + 1]];
            c = Mesh.Normals[Mesh.Triangles[Triangle + 2]];
        }

        public float3 NormalCenter
        {
            get
            {
                float3 a, b, c;
                GetNormals(out a, out b, out c);
                return (a + b + c) / 3;
            }
        }

        public float3 NormalBarycentric
        {
            get
            {
                float3 a, b, c;
                GetNormals(out a, out b, out c);
                return float3.Barycentric(a, b, c, U, V);
            }
        }

        public float3 ModelPos
        {
            get
            {
                return TriangleBarycentric;
            }
        }

        public float3 ClipPos
        {
            get
            {
                return ModelPos.TransformPerspective(Projection * View * Model);
            }
        }

        public float3 WorldPos
        {
            get
            {
                return ModelPos.TransformPerspective(Model);
            }
        }

        public float3 CameraPos
        {
            get
            {
                return ModelPos.TransformPerspective(View * Model);
            }
        }
    }


    public class ScenePicker : Viserator<PickResult, ScenePicker.PickerState>
    {
        #region State
        public class PickerState : VisitorState
        {

            private CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();
            public float4x4 Model
            {
                set { _model.Tos = value; }
                get { return _model.Tos; }
            }

            public PickerState()
            {
                RegisterState(_model);
            }
        };

        public float4x4 View, Projection;
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
        }


        public IEnumerable<PickResult> Pick(float2 pickPos)
        {
            PickPosClip = pickPos;
            return Viserate();
        }


        #region Visitors
        [VisitMethod]
        public void PickTransform(TransformComponent transform)
        {
            State.Model *= transform.Matrix();
        }

        [VisitMethod]
        public void PickMesh(Mesh mesh)
        {
            float4x4 mvp = Projection * View * State.Model;
            for (int i = 0; i < mesh.Triangles.Length; i += 3)
            {
                // a, b c: current triangle's vertices in clip coordinates
                float4 a = new float4(mesh.Vertices[mesh.Triangles[i + 0]], 1).TransformPerspective(mvp);
                float4 b = new float4(mesh.Vertices[mesh.Triangles[i + 1]], 1).TransformPerspective(mvp);
                float4 c = new float4(mesh.Vertices[mesh.Triangles[i + 2]], 1).TransformPerspective(mvp);

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
