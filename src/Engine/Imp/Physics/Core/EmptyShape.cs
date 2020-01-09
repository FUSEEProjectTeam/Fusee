using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Physics.Core
{
    /// <summary>
    /// Implements an empty shape as collision shape.
    /// </summary>
    public class EmptyShape : CollisionShape
    {
        internal IEmptyShapeImp _emtyShapeImp;
        //Inherited
        /// <summary>
        /// Gets and sets the margin for the empty shape.
        /// </summary>
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
