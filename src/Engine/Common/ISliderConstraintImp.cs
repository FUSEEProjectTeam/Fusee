using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Implementation agnostic representation of a slider constraint (linear joint).
    /// </summary>
    public interface ISliderConstraintImp : IConstraintImp
    {
        /// <summary>
        /// Gets the anchor in a.
        /// </summary>
        /// <value>
        /// The anchor in a.
        /// </value>
        float3 AnchorInA { get; }
        /// <summary>
        /// Gets the anchor in b.
        /// </summary>
        /// <value>
        /// The anchor in b.
        /// </value>
        float3 AnchorInB { get; }
        /// <summary>
        /// Gets the angular depth.
        /// </summary>
        /// <value>
        /// The angular depth.
        /// </value>
        float AngularDepth { get; }
        /// <summary>
        /// Gets the angular position.
        /// </summary>
        /// <value>
        /// The angular position.
        /// </value>
        float AngularPos { get; }

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
        /// Gets and sets the damping dir angular.
        /// </summary>
        /// <value>
        /// The damping dir angular.
        /// </value>
        float DampingDirAngular { get; set; }
        /// <summary>
        /// Gets and sets the damping dir lin.
        /// </summary>
        /// <value>
        /// The damping dir lin.
        /// </value>
        float DampingDirLin { get; set; }
        /// <summary>
        /// Gets and sets the damping lim angular.
        /// </summary>
        /// <value>
        /// The damping lim angular.
        /// </value>
        float DampingLimAngular { get; set; }
        /// <summary>
        /// Gets and sets the damping lim lin.
        /// </summary>
        /// <value>
        /// The damping lim lin.
        /// </value>
        float DampingLimLin { get; set; }
        /// <summary>
        /// Gets and sets the damping ortho angular.
        /// </summary>
        /// <value>
        /// The damping ortho angular.
        /// </value>
        float DampingOrthoAngular { get; set; }
        /// <summary>
        /// Gets and sets the damping ortho lin.
        /// </summary>
        /// <value>
        /// The damping ortho lin.
        /// </value>
        float DampingOrthoLin { get; set; }

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
        /// Gets the lin depth.
        /// </summary>
        /// <value>
        /// The lin depth.
        /// </value>
        float LinDepth { get; }
        /// <summary>
        /// Gets the lin position.
        /// </summary>
        /// <value>
        /// The lin position.
        /// </value>
        float LinPos { get; }

        /// <summary>
        /// Gets and sets the lower angular limit.
        /// </summary>
        /// <value>
        /// The lower angular limit.
        /// </value>
        float LowerAngularLimit { get; set; }
        /// <summary>
        /// Gets and sets the lower lin limit.
        /// </summary>
        /// <value>
        /// The lower lin limit.
        /// </value>
        float LowerLinLimit { get; set;}

        /// <summary>
        /// Gets and sets the maximum angular motor force.
        /// </summary>
        /// <value>
        /// The maximum angular motor force.
        /// </value>
        float MaxAngularMotorForce { get; set; }
        /// <summary>
        /// Gets and sets the maximum lin motor force.
        /// </summary>
        /// <value>
        /// The maximum lin motor force.
        /// </value>
        float MaxLinMotorForce { get; set; }

        /// <summary>
        /// Gets and sets a value indicating whether [powered angular motor].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [powered angular motor]; otherwise, <c>false</c>.
        /// </value>
        bool PoweredAngularMotor { get; set; }
        /// <summary>
        /// Gets and sets a value indicating whether [powered lin motor].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [powered lin motor]; otherwise, <c>false</c>.
        /// </value>
        bool PoweredLinMotor { get; set; }

        /// <summary>
        /// Gets and sets the restitution dir angular.
        /// </summary>
        /// <value>
        /// The restitution dir angular.
        /// </value>
        float RestitutionDirAngular { get; set; }
        /// <summary>
        /// Gets and sets the restitution dir lin.
        /// </summary>
        /// <value>
        /// The restitution dir lin.
        /// </value>
        float RestitutionDirLin { get; set; }
        /// <summary>
        /// Gets and sets the restitution lim angular.
        /// </summary>
        /// <value>
        /// The restitution lim angular.
        /// </value>
        float RestitutionLimAngular { get; set; }
        /// <summary>
        /// Gets and sets the restitution lim lin.
        /// </summary>
        /// <value>
        /// The restitution lim lin.
        /// </value>
        float RestitutionLimLin { get; set; }
        /// <summary>
        /// Gets and sets the restitution ortho angular.
        /// </summary>
        /// <value>
        /// The restitution ortho angular.
        /// </value>
        float RestitutionOrthoAngular { get; set; }
        /// <summary>
        /// Gets and sets the restitution ortho lin.
        /// </summary>
        /// <value>
        /// The restitution ortho lin.
        /// </value>
        float RestitutionOrthoLin { get; set; }

        /// <summary>
        /// Sets the frames.
        /// </summary>
        /// <param name="frameA">The frame a.</param>
        /// <param name="frameB">The frame b.</param>
        void SetFrames(float4x4 frameA, float4x4 frameB);

        /// <summary>
        /// Gets and sets the softness dir angular.
        /// </summary>
        /// <value>
        /// The softness dir angular.
        /// </value>
        float SoftnessDirAngular { get; set; }
        /// <summary>
        /// Gets and sets the softness dir lin.
        /// </summary>
        /// <value>
        /// The softness dir lin.
        /// </value>
        float SoftnessDirLin { get; set; }
        /// <summary>
        /// Gets and sets the softness lim angular.
        /// </summary>
        /// <value>
        /// The softness lim angular.
        /// </value>
        float SoftnessLimAngular { get; set; }
        /// <summary>
        /// Gets and sets the softness lim lin.
        /// </summary>
        /// <value>
        /// The softness lim lin.
        /// </value>
        float SoftnessLimLin { get; set; }
        /// <summary>
        /// Gets and sets the softness ortho angular.
        /// </summary>
        /// <value>
        /// The softness ortho angular.
        /// </value>
        float SoftnessOrthoAngular { get; set; }
        /// <summary>
        /// Gets and sets the softness ortho lin.
        /// </summary>
        /// <value>
        /// The softness ortho lin.
        /// </value>
        float SoftnessOrthoLin { get; set; }

        /// <summary>
        /// Gets a value indicating whether [solve angular limit].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [solve angular limit]; otherwise, <c>false</c>.
        /// </value>
        bool SolveAngularLimit { get;}
        /// <summary>
        /// Gets a value indicating whether [solve lin limit].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [solve lin limit]; otherwise, <c>false</c>.
        /// </value>
        bool SolveLinLimit { get; }

        /// <summary>
        /// Gets and sets the target angular motor velocity.
        /// </summary>
        /// <value>
        /// The target angular motor velocity.
        /// </value>
        float TargetAngularMotorVelocity { get; set; }
        /// <summary>
        /// Gets and sets the target lin motor velocity.
        /// </summary>
        /// <value>
        /// The target lin motor velocity.
        /// </value>
        float TargetLinMotorVelocity { get; set; }

        /// <summary>
        /// Tests the angular limits.
        /// </summary>
        void TestAngularLimits();
        /// <summary>
        /// Tests the lin limits.
        /// </summary>
        void TestLinLimits();

        /// <summary>
        /// Gets and sets the upper angular limit.
        /// </summary>
        /// <value>
        /// The upper angular limit.
        /// </value>
        float UpperAngularLimit { get; set; }
        /// <summary>
        /// Gets and sets the upper lin limit.
        /// </summary>
        /// <value>
        /// The upper lin limit.
        /// </value>
        float UpperLinLimit { get; set; }

        /// <summary>
        /// Gets and sets a value indicating whether [use frame offset].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use frame offset]; otherwise, <c>false</c>.
        /// </value>
        bool UseFrameOffset { get; set; }

        /// <summary>
        /// Gets a value indicating whether [use linear reference frame a].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use linear reference frame a]; otherwise, <c>false</c>.
        /// </value>
        bool UseLinearReferenceFrameA { get; }

    }
}
