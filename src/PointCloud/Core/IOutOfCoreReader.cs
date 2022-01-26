using Fusee.PointCloud.Common;
using System.Threading.Tasks;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Allow multiple versions of out of core readers.
    /// Must be based on a PtOctree.
    /// </summary>
    public interface IOutOfCoreReader
    {
        public PointType GetPointType(string pathToNodeFileFolder = "");

        public PtOctree GetOctree(string fileFolderPath);

        public Task<IPointCloudPoint[]> LoadPointsForNodeAsync(string guid, IPointAccessor pointAccessor);

        public IPointCloudPoint[] LoadNodeData(string id, IPointAccessor pointAccessor);

    }
}
