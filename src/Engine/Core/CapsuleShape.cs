using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class CapsuleShape : CollisionShape
    {
        internal ICapsuleShapeImp CapsuleShapeImp;

        public virtual float Margin
        {
            get
            {
                var retval = CapsuleShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (CapsuleShape)CapsuleShapeImp.UserObject;
                o.CapsuleShapeImp.Margin = value;
            }
        }

        public float HalfHeight
        {
            get
            {
                var retval = CapsuleShapeImp.HalfHeight;
                return retval;
            }
        }

        public float Radius
        {
            get
            {
                var retval = CapsuleShapeImp.Radius;
                return retval;
            }
        }

        public float UpAxis
        {
            get
            {
                var retval = CapsuleShapeImp.UpAxis;
                return retval;
            }
        }
    }
}
