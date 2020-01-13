using BulletSharp;
using Fusee.Engine.Imp.Physics.Common;
using Fusee.Math.Core;
using Quaternion = Fusee.Math.Core.Quaternion;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IConeTwistConstraintImp" /> interface using the bullet physics engine.
    /// </summary>
    public class ConeTwistConstraintImp : IConeTwistConstraintImp
    {
        internal ConeTwistConstraint _cti;

        /// <summary>
        /// Gets a frame.
        /// </summary>
        /// <value>
        /// a frame.
        /// </value>
        public float4x4 AFrame
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_cti.AFrame);
                return retval;
            }
        }
        /// <summary>
        /// Gets the b frame.
        /// </summary>
        /// <value>
        /// The b frame.
        /// </value>
        public float4x4 BFrame
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_cti.BFrame);
                return retval;
            }
        }

        /// <summary>
        /// Calculates the angle information.
        /// </summary>
        public void CalcAngleInfo()
        {
            _cti.CalcAngleInfo();
        }
        /// <summary>
        /// Calculates the angle info2.
        /// </summary>
        /// <param name="transA">The trans a.</param>
        /// <param name="transB">The trans b.</param>
        /// <param name="invInertiaWorldA">The inv inertia world a.</param>
        /// <param name="invInertiaWorldB">The inv inertia world b.</param>
        public void CalcAngleInfo2(float4x4 transA, float4x4 transB, float4x4 invInertiaWorldA, float4x4 invInertiaWorldB)
        {
            _cti.CalcAngleInfo2(Translator.Float4X4ToBtMatrix(transA), Translator.Float4X4ToBtMatrix(transB), Translator.Float4X4ToBtMatrix(invInertiaWorldA), Translator.Float4X4ToBtMatrix(invInertiaWorldB));
        }

        /// <summary>
        /// Enables the motor.
        /// </summary>
        /// <param name="b">if set to <c>true</c> [b].</param>
        public void EnableMotor(bool b)
        {
            _cti.EnableMotor(b);
        }

        /// <summary>
        /// Gets and sets the fix thresh.
        /// </summary>
        /// <value>
        /// The fix thresh.
        /// </value>
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

        /// <summary>
        /// Gets the frame offset a.
        /// </summary>
        /// <value>
        /// The frame offset a.
        /// </value>
        public float4x4 FrameOffsetA
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_cti.FrameOffsetA);
                return retval;
            }
        }
        /// <summary>
        /// Gets the frame offset b.
        /// </summary>
        /// <value>
        /// The frame offset b.
        /// </value>
        public float4x4 FrameOffsetB
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_cti.FrameOffsetB);
                return retval;
            }
        }

        /// <summary>
        /// Gets the point for angle.
        /// </summary>
        /// <param name="fAngleInRadius">The f angle in radius.</param>
        /// <param name="fLength">Length of the f.</param>
        /// <returns></returns>
        public float3 GetPointForAngle(float fAngleInRadius, float fLength)
        {
            var retval = Translator.BtVector3ToFloat3(_cti.GetPointForAngle(fAngleInRadius, fLength));
            return retval;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is past swing limit.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is past swing limit; otherwise, <c>false</c>.
        /// </value>
        public bool IsPastSwingLimit
        {
            get
            {
                var retval = _cti.IsPastSwingLimit;
                return retval;
            }
        }

        /// <summary>
        /// Sets the angular only.
        /// </summary>
        /// <param name="angularOnly">if set to <c>true</c> [angular only].</param>
        public void SetAngularOnly(bool angularOnly)
        {
            _cti.SetAngularOnly(angularOnly);
        }
        /// <summary>
        /// Sets the damping.
        /// </summary>
        /// <param name="damping">The damping.</param>
        public void SetDamping(float damping)
        {
            _cti.SetDamping(damping);
        }
        /// <summary>
        /// Sets the limit.
        /// </summary>
        /// <param name="limitIndex">Index of the limit.</param>
        /// <param name="limitValue">The limit value.</param>
        public void SetLimit(int limitIndex, float limitValue)
        {
            _cti.SetLimit(limitIndex, limitValue);
        }
        /// <summary>
        /// Sets the limit.
        /// </summary>
        /// <param name="swingSpan1">The swing span1.</param>
        /// <param name="swingSpan2">The swing span2.</param>
        /// <param name="twistSpan">The twist span.</param>
        /// <param name="softness">The softness.</param>
        /// <param name="biasFactor">The bias factor.</param>
        /// <param name="relaxationFactor">The relaxation factor.</param>
        public void SetLimit(float swingSpan1, float swingSpan2, float twistSpan, float softness, float biasFactor, float relaxationFactor)
        {
            _cti.SetLimit(swingSpan1, swingSpan2, twistSpan, softness, biasFactor, relaxationFactor);
        }

        /// <summary>
        /// Sets the maximum motor impulse.
        /// </summary>
        /// <param name="maxMotorImpulse">The maximum motor impulse.</param>
        public void SetMaxMotorImpulse(float maxMotorImpulse)
        {
            _cti.SetMaxMotorImpulse(maxMotorImpulse);
        }
        /// <summary>
        /// Sets the maximum motor impulse normalized.
        /// </summary>
        /// <param name="maxMotorImpulse">The maximum motor impulse.</param>
        public void SetMaxMotorImpulseNormalized(float maxMotorImpulse)
        {
           _cti.SetMaxMotorImpulseNormalized(maxMotorImpulse);
        }

        /// <summary>
        /// Sets the motor target.
        /// </summary>
        /// <param name="q">The q.</param>
        public void SetMotorTarget(Quaternion q)
        {
            _cti.SetMotorTarget(Translator.QuaternionToBtQuaternion(q));
        }
        /// <summary>
        /// Sets the motor target in constraint space.
        /// </summary>
        /// <param name="q">The q.</param>
        public void SetMotorTargetInConstraintSpace(Quaternion q)
        {
            _cti.SetMotorTargetInConstraintSpace(Translator.QuaternionToBtQuaternion(q));
        }

        /// <summary>
        /// Gets the solve swing limit.
        /// </summary>
        /// <value>
        /// The solve swing limit.
        /// </value>
        public int SolveSwingLimit
        {
            get
            {
                var retval = _cti.SolveSwingLimit;
                return retval;
            }
        }
        /// <summary>
        /// Gets the solve twist limit.
        /// </summary>
        /// <value>
        /// The solve twist limit.
        /// </value>
        public int SolveTwistLimit
        {
            get
            {
                var retval = _cti.SolveTwistLimit;
                return retval;
            }
        }

        /// <summary>
        /// Gets the swing span1.
        /// </summary>
        /// <value>
        /// The swing span1.
        /// </value>
        public float SwingSpan1
        {
            get
            {
                var retval = _cti.SwingSpan1;
                return retval;
            }
        }
        /// <summary>
        /// Gets the swing span2.
        /// </summary>
        /// <value>
        /// The swing span2.
        /// </value>
        public float SwingSpan2
        {
            get
            {
                var retval = _cti.SwingSpan2;
                return retval;
            }
        }

        /// <summary>
        /// Gets the twist angle.
        /// </summary>
        /// <value>
        /// The twist angle.
        /// </value>
        public float TwistAngle
        {
            get
            {
                var retval = _cti.TwistAngle;
                return retval;
            }
        }
        /// <summary>
        /// Gets the twist limit sign.
        /// </summary>
        /// <value>
        /// The twist limit sign.
        /// </value>
        public float TwistLimitSign
        {
            get
            {
                var retval = _cti.TwistLimitSign;
                return retval;
            }
        }
        /// <summary>
        /// Gets the twist span.
        /// </summary>
        /// <value>
        /// The twist span.
        /// </value>
        public float TwistSpan
        {
            get
            {
                var retval = _cti.TwistSpan;
                return retval;
            }
        }

        /// <summary>
        /// Updates the RHS.
        /// </summary>
        /// <param name="timeStep">The time step.</param>
        public void UpdateRhs(float timeStep)
        {
            _cti.UpdateRHS(timeStep);
        }

        #region IConstraintImp
        /// <summary>
        /// Gets the rigid body a.
        /// </summary>
        /// <value>
        /// The rigid body a.
        /// </value>
        public IRigidBodyImp RigidBodyA
        {
            get
            {
                var retval = _cti.RigidBodyA;
                return (RigidBodyImp)retval.UserObject;
            }
        }
        /// <summary>
        /// Gets the rigid body b.
        /// </summary>
        /// <value>
        /// The rigid body b.
        /// </value>
        public IRigidBodyImp RigidBodyB
        {
            get
            {
                var retval = _cti.RigidBodyB;
                return (RigidBodyImp)retval.UserObject;
            }
        }
        /// <summary>
        /// Gets the uid.
        /// </summary>
        /// <returns></returns>
        public int GetUid()
        {
            var retval = _cti.Uid;
            return retval;
        }

        private object _userObject;
        /// <summary>
        /// Gets and sets the user object.
        /// </summary>
        /// <value>
        /// The user object.
        /// </value>
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
        #endregion IConstraintImp 

        
    }
}
