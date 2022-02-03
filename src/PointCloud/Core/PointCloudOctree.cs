using Fusee.PointCloud.Common;
using Fusee.Math.Core;

namespace Fusee.PointCloud.Core
{
    public class PointCloudOctree : IPointCloudOctree
    {
        public IPointCloudOctant Root { get; private set; }

        public int MaxLevel { get; private set; }

        /// <summary>
        /// Constructor for creating an Octree that is suitable for creating files from it. 
        /// </summary>
        public PointCloudOctree(double3 center, double size, int maxLvl)
        {
            Root = new PointCloudOctant(center, size, "r");
            MaxLevel = maxLvl;
        }
    }
}