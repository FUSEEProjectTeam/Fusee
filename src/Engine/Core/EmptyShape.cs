using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    public class EmptyShape : CollisionShape
    {
        internal IEmptyShapeImp _emtyShapeImp;
        //Inherited
        public override float Margin
        {

            get
            {
                var retval = _emtyShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (BoxShape)_emtyShapeImp.UserObject;
                o._boxShapeImp.Margin = value;
            }
        }
    }
}
