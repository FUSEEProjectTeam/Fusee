using BulletSharp;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IBoxShapeImp" /> interface using the bullet physics engine
    /// </summary>
    public class BoxShapeImp : CollisonShapeImp, IBoxShapeImp
    {
        internal BoxShape BtBoxShape;

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
                var retval = Translator.BtVector3ToFloat3(BtBoxShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (BoxShapeImp)BtBoxShape.UserObject;
                o.BtBoxShape.LocalScaling = Translator.Float3ToBtVector3(value);
                //ToDo: Update RigidBody Inertia referring to the CollisionPbject
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
                var retval = new float3(BtBoxShape.HalfExtentsWithMargin.X, BtBoxShape.HalfExtentsWithMargin.Y,
                    BtBoxShape.HalfExtentsWithMargin.Z);
                return retval;
            }
        }


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
                var retval = BtBoxShape.Margin;
                return retval;
            }
            set
            {
                var o = (BoxShapeImp)BtBoxShape.UserObject;
                o.BtBoxShape.Margin = value;
            }
        }
    }
}