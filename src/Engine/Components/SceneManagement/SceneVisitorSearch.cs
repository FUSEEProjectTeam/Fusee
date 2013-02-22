using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// Derivate of SceneVisitor, used to search for SceneEntities or Components in the scene.
    /// </summary>
    class SceneVisitorSearch : SceneVisitor
    {
        /// <summary>
        /// The _name that will be searched for.
        /// </summary>
        private string _name;
        /// <summary>
        /// The _searchresult: If a result is found returns this as reference, else null
        /// </summary>
        private SceneEntity _searchresult;

        /// <summary>
        /// Visits the specified scene entity to check its name and set _searchresult as reference if the name matches.
        /// </summary>
        /// <param name="sceneEntity">The scene entity.</param>
        public override void Visit(SceneEntity sceneEntity)
        {

            if (sceneEntity.name == _name)
            {
                _searchresult = sceneEntity;
            }

        }

        /// <summary>
        /// Finds a scene entity by name.
        /// </summary>
        /// <param name="sceneEntities">The list of scene entities from SceneManager that will be visited.</param>
        /// <param name="name">The searched name of a SceneEntity.</param>
        /// <returns></returns>
        public SceneEntity FindSceneEntity(List<SceneEntity> sceneEntities, string name)
        {
            _name = name;
            foreach (var sceneMember in sceneEntities)
            {
                sceneMember.Accept(this);
                foreach (var child in sceneMember.GetChildren())
                {
                    child.Accept(this);
                }
            }
            return _searchresult;
        }
    }
}
