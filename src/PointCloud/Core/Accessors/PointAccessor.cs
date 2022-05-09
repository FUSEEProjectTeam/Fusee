using Fusee.Math.Core;
using Fusee.PointCloud.Common.Accessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Fusee.PointCloud.Core.Accessors
{
    /// <summary>
    /// Every point cloud needs a point accessor. Provides access to the point parameters like position or color.
    /// </summary>
    public abstract class PointAccessor<TPoint> : IPointAccessor
    {
        /// <summary>
        /// Returns the type of the point as list of the HasXY methods.
        /// </summary>
        /// <returns></returns>
        public List<string> GetSetPropertyNames()
        {
            // Point Type (enum)
            return GetType().GetProperties().Where(p => p.PropertyType == typeof(bool) && (bool)p.GetValue(this, null)).Select(p => p.Name).ToList();
        }

        /// <summary>
        /// Returns the point type of this accessor. If it is "undefined" it will translate the type from its type properties.
        /// </summary>
        public PointType PointType
        {
            get
            {
                if (_pointType == PointType.Undefined)
                    GetPointType();

                return _pointType;
            }

        }
        private PointType _pointType = PointType.Undefined;

        private void GetPointType()
        {
            // Pos64
            _pointType = PositionType switch
            {
                PointPositionType.Double3 when IntensityType == PointIntensityType.None && NormalType == PointNormalType.None && ColorType == PointColorType.None && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None => PointType.PosD3,
                PointPositionType.Double3 when IntensityType == PointIntensityType.UShort && NormalType == PointNormalType.None && ColorType == PointColorType.Float && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None => PointType.PosD3ColF3InUs,
                PointPositionType.Double3 when IntensityType == PointIntensityType.UShort && NormalType == PointNormalType.None && ColorType == PointColorType.None && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None => PointType.PosD3InUs,
                PointPositionType.Double3 when IntensityType == PointIntensityType.None && NormalType == PointNormalType.None && ColorType == PointColorType.Float && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None => PointType.PosD3ColF3,
                PointPositionType.Double3 when IntensityType == PointIntensityType.None && NormalType == PointNormalType.None && ColorType == PointColorType.None && LabelType == PointLabelType.Byte && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None => PointType.PosD3LblB,
                PointPositionType.Double3 when IntensityType == PointIntensityType.UShort && NormalType == PointNormalType.Float3 && ColorType == PointColorType.Float && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None => PointType.PosD3NorF3ColF3InUs,
                PointPositionType.Double3 when IntensityType == PointIntensityType.UShort && NormalType == PointNormalType.Float3 && ColorType == PointColorType.None && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None => PointType.PosD3NorF3InUs,
                PointPositionType.Double3 when IntensityType == PointIntensityType.None && NormalType == PointNormalType.Float3 && ColorType == PointColorType.Float && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None => PointType.PosD3NorF3ColF3,
                PointPositionType.Double3 when IntensityType == PointIntensityType.None && NormalType == PointNormalType.None && ColorType == PointColorType.Float && LabelType == PointLabelType.Byte && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None => PointType.PosD3ColF3LblB,
                _ => throw new Exception("Undefined Point Type!"),
            };
        }

        /// <summary>
        /// Data type of the position values.
        /// </summary>
        public PointPositionType PositionType { get; set; } = PointPositionType.Undefined;
        /// <summary>
        /// Data type of the intensity values.
        /// </summary>
        public PointIntensityType IntensityType { get; set; } = PointIntensityType.None;
        /// <summary>
        /// Data type of the normal vectors.
        /// </summary>
        public PointNormalType NormalType { get; set; } = PointNormalType.None;
        /// <summary>
        /// Data type of the color values.
        /// </summary>
        public PointColorType ColorType { get; set; } = PointColorType.None;
        /// <summary>
        /// Data type of the label values.
        /// </summary>
        public PointLabelType LabelType { get; set; } = PointLabelType.None;
        /// <summary>
        /// Data type of the curvature values.
        /// </summary>
        public PointCurvatureType CurvatureType { get; set; } = PointCurvatureType.None;
        /// <summary>
        /// Data type of the hit count values.
        /// </summary>
        public PointHitCountType HitCountType { get; set; } = PointHitCountType.None;
        /// <summary>
        /// Data type of the gps time values.
        /// </summary>
        public PointGpsTimeType GpsTimeType { get; set; } = PointGpsTimeType.None;

        /// <summary>
        /// Returns the generic raw point.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public byte[] GetRawPoint(ref TPoint point)
        {
            if (point == null)
                throw new NullReferenceException("Given point is null!");

            var position = GetRawPosition(ref point);
            var intensity = GetRawIntensity(ref point);
            var normals = GetRawNormals(ref point);
            var rgb = GetRawColor(ref point);
            var label = GetRawLabel(ref point);
            var curvature = GetRawCurvature(ref point);
            var hitCount = GetRawHitCount(ref point);
            var GPSTime = GetRawGPSTime(ref point);

            return position.Concat(intensity).Concat(normals).Concat(rgb).Concat(label).Concat(curvature).Concat(hitCount).Concat(GPSTime).ToArray();
        }

        /// <summary>
        /// Sets the values of a point cloud point.
        /// </summary>
        /// <param name="pointIn">The generic point.</param>
        /// <param name="byteIn">The values as byte array.</param>
        public void SetRawPoint(ref TPoint pointIn, byte[] byteIn)
        {
            if (pointIn == null || byteIn == null || byteIn.Length < 8)
                throw new NullReferenceException("Invalid data given");

            // Call all methods to recreate the point
            SetRawPosition(ref pointIn, byteIn);
            SetRawIntensity(ref pointIn, byteIn);
            SetRawNormals(ref pointIn, byteIn);
            SetRawColor(ref pointIn, byteIn);
            SetRawLabel(ref pointIn, byteIn);
            SetRawCurvature(ref pointIn, byteIn);
            SetRawHitCount(ref pointIn, byteIn);
            SetRawGPSTime(ref pointIn, byteIn);
        }

        #region PointT_Methods

        #region Get/Set Position
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float3 GetPositionFloat3_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetPositionFloat32");
        }
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public virtual void SetPositionFloat3_32(ref TPoint point, float3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetPositionFloat32");
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double3 GetPositionFloat3_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetPositionFloat64");
        }
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public virtual void SetPositionFloat3_64(ref TPoint point, double3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetPositionFloat64");
        }
        #endregion

        #region Get/Set Intensity

        #region Getter

        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.SByte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref sbyte GetIntensityInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityInt_8");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.Short"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref short GetIntensityInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityInt_16");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.Int"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref int GetIntensityInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityInt_32");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref long GetIntensityInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityInt_64");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.Byte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref byte GetIntensityUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityUInt_8");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.UShort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ushort GetIntensityUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityUInt_16");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.UInt"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref uint GetIntensityUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityUInt_32");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.ULong"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ulong GetIntensityUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityUInt_64");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.Float"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float GetIntensityFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityFloat32");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.Double"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double GetIntensityFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityFloat64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.SByte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityInt_8");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.Short"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityInt_16");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.Int"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityInt_32");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityInt_64");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.Byte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityUInt_8");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.UShort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityUInt_16");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.UInt"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityUInt_32");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.ULong"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityUInt_64");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.Float"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityFloat32");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.Double"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityFloat64(ref TPoint point, double val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityFloat64");
        }
        #endregion

        #endregion

        #region Get/Set Normal

        #region Getter

        /// <summary>
        /// Returns the normal vector of a point cloud point if <see cref="PointNormalType.Float3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float3 GetNormalFloat3_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetNormalFloat3_32");
        }

        /// <summary>
        /// Returns the normal vector of a point cloud point if <see cref="PointNormalType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double3 GetNormalFloat3_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetNormalFloat3_64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the normal vector of a point cloud point if <see cref="PointNormalType.Float3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new normal vector.</param>
        public virtual void SetNormalFloat3_32(ref TPoint point, float3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetNormalFloat3_32");
        }
        /// <summary>
        /// Sets the normal vector of a point cloud point if <see cref="PointNormalType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new normal vector.</param>
        public virtual void SetNormalFloat3_64(ref TPoint point, double3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetNormalFloat3_64");
        }
        #endregion

        #endregion

        #region Get/Set Color

        #region Getter
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.SByte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref sbyte GetColorInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorInt_8");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Short"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref short GetColorInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorInt_16");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Int"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref int GetColorInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorInt_32");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref long GetColorInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorInt_64");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Byte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref byte GetColorUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorUInt_8");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Ushort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ushort GetColorUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorUInt_16");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Uint"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref uint GetColorUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorUInt_32");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Ulong"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ulong GetColorUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorUInt_64");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Float"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float GetColorFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorFloat32");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Double"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double GetColorFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorFloat64");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Float3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float3 GetColorFloat3_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorFloat3_32");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double3 GetColorFloat3_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorFloat3_64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.SByte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorInt_8");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Short"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorInt_16");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Int"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorInt_32");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorInt_64");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Byte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorUInt_8");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Ushort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorUInt_16");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Uint"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorUInt_32");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Ulong"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorUInt_64");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Float"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorFloat32");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Double"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorFloat64(ref TPoint point, double val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorFloat64");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Float3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorFloat3_32(ref TPoint point, float3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorFloat3_32");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Double3"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorFloat3_64(ref TPoint point, double3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorFloat3_64");
        }
        #endregion

        #endregion

        #region Get/Set Label

        #region Getter
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.SByte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref sbyte GetLabelInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelInt_8");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Short"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref short GetLabelInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelInt_16");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Int"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref int GetLabelInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelInt_32");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref long GetLabelInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelInt_64");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Byte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref byte GetLabelUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelUInt_8");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.UShort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ushort GetLabelUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelUInt_16");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.UInt"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref uint GetLabelUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelUInt_32");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ulong GetLabelUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelUInt_64");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Float"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float GetLabelFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelFloat32");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Double"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double GetLabelFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelFloat64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.SByte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelInt_8");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.SByte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelInt_16");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.Int"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelInt_32");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelInt_64");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.Byte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelUInt_8");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.UShort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelUInt_16");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.UInt"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelUInt_32");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.ULong"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelUInt_64");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.Float"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelFloat32");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.Double"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelFloat64(ref TPoint point, double val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelFloat64");
        }
        #endregion

        #endregion

        #region Get/Set Curvature

        #region Getter
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.SByte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref sbyte GetCurvatureInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureInt_8");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.Short"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref short GetCurvatureInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureInt_16");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.Int"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref int GetCurvatureInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureInt_32");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref long GetCurvatureInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureInt_64");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.Byte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref byte GetCurvatureUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureUInt_8");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.UShort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ushort GetCurvatureUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureUInt_16");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.Uint"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref uint GetCurvatureUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureUInt_32");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.ULong"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ulong GetCurvatureUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureUInt_64");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.Float"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float GetCurvatureFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureFloat32");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.Double"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double GetCurvatureFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureFloat64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.SByte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureInt_8");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.Short"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureInt_16");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.Int"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureInt_32");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureInt_64");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.Byte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureUInt_8");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.UShort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureUInt_16");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.Uint"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureUInt_32");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.ULong"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureUInt_64");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.Float"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureFloat32");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.Double"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureFloat64(ref TPoint point, double val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureFloat64");
        }
        #endregion

        #endregion

        #region Get/Set Hit Count

        #region Getter
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.SByte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref sbyte GetHitCountInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountInt_8");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Short"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref short GetHitCountInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountInt_16");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Int"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref int GetHitCountInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountInt_32");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref long GetHitCountInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountInt_64");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Byte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref byte GetHitCountUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountUInt_8");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.UShort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ushort GetHitCountUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountUInt_16");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Int"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref uint GetHitCountUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountUInt_32");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ulong GetHitCountUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountUInt_64");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Float"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float GetHitCountFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountFloat32");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Double"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double GetHitCountFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountFloat64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.SByte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountInt_8");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.Short"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountInt_16");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.Int"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountInt_32");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountInt_64");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.Byte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountUInt_8");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.UShort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountUInt_16");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.Uint"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountUInt_32");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.ULong"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountUInt_64");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.Float"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountFloat32");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.Double"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountFloat64(ref TPoint point, double val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountFloat64");
        }
        #endregion

        #endregion

        #region Get/Set GPS Time

        #region Getter
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.SByte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref sbyte GetGPSTimeInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeInt_8");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.Short"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref short GetGPSTimeInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeInt_16");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.Int"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref int GetGPSTimeInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeInt_32");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref long GetGPSTimeInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeInt_64");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.Byte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref byte GetGPSTimeUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeUInt_8");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.UShort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ushort GetGPSTimeUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeUInt_16");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.Uint"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref uint GetGPSTimeUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeUInt_32");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.ULong"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ulong GetGPSTimeUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeUInt_64");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.Float"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float GetGPSTimeFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeFloat32");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.Double"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double GetGPSTimeFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeFloat64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.SByte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeInt_8");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.Short"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeInt_16");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.Int"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeInt_32");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.Long"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeInt_64");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.Byte"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeUInt_8");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.UShort"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeUInt_16");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.Uint"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeUInt_32");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.ULong"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeUInt_64");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.Float"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeFloat32");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.Double"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeFloat64(ref TPoint point, double val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeFloat64");
        }
        #endregion

        #endregion

        #endregion

        #region RawDataEncode
        private byte[] GetRawPosition(ref TPoint point)
        {
            if (PositionType == PointPositionType.Float3)
            {

                var x = BitConverter.GetBytes(GetPositionFloat3_32(ref point).x);
                var y = BitConverter.GetBytes(GetPositionFloat3_32(ref point).y);
                var z = BitConverter.GetBytes(GetPositionFloat3_32(ref point).z);

                return x.Concat(y).Concat(z).ToArray();

            }
            else if (PositionType == PointPositionType.Double3)
            {

                var x = BitConverter.GetBytes(GetPositionFloat3_64(ref point).x);
                var y = BitConverter.GetBytes(GetPositionFloat3_64(ref point).y);
                var z = BitConverter.GetBytes(GetPositionFloat3_64(ref point).z);

                return x.Concat(y).Concat(z).ToArray();

            }

            byte[] vs = Array.Empty<byte>();
            return vs;

        }

        private byte[] GetRawIntensity(ref TPoint point)
        {
            switch (IntensityType)
            {
                case PointIntensityType.SByte:
                    return BitConverter.GetBytes(GetIntensityInt_8(ref point));
                case PointIntensityType.Short:
                    return BitConverter.GetBytes(GetIntensityInt_16(ref point));
                case PointIntensityType.Int:
                    return BitConverter.GetBytes(GetIntensityInt_32(ref point));
                case PointIntensityType.Long:
                    return BitConverter.GetBytes(GetIntensityInt_64(ref point));
                case PointIntensityType.Byte:
                    return BitConverter.GetBytes(GetIntensityUInt_8(ref point));
                case PointIntensityType.UShort:
                    return BitConverter.GetBytes(GetIntensityUInt_16(ref point));
                case PointIntensityType.UInt:
                    return BitConverter.GetBytes(GetIntensityUInt_32(ref point));
                case PointIntensityType.ULong:
                    return BitConverter.GetBytes(GetIntensityUInt_64(ref point));
                case PointIntensityType.Float:
                    return BitConverter.GetBytes(GetIntensityFloat32(ref point));
                case PointIntensityType.Double:
                    return BitConverter.GetBytes(GetIntensityFloat64(ref point));
            }

            return Array.Empty<byte>();
        }

        private byte[] GetRawNormals(ref TPoint point)
        {

            if (NormalType == PointNormalType.Float3)
            {
                var x = BitConverter.GetBytes(GetNormalFloat3_32(ref point).x);
                var y = BitConverter.GetBytes(GetNormalFloat3_32(ref point).y);
                var z = BitConverter.GetBytes(GetNormalFloat3_32(ref point).z);

                return x.Concat(y).Concat(z).ToArray();

            }
            else if (NormalType == PointNormalType.Double3)
            {
                var x = BitConverter.GetBytes(GetNormalFloat3_64(ref point).x);
                var y = BitConverter.GetBytes(GetNormalFloat3_64(ref point).y);
                var z = BitConverter.GetBytes(GetNormalFloat3_64(ref point).z);

                return x.Concat(y).Concat(z).ToArray();

            }
            return Array.Empty<byte>();

        }

        private byte[] GetRawColor(ref TPoint point)
        {
            switch (ColorType)
            {
                case PointColorType.SByte:
                    return BitConverter.GetBytes(GetColorInt_8(ref point));
                case PointColorType.Short:
                    return BitConverter.GetBytes(GetColorInt_16(ref point));
                case PointColorType.Int:
                    return BitConverter.GetBytes(GetColorInt_32(ref point));
                case PointColorType.Long:
                    return BitConverter.GetBytes(GetColorInt_64(ref point));
                case PointColorType.Byte:
                    return BitConverter.GetBytes(GetColorUInt_8(ref point));
                case PointColorType.Ushort:
                    return BitConverter.GetBytes(GetColorUInt_16(ref point));
                case PointColorType.Uint:
                    return BitConverter.GetBytes(GetColorUInt_32(ref point));
                case PointColorType.Ulong:
                    return BitConverter.GetBytes(GetColorUInt_64(ref point));
                case PointColorType.Float:
                    return BitConverter.GetBytes(GetColorFloat32(ref point));
                case PointColorType.Double:
                    return BitConverter.GetBytes(GetColorFloat64(ref point));
            }

            if (ColorType == PointColorType.Float3)
            {

                var x = BitConverter.GetBytes(GetColorFloat3_32(ref point).x);
                var y = BitConverter.GetBytes(GetColorFloat3_32(ref point).y);
                var z = BitConverter.GetBytes(GetColorFloat3_32(ref point).z);

                return x.Concat(y).Concat(z).ToArray();

            }
            if (ColorType == PointColorType.Double3)
            {

                var x = BitConverter.GetBytes(GetColorFloat3_64(ref point).x);
                var y = BitConverter.GetBytes(GetColorFloat3_64(ref point).y);
                var z = BitConverter.GetBytes(GetColorFloat3_64(ref point).z);

                return x.Concat(y).Concat(z).ToArray();

            }
            return Array.Empty<byte>();
        }

        private byte[] GetRawLabel(ref TPoint point)
        {
            switch (LabelType)
            {
                case PointLabelType.SByte:
                    return BitConverter.GetBytes(GetLabelInt_8(ref point));
                case PointLabelType.Short:
                    return BitConverter.GetBytes(GetLabelInt_16(ref point));
                case PointLabelType.Int:
                    return BitConverter.GetBytes(GetLabelInt_32(ref point));
                case PointLabelType.Long:
                    return BitConverter.GetBytes(GetLabelInt_64(ref point));
                case PointLabelType.Byte:
                    return BitConverter.GetBytes(GetLabelUInt_8(ref point));
                case PointLabelType.UShort:
                    return BitConverter.GetBytes(GetLabelUInt_16(ref point));
                case PointLabelType.UInt:
                    return BitConverter.GetBytes(GetLabelUInt_32(ref point));
                case PointLabelType.ULong:
                    return BitConverter.GetBytes(GetLabelUInt_64(ref point));
                case PointLabelType.Float:
                    return BitConverter.GetBytes(GetLabelFloat32(ref point));
                case PointLabelType.Double:
                    return BitConverter.GetBytes(GetLabelFloat64(ref point));
            }

            return Array.Empty<byte>();
        }

        private byte[] GetRawCurvature(ref TPoint point)
        {
            switch (CurvatureType)
            {
                case PointCurvatureType.SByte:
                    return BitConverter.GetBytes(GetCurvatureInt_8(ref point));
                case PointCurvatureType.Short:
                    return BitConverter.GetBytes(GetCurvatureInt_16(ref point));
                case PointCurvatureType.Int:
                    return BitConverter.GetBytes(GetCurvatureInt_32(ref point));
                case PointCurvatureType.Long:
                    return BitConverter.GetBytes(GetCurvatureInt_64(ref point));
                case PointCurvatureType.Byte:
                    return BitConverter.GetBytes(GetCurvatureUInt_8(ref point));
                case PointCurvatureType.UShort:
                    return BitConverter.GetBytes(GetCurvatureUInt_16(ref point));
                case PointCurvatureType.Uint:
                    return BitConverter.GetBytes(GetCurvatureUInt_32(ref point));
                case PointCurvatureType.ULong:
                    return BitConverter.GetBytes(GetCurvatureUInt_64(ref point));
                case PointCurvatureType.Float:
                    return BitConverter.GetBytes(GetCurvatureFloat32(ref point));
                case PointCurvatureType.Double:
                    return BitConverter.GetBytes(GetCurvatureFloat64(ref point));
            }

            return Array.Empty<byte>();
        }

        private byte[] GetRawHitCount(ref TPoint point)
        {
            switch (HitCountType)
            {
                case PointHitCountType.SByte:
                    return BitConverter.GetBytes(GetHitCountInt_8(ref point));
                case PointHitCountType.Short:
                    return BitConverter.GetBytes(GetHitCountInt_16(ref point));
                case PointHitCountType.Int:
                    return BitConverter.GetBytes(GetHitCountInt_32(ref point));
                case PointHitCountType.Long:
                    return BitConverter.GetBytes(GetHitCountInt_64(ref point));
                case PointHitCountType.Byte:
                    return BitConverter.GetBytes(GetHitCountUInt_8(ref point));
                case PointHitCountType.UShort:
                    return BitConverter.GetBytes(GetHitCountUInt_16(ref point));
                case PointHitCountType.Uint:
                    return BitConverter.GetBytes(GetHitCountUInt_32(ref point));
                case PointHitCountType.ULong:
                    return BitConverter.GetBytes(GetHitCountUInt_64(ref point));
                case PointHitCountType.Float:
                    return BitConverter.GetBytes(GetHitCountFloat32(ref point));
                case PointHitCountType.Double:
                    return BitConverter.GetBytes(GetHitCountFloat64(ref point));
            }

            return Array.Empty<byte>();
        }

        private byte[] GetRawGPSTime(ref TPoint point)
        {
            switch (GpsTimeType)
            {
                case PointGpsTimeType.SByte:
                    return BitConverter.GetBytes(GetGPSTimeInt_8(ref point));
                case PointGpsTimeType.Short:
                    return BitConverter.GetBytes(GetGPSTimeInt_16(ref point));
                case PointGpsTimeType.Int:
                    return BitConverter.GetBytes(GetGPSTimeInt_32(ref point));
                case PointGpsTimeType.Long:
                    return BitConverter.GetBytes(GetGPSTimeInt_64(ref point));
                case PointGpsTimeType.Byte:
                    return BitConverter.GetBytes(GetGPSTimeUInt_8(ref point));
                case PointGpsTimeType.UShort:
                    return BitConverter.GetBytes(GetGPSTimeUInt_16(ref point));
                case PointGpsTimeType.Uint:
                    return BitConverter.GetBytes(GetGPSTimeUInt_32(ref point));
                case PointGpsTimeType.ULong:
                    return BitConverter.GetBytes(GetGPSTimeUInt_64(ref point));
                case PointGpsTimeType.Float:
                    return BitConverter.GetBytes(GetGPSTimeFloat32(ref point));
                case PointGpsTimeType.Double:
                    return BitConverter.GetBytes(GetGPSTimeFloat64(ref point));
            }

            return Array.Empty<byte>();
        }
        #endregion

        #region RawDataDecode

        /// <summary>
        /// Needed for correct array offsets during read
        /// </summary>
        private struct ByteArrayOffsets
        {
            internal int PositionOffset;
            internal int IntensityOffset;
            internal int NormalsOffset;
            internal int RGBOffset;
            internal int LabelOffset;
            internal int CurvatureOffset;
            internal int HitCountOffset;
            internal int GPSTimeOffset;
        }

        private bool _offsetsCalculated = false;
        private ByteArrayOffsets _offsets;

        private ByteArrayOffsets Offsets
        {
            get
            {
                if (_offsetsCalculated)
                    return _offsets;

                _offsets = new ByteArrayOffsets();

                // position
                switch (PositionType)
                {
                    case PointPositionType.Float3:
                        _offsets.PositionOffset = 3 * Marshal.SizeOf<float>();
                        break;
                    case PointPositionType.Double3:
                        _offsets.PositionOffset = 3 * Marshal.SizeOf<double>();
                        break;
                }

                // Intensity
                switch (IntensityType)
                {
                    case PointIntensityType.SByte:
                        _offsets.IntensityOffset = Marshal.SizeOf<sbyte>();
                        break;
                    case PointIntensityType.Short:
                        _offsets.IntensityOffset = Marshal.SizeOf<short>();
                        break;
                    case PointIntensityType.Int:
                        _offsets.IntensityOffset = Marshal.SizeOf<int>();
                        break;
                    case PointIntensityType.Long:
                        _offsets.IntensityOffset = Marshal.SizeOf<long>();
                        break;
                    case PointIntensityType.Byte:
                        _offsets.IntensityOffset = Marshal.SizeOf<byte>();
                        break;
                    case PointIntensityType.UShort:
                        _offsets.IntensityOffset = Marshal.SizeOf<ushort>();
                        break;
                    case PointIntensityType.UInt:
                        _offsets.IntensityOffset = Marshal.SizeOf<uint>();
                        break;
                    case PointIntensityType.ULong:
                        _offsets.IntensityOffset = Marshal.SizeOf<ulong>();
                        break;
                    case PointIntensityType.Float:
                        _offsets.IntensityOffset = Marshal.SizeOf<float>();
                        break;
                    case PointIntensityType.Double:
                        _offsets.IntensityOffset = Marshal.SizeOf<double>();
                        break;
                }

                // Normal
                switch (NormalType)
                {
                    case PointNormalType.Float3:
                        _offsets.NormalsOffset = 3 * Marshal.SizeOf<float>();
                        break;
                    case PointNormalType.Double3:
                        _offsets.NormalsOffset = 3 * Marshal.SizeOf<double>();
                        break;
                }

                // Color
                switch (ColorType)
                {
                    case PointColorType.SByte:
                        _offsets.RGBOffset = Marshal.SizeOf<sbyte>();
                        break;
                    case PointColorType.Short:
                        _offsets.RGBOffset = Marshal.SizeOf<short>();
                        break;
                    case PointColorType.Int:
                        _offsets.RGBOffset = Marshal.SizeOf<int>();
                        break;
                    case PointColorType.Long:
                        _offsets.RGBOffset = Marshal.SizeOf<long>();
                        break;
                    case PointColorType.Byte:
                        _offsets.RGBOffset = Marshal.SizeOf<byte>();
                        break;
                    case PointColorType.Ushort:
                        _offsets.RGBOffset = Marshal.SizeOf<ushort>();
                        break;
                    case PointColorType.Uint:
                        _offsets.RGBOffset = Marshal.SizeOf<uint>();
                        break;
                    case PointColorType.Ulong:
                        _offsets.RGBOffset = Marshal.SizeOf<ulong>();
                        break;
                    case PointColorType.Float:
                        _offsets.RGBOffset = Marshal.SizeOf<float>();
                        break;
                    case PointColorType.Double:
                        _offsets.RGBOffset = Marshal.SizeOf<double>();
                        break;
                    case PointColorType.Float3:
                        _offsets.RGBOffset = 3 * Marshal.SizeOf<float>();
                        break;
                    case PointColorType.Double3:
                        _offsets.RGBOffset = 3 * Marshal.SizeOf<double>();
                        break;
                }

                // Label
                switch (LabelType)
                {
                    case PointLabelType.SByte:
                        _offsets.LabelOffset = Marshal.SizeOf<sbyte>();
                        break;
                    case PointLabelType.Short:
                        _offsets.LabelOffset = Marshal.SizeOf<short>();
                        break;
                    case PointLabelType.Int:
                        _offsets.LabelOffset = Marshal.SizeOf<int>();
                        break;
                    case PointLabelType.Long:
                        _offsets.LabelOffset = Marshal.SizeOf<long>();
                        break;
                    case PointLabelType.Byte:
                        _offsets.LabelOffset = Marshal.SizeOf<byte>();
                        break;
                    case PointLabelType.UShort:
                        _offsets.LabelOffset = Marshal.SizeOf<ushort>();
                        break;
                    case PointLabelType.UInt:
                        _offsets.LabelOffset = Marshal.SizeOf<uint>();
                        break;
                    case PointLabelType.ULong:
                        _offsets.LabelOffset = Marshal.SizeOf<ulong>();
                        break;
                    case PointLabelType.Float:
                        _offsets.LabelOffset = Marshal.SizeOf<float>();
                        break;
                    case PointLabelType.Double:
                        _offsets.LabelOffset = Marshal.SizeOf<double>();
                        break;
                }

                // Curvature
                switch (CurvatureType)
                {
                    case PointCurvatureType.SByte:
                        _offsets.CurvatureOffset = Marshal.SizeOf<sbyte>();
                        break;
                    case PointCurvatureType.Short:
                        _offsets.CurvatureOffset = Marshal.SizeOf<short>();
                        break;
                    case PointCurvatureType.Int:
                        _offsets.CurvatureOffset = Marshal.SizeOf<int>();
                        break;
                    case PointCurvatureType.Long:
                        _offsets.CurvatureOffset = Marshal.SizeOf<long>();
                        break;
                    case PointCurvatureType.Byte:
                        _offsets.CurvatureOffset = Marshal.SizeOf<byte>();
                        break;
                    case PointCurvatureType.UShort:
                        _offsets.CurvatureOffset = Marshal.SizeOf<ushort>();
                        break;
                    case PointCurvatureType.Uint:
                        _offsets.CurvatureOffset = Marshal.SizeOf<uint>();
                        break;
                    case PointCurvatureType.ULong:
                        _offsets.CurvatureOffset = Marshal.SizeOf<ulong>();
                        break;
                    case PointCurvatureType.Float:
                        _offsets.CurvatureOffset = Marshal.SizeOf<float>();
                        break;
                    case PointCurvatureType.Double:
                        _offsets.CurvatureOffset = Marshal.SizeOf<double>();
                        break;
                }

                // Hit count
                switch (HitCountType)
                {
                    case PointHitCountType.SByte:
                        _offsets.HitCountOffset = Marshal.SizeOf<sbyte>();
                        break;
                    case PointHitCountType.Short:
                        _offsets.HitCountOffset = Marshal.SizeOf<short>();
                        break;
                    case PointHitCountType.Int:
                        _offsets.HitCountOffset = Marshal.SizeOf<int>();
                        break;
                    case PointHitCountType.Long:
                        _offsets.HitCountOffset = Marshal.SizeOf<long>();
                        break;
                    case PointHitCountType.Byte:
                        _offsets.HitCountOffset = Marshal.SizeOf<byte>();
                        break;
                    case PointHitCountType.UShort:
                        _offsets.HitCountOffset = Marshal.SizeOf<ushort>();
                        break;
                    case PointHitCountType.Uint:
                        _offsets.HitCountOffset = Marshal.SizeOf<uint>();
                        break;
                    case PointHitCountType.ULong:
                        _offsets.HitCountOffset = Marshal.SizeOf<ulong>();
                        break;
                    case PointHitCountType.Float:
                        _offsets.HitCountOffset = Marshal.SizeOf<float>();
                        break;
                    case PointHitCountType.Double:
                        _offsets.HitCountOffset = Marshal.SizeOf<double>();
                        break;
                }

                // GPSTime
                switch (GpsTimeType)
                {
                    case PointGpsTimeType.SByte:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<sbyte>();
                        break;
                    case PointGpsTimeType.Short:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<short>();
                        break;
                    case PointGpsTimeType.Int:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<int>();
                        break;
                    case PointGpsTimeType.Long:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<long>();
                        break;
                    case PointGpsTimeType.Byte:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<byte>();
                        break;
                    case PointGpsTimeType.UShort:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<ushort>();
                        break;
                    case PointGpsTimeType.Uint:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<uint>();
                        break;
                    case PointGpsTimeType.ULong:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<ulong>();
                        break;
                    case PointGpsTimeType.Float:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<float>();
                        break;
                    case PointGpsTimeType.Double:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<double>();
                        break;
                }

                _offsetsCalculated = true;

                return _offsets;
            }
        }

        private void SetRawPosition(ref TPoint pointIn, byte[] byteIn)
        {
            if (PositionType == PointPositionType.Float3)
            {
                var offset = Marshal.SizeOf<float>();
                var x = BitConverter.ToSingle(byteIn, 0);
                var y = BitConverter.ToSingle(byteIn, offset);
                var z = BitConverter.ToSingle(byteIn, offset * 2);

                SetPositionFloat3_32(ref pointIn, new float3(x, y, z));
            }
            else if (PositionType == PointPositionType.Double3)
            {
                var offset = Marshal.SizeOf<double>();
                var x = BitConverter.ToDouble(byteIn, 0);
                var y = BitConverter.ToDouble(byteIn, offset);
                var z = BitConverter.ToDouble(byteIn, offset * 2);

                SetPositionFloat3_64(ref pointIn, new double3(x, y, z));
            }
        }

        private void SetRawIntensity(ref TPoint pointIn, byte[] byteIn)
        {
            switch (IntensityType)
            {
                case PointIntensityType.SByte:
                    SetIntensityInt_8(ref pointIn, (sbyte)byteIn[Offsets.PositionOffset]);
                    break;
                case PointIntensityType.Short:
                    SetIntensityInt_16(ref pointIn, byteIn[Offsets.PositionOffset]);
                    break;
                case PointIntensityType.Int:
                    SetIntensityInt_32(ref pointIn, BitConverter.ToInt32(byteIn, Offsets.PositionOffset));
                    break;
                case PointIntensityType.Long:
                    SetIntensityInt_64(ref pointIn, BitConverter.ToInt64(byteIn, Offsets.PositionOffset));
                    break;
                case PointIntensityType.Byte:
                    SetIntensityUInt_8(ref pointIn, byteIn[Offsets.PositionOffset]);
                    break;
                case PointIntensityType.UShort:
                    SetIntensityUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, Offsets.PositionOffset));
                    break;
                case PointIntensityType.UInt:
                    SetIntensityUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, Offsets.PositionOffset));
                    break;
                case PointIntensityType.ULong:
                    SetIntensityUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, Offsets.PositionOffset));
                    break;
                case PointIntensityType.Float:
                    SetIntensityFloat32(ref pointIn, BitConverter.ToSingle(byteIn, Offsets.PositionOffset));
                    break;
                case PointIntensityType.Double:
                    SetIntensityFloat64(ref pointIn, BitConverter.ToDouble(byteIn, Offsets.PositionOffset));
                    break;
            }
        }

        private void SetRawNormals(ref TPoint pointIn, byte[] byteIn)
        {
            var offset = Offsets.PositionOffset + Offsets.IntensityOffset;

            if (NormalType == PointNormalType.Float3)
            {

                var dataOffset = Marshal.SizeOf<float>();
                var x = BitConverter.ToSingle(byteIn, offset);
                var y = BitConverter.ToSingle(byteIn, offset + dataOffset);
                var z = BitConverter.ToSingle(byteIn, offset + dataOffset * 2);

                SetNormalFloat3_32(ref pointIn, new float3(x, y, z));

            }
            if (NormalType == PointNormalType.Double3)
            {

                var dataOffset = Marshal.SizeOf<double>();

                var x = BitConverter.ToDouble(byteIn, offset);
                var y = BitConverter.ToDouble(byteIn, offset + dataOffset);
                var z = BitConverter.ToDouble(byteIn, offset + dataOffset * 2);

                SetNormalFloat3_64(ref pointIn, new double3(x, y, z));

            }
        }

        private void SetRawColor(ref TPoint pointIn, byte[] byteIn)
        {
            var offset = Offsets.PositionOffset + Offsets.IntensityOffset + Offsets.NormalsOffset;

            switch (ColorType)
            {
                case PointColorType.SByte:
                    SetColorInt_8(ref pointIn, (sbyte)byteIn[offset]);
                    break;
                case PointColorType.Short:
                    SetColorInt_16(ref pointIn, byteIn[offset]);
                    break;
                case PointColorType.Int:
                    SetColorInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                    break;
                case PointColorType.Long:
                    SetColorInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));
                    break;
                case PointColorType.Byte:
                    SetColorUInt_8(ref pointIn, byteIn[offset + 1]);
                    break;
                case PointColorType.Ushort:
                    SetColorUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                    break;
                case PointColorType.Uint:
                    SetColorUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                    break;
                case PointColorType.Ulong:
                    SetColorUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));
                    break;
                case PointColorType.Float:
                    SetColorFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                    break;
                case PointColorType.Double:
                    SetColorFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));
                    break;
                case PointColorType.Float3:
                    {

                        var dataOffset = Marshal.SizeOf<float>();

                        var x = BitConverter.ToSingle(byteIn, offset);
                        var y = BitConverter.ToSingle(byteIn, offset + dataOffset);
                        var z = BitConverter.ToSingle(byteIn, offset + dataOffset * 2);

                        SetColorFloat3_32(ref pointIn, new float3(x, y, z));

                    }
                    break;

                case PointColorType.Double3:
                    {

                        var dataOffset = Marshal.SizeOf<double>();

                        var x = BitConverter.ToDouble(byteIn, offset);
                        var y = BitConverter.ToDouble(byteIn, offset + dataOffset);
                        var z = BitConverter.ToDouble(byteIn, offset + dataOffset * 2);

                        SetColorFloat3_64(ref pointIn, new double3(x, y, z));

                    }
                    break;
            }
        }

        private void SetRawLabel(ref TPoint pointIn, byte[] byteIn)
        {
            var offset = Offsets.PositionOffset + Offsets.IntensityOffset + Offsets.NormalsOffset + Offsets.RGBOffset;

            switch (LabelType)
            {
                case PointLabelType.SByte:
                    SetLabelInt_8(ref pointIn, (sbyte)byteIn[offset]);
                    break;
                case PointLabelType.Short:
                    SetLabelInt_16(ref pointIn, byteIn[offset]);
                    break;
                case PointLabelType.Int:
                    SetLabelInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                    break;
                case PointLabelType.Long:
                    SetLabelInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));
                    break;
                case PointLabelType.Byte:
                    SetLabelUInt_8(ref pointIn, byteIn[offset]);
                    break;
                case PointLabelType.UShort:
                    SetLabelUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                    break;
                case PointLabelType.UInt:
                    SetLabelUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                    break;
                case PointLabelType.ULong:
                    SetLabelUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));
                    break;
                case PointLabelType.Float:
                    SetLabelFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                    break;
                case PointLabelType.Double:
                    SetLabelFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));
                    break;
            }
        }

        private void SetRawCurvature(ref TPoint pointIn, byte[] byteIn)
        {
            var offset = Offsets.PositionOffset + Offsets.IntensityOffset + Offsets.NormalsOffset + Offsets.RGBOffset + Offsets.LabelOffset;

            switch (CurvatureType)
            {
                case PointCurvatureType.SByte:
                    SetCurvatureInt_8(ref pointIn, (sbyte)byteIn[offset]);
                    break;
                case PointCurvatureType.Short:
                    SetCurvatureInt_16(ref pointIn, byteIn[offset]);
                    break;
                case PointCurvatureType.Int:
                    SetCurvatureInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                    break;
                case PointCurvatureType.Long:
                    SetCurvatureInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));
                    break;
                case PointCurvatureType.Byte:
                    SetCurvatureUInt_8(ref pointIn, byteIn[offset + 1]);
                    break;
                case PointCurvatureType.UShort:
                    SetCurvatureUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                    break;
                case PointCurvatureType.Uint:
                    SetCurvatureUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                    break;
                case PointCurvatureType.ULong:
                    SetCurvatureUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));
                    break;
                case PointCurvatureType.Float:
                    SetCurvatureFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                    break;
                case PointCurvatureType.Double:
                    SetCurvatureFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));
                    break;
            }
        }

        private void SetRawHitCount(ref TPoint pointIn, byte[] byteIn)
        {
            var offset = Offsets.PositionOffset + Offsets.IntensityOffset + Offsets.NormalsOffset + Offsets.RGBOffset + Offsets.LabelOffset;

            switch (HitCountType)
            {
                case PointHitCountType.SByte:
                    SetHitCountInt_8(ref pointIn, (sbyte)byteIn[offset]);
                    break;
                case PointHitCountType.Short:
                    SetHitCountInt_16(ref pointIn, byteIn[offset]);
                    break;
                case PointHitCountType.Int:
                    SetHitCountInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                    break;
                case PointHitCountType.Long:
                    SetHitCountInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));
                    break;
                case PointHitCountType.Byte:
                    SetHitCountUInt_8(ref pointIn, byteIn[offset + 1]);
                    break;
                case PointHitCountType.UShort:
                    SetHitCountUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                    break;
                case PointHitCountType.Uint:
                    SetHitCountUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                    break;
                case PointHitCountType.ULong:
                    SetHitCountUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));
                    break;
                case PointHitCountType.Float:
                    SetHitCountFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                    break;
                case PointHitCountType.Double:
                    SetHitCountFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));
                    break;
            }
        }

        private void SetRawGPSTime(ref TPoint pointIn, byte[] byteIn)
        {
            var offset = Offsets.PositionOffset + Offsets.IntensityOffset + Offsets.NormalsOffset + Offsets.RGBOffset + Offsets.LabelOffset + Offsets.HitCountOffset;

            switch (GpsTimeType)
            {
                case PointGpsTimeType.SByte:
                    SetGPSTimeInt_8(ref pointIn, (sbyte)byteIn[offset]);
                    break;
                case PointGpsTimeType.Short:
                    SetGPSTimeInt_16(ref pointIn, byteIn[offset]);
                    break;
                case PointGpsTimeType.Int:
                    SetGPSTimeInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                    break;
                case PointGpsTimeType.Long:
                    SetGPSTimeInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));
                    break;
                case PointGpsTimeType.Byte:
                    SetGPSTimeUInt_8(ref pointIn, byteIn[offset + 1]);
                    break;
                case PointGpsTimeType.UShort:
                    SetGPSTimeUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                    break;
                case PointGpsTimeType.Uint:
                    SetGPSTimeUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                    break;
                case PointGpsTimeType.ULong:
                    SetGPSTimeUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));
                    break;
                case PointGpsTimeType.Float:
                    SetGPSTimeFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                    break;
                case PointGpsTimeType.Double:
                    SetGPSTimeFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));
                    break;
            }
        }

        #endregion
    }
}