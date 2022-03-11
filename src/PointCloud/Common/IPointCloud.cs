using Fusee.Math.Core;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Interface for file type independent point cloud implementations.
    /// </summary>
    public interface IPointCloud
    {
        /// <summary>
        /// Non-Point-type-specific out-out-of-core point cloud implementation.
        /// </summary>
        public IPointCloudImp PointCloudImp { get; }

        /// <summary>
        /// Center of the point cloud.
        /// </summary>
        public float3 Center { get; }

        /// <summary>
        /// Size of the point clouds (quadratic) bounding box.
        /// </summary>
        public float3 Size { get; }
    }
}