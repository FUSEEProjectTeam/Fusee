using BulletSharp;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="ICollisionShapeImp" /> interface using the bullet physics engine
    /// </summary>
    public class CollisonShapeImp : ICollisionShapeImp
    {
        internal CollisionShape BtCollisionShape;

        /// <summary>
        /// Gets and sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public virtual float Margin
        {
            get
            {
                var retval = BtCollisionShape.Margin;
                return retval;
            }
            set
            {
                var o = (BoxShapeImp)BtCollisionShape.UserObject;
                o.BtBoxShape.Margin = value;
            }
        }

        /// <summary>
        /// Gets and sets the local scaling.
        /// </summary>
        /// <value>
        /// The local scaling.
        /// </value>
        public virtual float3 LocalScaling
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(BtCollisionShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (BoxShapeImp)BtCollisionShape.UserObject;
                o.BtBoxShape.LocalScaling = Translator.Float3ToBtVector3(value);
                //ToDo: Update RigidBody Inertia referring to the CollisionPbject
            }
        }


        private object _userObject;
        /// <summary>
        /// Gets and sets the user object.
        /// </summary>
        /// <value>
        /// The user object.
        /// </value>
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
    }
}
