using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class CollisionShapeMultiSphere : CollisionShape
    {
        public CollisionShapeMultiSphere(float3[] positions, float[] radi)
        {
            _positions = positions;
            _count = _positions.Count();
            _radi = radi;
        }

        private float3[] _positions;

        public float3 GetSpherePosition(int index)
        {
            return _positions[index];
        }

        private float[] _radi;

        public float GetSphereRadius(int index)
        {
            return _radi[index];
        }

        private int _count;
        public int SphereCount()
        {
            return _count;
        }
    }
}
