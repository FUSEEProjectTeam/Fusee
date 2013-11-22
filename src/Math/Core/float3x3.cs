using System;
using System.Runtime.InteropServices;

namespace Fusee.Math
{
    /// <summary>
    /// Represents a 3x3 Matrix
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable InconsistentNaming
    public struct float3x3 : IEquatable<float3x3>
    // ReSharper restore InconsistentNaming
    {
        #region Fields

        /// <summary>
        /// Top row of the matrix
        /// </summary>
        public float3 Row0;

        /// <summary>
        /// 2nd row of the matrix
        /// </summary>
        public float3 Row1;

        /// <summary>
        /// 3rd row of the matrix
        /// </summary>
        public float3 Row2;

        /// <summary>
        /// The identity matrix
        /// </summary>
        public static float3x3 Identity = new float3x3(float3.UnitX, float3.UnitY, float3.UnitZ);

        /// <summary>
        /// The zero matrix
        /// </summary>
        public static float3x3 Zero = new float3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="row0">Top row of the matrix</param>
        /// <param name="row1">Second row of the matrix</param>
        /// <param name="row2">Third row of the matrix</param>
        public float3x3(float3 row0, float3 row1, float3 row2)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="m00">First item of the first row of the matrix.</param>
        /// <param name="m01">Second item of the first row of the matrix.</param>
        /// <param name="m02">Third item of the first row of the matrix.</param>
        /// <param name="m10">First item of the second row of the matrix.</param>
        /// <param name="m11">Second item of the second row of the matrix.</param>
        /// <param name="m12">Third item of the second row of the matrix.</param>
        /// <param name="m20">First item of the third row of the matrix.</param>
        /// <param name="m21">Second item of the third row of the matrix.</param>
        /// <param name="m22">Third item of the third row of the matrix.</param>
        public float3x3(
            float m00, float m01, float m02,
            float m10, float m11, float m12,
            float m20, float m21, float m22)
        {
            Row0 = new float3(m00, m01, m02);
            Row1 = new float3(m10, m11, m12);
            Row2 = new float3(m20, m21, m22);
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="mat4">The incoming float4x4.</param>
        public float3x3(float4x4 mat4)
        {
            Row0 = new float3(mat4.Row0.x, mat4.Row0.y, mat4.Row0.z);
            Row1 = new float3(mat4.Row1.x, mat4.Row1.y, mat4.Row1.z);
            Row2 = new float3(mat4.Row2.x, mat4.Row2.y, mat4.Row2.z);
        }

        #endregion

        #region Public Members

        #region Properties

        /// <summary>
        /// The determinant of this matrix
        /// </summary>
        public float Determinant
        {
            get
            {
                return

                    Row0.x * Row1.y * Row2.z 
                    + Row0.y * Row1.z * Row2.x 
                    + Row0.z * Row1.x * Row2.y 
                    - Row0.z * Row1.y * Row2.x
                    - Row0.y * Row1.x * Row2.z 
                    - Row0.x * Row1.z * Row2.y;
            }
        }

        /// <summary>
        /// The first column of this matrix
        /// </summary>
        public float3 Column0
        {
            get { return new float3(Row0.x, Row1.x, Row2.x); }
        }

        /// <summary>
        /// The second column of this matrix
        /// </summary>
        public float3 Column1
        {
            get { return new float3(Row0.y, Row1.y, Row2.y); }
        }

        /// <summary>
        /// The third column of this matrix
        /// </summary>
        public float3 Column2
        {
            get { return new float3(Row0.z, Row1.z, Row2.z); }
        }

        /// <summary>
        /// Gets or sets the value at row 1, column 1 of this instance.
        /// </summary>
        public float M11
        {
            get { return Row0.x; }
            set { Row0.x = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 1, column 2 of this instance.
        /// </summary>
        public float M12
        {
            get { return Row0.y; }
            set { Row0.y = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 1, column 3 of this instance.
        /// </summary>
        public float M13
        {
            get { return Row0.z; }
            set { Row0.z = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 2, column 1 of this instance.
        /// </summary>
        public float M21
        {
            get { return Row1.x; }
            set { Row1.x = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 2, column 2 of this instance.
        /// </summary>
        public float M22
        {
            get { return Row1.y; }
            set { Row1.y = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 2, column 3 of this instance.
        /// </summary>
        public float M23
        {
            get { return Row1.z; }
            set { Row1.z = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 3, column 1 of this instance.
        /// </summary>
        public float M31
        {
            get { return Row2.x; }
            set { Row2.x = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 3, column 2 of this instance.
        /// </summary>
        public float M32
        {
            get { return Row2.y; }
            set { Row2.y = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 3, column 3 of this instance.
        /// </summary>
        public float M33
        {
            get { return Row2.z; }
            set { Row2.z = value; }
        }

        #endregion

        #region Instance

        #region public void Transpose()

        /// <summary>
        /// Converts this instance into its transpose.
        /// </summary>
        public void Transpose()
        {
            this = Transpose(this);
        }

        #region float[] ToArray()

        // ReSharper disable UnusedMember.Local
        private float[] ToArray()
        {
            return new[] { M11, M12, M13, M21, M22, M23, M31, M32, M33 };
            // return new float[] { M11, M21, M31, M12, M22, M32, M13, M23, M33 };
        }
        // ReSharper restore UnusedMember.Local

        #endregion

        #endregion

        #endregion

        #region Static

        #region Elementary Arithmetic Functions

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The left operand of the addition.</param>
        /// <param name="right">The right operand of the addition.</param>
        /// <returns>A new instance that is the result of the addition.</returns>
        public static float3x3 Add(float3x3 left, float3x3 right)
        {
            return new float3x3(left.M11 + right.M11, left.M12 + right.M12, left.M13 + right.M13,
                                left.M21 + right.M21, left.M22 + right.M22, left.M23 + right.M23,
                                left.M31 + right.M31, left.M32 + right.M32, left.M33 + right.M33);
        }

        /// <summary>
        /// Substracts the right instance from the left instance.
        /// </summary>
        /// <param name="left">The left operand of the substraction.</param>
        /// <param name="right">The right operand of the substraction.</param>
        /// <returns>A new instance that is the result of the substraction.</returns>
        public static float3x3 Substract(float3x3 left, float3x3 right)
        {
            return new float3x3(left.M11 - right.M11, left.M12 - right.M12, left.M13 - right.M13,
                                left.M21 - right.M21, left.M22 - right.M22, left.M23 - right.M23,
                                left.M31 - right.M31, left.M32 - right.M32, left.M33 - right.M33);
        }

        #endregion

        #region Multiply Functions

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <returns>A new instance that is the result of the multiplication</returns>
        public static float3x3 Mult(float3x3 left, float3x3 right)
        {
            if (left == Identity) return right;
            if (right == Identity) return left;
            if (left == Zero || right == Zero) return Zero;

            float3x3 result;

            Mult(ref left, ref right, out result);

            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <param name="result">A new instance that is the result of the multiplication</param>
        public static void Mult(ref float3x3 left, ref float3x3 right, out float3x3 result)
        {
            result = new float3x3(
                left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31,
                left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32,
                left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33,

                left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31,
                left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32,
                left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33,

                left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31,
                left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32,
                left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33);
        }

        #endregion

        #region Transpose

        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <returns>The transpose of the given matrix</returns>
        public static float3x3 Transpose(float3x3 mat)
        {
            return new float3x3(mat.Column0, mat.Column1, mat.Column2);
        }


        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <param name="result">The result of the calculation</param>
        public static void Transpose(ref float3x3 mat, out float3x3 result)
        {
            result.Row0 = mat.Column0;
            result.Row1 = mat.Column1;
            result.Row2 = mat.Column2;
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Matrix addition
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new float3x3 which holds the result of the multiplication</returns>
        public static float3x3 operator +(float3x3 left, float3x3 right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// Matrix substraction
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new float2x2 which holds the result of the multiplication</returns>
        public static float3x3 operator -(float3x3 left, float3x3 right)
        {
            return Substract(left, right);
        }

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Float3x3 which holds the result of the multiplication</returns>
        public static float3x3 operator *(float3x3 left, float3x3 right)
        {
            return Mult(left, right);
        }

        /// <summary>
        /// Transforms a given vector by a matrix via matrix*vector (Postmultiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float4"/> instance.</param>
        /// <returns>A new <see cref="float4"/> instance containing the result.</returns>
        public static float3 operator *(float3x3 matrix, float3 vector)
        {
            return new float3(  matrix.Column0.x * vector.x + matrix.Column1.x * vector.y + matrix.Column2.x * vector.z,
                                matrix.Column0.y * vector.x + matrix.Column1.y * vector.y + matrix.Column2.y * vector.z,
                                matrix.Column0.z * vector.x + matrix.Column1.z * vector.y + matrix.Column2.z * vector.z);
        }

        /// <summary>
        /// Transforms a given vector by a matrix via vector*matrix (Premultiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float4"/> instance.</param>
        /// <returns>A new <see cref="float4"/> instance containing the result.</returns>
        public static float3 operator *(float3 vector,float3x3 matrix)
        {
            return new float3(matrix.M11 * vector.x + matrix.M21 * vector.y + matrix.M31 * vector.z,
                                matrix.M12 * vector.x + matrix.M22 * vector.y + matrix.M32 * vector.z,
                                matrix.M13 * vector.x + matrix.M23 * vector.y + matrix.M33 * vector.z);
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(float3x3 left, float3x3 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(float3x3 left, float3x3 right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current Matrix44.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return String.Format("{0}\n{1}\n{2}", Row0, Row1, Row2);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyFieldInGetHashCode
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode();
            // ReSharper restore NonReadonlyFieldInGetHashCode
        }

        #endregion

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare tresult.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is float3x3))
                return false;

            return Equals((float3x3)obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Matri3x3> Members

        /// <summary>
        /// Indicates whether the current matrix represents an affine transformation.
        /// </summary>
        /// <returns>true if the current matrix represents an affine transformation; otherwise, false.</returns>
        public bool IsAffine
        {
            get
            {
                return (Column2 == float3.UnitZ);
            }
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="other">A matrix to compare with this matrix.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(float3x3 other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2;
        }

        #endregion
    }
}