using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;

namespace Fusee.Engine
{
    public class CapsuleShapeImp : CollisonShapeImp, ICapsuleShapeImp
    {
        internal CapsuleShape BtCapsuleShape;
        public virtual float Margin
        {
            get
            {
                var retval = BtCapsuleShape.Margin;
                return retval;
            }
            set
            {
                var o = (CapsuleShapeImp)BtCapsuleShape.UserObject;
                o.BtCapsuleShape.Margin = value;
            }
        }

        private object _userObject;
        public virtual object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }


        public float HalfHeight
        {
            get
            {
                var retval = BtCapsuleShape.HalfHeight;
                return retval;
            }
        }

        public float Radius
        {
            get
            {
                var retval = BtCapsuleShape.Radius;
                return retval;
            }
        }

        public int UpAxis
        {
            get
            {
                var retval = BtCapsuleShape.UpAxis;
                return retval;
            }
        }
    }
}
