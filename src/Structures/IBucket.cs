namespace Fusee.Structures
{
    /// <summary>
    /// Interface for implementing a node for use in an Grid-like structure.
    /// </summary>
    public interface IBucket<T, K>
    {
        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public T Center { get; }

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public K Size { get; }
    }
}