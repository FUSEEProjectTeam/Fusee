using Fusee.Math.Core;
using Fusee.PointCloud.Common.Accessors;

namespace Fusee.PointCloud.Core.Accessors
{
    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position, color and classification values.
    /// </summary>
    public class PosD3ColF3LblBAccessor : PointAccessor<PosD3ColF3LblB>
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public PosD3ColF3LblBAccessor()
        {
            PositionType = PointPositionType.Double3;
            ColorType = PointColorType.Float;
            LabelType = PointLabelType.Byte;
        }

        /// <summary>
        /// Sets the color of a point cloud point.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public override void SetColorFloat3_32(ref PosD3ColF3LblB point, float3 val)
        {
            point.Color = val;
        }
        /// <summary>
        /// Returns the normal color of a point cloud point.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetColorFloat3_32(ref PosD3ColF3LblB point)
        {
            return ref point.Color;
        }

        /// <summary>
        /// Sets the position of a point cloud point.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref PosD3ColF3LblB point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref PosD3ColF3LblB point)
        {
            return ref point.Position;
        }
        /// <summary>
        /// Sets the label of a point cloud point.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public override void SetLabelUInt_8(ref PosD3ColF3LblB point, byte val)
        {
            point.Label = val;
        }
        /// <summary>
        /// Returns the label of a point cloud point.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref byte GetLabelUInt_8(ref PosD3ColF3LblB point)
        {
            return ref point.Label;
        }
    }
}
