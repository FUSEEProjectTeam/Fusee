using Fusee.Engine.Core;
using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.PointCloud.Common
{
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

        public void Update(float fov, int viewportHeight, FrustumF renderFrustum, float3 camPos);

    }
}
