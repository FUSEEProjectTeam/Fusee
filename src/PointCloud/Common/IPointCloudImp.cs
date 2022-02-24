using Fusee.Engine.Core;
using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Smallest common set of properties that are needed to render point clouds out of core.
    /// </summary>
    public interface IPointCloudImp
    {
        /// <summary>
        /// The <see cref="GpuMesh"/>, created from visible octants/point chunks, that are ready to be rendered.
        /// </summary>
        public List<GpuMesh> MeshesToRender { get; set; }

        /// <summary>
        /// Center of the PointCloud's AABB
        /// </summary>
        public float3 Center { get; }

        /// <summary>
        /// Dimensions of the PointCloud's AABB
        /// </summary>
        public float3 Size { get; }

        /// <summary>
        /// The number of points that are currently visible.
        /// </summary>
        public int NumberOfVisiblePoints { get; }

        /// <summary>
        /// Changes the minimum, screen projected, size of a octant. If an octant is smaller it won't be rendered.
        /// </summary>
        public float MinProjSizeModifier
        {
            get;
            set;
        }

        /// <summary>
        /// Maximal number of points that are visible in one frame - trade-off between performance and quality.
        /// </summary>
        public int PointThreshold
        {
            get;
            set;
        }

        /// <summary>
        /// The amount of milliseconds needed to pass before rendering next frame
        /// </summary>
        public float UpdateRate
        {
            get;
            set;
        }

        /// <summary>
        /// Triggers a visibility check for the point cloud octree.
        /// Is called every frame.
        /// </summary>
        /// <param name="fov">The current field of view.</param>
        /// <param name="viewportHeight">The current viewport height.</param>
        /// <param name="renderFrustum">The current viewport width.</param>
        /// <param name="camPos">The current camera position in world coordinates.</param>
        public void Update(float fov, int viewportHeight, FrustumF renderFrustum, float3 camPos);

    }
}
