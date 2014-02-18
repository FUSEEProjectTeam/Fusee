using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface ICollisionShapeImp
    {
        float Margin { get; set; }
        float3 LocalScaling { get; set; }
        object UserObject { get; set; }
    }
}
