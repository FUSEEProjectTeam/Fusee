using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;
using Fusee.Serialization;

namespace Fusee.Engine.SimpleScene
{
    /*
    public static class ScenePickerExtensions
    {
        // Unfortunate construct, but there seems no other way. What we really needed here is a MixIn to make 
        // a SceneNodeContainer or SceneContainer implement IEnumerable (afterwards). All C# offers us is to 
        // define ExtensionMethods returning an IEnumerable<>.
        // Thus we need some class to implement that. Here it is - tada:
        internal class PickEnumerable : IEnumerable<PickResult>
        {
            internal IEnumerator<SceneNodeContainer> _rootList;
            public IEnumerator<PickResult> GetEnumerator() { return new ScenePicker(_rootList); }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        public static IEnumerable<PickResult> Pick(this SceneNodeContainer root)
        {
            return new PickEnumerable { _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }

        public static IEnumerable<PickResult> FindNodes<TNode>(this SceneNodeContainer root, Predicate<TNode> match) where TNode : SceneNodeContainer
        {
            return new PickEnumerable> {_rootList = SceneVisitorHelpers.SingleRootEnum(root)};
        }


    }

    public struct PickResult
    {
        public SceneNodeContainer Node;
        public MeshComponent Mesh;
        public int Triangle;
        public float2 BaryCoord;

        public T InterpolatedValue<T>(T v0, T v1, T v2)
        { throw new NotImplementedException(); }
        public float3 WorldPos
        { get { throw new NotImplementedException(); } }

        public float3 ModelPos
        { get { throw new NotImplementedException(); } }

        public float4 ScreenPos
        { get { throw new NotImplementedException();} }
    }

    public class ScenePicker : SceneVisitor
    {
        public class PickingState : VisitorState
        {
            private CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();

            public float4x4 Model
            {
                set { _model.Tos = value; }
                get { return _model.Tos; }
            }

            public PickingState()
            {
                RegisterState(_model);
            }
        }

        private PickingState _state;
        private float4x4 _view;
        private float4x4 _projection;

        public ScenePicker(IEnumerable<SceneNodeContainer> rootList)
        {
        }

        protected override void InitState()
        {
            _state.Clear();
            _state.Model = float4x4.Identity;
        }

        protected override void PushState()
        {
            _state.Push();
        }

        protected override void PopState()
        {
            _state.Pop();
        }



        #region Visitors
        [VisitMethod]
        public void PickNode(SceneNodeContainer node)
        {
            _state.Model *= node.Transform.Matrix();
        }

        [VisitMethod]
        public void PickMesh(MeshComponent meshComponent)
        {
            Mesh rm;
        }
        #endregion

    }*/
}
