using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface IConvexHullShapeImp : ICollisionShapeImp
    {
        void AddPoint(float3 point);
        float3 GetScaledPoint(int index);
        float3[] GetUnscaledPoints();
        int GetNumPoints();
        float3 LocalScaling { get; set; }
    }
}
