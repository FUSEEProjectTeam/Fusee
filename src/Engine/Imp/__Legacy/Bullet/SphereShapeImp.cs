using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using Fusee.Math;

namespace Fusee.Engine
{
    public class SphereShapeImp : CollisonShapeImp, ISphereShapeImp
    {
        internal SphereShape BtSphereShape;

        public float Margin
        {
            get
            {
                var retval = BtSphereShape.Margin;
                return retval;
            }
            set
            {
                var o = (SphereShapeImp)BtSphereShape.UserObject;
                o.BtSphereShape.Margin = value;
            }
        }

        public float3 LocalScaling
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(BtSphereShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (SphereShapeImp)BtSphereShape.UserObject;
                o.BtSphereShape.LocalScaling = Translater.Float3ToBtVector3(value);
            }
        }

        private object _userObject;
        public virtual object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }

        public float Radius
        {
            get
            {
                var retval = BtSphereShape.Radius;
                return retval;
            }
            set
            {
                var o = (SphereShapeImp) BtSphereShape.UserObject;
                BtSphereShape.SetUnscaledRadius(value);
            }
        }


        
    }
}
