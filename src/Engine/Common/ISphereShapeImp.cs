using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public interface ISphereShapeImp : ICollisionShapeImp
    {
        float Radius { get; set; }
    }
}
