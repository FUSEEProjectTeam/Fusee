using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
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
