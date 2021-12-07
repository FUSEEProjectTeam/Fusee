using Fusee.Base.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Used in a <see cref="PointCloudLoader{TPoint}"/> for caching loaded points using the <see cref="MemoryCache{TItem}.AddItem"/> event.
    /// </summary>
    /// <typeparam name="TPoint"></typeparam>
    public class LoadPointsEventArgs<TPoint> : EventArgs where TPoint : new()
    {
        /// <summary>
        /// The path to the potree file
        /// </summary>
        public string PathToFile;

        /// <summary>
        /// The <see cref="PointAccessor{TPoint}"/>.
        /// </summary>
        public PointAccessor<TPoint> PtAccessor;

        /// <summary>
        /// The octant we want to read the points for.
        /// </summary>
        public PtOctantRead<TPoint> Octant;

        /// <summary>
        /// Creates a new instance of type <see cref="LoadPointsEventArgs{TPoint}"/>.
        /// </summary>
        /// <param name="octant">The octant we want to read the points for.</param>
        /// <param name="pathToFile">The path to the potree file.</param>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/>.</param>
        public LoadPointsEventArgs(PtOctantRead<TPoint> octant, string pathToFile, PointAccessor<TPoint> ptAccessor)
        {
            Octant = octant;
            PathToFile = pathToFile;
            PtAccessor = ptAccessor;
        }
    }

    /// <summary>
    /// Used in a <see cref="PointCloudLoader{TPoint}"/> for caching meshes using the <see cref="MemoryCache{TItem}.AddItem"/> event.
    /// </summary>
    /// <typeparam name="TPoint"></typeparam>
    public class CreateMeshEventArgs<TPoint> : EventArgs where TPoint : new()
    {
        /// <summary>
        /// "Raw" point cloud points that go into a mesh.
        /// </summary>
        public TPoint[] Points;

        /// <summary>
        /// The octant we want to create the mesh for.
        /// </summary>
        public PtOctantRead<TPoint> Octant;

        /// <summary>
        /// Creates a new instance of type <see cref="CreateMeshEventArgs{TPoint}"/>.
        /// </summary>
        /// <param name="octant"></param>
        /// <param name="points"></param>
        public CreateMeshEventArgs(PtOctantRead<TPoint> octant, TPoint[] points)
        {
            Octant = octant;
            Points = points;
        }
    }
}
