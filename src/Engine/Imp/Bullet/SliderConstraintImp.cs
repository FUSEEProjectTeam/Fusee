using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;

namespace Fusee.Engine
{
    public class SliderConstraintImp : ISliderConstraintImp
    {
        internal SliderConstraint _sci;

        public IRigidBodyImp RigidBodyA
        {
            get
            {
                var retval = _sci.RigidBodyA;
                return (RigidBodyImp)retval.UserObject;
            }
        }

        public IRigidBodyImp RigidBodyB
        {
            get
            {
                var retval = _sci.RigidBodyB;
                return (RigidBodyImp)retval.UserObject;
            }
        }

        
        public int GetUid()
        {
            var retval = _sci.Uid;
            return retval;
        }

        private object _userObject;
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
    }
}
