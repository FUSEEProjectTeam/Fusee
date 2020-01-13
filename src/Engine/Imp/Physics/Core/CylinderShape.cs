using Fusee.Engine.Imp.Physics.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Core
{
    /// <summary>
    /// Implements a cylindric collision shape
    /// </summary>
    public class CylinderShape : CollisionShape
    {
        internal ICylinderShapeImp _cylinderShapeImp;

        public override float Margin
        {
            get
            {
                var retval = _cylinderShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (CylinderShape)_cylinderShapeImp.UserObject;
                o._cylinderShapeImp.Margin = value;
            }
        }
        /// <summary>
        /// Gets and sets the local scaling of the cylinder shape.
        /// </summary>
        public override float3 LocalScaling
        {
            get
            {
                var retval = _cylinderShapeImp.LocalScaling;
                return retval;
            }
            set
            {
                var o = (CylinderShape)_cylinderShapeImp.UserObject;
                o._cylinderShapeImp.LocalScaling = value;
            }
        }

        public float3 HalfExtents
        {
            get
            {
                var retval = _cylinderShapeImp.HalfExtents;
                return retval;
            }
        }
        /// <summary>
        /// Returns the radius of the cylinder shape.
        /// </summary>
        public float Radius
        {
            get
            {
                var retval = _cylinderShapeImp.Radius;
                return retval;
            }
        }
        /// <summary>
        /// Returns the up axis of the cylinder shape.
        /// </summary>
        public float UpAxis
        {
            get
            {
                var retval = _cylinderShapeImp.UpAxis;
                return retval;
            }
        }
    }
}
