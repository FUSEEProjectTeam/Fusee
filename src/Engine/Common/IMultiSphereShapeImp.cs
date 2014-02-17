using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface IMultiSphereShapeImp : ICollisionShapeImp
    {
        float3 GetSpherePosition(int index);
        float GetSphereRadius(int index);
        int SphereCount { get; }
    }
}
