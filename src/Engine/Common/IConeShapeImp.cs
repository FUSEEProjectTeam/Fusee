using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public interface IConeShapeImp : ICollisionShapeImp
    {
        int ConeUpIndex { get; set; }
        float Height { get; }
        float Radius { get; }
    }
}
