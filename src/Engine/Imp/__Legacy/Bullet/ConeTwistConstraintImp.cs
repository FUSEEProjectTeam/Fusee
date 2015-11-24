using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using Fusee.Math;
using Quaternion = Fusee.Math.Quaternion;

namespace Fusee.Engine
{
    public class ConeTwistConstraintImp : IConeTwistConstraintImp
    {
        internal ConeTwistConstraint _cti;
        internal Translater Translater = new Translater();

        public float4x4 AFrame
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_cti.AFrame);
                return retval;
            }
        }
        public float4x4 BFrame
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_cti.BFrame);
                return retval;
            }
        }

        public void CalcAngleInfo()
        {
            _cti.CalcAngleInfo();
        }
        public void CalcAngleInfo2(float4x4 transA, float4x4 transB, float4x4 invInertiaWorldA, float4x4 invInertiaWorldB)
        {
            _cti.CalcAngleInfo2(Translater.Float4X4ToBtMatrix(transA), Translater.Float4X4ToBtMatrix(transB), Translater.Float4X4ToBtMatrix(invInertiaWorldA), Translater.Float4X4ToBtMatrix(invInertiaWorldB));
        }

        public void EnableMotor(bool b)
        {
            _cti.EnableMotor(b);
        }

        public float FixThresh
        {
            get
            {
                var retval = _cti.FixThresh;
                return retval;
            }
            set
            {
                var o = (ConeTwistConstraintImp) _cti.UserObject;
                o._cti.FixThresh = value;
            }
        }

        public float4x4 FrameOffsetA
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_cti.FrameOffsetA);
                return retval;
            }
        }
        public float4x4 FrameOffsetB
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_cti.FrameOffsetB);
                return retval;
            }
        }

        public float3 GetPointForAngle(float fAngleInRadius, float fLength)
        {
            var retval = Translater.BtVector3ToFloat3(_cti.GetPointForAngle(fAngleInRadius, fLength));
            return retval;
        }

        public bool IsPastSwingLimit
        {
            get
            {
                var retval = _cti.IsPastSwingLimit;
                return retval;
            }
        }

        public void SetAngularOnly(bool angularOnly)
        {
            _cti.SetAngularOnly(angularOnly);
        }
        public void SetDamping(float damping)
        {
            _cti.SetDamping(damping);
        }
        public void SetLimit(int limitIndex, float limitValue)
        {
            _cti.SetLimit(limitIndex, limitValue);
        }
        public void SetLimit(float swingSpan1, float swingSpan2, float twistSpan, float softness, float biasFactor, float relaxationFactor)
        {
            _cti.SetLimit(swingSpan1, swingSpan2, twistSpan, softness, biasFactor, relaxationFactor);
        }

        public void SetMaxMotorImpulse(float maxMotorImpulse)
        {
            _cti.SetMaxMotorImpulse(maxMotorImpulse);
        }
        public void SetMaxMotorImpulseNormalized(float maxMotorImpulse)
        {
           _cti.SetMaxMotorImpulseNormalized(maxMotorImpulse);
        }

        public void SetMotorTarget(Quaternion q)
        {
            _cti.SetMotorTarget(Translater.QuaternionToBtQuaternion(q));
        }
        public void SetMotorTargetInConstraintSpace(Quaternion q)
        {
            _cti.SetMotorTargetInConstraintSpace(Translater.QuaternionToBtQuaternion(q));
        }

        public int SolveSwingLimit
        {
            get
            {
                var retval = _cti.SolveSwingLimit;
                return retval;
            }
        }
        public int SolveTwistLimit
        {
            get
            {
                var retval = _cti.SolveTwistLimit;
                return retval;
            }
        }

        public float SwingSpan1
        {
            get
            {
                var retval = _cti.SwingSpan1;
                return retval;
            }
        }
        public float SwingSpan2
        {
            get
            {
                var retval = _cti.SwingSpan2;
                return retval;
            }
        }

        public float TwistAngle
        {
            get
            {
                var retval = _cti.TwistAngle;
                return retval;
            }
        }
        public float TwistLimitSign
        {
            get
            {
                var retval = _cti.TwistLimitSign;
                return retval;
            }
        }
        public float TwistSpan
        {
            get
            {
                var retval = _cti.TwistSpan;
                return retval;
            }
        }

        public void UpdateRhs(float timeStep)
        {
            _cti.UpdateRHS(timeStep);
        }

        #region IConstraintImp
        public IRigidBodyImp RigidBodyA
        {
            get
            {
                var retval = _cti.RigidBodyA;
                return (RigidBodyImp)retval.UserObject;
            }
        }
        public IRigidBodyImp RigidBodyB
        {
            get
            {
                var retval = _cti.RigidBodyB;
                return (RigidBodyImp)retval.UserObject;
            }
        }
        public int GetUid()
        {
            var retval = _cti.Uid;
            return retval;
        }

        private object _userObject;
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
        #endregion IConstraintImp 

        
    }
}
