using Fusee.Math.Core;

namespace Fusee.PointCloud.Common
{
    public interface IPointCloudOctree
    {
        public IPointCloudOctant Root { get; }

        public int MaxLevel { get; }
    }
}
