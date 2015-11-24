using BulletSharp;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IConeShapeImp" /> interface using the bullet physics engine.
    /// </summary>
    public class ConeShapeImp : CollisonShapeImp, IConeShapeImp
    {
        internal ConeShape BtConeShape;
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
                var retval = BtConeShape.Margin;
                return retval;
            }
            set
            {
                var o = (ConeShapeImp)BtConeShape.UserObject;
                o.BtConeShape.Margin = value;
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
                var retval = Translator.BtVector3ToFloat3(BtConeShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (ConeShapeImp)BtConeShape.UserObject;
                o.BtConeShape.LocalScaling = Translator.Float3ToBtVector3(value);
            }
        }

        /// <summary>
        /// Gets or sets the index of the cone up.
        /// </summary>
        /// <value>
        /// The index of the cone up.
        /// </value>
        public int ConeUpIndex
        {
            get
            {
                var retval = BtConeShape.ConeUpIndex;
                return retval;
            }
            set
            {
                var o = (ConeShapeImp) BtConeShape.UserObject;
                o.BtConeShape.ConeUpIndex = value;
            }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public float Height
        {
            get
            {
                var retval = BtConeShape.Height;
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
                var retval = BtConeShape.Radius;
                return retval;
            }
        }
    }
}
