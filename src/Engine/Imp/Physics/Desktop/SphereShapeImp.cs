using BulletSharp;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="ISphereShapeImp" /> interface using the bullet physics engine.
    /// </summary>
    public class SphereShapeImp : CollisonShapeImp, ISphereShapeImp
    {
        internal SphereShape BtSphereShape;

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
                var retval = BtSphereShape.Margin;
                return retval;
            }
            set
            {
                var o = (SphereShapeImp)BtSphereShape.UserObject;
                o.BtSphereShape.Margin = value;
            }
        }

        /// <summary>
        /// Gets and sets the local scaling.
        /// </summary>
        /// <value>
        /// The local scaling.
        /// </value>
        public override float3 LocalScaling
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(BtSphereShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (SphereShapeImp)BtSphereShape.UserObject;
                o.BtSphereShape.LocalScaling = Translator.Float3ToBtVector3(value);
            }
        }

        /// <summary>
        /// Gets and sets the radius.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        public float Radius
        {
            get
            {
                var retval = BtSphereShape.Radius;
                return retval;
            }
            set
            {
                var o = (SphereShapeImp) BtSphereShape.UserObject;
                BtSphereShape.SetUnscaledRadius(value);
            }
        }


        
    }
}
