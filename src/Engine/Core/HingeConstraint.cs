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

        public bool AngularOnly
        {
            get
            {
                var retval = _iHConstraintImp.AngularOnly;
                return retval;
            }
            set
            {
                var o = (HingeConstraint) _iHConstraintImp.UserObject;
                o._iHConstraintImp.AngularOnly = value;
            }
        }

        public bool EnableMotor
        {
            get
            {
                var retval = _iHConstraintImp.EnableMotor;
                return retval;
            }
            set
            {
                var o = (HingeConstraint)_iHConstraintImp.UserObject;
                o._iHConstraintImp.EnableMotor = value;
            }
        }

        public void EnableAngularMotor(bool enableMotor, float targetVelocity, float maxMotorImpulse)
        {
            var o = (HingeConstraint) _iHConstraintImp.UserObject;
            o._iHConstraintImp.EnableAngularMotor(enableMotor, targetVelocity, maxMotorImpulse);
        }

        public void SetMotorTarget(Quaternion qAinB, float dt)
        {
            var o = (HingeConstraint) _iHConstraintImp.UserObject;
            o._iHConstraintImp.SetMotorTarget(qAinB, dt);
        }

        public void SetMotorTarget(float targetAngle, float dt)
        {
            var o = (HingeConstraint)_iHConstraintImp.UserObject;
            o._iHConstraintImp.SetMotorTarget(targetAngle, dt);
        }

        public float MaxMotorImpulse
        {
            get
            {
                var retval = _iHConstraintImp.MaxMotorImpulse;
                return retval;
            }
            set
            {
                var o = (HingeConstraint) _iHConstraintImp.UserObject;
                o._iHConstraintImp.MaxMotorImpulse = value;
            }
        }

        public float MotorTargetVelocity
        {
            get
            {
                var retval = _iHConstraintImp.MotorTargetVelocity;
                return retval;
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

        public void SetAxis(float3 axisInA)
        {
            var o = (HingeConstraint)_iHConstraintImp.UserObject;
            _iHConstraintImp.SetAxis(axisInA);
        }

        public float GetHingeAngle()
        {
            var retval = _iHConstraintImp.GetHingeAngle();
            return retval;
        }

        public float GetHingeAngle(float4x4 transA, float4x4 transB)
        {
            var retval = _iHConstraintImp.GetHingeAngle(transA, transB);
            return retval;
        }

        public void SetLimit(float low, float high, float softness = 0.9f, float biasFactor = 0.3f, float relaxationFactor = 1)
        {
            _iHConstraintImp.SetLimit(low, high, softness, biasFactor, relaxationFactor);
        }

        public int SolverLimit
        {
            get
            {
                var retval = _iHConstraintImp.SolverLimit;
                return retval;
            }
        }

        public float GetLowerLimit
        {
            get
            {
                var retval = _iHConstraintImp.LowerLimit;
                return retval;
            }
        }

        public float GetUpperLimit
        {
            get
            {

                var retval = _iHConstraintImp.LowerLimit;
                return retval;
            }
        }

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

        public int GetUid()
        {
            var retval = _iHConstraintImp.GetUid();
            return retval;
        }
    }
}
