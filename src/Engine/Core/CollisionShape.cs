using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class CollisionShape
    {
        internal ICollisionShapeImp ICollisionShapeImp;


        public float Margin
        {
            get { return ICollisionShapeImp.Margin; }
            set
            {
                var o = (CollisionShape)ICollisionShapeImp.UserObject;
                o.ICollisionShapeImp.Margin = value; 
            }
        }
    }
}
