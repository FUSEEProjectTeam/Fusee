using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface IBoxShapeImp : ICollisionShapeImp
    {
        float3 LocalScaling { get; set; }
        float3 HalfExtents { get; }
    }
}
