using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    public class GImpactMeshShape : CollisionShape
    {

        internal IGImpactMeshShapeImp GImpactMeshShapeImp;

        public virtual float Margin
        {
            get
            {
                var retval = GImpactMeshShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (GImpactMeshShape)GImpactMeshShapeImp.UserObject;
                o.GImpactMeshShapeImp.Margin = value;
            }
        }
    }
}
