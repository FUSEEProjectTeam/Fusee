using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using Fusee.Math;

namespace Fusee.Engine
{
    public class MultiSphereShapeImp : CollisonShapeImp, IMultiSphereShapeImp
    {
        internal MultiSphereShape BtMultiSphereShape;

        public float Margin
        {
            get
            {
                var retval = BtMultiSphereShape.Margin;
                return retval;
            }
            set
            {
                var o = (MultiSphereShapeImp)BtMultiSphereShape.UserObject;
                o.BtMultiSphereShape.Margin = value;
            }
        }

        private object _userObject;
        public virtual object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }


        public float3 GetSpherePosition(int index)
        {
            var retval = new float3(BtMultiSphereShape.GetSpherePosition(index).X, BtMultiSphereShape.GetSpherePosition(index).Y, BtMultiSphereShape.GetSpherePosition(index).Z);
            return retval;
        }

        public float GetSphereRadius(int index)
        {
            var retval = BtMultiSphereShape.GetSphereRadius(index);
            return retval;
        }

        public int SphereCount
        {
            get
            {
                var retval = BtMultiSphereShape.SphereCount;
                return retval;
            }
        }
    }
}
