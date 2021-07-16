using Fusee.Math.Core;

namespace Fusee.PointCloud.PointAccessorCollections
{
    /// <summary>
    /// Enum that contains all available point types.
    /// Abbreviations:
    /// 32: float
    /// 64: double
    /// Pos: position
    /// Col: Color
    /// I: Intensity
    /// Nor: Normal
    /// </summary>
    public enum PointType
    {
        Pos64,
        Pos64Col32IShort,
        Pos64IShort,
        Pos64Col32,
        Pos64Label8,
        Pos64Nor32Col32IShort,
        Pos64Nor32IShort,
        Pos64Nor32Col32
    }

    /// <summary>
    /// Point type: Position float3.
    /// </summary>
    public class Pos32
    {
        public float3 Position;
    }

    /// <summary>
    /// Point type: Position double3.
    /// </summary>
    public class Pos64
    {
        public double3 Position;
    }

    /// <summary>
    /// Point type: Position, color, intensity.
    /// </summary>
    public class Pos64Col32IShort
    {
        public double3 Position;
        public float3 Color;
        public ushort Intensity;
    }

    /// <summary>
    /// Point type: Position, intensity.
    /// </summary>
    public class Pos64IShort
    {
        public double3 Position;
        public ushort Intensity;
    }

    /// <summary>
    /// Point type: Position, color.
    /// </summary>
    public class Pos64Col32
    {
        public double3 Position;
        public float3 Color;
    }

    /// <summary>
    /// Point type: Position, color of the classification.
    /// </summary>
    public class Pos64Label8
    {
        public double3 Position;
        public byte Label;
    }

    /// <summary>
    /// Point type: Position, normal, color, intensity.
    /// </summary>
    public class Pos64Nor32Col32IShort
    {
        public double3 Position;
        public float3 Normal;
        public float3 Color;
        public ushort Intensity;
    }

    /// <summary>
    /// Point type: Position, normal, intensity.
    /// </summary>
    public class Pos64Nor32IShort
    {
        public double3 Position;
        public float3 Normal;
        public ushort Intensity;
    }

    /// <summary>
    /// Point type: Position, normal, color.
    /// </summary>
    public class Pos64Nor32Col32
    {
        public double3 Position;
        public float3 Normal;
        public float3 Color;
    }
}