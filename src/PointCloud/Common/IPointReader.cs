using Fusee.PointCloud.Common.Accessors;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Implement this into any Point Cloud Reader.
    /// </summary>
    public interface IPointReader
    {
        public IPointAccessor PointAccessor { get; }

        public IPointCloud GetPointCloudComponent(string fileFolderPath);

        public PointType GetPointType();

        public IPointCloudOctree GetOctree();

        public TPoint[] LoadNodeData<TPoint>(string id) where TPoint : new();

    }
}