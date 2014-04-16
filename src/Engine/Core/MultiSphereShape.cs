using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class MultiSphereShape : CollisionShape
    {
        internal IMultiSphereShapeImp MultiSphereShapeImp;

        public float Margin
        {
            get
            {
                var retval = MultiSphereShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (CapsuleShape)MultiSphereShapeImp.UserObject;
                o.CapsuleShapeImp.Margin = value;
            }
        }

        public float3 GetSpherePosition(int index)
        {
            var retval = MultiSphereShapeImp.GetSpherePosition(index);
            return retval;
        }

        public float GetSphereRadius(int index)
        {
            var retval = MultiSphereShapeImp.GetSphereRadius(index);
            return retval;
        }

        public int SphereCount
        {
            get
            {
                var retval = MultiSphereShapeImp.SphereCount;
                return retval;
            }
        }
    }
}
