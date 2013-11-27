using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class SliderConstraint
    {
        internal ISliderConstraintImp _iSliderConstraintImp;

        public RigidBody RigidBodyA
        {
            get
            {
                var retval = _iSliderConstraintImp.RigidBodyA.UserObject;
                return (RigidBody)retval;
            }
        }

        public RigidBody RigidBodyB
        {
            get
            {
                var retval = _iSliderConstraintImp.RigidBodyB.UserObject;
                return (RigidBody)retval;
            }
        }
    }
}
