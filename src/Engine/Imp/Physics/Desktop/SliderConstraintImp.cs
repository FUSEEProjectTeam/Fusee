using BulletSharp;
using Fusee.Engine.Imp.Physics.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="ISliderConstraintImp" /> interface using the bullet physics engine.
    /// </summary>
    public class SliderConstraintImp : ISliderConstraintImp
    {
        internal SliderConstraint _sci;

        /// <summary>
        /// Gets the anchor in a.
        /// </summary>
        /// <value>
        /// The anchor in a.
        /// </value>
        public float3 AnchorInA
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(_sci.AnchorInA);
                return retval;
            }
        }
        /// <summary>
        /// Gets the anchor in b.
        /// </summary>
        /// <value>
        /// The anchor in b.
        /// </value>
        public float3 AnchorInB
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(_sci.AnchorInB);
                return retval;
            }
        }

        /// <summary>
        /// Gets the angular depth.
        /// </summary>
        /// <value>
        /// The angular depth.
        /// </value>
        public float AngularDepth
        {
            get
            {
                var retval = _sci.AngularDepth;
                return retval;
            }
        }
        /// <summary>
        /// Gets the angular position.
        /// </summary>
        /// <value>
        /// The angular position.
        /// </value>
        public float AngularPos
        {
            get
            {
                var retval = _sci.AngularPos;
                return retval;
            }
        }

        /// <summary>
        /// Calculates the transforms.
        /// </summary>
        /// <param name="transA">The trans a.</param>
        /// <param name="transB">The trans b.</param>
        public void CalculateTransforms(float4x4 transA, float4x4 transB)
        {
            _sci.CalculateTransforms(Translator.Float4X4ToBtMatrix(transA), Translator.Float4X4ToBtMatrix(transB));
        }
        /// <summary>
        /// Gets the calculated transform a.
        /// </summary>
        /// <value>
        /// The calculated transform a.
        /// </value>
        public float4x4 CalculatedTransformA
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_sci.CalculatedTransformA);
                return retval;
            }
        }
        /// <summary>
        /// Gets the calculated transform b.
        /// </summary>
        /// <value>
        /// The calculated transform b.
        /// </value>
        public float4x4 CalculatedTransformB
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_sci.CalculatedTransformB);
                return retval;
            }
        }

        /// <summary>
        /// Gets and sets the damping dir angular.
        /// </summary>
        /// <value>
        /// The damping dir angular.
        /// </value>
        public float DampingDirAngular
        {
            get
            {
                var retval = _sci.DampingDirAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp) _sci.UserObject;
                o._sci.DampingDirAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the damping dir lin.
        /// </summary>
        /// <value>
        /// The damping dir lin.
        /// </value>
        public float DampingDirLin
        {
            get
            {
                var retval = _sci.DampingDirLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.DampingDirLin = value;
            }
        }
        /// <summary>
        /// Gets and sets the damping lim angular.
        /// </summary>
        /// <value>
        /// The damping lim angular.
        /// </value>
        public float DampingLimAngular
        {
            get
            {
                var retval = _sci.DampingLimAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp) _sci.UserObject;
                o._sci.DampingLimAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the damping lim lin.
        /// </summary>
        /// <value>
        /// The damping lim lin.
        /// </value>
        public float DampingLimLin
        {
            get
            {
                var retval = _sci.DampingLimLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp) _sci.UserObject;
                o._sci.DampingLimLin = value;
            }
        }
        /// <summary>
        /// Gets and sets the damping ortho angular.
        /// </summary>
        /// <value>
        /// The damping ortho angular.
        /// </value>
        public float DampingOrthoAngular
        {
            get
            {
                var retval = _sci.DampingOrthoAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.DampingOrthoAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the damping ortho lin.
        /// </summary>
        /// <value>
        /// The damping ortho lin.
        /// </value>
        public float DampingOrthoLin
        {
            get
            {
                var retval = _sci.DampingOrthoLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.DampingOrthoLin = value;
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
                var retval = Translator.BtMatrixToFloat4X4(_sci.FrameOffsetA);
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
                var retval = Translator.BtMatrixToFloat4X4(_sci.FrameOffsetB);
                return retval;
            }
        }

        /// <summary>
        /// Gets the lin depth.
        /// </summary>
        /// <value>
        /// The lin depth.
        /// </value>
        public float LinDepth
        {
            get
            {
                var retval = _sci.LinDepth;
                return retval;
            }
        }
        /// <summary>
        /// Gets the lin position.
        /// </summary>
        /// <value>
        /// The lin position.
        /// </value>
        public float LinPos
        {
            get
            {
                var retval = _sci.LinearPos;
                return retval;
            }
        }

        /// <summary>
        /// Gets and sets the lower angular limit.
        /// </summary>
        /// <value>
        /// The lower angular limit.
        /// </value>
        public float LowerAngularLimit
        {
            get
            {
                var retval = _sci.LowerAngularLimit;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp) _sci.UserObject;
                o._sci.LowerAngularLimit = value;
            }
        }
        /// <summary>
        /// Gets and sets the lower lin limit.
        /// </summary>
        /// <value>
        /// The lower lin limit.
        /// </value>
        public float LowerLinLimit
        {
            get
            {
                var retval = _sci.LowerLinLimit;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp) _sci.UserObject;
                o._sci.LowerLinLimit = value;
            }
        }

        /// <summary>
        /// Gets and sets the maximum angular motor force.
        /// </summary>
        /// <value>
        /// The maximum angular motor force.
        /// </value>
        public float MaxAngularMotorForce
        {
            get
            {
                var retval = _sci.MaxAngularMotorForce;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp) _sci.UserObject;
                o._sci.MaxAngularMotorForce = value;
            }
        }
        /// <summary>
        /// Gets and sets the maximum lin motor force.
        /// </summary>
        /// <value>
        /// The maximum lin motor force.
        /// </value>
        public float MaxLinMotorForce
        {
            get
            {
                var retval = _sci.MaxLinMotorForce;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.MaxLinMotorForce = value;
            }
        }

        /// <summary>
        /// Gets and sets a value indicating whether [powered angular motor].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [powered angular motor]; otherwise, <c>false</c>.
        /// </value>
        public bool PoweredAngularMotor
        {
            get
            {
                var retval = _sci.PoweredAngularMotor;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp) _sci.UserObject;
                o._sci.PoweredAngularMotor = value;
            }
        }
        /// <summary>
        /// Gets and sets a value indicating whether [powered lin motor].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [powered lin motor]; otherwise, <c>false</c>.
        /// </value>
        public bool PoweredLinMotor
        {
            get
            {
                var retval = _sci.PoweredLinMotor;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.PoweredLinMotor = value;
            }
        }

        /// <summary>
        /// Gets and sets the restitution dir angular.
        /// </summary>
        /// <value>
        /// The restitution dir angular.
        /// </value>
        public float RestitutionDirAngular
        {
            get
            {
                var retval = _sci.RestitutionDirAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp) _sci.UserObject;
                o._sci.RestitutionDirAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the restitution dir lin.
        /// </summary>
        /// <value>
        /// The restitution dir lin.
        /// </value>
        public float RestitutionDirLin
        {
            get
            {
                var retval = _sci.RestitutionDirLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.RestitutionDirLin = value;
            }
        }
        /// <summary>
        /// Gets and sets the restitution lim angular.
        /// </summary>
        /// <value>
        /// The restitution lim angular.
        /// </value>
        public float RestitutionLimAngular
        {
            get
            {
                var retval = _sci.RestitutionLimAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.RestitutionLimAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the restitution lim lin.
        /// </summary>
        /// <value>
        /// The restitution lim lin.
        /// </value>
        public float RestitutionLimLin
        {
            get
            {
                var retval = _sci.RestitutionLimLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.RestitutionLimLin = value;
            }
        }
        /// <summary>
        /// Gets and sets the restitution ortho angular.
        /// </summary>
        /// <value>
        /// The restitution ortho angular.
        /// </value>
        public float RestitutionOrthoAngular
        {
            get
            {
                var retval = _sci.RestitutionOrthoAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.RestitutionOrthoAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the restitution ortho lin.
        /// </summary>
        /// <value>
        /// The restitution ortho lin.
        /// </value>
        public float RestitutionOrthoLin
        {
            get
            {
                var retval = _sci.RestitutionOrthoLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.RestitutionOrthoLin = value;
            }
        }

        /// <summary>
        /// Sets the frames.
        /// </summary>
        /// <param name="frameA">The frame a.</param>
        /// <param name="frameB">The frame b.</param>
        public void SetFrames(float4x4 frameA, float4x4 frameB)
        {
            var o = (SliderConstraintImp) _sci.UserObject;
            o._sci.SetFrames(Translator.Float4X4ToBtMatrix(frameA), Translator.Float4X4ToBtMatrix(frameB));
        }

        /// <summary>
        /// Gets and sets the softness dir angular.
        /// </summary>
        /// <value>
        /// The softness dir angular.
        /// </value>
        public float SoftnessDirAngular
        {
            get
            {
                var retval = _sci.SoftnessDirAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp) _sci.UserObject;
                o._sci.SoftnessDirAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the softness dir lin.
        /// </summary>
        /// <value>
        /// The softness dir lin.
        /// </value>
        public float SoftnessDirLin
        {
            get
            {
                var retval = _sci.SoftnessDirLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.SoftnessDirLin = value;
            }
        }
        /// <summary>
        /// Gets and sets the softness lim angular.
        /// </summary>
        /// <value>
        /// The softness lim angular.
        /// </value>
        public float SoftnessLimAngular
        {
            get
            {
                var retval = _sci.SoftnessLimAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.SoftnessLimAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the softness lim lin.
        /// </summary>
        /// <value>
        /// The softness lim lin.
        /// </value>
        public float SoftnessLimLin
        {
            get
            {
                var retval = _sci.SoftnessLimLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.SoftnessLimLin = value;
            }
        }
        /// <summary>
        /// Gets and sets the softness ortho angular.
        /// </summary>
        /// <value>
        /// The softness ortho angular.
        /// </value>
        public float SoftnessOrthoAngular
        {
            get
            {
                var retval = _sci.SoftnessOrthoAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.SoftnessOrthoAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the softness ortho lin.
        /// </summary>
        /// <value>
        /// The softness ortho lin.
        /// </value>
        public float SoftnessOrthoLin
        {
            get
            {
                var retval = _sci.SoftnessOrthoLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.SoftnessOrthoLin = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [solve angular limit].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [solve angular limit]; otherwise, <c>false</c>.
        /// </value>
        public bool SolveAngularLimit
        {
            get
            {
                var retval = _sci.SolveAngularLimit;
                return retval;
            }  
        }
        /// <summary>
        /// Gets a value indicating whether [solve lin limit].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [solve lin limit]; otherwise, <c>false</c>.
        /// </value>
        public bool SolveLinLimit
        {
            get
            {
                var retval = _sci.SolveLinLimit;
                return retval;
            }
        }

        /// <summary>
        /// Gets and sets the target angular motor velocity.
        /// </summary>
        /// <value>
        /// The target angular motor velocity.
        /// </value>
        public float TargetAngularMotorVelocity
        {
            get
            {
                var retval = _sci.TargetAngularMotorVelocity;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp) _sci.UserObject;
                o._sci.TargetAngularMotorVelocity = value;
            }
        }
        /// <summary>
        /// Gets and sets the target lin motor velocity.
        /// </summary>
        /// <value>
        /// The target lin motor velocity.
        /// </value>
        public float TargetLinMotorVelocity
        {
            get
            {
                var retval = _sci.TargetLinMotorVelocity;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.TargetLinMotorVelocity = value;
            }
        }

        /// <summary>
        /// Tests the angular limits.
        /// </summary>
        public void TestAngularLimits()
        {
            _sci.TestAngularLimits();
        }
        /// <summary>
        /// Tests the lin limits.
        /// </summary>
        public void TestLinLimits()
        {
            _sci.TestLinLimits();
        }

        /// <summary>
        /// Gets and sets the upper angular limit.
        /// </summary>
        /// <value>
        /// The upper angular limit.
        /// </value>
        public float UpperAngularLimit
        {
            get
            {
                var retval = _sci.UpperAngularLimit;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp) _sci.UserObject;
                o._sci.UpperAngularLimit = value;
            }
        }
        /// <summary>
        /// Gets and sets the upper lin limit.
        /// </summary>
        /// <value>
        /// The upper lin limit.
        /// </value>
        public float UpperLinLimit
        {
            get
            {
                var retval = _sci.UpperLinLimit;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.UpperLinLimit = value;
            }
        }

        /// <summary>
        /// Gets and sets a value indicating whether [use frame offset].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use frame offset]; otherwise, <c>false</c>.
        /// </value>
        public bool UseFrameOffset
        {
            get
            {
                var retval = _sci.UseFrameOffset;
                return retval;
            }
            set
            {
                var o = (SliderConstraintImp)_sci.UserObject;
                o._sci.UseFrameOffset = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [use linear reference frame a].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use linear reference frame a]; otherwise, <c>false</c>.
        /// </value>
        public bool UseLinearReferenceFrameA
        {
            get
            {
                var retval = _sci.UseLinearReferenceFrameA;
                return retval;
            }
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
                var retval = _sci.RigidBodyA;
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
                var retval = _sci.RigidBodyB;
                return (RigidBodyImp)retval.UserObject;
            }
        }
        /// <summary>
        /// Gets the uid.
        /// </summary>
        /// <returns></returns>
        public int GetUid()
        {
            var retval = _sci.Uid;
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
