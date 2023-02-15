using CommunityToolkit.HighPerformance.Buffers;
using Fusee.PointCloud.Common.Accessors;
using System;
using System.Buffers;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Implement this into any Point Cloud Reader.
    /// </summary>
    public interface IPointReader
    {
        /// <summary>
        /// Returns a renderable point cloud component.
        /// </summary>
        /// <param name="fileFolderPath">Path to the file.</param>
        /// <param name="renderMode">Determines which <see cref="RenderMode"/> is used to display the returned point cloud."/></param>
        public IPointCloud GetPointCloudComponent(RenderMode renderMode);

        /// <summary>
        /// Returns the point type.
        /// </summary>
        public PointType PointType { get; }

        /// <summary>
        /// Reads the Potree file and returns an octree.
        /// </summary>
        public IPointCloudOctree GetOctree();

        /// <summary>
        /// Returns the points for one octant as generic array.
        /// </summary>
        /// <typeparam name="TPoint">The generic point type.</typeparam>
        /// <param name="id">The unique id of the octant.</param>
        public MemoryOwner<TPoint> LoadNodeData<TPoint>(OctantId id) where TPoint : struct;

    }
}