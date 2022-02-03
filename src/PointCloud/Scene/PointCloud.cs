using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Scene.Imp;

namespace Fusee.PointCloud.Scene
{
    /// <summary>
    /// File type independent point cloud implementation.
    /// </summary>
    public class PointCloud : SceneComponent
    {
        /// <summary>
        /// File type independent properties for point cloud rendering.
        /// </summary>
        public IPointCloudImp PointCloudImp { get; private set; }

        /// <summary>
        /// Center of the point cloud.
        /// </summary>
        public float3 Center { get => PointCloudImp.Center; }

        /// <summary>
        /// Size of the point clouds (quadratic) bounding box.
        /// </summary>
        public float3 Size { get => PointCloudImp.Size; }

        /// <summary>
        /// Instantiates the <see cref="PointCloudImp"/>, depending on the file type.
        /// </summary>
        /// <param name="fileFileFolderPath">The path to the point cloud file.</param>
        /// <param name="fileType">The type of the point cloud file.</param>
        public PointCloud(string fileFileFolderPath, PointCloudFileType fileType)
        {
            switch (fileType)
            {
                case PointCloudFileType.Las:
                    PointCloudImp = new LasCloud(fileFileFolderPath);
                    break;
                case PointCloudFileType.Potree2:
                    PointCloudImp = new Potree2Cloud(fileFileFolderPath);
                    break;
            }
        }
    }
}
