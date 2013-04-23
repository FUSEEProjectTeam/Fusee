using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;

namespace Fusee.SceneManagement
{
    class SceneVisitorStartAction : SceneVisitor
    {
        public override void Visit(SceneEntity cEntity)
        {
            cEntity.TraverseForRendering(this);
        }

        public override void Visit(ActionCode actionCode)
        {
            actionCode.Start();
        }
    }
}
