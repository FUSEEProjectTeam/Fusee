using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class BoxShape : CollisionShape
    {
        internal IBoxShapeImp BoxShapeImp;

        public float3 HalfExtents
        {
            get
            {
                var retval = BoxShapeImp.HalfExtents;
                return retval;
            }
        }

        //Inherited
        public override float Margin
        {

            get
            {
                var retval = BoxShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (BoxShape)BoxShapeImp.UserObject;
                o.BoxShapeImp.Margin = value;
            }
        }
    }
}
