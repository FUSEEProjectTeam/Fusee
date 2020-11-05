using Fusee.Math.Core;
using Fusee.Structures;

namespace Fusee.PointCloud.OoCReaderWriter
{
    /// <summary>
    /// The cell of a <see cref="PtGrid{TPoint}"/>.
    /// </summary>
    /// <typeparam name="O">The type of the point that occupies this cell.</typeparam>
    public class PtGridCell<O> : IBucket<double3, double>
    {
        /// <summary>
        /// The point that occupies this cell.
        /// </summary>
        public O Occupant;

        /// <summary>
        /// Creates a new instance of type GridCell.
        /// </summary>
        /// <param name="center">The center of the cell.</param>
        /// <param name="size">The size of the cell.</param>
        public PtGridCell(double3 center, double size)
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
        public double Size { get; }
    }
}