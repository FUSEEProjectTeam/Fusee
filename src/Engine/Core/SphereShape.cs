using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// implements a collision shape in form of a sphere.
    /// </summary>
    public class SphereShape : CollisionShape
    {
        internal ISphereShapeImp _sphereShapeImp;
        /// <summary>
        /// Gets and sets the margin of the sphere shape.
        /// </summary>
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
        /// <summary>
        /// Gats and sets the local scaling of the sphere shape.
        /// </summary>
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
        /// <summary>
        /// Gets and sets the radius of the sphere shape.
        /// </summary>
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
