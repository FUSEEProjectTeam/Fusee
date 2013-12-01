using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface IHingeConstraintImp : IConstraintImp
    {

        
        float4x4 FrameA { get; }
        float4x4 FrameB { get; }
        void SetLimit(float low, float high, float softness = 0.9f, float biasFactor=0.3f, float relaxationFactor=1.0f);
        
    }
}
