using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Structures;
using Fusee.Math.Core;

namespace Fusee.PointCloud.Common
{
    public interface IPointCloudOctant : IEmptyOctant<double3, double>
    {
        public int NumberOfPointsInNode { get; set; }
    }
}
