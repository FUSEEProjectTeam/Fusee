using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class CollisionShapeCapsule : CollisionShape
    {
        public CollisionShapeCapsule(float radius, float height)
        {
            Radius = radius;
            Height = height;
        }

        public float Radius { get; private set; }

        public float Height { get; private set; }
    }
}
