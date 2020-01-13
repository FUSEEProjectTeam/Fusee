using Fusee.Engine.Imp.Physics.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Core
{
    /// <summary>
    /// A capsule shaped collision object.
    /// </summary>
    public class CapsuleShape : CollisionShape
    {
        internal ICapsuleShapeImp _capsuleShapeImp;

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
                var retval = _capsuleShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (CapsuleShape)_capsuleShapeImp.UserObject;
                o._capsuleShapeImp.Margin = value;
            }
        }

        /// <summary>
        /// Retrieves or sets the local scaling.
        /// </summary>
        /// <value>
        /// The local scaling.
        /// </value>
        public override float3 LocalScaling
        {
            get
            {
                var retval = _capsuleShapeImp.LocalScaling;
                return retval;
            }
            set
            {
                var o = (CapsuleShape)_capsuleShapeImp.UserObject;
                o._capsuleShapeImp.LocalScaling = value;
            }
        }

        /// <summary>
        /// Gets the half of this capsule's height.
        /// </summary>
        /// <value>
        /// Half of the height.
        /// </value>
        public float HalfHeight
        {
            get
            {
                var retval = _capsuleShapeImp.HalfHeight;
                return retval;
            }
        }

        /// <summary>
        /// Gets this capsules radius.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        public float Radius
        {
            get
            {
                var retval = _capsuleShapeImp.Radius;
                return retval;
            }
        }

        /// <summary>
        /// Gets the up axis of the capsule.
        /// </summary>
        /// <value>
        /// The up axis.
        /// </value>
        public float UpAxis
        {
            get
            {
                var retval = _capsuleShapeImp.UpAxis;
                return retval;
            }
        }
    }
}
