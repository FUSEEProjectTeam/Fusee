using System.Collections.Generic;
using Fusee.Math.Core;

namespace Fusee.Structures.Octree
{
    /// <summary>
    /// Node for use in an octree. Note that this is not intended to be used in a scene graph!
    /// </summary>
    /// <typeparam name="T">Defines the type of the payload.</typeparam>
    public class PayloadOctantD<T> : OctantD
    {
        /// <summary>
        /// Children of this Octant. Must contain eight or null (leaf node) children.
        /// </summary>
        public PayloadOctantD<T>[] Children;

        /// <summary>
        /// The payload of this octant.
        /// </summary>
        public List<T> Payload;

        /// <summary>
        /// Creates a new instance of type Octant.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="children"></param>
        /// <param name="payload"></param>
        public PayloadOctantD(double3 center, double size, List<T> payload, PayloadOctantD<T>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new PayloadOctantD<T>[8];
            else
                Children = children;

            Payload = payload;
        }

        /// <summary>
        /// Create a new instance of type Octant.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="children"></param>        
        public PayloadOctantD(double3 center, double size, PayloadOctantD<T>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new PayloadOctantD<T>[8];
            else
                Children = children;
        }

        /// <summary>
        /// Create a new instance of type Octant.
        /// </summary>
        protected PayloadOctantD() { }
    }
}