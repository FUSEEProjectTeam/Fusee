using System;
using System.Collections.Generic;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    public struct PickResult
    {
        public SceneNodeContainer Node;
        public MeshComponent Mesh;
        public int Triangle;
        public float WA, WB, WC;

        // TODO: Implement
        public float3 WorldPos
        { get { throw new NotImplementedException(); } }

        // TODO: Implement
        public float3 ModelPos
        { get { throw new NotImplementedException(); } }

        // TODO: Implement
        public float4 ScreenPos
        { get { throw new NotImplementedException();} }
     }


    public class ScenePicker : Viserator<PickResult, ScenePicker.PickingState>
    {
        public class PickingState : VisitorState
        {
            private CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();
            private CollapsingStateStack<float4x4> _view = new CollapsingStateStack<float4x4>();
            private CollapsingStateStack<float4x4> _projection = new CollapsingStateStack<float4x4>();

            public float4x4 Model
            {
                set { _model.Tos = value; }
                get { return _model.Tos; }
            }

            public float4x4 View
            {
                set { _view.Tos = value; }
                get { return _view.Tos; }
            }

            public float4x4 Projection
            {
                set { _projection.Tos =  value; }
                get { return _projection.Tos; }
            }

            public PickingState()
            {
                RegisterState(_model);
                RegisterState(_view);
                RegisterState(_projection);
            }
        }

        public ScenePicker(IEnumerator<SceneNodeContainer> rootList)
            : base(rootList)
        {
            State.Model = float4x4.Identity;
        }

        #region Visitors
        [VisitMethod]
        public void PickTransform(TransformComponent transform)
        {
            State.Model *= transform.Matrix();
        }

        [VisitMethod]
        public void PickMesh(MeshComponent meshComponent)
        {
            // Mesh rm;

            // TODO: DO THE PICK TEST HERE
            // foreach triangle
            // {
            //   if (triangle is hit by pickpos)
            //   {
            //     YieldItem(new PickResult
            //          {
            //              Mesh = meshComponent,
            //              Node = CurrentNode,
            //              Triangle = TODO,
            //              WA = TODO,
            //              WB = TODO,
            //              WC = TODO
            //          });
            //   }
            // }
        }
        #endregion
 
    }
}
