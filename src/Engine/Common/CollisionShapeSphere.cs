using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class CollisionShapeSphere : CollisionShape
    {
        public CollisionShapeSphere(float radius)
        {
            _radius = radius;
        }

        internal float _radius;

        virtual public float GetRadius()
        {
            return _radius;
        }

        private void SetUnScaledRaduis(float radius)
        {

        }
    }
}
