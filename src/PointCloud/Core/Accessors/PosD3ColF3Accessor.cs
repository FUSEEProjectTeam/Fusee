using Fusee.Math.Core;
using Fusee.PointCloud.Common.Accessors;

namespace Fusee.PointCloud.Core.Accessors
{
    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position and color values.
    /// </summary>
    public class PosD3ColF3Accessor : PointAccessor<PosD3ColF3>
    {
        public PosD3ColF3Accessor()
        {
            PositionType = PointPositionType.Double3;
            ColorType = PointColorType.Float3;
        }

        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public override void SetColorFloat3_32(ref PosD3ColF3 point, float3 val)
        {
            point.Color = val;
        }

        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetColorFloat3_32(ref PosD3ColF3 point)
        {
            return ref point.Color;
        }
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref PosD3ColF3 point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref PosD3ColF3 point)
        {
            return ref point.Position;
        }
    }
}