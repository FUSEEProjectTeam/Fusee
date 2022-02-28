namespace Fusee.PointCloud.Potree.V2.Data
{
    /// <summary>
    /// Contains information about the Potree file's meta data and hierarchy/octree.
    /// </summary>
    public class PotreeData
    {
        /// <summary>
        /// The hierarchy as linear list of <see cref="PotreeNode"/>s.
        /// </summary>
        public PotreeHierarchy Hierarchy;

        /// <summary>
        /// The meta data of the file.
        /// </summary>
        public PotreeMetadata Metadata;
    }
}
