using Fusee.Engine.Imp.Physics.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Core
{
    /// <summary>
    ///  Defines a cone-shaped collision object
    /// </summary>
    public class ConeShape : CollisionShape
    {
        internal IConeShapeImp _coneShapeImp;

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
                var retval = _coneShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (ConeShape)_coneShapeImp.UserObject;
                o._coneShapeImp.Margin = value;
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
                var retval = _coneShapeImp.LocalScaling;
                return retval;
            }
            set
            {
                var o = (ConeShape) _coneShapeImp.UserObject;
                o._coneShapeImp.LocalScaling = value;
            }
        }

        /// <summary>
        /// Gets and sets the index of the cone's up axis
        /// </summary>
        /// <value>
        /// The index of cone's up axis.
        /// </value>
        public int ConeUpIndex
        {
            get
            {
                var retval = _coneShapeImp.ConeUpIndex;
                return retval;
            }
            set
            {
                var o = (ConeShape) _coneShapeImp.UserObject;
                o._coneShapeImp.ConeUpIndex = value;
            }
        }

        /// <summary>
        /// Gets the cone's height along the up axis.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public float Height
        {
            get
            {
                var retval = _coneShapeImp.Height;
                return retval;
            }
        }

        /// <summary>
        /// Gets the cone's radius.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        public float Radius
        {
            get
            {
                var retval = _coneShapeImp.Radius;
                return retval;
            }
        }
    }
}
