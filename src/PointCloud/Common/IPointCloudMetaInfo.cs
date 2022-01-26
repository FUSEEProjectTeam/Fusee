namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Interface used in <see cref="IPointReader"/>. 
    /// Used to store the meta info of a point cloud, which is usually found in the header of the file.
    /// </summary>
    public interface IPointCloudMetaInfo
    {
        long PointCount { get; set; }

    }
}