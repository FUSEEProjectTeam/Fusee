using Fusee.Math.Core;
using Fusee.PointCloud.Common.Accessors;

namespace Fusee.PointCloud.Core.Accessors
{
    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position and label values.
    /// </summary>
    public class PosD3LblBAccessor : PointAccessor<PosD3LblB>
    {
        public PosD3LblBAccessor()
        {
            PositionType = PointPositionType.Double3;
            LabelType = PointLabelType.UInt_8;
        }

        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public override void SetLabelUInt_8(ref PosD3LblB point, byte val)
        {
            point.Label = val;
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref byte GetLabelUInt_8(ref PosD3LblB point)
        {
            return ref point.Label;
        }
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref PosD3LblB point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref PosD3LblB point)
        {
            return ref point.Position;
        }
    }
}