using Fusee.Base.Core;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Non-generic base class for <see cref="GpuMeshFromPointsEventArgs{TPoint}"/>.
    /// </summary>
    internal abstract class GpuMeshFromPointsEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="RenderContext"/>, used to create the mesh data on the GPU.
        /// </summary>
        public RenderContext RenderContext;

        /// <summary>
        /// The number of point cloud points.
        /// </summary>
        public int NumberOfPoints;
    }

    /// <summary>
    /// Used in a <see cref="PointCloudLoader{TPoint}"/> for caching meshes using the <see cref="MemoryCache{TItem}.AddItemAsync"/> event.
    /// </summary>
    /// <typeparam name="TPoint"></typeparam>
    internal class GpuMeshFromPointsEventArgs<TPoint> : GpuMeshFromPointsEventArgs where TPoint : new()
    {
        /// <summary>
        /// "Raw" point cloud points that go into a mesh.
        /// </summary>
        public TPoint[] Points;

        /// <summary>
        /// Creates a new instance of type <see cref="GpuMeshFromPointsEventArgs{TPoint}"/>.
        /// </summary>
        /// <param name="points">The array of point cloud points.</param>
        /// <param name="rc">The <see cref="RenderContext"/>, used to create the <see cref="GpuMesh"/>.</param>
        /// <param name="numberOfPoints">The number of point cloud points.</param>
        public GpuMeshFromPointsEventArgs(TPoint[] points, RenderContext rc, int numberOfPoints)
        {
            Points = points;
            RenderContext = rc;
            NumberOfPoints = numberOfPoints;
        }
    }
}
