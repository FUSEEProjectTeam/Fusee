namespace Fusee.PointCloud.PointCloudFileReader.Las
{
    internal struct LasInternalPoint
    {
        public int X;
        public int Y;
        public int Z;

        public ushort Intensity;

        public byte Classification;
        public byte UserData;

        public ushort R;
        public ushort G;
        public ushort B;
    }
}