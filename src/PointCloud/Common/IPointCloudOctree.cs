namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// This octree's purpose is the out of core rendering of point clouds. It is used to determine the visibility of point chunks in octants.
    /// Allows the use in non-generic context, e.g. in <see cref="IPointReader"/>s.
    /// </summary>
    public interface IPointCloudOctree
    {
        /// <summary>
        /// The root node of the octree.
        /// </summary>
        public IPointCloudOctant Root { get; }

        /// <summary>
        /// The maximum level of the octree.
        /// </summary>
        public int Depth { get; }
    }
}