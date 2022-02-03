using Fusee.PointCloud.Common;
namespace Fusee.PointCloud.FileReader.Las.Desktop
{
    public struct LasMetaInfo : IPointCloudMetaInfo
    {
        public string Filename;

        public byte PointDataFormat;

        public long PointCount { get; set; }

        public double ScaleFactorX;
        public double ScaleFactorY;
        public double ScaleFactorZ;

        public double OffsetX;
        public double OffsetY;
        public double OffsetZ;
    }
}