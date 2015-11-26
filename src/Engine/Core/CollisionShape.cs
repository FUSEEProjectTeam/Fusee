using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class CollisionShape
    {
        internal ICollisionShapeImp _collisionShapeImp;
        

        public virtual float Margin
        {
            get { return _collisionShapeImp.Margin; }
            set
            {
                var o = (CollisionShape)_collisionShapeImp.UserObject;
                o._collisionShapeImp.Margin = value; 
            }
        }

        public virtual float3 LocalScaling
        {
            get { return _collisionShapeImp.LocalScaling; }
            set
            {
                var o = (CollisionShape)_collisionShapeImp.UserObject;
                o._collisionShapeImp.LocalScaling = value;
            }
        }
    }
}
