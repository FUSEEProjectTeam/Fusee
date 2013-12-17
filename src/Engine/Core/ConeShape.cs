using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class ConeShape : CollisionShape
    {
        internal IConeShapeImp ConeShapeImp;

        public float Margin
        {
            get
            {
                var retval = ConeShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (CapsuleShape)ConeShapeImp.UserObject;
                o.CapsuleShapeImp.Margin = value;
            }
        }

        public int ConeUpIndex
        {
            get
            {
                var retval = ConeShapeImp.ConeUpIndex;
                return retval;
            }
            set
            {
                var o = (ConeShape) ConeShapeImp.UserObject;
                o.ConeShapeImp.ConeUpIndex = value;
            }
        }

        public float Height
        {
            get
            {
                var retval = ConeShapeImp.Height;
                return retval;
            }
        }

        public float Radius
        {
            get
            {
                var retval = ConeShapeImp.Radius;
                return retval;
            }
        }
    }
}
