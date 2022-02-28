namespace Fusee.PointCloud.Common
{
    public interface IPointCloudOctree
    {
        public IPointCloudOctant Root { get; }

        public int Depth { get; }
    }
}
