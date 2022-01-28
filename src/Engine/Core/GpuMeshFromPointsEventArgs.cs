using Fusee.Base.Core;
using Fusee.PointCloud.Common;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Used in a <see cref="PointCloudLoader"/> for caching meshes using the <see cref="MemoryCache{TItem}.AddItemAsync"/> event.
    /// </summary>
    internal class GpuMeshFromPointsEventArgs<TPoint> : EventArgs
    {
        /// <summary>
        /// The <see cref="RenderContext"/>, used to create the mesh data on the GPU.
        /// </summary>
        public CreateGpuMesh CreateGpuMesh;

        /// <summary>
        /// The number of point cloud points.
        /// </summary>
        public int NumberOfPoints;

        /// <summary>
        /// "Raw" point cloud points that go into a mesh.
        /// </summary>
        public TPoint[] Points;

        /// <summary>
        /// Creates a new instance of type <see cref="GpuMeshFromPointsEventArgs"/>.
        /// </summary>
        /// <param name="points">The array of point cloud points.</param>
        /// <param name="rc">The <see cref="RenderContext"/>, used to create the <see cref="GpuMesh"/>.</param>
        /// <param name="numberOfPoints">The number of point cloud points.</param>
        public GpuMeshFromPointsEventArgs(TPoint[] points, CreateGpuMesh createGpuMesh, int numberOfPoints)
        {
            Points = points;
            CreateGpuMesh = createGpuMesh;
            NumberOfPoints = numberOfPoints;
        }
    }
}
