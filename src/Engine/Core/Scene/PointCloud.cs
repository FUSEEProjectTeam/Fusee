using Fusee.Math.Core;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Point cloud file types that Fusee can handle.
    /// </summary>
    public enum PointCloudFileType
    {
        Las,
        Potree2
    }

    /// <summary>
    /// Smallest common set of properties that are needed to render point clouds.
    /// Should be able to handle all file types that are defined in <see cref="PointCloudFileType"/>.
    /// </summary>
    public interface IPointCloudImp
    {
        public List<GpuMesh> MeshesToRender { get; set; }

        public PointCloudFileType FileType { get; }

        public float3 Center { get; }

        public float3 Size { get; }

    }

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
        /// Instantiates the <see cref="PointCloudImp"/>, depending on the file type.
        /// </summary>
        /// <param name="fileFileFolderPath">The path to the point cloud file.</param>
        /// <param name="fileType">The type of the point cloud file.</param>
        public PointCloud(string fileFileFolderPath, PointCloudFileType fileType)
        {
            switch (fileType)
            {
                case PointCloudFileType.Las:
                    throw new NotImplementedException();
                case PointCloudFileType.Potree2:
                    PointCloudImp = new Potree2Cloud(fileFileFolderPath);
                    break;
            }
        }

        /// <summary>
        /// Center of the point cloud.
        /// </summary>
        public float3 Center { get => PointCloudImp.Center; }

        /// <summary>
        /// Size of the point clouds (quadratic) bounding box.
        /// </summary>
        public float3 Size { get => PointCloudImp.Size; }
    }
}
