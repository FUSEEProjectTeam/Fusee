using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class SphereShape : CollisionShape
    {
        internal ISphereShapeImp _sphereShapeImp;

        public override float Margin
        {
            get
            {
                var retval = _sphereShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (SphereShape)_sphereShapeImp.UserObject;
                o._sphereShapeImp.Margin = value;
            }
        }
        public override float3 LocalScaling
        {
            get
            {
                var retval = _sphereShapeImp.LocalScaling;
                return retval;
            }
            set
            {
                var o = (SphereShape)_sphereShapeImp.UserObject;
                o._sphereShapeImp.LocalScaling = value;
            }
        }
        public float Radius
        {
            get
            {
                var retval = _sphereShapeImp.Radius;
                return retval;
            }
            set
            {
                var o = (SphereShape) _sphereShapeImp.UserObject;
                o._sphereShapeImp.Radius = value;
            }
        }
        
    }
}
