using Fusee.Math.Core;
using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// This octree's purpose is the out of core rendering of point clouds. It is used to determine the visibility of point chunks in octants.
    /// </summary>
    public class PointCloudOctree : IPointCloudOctree
    {
        /// <summary>
        /// The root node of the octree.
        /// </summary>
        public IPointCloudOctant Root { get; private set; }

        /// <summary>
        /// The maximum level of the octree.
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// Constructor for creating an Octree that is suitable for creating files from it. 
        /// </summary>
        public PointCloudOctree(double3 center, double size, int maxLvl)
        {
            Root = new PointCloudOctant(center, size, "r");
            Depth = maxLvl;
        }
    }
}