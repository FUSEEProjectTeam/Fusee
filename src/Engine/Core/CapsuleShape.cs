using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class CapsuleShape : CollisionShape
    {
        internal ICapsuleShapeImp _capsuleShapeImp;

        public override float Margin
        {
            get
            {
                var retval = _capsuleShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (CapsuleShape)_capsuleShapeImp.UserObject;
                o._capsuleShapeImp.Margin = value;
            }
        }

        public override float3 LocalScaling
        {
            get
            {
                var retval = _capsuleShapeImp.LocalScaling;
                return retval;
            }
            set
            {
                var o = (CapsuleShape)_capsuleShapeImp.UserObject;
                o._capsuleShapeImp.LocalScaling = value;
            }
        }

        public float HalfHeight
        {
            get
            {
                var retval = _capsuleShapeImp.HalfHeight;
                return retval;
            }
        }

        public float Radius
        {
            get
            {
                var retval = _capsuleShapeImp.Radius;
                return retval;
            }
        }

        public float UpAxis
        {
            get
            {
                var retval = _capsuleShapeImp.UpAxis;
                return retval;
            }
        }
    }
}
