using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class MultiSphereShape : CollisionShape
    {
        internal IMultiSphereShapeImp _multiSphereShapeImp;

        public override float Margin
        {
            get
            {
                var retval = _multiSphereShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (CapsuleShape)_multiSphereShapeImp.UserObject;
                o._capsuleShapeImp.Margin = value;
            }
        }

        public float3 GetSpherePosition(int index)
        {
            var retval = _multiSphereShapeImp.GetSpherePosition(index);
            return retval;
        }

        public float GetSphereRadius(int index)
        {
            var retval = _multiSphereShapeImp.GetSphereRadius(index);
            return retval;
        }

        public int SphereCount
        {
            get
            {
                var retval = _multiSphereShapeImp.SphereCount;
                return retval;
            }
        }
    }
}
