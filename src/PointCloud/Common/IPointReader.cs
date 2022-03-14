using Fusee.PointCloud.Common.Accessors;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Implement this into any Point Cloud Reader.
    /// </summary>
    public interface IPointReader
    {
        /// <summary>
        /// A PointAccessor allows access to the point information (position, color, ect.) without casting it to a specific <see cref="PointType"/>.
        /// </summary>
        public IPointAccessor PointAccessor { get; }

        /// <summary>
        /// Returns a renderable point cloud component.
        /// </summary>
        /// <param name="fileFolderPath">Path to the file.</param>
        public IPointCloud GetPointCloudComponent(string fileFolderPath);

        /// <summary>
        /// Returns the point type.
        /// </summary>
        public PointType GetPointType();

        /// <summary>
        /// Reads the Potree file and returns an octree.
        /// </summary>
        public IPointCloudOctree GetOctree();

        /// <summary>
        /// Returns the points for one octant as generic array.
        /// </summary>
        /// <typeparam name="TPoint">The generic point type.</typeparam>
        /// <param name="id">The unique id of the octant.</param>
        public TPoint[] LoadNodeData<TPoint>(string id) where TPoint : new();

    }
}