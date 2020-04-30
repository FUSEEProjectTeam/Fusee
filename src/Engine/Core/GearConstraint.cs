using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
#pragma warning disable CS1591 // Missing XML-comment
    public class GearConstraint
    {
        internal IGearConstraintImp _iGearConstraintImp;

        public RigidBody RigidBodyA
        {
            get
            {
                var retval = _iGearConstraintImp.RigidBodyA.UserObject;
                return (RigidBody)retval;
            }
        }

        public RigidBody RigidBodyB
        {
            get
            {
                var retval = _iGearConstraintImp.RigidBodyB.UserObject;
                return (RigidBody)retval;
            }
        }

        public int GetUid()
        {
            var retval = _iGearConstraintImp.GetUid();
            return retval;
        }
    }
#pragma warning restore CS1591 // Missing XML-comment

}
