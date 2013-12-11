using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class CollisionShapeCone : CollisionShape
    {
        public CollisionShapeCone(float radius, float height, int upAxis)
        {
            Radius = radius;
            Height = height;
            UpAxis = upAxis;
        }

        public float Radius { get; private set; }

        public float Height { get; private set; }

        public int UpAxis { get; private set; }
    }
}
