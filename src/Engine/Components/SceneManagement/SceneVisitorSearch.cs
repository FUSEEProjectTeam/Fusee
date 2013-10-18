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
        #region Fields
        private string _name;
        private SceneEntity _searchresult;
        #endregion

        #region Overrides
        /// <summary>
        /// Visits the specified <see cref="SceneEntity"/> to check it's name and set _searchresult as reference if the name matches.
        /// </summary>
        /// <param name="sceneEntity">The scene entity.</param>
        public override void Visit(SceneEntity sceneEntity)
        {

            if (sceneEntity.name == _name)
            {
                _searchresult = sceneEntity;
            }

        }
        #endregion

        #region Members

        /// <summary>
        /// Finds a <see cref="SceneEntity"/> by it's name.
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
        #endregion

    }
}
