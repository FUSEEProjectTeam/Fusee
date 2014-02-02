using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class ConeTwistConstraint
    {
        internal IConeTwistConstraintImp _iCTConstraintImp;
        public float4x4 AFrame
        {
            get
            {
                var retval =_iCTConstraintImp.AFrame;
                return retval;
            }
        }
        public float4x4 BFrame
        {
            get
            {
                var retval = _iCTConstraintImp.BFrame;
                return retval;
            }
        }

        public void CalcAngleInfo()
        {
            _iCTConstraintImp.CalcAngleInfo();
        }
        public void CalcAngleInfo2(float4x4 transA, float4x4 transB, float4x4 invInertiaWorldA, float4x4 invInertiaWorldB)
        {
            _iCTConstraintImp.CalcAngleInfo2(transA, transB, invInertiaWorldA, invInertiaWorldB);
        }

        public void EnableMotor(bool b)
        {
            _iCTConstraintImp.EnableMotor(b);
        }

        public float FixThresh
        {
            get
            {
                var retval = _iCTConstraintImp.FixThresh;
                return retval;
            }
            set
            {
                var o = (ConeTwistConstraint)_iCTConstraintImp.UserObject;
                o._iCTConstraintImp.FixThresh = value;
            }
        }

        public float4x4 FrameOffsetA
        {
            get
            {
                var retval = _iCTConstraintImp.FrameOffsetA;
                return retval;
            }
        }
        public float4x4 FrameOffsetB
        {
            get
            {
                var retval = _iCTConstraintImp.FrameOffsetB;
                return retval;
            }
        }

        public float3 GetPointForAngle(float fAngleInRadius, float fLength)
        {
            var retval = _iCTConstraintImp.GetPointForAngle(fAngleInRadius, fLength);
            return retval;
        }

        public bool IsPastSwingLimit
        {
            get
            {
                var retval = _iCTConstraintImp.IsPastSwingLimit;
                return retval;
            }
        }

        public void SetAngularOnly(bool angularOnly)
        {
            _iCTConstraintImp.SetAngularOnly(angularOnly);
        }
        public void SetDamping(float damping)
        {
            _iCTConstraintImp.SetDamping(damping);
        }
        public void SetLimit(int limitIndex, float limitValue)
        {
            _iCTConstraintImp.SetLimit(limitIndex, limitValue);
        }
        public void SetLimit(float swingSpan1, float swingSpan2, float twistSpan, float softness = 1.0f, float biasFactor = 0.3f, float relaxationFactor = 1.0f)
        {
            _iCTConstraintImp.SetLimit(swingSpan1, swingSpan2, twistSpan, softness, biasFactor, relaxationFactor);
        }

        public void SetMaxMotorImpulse(float maxMotorImpulse)
        {
            _iCTConstraintImp.SetMaxMotorImpulse(maxMotorImpulse);
        }
        public void SetMaxMotorImpulseNormalized(float maxMotorImpulse)
        {
            _iCTConstraintImp.SetMaxMotorImpulseNormalized(maxMotorImpulse);
        }

        public void SetMotorTarget(Quaternion q)
        {
            _iCTConstraintImp.SetMotorTarget(q);
        }
        public void SetMotorTargetInConstraintSpace(Quaternion q)
        {
            _iCTConstraintImp.SetMotorTargetInConstraintSpace(q);
        }

        public int SolveSwingLimit
        {
            get
            {
                var retval = _iCTConstraintImp.SolveSwingLimit;
                return retval;
            }
        }
        public int SolveTwistLimit
        {
            get
            {
                var retval = _iCTConstraintImp.SolveTwistLimit;
                return retval;
            }
        }

        public float SwingSpan1
        {
            get
            {
                var retval = _iCTConstraintImp.SwingSpan1;
                return retval;
            }
        }
        public float SwingSpan2
        {
            get
            {
                var retval = _iCTConstraintImp.SwingSpan2;
                return retval;
            }
        }

        public float TwistAngle
        {
            get
            {
                var retval = _iCTConstraintImp.TwistAngle;
                return retval;
            }
        }
        public float TwistLimitSign
        {
            get
            {
                var retval = _iCTConstraintImp.TwistLimitSign;
                return retval;
            }
        }
        public float TwistSpan
        {
            get
            {
                var retval = _iCTConstraintImp.TwistSpan;
                return retval;
            }
        }

        public void UpdateRhs(float timeStep)
        {
            _iCTConstraintImp.UpdateRhs(timeStep);
        }

        public RigidBody RigidBodyA
        {
            get
            {

                var retval = _iCTConstraintImp.RigidBodyA.UserObject;
                return (RigidBody)retval;
            }
        }
        public RigidBody RigidBodyB
        {
            get
            {
                var retval = _iCTConstraintImp.RigidBodyB.UserObject;
                return (RigidBody)retval;
            }
        }
        public int GetUid()
        {
            var retval = _iCTConstraintImp.GetUid();
            return retval;
        }
    }
}
