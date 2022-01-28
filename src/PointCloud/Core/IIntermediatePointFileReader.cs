using Fusee.PointCloud.Common;
using System.Threading.Tasks;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Allow multiple versions of out of core readers.
    /// Must be based on a PtOctree.
    /// </summary>
    public interface IIntermediatePointFileReader
    {
        public PointType GetPointType(string pathToNodeFileFolder = "");

        public IPointCloudOctree GetOctree(string fileFolderPath);

        public Task<TPoint[]> LoadPointsForNodeAsync<TPoint>(string guid, IPointAccessor pointAccessor) where TPoint : new();

        public TPoint[] LoadNodeData<TPoint>(string id, IPointAccessor pointAccessor) where TPoint : new();

    }
}
