using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Core
{
    /// <summary>
    /// A cone twist constraint
    /// </summary>
    public class ConeTwistConstraint
    {
        internal IConeTwistConstraintImp _iCTConstraintImp;
        /// <summary>
        /// Gets the reference frame of object a.
        /// </summary>
        /// <value>
        /// a's reference frame.
        /// </value>
        public float4x4 AFrame
        {
            get
            {
                var retval =_iCTConstraintImp.AFrame;
                return retval;
            }
        }
        /// <summary>
        /// Gets the reference frame of object b.
        /// </summary>
        /// <value>
        /// b's reference frame.
        /// </value>
        public float4x4 BFrame
        {
            get
            {
                var retval = _iCTConstraintImp.BFrame;
                return retval;
            }
        }

        /// <summary>
        /// Calculates the angle information.
        /// </summary>
        public void CalcAngleInfo()
        {
            _iCTConstraintImp.CalcAngleInfo();
        }
        /// <summary>
        /// Calculates the angle info 2.
        /// </summary>
        /// <param name="transA">The trans a.</param>
        /// <param name="transB">The trans b.</param>
        /// <param name="invInertiaWorldA">The inv inertia world a.</param>
        /// <param name="invInertiaWorldB">The inv inertia world b.</param>
        public void CalcAngleInfo2(float4x4 transA, float4x4 transB, float4x4 invInertiaWorldA, float4x4 invInertiaWorldB)
        {
            _iCTConstraintImp.CalcAngleInfo2(transA, transB, invInertiaWorldA, invInertiaWorldB);
        }

        /// <summary>
        /// Enables the motor functionality.
        /// </summary>
        /// <param name="b">if set to <c>true</c> enables motor behavior.</param>
        public void EnableMotor(bool b)
        {
            _iCTConstraintImp.EnableMotor(b);
        }

        /// <summary>
        /// Gets and sets the fix threshold.
        /// </summary>
        /// <value>
        /// The fix thresh.
        /// </value>
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

        /// <summary>
        /// Gets the frame offset of object a.
        /// </summary>
        /// <value>
        /// The frame offset of a.
        /// </value>
        public float4x4 FrameOffsetA
        {
            get
            {
                var retval = _iCTConstraintImp.FrameOffsetA;
                return retval;
            }
        }
        /// <summary>
        /// Gets the frame offset of object b.
        /// </summary>
        /// <value>
        /// The frame offset of b.
        /// </value>
        public float4x4 FrameOffsetB
        {
            get
            {
                var retval = _iCTConstraintImp.FrameOffsetB;
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
            var retval = _iCTConstraintImp.GetPointForAngle(fAngleInRadius, fLength);
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
                var retval = _iCTConstraintImp.IsPastSwingLimit;
                return retval;
            }
        }

        /// <summary>
        /// Sets the angular only.
        /// </summary>
        /// <param name="angularOnly">if set to <c>true</c> [angular only].</param>
        public void SetAngularOnly(bool angularOnly)
        {
            _iCTConstraintImp.SetAngularOnly(angularOnly);
        }
        /// <summary>
        /// Sets the damping.
        /// </summary>
        /// <param name="damping">The damping.</param>
        public void SetDamping(float damping)
        {
            _iCTConstraintImp.SetDamping(damping);
        }
        /// <summary>
        /// Sets the limit.
        /// </summary>
        /// <param name="limitIndex">Index of the limit.</param>
        /// <param name="limitValue">The limit value.</param>
        public void SetLimit(int limitIndex, float limitValue)
        {
            _iCTConstraintImp.SetLimit(limitIndex, limitValue);
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
        public void SetLimit(float swingSpan1, float swingSpan2, float twistSpan, float softness = 1.0f, float biasFactor = 0.3f, float relaxationFactor = 1.0f)
        {
            _iCTConstraintImp.SetLimit(swingSpan1, swingSpan2, twistSpan, softness, biasFactor, relaxationFactor);
        }

        /// <summary>
        /// Sets the maximum motor impulse.
        /// </summary>
        /// <param name="maxMotorImpulse">The maximum motor impulse.</param>
        public void SetMaxMotorImpulse(float maxMotorImpulse)
        {
            _iCTConstraintImp.SetMaxMotorImpulse(maxMotorImpulse);
        }
        /// <summary>
        /// Sets the normalized maximum motor impulse.
        /// </summary>
        /// <param name="maxMotorImpulse">The maximum motor impulse.</param>
        public void SetMaxMotorImpulseNormalized(float maxMotorImpulse)
        {
            _iCTConstraintImp.SetMaxMotorImpulseNormalized(maxMotorImpulse);
        }

        /// <summary>
        /// Sets the motor target.
        /// </summary>
        /// <param name="q">The q.</param>
        public void SetMotorTarget(Quaternion q)
        {
            _iCTConstraintImp.SetMotorTarget(q);
        }
        /// <summary>
        /// Sets the motor target in constraint space.
        /// </summary>
        /// <param name="q">The q.</param>
        public void SetMotorTargetInConstraintSpace(Quaternion q)
        {
            _iCTConstraintImp.SetMotorTargetInConstraintSpace(q);
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
                var retval = _iCTConstraintImp.SolveSwingLimit;
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
                var retval = _iCTConstraintImp.SolveTwistLimit;
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
                var retval = _iCTConstraintImp.SwingSpan1;
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
                var retval = _iCTConstraintImp.SwingSpan2;
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
                var retval = _iCTConstraintImp.TwistAngle;
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
                var retval = _iCTConstraintImp.TwistLimitSign;
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
                var retval = _iCTConstraintImp.TwistSpan;
                return retval;
            }
        }

        /// <summary>
        /// Updates the RHS.
        /// </summary>
        /// <param name="timeStep">The time step.</param>
        public void UpdateRhs(float timeStep)
        {
            _iCTConstraintImp.UpdateRhs(timeStep);
        }

        /// <summary>
        /// Gets the rigid body a.
        /// </summary>
        /// <value>
        /// The rigid body a.
        /// </value>
        public RigidBody RigidBodyA
        {
            get
            {

                var retval = _iCTConstraintImp.RigidBodyA.UserObject;
                return (RigidBody)retval;
            }
        }
        /// <summary>
        /// Gets the rigid body b.
        /// </summary>
        /// <value>
        /// The rigid body b.
        /// </value>
        public RigidBody RigidBodyB
        {
            get
            {
                var retval = _iCTConstraintImp.RigidBodyB.UserObject;
                return (RigidBody)retval;
            }
        }
        /// <summary>
        /// Gets the uid.
        /// </summary>
        /// <returns></returns>
        public int GetUid()
        {
            var retval = _iCTConstraintImp.GetUid();
            return retval;
        }
    }
}
