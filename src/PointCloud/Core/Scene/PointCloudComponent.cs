using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.Core.Scene
{
    /// <summary>
    /// File type independent point cloud implementation.
    /// </summary>
    public class PointCloudComponent : SceneComponent, IPointCloud
    {
        /// <summary>
        /// File type independent properties for point cloud rendering.
        /// </summary>
        public IPointCloudImpBase PointCloudImp { get; protected set; }

        /// <summary>
        /// Center of the point cloud.
        /// </summary>
        public float3 Center { get => PointCloudImp.Center; }

        /// <summary>
        /// Size of the point clouds (quadratic) bounding box.
        /// </summary>
        public float3 Size { get => PointCloudImp.Size; }

        public readonly bool DoRenderInstanced;

        /// <summary>
        /// Instantiates the <see cref="IPointCloudImp{TGpuData}"/>.
        /// </summary>
        public PointCloudComponent(IPointCloudImpBase imp, bool doRenderInstanced)
        {
            DoRenderInstanced = doRenderInstanced;
            PointCloudImp = imp;
        }
    }
}