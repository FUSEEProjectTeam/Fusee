using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use ConVSceneToHighLevel to traverse a scene hierarchy (made out of scene nodes and components) in order
    /// to have each visited element converted and/or split into its high level, render-ready components.
    /// </summary>
    public class ConvertSceneGraph : SceneVisitor
    {

        /*private int _hierarchyLevel;

        protected override void PushState()
        {
            _hierarchyLevel++;
        }
        protected override void PopState()
        {
            _hierarchyLevel--;
        }*/

        /// <summary>
        /// Traverses the given SceneContainer and convertsa and/or splits its components into the high level equivalents.
        /// </summary>
        /// <param name="sc">The SceneContainer to convert.</param>
        /// <returns></returns>
        public SceneContainer Convert(SceneContainer sc)
        {
            Traverse(sc.Children);
            return sc;
        }

        protected override void DoVisitComponents(SceneNodeContainer node)
        {
            // Are there any components at all?
            if (node.Components == null)
                return;
            // Visit each component --> for-loop is needed because the collecton can be changed while iterating!
            for (var i = 0; i < node.Components.Count; i++)
            {
                var component = node.Components[i];
                CurrentComponent = component;
                DoVisitComponent(component);
                CurrentComponent = null;
            }
        }

        #region Visitors

        [VisitMethod]
        public void ConvTransform(TransformComponent transform)
        {
            //CurrentNode.Components.Remove(transform);
        }

        [VisitMethod]
        public void ConvMaterial(MaterialComponent matComp)
        {
            CurrentNode.Components.Add(new TextureComponent());
        }

        [VisitMethod]
        public void ConvMesh(Mesh mesh)
        {
            
        }

        [VisitMethod]
        public void ConvLight(LightComponent lightComponent)
        {
            
        }
        #endregion


    }
}
