using System.Collections.Generic;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use ConVSceneToHighLevel to create new high level graph from a low level graph (made out of scene nodes and components) in order
    /// to have each visited element converted and/or split into its high level, render-ready components.
    /// </summary>
    public class ConvertSceneGraph : SceneVisitor
    {
        private SceneContainer _convertedScene;
        private Stack<SceneNodeContainer> _predecessors;
        private SceneNodeContainer _currentNode;

        protected override void PopState()
        {
            _predecessors.Pop();
        }

        /// <summary>
        /// Traverses the given SceneContainer and creates new high level graph by converting and/or spliting its components into the high level equivalents.
        /// </summary>
        /// <param name="sc">The SceneContainer to convert.</param>
        /// <returns></returns>
        public SceneContainer Convert(SceneContainer sc)
        {
            _predecessors = new Stack<SceneNodeContainer>();
            _convertedScene = new SceneContainer();

            Traverse(sc.Children);
            return _convertedScene;
        }

        #region Visitors

        [VisitMethod]
        public void ConvScneNodeContainer(SceneNodeContainer snc)
        {
            if (_predecessors.Count != 0)
            {
                var parent = _predecessors.Peek();

                if (parent.Children == null)
                    parent.Children = new List<SceneNodeContainer>();

                _currentNode = new SceneNodeContainer { Name = snc.Name + "_copy" };
                parent.Children.Add(_currentNode);
                _predecessors.Push(_currentNode);
            }
            else //Add first node to SceneContainer
            {
                _predecessors.Push(new SceneNodeContainer { Name = CurrentNode.Name + "_copy" });
                _currentNode = _predecessors.Peek();
                _convertedScene.Children = new List<SceneNodeContainer> { _currentNode };
            }
        }

        [VisitMethod]
        public void ConvTransform(TransformComponent transform)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponentContainer>();

            _currentNode.Components.Add(transform);
        }

        [VisitMethod]
        public void ConvMaterial(MaterialComponent matComp)
        {

        }

        [VisitMethod]
        public void ConvMaterial(MaterialLightComponent matComp)
        {

        }

        [VisitMethod]
        public void ConvMaterial(MaterialPBRComponent matComp)
        {

        }

        [VisitMethod]
        public void ConvShader(ShaderComponent shaderComponent)
        {

        }


        [VisitMethod]
        public void ConvShaderEffect(ShaderEffectComponent shaderComponent)
        {

        }

        [VisitMethod]
        public void ConvMesh(Mesh mesh)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponentContainer>();

            _currentNode.Components.Add(mesh);
        }

        [VisitMethod]
        public void ConvLight(LightComponent lightComponent)
        {

        }

        [VisitMethod]
        public void ConvBone(BoneComponent bone)
        {

        }

        [VisitMethod]
        public void ConVWeight(WeightComponent weight)
        {

        }

        [VisitMethod]
        public void ConCanvasTransform(CanvasTransformComponent ctc)
        {

        }

        [VisitMethod]
        public void ConvRectTransform(RectTransformComponent rtc)
        {

        }
        #endregion
    }
}
