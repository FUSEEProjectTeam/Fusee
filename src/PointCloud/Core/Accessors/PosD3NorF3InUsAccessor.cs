using Fusee.Math.Core;
using Fusee.PointCloud.Common.Accessors;

namespace Fusee.PointCloud.Core.Accessors
{
    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position, normal vectors and intensity values.
    /// </summary>
    public class PosD3NorF3InUsAccessor : PointAccessor<PosD3NorF3InUs>
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public PosD3NorF3InUsAccessor()
        {
            PositionType = PointPositionType.Double3;
            IntensityType = PointIntensityType.UShort;
            NormalType = PointNormalType.Double3;
        }

        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref PosD3NorF3InUs point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref PosD3NorF3InUs point)
        {
            return ref point.Position;
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.UShort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref ushort GetIntensityUInt_16(ref PosD3NorF3InUs point)
        {
            return ref point.Intensity;
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.UShort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public override void SetIntensityUInt_16(ref PosD3NorF3InUs point, ushort val)
        {
            point.Intensity = val;
        }
        /// <summary>
        /// Returns the normal vector of a point cloud point if <see cref="PointNormalType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetNormalFloat3_32(ref PosD3NorF3InUs point)
        {
            return ref point.Normal;
        }
        /// <summary>
        /// Sets the normal vector of a point cloud point if <see cref="PointNormalType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new normal vector.</param>
        public override void SetNormalFloat3_32(ref PosD3NorF3InUs point, float3 val)
        {
            point.Normal = val;
        }
    }
}