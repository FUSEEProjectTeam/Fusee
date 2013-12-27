using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class CollisionShape
    {
        internal ICollisionShapeImp ICollisionShapeImp;


        public virtual float Margin { get; set; }
    }
}
