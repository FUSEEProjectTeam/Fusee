using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
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
