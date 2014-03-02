using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface ICylinderShapeImp : ICollisionShapeImp
    {
        float3 HalfExtents { get; }
        float Radius { get; }
        int UpAxis { get; }
    }
}
