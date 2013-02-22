using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.SceneManagement
{
    class SceneVisitorSearch : SceneVisitor
    {
        private string _name;
        private SceneEntity _searchresult;

        public override void Visit(SceneEntity sceneEntity)
        {

            if (sceneEntity.name == _name)
            {
                _searchresult = sceneEntity;
            }

        }

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
