namespace Fusee.PointCloud.Common.Accessors
{
    public enum PointIntensityType
    {
        /// <summary>
        /// A point cloud point without an intensity value.
        /// </summary>
        None,

        /// <summary>
        /// A point cloud point has a intensity value of type <see cref="sbyte"/>.
        /// </summary>
        Int_8,

        /// <summary>
        ///A point cloud point has a intensity value of type <see cref="short"/>.
        /// </summary>
        Int_16,

        /// <summary>
        /// A point cloud point has a intensity value of type <see cref="int"/>.
        /// </summary>
        Int_32,

        /// <summary>
        /// A point cloud point has a intensity value of type <see cref="long"/>.
        /// </summary>
        Int_64,

        /// <summary>
        /// A point cloud point has a intensity value of type <see cref="byte"/>.
        /// </summary>
        UInt_8,

        /// <summary>
        /// A point cloud point has a intensity value of type <see cref="ushort"/>.
        /// </summary>
        UInt_16,

        /// <summary>
        /// A point cloud point has a intensity value of type <see cref="uint"/>.
        /// </summary>
        UInt_32,

        /// <summary>
        /// Returns a bool that tells if a point cloud point has a intensity value of type <see cref="ulong"/>.
        /// </summary>
        UInt_64,

        /// <summary>
        /// Returns a bool that tells if a point cloud point has a intensity value of type <see cref="float"/>.
        /// </summary>
        Float32,

        /// <summary>
        /// Returns a bool that tells if a point cloud point has a intensity value of type <see cref="double"/>.
        /// </summary>
        Float64
    }
}