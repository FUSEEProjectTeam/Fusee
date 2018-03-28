using System.Collections.Generic;
using System.Diagnostics;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use ConVSceneToHighLevel to traverse a scene hierarchy (made out of scene nodes and components) in order
    /// to have each visited element converted and split to its high level, render-ready components.
    /// </summary>
    public class ConVSceneToHighLevel : SceneVisitor
    {

        private SceneContainer _sc;
        private SceneContainer _highSc;

        public ConVSceneToHighLevel(SceneContainer sc)
        {
            _sc = sc;
            _highSc = new SceneContainer {Children = new List<SceneNodeContainer>()};
        }

        private int _hierarchyLevel;

        protected override void PushState()
        {
            _hierarchyLevel++;
        }
        protected override void PopState()
        {
            _hierarchyLevel--;
        }

        
        public SceneContainer Convert()
        {
            _highSc = new SceneContainer();
            Traverse(_sc.Children);
            return _sc;

        }

        #region Visitors

        [VisitMethod]
        public void ConvTransform(TransformComponent transform)
        {
            transform.Name = _hierarchyLevel.ToString();
            Debug.WriteLine(_hierarchyLevel);
        }

        [VisitMethod]
        public void ConvMaterial(MaterialComponent matComp)
        {
            matComp.Name = _hierarchyLevel.ToString();
            Debug.WriteLine(_hierarchyLevel);
        }

        [VisitMethod]
        public void ConvMesh(Mesh mesh)
        {
            mesh.Name = _hierarchyLevel.ToString();
            Debug.WriteLine(_hierarchyLevel);
        }

        [VisitMethod]
        public void ConvLight(LightComponent lightComponent)
        {
            lightComponent.Name = _hierarchyLevel.ToString();
            Debug.WriteLine(_hierarchyLevel);

        }
        #endregion


    }



}
