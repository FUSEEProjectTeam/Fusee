using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class SphereShape : CollisionShape
    {
        internal ISphereShapeImp SphereShapeImp;

        public float Margin
        {
            get
            {
                var retval = SphereShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (SphereShape)SphereShapeImp.UserObject;
                o.SphereShapeImp.Margin = value;
            }
        }
        public float3 LocalScaling
        {
            get
            {
                var retval = SphereShapeImp.LocalScaling;
                return retval;
            }
            set
            {
                var o = (SphereShape)SphereShapeImp.UserObject;
                o.SphereShapeImp.LocalScaling = value;
            }
        }
        public float Radius
        {
            get
            {
                var retval = SphereShapeImp.Radius;
                return retval;
            }
            set
            {
                var o = (SphereShape) SphereShapeImp.UserObject;
                o.SphereShapeImp.Radius = value;
            }
        }
        
    }
}
