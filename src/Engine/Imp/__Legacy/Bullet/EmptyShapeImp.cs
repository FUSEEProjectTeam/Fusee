using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;

namespace Fusee.Engine
{
    public class EmptyShapeImp : CollisonShapeImp, IEmptyShapeImp
    {
        internal EmptyShape BtEmptyShape;
        //Inherited
        public virtual float Margin
        {
            get
            {
                var retval = BtEmptyShape.Margin;
                return retval;
            }
            set
            {
                var o = (EmptyShapeImp)BtEmptyShape.UserObject;
                o.BtEmptyShape.Margin = value;
            }
        }

        private object _userObject;
        public virtual object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }

    }
}
