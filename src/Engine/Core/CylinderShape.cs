using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class CylinderShape : CollisionShape
    {
        internal ICylinderShapeImp _cylinderShapeImp;

        public override float Margin
        {
            get
            {
                var retval = _cylinderShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (CylinderShape)_cylinderShapeImp.UserObject;
                o._cylinderShapeImp.Margin = value;
            }
        }

        public override float3 LocalScaling
        {
            get
            {
                var retval = _cylinderShapeImp.LocalScaling;
                return retval;
            }
            set
            {
                var o = (CylinderShape)_cylinderShapeImp.UserObject;
                o._cylinderShapeImp.LocalScaling = value;
            }
        }

        public float3 HalfExtents
        {
            get
            {
                var retval = _cylinderShapeImp.HalfExtents;
                return retval;
            }
        }

        public float Radius
        {
            get
            {
                var retval = _cylinderShapeImp.Radius;
                return retval;
            }
        }

        public float UpAxis
        {
            get
            {
                var retval = _cylinderShapeImp.UpAxis;
                return retval;
            }
        }
    }
}
