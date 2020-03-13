﻿using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    ///     Represents a 3x3 Matrix
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct float3x3 : IEquatable<float3x3>
    {
        #region Fields

        /// <summary>
        ///     Top row of the matrix
        /// </summary>
        public float3 Row0;

        /// <summary>
        ///     2nd row of the matrix
        /// </summary>
        public float3 Row1;

        /// <summary>
        ///     3rd row of the matrix
        /// </summary>
        public float3 Row2;

        /// <summary>
        ///     The identity matrix
        /// </summary>
        public static float3x3 Identity = new float3x3(float3.UnitX, float3.UnitY, float3.UnitZ);

        /// <summary>
        ///     The zero matrix
        /// </summary>
        public static float3x3 Zero = new float3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

        #endregion Fields

        #region Constructors

        /// <summary>
        ///     Constructs a new instance.
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
        ///     Constructs a new instance.
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
        ///     Constructs a new instance.
        /// </summary>
        /// <param name="mat4">The incoming float4x4.</param>
        public float3x3(float4x4 mat4)
        {
            Row0 = new float3(mat4.Row0.x, mat4.Row0.y, mat4.Row0.z);
            Row1 = new float3(mat4.Row1.x, mat4.Row1.y, mat4.Row1.z);
            Row2 = new float3(mat4.Row2.x, mat4.Row2.y, mat4.Row2.z);
        }

        #endregion Constructors

        #region Public Members

        #region Properties

        /// <summary>
        ///     The determinant of this matrix
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
        ///     The first column of this matrix
        /// </summary>
        public float3 Column0
        {
            get { return new float3(Row0.x, Row1.x, Row2.x); }
            set { Row0.x = value.x; Row1.x = value.y; Row2.x = value.z; }
        }

        /// <summary>
        ///     The second column of this matrix
        /// </summary>
        public float3 Column1
        {
            get { return new float3(Row0.y, Row1.y, Row2.y); }
            set { Row0.y = value.x; Row1.y = value.y; Row2.y = value.z; }
        }

        /// <summary>
        ///     The third column of this matrix
        /// </summary>
        public float3 Column2
        {
            get { return new float3(Row0.z, Row1.z, Row2.z); }
            set { Row0.z = value.x; Row1.z = value.y; Row2.z = value.z; }
        }

        /// <summary>
        ///     Gets and sets the value at row 1, column 1 of this instance.
        /// </summary>
        public float M11
        {
            get { return Row0.x; }
            set { Row0.x = value; }
        }

        /// <summary>
        ///     Gets and sets the value at row 1, column 2 of this instance.
        /// </summary>
        public float M12
        {
            get { return Row0.y; }
            set { Row0.y = value; }
        }

        /// <summary>
        ///     Gets and sets the value at row 1, column 3 of this instance.
        /// </summary>
        public float M13
        {
            get { return Row0.z; }
            set { Row0.z = value; }
        }

        /// <summary>
        ///     Gets and sets the value at row 2, column 1 of this instance.
        /// </summary>
        public float M21
        {
            get { return Row1.x; }
            set { Row1.x = value; }
        }

        /// <summary>
        ///     Gets and sets the value at row 2, column 2 of this instance.
        /// </summary>
        public float M22
        {
            get { return Row1.y; }
            set { Row1.y = value; }
        }

        /// <summary>
        ///     Gets and sets the value at row 2, column 3 of this instance.
        /// </summary>
        public float M23
        {
            get { return Row1.z; }
            set { Row1.z = value; }
        }

        /// <summary>
        ///     Gets and sets the value at row 3, column 1 of this instance.
        /// </summary>
        public float M31
        {
            get { return Row2.x; }
            set { Row2.x = value; }
        }

        /// <summary>
        ///     Gets and sets the value at row 3, column 2 of this instance.
        /// </summary>
        public float M32
        {
            get { return Row2.y; }
            set { Row2.y = value; }
        }

        /// <summary>
        ///     Gets and sets the value at row 3, column 3 of this instance.
        /// </summary>
        public float M33
        {
            get { return Row2.z; }
            set { Row2.z = value; }
        }

        #endregion Properties



        #region Instance

        #region public Transpose()

        /// <summary>
        ///     Returns the transposes of this instance.
        /// </summary>
        public float3x3 Transpose()
        {
            return Transpose(this);
        }

        #endregion public Transpose()

        #endregion Instance

        #region Static

        #region Elementary Arithmetic Functions

        /// <summary>
        ///     Adds two instances.
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
        ///     Subtracts the right instance from the left instance.
        /// </summary>
        /// <param name="left">The left operand of the subtraction.</param>
        /// <param name="right">The right operand of the subtraction.</param>
        /// <returns>A new instance that is the result of the subtraction.</returns>
        public static float3x3 Subtract(float3x3 left, float3x3 right)
        {
            return new float3x3(left.M11 - right.M11, left.M12 - right.M12, left.M13 - right.M13,
                left.M21 - right.M21, left.M22 - right.M22, left.M23 - right.M23,
                left.M31 - right.M31, left.M32 - right.M32, left.M33 - right.M33);
        }

        #endregion Elementary Arithmetic Functions

        #region Multiply Functions

        /// <summary>
        ///     Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <returns>A new instance that is the result of the multiplication</returns>
        public static float3x3 Mult(float3x3 left, float3x3 right)
        {
            if (left == Identity) return right;
            if (right == Identity) return left;
            if (left == Zero || right == Zero) return Zero;

            float3x3 result = new float3x3(
                left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31,
                left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32,
                left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33,
                left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31,
                left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32,
                left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33,
                left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31,
                left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32,
                left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33);

            return result;
        }

        #endregion Multiply Functions

        #region Transpose

        /// <summary>
        ///     Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <returns>The transpose of the given matrix</returns>
        public static float3x3 Transpose(float3x3 mat)
        {
            return new float3x3(mat.Column0, mat.Column1, mat.Column2);
        }

        #endregion Transpose

        #region Transform

        /// <summary>
        ///     Transforms a given vector by a matrix via matrix*vector (post-multiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float3x3" /> instance.</param>
        /// <param name="vector">A <see cref="float3" /> instance.</param>
        /// <returns>A new <see cref="float3" /> instance containing the result.</returns>
        public static float3 Transform(float3x3 matrix, float3 vector)
        {
            float3 result;

            result = new float3((matrix.M11 * vector.x) + (matrix.M12 * vector.y) + (matrix.M13 * vector.z),
                                (matrix.M21 * vector.x) + (matrix.M22 * vector.y) + (matrix.M23 * vector.z),
                                (matrix.M31 * vector.x) + (matrix.M32 * vector.y) + (matrix.M33 * vector.z));

            return result;
        }

        /// <summary>
        ///     Transforms a given vector by a matrix via vector*matrix (pre-multiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float3x3" /> instance.</param>
        /// <param name="vector">A <see cref="float3" /> instance.</param>
        /// <returns>A new <see cref="float3" /> instance containing the result.</returns>
        public static float3 Transform(float3 vector, float3x3 matrix)
        {
            float3 result;

            result = new float3((matrix.M11 * vector.x) + (matrix.M21 * vector.y) + (matrix.M31 * vector.z),
                                (matrix.M12 * vector.x) + (matrix.M22 * vector.y) + (matrix.M32 * vector.z),
                                (matrix.M13 * vector.x) + (matrix.M23 * vector.y) + (matrix.M33 * vector.z));

            return result;
        }

        /// <summary>
        ///     Transforms a given vector by a matrix via matrix*vector (post-multiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float3x3" /> instance.</param>
        /// <param name="vector">A <see cref="float2" /> instance.</param>
        /// <returns>A new <see cref="float2" /> instance containing the result.</returns>
        public static float2 Transform(float3x3 matrix, float2 vector)
        {
            float2 result;

            float3 temp = new float3(vector.x, vector.y, 1);
            result = Transform(matrix, temp).xy;

            return result;
        }

        /// <summary>
        ///     Transforms a given vector by a matrix via matrix*vector (post-multiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float3x3" /> instance.</param>
        /// <param name="vector">A <see cref="float2" /> instance.</param>
        /// <returns>A new <see cref="float2" /> instance containing the result.</returns>
        public static float2 Transform(float2 vector, float3x3 matrix)
        {
            float2 result;

            float3 temp = new float3(vector.x, vector.y, 1);
            result = Transform(temp, matrix).xy;

            return result;
        }

        #endregion Transform

        #endregion Static

        #region Operators

        /// <summary>
        ///     Matrix addition
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new float3x3 which holds the result of the multiplication</returns>
        public static float3x3 operator +(float3x3 left, float3x3 right)
        {
            return Add(left, right);
        }

        /// <summary>
        ///     Matrix subtraction
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new float2x2 which holds the result of the multiplication</returns>
        public static float3x3 operator -(float3x3 left, float3x3 right)
        {
            return Subtract(left, right);
        }

        /// <summary>
        ///     Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Float3x3 which holds the result of the multiplication</returns>
        public static float3x3 operator *(float3x3 left, float3x3 right)
        {
            return Mult(left, right);
        }

        /// <summary>
        ///     Transforms a given vector by a matrix via matrix*vector (post-multiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float3x3" /> instance.</param>
        /// <param name="vector">A <see cref="float3" /> instance.</param>
        /// <returns>A new <see cref="float3" /> instance containing the result.</returns>
        public static float3 operator *(float3x3 matrix, float3 vector)
        {
            return Transform(matrix, vector);
        }

        /// <summary>
        ///     Transforms a given vector by a matrix via vector*matrix (pre-multiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float3x3" /> instance.</param>
        /// <param name="vector">A <see cref="float3" /> instance.</param>
        /// <returns>A new <see cref="float3" /> instance containing the result.</returns>
        public static float3 operator *(float3 vector, float3x3 matrix)
        {
            return Transform(vector, matrix);
        }

        /// <summary>
        ///     Transforms a given vector by a matrix via matrix*vector (post-multiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float3x3" /> instance.</param>
        /// <param name="vector">A <see cref="float2" /> instance.</param>
        /// <returns>A new <see cref="float2" /> instance containing the result.</returns>
        public static float2 operator *(float3x3 matrix, float2 vector)
        {
            return Transform(matrix, vector);
        }

        /// <summary>
        ///     Transforms a given vector by a matrix via matrix*vector (post-multiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float3x3" /> instance.</param>
        /// <param name="vector">A <see cref="float2" /> instance.</param>
        /// <returns>A new <see cref="float2" /> instance containing the result.</returns>
        public static float2 operator *(float2 vector, float3x3 matrix)
        {
            return Transform(vector, matrix);
        }

        /// <summary>
        ///     Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(float3x3 left, float3x3 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(float3x3 left, float3x3 right)
        {
            return !left.Equals(right);
        }

        #endregion Operators

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current float3x3.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current float3x3.
        /// </summary>
        /// <param name="provider">Provides information about a specific culture.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider provider)
        {
            return ConvertToString(provider);
        }

        internal string ConvertToString(IFormatProvider? provider)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            return String.Format(provider, "{0}\n{1}\n{2}", Row0.ToString(provider), Row1.ToString(provider), Row2.ToString(provider));
        }

        #endregion public override string ToString()

        #region public override int GetHashCode()

        /// <summary>
        ///     Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode();
        }

        #endregion public override int GetHashCode()

        #region public override bool Equals(object obj)

        /// <summary>
        ///     Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare tresult.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is float3x3))
                return false;

            return Equals((float3x3)obj);
        }

        #endregion public override bool Equals(object obj)

        #endregion Overrides

        #endregion Public Members

        #region ToArray()

        /// <summary>
        ///     Returns this matrix as an array
        /// </summary>
        public float[] ToArray()
        {
            return new[] { M11, M12, M13, M21, M22, M23, M31, M32, M33 };
        }

        #endregion

        #region IEquatable<Matri3x3> Members

        /// <summary>
        ///     Indicates whether the current matrix represents an affine transformation.
        /// </summary>
        /// <returns>true if the current matrix represents an affine transformation; otherwise, false.</returns>
        public bool IsAffine
        {
            get { return (Column2 == float3.UnitZ); }
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

        #endregion IEquatable<Matri3x3> Members

        /// <summary>
        /// Gets and sets the Converter object. Has the ability to convert a string to a float3x3.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, float3x3> ParseConverter { get; set; } = (x => float3x3.Parse(x));

        /// <summary>
        /// Parses a string into a float3x3.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static float3x3 Parse(string source, IFormatProvider? provider = null)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            char separator = M.GetNumericListSeparator(provider);

            string[] strings = source.Split(new char[] { separator, '(', ')', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (strings.Length != 9)
                throw new FormatException("String parse for float3x3 did not result in exactly 9 items.");

            float[] floats = new float[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                try
                {
                    floats[i] = float.Parse(strings[i], provider);
                }
                catch
                {
                    throw new FormatException();
                }
            }

            return new float3x3(floats[0], floats[1], floats[2], floats[3], floats[4], floats[5], floats[6], floats[7], floats[8]);
        }
    }
}