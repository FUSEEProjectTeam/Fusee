﻿using Fusee.Base.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Used in a <see cref="PointCloudLoader{TPoint}"/> for caching loaded points using the <see cref="MemoryCache{TItem}.AddItemAsync"/> event.
    /// </summary>
    /// <typeparam name="TPoint"></typeparam>
    public class LoadPointEventArgs<TPoint> : EventArgs where TPoint : new()
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

        /// <summary>
        /// Creates a new instance of type <see cref="LoadPointEventArgs{TPoint}"/>.
        /// </summary>
        /// <param name="octant">The octant we want to read the points for.</param>
        /// <param name="pathToFile">The path to the potree file.</param>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/>.</param>
        public LoadPointEventArgs(PtOctantRead<TPoint> octant, string pathToFile, IPointAccessor ptAccessor)
        {
            Octant = octant;
            PathToFile = pathToFile;
            PtAccessor = ptAccessor;
        }
    }
}
