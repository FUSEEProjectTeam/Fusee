using System.Collections.Generic;

namespace Fusee.Structures
{
    /// <summary>
    /// Interface for implementing an octant (1/8 of a cube and node in an Octree).
    /// </summary>
    /// <typeparam name="T">Type of the center point of this octant.</typeparam>
    /// <typeparam name="K">Type of the size of this octant.</typeparam>
    /// <typeparam name="P">Type of the payload of this octant.</typeparam>
    public interface IOctant<T, K, P> : IBucket<T, K>
    {
        /// <summary>
        /// Children of this Octant. Must contain eight or null (leaf node) children.
        /// </summary>
        public IOctant<T, K, P>[] Children { get; }

        /// <summary>
        /// The payload of this octant.
        /// </summary>
        public List<P> Payload { get; }

        /// <summary>
        /// Is this octant a leaf node in the octree?
        /// </summary>
        public bool IsLeaf { get; }

        /// <summary>
        /// Integer that defines this octants position in its parent.
        /// </summary>
        public int PosInParent { get; }

        /// <summary>
        /// The level of the octree this octant belongs to.
        /// </summary>
        public int Level { get; }
    }
}