namespace Fusee.PointCloud.Las.Desktop
{
    public struct LasMetaInfo
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