using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class HingeConstraint
    {
        internal IHingeConstraintImp _iHConstraintImp;

        public RigidBody RigidBodyA
        {
            get
            {
                var retval = _iHConstraintImp.RigidBodyA.UserObject;
                return (RigidBody)retval;
            }
        }

        public RigidBody RigidBodyB
        {
            get
            {
                var retval = _iHConstraintImp.RigidBodyB.UserObject;
                return (RigidBody)retval;
            }
        }
    }
}
