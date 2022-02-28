using Fusee.Math.Core;

namespace Fusee.PointCloud.Common.Accessors
{
    /// <summary>
    /// Declares valid data types for a point cloud's per point color data.
    /// </summary>
    public enum PointColorType
    {
        /// <summary>
        /// A point cloud point without a color value.
        /// </summary>
        None,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="sbyte"/>.
        /// </summary>
        SByte,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="short"/>.
        /// </summary>
        Short,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="int"/>.
        /// </summary>
        Int,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="long"/>.
        /// </summary>
        Long,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="byte"/>.
        /// </summary>
        Byte,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="ushort"/>.
        /// </summary>
        Ushort,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="uint"/>.
        /// </summary>
        Uint,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="ulong"/>.
        /// </summary>
        Ulong,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="float"/>.
        /// </summary>
        Float,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="double"/>.
        /// </summary>
        Double,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="float3"/>.
        /// </summary>
        Float3,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="double3"/>.
        /// </summary>
        Double3
    }
}