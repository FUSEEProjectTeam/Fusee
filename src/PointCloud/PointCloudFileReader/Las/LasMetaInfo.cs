using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.PointCloudFileReader.Las
{
    public struct LasMetaInfo : IPointCloudMetaInfo
    {
        public string Filename;

        public byte PointDataFormat;

        public long PointCnt;

        public double ScaleFactorX;
        public double ScaleFactorY;
        public double ScaleFactorZ;

        public double OffsetX;
        public double OffsetY;
        public double OffsetZ;
    }
}