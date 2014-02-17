using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface IConeTwistConstraintImp : IConstraintImp
    {
        float4x4 AFrame { get; }
        float4x4 BFrame { get; }

        void CalcAngleInfo();
        void CalcAngleInfo2(float4x4 transA, float4x4 transB, float4x4 invInertiaWorldA, float4x4 invInertiaWorldB);

        void EnableMotor(bool b);

        float FixThresh { get; set; }

        float4x4 FrameOffsetA { get; }
        float4x4 FrameOffsetB { get; }

        float3 GetPointForAngle(float fAngleInRadius, float fLength);
        
        bool IsPastSwingLimit { get; }

        void SetAngularOnly(bool angularOnly);
        void SetDamping(float damping);
        void SetLimit(int limitIndex, float limitValue);
        void SetLimit(float swingSpan1, float swingSpan2, float twistSpan, float softness, float biasFactor, float relaxationFactor);

        void SetMaxMotorImpulse(float maxMotorImpulse);
        void SetMaxMotorImpulseNormalized(float maxMotorImpulse);

        void SetMotorTarget(Quaternion q);
        void SetMotorTargetInConstraintSpace(Quaternion q);

        int SolveSwingLimit { get; }
        int SolveTwistLimit { get; }

        float SwingSpan1 { get; }
        float SwingSpan2 { get; }

        float TwistAngle { get; }
        float TwistLimitSign { get; }
        float TwistSpan { get; }

        void UpdateRhs(float timeStep);
    }
}
