using Fusee.Math.Core;
using Fusee.Structures;

namespace Fusee.Structures
{
    /// <summary>
    /// The cell of a <see cref="GridD{P}"/>.
    /// </summary>
    /// <typeparam name="O">The type of the point that occupies this cell.</typeparam>
    public class GridCellD<O> : IBucket<double3, double3>
    {
        /// <summary>
        /// The point that occupies this cell.
        /// </summary>
        public O Payload { get; set; }

        /// <summary>
        /// Creates a new instance of type GridCell.
        /// </summary>
        /// <param name="center">The center of the cell.</param>
        /// <param name="size">The size of the cell.</param>
        public GridCellD(double3 center, double3 size)
        {
            Center = center;
            Size = size;
        }

        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public double3 Center { get; }

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public double3 Size { get; }
    }
}