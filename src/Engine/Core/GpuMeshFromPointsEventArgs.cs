using Fusee.Base.Core;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Used in a <see cref="PointCloudLoader{TPoint}"/> for caching meshes using the <see cref="MemoryCache{TItem}.AddItem"/> event.
    /// </summary>
    /// <typeparam name="TPoint"></typeparam>
    public class GpuMeshFromPointsEventArgs<TPoint> : EventArgs where TPoint : new()
    {
        /// <summary>
        /// "Raw" point cloud points that go into a mesh.
        /// </summary>
        public TPoint[] Points;

        /// <summary>
        /// The <see cref="RenderContext"/>, used to create the mesh data on the GPU.
        /// </summary>
        public RenderContext RenderContext;

        /// <summary>
        /// Creates a new instance of type <see cref="GpuMeshFromPointsEventArgs{TPoint}"/>.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="rc"></param>
        public GpuMeshFromPointsEventArgs(TPoint[] points, RenderContext rc)
        {
            Points = points;
            RenderContext = rc;
        }
    }
}
