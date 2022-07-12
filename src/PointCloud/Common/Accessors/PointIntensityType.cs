namespace Fusee.PointCloud.Common.Accessors
{
    /// <summary>
    /// Declares valid data types for a point cloud's intensity data.
    /// </summary>
    public enum PointIntensityType
    {
        /// <summary>
        /// A point cloud point without an intensity value.
        /// </summary>
        None,

        /// <summary>
        /// A point cloud point has a intensity value of type <see cref="sbyte"/>.
        /// </summary>
        SByte,

        /// <summary>
        ///A point cloud point has a intensity value of type <see cref="short"/>.
        /// </summary>
        Short,

        /// <summary>
        /// A point cloud point has a intensity value of type <see cref="int"/>.
        /// </summary>
        Int,

        /// <summary>
        /// A point cloud point has a intensity value of type <see cref="long"/>.
        /// </summary>
        Long,

        /// <summary>
        /// A point cloud point has a intensity value of type <see cref="byte"/>.
        /// </summary>
        Byte,

        /// <summary>
        /// A point cloud point has a intensity value of type <see cref="ushort"/>.
        /// </summary>
        UShort,

        /// <summary>
        /// A point cloud point has a intensity value of type <see cref="uint"/>.
        /// </summary>
        UInt,

        /// <summary>
        /// Returns a bool that tells if a point cloud point has a intensity value of type <see cref="ulong"/>.
        /// </summary>
        ULong,

        /// <summary>
        /// Returns a bool that tells if a point cloud point has a intensity value of type <see cref="float"/>.
        /// </summary>
        Float,

        /// <summary>
        /// Returns a bool that tells if a point cloud point has a intensity value of type <see cref="double"/>.
        /// </summary>
        Double
    }
}