using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Engine;
using Fusee;
using BulletSharp;
using Fusee.Math;

namespace Fusee.Engine
{
    public class HingeConstraintImp : IHingeConstraintImp
    {
        internal Translater Translater = new Translater();
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

        public float4x4 FrameA
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_hci.AFrame);
                return retval;
            }
        }

        public float4x4 FrameB
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_hci.BFrame);
                return retval;
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
