using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class CollisionShape
    {
        internal ICollisionShapeImp ICollisionShapeImp;
        

        public virtual float Margin
        {
            get { return ICollisionShapeImp.Margin; }
            set
            {
                var o = (CollisionShape)ICollisionShapeImp.UserObject;
                o.ICollisionShapeImp.Margin = value; 
            }
        }

        public virtual float3 LocalScaling
        {
            get { return ICollisionShapeImp.LocalScaling; }
            set
            {
                var o = (CollisionShape)ICollisionShapeImp.UserObject;
                o.ICollisionShapeImp.LocalScaling = value;
            }
        }
    }
}
