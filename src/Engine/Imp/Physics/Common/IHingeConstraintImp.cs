using System;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Common
{
    /// <summary>
    /// Flags describing hinge detail.
    /// </summary>
    [Flags]
    public enum HingeFlags
    {
#pragma warning disable 1591
        HingeFlagsStopCfm = 1,

        HingeFlagsStopErp = 2,
        HingeFlagsNormCfm = 4
#pragma warning restore 1591

    };

    /// <summary>
    /// Implementation agnostic representation of a hinge constraint.
    /// </summary>
    public interface IHingeConstraintImp : IConstraintImp
    {

        /// <summary>
        /// Gets and sets a value indicating whether [angular only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [angular only]; otherwise, <c>false</c>.
        /// </value>
        bool AngularOnly { get; set; }
        /// <summary>
        /// Gets and sets a value indicating whether [enable motor].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable motor]; otherwise, <c>false</c>.
        /// </value>
        bool EnableMotor { get; set; }
        /// <summary>
        /// Enables the angular motor.
        /// </summary>
        /// <param name="enableMotor">if set to <c>true</c> [enable motor].</param>
        /// <param name="targetVelocity">The target velocity.</param>
        /// <param name="maxMotorImpulse">The maximum motor impulse.</param>
        void EnableAngularMotor(bool enableMotor, float targetVelocity, float maxMotorImpulse);

        /// <summary>
        /// Sets the motor target.
        /// </summary>
        /// <param name="qAinB">The q ain b.</param>
        /// <param name="dt">The dt.</param>
        void SetMotorTarget(Quaternion qAinB, float dt);
        /// <summary>
        /// Sets the motor target.
        /// </summary>
        /// <param name="targetAngle">The target angle.</param>
        /// <param name="dt">The dt.</param>
        void SetMotorTarget(float targetAngle, float dt);

        /// <summary>
        /// Gets and sets the maximum motor impulse.
        /// </summary>
        /// <value>
        /// The maximum motor impulse.
        /// </value>
        float MaxMotorImpulse { get; set; }
        /// <summary>
        /// Gets the motor target velocity.
        /// </summary>
        /// <value>
        /// The motor target velocity.
        /// </value>
        float MotorTargetVelocity { get; }

        /// <summary>
        /// Gets the frame of object A.
        /// </summary>
        /// <value>
        /// The frame for object A.
        /// </value>
        float4x4 FrameA { get; }
        /// <summary>
        /// Gets the frame of object B.
        /// </summary>
        /// <value>
        /// The frame of B.
        /// </value>
        float4x4 FrameB { get; }
        /// <summary>
        /// Gets the frame offset of object A.
        /// </summary>
        /// <value>
        /// The frame offset for A.
        /// </value>
        float4x4 FrameOffsetA { get; }
        /// <summary>
        /// Gets the frame offset of object B.
        /// </summary>
        /// <value>
        /// The frame offset for B.
        /// </value>
        float4x4 FrameOffsetB { get; }

        /// <summary>
        /// Sets the axis.
        /// </summary>
        /// <param name="axisInA">The axis in a.</param>
        void SetAxis(float3 axisInA);

        /// <summary>
        /// Gets the hinge angle.
        /// </summary>
        /// <returns></returns>
        float GetHingeAngle();
        /// <summary>
        /// Gets the hinge angle.
        /// </summary>
        /// <param name="transA">The trans a.</param>
        /// <param name="transB">The trans b.</param>
        /// <returns></returns>
        float GetHingeAngle(float4x4 transA, float4x4 transB);

        /// <summary>
        /// Sets the limit.
        /// </summary>
        /// <param name="low">The low.</param>
        /// <param name="high">The high.</param>
        /// <param name="softness">The softness.</param>
        /// <param name="biasFactor">The bias factor.</param>
        /// <param name="relaxationFactor">The relaxation factor.</param>
        void SetLimit(float low, float high, float softness = 0.9f, float biasFactor=0.3f, float relaxationFactor=1.0f);
        /// <summary>
        /// Gets the solver limit.
        /// </summary>
        /// <value>
        /// The solver limit.
        /// </value>
        int SolverLimit { get; }
        /// <summary>
        /// Gets the lower limit.
        /// </summary>
        /// <value>
        /// The lower limit.
        /// </value>
        float LowerLimit { get; }
        /// <summary>
        /// Gets the upper limit.
        /// </summary>
        /// <value>
        /// The upper limit.
        /// </value>
        float UpperLimit { get; }
    }
}
