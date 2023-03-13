namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Implement this into any Point Cloud Reader.
    /// </summary>
    public interface IPointReader
    {
        /// <summary>
        /// Returns a renderable point cloud component.
        /// </summary>
        /// <param name="renderMode">Determines which <see cref="RenderMode"/> is used to display the returned point cloud."/></param>
        public IPointCloud GetPointCloudComponent(RenderMode renderMode);

        /// <summary>
        /// Reads the Potree file and returns an octree.
        /// </summary>
        public IPointCloudOctree GetOctree();
    }
}