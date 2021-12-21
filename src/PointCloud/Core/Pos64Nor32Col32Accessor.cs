using Fusee.Math.Core;
using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position, color and intensity values.
    /// </summary>
    public class Pos64Nor32Col32Accessor : PointAccessor<Pos64Nor32Col32>
    {
        public Pos64Nor32Col32Accessor()
        {
            PositionType = PointPositionType.Float3_64;
            ColorType = PointColorType.Float3_32;
            NormalType = PointNormalType.Float3_32;
        }

        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public override void SetColorFloat3_32(ref Pos64Nor32Col32 point, float3 val)
        {
            point.Color = val;
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetColorFloat3_32(ref Pos64Nor32Col32 point)
        {
            return ref point.Color;
        }

        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64Nor32Col32 point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64Nor32Col32 point)
        {
            return ref point.Position;
        }
        /// <summary>
        /// Returns the normal vector of a point cloud point if <see cref="PointNormalType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetNormalFloat3_32(ref Pos64Nor32Col32 point)
        {
            return ref point.Normal;
        }
        /// <summary>
        /// Sets the normal vector of a point cloud point if <see cref="PointNormalType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new normal vector.</param>
        public override void SetNormalFloat3_32(ref Pos64Nor32Col32 point, float3 val)
        {
            point.Normal = val;
        }
    }
}