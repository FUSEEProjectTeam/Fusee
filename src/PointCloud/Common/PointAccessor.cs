using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Fusee.PointCloud.Common
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
            switch (PositionType)
            {
                case PointPositionType.Float3_64 when IntensityType == PointIntensityType.None && NormalType == PointNormalType.None && ColorType == PointColorType.None && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None:
                    _pointType = PointType.Pos64;
                    break;
                case PointPositionType.Float3_64 when IntensityType == PointIntensityType.UInt_16 && NormalType == PointNormalType.None && ColorType == PointColorType.Float32 && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None:
                    _pointType = PointType.Pos64Col32IShort;
                    break;
                case PointPositionType.Float3_64 when IntensityType == PointIntensityType.UInt_16 && NormalType == PointNormalType.None && ColorType == PointColorType.None && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None:
                    _pointType = PointType.Pos64IShort;
                    break;
                case PointPositionType.Float3_64 when IntensityType == PointIntensityType.None && NormalType == PointNormalType.None && ColorType == PointColorType.Float32 && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None:
                    _pointType = PointType.Pos64Col32;
                    break;
                case PointPositionType.Float3_64 when IntensityType == PointIntensityType.None && NormalType == PointNormalType.None && ColorType == PointColorType.None && LabelType == PointLabelType.UInt_8 && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None:
                    _pointType = PointType.Pos64Label8;
                    break;
                case PointPositionType.Float3_64 when IntensityType == PointIntensityType.UInt_16 && NormalType == PointNormalType.Float3_32 && ColorType == PointColorType.Float32 && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None:
                    _pointType = PointType.Pos64Nor32Col32IShort;
                    break;
                case PointPositionType.Float3_64 when IntensityType == PointIntensityType.UInt_16 && NormalType == PointNormalType.Float3_32 && ColorType == PointColorType.None && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None:
                    _pointType = PointType.Pos64Nor32IShort;
                    break;
                case PointPositionType.Float3_64 when IntensityType == PointIntensityType.None && NormalType == PointNormalType.Float3_32 && ColorType == PointColorType.Float32 && LabelType == PointLabelType.None && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None:
                    _pointType = PointType.Pos64Nor32Col32;
                    break;
                case PointPositionType.Float3_64 when IntensityType == PointIntensityType.None && NormalType == PointNormalType.None && ColorType == PointColorType.Float32 && LabelType == PointLabelType.UInt_8 && CurvatureType == PointCurvatureType.None && HitCountType == PointHitCountType.None && GpsTimeType == PointGpsTimeType.None:
                    _pointType = PointType.Position_double__Color_float__Label_byte;
                    break;
                default:
                    throw new Exception("Undefined Point Type!");
            }
        }

        public PointPositionType PositionType { get; set; } = PointPositionType.Undefined;
        public PointIntensityType IntensityType { get; set; } = PointIntensityType.None;
        public PointNormalType NormalType { get; set; } = PointNormalType.None;
        public PointColorType ColorType { get; set; } = PointColorType.None;
        public PointLabelType LabelType { get; set; } = PointLabelType.None;
        public PointCurvatureType CurvatureType { get; set; } = PointCurvatureType.None;
        public PointHitCountType HitCountType { get; set; } = PointHitCountType.None;
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
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float3 GetPositionFloat3_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetPositionFloat32");
        }
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new position value.</param>
        public virtual void SetPositionFloat3_32(ref TPoint point, float3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetPositionFloat32");
        }
        /// <summary>
        /// Returns the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double3 GetPositionFloat3_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetPositionFloat64");
        }
        /// <summary>
        /// Sets the position of a point cloud point if <see cref="PointPositionType.Float3_64"/> is true.
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
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.Int_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref sbyte GetIntensityInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityInt_8");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.Int_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref short GetIntensityInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityInt_16");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.Int_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref int GetIntensityInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityInt_32");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref long GetIntensityInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityInt_64");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref byte GetIntensityUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityUInt_8");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ushort GetIntensityUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityUInt_16");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.UInt_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref uint GetIntensityUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityUInt_32");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.UInt_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ulong GetIntensityUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityUInt_64");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.Float32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float GetIntensityFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityFloat32");
        }
        /// <summary>
        /// Returns the intensity of a point cloud point if <see cref="PointIntensityType.Float64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double GetIntensityFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityFloat64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.Int_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityInt_8");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.Int_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityInt_16");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.Int_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityInt_32");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityInt_64");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityUInt_8");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityUInt_16");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.UInt_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityUInt_32");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.UInt_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityUInt_64");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.Float32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new intensity value.</param>
        public virtual void SetIntensityFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityFloat32");
        }
        /// <summary>
        /// Sets the intensity of a point cloud point if <see cref="PointIntensityType.Float64"/> is true.
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
        /// Returns the normal vector of a point cloud point if <see cref="PointNormalType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float3 GetNormalFloat3_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetNormalFloat3_32");
        }

        /// <summary>
        /// Returns the normal vector of a point cloud point if <see cref="PointNormalType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double3 GetNormalFloat3_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetNormalFloat3_64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the normal vector of a point cloud point if <see cref="PointNormalType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new normal vector.</param>
        public virtual void SetNormalFloat3_32(ref TPoint point, float3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetNormalFloat3_32");
        }
        /// <summary>
        /// Sets the normal vector of a point cloud point if <see cref="PointNormalType.Float3_64"/> is true.
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
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Int_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref sbyte GetColorInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorInt_8");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Int_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref short GetColorInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorInt_16");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Int_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref int GetColorInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorInt_32");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref long GetColorInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorInt_64");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref byte GetColorUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorUInt_8");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ushort GetColorUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorUInt_16");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.UInt_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref uint GetColorUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorUInt_32");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.UInt_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ulong GetColorUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorUInt_64");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Float32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float GetColorFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorFloat32");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Float64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double GetColorFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorFloat64");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float3 GetColorFloat3_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorFloat3_32");
        }
        /// <summary>
        /// Returns the normal color of a point cloud point if <see cref="PointColorType.Float3_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double3 GetColorFloat3_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorFloat3_64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Int_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorInt_8");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Int_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorInt_16");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Int_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorInt_32");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorInt_64");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorUInt_8");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorUInt_16");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.UInt_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorUInt_32");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.UInt_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorUInt_64");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Float32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorFloat32");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Float64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorFloat64(ref TPoint point, double val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorFloat64");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Float3_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new color.</param>
        public virtual void SetColorFloat3_32(ref TPoint point, float3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorFloat3_32");
        }
        /// <summary>
        /// Sets the color of a point cloud point if <see cref="PointColorType.Float3_64"/> is true.
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
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Int_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref sbyte GetLabelInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelInt_8");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Int_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref short GetLabelInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelInt_16");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Int_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref int GetLabelInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelInt_32");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref long GetLabelInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelInt_64");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref byte GetLabelUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelUInt_8");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ushort GetLabelUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelUInt_16");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.UInt_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref uint GetLabelUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelUInt_32");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ulong GetLabelUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelUInt_64");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Float32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float GetLabelFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelFloat32");
        }
        /// <summary>
        /// Returns the label of a point cloud point if <see cref="PointLabelType.Float64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double GetLabelFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelFloat64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.Int_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelInt_8");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.Int_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelInt_16");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.Int_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelInt_32");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelInt_64");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelUInt_8");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelUInt_16");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.UInt_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelUInt_32");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.UInt_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelUInt_64");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.Float32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new label.</param>
        public virtual void SetLabelFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelFloat32");
        }
        /// <summary>
        /// Sets the label of a point cloud point if <see cref="PointLabelType.Float64"/> is true.
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
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.Int_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref sbyte GetCurvatureInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureInt_8");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.Int_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref short GetCurvatureInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureInt_16");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.Int_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref int GetCurvatureInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureInt_32");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref long GetCurvatureInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureInt_64");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref byte GetCurvatureUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureUInt_8");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ushort GetCurvatureUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureUInt_16");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.UInt_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref uint GetCurvatureUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureUInt_32");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.UInt_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ulong GetCurvatureUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureUInt_64");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.Float32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float GetCurvatureFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureFloat32");
        }
        /// <summary>
        /// Returns the curvature of a point cloud point if <see cref="PointCurvatureType.Float64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double GetCurvatureFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureFloat64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.Int_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureInt_8");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.Int_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureInt_16");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.Int_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureInt_32");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureInt_64");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureUInt_8");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureUInt_16");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.UInt_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureUInt_32");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.UInt_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureUInt_64");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.Float32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new curvature.</param>
        public virtual void SetCurvatureFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureFloat32");
        }
        /// <summary>
        /// Sets the curvature of a point cloud point if <see cref="PointCurvatureType.Float64"/> is true.
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
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Int_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref sbyte GetHitCountInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountInt_8");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Int_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref short GetHitCountInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountInt_16");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Int_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref int GetHitCountInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountInt_32");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref long GetHitCountInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountInt_64");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref byte GetHitCountUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountUInt_8");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ushort GetHitCountUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountUInt_16");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Int_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref uint GetHitCountUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountUInt_32");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ulong GetHitCountUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountUInt_64");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Float32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float GetHitCountFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountFloat32");
        }
        /// <summary>
        /// Returns the hit count of a point cloud point if <see cref="PointHitCountType.Float64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double GetHitCountFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountFloat64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.Int_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountInt_8");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.Int_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountInt_16");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.Int_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountInt_32");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountInt_64");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountUInt_8");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountUInt_16");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.UInt_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountUInt_32");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.UInt_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountUInt_64");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.Float32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new hit count.</param>
        public virtual void SetHitCountFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountFloat32");
        }
        /// <summary>
        /// Sets the hit count of a point cloud point if <see cref="PointHitCountType.Float64"/> is true.
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
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.Int_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref sbyte GetGPSTimeInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeInt_8");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.Int_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref short GetGPSTimeInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeInt_16");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.Int_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref int GetGPSTimeInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeInt_32");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref long GetGPSTimeInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeInt_64");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref byte GetGPSTimeUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeUInt_8");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ushort GetGPSTimeUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeUInt_16");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.UInt_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref uint GetGPSTimeUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeUInt_32");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.UInt_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref ulong GetGPSTimeUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeUInt_64");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.Float32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref float GetGPSTimeFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeFloat32");
        }
        /// <summary>
        /// Returns the GPS time of a point cloud point if <see cref="PointGpsTimeType.Float64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        public virtual ref double GetGPSTimeFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeFloat64");
        }
        #endregion

        #region Setter
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.Int_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeInt_8");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.Int_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeInt_16");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.Int_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeInt_32");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.Int_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeInt_64");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.UInt_8"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeUInt_8");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.UInt_16"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeUInt_16");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.UInt_32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeUInt_32");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.UInt_64"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeUInt_64");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.Float32"/> is true.
        /// </summary>
        /// <param name="point">The point cloud point.</param>
        /// <param name="val">The new GPS time.</param>
        public virtual void SetGPSTimeFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeFloat32");
        }
        /// <summary>
        /// Sets the GPS time of a point cloud point if <see cref="PointGpsTimeType.Float64"/> is true.
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
            if (PositionType == PointPositionType.Float3_32)
            {

                var x = BitConverter.GetBytes(GetPositionFloat3_32(ref point).x);
                var y = BitConverter.GetBytes(GetPositionFloat3_32(ref point).y);
                var z = BitConverter.GetBytes(GetPositionFloat3_32(ref point).z);

                return x.Concat(y).Concat(z).ToArray();

            }
            else if (PositionType == PointPositionType.Float3_64)
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
                case PointIntensityType.Int_8:
                    return BitConverter.GetBytes(GetIntensityInt_8(ref point));
                case PointIntensityType.Int_16:
                    return BitConverter.GetBytes(GetIntensityInt_16(ref point));
                case PointIntensityType.Int_32:
                    return BitConverter.GetBytes(GetIntensityInt_32(ref point));
                case PointIntensityType.Int_64:
                    return BitConverter.GetBytes(GetIntensityInt_64(ref point));
                case PointIntensityType.UInt_8:
                    return BitConverter.GetBytes(GetIntensityUInt_8(ref point));
                case PointIntensityType.UInt_16:
                    return BitConverter.GetBytes(GetIntensityUInt_16(ref point));
                case PointIntensityType.UInt_32:
                    return BitConverter.GetBytes(GetIntensityUInt_32(ref point));
                case PointIntensityType.UInt_64:
                    return BitConverter.GetBytes(GetIntensityUInt_64(ref point));
                case PointIntensityType.Float32:
                    return BitConverter.GetBytes(GetIntensityFloat32(ref point));
                case PointIntensityType.Float64:
                    return BitConverter.GetBytes(GetIntensityFloat64(ref point));
            }

            return Array.Empty<byte>();
        }

        private byte[] GetRawNormals(ref TPoint point)
        {

            if (NormalType == PointNormalType.Float3_32)
            {
                var x = BitConverter.GetBytes(GetNormalFloat3_32(ref point).x);
                var y = BitConverter.GetBytes(GetNormalFloat3_32(ref point).y);
                var z = BitConverter.GetBytes(GetNormalFloat3_32(ref point).z);

                return x.Concat(y).Concat(z).ToArray();

            }
            else if (NormalType == PointNormalType.Float3_64)
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
                case PointColorType.Int_8:
                    return BitConverter.GetBytes(GetColorInt_8(ref point));
                case PointColorType.Int_16:
                    return BitConverter.GetBytes(GetColorInt_16(ref point));
                case PointColorType.Int_32:
                    return BitConverter.GetBytes(GetColorInt_32(ref point));
                case PointColorType.Int_64:
                    return BitConverter.GetBytes(GetColorInt_64(ref point));
                case PointColorType.UInt_8:
                    return BitConverter.GetBytes(GetColorUInt_8(ref point));
                case PointColorType.UInt_16:
                    return BitConverter.GetBytes(GetColorUInt_16(ref point));
                case PointColorType.UInt_32:
                    return BitConverter.GetBytes(GetColorUInt_32(ref point));
                case PointColorType.UInt_64:
                    return BitConverter.GetBytes(GetColorUInt_64(ref point));
                case PointColorType.Float32:
                    return BitConverter.GetBytes(GetColorFloat32(ref point));
                case PointColorType.Float64:
                    return BitConverter.GetBytes(GetColorFloat64(ref point));
            }

            if (ColorType == PointColorType.Float3_32)
            {

                var x = BitConverter.GetBytes(GetColorFloat3_32(ref point).x);
                var y = BitConverter.GetBytes(GetColorFloat3_32(ref point).y);
                var z = BitConverter.GetBytes(GetColorFloat3_32(ref point).z);

                return x.Concat(y).Concat(z).ToArray();

            }
            if (ColorType == PointColorType.Float3_64)
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
                case PointLabelType.Int_8:
                    return BitConverter.GetBytes(GetLabelInt_8(ref point));
                case PointLabelType.Int_16:
                    return BitConverter.GetBytes(GetLabelInt_16(ref point));
                case PointLabelType.Int_32:
                    return BitConverter.GetBytes(GetLabelInt_32(ref point));
                case PointLabelType.Int_64:
                    return BitConverter.GetBytes(GetLabelInt_64(ref point));
                case PointLabelType.UInt_8:
                    return BitConverter.GetBytes(GetLabelUInt_8(ref point));
                case PointLabelType.UInt_16:
                    return BitConverter.GetBytes(GetLabelUInt_16(ref point));
                case PointLabelType.UInt_32:
                    return BitConverter.GetBytes(GetLabelUInt_32(ref point));
                case PointLabelType.UInt_64:
                    return BitConverter.GetBytes(GetLabelUInt_64(ref point));
                case PointLabelType.Float32:
                    return BitConverter.GetBytes(GetLabelFloat32(ref point));
                case PointLabelType.Float64:
                    return BitConverter.GetBytes(GetLabelFloat64(ref point));
            }

            return Array.Empty<byte>();
        }

        private byte[] GetRawCurvature(ref TPoint point)
        {
            switch (CurvatureType)
            {
                case PointCurvatureType.Int_8:
                    return BitConverter.GetBytes(GetCurvatureInt_8(ref point));
                case PointCurvatureType.Int_16:
                    return BitConverter.GetBytes(GetCurvatureInt_16(ref point));
                case PointCurvatureType.Int_32:
                    return BitConverter.GetBytes(GetCurvatureInt_32(ref point));
                case PointCurvatureType.Int_64:
                    return BitConverter.GetBytes(GetCurvatureInt_64(ref point));
                case PointCurvatureType.UInt_8:
                    return BitConverter.GetBytes(GetCurvatureUInt_8(ref point));
                case PointCurvatureType.UInt_16:
                    return BitConverter.GetBytes(GetCurvatureUInt_16(ref point));
                case PointCurvatureType.UInt_32:
                    return BitConverter.GetBytes(GetCurvatureUInt_32(ref point));
                case PointCurvatureType.UInt_64:
                    return BitConverter.GetBytes(GetCurvatureUInt_64(ref point));
                case PointCurvatureType.Float32:
                    return BitConverter.GetBytes(GetCurvatureFloat32(ref point));
                case PointCurvatureType.Float64:
                    return BitConverter.GetBytes(GetCurvatureFloat64(ref point));
            }

            return Array.Empty<byte>();
        }

        private byte[] GetRawHitCount(ref TPoint point)
        {
            switch (HitCountType)
            {
                case PointHitCountType.Int_8:
                    return BitConverter.GetBytes(GetHitCountInt_8(ref point));
                case PointHitCountType.Int_16:
                    return BitConverter.GetBytes(GetHitCountInt_16(ref point));
                case PointHitCountType.Int_32:
                    return BitConverter.GetBytes(GetHitCountInt_32(ref point));
                case PointHitCountType.Int_64:
                    return BitConverter.GetBytes(GetHitCountInt_64(ref point));
                case PointHitCountType.UInt_8:
                    return BitConverter.GetBytes(GetHitCountUInt_8(ref point));
                case PointHitCountType.UInt_16:
                    return BitConverter.GetBytes(GetHitCountUInt_16(ref point));
                case PointHitCountType.UInt_32:
                    return BitConverter.GetBytes(GetHitCountUInt_32(ref point));
                case PointHitCountType.UInt_64:
                    return BitConverter.GetBytes(GetHitCountUInt_64(ref point));
                case PointHitCountType.Float32:
                    return BitConverter.GetBytes(GetHitCountFloat32(ref point));
                case PointHitCountType.Float64:
                    return BitConverter.GetBytes(GetHitCountFloat64(ref point));
            }

            return Array.Empty<byte>();
        }

        private byte[] GetRawGPSTime(ref TPoint point)
        {
            switch (GpsTimeType)
            {
                case PointGpsTimeType.Int_8:
                    return BitConverter.GetBytes(GetGPSTimeInt_8(ref point));
                case PointGpsTimeType.Int_16:
                    return BitConverter.GetBytes(GetGPSTimeInt_16(ref point));
                case PointGpsTimeType.Int_32:
                    return BitConverter.GetBytes(GetGPSTimeInt_32(ref point));
                case PointGpsTimeType.Int_64:
                    return BitConverter.GetBytes(GetGPSTimeInt_64(ref point));
                case PointGpsTimeType.UInt_8:
                    return BitConverter.GetBytes(GetGPSTimeUInt_8(ref point));
                case PointGpsTimeType.UInt_16:
                    return BitConverter.GetBytes(GetGPSTimeUInt_16(ref point));
                case PointGpsTimeType.UInt_32:
                    return BitConverter.GetBytes(GetGPSTimeUInt_32(ref point));
                case PointGpsTimeType.UInt_64:
                    return BitConverter.GetBytes(GetGPSTimeUInt_64(ref point));
                case PointGpsTimeType.Float32:
                    return BitConverter.GetBytes(GetGPSTimeFloat32(ref point));
                case PointGpsTimeType.Float64:
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
                    case PointPositionType.Float3_32:
                        _offsets.PositionOffset = 3 * Marshal.SizeOf<float>();
                        break;
                    case PointPositionType.Float3_64:
                        _offsets.PositionOffset = 3 * Marshal.SizeOf<double>();
                        break;
                }

                // Intensity
                switch (IntensityType)
                {
                    case PointIntensityType.Int_8:
                        _offsets.IntensityOffset = Marshal.SizeOf<sbyte>();
                        break;
                    case PointIntensityType.Int_16:
                        _offsets.IntensityOffset = Marshal.SizeOf<short>();
                        break;
                    case PointIntensityType.Int_32:
                        _offsets.IntensityOffset = Marshal.SizeOf<int>();
                        break;
                    case PointIntensityType.Int_64:
                        _offsets.IntensityOffset = Marshal.SizeOf<long>();
                        break;
                    case PointIntensityType.UInt_8:
                        _offsets.IntensityOffset = Marshal.SizeOf<byte>();
                        break;
                    case PointIntensityType.UInt_16:
                        _offsets.IntensityOffset = Marshal.SizeOf<ushort>();
                        break;
                    case PointIntensityType.UInt_32:
                        _offsets.IntensityOffset = Marshal.SizeOf<uint>();
                        break;
                    case PointIntensityType.UInt_64:
                        _offsets.IntensityOffset = Marshal.SizeOf<ulong>();
                        break;
                    case PointIntensityType.Float32:
                        _offsets.IntensityOffset = Marshal.SizeOf<float>();
                        break;
                    case PointIntensityType.Float64:
                        _offsets.IntensityOffset = Marshal.SizeOf<double>();
                        break;
                }

                // Normal
                switch (NormalType)
                {
                    case PointNormalType.Float3_32:
                        _offsets.NormalsOffset = 3 * Marshal.SizeOf<float>();
                        break;
                    case PointNormalType.Float3_64:
                        _offsets.NormalsOffset = 3 * Marshal.SizeOf<double>();
                        break;
                }

                // Color
                switch (ColorType)
                {
                    case PointColorType.Int_8:
                        _offsets.RGBOffset = Marshal.SizeOf<sbyte>();
                        break;
                    case PointColorType.Int_16:
                        _offsets.RGBOffset = Marshal.SizeOf<short>();
                        break;
                    case PointColorType.Int_32:
                        _offsets.RGBOffset = Marshal.SizeOf<int>();
                        break;
                    case PointColorType.Int_64:
                        _offsets.RGBOffset = Marshal.SizeOf<long>();
                        break;
                    case PointColorType.UInt_8:
                        _offsets.RGBOffset = Marshal.SizeOf<byte>();
                        break;
                    case PointColorType.UInt_16:
                        _offsets.RGBOffset = Marshal.SizeOf<ushort>();
                        break;
                    case PointColorType.UInt_32:
                        _offsets.RGBOffset = Marshal.SizeOf<uint>();
                        break;
                    case PointColorType.UInt_64:
                        _offsets.RGBOffset = Marshal.SizeOf<ulong>();
                        break;
                    case PointColorType.Float32:
                        _offsets.RGBOffset = Marshal.SizeOf<float>();
                        break;
                    case PointColorType.Float64:
                        _offsets.RGBOffset = Marshal.SizeOf<double>();
                        break;
                    case PointColorType.Float3_32:
                        _offsets.RGBOffset = 3 * Marshal.SizeOf<float>();
                        break;
                    case PointColorType.Float3_64:
                        _offsets.RGBOffset = 3 * Marshal.SizeOf<double>();
                        break;
                }

                // Label
                switch (LabelType)
                {
                    case PointLabelType.Int_8:
                        _offsets.LabelOffset = Marshal.SizeOf<sbyte>();
                        break;
                    case PointLabelType.Int_16:
                        _offsets.LabelOffset = Marshal.SizeOf<short>();
                        break;
                    case PointLabelType.Int_32:
                        _offsets.LabelOffset = Marshal.SizeOf<int>();
                        break;
                    case PointLabelType.Int_64:
                        _offsets.LabelOffset = Marshal.SizeOf<long>();
                        break;
                    case PointLabelType.UInt_8:
                        _offsets.LabelOffset = Marshal.SizeOf<byte>();
                        break;
                    case PointLabelType.UInt_16:
                        _offsets.LabelOffset = Marshal.SizeOf<ushort>();
                        break;
                    case PointLabelType.UInt_32:
                        _offsets.LabelOffset = Marshal.SizeOf<uint>();
                        break;
                    case PointLabelType.UInt_64:
                        _offsets.LabelOffset = Marshal.SizeOf<ulong>();
                        break;
                    case PointLabelType.Float32:
                        _offsets.LabelOffset = Marshal.SizeOf<float>();
                        break;
                    case PointLabelType.Float64:
                        _offsets.LabelOffset = Marshal.SizeOf<double>();
                        break;
                }

                // Curvature
                switch (CurvatureType)
                {
                    case PointCurvatureType.Int_8:
                        _offsets.CurvatureOffset = Marshal.SizeOf<sbyte>();
                        break;
                    case PointCurvatureType.Int_16:
                        _offsets.CurvatureOffset = Marshal.SizeOf<short>();
                        break;
                    case PointCurvatureType.Int_32:
                        _offsets.CurvatureOffset = Marshal.SizeOf<int>();
                        break;
                    case PointCurvatureType.Int_64:
                        _offsets.CurvatureOffset = Marshal.SizeOf<long>();
                        break;
                    case PointCurvatureType.UInt_8:
                        _offsets.CurvatureOffset = Marshal.SizeOf<byte>();
                        break;
                    case PointCurvatureType.UInt_16:
                        _offsets.CurvatureOffset = Marshal.SizeOf<ushort>();
                        break;
                    case PointCurvatureType.UInt_32:
                        _offsets.CurvatureOffset = Marshal.SizeOf<uint>();
                        break;
                    case PointCurvatureType.UInt_64:
                        _offsets.CurvatureOffset = Marshal.SizeOf<ulong>();
                        break;
                    case PointCurvatureType.Float32:
                        _offsets.CurvatureOffset = Marshal.SizeOf<float>();
                        break;
                    case PointCurvatureType.Float64:
                        _offsets.CurvatureOffset = Marshal.SizeOf<double>();
                        break;
                }

                // Hit count
                switch (HitCountType)
                {
                    case PointHitCountType.Int_8:
                        _offsets.HitCountOffset = Marshal.SizeOf<sbyte>();
                        break;
                    case PointHitCountType.Int_16:
                        _offsets.HitCountOffset = Marshal.SizeOf<short>();
                        break;
                    case PointHitCountType.Int_32:
                        _offsets.HitCountOffset = Marshal.SizeOf<int>();
                        break;
                    case PointHitCountType.Int_64:
                        _offsets.HitCountOffset = Marshal.SizeOf<long>();
                        break;
                    case PointHitCountType.UInt_8:
                        _offsets.HitCountOffset = Marshal.SizeOf<byte>();
                        break;
                    case PointHitCountType.UInt_16:
                        _offsets.HitCountOffset = Marshal.SizeOf<ushort>();
                        break;
                    case PointHitCountType.UInt_32:
                        _offsets.HitCountOffset = Marshal.SizeOf<uint>();
                        break;
                    case PointHitCountType.UInt_64:
                        _offsets.HitCountOffset = Marshal.SizeOf<ulong>();
                        break;
                    case PointHitCountType.Float32:
                        _offsets.HitCountOffset = Marshal.SizeOf<float>();
                        break;
                    case PointHitCountType.Float64:
                        _offsets.HitCountOffset = Marshal.SizeOf<double>();
                        break;
                }

                // GPSTime
                switch (GpsTimeType)
                {
                    case PointGpsTimeType.Int_8:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<sbyte>();
                        break;
                    case PointGpsTimeType.Int_16:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<short>();
                        break;
                    case PointGpsTimeType.Int_32:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<int>();
                        break;
                    case PointGpsTimeType.Int_64:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<long>();
                        break;
                    case PointGpsTimeType.UInt_8:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<byte>();
                        break;
                    case PointGpsTimeType.UInt_16:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<ushort>();
                        break;
                    case PointGpsTimeType.UInt_32:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<uint>();
                        break;
                    case PointGpsTimeType.UInt_64:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<ulong>();
                        break;
                    case PointGpsTimeType.Float32:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<float>();
                        break;
                    case PointGpsTimeType.Float64:
                        _offsets.GPSTimeOffset = Marshal.SizeOf<double>();
                        break;
                }

                _offsetsCalculated = true;

                return _offsets;
            }
        }

        private void SetRawPosition(ref TPoint pointIn, byte[] byteIn)
        {
            if (PositionType == PointPositionType.Float3_32)
            {
                var offset = Marshal.SizeOf<float>();
                var x = BitConverter.ToSingle(byteIn, 0);
                var y = BitConverter.ToSingle(byteIn, offset);
                var z = BitConverter.ToSingle(byteIn, offset * 2);

                SetPositionFloat3_32(ref pointIn, new float3(x, y, z));
            }
            else if (PositionType == PointPositionType.Float3_64)
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
                case PointIntensityType.Int_8:
                    SetIntensityInt_8(ref pointIn, (sbyte)byteIn[Offsets.PositionOffset]);
                    break;
                case PointIntensityType.Int_16:
                    SetIntensityInt_16(ref pointIn, byteIn[Offsets.PositionOffset]);
                    break;
                case PointIntensityType.Int_32:
                    SetIntensityInt_32(ref pointIn, BitConverter.ToInt32(byteIn, Offsets.PositionOffset));
                    break;
                case PointIntensityType.Int_64:
                    SetIntensityInt_64(ref pointIn, BitConverter.ToInt64(byteIn, Offsets.PositionOffset));
                    break;
                case PointIntensityType.UInt_8:
                    SetIntensityUInt_8(ref pointIn, byteIn[Offsets.PositionOffset]);
                    break;
                case PointIntensityType.UInt_16:
                    SetIntensityUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, Offsets.PositionOffset));
                    break;
                case PointIntensityType.UInt_32:
                    SetIntensityUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, Offsets.PositionOffset));
                    break;
                case PointIntensityType.UInt_64:
                    SetIntensityUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, Offsets.PositionOffset));
                    break;
                case PointIntensityType.Float32:
                    SetIntensityFloat32(ref pointIn, BitConverter.ToSingle(byteIn, Offsets.PositionOffset));
                    break;
                case PointIntensityType.Float64:
                    SetIntensityFloat64(ref pointIn, BitConverter.ToDouble(byteIn, Offsets.PositionOffset));
                    break;
            }
        }

        private void SetRawNormals(ref TPoint pointIn, byte[] byteIn)
        {
            var offset = Offsets.PositionOffset + Offsets.IntensityOffset;

            if (NormalType == PointNormalType.Float3_32)
            {

                var dataOffset = Marshal.SizeOf<float>();
                var x = BitConverter.ToSingle(byteIn, offset);
                var y = BitConverter.ToSingle(byteIn, offset + dataOffset);
                var z = BitConverter.ToSingle(byteIn, offset + dataOffset * 2);

                SetNormalFloat3_32(ref pointIn, new float3(x, y, z));

            }
            if (NormalType == PointNormalType.Float3_64)
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
                case PointColorType.Int_8:
                    SetColorInt_8(ref pointIn, (sbyte)byteIn[offset]);
                    break;
                case PointColorType.Int_16:
                    SetColorInt_16(ref pointIn, byteIn[offset]);
                    break;
                case PointColorType.Int_32:
                    SetColorInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                    break;
                case PointColorType.Int_64:
                    SetColorInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));
                    break;
                case PointColorType.UInt_8:
                    SetColorUInt_8(ref pointIn, byteIn[offset + 1]);
                    break;
                case PointColorType.UInt_16:
                    SetColorUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                    break;
                case PointColorType.UInt_32:
                    SetColorUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                    break;
                case PointColorType.UInt_64:
                    SetColorUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));
                    break;
                case PointColorType.Float32:
                    SetColorFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                    break;
                case PointColorType.Float64:
                    SetColorFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));
                    break;
                case PointColorType.Float3_32:
                    {

                        var dataOffset = Marshal.SizeOf<float>();

                        var x = BitConverter.ToSingle(byteIn, offset);
                        var y = BitConverter.ToSingle(byteIn, offset + dataOffset);
                        var z = BitConverter.ToSingle(byteIn, offset + dataOffset * 2);

                        SetColorFloat3_32(ref pointIn, new float3(x, y, z));

                    }
                    break;

                case PointColorType.Float3_64:
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
                case PointLabelType.Int_8:
                    SetLabelInt_8(ref pointIn, (sbyte)byteIn[offset]);
                    break;
                case PointLabelType.Int_16:
                    SetLabelInt_16(ref pointIn, byteIn[offset]);
                    break;
                case PointLabelType.Int_32:
                    SetLabelInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                    break;
                case PointLabelType.Int_64:
                    SetLabelInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));
                    break;
                case PointLabelType.UInt_8:
                    SetLabelUInt_8(ref pointIn, byteIn[offset]);
                    break;
                case PointLabelType.UInt_16:
                    SetLabelUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                    break;
                case PointLabelType.UInt_32:
                    SetLabelUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                    break;
                case PointLabelType.UInt_64:
                    SetLabelUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));
                    break;
                case PointLabelType.Float32:
                    SetLabelFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                    break;
                case PointLabelType.Float64:
                    SetLabelFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));
                    break;
            }
        }

        private void SetRawCurvature(ref TPoint pointIn, byte[] byteIn)
        {
            var offset = Offsets.PositionOffset + Offsets.IntensityOffset + Offsets.NormalsOffset + Offsets.RGBOffset + Offsets.LabelOffset;

            switch (CurvatureType)
            {
                case PointCurvatureType.Int_8:
                    SetCurvatureInt_8(ref pointIn, (sbyte)byteIn[offset]);
                    break;
                case PointCurvatureType.Int_16:
                    SetCurvatureInt_16(ref pointIn, byteIn[offset]);
                    break;
                case PointCurvatureType.Int_32:
                    SetCurvatureInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                    break;
                case PointCurvatureType.Int_64:
                    SetCurvatureInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));
                    break;
                case PointCurvatureType.UInt_8:
                    SetCurvatureUInt_8(ref pointIn, byteIn[offset + 1]);
                    break;
                case PointCurvatureType.UInt_16:
                    SetCurvatureUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                    break;
                case PointCurvatureType.UInt_32:
                    SetCurvatureUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                    break;
                case PointCurvatureType.UInt_64:
                    SetCurvatureUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));
                    break;
                case PointCurvatureType.Float32:
                    SetCurvatureFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                    break;
                case PointCurvatureType.Float64:
                    SetCurvatureFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));
                    break;
            }
        }

        private void SetRawHitCount(ref TPoint pointIn, byte[] byteIn)
        {
            var offset = Offsets.PositionOffset + Offsets.IntensityOffset + Offsets.NormalsOffset + Offsets.RGBOffset + Offsets.LabelOffset;

            switch (HitCountType)
            {
                case PointHitCountType.Int_8:
                    SetHitCountInt_8(ref pointIn, (sbyte)byteIn[offset]);
                    break;
                case PointHitCountType.Int_16:
                    SetHitCountInt_16(ref pointIn, byteIn[offset]);
                    break;
                case PointHitCountType.Int_32:
                    SetHitCountInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                    break;
                case PointHitCountType.Int_64:
                    SetHitCountInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));
                    break;
                case PointHitCountType.UInt_8:
                    SetHitCountUInt_8(ref pointIn, byteIn[offset + 1]);
                    break;
                case PointHitCountType.UInt_16:
                    SetHitCountUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                    break;
                case PointHitCountType.UInt_32:
                    SetHitCountUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                    break;
                case PointHitCountType.UInt_64:
                    SetHitCountUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));
                    break;
                case PointHitCountType.Float32:
                    SetHitCountFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                    break;
                case PointHitCountType.Float64:
                    SetHitCountFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));
                    break;
            }
        }

        private void SetRawGPSTime(ref TPoint pointIn, byte[] byteIn)
        {
            var offset = Offsets.PositionOffset + Offsets.IntensityOffset + Offsets.NormalsOffset + Offsets.RGBOffset + Offsets.LabelOffset + Offsets.HitCountOffset;

            switch (GpsTimeType)
            {
                case PointGpsTimeType.Int_8:
                    SetGPSTimeInt_8(ref pointIn, (sbyte)byteIn[offset]);
                    break;
                case PointGpsTimeType.Int_16:
                    SetGPSTimeInt_16(ref pointIn, byteIn[offset]);
                    break;
                case PointGpsTimeType.Int_32:
                    SetGPSTimeInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                    break;
                case PointGpsTimeType.Int_64:
                    SetGPSTimeInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));
                    break;
                case PointGpsTimeType.UInt_8:
                    SetGPSTimeUInt_8(ref pointIn, byteIn[offset + 1]);
                    break;
                case PointGpsTimeType.UInt_16:
                    SetGPSTimeUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                    break;
                case PointGpsTimeType.UInt_32:
                    SetGPSTimeUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                    break;
                case PointGpsTimeType.UInt_64:
                    SetGPSTimeUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));
                    break;
                case PointGpsTimeType.Float32:
                    SetGPSTimeFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                    break;
                case PointGpsTimeType.Float64:
                    SetGPSTimeFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));
                    break;
            }
        }

        #endregion
    }
}