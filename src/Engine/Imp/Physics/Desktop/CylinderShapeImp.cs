using BulletSharp;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="ICylinderShapeImp" /> interface using the bullet physics engine.
    /// </summary>
    public class CylinderShapeImp : CollisonShapeImp, ICylinderShapeImp
    {
        internal CylinderShape BtCylinderShape;
        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public override float Margin
        {
            get
            {
                var retval = BtCylinderShape.Margin;
                return retval;
            }
            set
            {
                var o = (CylinderShapeImp)BtCylinderShape.UserObject;
                o.BtCylinderShape.Margin = value;
            }
        }

        /// <summary>
        /// Gets or sets the local scaling.
        /// </summary>
        /// <value>
        /// The local scaling.
        /// </value>
        public override float3 LocalScaling
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(BtCylinderShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (CylinderShapeImp)BtCylinderShape.UserObject;
                o.BtCylinderShape.LocalScaling = Translator.Float3ToBtVector3(value);
            }
        }


        /// <summary>
        /// Gets the half extents.
        /// </summary>
        /// <value>
        /// The half extents.
        /// </value>
        public float3 HalfExtents
        {
            get
            {
                var retval = new float3(BtCylinderShape.HalfExtentsWithMargin.X, BtCylinderShape.HalfExtentsWithMargin.Y, BtCylinderShape.HalfExtentsWithMargin.Z);
                return retval;
            }
        }

        /// <summary>
        /// Gets the radius.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        public float Radius
        {
            get
            {
                var retval = BtCylinderShape.Radius;
                return retval;
            }
        }

        /// <summary>
        /// Gets up axis.
        /// </summary>
        /// <value>
        /// Up axis.
        /// </value>
        public int UpAxis
        {
            get
            {
                var retval = BtCylinderShape.UpAxis;
                return retval;
            }
        }
    }
}
