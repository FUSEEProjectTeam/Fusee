using Fusee.Math.Core;
using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.Core
{
    //Collection of PointAccessor classes. There has to be one for each PointType.

    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position information only.
    /// </summary>
    public class Pos64Accessor : PointAccessor<Pos64>
    {
        public Pos64Accessor()
        {
            PositionType = PointPositionType.Float3_64;
        }

        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64 point, double3 val)
        {
            point.Position = val;
        }

        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64 point)
        {
            return ref point.Position;
        }
    }

    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position, color and intensity values.
    /// </summary>
    public class Pos64Col32IShortAccessor : PointAccessor<Pos64Col32IShort>
    {
        public Pos64Col32IShortAccessor()
        {
            PositionType = PointPositionType.Float3_64;
            ColorType = PointColorType.Float3_32;
            IntensityType = PointIntensityType.UInt_8;
        }

        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public override void SetColorFloat3_32(ref Pos64Col32IShort point, float3 val)
        {
            point.Color = val;
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetColorFloat3_32(ref Pos64Col32IShort point)
        {
            return ref point.Color;
        }
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64Col32IShort point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64Col32IShort point)
        {
            return ref point.Position;
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref ushort GetIntensityUInt_16(ref Pos64Col32IShort point)
        {
            return ref point.Intensity;
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public override void SetIntensityUInt_16(ref Pos64Col32IShort point, ushort val)
        {
            point.Intensity = val;
        }
    }

    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position and intensity values.
    /// </summary>
    public class Pos64IShortAccessor : PointAccessor<Pos64IShort>
    {
        public Pos64IShortAccessor()
        {
            PositionType = PointPositionType.Float3_64;
            IntensityType = PointIntensityType.UInt_16;
        }

        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64IShort point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64IShort point)
        {
            return ref point.Position;
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref ushort GetIntensityUInt_16(ref Pos64IShort point)
        {
            return ref point.Intensity;
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public override void SetIntensityUInt_16(ref Pos64IShort point, ushort val)
        {
            point.Intensity = val;
        }
    }

    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position and color values.
    /// </summary>
    public class Pos64Col32Accessor : PointAccessor<Pos64Col32>
    {
        public Pos64Col32Accessor()
        {
            PositionType = PointPositionType.Float3_64;
            ColorType = PointColorType.Float3_32;
        }

        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public override void SetColorFloat3_32(ref Pos64Col32 point, float3 val)
        {
            point.Color = val;
        }

        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetColorFloat3_32(ref Pos64Col32 point)
        {
            return ref point.Color;
        }
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64Col32 point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64Col32 point)
        {
            return ref point.Position;
        }
    }

    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position and label values.
    /// </summary>
    public class Pos64Label8Accessor : PointAccessor<Pos64Label8>
    {
        public Pos64Label8Accessor()
        {
            PositionType = PointPositionType.Float3_64;
            LabelType = PointLabelType.UInt_8;
        }

        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public override void SetLabelUInt_8(ref Pos64Label8 point, byte val)
        {
            point.Label = val;
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref byte GetLabelUInt_8(ref Pos64Label8 point)
        {
            return ref point.Label;
        }
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64Label8 point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64Label8 point)
        {
            return ref point.Position;
        }
    }

    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position, normal vectors, color and intensity values.
    /// </summary>
    public class Pos64Nor32Col32IShortAccessor : PointAccessor<Pos64Nor32Col32IShort>
    {
        public Pos64Nor32Col32IShortAccessor()
        {
            PositionType = PointPositionType.Float3_64;
            ColorType = PointColorType.Float3_32;
            IntensityType = PointIntensityType.UInt_16;
            NormalType = PointNormalType.Float3_32;
        }

        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public override void SetColorFloat3_32(ref Pos64Nor32Col32IShort point, float3 val)
        {
            point.Color = val;
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetColorFloat3_32(ref Pos64Nor32Col32IShort point)
        {
            return ref point.Color;
        }
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64Nor32Col32IShort point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64Nor32Col32IShort point)
        {
            return ref point.Position;
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref ushort GetIntensityUInt_16(ref Pos64Nor32Col32IShort point)
        {
            return ref point.Intensity;
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public override void SetIntensityUInt_16(ref Pos64Nor32Col32IShort point, ushort val)
        {
            point.Intensity = val;
        }
        /// <summary>
        /// Returns the normal vector of a point cloud point if <see cref="PointNormalType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetNormalFloat3_32(ref Pos64Nor32Col32IShort point)
        {
            return ref point.Normal;
        }
        /// <summary>
        /// Sets the normal vector of a point cloud point if <see cref="PointNormalType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new normal vector.</param>
        public override void SetNormalFloat3_32(ref Pos64Nor32Col32IShort point, float3 val)
        {
            point.Normal = val;
        }
    }

    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position, normal vectors and intensity values.
    /// </summary>
    public class Pos64Nor32IShortAccessor : PointAccessor<Pos64Nor32IShort>
    {
        public Pos64Nor32IShortAccessor()
        {
            PositionType = PointPositionType.Float3_64;
            IntensityType = PointIntensityType.UInt_16;
            NormalType = PointNormalType.Float3_64;
        }
        
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64Nor32IShort point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64Nor32IShort point)
        {
            return ref point.Position;
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref ushort GetIntensityUInt_16(ref Pos64Nor32IShort point)
        {
            return ref point.Intensity;
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public override void SetIntensityUInt_16(ref Pos64Nor32IShort point, ushort val)
        {
            point.Intensity = val;
        }
        /// <summary>
        /// Returns the normal vector of a point cloud point if <see cref="PointNormalType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetNormalFloat3_32(ref Pos64Nor32IShort point)
        {
            return ref point.Normal;
        }
        /// <summary>
        /// Sets the normal vector of a point cloud point if <see cref="PointNormalType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new normal vector.</param>
        public override void SetNormalFloat3_32(ref Pos64Nor32IShort point, float3 val)
        {
            point.Normal = val;
        }
    }

    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position, color and intensity values.
    /// </summary>
    public class Pos64Nor32Col32Accessor : PointAccessor<Pos64Nor32Col32>
    {
        public Pos64Nor32Col32Accessor()
        {
            PositionType = PointPositionType.Float3_64;
            ColorType = PointColorType.Float3_32;
            NormalType = PointNormalType.Float3_32;
        }

        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public override void SetColorFloat3_32(ref Pos64Nor32Col32 point, float3 val)
        {
            point.Color = val;
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetColorFloat3_32(ref Pos64Nor32Col32 point)
        {
            return ref point.Color;
        }

        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64Nor32Col32 point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64Nor32Col32 point)
        {
            return ref point.Position;
        }
        /// <summary>
        /// Returns the normal vector of a point cloud point if <see cref="PointNormalType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetNormalFloat3_32(ref Pos64Nor32Col32 point)
        {
            return ref point.Normal;
        }
        /// <summary>
        /// Sets the normal vector of a point cloud point if <see cref="PointNormalType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new normal vector.</param>
        public override void SetNormalFloat3_32(ref Pos64Nor32Col32 point, float3 val)
        {
            point.Normal = val;
        }
    }
}