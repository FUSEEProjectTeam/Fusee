using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace Fusee.Engine
{
    public interface ICapsuleShapeImp : ICollisionShapeImp
    {
        float HalfHeight { get; }
        float Radius { get; }
        int UpAxis { get; }
    }
}
