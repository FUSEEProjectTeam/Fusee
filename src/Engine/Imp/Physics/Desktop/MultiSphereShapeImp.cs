using BulletSharp;
using Fusee.Engine.Imp.Physics.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IMultiSphereShapeImp" /> interface using the bullet physics engine.
    /// </summary>
    public class MultiSphereShapeImp : CollisonShapeImp, IMultiSphereShapeImp
    {
        internal MultiSphereShape BtMultiSphereShape;

        /// <summary>
        /// Gets and sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public override float Margin
        {
            get
            {
                var retval = BtMultiSphereShape.Margin;
                return retval;
            }
            set
            {
                var o = (MultiSphereShapeImp)BtMultiSphereShape.UserObject;
                o.BtMultiSphereShape.Margin = value;
            }
        }


        /// <summary>
        /// Gets the sphere position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public float3 GetSpherePosition(int index)
        {
            var retval = new float3(BtMultiSphereShape.GetSpherePosition(index).X, BtMultiSphereShape.GetSpherePosition(index).Y, BtMultiSphereShape.GetSpherePosition(index).Z);
            return retval;
        }

        /// <summary>
        /// Gets the sphere radius.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public float GetSphereRadius(int index)
        {
            var retval = BtMultiSphereShape.GetSphereRadius(index);
            return retval;
        }

        /// <summary>
        /// Gets the sphere count.
        /// </summary>
        /// <value>
        /// The sphere count.
        /// </value>
        public int SphereCount
        {
            get
            {
                var retval = BtMultiSphereShape.SphereCount;
                return retval;
            }
        }
    }
}
