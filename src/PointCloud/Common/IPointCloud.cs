using Fusee.Math.Core;

namespace Fusee.PointCloud.Common
{
    public interface IPointCloud
    {
        /// <summary>
        /// File type independent properties for point cloud rendering.
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
