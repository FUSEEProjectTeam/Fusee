using Fusee.Base.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Used in a <see cref="PointCloudLoader"/> for caching loaded points using the <see cref="MemoryCache{TItem}.AddItemAsync"/> event.
    /// </summary>
    public class LoadPointEventArgs<TPoint> : EventArgs
    {
        /// <summary>
        /// The path to the potree file
        /// </summary>
        public string PathToFile;

        /// <summary>
        /// The <see cref="PointAccessor{TPoint}"/>.
        /// </summary>
        public IPointAccessor PtAccessor;

        /// <summary>
        /// The octant we want to read the points for.
        /// </summary>
        public PtOctantRead<TPoint> Octant;

        public string Guid;

        /// <summary>
        /// Creates a new instance of type <see cref="LoadPointEventArgs"/>.
        /// </summary>
        /// <param name="octant">The octant we want to read the points for.</param>
        /// <param name="pathToFile">The path to the potree file.</param>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/>.</param>
        public LoadPointEventArgs(string guid, PtOctantRead<TPoint> octant, string pathToFile, IPointAccessor ptAccessor)
        {
            Guid = guid;
            Octant = octant;
            PathToFile = pathToFile;
            PtAccessor = ptAccessor;
        }
    }
}
