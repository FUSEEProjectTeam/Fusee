namespace Fusee.PointCloud.Common.Accessors
{
    /// <summary>
    /// Every point cloud needs a point accessor. Provides access to the point parameters like position or color.
    /// </summary>
    public interface IPointAccessor
    {
        /// <summary>
        /// Returns the point type this accessor can use.
        /// </summary>
        PointType PointType { get; }

        /// <summary>
        /// Data type of the position values.
        /// </summary>
        PointPositionType PositionType { get; }

        /// <summary>
        /// Data type of the intensity values.
        /// </summary>
        PointIntensityType IntensityType { get; }

        /// <summary>
        /// Data type of the normal vectors.
        /// </summary>
        PointNormalType NormalType { get; }

        /// <summary>
        /// Data type of the color values.
        /// </summary>
        PointColorType ColorType { get; }

        /// <summary>
        /// Data type of the label values.
        /// </summary>
        PointLabelType LabelType { get; }

        /// <summary>
        /// Data type of the curvature values.
        /// </summary>
        PointCurvatureType CurvatureType { get; }

        /// <summary>
        /// Data type of the hit count values.
        /// </summary>
        PointHitCountType HitCountType { get; }

        /// <summary>
        /// Data type of the gps time values.
        /// </summary>
        PointGpsTimeType GpsTimeType { get; }
    }
}