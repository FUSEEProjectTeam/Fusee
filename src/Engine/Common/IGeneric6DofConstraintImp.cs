using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// Implementation agnostic representation of a six degrees of freedom constraint.
    /// </summary>
    public interface IGeneric6DofConstraintImp : IConstraintImp
    {
        /// <summary>
        /// Gets or sets the angular lower limit.
        /// </summary>
        /// <value>
        /// The angular lower limit.
        /// </value>
        float3 AngularLowerLimit { get; set; }
        /// <summary>
        /// Gets or sets the angular upper limit.
        /// </summary>
        /// <value>
        /// The angular upper limit.
        /// </value>
        float3 AngularUpperLimit { get; set; }

        /// <summary>
        /// Calculates the anchor position.
        /// </summary>
        void CalcAnchorPos();

        /// <summary>
        /// Calculates the transforms.
        /// </summary>
        void CalculateTransforms();
        /// <summary>
        /// Calculates the transforms.
        /// </summary>
        /// <param name="transA">The trans a.</param>
        /// <param name="transB">The trans b.</param>
        void CalculateTransforms(float4x4 transA, float4x4 transB);
        /// <summary>
        /// Gets the calculated transform a.
        /// </summary>
        /// <value>
        /// The calculated transform a.
        /// </value>
        float4x4 CalculatedTransformA { get; }
        /// <summary>
        /// Gets the calculated transform b.
        /// </summary>
        /// <value>
        /// The calculated transform b.
        /// </value>
        float4x4 CalculatedTransformB { get; }

        /// <summary>
        /// Gets or sets the frame offset a.
        /// </summary>
        /// <value>
        /// The frame offset a.
        /// </value>
        float4x4 FrameOffsetA { get; set; }
        /// <summary>
        /// Gets or sets the frame offset b.
        /// </summary>
        /// <value>
        /// The frame offset b.
        /// </value>
        float4x4 FrameOffsetB { get; set; }

        /// <summary>
        /// Gets the angle.
        /// </summary>
        /// <param name="axisIndex">Index of the axis.</param>
        /// <returns></returns>
        float GetAngle(int axisIndex);
        /// <summary>
        /// Gets the axis.
        /// </summary>
        /// <param name="axisIndex">Index of the axis.</param>
        /// <returns></returns>
        float3 GetAxis(int axisIndex);

        /// <summary>
        /// Gets the relative pivot position.
        /// </summary>
        /// <param name="axisIndex">Index of the axis.</param>
        /// <returns></returns>
        float GetRelativePivotPosition(int axisIndex);
        //Todo: RotationalLimitMotor

        /// <summary>
        /// Determines whether the specified limit index is limited.
        /// </summary>
        /// <param name="limitIndex">Index of the limit.</param>
        /// <returns></returns>
        bool IsLimited(int limitIndex);
        /// <summary>
        /// Gets or sets the linear lower limit.
        /// </summary>
        /// <value>
        /// The linear lower limit.
        /// </value>
        float3 LinearLowerLimit { get; set; }
        /// <summary>
        /// Gets or sets the linear upper limit.
        /// </summary>
        /// <value>
        /// The linear upper limit.
        /// </value>
        float3 LinearUpperLimit { get; set; }

        /// <summary>
        /// Sets the axis.
        /// </summary>
        /// <param name="axis1">The axis1.</param>
        /// <param name="axis2">The axis2.</param>
        void SetAxis(float3 axis1, float3 axis2);
        /// <summary>
        /// Sets the frames.
        /// </summary>
        /// <param name="frameA">The frame a.</param>
        /// <param name="frameB">The frame b.</param>
        void SetFrames(float4x4 frameA, float4x4 frameB);
        /// <summary>
        /// Sets the limit.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="lo">The lo.</param>
        /// <param name="hi">The hi.</param>
        void SetLimit(int axis, float lo, float hi);

        /// <summary>
        /// Tests the angular limit motor.
        /// </summary>
        /// <param name="axisIndex">Index of the axis.</param>
        /// <returns></returns>
        bool TestAngularLimitMotor(int axisIndex);
        //Todo: TranslationalLimitMotor

        /// <summary>
        /// Gets or sets a value indicating whether [use frame offset].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use frame offset]; otherwise, <c>false</c>.
        /// </value>
        bool UseFrameOffset { get; set; }

        /// <summary>
        /// Updates the RHS.
        /// </summary>
        /// <param name="timeStep">The time step.</param>
        void UpdateRhs(float timeStep);

    }
}
