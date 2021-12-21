using Fusee.Math.Core;
using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.Core
{
    //Collection of PointAccessor classes. There has to be one for each PointType.

    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position information only.
    /// </summary>
    public class Pos64Accessor : PointAccessor<Pos64>
    {
        public Pos64Accessor()
        {
            PositionType = PointPositionType.Float3_64;
        }

        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64 point, double3 val)
        {
            point.Position = val;
        }

        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64 point)
        {
            return ref point.Position;
        }
    }
}