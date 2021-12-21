namespace Fusee.PointCloud.Common
{
    public enum PointGpsTimeType
    {
        /// <summary>
        /// A point cloud point without gps time.
        /// </summary>
        None,        
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="sbyte"/>.
        /// </summary>
        Int_8,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="short"/>.
        /// </summary>
        Int_16,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="int"/>.
        /// </summary>
        Int_32,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="long"/>.
        /// </summary>
        Int_64,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="byte"/>.
        /// </summary>
        UInt_8,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="ushort"/>.
        /// </summary>
        UInt_16,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="uint"/>.
        /// </summary>
        UInt_32,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="ulong"/>.
        /// </summary>
        UInt_64,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="float"/>.
        /// </summary>
        Float32,
        /// <summary>
        /// A point cloud point has a gps time value of type <see cref="double"/>.
        /// </summary>
        Float64
    }
}