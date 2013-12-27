using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
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
}
