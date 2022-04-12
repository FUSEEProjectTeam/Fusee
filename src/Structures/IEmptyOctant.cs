namespace Fusee.Structures
{
    /// <summary>
    /// Interface for implementing an octant (1/8 of a cube and node in an Octree).
    /// </summary>
    /// <typeparam name="T">Type of the center point of this octant.</typeparam>
    /// <typeparam name="K">Type of the size of this octant.</typeparam>
    public interface IEmptyOctant<T, K> : IBucket<T, K>
    {
        /// <summary>
        /// Children of this Octant. Must contain eight or null (leaf node) children.
        /// </summary>
        public IEmptyOctant<T, K>[] Children { get; }


        /// <summary>
        /// Is this octant a leaf node in the octree?
        /// </summary>
        public bool IsLeaf { get; set; }

        /// <summary>
        /// Integer that defines this octants position (1 to 8) in its parent.
        /// </summary>
        public int PosInParent { get; }

        /// <summary>
        /// The level of the octree this octant belongs to.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Instantiates a child octant at the given position.
        /// </summary>
        /// <param name="atPosInParent">The <see cref="PosInParent"/> the new child has.</param>
        public IEmptyOctant<T, K> CreateChild(int atPosInParent);
    }
}