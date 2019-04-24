using BulletSharp;
using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IGearConstraintImp" /> interface using the bullet physics engine.
    /// </summary>
    public class GearConstraintImp : IGearConstraintImp
    {
        internal GearConstraint _gci;

        /// <summary>
        /// Gets the rigid body a.
        /// </summary>
        /// <value>
        /// The rigid body a.
        /// </value>
        public IRigidBodyImp RigidBodyA
        {
            get
            {
                var retval = _gci.RigidBodyA;
                return (RigidBodyImp)retval.UserObject;
            }
        }

        /// <summary>
        /// Gets the rigid body b.
        /// </summary>
        /// <value>
        /// The rigid body b.
        /// </value>
        public IRigidBodyImp RigidBodyB
        {
            get
            {
                var retval = _gci.RigidBodyB;
                return (RigidBodyImp)retval.UserObject;
            }
        }

        /// <summary>
        /// Gets the uid.
        /// </summary>
        /// <returns></returns>
        public int GetUid()
        {
            var retval = _gci.Uid;
            return retval;
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
