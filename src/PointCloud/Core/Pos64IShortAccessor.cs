using Fusee.Math.Core;
using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position and intensity values.
    /// </summary>
    public class Pos64IShortAccessor : PointAccessor<Pos64IShort>
    {
        public Pos64IShortAccessor()
        {
            PositionType = PointPositionType.Float3_64;
            IntensityType = PointIntensityType.UInt_16;
        }

        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64IShort point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64IShort point)
        {
            return ref point.Position;
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref ushort GetIntensityUInt_16(ref Pos64IShort point)
        {
            return ref point.Intensity;
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public override void SetIntensityUInt_16(ref Pos64IShort point, ushort val)
        {
            point.Intensity = val;
        }
    }
}