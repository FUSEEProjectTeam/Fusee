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

        /// <summary>
        /// Determines whether this point cloud should be rendered using gpu instancing.
        /// </summary>
        public readonly RenderMode RenderMode;

        /// <summary>
        /// Reference to the Camera whose properties are used to control the visibility of point cloud chunks (octants).
        /// </summary>
        public Camera? Camera;

        /// <summary>
        /// Instantiates the <see cref="IPointCloudImpBase"/>.
        /// </summary>
        public PointCloudComponent(IPointCloudImpBase imp, RenderMode renderMode = RenderMode.StaticMesh)
        {
            RenderMode = renderMode;
            PointCloudImp = imp;
        }
    }
}