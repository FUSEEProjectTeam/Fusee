using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using Fusee.Math;

namespace Fusee.Engine
{
    public class SliderConstraintImp : ISliderConstraintImp
    {
        internal SliderConstraint _sci;
        internal Translater Translater = new Translater();

        public float3 AnchorInA
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(_sci.AnchorInA);
                return retval;
            }
        }
        public float3 AnchorInB
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(_sci.AnchorInB);
                return retval;
            }
        }

        public float AngularDepth
        {
            get
            {
                var retval = _sci.AngularDepth;
                return retval;
            }
        }
        public float AngularPos
        {
            get
            {
                var retval = _sci.AngularPos;
                return retval;
            }
        }

        public void CalculateTransforms(float4x4 transA, float4x4 transB)
        {
            _sci.CalculateTransforms(Translater.Float4X4ToBtMatrix(transA), Translater.Float4X4ToBtMatrix(transB));
        }
        public float4x4 CalculatedTransformA
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_sci.CalculatedTransformA);
                return retval;
            }
        }
        public float4x4 CalculatedTransformB
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_sci.CalculatedTransformB);
                return retval;
            }
        }

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

        public float4x4 FrameOffsetA
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_sci.FrameOffsetA);
                return retval;
            }
        }
        public float4x4 FrameOffsetB
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_sci.FrameOffsetB);
                return retval;
            }
        }

        public float LinDepth
        {
            get
            {
                var retval = _sci.LinDepth;
                return retval;
            }
        }
        public float LinPos
        {
            get
            {
                var retval = _sci.LinearPos;
                return retval;
            }
        }

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

        public void SetFrames(float4x4 frameA, float4x4 frameB)
        {
            var o = (SliderConstraintImp) _sci.UserObject;
            o._sci.SetFrames(Translater.Float4X4ToBtMatrix(frameA), Translater.Float4X4ToBtMatrix(frameB));
        }

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

        public bool SolveAngularLimit
        {
            get
            {
                var retval = _sci.SolveAngularLimit;
                return retval;
            }  
        }
        public bool SolveLinLimit
        {
            get
            {
                var retval = _sci.SolveLinLimit;
                return retval;
            }
        }

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

        public void TestAngularLimits()
        {
            _sci.TestAngularLimits();
        }
        public void TestLinLimits()
        {
            _sci.TestLinLimits();
        }

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
        
        public bool UseLinearReferenceFrameA
        {
            get
            {
                var retval = _sci.UseLinearReferenceFrameA;
                return retval;
            }
        }

        #region IConstraintImp
        public IRigidBodyImp RigidBodyA
        {
            get
            {
                var retval = _sci.RigidBodyA;
                return (RigidBodyImp)retval.UserObject;
            }
        }
        public IRigidBodyImp RigidBodyB
        {
            get
            {
                var retval = _sci.RigidBodyB;
                return (RigidBodyImp)retval.UserObject;
            }
        }
        public int GetUid()
        {
            var retval = _sci.Uid;
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
