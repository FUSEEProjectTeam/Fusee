using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class EmptyShape : CollisionShape
    {
        internal IEmptyShapeImp EmtyShapeImp;
        //Inherited
        public virtual float Margin
        {

            get
            {
                var retval = EmtyShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (BoxShape)EmtyShapeImp.UserObject;
                o.BoxShapeImp.Margin = value;
            }
        }
    }
}
