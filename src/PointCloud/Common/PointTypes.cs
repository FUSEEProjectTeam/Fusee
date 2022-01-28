
using Fusee.Math.Core;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Enum that contains all available point types.
    /// Abbreviations:
    /// F3: float
    /// D3: double
    /// Us: ushort
    /// B: byte
    /// Pos: position
    /// Col: Color
    /// In: Intensity
    /// Nor: Normal
    /// Lbl: Label / Classification
    /// </summary>
    public enum PointType
    {
        Undefined,

        /// <summary>
        /// Position only (double)
        /// </summary>
        PosD3,
        /// <summary>
        /// Position (double), Color (float), Intensity (short)
        /// </summary>
        PosD3ColF3InUs,
        /// <summary>
        /// Position (double), Intensity (ushort)
        /// </summary>
        PosD3InUs,
        /// <summary>
        /// Position (double), Color (float)
        /// </summary>
        PosD3ColF3,
        /// <summary>
        /// Position (double), Label (byte)
        /// </summary>
        PosD3LblB,
        /// <summary>
        /// Position (double), Normal (float), Color (float), Intensity (ushort)
        /// </summary>
        PosD3NorF3ColF3InUs,
        /// <summary>
        /// Position (double), Normal (float), Intensity (short)
        /// </summary>
        PosD3NorF3InUs,
        /// <summary>
        /// Position (double), Normal (float), Color (float)
        /// </summary>
        PosD3NorF3ColF3,
        /// <summary>
        /// Position (double), Color (float), Classification (byte)
        /// </summary>
        PosD3ColF3LblB,
        /// <summary>
        /// Position (double), Color (float), Classification (byte), Intensity (ushort)
        /// </summary>
        PosD3ColF3InUsLblB
    }

    /// <summary>
    /// Point type: Position double3.
    /// </summary>
    public struct PosD3
    {
        /// <summary>
        /// The points coordinate in 3D space.
        /// </summary>
        public double3 Position;
    }

    /// <summary>
    /// Point type: Position, color, intensity.
    /// </summary>
    public struct PosD3ColF3InUs
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
    public struct PosD3InUs
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
    public struct PosD3ColF3
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
    public struct PosD3LblB
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
    public struct PosD3NorF3ColF3InUs
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

    //

    /// <summary>
    /// Point type: Position, normal, intensity.
    /// </summary>
    public struct PosD3NorF3InUs
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
    /// Point type: <see cref="Position"/>, <see cref="Color"/>, <see cref="Normal"/>.
    /// </summary>
    public struct PosD3NorF3ColF3
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


    /// <summary>
    /// Point type: <see cref="Position"/>, <see cref="Color"/>, <see cref="Label"/>.
    /// </summary>
    public struct PosD3ColF3LblB
    {
        /// <summary>
        /// The point's coordinate in 3D space.
        /// </summary>
        public double3 Position;
        /// <summary>
        /// The point's rgb color.
        /// </summary>
        public float3 Color;
        /// <summary>
        /// The point's classification.
        /// </summary>
        public byte Label;
    }

    /// <summary>
    /// Point type: <see cref="Position"/>, <see cref="Color"/>, <see cref="Label"/> and <see cref="Intensity"/>.
    /// </summary>
    public struct PosD3ColF3InUsLblB
    {
        /// <summary>
        /// The point's coordinate in 3D space.
        /// </summary>
        public double3 Position;
        /// <summary>
        /// The point's rgb color.
        /// </summary>
        public float3 Color;
        /// <summary>
        /// The point's classification.
        /// </summary>
        public byte Label;
        /// <summary>
        /// The point's intensity (gray scale).
        /// </summary>
        public ushort Intensity;
    }
}