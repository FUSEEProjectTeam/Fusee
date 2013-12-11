using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class CollisionShapeBox : CollisionShape
    {

        internal IRigidBodyImp _iRbImp;

        public CollisionShapeBox(float boxHalfExtends)
        {
            var boxHalfExtendsTemp = new float3(boxHalfExtends, boxHalfExtends, boxHalfExtends);
            HalfExtents = boxHalfExtendsTemp;
        }

        public CollisionShapeBox(float boxHalfExtendsX, float boxHalfExtendsY, float boxHalfExtendsZ)
        {
            var boxHalfExtendsTemp = new float3(boxHalfExtendsX, boxHalfExtendsY, boxHalfExtendsZ);
            HalfExtents = boxHalfExtendsTemp;
        }


        public float3 HalfExtents { get; private set; }
    }
}
