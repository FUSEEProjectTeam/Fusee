using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusee.Math;
using BulletSharp;

namespace Fusee.Engine
{
    public class HingeConstraintImp : IHingeConstraintImp
    {
        internal HingeConstraint _hci;

        public IRigidBodyImp RigidBodyA
        {
            get
            {
                var retval = _hci.RigidBodyA;
                return (RigidBodyImp)retval.UserObject;
            }
        }

        public IRigidBodyImp RigidBodyB
        {
            get
            {
                var retval = _hci.RigidBodyB;
                return (RigidBodyImp)retval.UserObject;
            }
        }

        private object _userObject;
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
    }
}
