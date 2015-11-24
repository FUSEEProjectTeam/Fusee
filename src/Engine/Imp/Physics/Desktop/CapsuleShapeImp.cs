using BulletSharp;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="ICapsuleShapeImp" /> interface using the bullet physics engine
    /// </summary>
    public class CapsuleShapeImp : CollisonShapeImp, ICapsuleShapeImp
    {
        internal CapsuleShape BtCapsuleShape;
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
                var retval = BtCapsuleShape.Margin;
                return retval;
            }
            set
            {
                var o = (CapsuleShapeImp)BtCapsuleShape.UserObject;
                o.BtCapsuleShape.Margin = value;
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
                var retval = Translator.BtVector3ToFloat3(BtCapsuleShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (CapsuleShapeImp)BtCapsuleShape.UserObject;
                o.BtCapsuleShape.LocalScaling = Translator.Float3ToBtVector3(value);
            }
        }

        /// <summary>
        /// Gets the height of the half.
        /// </summary>
        /// <value>
        /// The height of the half.
        /// </value>
        public float HalfHeight
        {
            get
            {
                var retval = BtCapsuleShape.HalfHeight;
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
                var retval = BtCapsuleShape.Radius;
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
                var retval = BtCapsuleShape.UpAxis;
                return retval;
            }
        }
    }
}
