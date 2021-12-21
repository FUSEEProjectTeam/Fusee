﻿using Fusee.Math.Core;
using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// <see cref="PointAccessor{TPoint}"/> for Point Clouds which position, normal vectors and intensity values.
    /// </summary>
    public class Pos64Nor32IShortAccessor : PointAccessor<Pos64Nor32IShort>
    {
        public Pos64Nor32IShortAccessor()
        {
            PositionType = PointPositionType.Float3_64;
            IntensityType = PointIntensityType.UInt_16;
            NormalType = PointNormalType.Float3_64;
        }
        
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public override void SetPositionFloat3_64(ref Pos64Nor32IShort point, double3 val)
        {
            point.Position = val;
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref double3 GetPositionFloat3_64(ref Pos64Nor32IShort point)
        {
            return ref point.Position;
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref ushort GetIntensityUInt_16(ref Pos64Nor32IShort point)
        {
            return ref point.Intensity;
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public override void SetIntensityUInt_16(ref Pos64Nor32IShort point, ushort val)
        {
            point.Intensity = val;
        }
        /// <summary>
        /// Returns the normal vector of a point cloud point if <see cref="PointNormalType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public override ref float3 GetNormalFloat3_32(ref Pos64Nor32IShort point)
        {
            return ref point.Normal;
        }
        /// <summary>
        /// Sets the normal vector of a point cloud point if <see cref="PointNormalType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new normal vector.</param>
        public override void SetNormalFloat3_32(ref Pos64Nor32IShort point, float3 val)
        {
            point.Normal = val;
        }
    }
}