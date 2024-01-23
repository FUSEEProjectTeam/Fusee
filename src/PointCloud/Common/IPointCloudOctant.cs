﻿using Fusee.Math.Core;
using Fusee.Structures;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Used in <see cref="IPointCloudOctree"/>. Allows the use in non-generic context, e.g. in <see cref="IPointReader"/>s.
    /// </summary>
    public interface IPointCloudOctant : IEmptyOctant<double3, double3>
    {
        /// <summary>
        /// The number of points that fall into this octant.
        /// </summary>
        public int NumberOfPointsInNode { get; set; }
    }
}