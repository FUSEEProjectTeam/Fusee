using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface ISliderConstraintImp : IConstraintImp
    {
        float3 AnchorInA { get; }
        float3 AnchorInB { get; }
        float AngularDepth { get; }
        float AngularPos { get; }

        void CalculateTransforms(float4x4 transA, float4x4 transB);
        float4x4 CalculatedTransformA { get; }
        float4x4 CalculatedTransformB { get; }

        float DampingDirAngular { get; set; }
        float DampingDirLin { get; set; }
        float DampingLimAngular { get; set; }
        float DampingLimLin { get; set; }
        float DampingOrthoAngular { get; set; }
        float DampingOrthoLin { get; set; }

        float4x4 FrameOffsetA { get; }
        float4x4 FrameOffsetB { get; }

        float LinDepth { get; }
        float LinPos { get; }

        float LowerAngularLimit { get; set; }
        float LowerLinLimit { get; set;}

        float MaxAngularMotorForce { get; set; }
        float MaxLinMotorForce { get; set; }

        bool PoweredAngularMotor { get; set; }
        bool PoweredLinMotor { get; set; }

        float RestitutionDirAngular { get; set; }
        float RestitutionDirLin { get; set; }
        float RestitutionLimAngular { get; set; }
        float RestitutionLimLin { get; set; }
        float RestitutionOrthoAngular { get; set; }
        float RestitutionOrthoLin { get; set; }

        void SetFrames(float4x4 frameA, float4x4 frameB);

        float SoftnessDirAngular { get; set; }
        float SoftnessDirLin { get; set; }
        float SoftnessLimAngular { get; set; }
        float SoftnessLimLin { get; set; }
        float SoftnessOrthoAngular { get; set; }
        float SoftnessOrthoLin { get; set; }

        bool SolveAngularLimit { get;}
        bool SolveLinLimit { get; }

        float TargetAngularMotorVelocity { get; set; }
        float TargetLinMotorVelocity { get; set; }

        void TestAngularLimits();
        void TestLinLimits();

        float UpperAngularLimit { get; set; }
        float UpperLinLimit { get; set; }

        bool UseFrameOffset { get; set; }

        bool UseLinearReferenceFrameA { get; }

    }
}
