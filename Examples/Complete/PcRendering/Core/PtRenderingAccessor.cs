using Fusee.Math.Core;
using Fusee.Pointcloud.Common;

namespace Fusee.Examples.PcRendering.Core
{
    public class PtRenderingAccessor : PointAccessor<LAZPointType>
    {
        public override bool HasPositionFloat3_64 => true;
        public override bool HasColorFloat3_32 => true;
        public override bool HasIntensityUInt_16 => true;

        public override void SetColorFloat3_32(ref LAZPointType point, float3 val)
        {
            point.Color = val;
        }

        public override ref float3 GetColorFloat3_32(ref LAZPointType point)
        {
            return ref point.Color;
        }

        public override void SetPositionFloat3_64(ref LAZPointType point, double3 val)
        {
            point.Position = val;
        }

        public override ref double3 GetPositionFloat3_64(ref LAZPointType point)
        {
            return ref point.Position;
        }

        public override ref ushort GetIntensityUInt_16(ref LAZPointType point)
        {
            return ref point.Intensity;
        }

        public override void SetIntensityUInt_16(ref LAZPointType point, ushort val)
        {
            point.Intensity = val;
        }
    }
}
