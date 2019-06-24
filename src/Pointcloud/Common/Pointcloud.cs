using Fusee.Math.Core;
using System;

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
