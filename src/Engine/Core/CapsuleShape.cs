using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class CapsuleShape : CollisionShape
    {
        internal ICapsuleShapeImp CapsuleShapeImp;

        public float Margin
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

        public float3 LocalScaling
        {
            get
            {
                var retval = CapsuleShapeImp.LocalScaling;
                return retval;
            }
            set
            {
                var o = (CapsuleShape)CapsuleShapeImp.UserObject;
                o.CapsuleShapeImp.LocalScaling = value;
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
