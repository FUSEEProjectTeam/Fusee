using Fusee.Math.Core;
using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position and color values.
    /// </summary>
    public class Pos64Col32Accessor : PointAccessor<Pos64Col32>
    {
        public Pos64Col32Accessor()
        {
            PositionType = PointPositionType.Float3_64;
            ColorType = PointColorType.Float3_32;
        }

        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public override void SetColorFloat3_32(ref Pos64Col32 point, float3 val)
        {
            point.Color = val;
        }

        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetColorFloat3_32(ref Pos64Col32 point)
        {
            return ref point.Color;
        }
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64Col32 point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64Col32 point)
        {
            return ref point.Position;
        }
    }
}