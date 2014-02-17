using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface IGeneric6DofConstraintImp : IConstraintImp
    {
        float3 AngularLowerLimit { get; set; }
        float3 AngularUpperLimit { get; set; }

        void CalcAnchorPos();

        void CalculateTransforms();
        void CalculateTransforms(float4x4 transA, float4x4 transB);
        float4x4 CalculatedTransformA { get; }
        float4x4 CalculatedTransformB { get; }

        float4x4 FrameOffsetA { get; set; }
        float4x4 FrameOffsetB { get; set; }

        float GetAngle(int axisIndex);
        float3 GetAxis(int axisIndex);

        float GetRelativePivotPosition(int axisIndex);
        //Todo: RotationalLimitMotor

        bool IsLimited(int limitIndex);
        float3 LinearLowerLimit { get; set; }
        float3 LinearUpperLimit { get; set; }

        void SetAxis(float3 axis1, float3 axis2);
        void SetFrames(float4x4 frameA, float4x4 frameB);
        void SetLimit(int axis, float lo, float hi);

        bool TestAngularLimitMotor(int axisIndex);
        //Todo: TranslationalLimitMotor

        bool UseFrameOffset { get; set; }

        void UpdateRhs(float timeStep);

    }
}
