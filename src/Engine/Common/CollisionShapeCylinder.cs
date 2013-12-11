using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class CollisionShapeCylinder : CollisionShape
    {
        public CollisionShapeCylinder(float3 halfExtents, int upAxis)
        {
            HalfExtents = halfExtents;
        }

        public CollisionShapeCylinder(float halfExtentsX, float halfExtentsY, float halfExtentsZ, int upAxis)
        {
            HalfExtents = new float3(halfExtentsX, halfExtentsY, halfExtentsZ);
        }

        public CollisionShapeCylinder(float halfExtents, int upAxis)
        {
            HalfExtents = new float3(halfExtents, halfExtents, halfExtents);
        }

        public float3 HalfExtents { get; private set; }

    }
}
