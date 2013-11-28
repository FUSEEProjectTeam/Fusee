using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface IHingeConstraintImp
    {

        IRigidBodyImp RigidBodyA { get; }
        IRigidBodyImp RigidBodyB { get; }
        float4x4 FrameA { get; }
        float4x4 FrameB { get; }

        object UserObject { get; set; }
    }
}
