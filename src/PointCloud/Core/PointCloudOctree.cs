using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.Core
{
    public class PointCloudOctree : IPointCloudOctree
    {
        public PointCloudOctant Root { get; private set; }

        public int MaxLevel;

        /// <summary>
        /// Constructor for creating an Octree that is suitable for creating files from it. 
        /// </summary>
        public PointCloudOctree(PointCloudOctant root)
        {
            Root = root;
        }

    }
}