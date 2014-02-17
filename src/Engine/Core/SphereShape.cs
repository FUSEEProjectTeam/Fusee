using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class SphereShape : CollisionShape
    {
        internal ISphereShapeImp SphereShapeImp;

        public virtual float Margin
        {
            get
            {
                var retval = SphereShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (SphereShape)SphereShapeImp.UserObject;
                o.SphereShapeImp.Margin = value;
            }
        }

        public float Radius
        {
            get
            {
                var retval = SphereShapeImp.Radius;
                return retval;
            }
            set
            {
                var o = (SphereShape) SphereShapeImp.UserObject;
                o.SphereShapeImp.Radius = value;
            }
        }
        
    }
}
