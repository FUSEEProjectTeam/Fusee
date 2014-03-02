using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Engine;
using Fusee;
using BulletSharp;
using Fusee.Math;
using Quaternion = Fusee.Math.Quaternion;

namespace Fusee.Engine
{
    public class HingeConstraintImp : IHingeConstraintImp
    {
        internal Translater Translater = new Translater();
        internal HingeConstraint _hci;

        public bool AngularOnly
        {
            get
            {
                var retval = _hci.AngularOnly;
                return retval;
            }
            set
            {
                var o = (HingeConstraintImp)_hci.UserObject;
                o._hci.AngularOnly = value;
            }
        }

        public bool EnableMotor
        {
            get
            {
                var retval = _hci.EnableMotor;
                return retval;
            }
            set
            {
                var o = (HingeConstraintImp) _hci.UserObject;
                o._hci.EnableMotor = value;
            }
        }
        public void EnableAngularMotor(bool enableMotor, float targetVelocity, float maxMotorImpulse)
        {
            var o = (HingeConstraintImp) _hci.UserObject;
            o._hci.EnableAngularMotor(enableMotor, targetVelocity, maxMotorImpulse);
        }

        public void SetMotorTarget(Quaternion qAinB, float dt)
        {
            var o = (HingeConstraintImp) _hci.UserObject;
            o._hci.SetMotorTarget(Translater.QuaternionToBtQuaternion(qAinB), dt);
        }
        public void SetMotorTarget(float targetAngle, float dt)
        {
            var o = (HingeConstraintImp) _hci.UserObject;
            o._hci.SetMotorTarget(targetAngle, dt);
        }

        public float MaxMotorImpulse
        {
            get
            {
                var retval = _hci.MaxMotorImpulse;
                return retval;
            }
            set
            {
                var o = (HingeConstraintImp) _hci.UserObject;
                o._hci.MaxMotorImpulse = value;
            }
        }
        public float MotorTargetVelocity
        {
            get
            {
                var retval = _hci.MotorTargetVelocity;
                return retval;
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

        public float4x4 FrameOffsetA
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_hci.FrameOffsetA);
                return retval;
            }
        }
        public float4x4 FrameOffsetB
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_hci.FrameOffsetB);
                return retval;
            }
        }

        public void SetAxis(float3 axisInA)
        {
            var o = (HingeConstraintImp) _hci.UserObject;
            _hci.SetAxis(Translater.Float3ToBtVector3(axisInA));
        }

        public float GetHingeAngle()
        {
            var retval =  _hci.GetHingeAngle();
            return retval;
        }
        public float GetHingeAngle(float4x4 transA, float4x4 transB)
        {
            var retval = _hci.GetHingeAngle(Translater.Float4X4ToBtMatrix(transA), Translater.Float4X4ToBtMatrix(transB));
            return retval;
        }

        public void SetLimit(float low, float high, float softness = 0.9f, float biasFactor = 0.3f, float relaxationFactor = 1)
        {
            _hci.SetLimit(low, high, softness, biasFactor, relaxationFactor);
        }

        public int SolverLimit
        {
            get
            {
                var retval = _hci.SolveLimit;
                return retval;
            }
        }
        public float LowerLimit
        {
            get
            {
                var retval = _hci.LowerLimit;
                return retval;
            }
        }
        public float UpperLimit
        {
            get
            {

                var retval = _hci.UpperLimit;
                return retval;
            }
        }


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
        public int GetUid()
        {
            var retval = _hci.Uid;
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
