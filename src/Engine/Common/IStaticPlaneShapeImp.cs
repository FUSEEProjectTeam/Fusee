using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface IStaticPlaneShapeImp : ICollisionShapeImp
    {
        float PlaneConstant { get; }
        float3 PlaneNormal { get; }
    }
}
