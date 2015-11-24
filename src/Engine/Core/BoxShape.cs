using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class BoxShape : CollisionShape
    {
        internal IBoxShapeImp BoxShapeImp;

        public float3 LocalScaling
        {
            get { return BoxShapeImp.LocalScaling; }
            set
            {
                var o = (BoxShape)BoxShapeImp.UserObject;
                o.BoxShapeImp.LocalScaling = value;
            }
        }
        public float3 HalfExtents
        {
            get
            {
                var retval = BoxShapeImp.HalfExtents;
                return retval;
            }
        }

        //Inherited
        public float Margin
        {

            get
            {
                var retval = BoxShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (BoxShape) BoxShapeImp.UserObject;
                o.BoxShapeImp.Margin = value;
            }
        }
    }
}
