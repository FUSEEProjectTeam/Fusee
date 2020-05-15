using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// A box shaped collision object.
    /// </summary>
    public class BoxShape : CollisionShape
    {
        /// <summary>
        /// The implementation object.
        /// </summary>
        internal IBoxShapeImp _boxShapeImp;

        /// <summary>
        /// Retrieves or sets the local scaling.
        /// </summary>
        /// <value>
        /// The local scaling.
        /// </value>
        public override float3 LocalScaling
        {
            get { return _boxShapeImp.LocalScaling; }
            set
            {
                var o = (BoxShape)_boxShapeImp.UserObject;
                o._boxShapeImp.LocalScaling = value;
            }
        }
        /// <summary>
        /// Gets a vector containing half of the values of width, height and depth of the box.
        /// </summary>
        /// <value>
        /// The half extents.
        /// </value>
        public float3 HalfExtents
        {
            get
            {
                var retval = _boxShapeImp.HalfExtents;
                return retval;
            }
        }

        /// <summary>
        /// Retrieves or sets the margin.
        /// </summary>
        /// <value>
        /// The size of the collision shape's margin.
        /// </value>
        public override float Margin
        {
            get
            {
                var retval = _boxShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (BoxShape) _boxShapeImp.UserObject;
                o._boxShapeImp.Margin = value;
            }
        }
    }
}
