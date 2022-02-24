using Fusee.Math.Core;
using Fusee.PointCloud.Common.Accessors;

namespace Fusee.PointCloud.Core.Accessors
{
    //Collection of PointAccessor classes. There has to be one for each PointType.

    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position information only.
    /// </summary>
    public class PosD3Accessor : PointAccessor<PosD3>
    {
        public PosD3Accessor()
        {
            PositionType = PointPositionType.Double3;
        }

        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref PosD3 point, double3 val)
        {
            point.Position = val;
        }

        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref PosD3 point)
        {
            return ref point.Position;
        }
    }
}