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
            var _boxHalfExtendsTemp = new float3(boxHalfExtends, boxHalfExtends, boxHalfExtends);
            _boxHalfExtends = _boxHalfExtendsTemp;
        }

        public CollisionShapeBox(float boxHalfExtendsX, float boxHalfExtendsY, float boxHalfExtendsZ)
        {
            var _boxHalfExtendsTemp = new float3(boxHalfExtendsX, boxHalfExtendsY, boxHalfExtendsZ);
            _boxHalfExtends = _boxHalfExtendsTemp;
        }

      

        internal float3 _boxHalfExtends;

        public float3 HalfExtents
        {
            get
            {
                return _boxHalfExtends;
            }
        }
    }
}
