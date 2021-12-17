using Fusee.Math.Core;

namespace Fusee.PointCloud.Common
{
    public enum PointPositionType
    {
        /// <summary>
        /// The position of this point is undefined - renders the point unusable.
        /// </summary>
        Undefined,

        /// <summary>
        /// A point cloud point has a position value of type <see cref="float3"/>.
        /// </summary>
        Float3_32,
        /// <summary>
        /// A point cloud point has a position value of type <see cref="double3"/>.
        /// </summary>
        Float3_64
    }

    public enum PointNormalType
    {
        /// <summary>
        /// A point cloud point without an normal.
        /// </summary>
        None,

        /// <summary>
        /// A point cloud point has a position value of type <see cref="float3"/>.
        /// </summary>
        Float3_32,
        /// <summary>
        /// A point cloud point has a position value of type <see cref="double3"/>.
        /// </summary>
        Float3_64
    }

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

    public enum PointColorType
    {
        /// <summary>
        /// A point cloud point without a color value.
        /// </summary>
        None,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="sbyte"/>.
        /// </summary>
        Int_8,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="short"/>.
        /// </summary>
        Int_16,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="int"/>.
        /// </summary>
        Int_32,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="long"/>.
        /// </summary>
        Int_64,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="byte"/>.
        /// </summary>
        UInt_8,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="ushort"/>.
        /// </summary>
        UInt_16,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="uint"/>.
        /// </summary>
        UInt_32,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="ulong"/>.
        /// </summary>
        UInt_64,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="float"/>.
        /// </summary>
        Float32,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="double"/>.
        /// </summary>
        Float64,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="float3"/>.
        /// </summary>
        Float3_32,
        /// <summary>
        /// A point cloud point has a color value of type <see cref="double3"/>.
        /// </summary>
        Float3_64
    }

    public enum PointLabelType
    {
        /// <summary>
        /// A point cloud without a label.
        /// </summary>
        None,
        /// <summary>
        /// A point cloud point has a label with a value of type <see cref="sbyte"/>.
        /// </summary>
        Int_8,
        /// <summary>
        /// A point cloud point has a label with a value of type <see cref="short"/>.
        /// </summary>
        Int_16,
        /// <summary>
        /// A point cloud point has a label with a value of type <see cref="int"/>.
        /// </summary>
        Int_32,
        /// <summary>
        /// A point cloud point has a label with a value of type <see cref="long"/>.
        /// </summary>
        Int_64,
        /// <summary>
        /// A point cloud point has a label with a value of type <see cref="byte"/>.
        /// </summary>
        UInt_8,
        /// <summary>
        /// A point cloud point has a label with a value of type <see cref="ushort"/>.
        /// </summary>
        UInt_16,
        /// <summary>
        /// A point cloud point has a label with a value of type <see cref="uint"/>.
        /// </summary>
        UInt_32,
        /// <summary>
        /// A point cloud point has a label with a value of type <see cref="ulong"/>.
        /// </summary>
        UInt_64,
        /// <summary>
        /// A point cloud point has a label with a value of type <see cref="float"/>.
        /// </summary>
        Float32,
        /// <summary>
        /// A point cloud point has a label with a value of type <see cref="double"/>.
        /// </summary>
        Float64,
    }

    public enum PointCurvatureType
    {
        /// <summary>
        /// A point cloud without a curvature.
        /// </summary>
        None,
        /// <summary>
        /// A point cloud point has a curvature value of type <see cref="sbyte"/>.
        /// </summary>
        Int_8,
        /// <summary>
        /// A point cloud point has a curvature value of type <see cref="short"/>.
        /// </summary>
        Int_16,
        /// <summary>
        /// A point cloud point has a curvature value of type <see cref="int"/>.
        /// </summary>
        Int_32,
        /// <summary>
        /// A point cloud point has a curvature value of type <see cref="long"/>.
        /// </summary>
        Int_64,
        /// <summary>
        /// A point cloud point has a curvature value of type <see cref="byte"/>.
        /// </summary>
        UInt_8,
        /// <summary>
        /// A point cloud point has a curvature value of type <see cref="ushort"/>.
        /// </summary>
        UInt_16,
        /// <summary>
        /// A point cloud point has a curvature value of type <see cref="uint"/>.
        /// </summary>
        UInt_32,
        /// <summary>
        /// A point cloud point has a curvature value of type <see cref="ulong"/>.
        /// </summary>
        UInt_64,
        /// <summary>
        /// A point cloud point has a curvature value of type <see cref="float"/>.
        /// </summary>
        Float32,
        /// <summary>
        /// A point cloud point has a curvature value of type <see cref="double"/>.
        /// </summary>
        Float64,
    }

    public enum PointHitCountType
    {
        /// <summary>
        /// A point cloud point has a label without a hit count.
        /// </summary>
        None,
        /// <summary>
        /// A point cloud point has a hit count value of type <see cref="sbyte"/>.
        /// </summary>
        Int_8,
        /// <summary>
        /// A point cloud point has a hit count value of type <see cref="short"/>.
        /// </summary>
        Int_16,
        /// <summary>
        /// A point cloud point has a hit count value of type <see cref="int"/>.
        /// </summary>
        Int_32,
        /// <summary>
        /// A point cloud point has a hit count value of type <see cref="long"/>.
        /// </summary>
        Int_64,
        /// <summary>
        /// A point cloud point has a hit count value of type <see cref="byte"/>.
        /// </summary>
        UInt_8,
        /// <summary>
        /// A point cloud point has a hit count value of type <see cref="ushort"/>.
        /// </summary>
        UInt_16,
        /// <summary>
        /// A point cloud point has a hit count value of type <see cref="uint"/>.
        /// </summary>
        UInt_32,
        /// <summary>
        /// A point cloud point has a hit count value of type <see cref="ulong"/>.
        /// </summary>
        UInt_64,
        /// <summary>
        /// A point cloud point has a hit count value of type <see cref="float"/>.
        /// </summary>
        Float32,
        /// <summary>
        /// A point cloud point has a hit count value of type <see cref="double"/>.
        /// </summary>
        Float64,
    }

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

    public interface IPointAccessor
    {
        /// <summary>
        /// Returns the point type this accessor can use.
        /// </summary>
        /// <returns></returns>
        PointType PointType { get; }

        PointPositionType PositionType { get; }

        PointIntensityType IntensityType { get;}

        PointNormalType NormalType { get; }

        PointColorType ColorType { get;}

        PointLabelType LabelType { get;}

        PointCurvatureType CurvatureType { get;}

        PointHitCountType HitCountType { get;}

        PointGpsTimeType GpsTimeType { get;}
    }
}