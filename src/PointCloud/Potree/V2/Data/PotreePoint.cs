using Fusee.Math.Core;

namespace Fusee.PointCloud.Potree.V2.Data
{
    public class PotreePoint
    {
        public double3 Position;
        public short Intensity;
        public byte ReturnNumber;
        public byte NumberOfReturns;
        public byte Classification;
        public byte ScanAngleRank;
        public byte UserData;
        public byte PointSourceId;
        public float3 Color;
    }
}