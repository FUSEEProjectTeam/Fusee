using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;

namespace Fusee.Engine
{
    public class ConeShapeImp : CollisonShapeImp, IConeShapeImp
    {
        internal ConeShape BtConeShape;
        public float Margin
        {
            get
            {
                var retval = BtConeShape.Margin;
                return retval;
            }
            set
            {
                var o = (ConeShapeImp)BtConeShape.UserObject;
                o.BtConeShape.Margin = value;
            }
        }

        private object _userObject;
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }

        public int ConeUpIndex
        {
            get
            {
                var retval = BtConeShape.ConeUpIndex;
                return retval;
            }
            set
            {
                var o = (ConeShapeImp) BtConeShape.UserObject;
                o.BtConeShape.ConeUpIndex = value;
            }
        }

        public float Height
        {
            get
            {
                var retval = BtConeShape.Height;
                return retval;
            }
        }

        public float Radius
        {
            get
            {
                var retval = BtConeShape.Radius;
                return retval;
            }
        }
    }
}
