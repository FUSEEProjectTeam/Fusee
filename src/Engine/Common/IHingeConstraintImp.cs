using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public enum HingeFlags
    {
        HingeFlagsStopCfm = 1,
        HingeFlagsStopErp = 2,
        HingeFlagsNormCfm = 4
    };

    public interface IHingeConstraintImp : IConstraintImp
    {

        bool AngularOnly { get; set; }
        bool EnableMotor { get; set; }
        void EnableAngularMotor(bool enableMotor, float targetVelocity, float maxMotorImpulse);

        void SetMotorTarget(Quaternion qAinB, float dt);
        void SetMotorTarget(float targetAngle, float dt);

        float MaxMotorImpulse { get; set; }
        float MotorTargetVelocity { get; }

        float4x4 FrameA { get; }
        float4x4 FrameB { get; }
        float4x4 FrameOffsetA { get; }
        float4x4 FrameOffsetB { get; }

        void SetAxis(float3 axisInA);

        float GetHingeAngle();
        float GetHingeAngle(float4x4 transA, float4x4 transB);

        void SetLimit(float low, float high, float softness = 0.9f, float biasFactor=0.3f, float relaxationFactor=1.0f);
        int SolverLimit { get; }
        float LowerLimit { get; }
        float UpperLimit { get; }
    }
}
