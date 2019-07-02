using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Fusee.Pointcloud.Common
{
    /// <summary>
    ///     Every pointcloud needs a point accessor
    /// </summary>
    /// <example>
    /// 
    /// internal class ExamplePoint
    /// {
    ///     public float3 Position;
    ///     public float3 Normal;
    /// }
    /// 
    /// internal class ExamplePointAccessor : PointAccessor<ExamplePoint>
    /// {     
    ///     public override bool HasNormalFloat3_32 => true;
    ///     public override ref float3 GetNormalFloat3_32(ExamplePoint point)
    ///     {
    ///         return ref point.Normal;
    ///     }
    /// } 
    /// 
    /// </example>
    /// <typeparam name="TPoint"></typeparam>
    public class PointAccessor<TPoint>
    {
        #region PointT_Member

        #region PointXYZ
        // PointXYZ
        public virtual bool HasPositionFloat3_32 => false;
        public virtual bool HasPositionFloat3_64 => false;
        #endregion

        #region PointXYZI
        /// PointXYZI
        public virtual bool HasIntensityInt_8 => false;
        public virtual bool HasIntensityInt_16 => false;
        public virtual bool HasIntensityInt_32 => false;
        public virtual bool HasIntensityInt_64 => false;
        public virtual bool HasIntensityUInt_8 => false;
        public virtual bool HasIntensityUInt_16 => false;
        public virtual bool HasIntensityUInt_32 => false;
        public virtual bool HasIntensityUInt_64 => false;
        public virtual bool HasIntensityFloat32 => false;
        public virtual bool HasIntensityFloat64 => false;
        #endregion

        #region PointXYZINormal
        // PointXYZINormal
        public virtual bool HasNormalFloat3_32 => false;
        public virtual bool HasNormalFloat3_64 => false;
        #endregion

        #region PointXYZINormalRGB
        // PointXYZINormalRGB
        public virtual bool HasColorInt_8 => false;
        public virtual bool HasColorInt_16 => false;
        public virtual bool HasColorInt_32 => false;
        public virtual bool HasColorInt_64 => false;
        public virtual bool HasColorUInt_8 => false;
        public virtual bool HasColorUInt_16 => false;
        public virtual bool HasColorUInt_32 => false;
        public virtual bool HasColorUInt_64 => false;
        public virtual bool HasColorFloat32 => false;
        public virtual bool HasColorFloat64 => false;
        public virtual bool HasColorFloat3_32 => false;
        public virtual bool HasColorFloat3_64 => false;
        #endregion

        /// <summary>
        /// Gets the type of the point as list of the HasXY methods.
        /// </summary>
        /// <returns></returns>
        public List<string> GetPointType()
        {
            return GetType().GetProperties().Where(p => p.PropertyType == typeof(bool) && (bool)p.GetValue(this, null)).Select(p => p.Name).ToList();
        }

        #region PointXYZINormalRGBL
        // PointXYZINormalRGBL
        public virtual bool HasLabelInt_8 => false;
        public virtual bool HasLabelInt_16 => false;
        public virtual bool HasLabelInt_32 => false;
        public virtual bool HasLabelInt_64 => false;
        public virtual bool HasLabelUInt_8 => false;
        public virtual bool HasLabelUInt_16 => false;
        public virtual bool HasLabelUInt_32 => false;
        public virtual bool HasLabelUInt_64 => false;
        public virtual bool HasLabelFloat32 => false;
        public virtual bool HasLabelFloat64 => false;
        #endregion

        #region PointXYZINormalRGBLCurvature
        // PointXYZINormalRGBLCurvature
        public virtual bool HasCurvatureInt_8 => false;
        public virtual bool HasCurvatureInt_16 => false;
        public virtual bool HasCurvatureInt_32 => false;
        public virtual bool HasCurvatureInt_64 => false;
        public virtual bool HasCurvatureUInt_8 => false;
        public virtual bool HasCurvatureUInt_16 => false;
        public virtual bool HasCurvatureUInt_32 => false;
        public virtual bool HasCurvatureUInt_64 => false;
        public virtual bool HasCurvatureFloat32 => false;
        public virtual bool HasCurvatureFloat64 => false;
        #endregion

        #region PointXYZINormalRGBLCurvatureHitCount
        // PointXYZINormalRGBLCurvatureHitCount
        public virtual bool HasHitCountInt_8 => false;
        public virtual bool HasHitCountInt_16 => false;
        public virtual bool HasHitCountInt_32 => false;
        public virtual bool HasHitCountInt_64 => false;
        public virtual bool HasHitCountUInt_8 => false;
        public virtual bool HasHitCountUInt_16 => false;
        public virtual bool HasHitCountUInt_32 => false;
        public virtual bool HasHitCountUInt_64 => false;
        public virtual bool HasHitCountFloat32 => false;
        public virtual bool HasHitCountFloat64 => false;
        #endregion

        #region PointXYZINormalRGBLCurvatureHitCountGPSTime
        // PointXYZINormalRGBLCurvatureHitCountGPSTime
        public virtual bool HasGPSTimeInt_8 => false;
        public virtual bool HasGPSTimeInt_16 => false;
        public virtual bool HasGPSTimeInt_32 => false;
        public virtual bool HasGPSTimeInt_64 => false;
        public virtual bool HasGPSTimeUInt_8 => false;
        public virtual bool HasGPSTimeUInt_16 => false;
        public virtual bool HasGPSTimeUInt_32 => false;
        public virtual bool HasGPSTimeUInt_64 => false;
        public virtual bool HasGPSTimeFloat32 => false;
        public virtual bool HasGPSTimeFloat64 => false;
        #endregion

        #endregion

        #region PointT_Methods

        #region PointXYZ
        public virtual ref float3 GetPositionFloat3_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetPositionFloat32");
        }

        public virtual void SetPositionFloat3_32(ref TPoint point, float3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetPositionFloat32");
        }

        public virtual ref double3 GetPositionFloat3_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetPositionFloat64");
        }

        public virtual void SetPositionFloat3_64(ref TPoint point, double3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetPositionFloat64");
        }
        #endregion

        #region PointXYZI

        #region Getter
        public virtual ref sbyte GetIntensityInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityInt_8");
        }

        public virtual ref short GetIntensityInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityInt_16");
        }

        public virtual ref int GetIntensityInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityInt_32");
        }

        public virtual ref long GetIntensityInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityInt_64");
        }

        public virtual ref byte GetIntensityUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityUInt_8");
        }

        public virtual ref ushort GetIntensityUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityUInt_16");
        }

        public virtual ref uint GetIntensityUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityUInt_32");
        }

        public virtual ref ulong GetIntensityUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityUInt_64");
        }

        public virtual ref float GetIntensityFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityFloat32");
        }

        public virtual ref double GetIntensityFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetIntensityFloat64");
        }
        #endregion

        #region Setter
        public virtual void SetIntensityInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityInt_8");
        }

        public virtual void SetIntensityInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityInt_16");
        }

        public virtual void SetIntensityInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityInt_32");
        }

        public virtual void SetIntensityInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityInt_64");
        }

        public virtual void SetIntensityUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityUInt_8");
        }

        public virtual void SetIntensityUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityUInt_16");
        }

        public virtual void SetIntensityUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityUInt_32");
        }

        public virtual void SetIntensityUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityUInt_64");
        }

        public virtual void SetIntensityFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityFloat32");
        }

        public virtual void SetIntensityFloat64(ref TPoint point, double val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetIntensityFloat64");
        }
        #endregion

        #endregion

        #region PointXYZINormal

        #region Getter      
        public virtual ref float3 GetNormalFloat3_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetNormalFloat3_32");
        }

        public virtual ref double3 GetNormalFloat3_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetNormalFloat3_64");
        }
        #endregion

        #region Setter        
        public virtual void SetNormalFloat3_32(ref TPoint point, float3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetNormalFloat3_32");
        }

        public virtual void SetNormalFloat3_64(ref TPoint point, double3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetNormalFloat3_64");
        }
        #endregion

        #endregion

        #region PointXYZINormalRGB

        #region Getter
        public virtual ref sbyte GetColorInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorInt_8");
        }

        public virtual ref short GetColorInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorInt_16");
        }

        public virtual ref int GetColorInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorInt_32");
        }

        public virtual ref long GetColorInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorInt_64");
        }

        public virtual ref byte GetColorUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorUInt_8");
        }

        public virtual ref ushort GetColorUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorUInt_16");
        }

        public virtual ref uint GetColorUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorUInt_32");
        }

        public virtual ref ulong GetColorUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorUInt_64");
        }

        public virtual ref float GetColorFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorFloat32");
        }

        public virtual ref double GetColorFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorFloat64");
        }

        public virtual ref float3 GetColorFloat3_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorFloat3_32");
        }

        public virtual ref double3 GetColorFloat3_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetColorFloat3_64");
        }
        #endregion

        #region Setter
        public virtual void SetColorInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorInt_8");
        }

        public virtual void SetColorInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorInt_16");
        }

        public virtual void SetColorInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorInt_32");
        }

        public virtual void SetColorInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorInt_64");
        }

        public virtual void SetColorUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorUInt_8");
        }

        public virtual void SetColorUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorUInt_16");
        }

        public virtual void SetColorUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorUInt_32");
        }

        public virtual void SetColorUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorUInt_64");
        }

        public virtual void SetColorFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorFloat32");
        }

        public virtual void SetColorFloat64(ref TPoint point, double val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorFloat64");
        }

        public virtual void SetColorFloat3_32(ref TPoint point, float3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorFloat3_32");
        }

        public virtual void SetColorFloat3_64(ref TPoint point, double3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetColorFloat3_64");
        }
        #endregion

        #endregion

        #region PointXYZINormalRGBL

        #region Getter
        public virtual ref sbyte GetLabelInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelInt_8");
        }

        public virtual ref short GetLabelInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelInt_16");
        }

        public virtual ref int GetLabelInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelInt_32");
        }

        public virtual ref long GetLabelInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelInt_64");
        }

        public virtual ref byte GetLabelUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelUInt_8");
        }

        public virtual ref ushort GetLabelUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelUInt_16");
        }

        public virtual ref uint GetLabelUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelUInt_32");
        }

        public virtual ref ulong GetLabelUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelUInt_64");
        }

        public virtual ref float GetLabelFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelFloat32");
        }

        public virtual ref double GetLabelFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetLabelFloat64");
        }
        #endregion

        #region Setter
        public virtual void SetLabelInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelInt_8");
        }

        public virtual void SetLabelInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelInt_16");
        }

        public virtual void SetLabelInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelInt_32");
        }

        public virtual void SetLabelInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelInt_64");
        }

        public virtual void SetLabelUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelUInt_8");
        }

        public virtual void SetLabelUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelUInt_16");
        }

        public virtual void SetLabelUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelUInt_32");
        }

        public virtual void SetLabelUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelUInt_64");
        }

        public virtual void SetLabelFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelFloat32");
        }

        public virtual void SetLabelFloat64(ref TPoint point, double val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetLabelFloat64");
        }
        #endregion

        #endregion

        #region PointXYZINormalRGBLCurvature

        #region Getter
        public virtual ref sbyte GetCurvatureInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureInt_8");
        }

        public virtual ref short GetCurvatureInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureInt_16");
        }

        public virtual ref int GetCurvatureInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureInt_32");
        }

        public virtual ref long GetCurvatureInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureInt_64");
        }

        public virtual ref byte GetCurvatureUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureUInt_8");
        }

        public virtual ref ushort GetCurvatureUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureUInt_16");
        }

        public virtual ref uint GetCurvatureUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureUInt_32");
        }

        public virtual ref ulong GetCurvatureUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureUInt_64");
        }

        public virtual ref float GetCurvatureFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureFloat32");
        }

        public virtual ref double GetCurvatureFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetCurvatureFloat64");
        }
        #endregion

        #region Setter
        public virtual void SetCurvatureInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureInt_8");
        }

        public virtual void SetCurvatureInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureInt_16");
        }

        public virtual void SetCurvatureInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureInt_32");
        }

        public virtual void SetCurvatureInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureInt_64");
        }

        public virtual void SetCurvatureUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureUInt_8");
        }

        public virtual void SetCurvatureUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureUInt_16");
        }

        public virtual void SetCurvatureUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureUInt_32");
        }

        public virtual void SetCurvatureUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureUInt_64");
        }

        public virtual void SetCurvatureFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureFloat32");
        }

        public virtual void SetCurvatureFloat64(ref TPoint point, double val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetCurvatureFloat64");
        }
        #endregion

        #endregion

        #region PointXYZINormalRGBLCurvatureHitCount

        #region Getter
        public virtual ref sbyte GetHitCountInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountInt_8");
        }

        public virtual ref short GetHitCountInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountInt_16");
        }

        public virtual ref int GetHitCountInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountInt_32");
        }

        public virtual ref long GetHitCountInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountInt_64");
        }

        public virtual ref byte GetHitCountUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountUInt_8");
        }

        public virtual ref ushort GetHitCountUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountUInt_16");
        }

        public virtual ref uint GetHitCountUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountUInt_32");
        }

        public virtual ref ulong GetHitCountUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountUInt_64");
        }

        public virtual ref float GetHitCountFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountFloat32");
        }

        public virtual ref double GetHitCountFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetHitCountFloat64");
        }
        #endregion

        #region Setter
        public virtual void SetHitCountInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountInt_8");
        }

        public virtual void SetHitCountInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountInt_16");
        }

        public virtual void SetHitCountInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountInt_32");
        }

        public virtual void SetHitCountInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountInt_64");
        }

        public virtual void SetHitCountUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountUInt_8");
        }

        public virtual void SetHitCountUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountUInt_16");
        }

        public virtual void SetHitCountUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountUInt_32");
        }

        public virtual void SetHitCountUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountUInt_64");
        }

        public virtual void SetHitCountFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountFloat32");
        }

        public virtual void SetHitCountFloat64(ref TPoint point, double val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetHitCountFloat64");
        }
        #endregion

        #endregion

        #region PointXYZINormalRGBLCurvatureHitCountGPSTime

        #region Getter
        public virtual ref sbyte GetGPSTimeInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeInt_8");
        }

        public virtual ref short GetGPSTimeInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeInt_16");
        }

        public virtual ref int GetGPSTimeInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeInt_32");
        }

        public virtual ref long GetGPSTimeInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeInt_64");
        }

        public virtual ref byte GetGPSTimeUInt_8(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeUInt_8");
        }

        public virtual ref ushort GetGPSTimeUInt_16(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeUInt_16");
        }

        public virtual ref uint GetGPSTimeUInt_32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeUInt_32");
        }

        public virtual ref ulong GetGPSTimeUInt_64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeUInt_64");
        }

        public virtual ref float GetGPSTimeFloat32(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeFloat32");
        }

        public virtual ref double GetGPSTimeFloat64(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetGPSTimeFloat64");
        }
        #endregion

        #region Setter
        public virtual void SetGPSTimeInt_8(ref TPoint point, sbyte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeInt_8");
        }

        public virtual void SetGPSTimeInt_16(ref TPoint point, short val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeInt_16");
        }

        public virtual void SetGPSTimeInt_32(ref TPoint point, int val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeInt_32");
        }

        public virtual void SetGPSTimeInt_64(ref TPoint point, long val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeInt_64");
        }

        public virtual void SetGPSTimeUInt_8(ref TPoint point, byte val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeUInt_8");
        }

        public virtual void SetGPSTimeUInt_16(ref TPoint point, ushort val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeUInt_16");
        }

        public virtual void SetGPSTimeUInt_32(ref TPoint point, uint val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeUInt_32");
        }

        public virtual void SetGPSTimeUInt_64(ref TPoint point, ulong val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeUInt_64");
        }

        public virtual void SetGPSTimeFloat32(ref TPoint point, float val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeFloat32");
        }

        public virtual void SetGPSTimeFloat64(ref TPoint point, double val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetGPSTimeFloat64");
        }
        #endregion

        #endregion

        #endregion

        #region RawData

        private delegate byte[] EncodeRawPoint(ref TPoint point);

        private delegate void DecodeRawPoint(ref TPoint pointIn, byte[] byteIn);

        #region RawDataEncode

        private delegate byte[] EncodeRawPosition(ref TPoint point);

        private delegate byte[] EncodeRawIntensity(ref TPoint point);

        private delegate byte[] EncodeRawNormals(ref TPoint point);

        private delegate byte[] EncodeRawRGB(ref TPoint point);

        private delegate byte[] EncodeRawLabel(ref TPoint point);

        private delegate byte[] EncodeRawCurvature(ref TPoint point);

        private delegate byte[] EncodeRawHitCount(ref TPoint point);

        private delegate byte[] EncodeRawGPSTime(ref TPoint point);

        private EncodeRawPoint _getRawPointMethod = null;

        private EncodeRawPosition _encodeRawPositionMethod;
        private EncodeRawIntensity _encodeRawIntensityMethod;
        private EncodeRawNormals _encodeRawNormalsMethod;
        private EncodeRawRGB _encodeRawRGBMethod;
        private EncodeRawLabel _encodeRawLabelMethod;
        private EncodeRawCurvature _encodeRawCurvatureMethod;
        private EncodeRawHitCount _encodeRawHitCountMethod;
        private EncodeRawGPSTime _encodeRawGPSTimeMethod;

        private EncodeRawPoint GetRawPointMethod
        {
            get
            {
                if (_getRawPointMethod != null)
                    return _getRawPointMethod;

                // First call, construct everything and save the resulting methods
                _encodeRawPositionMethod = GetRawPositionMethod;
                _encodeRawIntensityMethod = GetRawIntensityMethod;
                _encodeRawNormalsMethod = GetRawNormalsMethod;
                _encodeRawRGBMethod = GetRawRGBMethod;
                _encodeRawLabelMethod = GetRawLabelMethod;
                _encodeRawCurvatureMethod = GetRawCurvatureMethod;
                _encodeRawHitCountMethod = GetRawHitCountMethod;
                _encodeRawGPSTimeMethod = GetRawGPSTimeMethod;

                _getRawPointMethod = (ref TPoint point) =>
                {
                    var position = _encodeRawPositionMethod(ref point);
                    var intensity = _encodeRawIntensityMethod(ref point);
                    var normals = _encodeRawNormalsMethod(ref point);
                    var rgb = _encodeRawRGBMethod(ref point);
                    var label = _encodeRawLabelMethod(ref point);
                    var curvature = _encodeRawCurvatureMethod(ref point);
                    var hitCount = _encodeRawHitCountMethod(ref point);
                    var GPSTime = _encodeRawGPSTimeMethod(ref point);

                    // XYZINormalRGBLCurvatureHitCountGPSTime
                    return position.Concat(intensity).Concat(normals).Concat(rgb).Concat(label).Concat(curvature).Concat(hitCount).Concat(GPSTime).ToArray();
                };

                return _getRawPointMethod;
            }
        }

        private EncodeRawPosition GetRawPositionMethod
        {
            get
            {
                if (HasPositionFloat3_32)
                {
                    return (ref TPoint point) =>
                    {
                        var x = BitConverter.GetBytes(GetPositionFloat3_32(ref point).x);
                        var y = BitConverter.GetBytes(GetPositionFloat3_32(ref point).y);
                        var z = BitConverter.GetBytes(GetPositionFloat3_32(ref point).z);

                        return x.Concat(y).Concat(z).ToArray();
                    };
                }
                if (HasPositionFloat3_64)
                {
                    return (ref TPoint point) =>
                    {
                        var x = BitConverter.GetBytes(GetPositionFloat3_64(ref point).x);
                        var y = BitConverter.GetBytes(GetPositionFloat3_64(ref point).y);
                        var z = BitConverter.GetBytes(GetPositionFloat3_64(ref point).z);

                        return x.Concat(y).Concat(z).ToArray();
                    };
                }
                return (ref TPoint point) => new byte[] { };
            }
        }

        private EncodeRawIntensity GetRawIntensityMethod
        {
            get
            {
                // Int
                if (HasIntensityInt_8)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetIntensityInt_8(ref point)); };
                if (HasIntensityInt_16)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetIntensityInt_16(ref point)); };
                if (HasIntensityInt_32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetIntensityInt_32(ref point)); };
                if (HasIntensityInt_64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetIntensityInt_64(ref point)); };

                // Uint
                if (HasIntensityUInt_8)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetIntensityUInt_8(ref point)); };
                if (HasIntensityUInt_16)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetIntensityUInt_16(ref point)); };
                if (HasIntensityUInt_32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetIntensityUInt_32(ref point)); };
                if (HasIntensityUInt_64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetIntensityUInt_64(ref point)); };

                if (HasIntensityFloat32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetIntensityFloat32(ref point)); };
                if (HasIntensityFloat64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetIntensityFloat64(ref point)); };

                return (ref TPoint point) => new byte[] { };
            }
        }

        private EncodeRawNormals GetRawNormalsMethod
        {
            get
            {
                if (HasNormalFloat3_32)
                {
                    return (ref TPoint point) =>
                    {
                        var x = BitConverter.GetBytes(GetNormalFloat3_32(ref point).x);
                        var y = BitConverter.GetBytes(GetNormalFloat3_32(ref point).y);
                        var z = BitConverter.GetBytes(GetNormalFloat3_32(ref point).z);

                        return x.Concat(y).Concat(z).ToArray();
                    };
                }
                if (HasNormalFloat3_64)
                {
                    return (ref TPoint point) =>
                    {
                        var x = BitConverter.GetBytes(GetNormalFloat3_64(ref point).x);
                        var y = BitConverter.GetBytes(GetNormalFloat3_64(ref point).y);
                        var z = BitConverter.GetBytes(GetNormalFloat3_64(ref point).z);

                        return x.Concat(y).Concat(z).ToArray();
                    };
                }
                return (ref TPoint point) => new byte[] { };
            }
        }

        private EncodeRawRGB GetRawRGBMethod
        {
            get
            {
                // Int
                if (HasColorInt_8)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetColorInt_8(ref point)); };
                if (HasColorInt_16)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetColorInt_16(ref point)); };
                if (HasColorInt_32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetColorInt_32(ref point)); };
                if (HasColorInt_64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetColorInt_64(ref point)); };

                // Uint
                if (HasColorUInt_8)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetColorUInt_8(ref point)); };
                if (HasColorUInt_16)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetColorUInt_16(ref point)); };
                if (HasColorUInt_32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetColorUInt_32(ref point)); };
                if (HasColorUInt_64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetColorUInt_64(ref point)); };

                if (HasColorFloat32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetColorFloat32(ref point)); };
                if (HasColorFloat64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetColorFloat64(ref point)); };


                if (HasColorFloat3_32)
                {
                    return (ref TPoint point) =>
                    {
                        var x = BitConverter.GetBytes(GetColorFloat3_32(ref point).x);
                        var y = BitConverter.GetBytes(GetColorFloat3_32(ref point).y);
                        var z = BitConverter.GetBytes(GetColorFloat3_32(ref point).z);

                        return x.Concat(y).Concat(z).ToArray();
                    };
                }
                if (HasColorFloat3_64)
                {
                    return (ref TPoint point) =>
                    {
                        var x = BitConverter.GetBytes(GetColorFloat3_64(ref point).x);
                        var y = BitConverter.GetBytes(GetColorFloat3_64(ref point).y);
                        var z = BitConverter.GetBytes(GetColorFloat3_64(ref point).z);

                        return x.Concat(y).Concat(z).ToArray();
                    };
                }
                return (ref TPoint point) => new byte[] { };
            }

        }

        private EncodeRawLabel GetRawLabelMethod
        {
            get
            {
                // Int
                if (HasLabelInt_8)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetLabelInt_8(ref point)); };
                if (HasLabelInt_16)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetLabelInt_16(ref point)); };
                if (HasLabelInt_32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetLabelInt_32(ref point)); };
                if (HasLabelInt_64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetLabelInt_64(ref point)); };

                // Uint
                if (HasLabelUInt_8)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetLabelUInt_8(ref point)); };
                if (HasLabelUInt_16)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetLabelUInt_16(ref point)); };
                if (HasLabelUInt_32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetLabelUInt_32(ref point)); };
                if (HasLabelUInt_64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetLabelUInt_64(ref point)); };

                if (HasLabelFloat32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetLabelFloat32(ref point)); };
                if (HasLabelFloat64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetLabelFloat64(ref point)); };

                return (ref TPoint point) => new byte[] { };
            }

        }

        private EncodeRawCurvature GetRawCurvatureMethod
        {
            get
            {
                // Int
                if (HasCurvatureInt_8)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetCurvatureInt_8(ref point)); };
                if (HasCurvatureInt_16)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetCurvatureInt_16(ref point)); };
                if (HasCurvatureInt_32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetCurvatureInt_32(ref point)); };
                if (HasCurvatureInt_64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetCurvatureInt_64(ref point)); };

                // Uint
                if (HasCurvatureUInt_8)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetCurvatureUInt_8(ref point)); };
                if (HasCurvatureUInt_16)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetCurvatureUInt_16(ref point)); };
                if (HasCurvatureUInt_32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetCurvatureUInt_32(ref point)); };
                if (HasCurvatureUInt_64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetCurvatureUInt_64(ref point)); };

                if (HasCurvatureFloat32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetCurvatureFloat32(ref point)); };
                if (HasCurvatureFloat64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetCurvatureFloat64(ref point)); };

                return (ref TPoint point) => new byte[] { };
            }

        }

        private EncodeRawHitCount GetRawHitCountMethod
        {
            get
            {
                // Int
                if (HasHitCountInt_8)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetHitCountInt_8(ref point)); };
                if (HasHitCountInt_16)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetHitCountInt_16(ref point)); };
                if (HasHitCountInt_32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetHitCountInt_32(ref point)); };
                if (HasHitCountInt_64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetHitCountInt_64(ref point)); };

                // Uint
                if (HasHitCountUInt_8)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetHitCountUInt_8(ref point)); };
                if (HasHitCountUInt_16)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetHitCountUInt_16(ref point)); };
                if (HasHitCountUInt_32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetHitCountUInt_32(ref point)); };
                if (HasHitCountUInt_64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetHitCountUInt_64(ref point)); };

                if (HasHitCountFloat32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetHitCountFloat32(ref point)); };
                if (HasHitCountFloat64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetHitCountFloat64(ref point)); };

                return (ref TPoint point) => new byte[] { };
            }

        }

        private EncodeRawGPSTime GetRawGPSTimeMethod
        {
            get
            {
                // Int
                if (HasGPSTimeInt_8)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetGPSTimeInt_8(ref point)); };
                if (HasGPSTimeInt_16)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetGPSTimeInt_16(ref point)); };
                if (HasGPSTimeInt_32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetGPSTimeInt_32(ref point)); };
                if (HasGPSTimeInt_64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetGPSTimeInt_64(ref point)); };

                // Uint
                if (HasGPSTimeUInt_8)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetGPSTimeUInt_8(ref point)); };
                if (HasGPSTimeUInt_16)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetGPSTimeUInt_16(ref point)); };
                if (HasGPSTimeUInt_32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetGPSTimeUInt_32(ref point)); };
                if (HasGPSTimeUInt_64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetGPSTimeUInt_64(ref point)); };

                if (HasGPSTimeFloat32)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetGPSTimeFloat32(ref point)); };
                if (HasGPSTimeFloat64)
                    return (ref TPoint point) => { return BitConverter.GetBytes(GetGPSTimeFloat64(ref point)); };

                return (ref TPoint point) => new byte[] { };
            }

        }

        #endregion

        #region RawDataDecode

        private delegate void DecodeRawPosition(ref TPoint pointIn, byte[] byteIn);

        private delegate void DecodeRawIntensity(ref TPoint pointIn, byte[] byteIn);

        private delegate void DecodeRawNormals(ref TPoint pointIn, byte[] byteIn);

        private delegate void DecodeRawRGB(ref TPoint pointIn, byte[] byteIn);

        private delegate void DecodeRawLabel(ref TPoint pointIn, byte[] byteIn);

        private delegate void DecodeRawCurvature(ref TPoint pointIn, byte[] byteIn);

        private delegate void DecodeRawHitCount(ref TPoint pointIn, byte[] byteIn);

        private delegate void DecodeRawGPSTime(ref TPoint pointIn, byte[] byteIn);

        private DecodeRawPoint _setRawPointMethod = null;

        private DecodeRawPosition _decodeRawPositionMethod;
        private DecodeRawIntensity _decodeRawIntensityMethod;
        private DecodeRawNormals _decodeRawNormalsMethod;
        private DecodeRawRGB _decodeRawRGBMethod;
        private DecodeRawLabel _decodeRawLabelMethod;
        private DecodeRawCurvature _decodeRawCurvatureMethod;
        private DecodeRawHitCount _decodeRawHitCountMethod;
        private DecodeRawGPSTime _decodeRawGPSTimeMethod;

        private DecodeRawPoint SetRawPointMethod
        {
            get
            {
                if (_setRawPointMethod != null)
                    return _setRawPointMethod;

                // First call, construct everything and save the resulting method
                _decodeRawPositionMethod = SetRawPositionMethod;
                _decodeRawIntensityMethod = SetRawIntensityMethod;
                _decodeRawNormalsMethod = SetRawNormalsMethod;
                _decodeRawRGBMethod = SetRawRGBMethod;
                _decodeRawLabelMethod = SetRawLabelMethod;
                _decodeRawCurvatureMethod = SetRawCurvatureMethod;
                _decodeRawHitCountMethod = SetRawHitCountMethod;
                _decodeRawGPSTimeMethod = SetRawGPSTimeMethod;

                _setRawPointMethod = (ref TPoint pointInt, byte[] byteIn) =>
                {
                    // Call all methods to recreate the point
                    _decodeRawPositionMethod(ref pointInt, byteIn);
                    _decodeRawIntensityMethod(ref pointInt, byteIn);
                    _decodeRawNormalsMethod(ref pointInt, byteIn);
                    _decodeRawRGBMethod(ref pointInt, byteIn);
                    _decodeRawLabelMethod(ref pointInt, byteIn);
                    _decodeRawCurvatureMethod(ref pointInt, byteIn);
                    _decodeRawHitCountMethod(ref pointInt, byteIn);
                    _decodeRawGPSTimeMethod(ref pointInt, byteIn);
                };

                return _setRawPointMethod;
            }
        }

        /// <summary>
        ///     Needed for correct array offsets during read
        /// </summary>
        internal struct ByteArrayOffsets
        {
            // XYZINormalRGBLCurvatureHitCountGPSTime

            internal int PositionOffset;
            internal int IntensityOffset;
            internal int NormalsOffset;
            internal int RGBOffset;
            internal int LabelOffset;
            internal int CurvatureOffset;
            internal int HitCountOffset;
            internal int GPSTimeOffset;
        }

        private bool OffsetsCalculated = false;
        private ByteArrayOffsets _offsets;

        internal ByteArrayOffsets Offsets
        {
            get
            {
                if (OffsetsCalculated)
                    return _offsets;

                // XYZINormalRGBLCurvatureHitCountGPSTime

                _offsets = new ByteArrayOffsets();

                // position
                if (HasPositionFloat3_32)
                    _offsets.PositionOffset = 3 * Marshal.SizeOf<float>();
                if (HasPositionFloat3_64)
                    _offsets.PositionOffset = 3 * Marshal.SizeOf<double>();

                // Intensity
                if (HasIntensityInt_8)
                    _offsets.IntensityOffset = Marshal.SizeOf<sbyte>();
                if (HasIntensityInt_16)
                    _offsets.IntensityOffset = Marshal.SizeOf<short>();
                if (HasIntensityInt_32)
                    _offsets.IntensityOffset = Marshal.SizeOf<int>();
                if (HasIntensityInt_64)
                    _offsets.IntensityOffset = Marshal.SizeOf<long>();

                if (HasIntensityUInt_8)
                    _offsets.IntensityOffset = Marshal.SizeOf<byte>();
                if (HasIntensityUInt_16)
                    _offsets.IntensityOffset = Marshal.SizeOf<ushort>();
                if (HasIntensityUInt_32)
                    _offsets.IntensityOffset = Marshal.SizeOf<uint>();
                if (HasIntensityUInt_64)
                    _offsets.IntensityOffset = Marshal.SizeOf<ulong>();

                if (HasIntensityFloat32)
                    _offsets.IntensityOffset = Marshal.SizeOf<float>();

                if (HasIntensityFloat64)
                    _offsets.IntensityOffset = Marshal.SizeOf<double>();

                // Normal
                if (HasNormalFloat3_32)
                    _offsets.NormalsOffset = 3 * Marshal.SizeOf<float>();
                if (HasNormalFloat3_64)
                    _offsets.NormalsOffset = 3 * Marshal.SizeOf<double>();

                // Color
                if (HasColorInt_8)
                    _offsets.RGBOffset = Marshal.SizeOf<sbyte>();
                if (HasColorInt_16)
                    _offsets.RGBOffset = Marshal.SizeOf<short>();
                if (HasColorInt_32)
                    _offsets.RGBOffset = Marshal.SizeOf<int>();
                if (HasColorInt_64)
                    _offsets.RGBOffset = Marshal.SizeOf<long>();

                if (HasColorUInt_8)
                    _offsets.RGBOffset = Marshal.SizeOf<byte>();
                if (HasColorUInt_16)
                    _offsets.RGBOffset = Marshal.SizeOf<ushort>();
                if (HasColorUInt_32)
                    _offsets.RGBOffset = Marshal.SizeOf<uint>();
                if (HasColorUInt_64)
                    _offsets.RGBOffset = Marshal.SizeOf<ulong>();

                if (HasColorFloat32)
                    _offsets.RGBOffset = Marshal.SizeOf<float>();

                if (HasColorFloat64)
                    _offsets.RGBOffset = Marshal.SizeOf<double>();

                if (HasColorFloat3_32)
                    _offsets.RGBOffset = 3 * Marshal.SizeOf<float>();

                if (HasColorFloat3_64)
                    _offsets.RGBOffset = 3 * Marshal.SizeOf<double>();

                // Label                
                if (HasLabelInt_8)
                    _offsets.LabelOffset = Marshal.SizeOf<sbyte>();
                if (HasLabelInt_16)
                    _offsets.LabelOffset = Marshal.SizeOf<short>();
                if (HasLabelInt_32)
                    _offsets.LabelOffset = Marshal.SizeOf<int>();
                if (HasLabelInt_64)
                    _offsets.LabelOffset = Marshal.SizeOf<long>();

                if (HasLabelUInt_8)
                    _offsets.LabelOffset = Marshal.SizeOf<byte>();
                if (HasLabelUInt_16)
                    _offsets.LabelOffset = Marshal.SizeOf<ushort>();
                if (HasLabelUInt_32)
                    _offsets.LabelOffset = Marshal.SizeOf<uint>();
                if (HasLabelUInt_64)
                    _offsets.LabelOffset = Marshal.SizeOf<ulong>();

                if (HasLabelFloat32)
                    _offsets.LabelOffset = Marshal.SizeOf<float>();

                if (HasLabelFloat64)
                    _offsets.LabelOffset = Marshal.SizeOf<double>();

                // Curvature
                if (HasCurvatureInt_8)
                    _offsets.CurvatureOffset = Marshal.SizeOf<sbyte>();
                if (HasCurvatureInt_16)
                    _offsets.CurvatureOffset = Marshal.SizeOf<short>();
                if (HasCurvatureInt_32)
                    _offsets.CurvatureOffset = Marshal.SizeOf<int>();
                if (HasCurvatureInt_64)
                    _offsets.CurvatureOffset = Marshal.SizeOf<long>();

                if (HasCurvatureUInt_8)
                    _offsets.CurvatureOffset = Marshal.SizeOf<byte>();
                if (HasCurvatureUInt_16)
                    _offsets.CurvatureOffset = Marshal.SizeOf<ushort>();
                if (HasCurvatureUInt_32)
                    _offsets.CurvatureOffset = Marshal.SizeOf<uint>();
                if (HasCurvatureUInt_64)
                    _offsets.CurvatureOffset = Marshal.SizeOf<ulong>();

                if (HasCurvatureFloat32)
                    _offsets.CurvatureOffset = Marshal.SizeOf<float>();

                if (HasCurvatureFloat64)
                    _offsets.CurvatureOffset = Marshal.SizeOf<double>();


                // Hitcount
                if (HasHitCountInt_8)
                    _offsets.HitCountOffset = Marshal.SizeOf<sbyte>();
                if (HasHitCountInt_16)
                    _offsets.HitCountOffset = Marshal.SizeOf<short>();
                if (HasHitCountInt_32)
                    _offsets.HitCountOffset = Marshal.SizeOf<int>();
                if (HasHitCountInt_64)
                    _offsets.HitCountOffset = Marshal.SizeOf<long>();

                if (HasHitCountUInt_8)
                    _offsets.HitCountOffset = Marshal.SizeOf<byte>();
                if (HasHitCountUInt_16)
                    _offsets.HitCountOffset = Marshal.SizeOf<ushort>();
                if (HasHitCountUInt_32)
                    _offsets.HitCountOffset = Marshal.SizeOf<uint>();
                if (HasHitCountUInt_64)
                    _offsets.HitCountOffset = Marshal.SizeOf<ulong>();

                if (HasHitCountFloat32)
                    _offsets.HitCountOffset = Marshal.SizeOf<float>();

                if (HasHitCountFloat64)
                    _offsets.HitCountOffset = Marshal.SizeOf<double>();

                // GPSTime
                if (HasGPSTimeInt_8)
                    _offsets.GPSTimeOffset = Marshal.SizeOf<sbyte>();
                if (HasGPSTimeInt_16)
                    _offsets.GPSTimeOffset = Marshal.SizeOf<short>();
                if (HasGPSTimeInt_32)
                    _offsets.GPSTimeOffset = Marshal.SizeOf<int>();
                if (HasGPSTimeInt_64)
                    _offsets.GPSTimeOffset = Marshal.SizeOf<long>();

                if (HasGPSTimeUInt_8)
                    _offsets.GPSTimeOffset = Marshal.SizeOf<byte>();
                if (HasGPSTimeUInt_16)
                    _offsets.GPSTimeOffset = Marshal.SizeOf<ushort>();
                if (HasGPSTimeUInt_32)
                    _offsets.GPSTimeOffset = Marshal.SizeOf<uint>();
                if (HasGPSTimeUInt_64)
                    _offsets.GPSTimeOffset = Marshal.SizeOf<ulong>();

                if (HasGPSTimeFloat32)
                    _offsets.GPSTimeOffset = Marshal.SizeOf<float>();

                if (HasGPSTimeFloat64)
                    _offsets.GPSTimeOffset = Marshal.SizeOf<double>();

                OffsetsCalculated = true;

                return _offsets;
            }
        }

        private DecodeRawPosition SetRawPositionMethod
        {
            get
            {
                if (HasPositionFloat3_32)
                {
                    return (ref TPoint pointIn, byte[] byteIn) =>
                    {
                        var offset = Marshal.SizeOf<float>();
                        var x = BitConverter.ToSingle(byteIn, 0);
                        var y = BitConverter.ToSingle(byteIn, offset);
                        var z = BitConverter.ToSingle(byteIn, offset * 2);

                        SetPositionFloat3_32(ref pointIn, new float3(x, y, z));
                    };
                }
                if (HasPositionFloat3_64)
                {
                    return (ref TPoint pointIn, byte[] byteIn) =>
                    {
                        var offset = Marshal.SizeOf<double>();
                        var x = BitConverter.ToDouble(byteIn, 0);
                        var y = BitConverter.ToDouble(byteIn, offset);
                        var z = BitConverter.ToDouble(byteIn, offset * 2);

                        SetPositionFloat3_64(ref pointIn, new double3(x, y, z));
                    };
                }
                return (ref TPoint pointIn, byte[] byteIn) => { }; // Do nothing
            }
        }

        private DecodeRawIntensity SetRawIntensityMethod
        {
            get
            {
                if (HasIntensityInt_8)
                    return (ref TPoint pointIn, byte[] byteIn) => SetIntensityInt_8(ref pointIn, (sbyte)byteIn[Offsets.PositionOffset]);
                if (HasIntensityInt_16)
                    return (ref TPoint pointIn, byte[] byteIn) => SetIntensityInt_16(ref pointIn, byteIn[Offsets.PositionOffset]);
                if (HasIntensityInt_32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetIntensityInt_32(ref pointIn, BitConverter.ToInt32(byteIn, Offsets.PositionOffset));
                if (HasIntensityInt_64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetIntensityInt_64(ref pointIn, BitConverter.ToInt64(byteIn, Offsets.PositionOffset));

                if (HasIntensityUInt_8)
                    return (ref TPoint pointIn, byte[] byteIn) => SetIntensityUInt_8(ref pointIn, byteIn[Offsets.PositionOffset]);
                if (HasIntensityUInt_16)
                    return (ref TPoint pointIn, byte[] byteIn) => SetIntensityUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, Offsets.PositionOffset));
                if (HasIntensityUInt_32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetIntensityUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, Offsets.PositionOffset));
                if (HasIntensityUInt_64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetIntensityUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, Offsets.PositionOffset));

                if (HasIntensityFloat32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetIntensityFloat32(ref pointIn, BitConverter.ToSingle(byteIn, Offsets.PositionOffset));
                if (HasIntensityFloat64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetIntensityFloat64(ref pointIn, BitConverter.ToDouble(byteIn, Offsets.PositionOffset));

                return (ref TPoint pointIn, byte[] byteIn) => { }; // Do nothing
            }
        }

        private DecodeRawNormals SetRawNormalsMethod
        {
            get
            {
                var offset = Offsets.PositionOffset + Offsets.IntensityOffset;

                if (HasNormalFloat3_32)
                {
                    return (ref TPoint pointIn, byte[] byteIn) =>
                    {
                        var dataOffset = Marshal.SizeOf<float>();
                        var x = BitConverter.ToSingle(byteIn, offset);
                        var y = BitConverter.ToSingle(byteIn, offset + dataOffset);
                        var z = BitConverter.ToSingle(byteIn, offset + dataOffset * 2);

                        SetNormalFloat3_32(ref pointIn, new float3(x, y, z));
                    };
                }
                if (HasNormalFloat3_64)
                {
                    return (ref TPoint pointIn, byte[] byteIn) =>
                    {
                        var dataOffset = Marshal.SizeOf<double>();

                        var x = BitConverter.ToDouble(byteIn, offset);
                        var y = BitConverter.ToDouble(byteIn, offset + dataOffset);
                        var z = BitConverter.ToDouble(byteIn, offset + dataOffset * 2);

                        SetNormalFloat3_64(ref pointIn, new double3(x, y, z));
                    };
                }

                return (ref TPoint pointIn, byte[] byteIn) => { }; // Do nothing
            }
        }

        private DecodeRawRGB SetRawRGBMethod
        {
            get
            {
                var offset = Offsets.PositionOffset + Offsets.IntensityOffset + Offsets.NormalsOffset;

                if (HasColorInt_8)
                    return (ref TPoint pointIn, byte[] byteIn) => SetColorInt_8(ref pointIn, (sbyte)byteIn[offset]);
                if (HasColorInt_16)
                    return (ref TPoint pointIn, byte[] byteIn) => SetColorInt_16(ref pointIn, byteIn[offset]);
                if (HasColorInt_32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetColorInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                if (HasColorInt_64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetColorInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));

                if (HasColorUInt_8)
                    return (ref TPoint pointIn, byte[] byteIn) => SetColorUInt_8(ref pointIn, byteIn[offset + 1]);
                if (HasColorUInt_16)
                    return (ref TPoint pointIn, byte[] byteIn) => SetColorUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                if (HasColorUInt_32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetColorUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                if (HasColorUInt_64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetColorUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));

                if (HasColorFloat32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetColorFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                if (HasColorFloat64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetColorFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));

                if (HasColorFloat3_32)
                {
                    return (ref TPoint pointIn, byte[] byteIn) =>
                    {
                        var dataOffset = Marshal.SizeOf<float>();

                        var x = BitConverter.ToSingle(byteIn, offset);
                        var y = BitConverter.ToSingle(byteIn, offset + dataOffset);
                        var z = BitConverter.ToSingle(byteIn, offset + dataOffset * 2);

                        SetColorFloat3_32(ref pointIn, new float3(x, y, z));
                    };
                }
                if (HasColorFloat3_64)
                {
                    return (ref TPoint pointIn, byte[] byteIn) =>
                    {
                        var dataOffset = Marshal.SizeOf<double>();

                        var x = BitConverter.ToDouble(byteIn, offset);
                        var y = BitConverter.ToDouble(byteIn, offset + dataOffset);
                        var z = BitConverter.ToDouble(byteIn, offset + dataOffset * 2);

                        SetColorFloat3_64(ref pointIn, new double3(x, y, z));
                    };
                }

                return (ref TPoint pointIn, byte[] byteIn) => { }; // Do nothing
            }

        }

        private DecodeRawLabel SetRawLabelMethod
        {
            get
            {
                var offset = Offsets.PositionOffset + Offsets.IntensityOffset + Offsets.NormalsOffset + Offsets.RGBOffset;

                if (HasLabelInt_8)
                    return (ref TPoint pointIn, byte[] byteIn) => SetLabelInt_8(ref pointIn, (sbyte)byteIn[offset]);
                if (HasLabelInt_16)
                    return (ref TPoint pointIn, byte[] byteIn) => SetLabelInt_16(ref pointIn, byteIn[offset]);
                if (HasLabelInt_32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetLabelInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                if (HasLabelInt_64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetLabelInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));

                if (HasLabelUInt_8)
                    return (ref TPoint pointIn, byte[] byteIn) => SetLabelUInt_8(ref pointIn, byteIn[offset + 1]);
                if (HasLabelUInt_16)
                    return (ref TPoint pointIn, byte[] byteIn) => SetLabelUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                if (HasLabelUInt_32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetLabelUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                if (HasLabelUInt_64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetLabelUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));

                if (HasLabelFloat32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetLabelFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                if (HasLabelFloat64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetLabelFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));

                return (ref TPoint pointIn, byte[] byteIn) => { }; // Do nothing
            }

        }

        private DecodeRawCurvature SetRawCurvatureMethod
        {
            get
            {
                var offset = Offsets.PositionOffset + Offsets.IntensityOffset + Offsets.NormalsOffset + Offsets.RGBOffset + Offsets.LabelOffset;

                if (HasCurvatureInt_8)
                    return (ref TPoint pointIn, byte[] byteIn) => SetCurvatureInt_8(ref pointIn, (sbyte)byteIn[offset]);
                if (HasCurvatureInt_16)
                    return (ref TPoint pointIn, byte[] byteIn) => SetCurvatureInt_16(ref pointIn, byteIn[offset]);
                if (HasCurvatureInt_32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetCurvatureInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                if (HasCurvatureInt_64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetCurvatureInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));

                if (HasCurvatureUInt_8)
                    return (ref TPoint pointIn, byte[] byteIn) => SetCurvatureUInt_8(ref pointIn, byteIn[offset + 1]);
                if (HasCurvatureUInt_16)
                    return (ref TPoint pointIn, byte[] byteIn) => SetCurvatureUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                if (HasCurvatureUInt_32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetCurvatureUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                if (HasCurvatureUInt_64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetCurvatureUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));

                if (HasCurvatureFloat32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetCurvatureFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                if (HasCurvatureFloat64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetCurvatureFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));

                return (ref TPoint pointIn, byte[] byteIn) => { }; // Do nothing
            }

        }

        private DecodeRawHitCount SetRawHitCountMethod
        {
            get
            {
                var offset = Offsets.PositionOffset + Offsets.IntensityOffset + Offsets.NormalsOffset + Offsets.RGBOffset + Offsets.LabelOffset;

                if (HasHitCountInt_8)
                    return (ref TPoint pointIn, byte[] byteIn) => SetHitCountInt_8(ref pointIn, (sbyte)byteIn[offset]);
                if (HasHitCountInt_16)
                    return (ref TPoint pointIn, byte[] byteIn) => SetHitCountInt_16(ref pointIn, byteIn[offset]);
                if (HasHitCountInt_32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetHitCountInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                if (HasHitCountInt_64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetHitCountInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));

                if (HasHitCountUInt_8)
                    return (ref TPoint pointIn, byte[] byteIn) => SetHitCountUInt_8(ref pointIn, byteIn[offset + 1]);
                if (HasHitCountUInt_16)
                    return (ref TPoint pointIn, byte[] byteIn) => SetHitCountUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                if (HasHitCountUInt_32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetHitCountUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                if (HasHitCountUInt_64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetHitCountUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));

                if (HasHitCountFloat32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetHitCountFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                if (HasHitCountFloat64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetHitCountFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));

                return (ref TPoint pointIn, byte[] byteIn) => { }; // Do nothing
            }

        }

        private DecodeRawGPSTime SetRawGPSTimeMethod
        {
            get
            {
                var offset = Offsets.PositionOffset + Offsets.IntensityOffset + Offsets.NormalsOffset + Offsets.RGBOffset + Offsets.LabelOffset + Offsets.HitCountOffset;

                if (HasGPSTimeInt_8)
                    return (ref TPoint pointIn, byte[] byteIn) => SetGPSTimeInt_8(ref pointIn, (sbyte)byteIn[offset]);
                if (HasGPSTimeInt_16)
                    return (ref TPoint pointIn, byte[] byteIn) => SetGPSTimeInt_16(ref pointIn, byteIn[offset]);
                if (HasGPSTimeInt_32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetGPSTimeInt_32(ref pointIn, BitConverter.ToInt32(byteIn, offset));
                if (HasGPSTimeInt_64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetGPSTimeInt_64(ref pointIn, BitConverter.ToInt64(byteIn, offset));

                if (HasGPSTimeUInt_8)
                    return (ref TPoint pointIn, byte[] byteIn) => SetGPSTimeUInt_8(ref pointIn, byteIn[offset + 1]);
                if (HasGPSTimeUInt_16)
                    return (ref TPoint pointIn, byte[] byteIn) => SetGPSTimeUInt_16(ref pointIn, BitConverter.ToUInt16(byteIn, offset));
                if (HasGPSTimeUInt_32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetGPSTimeUInt_32(ref pointIn, BitConverter.ToUInt32(byteIn, offset));
                if (HasGPSTimeUInt_64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetGPSTimeUInt_64(ref pointIn, BitConverter.ToUInt64(byteIn, offset));

                if (HasGPSTimeFloat32)
                    return (ref TPoint pointIn, byte[] byteIn) => SetGPSTimeFloat32(ref pointIn, BitConverter.ToSingle(byteIn, offset));
                if (HasGPSTimeFloat64)
                    return (ref TPoint pointIn, byte[] byteIn) => SetGPSTimeFloat64(ref pointIn, BitConverter.ToDouble(byteIn, offset));

                return (ref TPoint pointIn, byte[] byteIn) => { }; // Do nothing
            }
        }

        #endregion

        public byte[] GetRawPoint(ref TPoint point)
        {
            if (point == null)
                throw new NullReferenceException("Given point is null!");

            return GetRawPointMethod(ref point);
        }

        public void SetRawPoint(ref TPoint pointIn, byte[] byteIn)
        {
            if (pointIn == null || byteIn == null || byteIn.Length < 8)
                throw new NullReferenceException("Invalid data given");

            SetRawPointMethod(ref pointIn, byteIn);
        }

        #endregion
    }

    /// <summary>
    ///     A pointcloud consists of a point accessor which enables access to the 
    ///     as well as some information about the point type.
    ///     Furthermore the data itself as well as some meta information like offset information.
    /// </summary>   
    /// <typeparam name="TPoint">Point type</typeparam>
    public interface IPointcloud<TPoint>
    {
        PointAccessor<TPoint> Pa { get; }

        Span<TPoint> Points { get; }

        IMeta MetaInfo { get; }
    }
}
