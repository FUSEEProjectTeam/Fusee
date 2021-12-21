namespace Fusee.PointCloud.Common
{
    public interface IPointAccessor
    {
        /// <summary>
        /// Returns the point type this accessor can use.
        /// </summary>
        /// <returns></returns>
        PointType PointType { get; }

        PointPositionType PositionType { get; }

        PointIntensityType IntensityType { get; }

        PointNormalType NormalType { get; }

        PointColorType ColorType { get; }

        PointLabelType LabelType { get; }

        PointCurvatureType CurvatureType { get; }

        PointHitCountType HitCountType { get; }

        PointGpsTimeType GpsTimeType { get; }
    }
}