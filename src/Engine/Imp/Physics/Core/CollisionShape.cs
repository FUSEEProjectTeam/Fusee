using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Core
{
    /// <summary>
    /// Base class for various collision shape types.
    /// </summary>
    public class CollisionShape
    {
        /// <summary>
        /// The implementation object.
        /// </summary>
        internal ICollisionShapeImp _collisionShapeImp;


        /// <summary>
        /// Retrieves or sets the margin.
        /// </summary>
        /// <value>
        /// The size of the collision shape's margin.
        /// </value>
        public virtual float Margin
        {
            get { return _collisionShapeImp.Margin; }
            set
            {
                var o = (CollisionShape)_collisionShapeImp.UserObject;
                o._collisionShapeImp.Margin = value; 
            }
        }

        /// <summary>
        /// Retrieves or sets the local scaling.
        /// </summary>
        /// <value>
        /// The local scaling.
        /// </value>
        public virtual float3 LocalScaling
        {
            get { return _collisionShapeImp.LocalScaling; }
            set
            {
                var o = (CollisionShape)_collisionShapeImp.UserObject;
                o._collisionShapeImp.LocalScaling = value;
            }
        }
    }
}
