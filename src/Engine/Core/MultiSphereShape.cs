using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Implements a multi sphere collision shape.
    /// </summary>
    public class MultiSphereShape : CollisionShape
    {
        internal IMultiSphereShapeImp _multiSphereShapeImp;

        /// <summary>
        /// Gets or sets the margin of the multi sphere collision shape.
        /// </summary>
        public override float Margin
        {
            get
            {
                var retval = _multiSphereShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (CapsuleShape)_multiSphereShapeImp.UserObject;
                o._capsuleShapeImp.Margin = value;
            }
        }
        /// <summary>
        /// Returns the spheres position.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float3 GetSpherePosition(int index)
        {
            var retval = _multiSphereShapeImp.GetSpherePosition(index);
            return retval;
        }
        /// <summary>
        /// Returns the spheres radius. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetSphereRadius(int index)
        {
            var retval = _multiSphereShapeImp.GetSphereRadius(index);
            return retval;
        }
        /// <summary>
        /// Returns the number of spheres.
        /// </summary>
        public int SphereCount
        {
            get
            {
                var retval = _multiSphereShapeImp.SphereCount;
                return retval;
            }
        }
    }
}