using CommunityToolkit.HighPerformance.Buffers;
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        MemoryOwner<PosD3ColF3LblB> LoadNodeDataPosD3ColF3LblB(OctantId id);

    }
}