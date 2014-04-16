using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class CylinderShape : CollisionShape
    {
        internal ICylinderShapeImp CylinderShapeImp;

        public float Margin
        {
            get
            {
                var retval = CylinderShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (CylinderShape)CylinderShapeImp.UserObject;
                o.CylinderShapeImp.Margin = value;
            }
        }

        public float3 LocalScaling
        {
            get
            {
                var retval = CylinderShapeImp.LocalScaling;
                return retval;
            }
            set
            {
                var o = (CylinderShape)CylinderShapeImp.UserObject;
                o.CylinderShapeImp.LocalScaling = value;
            }
        }

        public float3 HalfExtents
        {
            get
            {
                var retval = CylinderShapeImp.HalfExtents;
                return retval;
            }
        }

        public float Radius
        {
            get
            {
                var retval = CylinderShapeImp.Radius;
                return retval;
            }
        }

        public float UpAxis
        {
            get
            {
                var retval = CylinderShapeImp.UpAxis;
                return retval;
            }
        }
    }
}
