using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Math.ArrayConversion
{
    /// <summary>
    /// Contains functions to convert number Arrays to\from Fusee.Math structures.
    /// </summary>
    public sealed class Convert
    {
        #region Matrix Conversion
        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Matrix4F"/> to an Array of floats. The row major order storage of the 
        /// original matrix is kept. This way the returned array can be used in environments taking row major order matrices
        /// (e.g. OpenGL). Use ToArray[]ColOrder for converting the original matrix to environments taking column major order
        /// matrices (like e.g. DirectX).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A float array containing 16 values in row major order [m11, m12, m13, ...].</returns>
        public static float[] Matrix4FToArrayFloatRowOrder(Fusee.Math.Core.Matrix4F value)
        {
            return new float[] {value[0, 0], value[0, 1], value[0, 2], value[0, 3],
                                value[1, 0], value[1, 1], value[1, 2], value[1, 3],
                                value[2, 0], value[2, 1], value[2, 2], value[2, 3],
                                value[3, 0], value[3, 1], value[3, 2], value[3, 3]};
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Matrix4F"/> to an Array of floats. The row major order storage of the 
        /// original matrix is converted to column major order (the matrix is transposed before exporting it to an Array).
        /// This way the returned array can be used in environments taking column major order
        /// matrices (like e.g. DirectX). Use ToArray[]RowOrder for converting the original matrix to environments taking 
        /// row major order matrices (e.g. OpenGL).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A float array containing 16 values in column major order [m11, m21, m31, ...].</returns>
        public static float[] Matrix4FToArrayFloatColOrder(Fusee.Math.Core.Matrix4F value)
        {
            return new float[] {value[0, 0], value[1, 0], value[2, 0], value[3, 0],
                                value[0, 1], value[1, 1], value[2, 1], value[3, 1],
                                value[0, 2], value[1, 2], value[2, 2], value[3, 2],
                                value[0, 3], value[1, 3], value[2, 3], value[3, 3]};
        }
        
        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Matrix4F"/> to an Array of doubles. The row major order storage of the 
        /// original matrix is kept. This way the returned array can be used in environments taking row major order matrices
        /// (e.g. OpenGL). Use ToArray[]ColOrder for converting the original matrix to environments taking column major order
        /// matrices (like e.g. DirectX).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A double array containing 16 values in row major order [m11, m12, m13, ...].</returns>
        public static double[] Matrix4FToArrayDoubleRowOrder(Fusee.Math.Core.Matrix4F value)
        {
            return new double[] {value[0, 0], value[0, 1], value[0, 2], value[0, 3],
                                 value[1, 0], value[1, 1], value[1, 2], value[1, 3],
                                 value[2, 0], value[2, 1], value[2, 2], value[2, 3],
                                 value[3, 0], value[3, 1], value[3, 2], value[3, 3]};
        }
        
        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Matrix4F"/> to an Array of double values. The row major order storage of the 
        /// original matrix is converted to column major order (the matrix is transposed before exporting it to an Array).
        /// This way the returned array can be used in environments taking column major order
        /// matrices (like e.g. DirectX). Use ToArray[]RowOrder for converting the original matrix to environments taking 
        /// row major order matrices (e.g. OpenGL).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A double array containing 16 values in column major order [m11, m21, m31, ...].</returns>
        public static double[] Matrix4FToArrayDoubleColOrder(Fusee.Math.Core.Matrix4F value)
        {
            return new double[] {value[0, 0], value[1, 0], value[2, 0], value[3, 0],
                                 value[0, 1], value[1, 1], value[2, 1], value[3, 1],
                                 value[0, 2], value[1, 2], value[2, 2], value[3, 2],
                                 value[0, 3], value[1, 3], value[2, 3], value[3, 3]};
        }


        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Matrix4D"/> to an Array of floats. The row major order storage of the 
        /// original matrix is kept. This way the returned array can be used in environments taking row major order matrices
        /// (e.g. OpenGL). Use ToArray[]ColOrder for converting the original matrix to environments taking column major order
        /// matrices (like e.g. DirectX).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A float array containing 16 values in row major order [m11, m12, m13, ...].</returns>
        public static float[] Matrix4DToArrayFloatRowOrder(Fusee.Math.Core.Matrix4D value)
        {
            return new float[] {(float)value[0, 0], (float)value[0, 1], (float)value[0, 2], (float)value[0, 3],
                                (float)value[1, 0], (float)value[1, 1], (float)value[1, 2], (float)value[1, 3],
                                (float)value[2, 0], (float)value[2, 1], (float)value[2, 2], (float)value[2, 3],
                                (float)value[3, 0], (float)value[3, 1], (float)value[3, 2], (float)value[3, 3]};
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Matrix4D"/> to an Array of floats. The row major order storage of the 
        /// original matrix is converted to column major order (the matrix is transposed before exporting it to an Array).
        /// This way the returned array can be used in environments taking column major order
        /// matrices (like e.g. DirectX). Use ToArray[]RowOrder for converting the original matrix to environments taking 
        /// row major order matrices (e.g. OpenGL).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A float array containing 16 values in column major order [m11, m21, m31, ...].</returns>
        public static float[] Matrix4DToArrayFloatColOrder(Fusee.Math.Core.Matrix4D value)
        {
            return new float[] {(float)value[0, 0], (float)value[1, 0], (float)value[2, 0], (float)value[3, 0],
                                (float)value[0, 1], (float)value[1, 1], (float)value[2, 1], (float)value[3, 1],
                                (float)value[0, 2], (float)value[1, 2], (float)value[2, 2], (float)value[3, 2],
                                (float)value[0, 3], (float)value[1, 3], (float)value[2, 3], (float)value[3, 3]};
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Matrix4D"/> to an Array of doubles. The row major order storage of the 
        /// original matrix is kept. This way the returned array can be used in environments taking row major order matrices
        /// (e.g. OpenGL). Use ToArray[]ColOrder for converting the original matrix to environments taking column major order
        /// matrices (like e.g. DirectX).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A double array containing 16 values in row major order [m11, m12, m13, ...].</returns>
        public static double[] Matrix4DToArrayDoubleRowOrder(Fusee.Math.Core.Matrix4D value)
        {
            return new double[] {value[0, 0], value[0, 1], value[0, 2], value[0, 3],
                                 value[1, 0], value[1, 1], value[1, 2], value[1, 3],
                                 value[2, 0], value[2, 1], value[2, 2], value[2, 3],
                                 value[3, 0], value[3, 1], value[3, 2], value[3, 3]};
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Matrix4D"/> to an Array of double values. The row major order storage of the 
        /// original matrix is converted to column major order (the matrix is transposed before exporting it to an Array).
        /// This way the returned array can be used in environments taking column major order
        /// matrices (like e.g. DirectX). Use ToArray[]RowOrder for converting the original matrix to environments taking 
        /// row major order matrices (e.g. OpenGL).
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A double array containing 16 values in column major order [m11, m21, m31, ...].</returns>
        public static double[] Matrix4DToArrayDoubleColOrder(Fusee.Math.Core.Matrix4D value)
        {
            return new double[] {value[0, 0], value[1, 0], value[2, 0], value[3, 0],
                                 value[0, 1], value[1, 1], value[2, 1], value[3, 1],
                                 value[0, 2], value[1, 2], value[2, 2], value[3, 2],
                                 value[0, 3], value[1, 3], value[2, 3], value[3, 3]};
        }


        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Matrix4D"/> to an Array of double values in the somewhat awkward Cinema 4D Matrix layout.
        /// This is a 3 rows, 4 columns matrix stored as column vectors with the "offset" vector first (although internally the C4D Matrix arithmetics
        /// is rather row major order oriented). Anyway, the layout of the resulting array will be [m14, m24, m34, m11, m21, m31, m12, m22, m32, m13, m23, m33].
        /// The "projection part" of the original 4x4 matrix is ignored.
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A double array containing 12 double values in the Cinema 4D matrix layout described above.</returns>
        public static double[] Matrix4DToArrayDoubleC4DLayout(Fusee.Math.Core.Matrix4D value)
        {
            return new double[] {value[0, 3], value[1, 3], value[2, 3],
                                 value[0, 0], value[1, 0], value[2, 0],
                                 value[0, 1], value[1, 1], value[2, 1],
                                 value[0, 2], value[1, 2], value[2, 2]};
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.Core.Matrix4D"/> to an existing array of double values in the somewhat awkward Cinema 4D Matrix layout.
        /// This is a 3 rows, 4 columns matrix stored as column vectors with the "offset" vector first (although internally the C4D Matrix arithmetics
        /// is rather row major order oriented). Anyway, the layout of the resulting array will be [m14, m24, m34, m11, m21, m31, m12, m22, m32, m13, m23, m33].
        /// The "projection part" of the original 4x4 matrix is ignored.
        /// </summary>
        /// <param name="value">The matrix value to convert.</param>
        /// <returns>A double array containing 12 double values in the Cinema 4D matrix layout described above.</returns>
        public static unsafe void Matrix4DToArrayDoubleC4DLayout(Fusee.Math.Core.Matrix4D value, double *pDst)
        {
            pDst[0] = value[0, 3];
            pDst[1] = value[1, 3]; 
            pDst[2] = value[2, 3];

            pDst[3] = value[0, 0]; 
            pDst[4] = value[1, 0]; 
            pDst[5] = value[2, 0];
  
            pDst[6] = value[0, 1]; 
            pDst[7] = value[1, 1]; 
            pDst[8] = value[2, 1];

            pDst[9] = value[0, 2]; 
            pDst[10] = value[1, 2]; 
            pDst[11] = value[2, 2];
        }


        /// <summary>
        /// Creates a new <see cref="Fusee.Math.Core.Matrix4D"/> from an Array of twelve double values in the somewhat awkward Cinema 4D Matrix layout.
        /// This is a 3 rows, 4 columns matrix stored as column vectors with the "offset" vector first (although internally the C4D Matrix arithmetics
        /// is rather row major order oriented). Anyway, the layout of the parameter array is expected as follows [m14, m24, m34, m11, m21, m31, m12, m22, m32, m13, m23, m33].
        /// The "projection part" of the resulting 4x4 matrix (the lower row) is always set to [0, 0, 0, 1].
        /// </summary>
        /// <param name="value">An array containing 12 double values in the Cinema 4D matrix layout described above.</param>
        /// <returns>A newly created <see cref="Fusee.Math.Core.Matrix4D"/> object</returns>
        public static unsafe Fusee.Math.Core.Matrix4D ArrayDoubleC4DLayoutToMatrix4D(double* pValue)
        {
            return new Fusee.Math.Core.Matrix4D( pValue[3], pValue[6], pValue[9],  pValue[0],
                                                   pValue[4], pValue[7], pValue[10], pValue[1],
                                                   pValue[5], pValue[8], pValue[11], pValue[2],
                                                   0,         0,         0,          1);
        }

        // TODO: add all the other FromMatrix methods
        // TODO: add all 16 from and to methods for all the other matrix types
        #endregion

        #region Vector2 Conversion
        // TODO:
        #endregion

        #region Vector3 Conversion
        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Vector3F"/> to  a float array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A float array containing three values [x, y, z]</returns>
        public static float[] Vector3FToArrayFloat(Fusee.Math.Core.Vector3F value)
        {
            return new float[]{value.X, value.Y, value.Z};
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Vector3F"/> to  a double array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A double array containing three values [x, y, z]</returns>
        public static double[] Vector3FToArrayDouble(Fusee.Math.Core.Vector3F value)
        {
            return new double[] { value.X, value.Y, value.Z };
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Vector3D"/> to  a float array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A float array containing three values [x, y, z]</returns>
        public static float[] Vector3DToArrayFloat(Fusee.Math.Core.Vector3D value)
        {
            return new float[] { (float)value.X, (float)value.Y, (float)value.Z };
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Vector3D"/> to  a double array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A double array containing three values [x, y, z]</returns>
        public static double[] Vector3DToArrayDouble(Fusee.Math.Core.Vector3D value)
        {
            return new double[] { value.X, value.Y, value.Z };
        }


        /// <summary>
        /// Copys a <see cref="Fusee.Math.Core.Vector3F"/> to an existing float array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination float array [x, y, z]</returns>
        public static unsafe void Vector3FToArrayFloat(Fusee.Math.Core.Vector3F value, float* pDst)
        {
            pDst[0] = value.X;
            pDst[1] = value.Y;
            pDst[2] = value.Z;
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.Core.Vector3F"/> to an existing double array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination double array [x, y, z]</param>
        public static unsafe void Vector3FToArrayDouble(Fusee.Math.Core.Vector3F value, double* pDst)
        {
            pDst[0] = value.X;
            pDst[1] = value.Y;
            pDst[2] = value.Z;
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.Core.Vector3D"/> to an existing float array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination float array [x, y, z]</param>
        public static unsafe void Vector3DToArrayFloat(Fusee.Math.Core.Vector3D value, float* pDst)
        {
            pDst[0] = (float)value.X;
            pDst[1] = (float)value.Y;
            pDst[2] = (float)value.Z;
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.Core.Vector3D"/> to an existing double array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination double array [x, y, z]</param>
        public static unsafe void Vector3DToArrayDouble(Fusee.Math.Core.Vector3D value, double* pDst)
        {
            pDst[0] = value.X;
            pDst[1] = value.Y;
            pDst[2] = value.Z;
        }

 
        /// <summary>
        /// Creates a new <see cref="Fusee.Math.Core.Vector3F"/> from a given float array.
        /// </summary>
        /// <param name="value">A float array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.Core.Vector3F"/> object</returns>
        public static unsafe Fusee.Math.Core.Vector3F ArrayFloatToVector3F(float *pValue)
        {
            return new Fusee.Math.Core.Vector3F(pValue[0], pValue[1], pValue[2]);
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.Core.Vector3F"/> from a given double array.
        /// </summary>
        /// <param name="value">A double array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.Core.Vector3F"/> object</returns>
        public static unsafe Fusee.Math.Core.Vector3F ArrayDoubleToVector3F(double* pValue)
        {
            return new Fusee.Math.Core.Vector3F((float)pValue[0], (float)pValue[1], (float)pValue[2]);
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.Core.Vector3D"/> from a given float array.
        /// </summary>
        /// <param name="value">A float array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.Core.Vector3D"/> object</returns>
        public static unsafe Fusee.Math.Core.Vector3D ArrayFloatToVector3D(float* pValue)
        {
            return new Fusee.Math.Core.Vector3D(pValue[0], pValue[1], pValue[2]);
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.Core.Vector3D"/> from a given double array.
        /// </summary>
        /// <param name="value">A double array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.Core.Vector3D"/> object</returns>
        public static unsafe Fusee.Math.Core.Vector3D ArrayDoubleToVector3D(double* pValue)
        {
            return new Fusee.Math.Core.Vector3D(pValue[0], pValue[1], pValue[2]);
        }
        #endregion

        #region Vector4 Conversion
        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Vector4F"/> to  a float array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A float array containing three values [x, y, z]</returns>
        public static float[] Vector4FToArrayFloat(Fusee.Math.Core.Vector4F value)
        {
            return new float[]{value.X, value.Y, value.Z, value.W };
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Vector4F"/> to  a double array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A double array containing three values [x, y, z]</returns>
        public static double[] Vector4FToArrayDouble(Fusee.Math.Core.Vector4F value)
        {
            return new double[] { value.X, value.Y, value.Z, value.W };
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Vector4D"/> to  a float array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A float array containing three values [x, y, z]</returns>
        public static float[] Vector4DToArrayFloat(Fusee.Math.Core.Vector4D value)
        {
            return new float[] { (float)value.X, (float)value.Y, (float)value.Z, (float)value.W };
        }

        /// <summary>
        /// Converts a <see cref="Fusee.Math.Core.Vector4D"/> to  a double array.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>A double array containing three values [x, y, z]</returns>
        public static double[] Vector4DToArrayDouble(Fusee.Math.Core.Vector4D value)
        {
            return new double[] { value.X, value.Y, value.Z, value.W };
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.Core.Vector4F"/> from a given float array.
        /// </summary>
        /// <param name="value">A float array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.Core.Vector4F"/> object</returns>
        public static unsafe Fusee.Math.Core.Vector4F ArrayFloatToVector4F(float* pValue)
        {
            return new Fusee.Math.Core.Vector4F(pValue[0], pValue[1], pValue[2], pValue[3]);
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.Core.Vector4F"/> from a given double array.
        /// </summary>
        /// <param name="value">A double array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.Core.Vector4F"/> object</returns>
        public static unsafe Fusee.Math.Core.Vector4F ArrayDoubleToVector4F(double* pValue)
        {
            return new Fusee.Math.Core.Vector4F((float)pValue[0], (float)pValue[1], (float)pValue[2], (float)pValue[3]);
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.Core.Vector4D"/> from a given float array.
        /// </summary>
        /// <param name="value">A float array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.Core.Vector4D"/> object</returns>
        public static unsafe Fusee.Math.Core.Vector4D ArrayFloatToVector4D(float* pValue)
        {
            return new Fusee.Math.Core.Vector4D(pValue[0], pValue[1], pValue[2], pValue[3]);
        }

        /// <summary>
        /// Creates a new <see cref="Fusee.Math.Core.Vector4D"/> from a given double array.
        /// </summary>
        /// <param name="value">A double array containing three values [x, y, z]</param>
        /// <returns>The new <see cref="Fusee.Math.Core.Vector4D"/> object</returns>
        public static unsafe Fusee.Math.Core.Vector4D ArrayDoubleToVector4D(double* pValue)
        {
            return new Fusee.Math.Core.Vector4D(pValue[0], pValue[1], pValue[2], pValue[3]);
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.Core.Vector4F"/> to an existing float array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination float array [x, y, z]</param>
        public static unsafe void Vector4FToArrayFloat(Fusee.Math.Core.Vector4F value, float* pDst)
        {
            pDst[0] = value.X;
            pDst[1] = value.Y;
            pDst[2] = value.Z;
            pDst[3] = value.W;
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.Core.Vector4F"/> to an existing double array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination double array [x, y, z]</param>
        public static unsafe void Vector4FToArrayDouble(Fusee.Math.Core.Vector4F value, double* pDst)
        {
            pDst[0] = value.X;
            pDst[1] = value.Y;
            pDst[2] = value.Z;
            pDst[3] = value.W;
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.Core.Vector4D"/> to an existing float array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination float array [x, y, z]</param>
        public static unsafe void Vector4DToArrayFloat(Fusee.Math.Core.Vector4D value, float* pDst)
        {
            pDst[0] = (float)value.X;
            pDst[1] = (float)value.Y;
            pDst[2] = (float)value.Z;
            pDst[3] = (float)value.W;
        }

        /// <summary>
        /// Copys a <see cref="Fusee.Math.Core.Vector4D"/> to an existing double array.
        /// </summary>
        /// <param name="value">The source vector to convert.</param>
        /// <param name="pDst">A pointer to the destination double array [x, y, z]</param>
        public static unsafe void Vector4DToArrayDouble(Fusee.Math.Core.Vector4D value, double* pDst)
        {
            pDst[0] = value.X;
            pDst[1] = value.Y;
            pDst[2] = value.Z;
            pDst[3] = value.W;
        }

        #endregion


        #region Quaternion Conversion
        // TODO
        #endregion

        #region Private Constructor
        private Convert()
        {
        }
        #endregion
    }
}
