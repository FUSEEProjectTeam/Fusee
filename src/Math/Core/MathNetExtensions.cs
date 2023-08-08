#if MathNet

using System;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Fusee.Math.Core
{
    /// <summary>
    /// Vector extensions for MathNet compatibility
    /// </summary>
    public static class MathNetVectorExtension
    {
        #region FuseeToMathNet

        public static MathNet.Numerics.LinearAlgebra.Single.DenseVector ToMathNetSingleVector(this float3 f3)

        {
            float[] f = new float[] { f3.x, f3.y, f3.z };
            return new MathNet.Numerics.LinearAlgebra.Single.DenseVector(f);
        }

        public static MathNet.Numerics.LinearAlgebra.Single.DenseVector ToMathNetSingleVector(this double3 d3)
        {
            return ToMathNetSingleVector((float3)d3);
        }

        public static MathNet.Numerics.LinearAlgebra.Double.DenseVector ToMathNetDoubleVector(this double3 d3)
        {
            double[] d = new double[] { d3.x, d3.y, d3.z };

            return new MathNet.Numerics.LinearAlgebra.Double.DenseVector(d);
        }

        public static MathNet.Numerics.LinearAlgebra.Double.DenseVector ToMathNetDoubleVector(this float3 f3)
        {
            return ToMathNetDoubleVector(f3);
        }

        #endregion FuseeToMathNet

        #region MathNetToFusee

        public static float3 ToFuseeSingleVector(this MathNet.Numerics.LinearAlgebra.Single.DenseVector sdv)
        {
            if (sdv.Values.Length != 3)
                throw new ArgumentException();

            var f = sdv.Storage.AsArray();

            return new float3(f[0], f[1], f[2]);
        }

        public static float3 ToFuseeSingleVector(this MathNet.Numerics.LinearAlgebra.Double.DenseVector ddv)
        {
            if (ddv.Values.Length != 3)
                throw new ArgumentException();

            var d = ddv.Storage.AsArray();

            return new float3((float)d[0], (float)d[1], (float)d[2]);
        }

        public static double3 ToFuseeDoubleVector(this MathNet.Numerics.LinearAlgebra.Single.DenseVector sdv)
        {
            if (sdv.Values.Length != 3)
                throw new ArgumentException();

            var f = sdv.Storage.AsArray();

            return new double3(f[0], f[1], f[2]);
        }

        public static double3 ToFuseeDoubleVector(this MathNet.Numerics.LinearAlgebra.Double.DenseVector ddv)
        {
            if (ddv.Values.Length != 3)
                throw new ArgumentException();

            var d = ddv.Storage.AsArray();

            return new double3(d[0], d[1], d[2]);
        }

        #endregion MathNetToFusee
    }

    /// <summary>
    /// Matrix extensions for MathNet compatibility
    /// </summary>
    public static class MathNetMatrixExtension
    {
        #region FuseeToMathNet

        public static MathNet.Numerics.LinearAlgebra.Single.DenseMatrix ToMathNetSingleMatrix(this float4x4 f4x4)
        {
            float[] f = new float[] {  f4x4.M11, f4x4.M21, f4x4.M31, f4x4.M41,
                                       f4x4.M12, f4x4.M22, f4x4.M32, f4x4.M42,
                                       f4x4.M13, f4x4.M23, f4x4.M33, f4x4.M43,
                                       f4x4.M14, f4x4.M24, f4x4.M34, f4x4.M44
            };

            return new MathNet.Numerics.LinearAlgebra.Single.DenseMatrix(4, 4, f);
        }

        public static MathNet.Numerics.LinearAlgebra.Single.DenseMatrix ToMathNetSingleMatrix(this double4x4 d4x4)
        {
            return ToMathNetSingleMatrix((float4x4)d4x4);
        }

        public static MathNet.Numerics.LinearAlgebra.Double.DenseMatrix ToMathNetDoubleMatrix(this double4x4 d4x4)
        {
            double[] f = new double[] { d4x4.M11, d4x4.M21, d4x4.M31, d4x4.M41,
                                        d4x4.M12, d4x4.M22, d4x4.M32, d4x4.M42,
                                        d4x4.M13, d4x4.M23, d4x4.M33, d4x4.M43,
                                        d4x4.M14, d4x4.M24, d4x4.M34, d4x4.M44
            };

            return new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(4, 4, f);
        }

        public static MathNet.Numerics.LinearAlgebra.Double.DenseMatrix ToMathNetDoubleMatrix(this float4x4 f4x4)
        {
            return ToMathNetDoubleMatrix((double4x4)f4x4);
        }

        #endregion FuseeToMathNet

        #region MathNetToFusee

        public static float4x4 ToFuseeSingleMatrix(this MathNet.Numerics.LinearAlgebra.Single.DenseMatrix sdm)
        {
            if (sdm.Values.Length != 16 || sdm.ColumnCount != 4 || sdm.RowCount != 4)
                throw new ArgumentException();

            return new float4x4
            {
                M11 = sdm.Values[0],
                M21 = sdm.Values[1],
                M31 = sdm.Values[2],
                M41 = sdm.Values[3],
                M12 = sdm.Values[4],
                M22 = sdm.Values[5],
                M32 = sdm.Values[6],
                M42 = sdm.Values[7],
                M13 = sdm.Values[8],
                M23 = sdm.Values[9],
                M33 = sdm.Values[10],
                M43 = sdm.Values[11],
                M14 = sdm.Values[12],
                M24 = sdm.Values[13],
                M34 = sdm.Values[14],
                M44 = sdm.Values[15]
            };
        }

        public static float4x4 ToFuseeSingleMatrix(this MathNet.Numerics.LinearAlgebra.Double.DenseMatrix ddm)
        {
            if (ddm.Values.Length != 16 || ddm.ColumnCount != 4 || ddm.RowCount != 4)
                throw new ArgumentException();

            return new float4x4
            {
                M11 = (float)ddm.Values[0],
                M21 = (float)ddm.Values[1],
                M31 = (float)ddm.Values[2],
                M41 = (float)ddm.Values[3],
                M12 = (float)ddm.Values[4],
                M22 = (float)ddm.Values[5],
                M32 = (float)ddm.Values[6],
                M42 = (float)ddm.Values[7],
                M13 = (float)ddm.Values[8],
                M23 = (float)ddm.Values[9],
                M33 = (float)ddm.Values[10],
                M43 = (float)ddm.Values[11],
                M14 = (float)ddm.Values[12],
                M24 = (float)ddm.Values[13],
                M34 = (float)ddm.Values[14],
                M44 = (float)ddm.Values[15]
            };
        }

        public static double4x4 ToFuseeDoubleMatrix(this MathNet.Numerics.LinearAlgebra.Single.DenseMatrix sdm)
        {
            if (sdm.Values.Length != 16 || sdm.ColumnCount != 4 || sdm.RowCount != 4)
                throw new ArgumentException();

            return new double4x4
            {
                M11 = sdm.Values[0],
                M21 = sdm.Values[1],
                M31 = sdm.Values[2],
                M41 = sdm.Values[3],
                M12 = sdm.Values[4],
                M22 = sdm.Values[5],
                M32 = sdm.Values[6],
                M42 = sdm.Values[7],
                M13 = sdm.Values[8],
                M23 = sdm.Values[9],
                M33 = sdm.Values[10],
                M43 = sdm.Values[11],
                M14 = sdm.Values[12],
                M24 = sdm.Values[13],
                M34 = sdm.Values[14],
                M44 = sdm.Values[15]
            };
        }

        public static double4x4 ToFuseeDoubleMatrix(this MathNet.Numerics.LinearAlgebra.Double.DenseMatrix ddm)
        {
            if (ddm.Values.Length != 16 || ddm.ColumnCount != 4 || ddm.RowCount != 4)
                throw new ArgumentException();

            return new double4x4
            {
                M11 = ddm.Values[0],
                M21 = ddm.Values[1],
                M31 = ddm.Values[2],
                M41 = ddm.Values[3],
                M12 = ddm.Values[4],
                M22 = ddm.Values[5],
                M32 = ddm.Values[6],
                M42 = ddm.Values[7],
                M13 = ddm.Values[8],
                M23 = ddm.Values[9],
                M33 = ddm.Values[10],
                M43 = ddm.Values[11],
                M14 = ddm.Values[12],
                M24 = ddm.Values[13],
                M34 = ddm.Values[14],
                M44 = ddm.Values[15]
            };
        }

        #endregion MathNetToFusee
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#endif