using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Core
{
    /// <summary>
    /// Implements the slider constraint
    /// </summary>
    public class SliderConstraint
    {
        internal ISliderConstraintImp _iSliderConstraintImp;
        /// <summary>
        /// Returns the anchor in a.
        /// </summary>
        public float3 AnchorInA
        {
            get
            {
                var retval = _iSliderConstraintImp.AnchorInA;
                return retval;
            }
        }
        /// <summary>
        /// Returns the anchor in b.
        /// </summary>
        public float3 AnchorInB
        {
            get
            {
                var retval = _iSliderConstraintImp.AnchorInB;
                return retval;
            }
        }
        /// <summary>
        /// Returns the angular depth.
        /// </summary>
        public float AngularDepth
        {
            get
            {
                var retval = _iSliderConstraintImp.AngularDepth;
                return retval;
            }
        } 
        /// <summary>
        /// Returns the angular position.
        /// </summary>
        public float AngularPos
        {
            get
            {
                var retval = _iSliderConstraintImp.AngularPos;
                return retval;
            }
        }
        /// <summary>
        /// Calcualtes the transform for A and B.
        /// </summary>
        /// <param name="transA"></param>
        /// <param name="transB"></param>
        public void CalculateTransforms(float4x4 transA, float4x4 transB)
        {
            _iSliderConstraintImp.CalculateTransforms(transA, transB);
        } 
        /// <summary>
        /// Returns the calcualted transforms of a.
        /// </summary>
        public float4x4 CalculatedTransformA
        {
            get
            {
                var retval = _iSliderConstraintImp.CalculatedTransformA;
                return retval;
            }
        }
        /// <summary>
        /// Returns the calculated transforms of b.
        /// </summary>
        public float4x4 CalculatedTransformB
        {
            get
            {
                var retval = _iSliderConstraintImp.CalculatedTransformB;
                return retval;
            }
        }
        /// <summary>
        /// Gets and sets the angular dampening direction.
        /// </summary>
        public float DampingDirAngular
        {
            get
            {
                var retval = _iSliderConstraintImp.DampingDirAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.DampingDirAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the linear dampening direction.
        /// </summary>
        public float DampingDirLin
        {
            get
            {
                var retval = _iSliderConstraintImp.DampingDirLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.DampingDirLin = value;
            }
        }
        /// <summary>
        /// Gets and sets the angular dampening lim.
        /// </summary>
        public float DampingLimAngular
        {
            get
            {
                var retval = _iSliderConstraintImp.DampingLimAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.DampingLimAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the linear dampening lim.
        /// </summary>
        public float DampingLimLin
        {
            get
            {
                var retval = _iSliderConstraintImp.DampingLimLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.DampingLimLin = value;
            }
        }
        /// <summary>
        /// Gets and sets the angular dampening orthogonals.
        /// </summary>
        public float DampingOrthoAngular
        {
            get
            {
                var retval = _iSliderConstraintImp.DampingOrthoAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.DampingOrthoAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the linear dampening orthogonals.
        /// </summary>
        public float DampingOrthoLin
        {
            get
            {
                var retval = _iSliderConstraintImp.DampingOrthoLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.DampingOrthoLin = value;
            }
        }
        /// <summary>
        /// Returns the frame offset for a.
        /// </summary>
        public float4x4 FrameOffsetA
        {
            get
            {
                var retval = _iSliderConstraintImp.FrameOffsetA;
                return retval;
            }
        }
        /// <summary>
        /// Returns the frame offset for b.
        /// </summary>
        public float4x4 FrameOffsetB
        {
            get
            {
                var retval = _iSliderConstraintImp.FrameOffsetB;
                return retval;
            }
        }
        /// <summary>
        /// Returns the linear depth.
        /// </summary>
        public float LinDepth
        {
            get
            {
                var retval = _iSliderConstraintImp.LinDepth;
                return retval;
            }
        }
        /// <summary>
        /// Return the linear position.
        /// </summary>
        public float LinPos
        {
            get
            {
                var retval = _iSliderConstraintImp.LinPos;
                return retval;
            }
        }
        /// <summary>
        /// Gets and sets the lower angular limit.
        /// </summary>
        public float LowerAngularLimit
        {
            get
            {
                var retval = _iSliderConstraintImp.LowerAngularLimit;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.LowerAngularLimit = value;
            }
        }
        /// <summary>
        /// Gets and sets the lower linear limit.
        /// </summary>
        public float LowerLinLimit
        {
            get
            {
                var retval = _iSliderConstraintImp.LowerLinLimit;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.LowerLinLimit = value;
            }
        }
        /// <summary>
        /// Gets and sets the maximum angular motor force.
        /// </summary>
        public float MaxAngularMotorForce
        {
            get
            {
                var retval = _iSliderConstraintImp.MaxAngularMotorForce;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.MaxAngularMotorForce = value;
            }
        }
        /// <summary>
        /// Gets and sets the maximum linear motor force.
        /// </summary>
        public float MaxLinMotorForce
        {
            get
            {
                var retval = _iSliderConstraintImp.MaxLinMotorForce;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.MaxLinMotorForce = value;
            }
        }
        /// <summary>
        /// Gets and sets the powered angular motor.
        /// </summary>
        public bool PoweredAngularMotor
        {
            get
            {
                var retval = _iSliderConstraintImp.PoweredAngularMotor;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.PoweredAngularMotor = value;
            }
        }
        /// <summary>
        /// Gets and sets the linear powered motor.
        /// </summary>
        public bool PoweredLinMotor
        {
            get
            {
                var retval = _iSliderConstraintImp.PoweredLinMotor;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.PoweredLinMotor = value;
            }
        }
        /// <summary>
        /// Gets and sets the  angular restitution direction.
        /// </summary>
        public float RestitutionDirAngular
        {
            get
            {
                var retval = _iSliderConstraintImp.RestitutionDirAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.RestitutionDirAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the linear restitution direction.
        /// </summary>
        public float RestitutionDirLin
        {
            get
            {
                var retval = _iSliderConstraintImp.RestitutionDirLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.RestitutionDirLin = value;
            }
        }
        /// <summary>
        /// Gets and sets the angular restitution lim.
        /// </summary>
        public float RestitutionLimAngular
        {
            get
            {
                var retval = _iSliderConstraintImp.RestitutionLimAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.RestitutionLimAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the linear restitution lim.
        /// </summary>
        public float RestitutionLimLin
        {
            get
            {
                var retval = _iSliderConstraintImp.RestitutionLimLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.RestitutionLimLin = value;
            }
        }
        /// <summary>
        /// Gets and sets the angular restitution orthogonal.
        /// </summary>
        public float RestitutionOrthoAngular
        {
            get
            {
                var retval = _iSliderConstraintImp.RestitutionOrthoAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.RestitutionOrthoAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the linear restitution orthogonal.
        /// </summary>
        public float RestitutionOrthoLin
        {
            get
            {
                var retval = _iSliderConstraintImp.RestitutionOrthoLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.RestitutionOrthoLin = value;
            }
        }
        /// <summary>
        /// Sets the frames.
        /// </summary>
        /// <param name="frameA"></param>
        /// <param name="frameB"></param>
        public void SetFrames(float4x4 frameA, float4x4 frameB)
        {
            var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
            o._iSliderConstraintImp.SetFrames(frameA, frameB);
        }
        /// <summary>
        /// Gets and sets the angular softness direction.
        /// </summary>
        public float SoftnessDirAngular
        {
            get
            {
                var retval = _iSliderConstraintImp.SoftnessDirAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.SoftnessDirAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the linear softness direction.
        /// </summary>
        public float SoftnessDirLin
        {
            get
            {
                var retval = _iSliderConstraintImp.SoftnessDirLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.SoftnessDirLin = value;
            }
        }
        /// <summary>
        /// Gets and sets the angular softness lim.
        /// </summary>
        public float SoftnessLimAngular
        {
            get
            {
                var retval = _iSliderConstraintImp.SoftnessLimAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.SoftnessLimAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the linear softness lim.
        /// </summary>
        public float SoftnessLimLin
        {
            get
            {
                var retval = _iSliderConstraintImp.SoftnessLimLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.SoftnessLimLin = value;
            }
        }
        /// <summary>
        /// Gets and sets the angular softness orthogonal.
        /// </summary>
        public float SoftnessOrthoAngular
        {
            get
            {
                var retval = _iSliderConstraintImp.SoftnessOrthoAngular;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.SoftnessOrthoAngular = value;
            }
        }
        /// <summary>
        /// Gets and sets the linear softness orthogonal.
        /// </summary>
        public float SoftnessOrthoLin
        {
            get
            {
                var retval = _iSliderConstraintImp.SoftnessOrthoLin;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.SoftnessOrthoLin = value;
            }
        }
        /// <summary>
        /// Returns the solved angular limit.
        /// </summary>
        public bool SolveAngularLimit
        {
            get
            {
                var retval = _iSliderConstraintImp.SolveAngularLimit;
                return retval;
            }
        }
        /// <summary>
        /// Returns the solved linear limit.
        /// </summary>
        public bool SolveLinLimit
        {
            get
            {
                var retval = _iSliderConstraintImp.SolveLinLimit;
                return retval;
            }
        }
        /// <summary>
        /// Gets and sets the target angular motor velocity.
        /// </summary>
        public float TargetAngularMotorVelocity
        {
            get
            {
                var retval = _iSliderConstraintImp.TargetAngularMotorVelocity;
                return retval;
            }
            set
            {
                var o = (SliderConstraint) _iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.TargetAngularMotorVelocity = value;
            }
        }
        /// <summary>
        /// Gets and sets the target linear motor velocity.
        /// </summary>
        public float TargetLinMotorVelocity
        {
            get
            {
                var retval = _iSliderConstraintImp.TargetLinMotorVelocity;
                return retval;
            }
            set
            {
                var o = (SliderConstraint) _iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.TargetLinMotorVelocity = value;
            }
        }
        /// <summary>
        /// Tests the angular limits.
        /// </summary>
        public void TestAngularLimits()
        {
            _iSliderConstraintImp.TestAngularLimits();
        }
        /// <summary>
        /// Tests the linear limits.
        /// </summary>
        public void TestLinLimits()
        {
            _iSliderConstraintImp.TestLinLimits();
        }
        /// <summary>
        /// Gets and sets the upper angular limit.
        /// </summary>
        public float UpperAngularLimit
        {
            get
            {
                var retval = _iSliderConstraintImp.UpperAngularLimit;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.UpperAngularLimit = value;
            }
        }
        /// <summary>
        /// Gets and sets the upper linear limit.
        /// </summary>
        public float UpperLinLimit
        {
            get
            {
                var retval = _iSliderConstraintImp.UpperLinLimit;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.UpperLinLimit = value;
            }
        }
        /// <summary>
        /// Gets and sets the use of frame offset.
        /// </summary>
        public bool UseFrameOffset
        {
            get
            {
                var retval = _iSliderConstraintImp.UseFrameOffset;
                return retval;
            }
            set
            {
                var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
                o._iSliderConstraintImp.UseFrameOffset = value;
            }
        }
        /// <summary>
        /// Gets and sets the use of the linear reference frame a.
        /// </summary>
        public bool UseLinearReferenceFrameA
        {
            get
            {
                var retval = _iSliderConstraintImp.UseLinearReferenceFrameA;
                return retval;
            }
        }
        #region IConstraintImp
        /// <summary>
        /// Returns the rigid body a.
        /// </summary>
        public RigidBody RigidBodyA
        {
            get
            {
                var retval = _iSliderConstraintImp.RigidBodyA.UserObject;
                return (RigidBody)retval;
            }
        }
        /// <summary>
        /// Returns the rigid body b.
        /// </summary>
        public RigidBody RigidBodyB
        {
            get
            {
                var retval = _iSliderConstraintImp.RigidBodyB.UserObject;
                return (RigidBody)retval;
            }
        }
        /// <summary>
        /// Returns the Uid.
        /// </summary>
        /// <returns></returns>
        public int GetUid()
        {
            var retval = _iSliderConstraintImp.GetUid();
            return retval;
        }
        #endregion IConstraintImp
    }
}
