
using Fusee.Math.Core;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Enum that contains all available point types.
    /// Abbreviations:
    /// 32: float
    /// 64: double
    /// 8: ushort
    /// Pos: position
    /// Col: Color
    /// I: Intensity
    /// Nor: Normal
    /// </summary>
    public enum PointType
    {
        Undefined,

        /// <summary>
        /// Position only (double)
        /// </summary>
        Pos64,
        /// <summary>
        /// Position (double), Color (float), Intensity (short)
        /// </summary>
        Pos64Col32IShort,
        /// <summary>
        /// Position (double), Intensity (short)
        /// </summary>
        Pos64IShort,
        /// <summary>
        /// Position (double), Color (float)
        /// </summary>
        Pos64Col32,
        /// <summary>
        /// Position (double), Label (ushort)
        /// </summary>
        Pos64Label8,
        /// <summary>
        /// Position (double), Normal (float), Color (float), Intensity (short)
        /// </summary>
        Pos64Nor32Col32IShort,
        /// <summary>
        /// Position (double), Normal (float), Intensity (short)
        /// </summary>
        Pos64Nor32IShort,
        /// <summary>
        /// Position (double), Normal (float), Color (float)
        /// </summary>
        Pos64Nor32Col32
    }

    /// <summary>
    /// Point type: Position float3.
    /// </summary>
    public struct Pos32
    {
        /// <summary>
        /// The points coordinate in 3D space.
        /// </summary>
        public float3 Position;
    }

    public interface IPointCloudPoint { }

    /// <summary>
    /// Point type: Position double3.
    /// </summary>
    public struct Pos64 : IPointCloudPoint
    {
        /// <summary>
        /// The points coordinate in 3D space.
        /// </summary>
        public double3 Position;
    }

    /// <summary>
    /// Point type: Position, color, intensity.
    /// </summary>
    public struct Pos64Col32IShort : IPointCloudPoint
    {
        /// <summary>
        /// The points coordinate in 3D space.
        /// </summary>
        public double3 Position;
        /// <summary>
        /// The points rgb color.
        /// </summary>
        public float3 Color;
        /// <summary>
        /// The points intensity (gray scale).
        /// </summary>
        public ushort Intensity;
    }

    /// <summary>
    /// Point type: Position, intensity.
    /// </summary>
    public struct Pos64IShort : IPointCloudPoint
    {
        /// <summary>
        /// The points coordinate in 3D space.
        /// </summary>
        public double3 Position;
        /// <summary>
        /// The points intensity (gray scale).
        /// </summary>
        public ushort Intensity;
    }

    /// <summary>
    /// Point type: Position, color.
    /// </summary>
    public struct Pos64Col32 : IPointCloudPoint
    {
        /// <summary>
        /// The point's coordinate in 3D space.
        /// </summary>
        public double3 Position;
        /// <summary>
        /// The point's rgb color.
        /// </summary>
        public float3 Color;
    }

    /// <summary>
    /// Point type: Position and label color.
    /// </summary>
    public struct Pos64Label8 : IPointCloudPoint
    {
        /// <summary>
        /// The point's coordinate in 3D space.
        /// </summary>
        public double3 Position;
        /// <summary>
        /// The point's struct label.
        /// </summary>
        public byte Label;
    }

    /// <summary>
    /// Point type: Position, normal, color, intensity.
    /// </summary>
    public struct Pos64Nor32Col32IShort : IPointCloudPoint
    {
        /// <summary>
        /// The point's coordinate in 3D space.
        /// </summary>
        public double3 Position;
        /// <summary>
        /// The point's normal vector.
        /// </summary>
        public float3 Normal;
        /// <summary>
        /// The point's rgb color.
        /// </summary>
        public float3 Color;
        /// <summary>
        /// The point's intensity (gray scale).
        /// </summary>
        public ushort Intensity;
    }

    /// <summary>
    /// Point type: Position, normal, intensity.
    /// </summary>
    public struct Pos64Nor32IShort : IPointCloudPoint
    {
        /// <summary>
        /// The point's coordinate in 3D space.
        /// </summary>
        public double3 Position;
        /// <summary>
        /// The point's normal vector.
        /// </summary>
        public float3 Normal;
        /// <summary>
        /// The point's intensity (gray scale).
        /// </summary>
        public ushort Intensity;
    }

    /// <summary>
    /// Point type: Position, normal, color.
    /// </summary>
    public struct Pos64Nor32Col32 : IPointCloudPoint
    {
        /// <summary>
        /// The point's coordinate in 3D space.
        /// </summary>
        public double3 Position;
        /// <summary>
        /// The point's normal vector.
        /// </summary>
        public float3 Normal;
        /// <summary>
        /// The point's rgb color.
        /// </summary>
        public float3 Color;
    }
}