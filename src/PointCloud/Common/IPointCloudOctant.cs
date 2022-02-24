using Fusee.Math.Core;
using Fusee.Structures;

namespace Fusee.PointCloud.Common
{
    public interface IPointCloudOctant : IEmptyOctant<double3, double>
    {
        public int NumberOfPointsInNode { get; set; }
    }
}
