using BulletSharp;
using Fusee.Engine.Imp.Physics.Common;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IEmptyShapeImp" /> interface using the bullet physics engine.
    /// </summary>
    public class EmptyShapeImp : CollisonShapeImp, IEmptyShapeImp
    {
        internal EmptyShape BtEmptyShape;
        //Inherited
        /// <summary>
        /// Gets and sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public override float Margin
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

    }
}
