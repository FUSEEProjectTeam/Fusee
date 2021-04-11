using Fusee.Math.Core;

namespace Fusee.Structures
{
    /// <summary>
    /// The cell of a <see cref="GridF{P}"/>.
    /// </summary>
    /// <typeparam name="O">The type of the payload.</typeparam>
    public class GridCellF<O> : IBucket<float3, float3>
    {
        /// <summary>
        /// The payload.
        /// </summary>
        public O Payload { get; set; }

        /// <summary>
        /// Creates a new instance of type GridCell.
        /// </summary>
        /// <param name="center">The center of the cell.</param>
        /// <param name="size">The size of the cell.</param>
        public GridCellF(float3 center, float3 size)
        {
            Center = center;
            Size = size;
        }

        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public float3 Center { get; }

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public float3 Size { get; }
    }
}