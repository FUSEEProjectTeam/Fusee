using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.PointCloudFileReader.Las
{
    public struct LasPointFormat : IPointFormat
    {
        public bool HasIntensity;
        public bool HasClassification;
        public bool HasUserData;
        public bool HasColor;
    }
}