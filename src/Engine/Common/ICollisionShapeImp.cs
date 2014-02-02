using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public interface ICollisionShapeImp
    {
        float Margin { get; set; }
        object UserObject { get; set; }
    }
}
