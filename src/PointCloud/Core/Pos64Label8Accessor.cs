using Fusee.Math.Core;
using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position and label values.
    /// </summary>
    public class Pos64Label8Accessor : PointAccessor<Pos64Label8>
    {
        public Pos64Label8Accessor()
        {
            PositionType = PointPositionType.Float3_64;
            LabelType = PointLabelType.UInt_8;
        }

        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public override void SetLabelUInt_8(ref Pos64Label8 point, byte val)
        {
            point.Label = val;
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref byte GetLabelUInt_8(ref Pos64Label8 point)
        {
            return ref point.Label;
        }
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64Label8 point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64Label8 point)
        {
            return ref point.Position;
        }
    }
}