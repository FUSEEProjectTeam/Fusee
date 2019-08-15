using Fusee.Math.Core;
using Fusee.Pointcloud.Common;

namespace Fusee.Pointcloud.PointAccessorCollections
{
    //Collection of PointAccessor classes. There has to be one for each PointType.

    public class Pos64_Accessor : PointAccessor<Pos64>
    {
        public override bool HasPositionFloat3_64 => true;
                

        public override void SetPositionFloat3_64(ref Pos64 point, double3 val)
        {
            point.Position = val;
        }

        public override ref double3 GetPositionFloat3_64(ref Pos64 point)
        {
            return ref point.Position;
        }
    }

    public class Pos64Col32IShort_Accessor : PointAccessor<Pos64Col32IShort>
    {
        public override bool HasPositionFloat3_64 => true;
        public override bool HasColorFloat3_32 => true;
        public override bool HasIntensityUInt_16 => true;

        public override void SetColorFloat3_32(ref Pos64Col32IShort point, float3 val)
        {
            point.Color = val;
        }

        public override ref float3 GetColorFloat3_32(ref Pos64Col32IShort point)
        {
            return ref point.Color;
        }

        public override void SetPositionFloat3_64(ref Pos64Col32IShort point, double3 val)
        {
            point.Position = val;
        }

        public override ref double3 GetPositionFloat3_64(ref Pos64Col32IShort point)
        {
            return ref point.Position;
        }

        public override ref ushort GetIntensityUInt_16(ref Pos64Col32IShort point)
        {
            return ref point.Intensity;
        }

        public override void SetIntensityUInt_16(ref Pos64Col32IShort point, ushort val)
        {
            point.Intensity = val;
        }
    }

    public class Pos64IShort_Accessor : PointAccessor<Pos64IShort>
    {
        public override bool HasPositionFloat3_64 => true;
        public override bool HasIntensityUInt_16 => true;
                
        public override void SetPositionFloat3_64(ref Pos64IShort point, double3 val)
        {
            point.Position = val;
        }

        public override ref double3 GetPositionFloat3_64(ref Pos64IShort point)
        {
            return ref point.Position;
        }

        public override ref ushort GetIntensityUInt_16(ref Pos64IShort point)
        {
            return ref point.Intensity;
        }

        public override void SetIntensityUInt_16(ref Pos64IShort point, ushort val)
        {
            point.Intensity = val;
        }
    }

    public class Pos64Col32_Accessor : PointAccessor<Pos64Col32>
    {
        public override bool HasPositionFloat3_64 => true;
        public override bool HasColorFloat3_32 => true;
        
        public override void SetColorFloat3_32(ref Pos64Col32 point, float3 val)
        {
            point.Color = val;
        }

        public override ref float3 GetColorFloat3_32(ref Pos64Col32 point)
        {
            return ref point.Color;
        }

        public override void SetPositionFloat3_64(ref Pos64Col32 point, double3 val)
        {
            point.Position = val;
        }

        public override ref double3 GetPositionFloat3_64(ref Pos64Col32 point)
        {
            return ref point.Position;
        }

        
    }

    public class Pos64Nor32Col32IShort_Accessor : PointAccessor<Pos64Nor32Col32IShort>
    {
        public override bool HasPositionFloat3_64 => true;
        public override bool HasColorFloat3_32 => true;
        public override bool HasIntensityUInt_16 => true;
        public override bool HasNormalFloat3_32 => true;

        public override void SetColorFloat3_32(ref Pos64Nor32Col32IShort point, float3 val)
        {
            point.Color = val;
        }

        public override ref float3 GetColorFloat3_32(ref Pos64Nor32Col32IShort point)
        {
            return ref point.Color;
        }

        public override void SetPositionFloat3_64(ref Pos64Nor32Col32IShort point, double3 val)
        {
            point.Position = val;
        }

        public override ref double3 GetPositionFloat3_64(ref Pos64Nor32Col32IShort point)
        {
            return ref point.Position;
        }

        public override ref ushort GetIntensityUInt_16(ref Pos64Nor32Col32IShort point)
        {
            return ref point.Intensity;
        }

        public override void SetIntensityUInt_16(ref Pos64Nor32Col32IShort point, ushort val)
        {
            point.Intensity = val;
        }

        public override ref float3 GetNormalFloat3_32(ref Pos64Nor32Col32IShort point)
        {
            return ref point.Normal;
        }

        public override void SetNormalFloat3_32(ref Pos64Nor32Col32IShort point, float3 val)
        {
            point.Normal = val;
        }
    }

    public class Pos64Nor32IShort_Accessor : PointAccessor<Pos64Nor32IShort>
    {
        public override bool HasPositionFloat3_64 => true;
        public override bool HasIntensityUInt_16 => true;
        public override bool HasNormalFloat3_32 => true;

        public override void SetPositionFloat3_64(ref Pos64Nor32IShort point, double3 val)
        {
            point.Position = val;
        }

        public override ref double3 GetPositionFloat3_64(ref Pos64Nor32IShort point)
        {
            return ref point.Position;
        }

        public override ref ushort GetIntensityUInt_16(ref Pos64Nor32IShort point)
        {
            return ref point.Intensity;
        }

        public override void SetIntensityUInt_16(ref Pos64Nor32IShort point, ushort val)
        {
            point.Intensity = val;
        }

        public override ref float3 GetNormalFloat3_32(ref Pos64Nor32IShort point)
        {
            return ref point.Normal;
        }

        public override void SetNormalFloat3_32(ref Pos64Nor32IShort point, float3 val)
        {
            point.Normal = val;
        }
    }

    public class Pos64Nor32Col32_Accessor : PointAccessor<Pos64Nor32Col32>
    {
        public override bool HasPositionFloat3_64 => true;
        public override bool HasColorFloat3_32 => true;

        public override void SetColorFloat3_32(ref Pos64Nor32Col32 point, float3 val)
        {
            point.Color = val;
        }

        public override ref float3 GetColorFloat3_32(ref Pos64Nor32Col32 point)
        {
            return ref point.Color;
        }

        public override void SetPositionFloat3_64(ref Pos64Nor32Col32 point, double3 val)
        {
            point.Position = val;
        }

        public override ref double3 GetPositionFloat3_64(ref Pos64Nor32Col32 point)
        {
            return ref point.Position;
        }

        public override ref float3 GetNormalFloat3_32(ref Pos64Nor32Col32 point)
        {
            return ref point.Normal;
        }

        public override void SetNormalFloat3_32(ref Pos64Nor32Col32 point, float3 val)
        {
            point.Normal = val;
        }


    }
}
