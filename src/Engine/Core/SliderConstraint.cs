using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class SliderConstraint
    {
        internal ISliderConstraintImp _iSliderConstraintImp;

        public float3 AnchorInA
        {
            get
            {
                var retval = _iSliderConstraintImp.AnchorInA;
                return retval;
            }
        }
        public float3 AnchorInB
        {
            get
            {
                var retval = _iSliderConstraintImp.AnchorInB;
                return retval;
            }
        }

        public float AngularDepth
        {
            get
            {
                var retval = _iSliderConstraintImp.AngularDepth;
                return retval;
            }
        }
        public float AngularPos
        {
            get
            {
                var retval = _iSliderConstraintImp.AngularPos;
                return retval;
            }
        }

        public void CalculateTransforms(float4x4 transA, float4x4 transB)
        {
            _iSliderConstraintImp.CalculateTransforms(transA, transB);
        }
        public float4x4 CalculatedTransformA
        {
            get
            {
                var retval = _iSliderConstraintImp.CalculatedTransformA;
                return retval;
            }
        }
        public float4x4 CalculatedTransformB
        {
            get
            {
                var retval = _iSliderConstraintImp.CalculatedTransformB;
                return retval;
            }
        }

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

        public float4x4 FrameOffsetA
        {
            get
            {
                var retval = _iSliderConstraintImp.FrameOffsetA;
                return retval;
            }
        }
        public float4x4 FrameOffsetB
        {
            get
            {
                var retval = _iSliderConstraintImp.FrameOffsetB;
                return retval;
            }
        }

        public float LinDepth
        {
            get
            {
                var retval = _iSliderConstraintImp.LinDepth;
                return retval;
            }
        }
        public float LinPos
        {
            get
            {
                var retval = _iSliderConstraintImp.LinPos;
                return retval;
            }
        }

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

        public void SetFrames(float4x4 frameA, float4x4 frameB)
        {
            var o = (SliderConstraint)_iSliderConstraintImp.UserObject;
            o._iSliderConstraintImp.SetFrames(frameA, frameB);
        }

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

        public bool SolveAngularLimit
        {
            get
            {
                var retval = _iSliderConstraintImp.SolveAngularLimit;
                return retval;
            }
        }
        public bool SolveLinLimit
        {
            get
            {
                var retval = _iSliderConstraintImp.SolveLinLimit;
                return retval;
            }
        }

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

        public void TestAngularLimits()
        {
            _iSliderConstraintImp.TestAngularLimits();
        }
        public void TestLinLimits()
        {
            _iSliderConstraintImp.TestLinLimits();
        }

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

        public bool UseLinearReferenceFrameA
        {
            get
            {
                var retval = _iSliderConstraintImp.UseLinearReferenceFrameA;
                return retval;
            }
        }
        #region IConstraintImp
        public RigidBody RigidBodyA
        {
            get
            {
                var retval = _iSliderConstraintImp.RigidBodyA.UserObject;
                return (RigidBody)retval;
            }
        }

        public RigidBody RigidBodyB
        {
            get
            {
                var retval = _iSliderConstraintImp.RigidBodyB.UserObject;
                return (RigidBody)retval;
            }
        }
        public int GetUid()
        {
            var retval = _iSliderConstraintImp.GetUid();
            return retval;
        }
        #endregion IConstraintImp
    }
}
