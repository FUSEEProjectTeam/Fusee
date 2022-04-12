namespace Fusee.PointCloud.Common.Accessors
{
    /// <summary>
    /// Declares valid data types for a point cloud's gps time data.
    /// </summary>
    public enum PointGpsTimeType
    {
        /// <summary>
        /// A point cloud point without gps time.
        /// </summary>
        None,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="sbyte"/>.
        /// </summary>
        SByte,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="short"/>.
        /// </summary>
        Short,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="int"/>.
        /// </summary>
        Int,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="long"/>.
        /// </summary>
        Long,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="byte"/>.
        /// </summary>
        Byte,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="ushort"/>.
        /// </summary>
        UShort,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="uint"/>.
        /// </summary>
        Uint,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="ulong"/>.
        /// </summary>
        ULong,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="float"/>.
        /// </summary>
        Float,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="double"/>.
        /// </summary>
        Double
    }
}