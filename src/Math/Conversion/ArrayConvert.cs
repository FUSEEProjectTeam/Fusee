using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Math
{
    /// <summary>
    /// Contains functions to convert number Arrays to\from Fusee.Math structures.
    /// </summary>
    public sealed class ArrayConvert
    {
     
        
        #region Matrix Conversion
        /// <summary>
        /// Converts a <see cref="Fusee.Math.float4x4"/> to an Array of floats. The row major order storage of the 
        /// original matrix is kept. This way the returned array can be used in environments taking row major order matrices
        /// (e.g. OpenGL). Use ToArray[]ColOrder for converting the original matrix to environments taking column major order
        /// matrices (like e.g. DirectX).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A float array containing 16 values in row major order [m11, m12, m13, ...].</returns>
        public static float[] float4x4ToArrayFloatRowOrder(Fusee.Math.float4x4 value)
        {
            return new float[] {value.M11, value.M12, value.M13, value.M14,
                                value.M21, value.M22, value.M23, value.M24,
                                value.M31, value.M32, value.M33, value.M34,
                                value.M41, value.M42, value.M43, value.M44};
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.float4x4"/> to an Array of floats. The row major order storage of the 
        /// original matrix is converted to column major order (the matrix is transposed before exporting it to an Array).
        /// This way the returned array can be used in environments taking column major order
        /// matrices (like e.g. DirectX). Use ToArray[]RowOrder for converting the original matrix to environments taking 
        /// row major order matrices (e.g. OpenGL).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A float array containing 16 values in column major order [m11, m21, m31, ...].</returns>
        public static float[] float4x4ToArrayFloatColOrder(Fusee.Math.float4x4 value)
        {
            return new float[] {value.M11, value.M21, value.M31, value.M41,
                                value.M12, value.M22, value.M32, value.M42,
                                value.M13, value.M23, value.M33, value.M43,
                                value.M14, value.M24, value.M34, value.M44};
        }
        
        /// <summary>
        /// Converts a <see cref="Fusee.Math.float4x4"/> to an Array of doubles. The row major order storage of the 
        /// original matrix is kept. This way the returned array can be used in environments taking row major order matrices
        /// (e.g. OpenGL). Use ToArray[]ColOrder for converting the original matrix to environments taking column major order
        /// matrices (like e.g. DirectX).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A double array containing 16 values in row major order [m11, m12, m13, ...].</returns>
        public static double[] float4x4ToArrayDoubleRowOrder(Fusee.Math.float4x4 value)
        {
            return new double[] {value.M11, value.M12, value.M13, value.M14,
                                 value.M21, value.M22, value.M23, value.M24,
                                 value.M31, value.M32, value.M33, value.M34,
                                 value.M41, value.M42, value.M43, value.M44};
        }
        
        /// <summary>
        /// Converts a <see cref="Fusee.Math.float4x4"/> to an Array of double values. The row major order storage of the 
        /// original matrix is converted to column major order (the matrix is transposed before exporting it to an Array).
        /// This way the returned array can be used in environments taking column major order
        /// matrices (like e.g. DirectX). Use ToArray[]RowOrder for converting the original matrix to environments taking 
        /// row major order matrices (e.g. OpenGL).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A double array containing 16 values in column major order [m11, m21, m31, ...].</returns>
        public static double[] float4x4ToArrayDoubleColOrder(Fusee.Math.float4x4 value)
        {
            return new double[] {value.M11, value.M21, value.M31, value.M41,
                                 value.M12, value.M22, value.M32, value.M42,
                                 value.M13, value.M23, value.M33, value.M43,
                                 value.M14, value.M24, value.M34, value.M44};
        }


        /// <summary>
        /// Converts a <see cref="Fusee.Math.double4x4"/> to an Array of floats. The row major order storage of the 
        /// original matrix is kept. This way the returned array can be used in environments taking row major order matrices
        /// (e.g. OpenGL). Use ToArray[]ColOrder for converting the original matrix to environments taking column major order
        /// matrices (like e.g. DirectX).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A float array containing 16 values in row major order [m11, m12, m13, ...].</returns>
        public static float[] double4x4ToArrayFloatRowOrder(Fusee.Math.double4x4 value)
        {
            return new float[] {(float)value.M11, (float)value.M12, (float)value.M13, (float)value.M14,
                                (float)value.M21, (float)value.M22, (float)value.M23, (float)value.M24,
                                (float)value.M31, (float)value.M32, (float)value.M33, (float)value.M34,
                                (float)value.M41, (float)value.M42, (float)value.M43, (float)value.M44};
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.double4x4"/> to an Array of floats. The row major order storage of the 
        /// original matrix is converted to column major order (the matrix is transposed before exporting it to an Array).
        /// This way the returned array can be used in environments taking column major order
        /// matrices (like e.g. DirectX). Use ToArray[]RowOrder for converting the original matrix to environments taking 
        /// row major order matrices (e.g. OpenGL).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A float array containing 16 values in column major order [m11, m21, m31, ...].</returns>
        public static float[] double4x4ToArrayFloatColOrder(Fusee.Math.double4x4 value)
        {
            return new float[] {(float)value.M11, (float)value.M21, (float)value.M31, (float)value.M41,
                                (float)value.M12, (float)value.M22, (float)value.M32, (float)value.M42,
                                (float)value.M13, (float)value.M23, (float)value.M33, (float)value.M43,
                                (float)value.M14, (float)value.M24, (float)value.M34, (float)value.M44};
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.double4x4"/> to an Array of doubles. The row major order storage of the 
        /// original matrix is kept. This way the returned array can be used in environments taking row major order matrices
        /// (e.g. OpenGL). Use ToArray[]ColOrder for converting the original matrix to environments taking column major order
        /// matrices (like e.g. DirectX).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A double array containing 16 values in row major order [m11, m12, m13, ...].</returns>
        public static double[] double4x4ToArrayDoubleRowOrder(Fusee.Math.double4x4 value)
        {
            return new double[] {value.M11, value.M12, value.M13, value.M14,
                                 value.M21, value.M22, value.M23, value.M24,
                                 value.M31, value.M32, value.M33, value.M34,
                                 value.M41, value.M42, value.M43, value.M44};
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.double4x4"/> to an Array of double values. The row major order storage of the 
        /// original matrix is converted to column major order (the matrix is transposed before exporting it to an Array).
        /// This way the returned array can be used in environments taking column major order
        /// matrices (like e.g. DirectX). Use ToArray[]RowOrder for converting the original matrix to environments taking 
        /// row major order matrices (e.g. OpenGL).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A double array containing 16 values in column major order [m11, m21, m31, ...].</returns>
        public static double[] double4x4ToArrayDoubleColOrder(Fusee.Math.double4x4 value)
        {
            return new double[] {value.M11, value.M21, value.M31, value.M41,
                                 value.M12, value.M22, value.M32, value.M42,
                                 value.M13, value.M23, value.M33, value.M43,
                                 value.M14, value.M24, value.M34, value.M44};
        }


        /// <summary>
        /// Converts a <see cref="Fusee.Math.double4x4"/> to an Array of double values in the somewhat awkward Cinema 4D Matrix layout.
        /// This is a 3 rows, 4 columns matrix stored as column vectors with the "offset" vector first (although internally the C4D Matrix arithmetics
        /// is rather row major order oriented). Anyway, the layout of the resulting array will be [m14, m24, m34, m11, m21, m31, m12, m22, m32, m13, m23, m33].
        /// The "projection part" of the original 4x4 matrix is ignored.
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A double array containing 12 double values in the Cinema 4D matrix layout described above.</returns>
        public static double[] double4x4ToArrayDoubleC4DLayout(Fusee.Math.double4x4 value)
        {
            return new double[] {value.M14, value.M24, value.M34,
                                 value.M11, value.M21, value.M31,
                                 value.M12, value.M22, value.M32,
                                 value.M13, value.M23, value.M33};
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.double4x4"/> to an existing array of double values in the somewhat awkward Cinema 4D Matrix layout.
        /// This is a 3 rows, 4 columns matrix stored as column vectors with the "offset" vector first (although internally the C4D Matrix arithmetics
        /// is rather row major order oriented). Anyway, the layout of the resulting array will be [m14, m24, m34, m11, m21, m31, m12, m22, m32, m13, m23, m33].
        /// The "projection part" of the original 4x4 matrix is ignored.
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <param name="pDst">The destination pointer to write the matrix values to.</param>
        /// <returns>A double array containing 12 double values in the Cinema 4D matrix layout described above.</returns>
        public static unsafe void double4x4ToArrayDoubleC4DLayout(Fusee.Math.double4x4 value, double *pDst)
        {
            pDst[0] = value.M14;
            pDst[1] = value.M24; 
            pDst[2] = value.M34;

            pDst[3] = value.M11; 
            pDst[4] = value.M21; 
            pDst[5] = value.M31;
  
            pDst[6] = value.M12; 
            pDst[7] = value.M22; 
            pDst[8] = value.M32;

            pDst[9] = value.M13; 
            pDst[10] = value.M23; 
            pDst[11] = value.M33;
        }


        /// <summary>
        /// Creates a new <see cref="Fusee.Math.double4x4"/> from an Array of twelve double values in the somewhat awkward Cinema 4D Matrix layout.
        /// This is a 3 rows, 4 columns matrix stored as column vectors with the "offset" vector first (although internally the C4D Matrix arithmetics
        /// is rather row major order oriented). Anyway, the layout of the parameter array is expected as follows [m14, m24, m34, m11, m21, m31, m12, m22, m32, m13, m23, m33].
        /// The "projection part" of the resulting 4x4 matrix (the lower row) is always set to [0, 0, 02.
        /// </summary>
        /// <param name="pValue">An array containing 12 double values in the Cinema 4D matrix layout described above.</param>
        /// <returns>A newly created <see cref="Fusee.Math.double4x4"/> object</returns>
        public static unsafe Fusee.Math.double4x4 ArrayDoubleC4DLayoutTodouble4x4(double* pValue)
        {
            return new Fusee.Math.double4x4(pValue[3], pValue[6], pValue[9], pValue[0],
                                            pValue[4], pValue[7], pValue[10], pValue[1],
                                            pValue[5], pValue[8], pValue[11], pValue[2],
                                            0, 0, 0, 1);
        }

        // TODO: add all the other FromMatrix methods
        // TODO: add all 16 from and to methods for all the other matrix types
        #endregion

        #region Vector2 Conversion
        // TODO:
        #endregion

        #region Vector3 Conversion
        /// <summary>
        /// Converts a <see cref="Fusee.Math.float3"/> to  a float array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A float array containing three values [x, y, z]</returns>
        public static float[] float3ToArrayFloat(Fusee.Math.float3 value)
        {
            return new float[]{value.x, value.y, value.z};
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.float3"/> to  a double array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A double array containing three values [x, y, z]</returns>
        public static double[] float3ToArrayDouble(Fusee.Math.float3 value)
        {
            return new double[] { value.x, value.y, value.z };
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.double3"/> to  a float array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A float array containing three values [x, y, z]</returns>
        public static float[] double3ToArrayFloat(Fusee.Math.double3 value)
        {
            return new float[] { (float)value.x, (float)value.y, (float)value.z };
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.double3"/> to  a double array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A double array containing three values [x, y, z]</returns>
        public static double[] double3ToArrayDouble(Fusee.Math.double3 value)
        {
            return new double[] { value.x, value.y, value.z };
        }


        /// <summary>
        /// Copys a <see cref="Fusee.Math.float3"/> to an existing float array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination float array [x, y, z]</param>
        public static unsafe void float3ToArrayFloat(Fusee.Math.float3 value, float* pDst)
        {
            pDst[0] = value.x;
            pDst[1] = value.y;
            pDst[2] = value.z;
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.float3"/> to an existing double array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination double array [x, y, z]</param>
        public static unsafe void float3ToArrayDouble(Fusee.Math.float3 value, double* pDst)
        {
            pDst[0] = value.x;
            pDst[1] = value.y;
            pDst[2] = value.z;
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.double3"/> to an existing float array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination float array [x, y, z]</param>
        public static unsafe void double3ToArrayFloat(Fusee.Math.double3 value, float* pDst)
        {
            pDst[0] = (float)value.x;
            pDst[1] = (float)value.y;
            pDst[2] = (float)value.z;
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.double3"/> to an existing double array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination double array [x, y, z]</param>
        public static unsafe void double3ToArrayDouble(Fusee.Math.double3 value, double* pDst)
        {
            pDst[0] = value.x;
            pDst[1] = value.y;
            pDst[2] = value.z;
        }

 
        /// <summary>
        /// Creates a new <see cref="Fusee.Math.float3"/> from a given float array.
        /// </summary>
        /// <param name="pValue">A float array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.float3"/> object</returns>
        public static unsafe Fusee.Math.float3 ArrayFloatTofloat3(float *pValue)
        {
            return new Fusee.Math.float3(pValue[0], pValue[1], pValue[2]);
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.float3"/> from a given double array.
        /// </summary>
        /// <param name="pValue">A double array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.float3"/> object</returns>
        public static unsafe Fusee.Math.float3 ArrayDoubleTofloat3(double* pValue)
        {
            return new Fusee.Math.float3((float)pValue[0], (float)pValue[1], (float)pValue[2]);
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.double3"/> from a given float array.
        /// </summary>
        /// <param name="pValue">A float array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.double3"/> object</returns>
        public static unsafe Fusee.Math.double3 ArrayFloatTodouble3(float* pValue)
        {
            return new Fusee.Math.double3(pValue[0], pValue[1], pValue[2]);
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.double3"/> from a given double array.
        /// </summary>
        /// <param name="pValue">A double array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.double3"/> object</returns>
        public static unsafe Fusee.Math.double3 ArrayDoubleTodouble3(double* pValue)
        {
            return new Fusee.Math.double3(pValue[0], pValue[1], pValue[2]);
        }
        #endregion

        #region Vector4 Conversion
        /// <summary>
        /// Converts a <see cref="Fusee.Math.float4"/> to  a float array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A float array containing three values [x, y, z]</returns>
        public static float[] float4ToArrayFloat(Fusee.Math.float4 value)
        {
            return new float[]{value.x, value.y, value.z, value.w };
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.float4"/> to  a double array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A double array containing three values [x, y, z]</returns>
        public static double[] float4ToArrayDouble(Fusee.Math.float4 value)
        {
            return new double[] { value.x, value.y, value.z, value.w };
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.double4"/> to  a float array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A float array containing three values [x, y, z]</returns>
        public static float[] double4ToArrayFloat(Fusee.Math.double4 value)
        {
            return new float[] { (float)value.x, (float)value.y, (float)value.z, (float)value.w };
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.double4"/> to  a double array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A double array containing three values [x, y, z]</returns>
        public static double[] double4ToArrayDouble(Fusee.Math.double4 value)
        {
            return new double[] { value.x, value.y, value.z, value.w };
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.float4"/> from a given float array.
        /// </summary>
        /// <param name="pValue">A float array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.float4"/> object</returns>
        public static unsafe Fusee.Math.float4 ArrayFloatTofloat4(float* pValue)
        {
            return new Fusee.Math.float4(pValue[0], pValue[1], pValue[2], pValue[3]);
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.float4"/> from a given double array.
        /// </summary>
        /// <param name="pValue">A double array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.float4"/> object</returns>
        public static unsafe Fusee.Math.float4 ArrayDoubleTofloat4(double* pValue)
        {
            return new Fusee.Math.float4((float)pValue[0], (float)pValue[1], (float)pValue[2], (float)pValue[3]);
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.double4"/> from a given float array.
        /// </summary>
        /// <param name="pValue">A float array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.double4"/> object</returns>
        public static unsafe Fusee.Math.double4 ArrayFloatTodouble4(float* pValue)
        {
            return new Fusee.Math.double4(pValue[0], pValue[1], pValue[2], pValue[3]);
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.double4"/> from a given double array.
        /// </summary>
        /// <param name="pValue">A double array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.double4"/> object</returns>
        public static unsafe Fusee.Math.double4 ArrayDoubleTodouble4(double* pValue)
        {
            return new Fusee.Math.double4(pValue[0], pValue[1], pValue[2], pValue[3]);
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.float4"/> to an existing float array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination float array [x, y, z]</param>
        public static unsafe void float4ToArrayFloat(Fusee.Math.float4 value, float* pDst)
        {
            pDst[0] = value.x;
            pDst[1] = value.y;
            pDst[2] = value.z;
            pDst[3] = value.w;
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.float4"/> to an existing double array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination double array [x, y, z]</param>
        public static unsafe void float4ToArrayDouble(Fusee.Math.float4 value, double* pDst)
        {
            pDst[0] = value.x;
            pDst[1] = value.y;
            pDst[2] = value.z;
            pDst[3] = value.w;
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.double4"/> to an existing float array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination float array [x, y, z]</param>
        public static unsafe void double4ToArrayFloat(Fusee.Math.double4 value, float* pDst)
        {
            pDst[0] = (float)value.x;
            pDst[1] = (float)value.y;
            pDst[2] = (float)value.z;
            pDst[3] = (float)value.w;
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.double4"/> to an existing double array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination double array [x, y, z]</param>
        public static unsafe void double4ToArrayDouble(Fusee.Math.double4 value, double* pDst)
        {
            pDst[0] = value.x;
            pDst[1] = value.y;
            pDst[2] = value.z;
            pDst[3] = value.w;
        }

        #endregion


        #region Quaternion Conversion
        // TODO
        #endregion

        #region Private Constructor
        private ArrayConvert()
        {
        }
        #endregion
    }
}
