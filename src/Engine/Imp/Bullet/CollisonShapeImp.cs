using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;

namespace Fusee.Engine
{
    public abstract class CollisonShapeImp : ICollisionShapeImp
    {
        public abstract float Margin { get; set; }

        public abstract  object UserObject { get; set; }
    }
}
