using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Implementation agnostic representation of a cone twist constraint (a shoulder joint with arbitrary arm roll).
    /// </summary>
    public interface IConeTwistConstraintImp : IConstraintImp
    {
        /// <summary>
        /// Gets the A frame.
        /// </summary>
        /// <value>
        /// The A frame.
        /// </value>
        float4x4 AFrame { get; }
        /// <summary>
        /// Gets the B frame.
        /// </summary>
        /// <value>
        /// The B frame.
        /// </value>
        float4x4 BFrame { get; }

        /// <summary>
        /// Calculates the angle information.
        /// </summary>
        void CalcAngleInfo();
        /// <summary>
        /// Alternative angle info calculation.
        /// </summary>
        /// <param name="transA">The trans a.</param>
        /// <param name="transB">The trans b.</param>
        /// <param name="invInertiaWorldA">The inv inertia world a.</param>
        /// <param name="invInertiaWorldB">The inv inertia world b.</param>
        void CalcAngleInfo2(float4x4 transA, float4x4 transB, float4x4 invInertiaWorldA, float4x4 invInertiaWorldB);
        /// <summary>
        /// Enables the motor.
        /// </summary>
        /// <param name="b">if set to <c>true</c> [b].</param>
        void EnableMotor(bool b);
        /// <summary>
        /// Gets or sets the fix threshold.
        /// </summary>
        /// <value>
        /// The fix threshold.
        /// </value>
        float FixThresh { get; set; }
        /// <summary>
        /// Gets the frame offset a.
        /// </summary>
        /// <value>
        /// The frame offset a.
        /// </value>
        float4x4 FrameOffsetA { get; }
        /// <summary>
        /// Gets the frame offset b.
        /// </summary>
        /// <value>
        /// The frame offset b.
        /// </value>
        float4x4 FrameOffsetB { get; }
        /// <summary>
        /// Gets the point for angle.
        /// </summary>
        /// <param name="fAngleInRadius">The f angle in radius.</param>
        /// <param name="fLength">Length of the f.</param>
        /// <returns></returns>
        float3 GetPointForAngle(float fAngleInRadius, float fLength);
        /// <summary>
        /// Gets a value indicating whether this constraint/joint is past the swing limit.
        /// </summary>
        /// <value>
        /// <c>true</c> if this constraint is past swing limit; otherwise, <c>false</c>.
        /// </value>
        bool IsPastSwingLimit { get; }
        /// <summary>
        /// Set the angular only mode.
        /// </summary>
        /// <param name="angularOnly">if set to <c>true</c> only angular constraint is effective.</param>
        void SetAngularOnly(bool angularOnly);
        /// <summary>
        /// Sets the damping.
        /// </summary>
        /// <param name="damping">The damping.</param>
        void SetDamping(float damping);
        /// <summary>
        /// Sets the limit.
        /// </summary>
        /// <param name="limitIndex">Index of the limit.</param>
        /// <param name="limitValue">The limit value.</param>
        void SetLimit(int limitIndex, float limitValue);
        /// <summary>
        /// Sets the limit.
        /// </summary>
        /// <param name="swingSpan1">The swing span1.</param>
        /// <param name="swingSpan2">The swing span2.</param>
        /// <param name="twistSpan">The twist span.</param>
        /// <param name="softness">The softness.</param>
        /// <param name="biasFactor">The bias factor.</param>
        /// <param name="relaxationFactor">The relaxation factor.</param>
        void SetLimit(float swingSpan1, float swingSpan2, float twistSpan, float softness, float biasFactor, float relaxationFactor);

        /// <summary>
        /// Sets the maximum motor impulse.
        /// </summary>
        /// <param name="maxMotorImpulse">The maximum motor impulse.</param>
        void SetMaxMotorImpulse(float maxMotorImpulse);
        /// <summary>
        /// Sets the normalized maximum motor impulse.
        /// </summary>
        /// <param name="maxMotorImpulse">The maximum motor impulse.</param>
        void SetMaxMotorImpulseNormalized(float maxMotorImpulse);

        /// <summary>
        /// Sets the motor target.
        /// </summary>
        /// <param name="q">Quaternion (orientation) of the target.</param>
        void SetMotorTarget(Quaternion q);
        /// <summary>
        /// Sets the motor target in constraint space.
        /// </summary>
        /// <param name="q">Quaternion (orientation) of the target in constraint space.</param>
        void SetMotorTargetInConstraintSpace(Quaternion q);

        /// <summary>
        /// Gets the solve swing limit.
        /// </summary>
        /// <value>
        /// The solve swing limit.
        /// </value>
        int SolveSwingLimit { get; }
        /// <summary>
        /// Gets the solve twist limit.
        /// </summary>
        /// <value>
        /// The solve twist limit.
        /// </value>
        int SolveTwistLimit { get; }

        /// <summary>
        /// Gets the swing span1.
        /// </summary>
        /// <value>
        /// The swing span1.
        /// </value>
        float SwingSpan1 { get; }
        /// <summary>
        /// Gets the swing span2.
        /// </summary>
        /// <value>
        /// The swing span2.
        /// </value>
        float SwingSpan2 { get; }

        /// <summary>
        /// Gets the twist angle.
        /// </summary>
        /// <value>
        /// The twist angle.
        /// </value>
        float TwistAngle { get; }
        /// <summary>
        /// Gets the twist limit sign.
        /// </summary>
        /// <value>
        /// The twist limit sign.
        /// </value>
        float TwistLimitSign { get; }
        /// <summary>
        /// Gets the twist span.
        /// </summary>
        /// <value>
        /// The twist span.
        /// </value>
        float TwistSpan { get; }

        /// <summary>
        /// Updates the RHS.
        /// </summary>
        /// <param name="timeStep">The time step.</param>
        void UpdateRhs(float timeStep);
    }
}
