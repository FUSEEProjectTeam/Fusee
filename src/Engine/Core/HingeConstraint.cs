using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

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

        public float4x4 AFrame
        {
            get
            {
                var retval = _iHConstraintImp.FrameA;
                return retval;
            }
        }

        public float4x4 BFrame
        {
            get
            {
                var retval = _iHConstraintImp.FrameB;
                return retval;
            }
        }
    }
}
