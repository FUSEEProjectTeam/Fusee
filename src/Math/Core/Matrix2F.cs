#region Copyright(C) Notice
//	Fusee.Math math library. Based on the Sharp3D math library originally developed by
//	Copyright (C) 2003-2004  
//	Eran Kampf
//	tentacle@zahav.net.il
//	http://www.ekampf.com/Fusee.Math/
//
//	This library is free software; you can redistribute it and/or
//	modify it under the terms of the GNU Lesser General Public
//	License as published by the Free Software Foundation; either
//	version 2.1 of the License, or (at your option) any later version.
//
//	This library is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//	Lesser General Public License for more details.
//
//	You should have received a copy of the GNU Lesser General Public
//	License along with this library; if not, write to the Free Software
//	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
	/// <summary>
	/// Represents a 2-dimentional single-precision floating point matrix.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct Matrix2F : ISerializable, ICloneable
	{
		#region Private Fields
		private float _m11, _m12;
		private float _m21, _m22;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix2F"/> structure with the specified values.
		/// </summary>
		public Matrix2F(
			float m11, float m12,
			float m21, float m22
			)
		{
			_m11 = m11; _m12 = m12;
			_m21 = m21; _m22 = m22;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix2F"/> structure with the specified values.
		/// </summary>
		/// <param name="elements">An array containing the matrix values in row-major order.</param>
		public Matrix2F(float[] elements)
		{
			Debug.Assert(elements != null);
			Debug.Assert(elements.Length >= 4);

			_m11 = elements[0]; _m12 = elements[1];
			_m21 = elements[2]; _m22 = elements[3];
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix2F"/> structure with the specified values.
		/// </summary>
		/// <param name="elements">An array containing the matrix values in row-major order.</param>
		public Matrix2F(FloatArrayList elements)
		{
			Debug.Assert(elements != null);
			Debug.Assert(elements.Count >= 4);

			_m11 = elements[0]; _m12 = elements[1];
			_m21 = elements[2]; _m22 = elements[3];
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix2F"/> structure with the specified values.
		/// </summary>
		/// <param name="column1">A <see cref="Vector2D"/> instance holding values for the first column.</param>
		/// <param name="column2">A <see cref="Vector2D"/> instance holding values for the second column.</param>
		public Matrix2F(Vector2F column1, Vector2F column2)
		{
			_m11 = column1.X; _m12 = column2.X;
			_m21 = column1.Y; _m22 = column2.Y;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix2F"/> class using a given matrix.
		/// </summary>
		public Matrix2F(Matrix2F m)
		{
			_m11 = m.M11; _m12 = m.M12;
			_m21 = m.M21; _m22 = m.M22;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix2F"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		private Matrix2F(SerializationInfo info, StreamingContext context)
		{
			// Get the first row
			_m11 = info.GetSingle("M11");
			_m12 = info.GetSingle("M12");

			// Get the second row
			_m21 = info.GetSingle("M21");
			_m22 = info.GetSingle("M22");
		}
		#endregion

		#region Constants
		/// <summary>
		/// 2-dimentional single-precision floating point zero matrix.
		/// </summary>
		public static readonly Matrix2F Zero = new Matrix2F(0,0,0,0);
		/// <summary>
		/// 2-dimentional single-precision floating point identity matrix.
		/// </summary>
		public static readonly Matrix2F Identity = new Matrix2F(1,0,0,1);
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Matrix2F"/> object.
		/// </summary>
		/// <returns>The <see cref="Matrix2F"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Matrix2F(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Matrix2F"/> object.
		/// </summary>
		/// <returns>The <see cref="Matrix2F"/> object this method creates.</returns>
		public Matrix2F Clone()
		{
			return new Matrix2F(this);
		}
		#endregion

		#region ISerializable Members
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize this object.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data. </param>
		/// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
		//[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// First row
			info.AddValue("M11", _m11);
			info.AddValue("M12", _m12);

			// Second row
			info.AddValue("M21", _m21);
			info.AddValue("M22", _m22);
		}
		#endregion

		#region Public Static Matrix Arithmetics
		/// <summary>
		/// Adds two matrices.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2F"/> instance.</param>
		/// <returns>A new <see cref="Matrix2F"/> instance containing the sum.</returns>
		public static Matrix2F Add(Matrix2F a, Matrix2F b)
		{
			return new Matrix2F(
				a.M11 + b.M11, a.M12 + b.M12,
				a.M21 + b.M21, a.M22 + b.M22
				);
		}
		/// <summary>
		/// Adds a matrix and a scalar.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix2F"/> instance containing the sum.</returns>
		public static Matrix2F Add(Matrix2F a, float s)
		{
			return new Matrix2F(
				a.M11 + s, a.M12 + s,
				a.M21 + s, a.M22 + s
				);
		}
		/// <summary>
		/// Adds two matrices and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="result">A <see cref="Matrix2F"/> instance to hold the result.</param>
		public static void Add(Matrix2F a, Matrix2F b, ref Matrix2F result)
		{
			result.M11 = a.M11 + b.M11;
			result.M12 = a.M12 + b.M12;

			result.M21 = a.M21 + b.M21;
			result.M22 = a.M22 + b.M22;
		}
		/// <summary>
		/// Adds a matrix and a scalar and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="result">A <see cref="Matrix2F"/> instance to hold the result.</param>
		public static void Add(Matrix2F a, float s, ref Matrix2F result)
		{
			result.M11 = a.M11 + s;
			result.M12 = a.M12 + s;

			result.M21 = a.M21 + s;
			result.M22 = a.M22 + s;
		}
		/// <summary>
		/// Subtracts a matrix from a matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance to subtract from.</param>
		/// <param name="b">A <see cref="Matrix2F"/> instance to subtract.</param>
		/// <returns>A new <see cref="Matrix2F"/> instance containing the difference.</returns>
		/// <remarks>result[x][y] = a[x][y] - b[x][y]</remarks>
		public static Matrix2F Subtract(Matrix2F a, Matrix2F b)
		{
			return new Matrix2F(
				a.M11 - b.M11, a.M12 - b.M12,
				a.M21 - b.M21, a.M22 - b.M22
				);
		}
		/// <summary>
		/// Subtracts a scalar from a matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix2F"/> instance containing the difference.</returns>
		public static Matrix2F Subtract(Matrix2F a, float s)
		{
			return new Matrix2F(
				a.M11 - s, a.M12 - s,
				a.M21 - s, a.M22 - s
				);
		}
		/// <summary>
		/// Subtracts a matrix from a matrix and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance to subtract from.</param>
		/// <param name="b">A <see cref="Matrix2F"/> instance to subtract.</param>
		/// <param name="result">A <see cref="Matrix2F"/> instance to hold the result.</param>
		/// <remarks>result[x][y] = a[x][y] - b[x][y]</remarks>
		public static void Subtract(Matrix2F a, Matrix2F b, ref Matrix2F result)
		{
			result.M11 = a.M11 - b.M11;
			result.M12 = a.M12 - b.M12;

			result.M21 = a.M21 - b.M21;
			result.M22 = a.M22 - b.M22;
		}
		/// <summary>
		/// Subtracts a scalar from a matrix and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="result">A <see cref="Matrix2F"/> instance to hold the result.</param>
		public static void Subtract(Matrix2F a, float s, ref Matrix2F result)
		{
			result.M11 = a.M11 - s;
			result.M12 = a.M12 - s;

			result.M21 = a.M21 - s;
			result.M22 = a.M22 - s;
		}
		/// <summary>
		/// Multiplies two matrices.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2F"/> instance.</param>
		/// <returns>A new <see cref="Matrix2F"/> instance containing the result.</returns>
		public static Matrix2F Multiply(Matrix2F a, Matrix2F b)
		{
			return new Matrix2F(
				a.M11 * b.M11 + a.M12 * b.M21,
				a.M11 * b.M12 + a.M12 * b.M22,
				a.M11 * b.M12 + a.M12 * b.M22,
				a.M21 * b.M12 + a.M22 * b.M22
				);
		}
		/// <summary>
		/// Multiplies two matrices and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="result">A <see cref="Matrix2F"/> instance to hold the result.</param>
		public static void Multiply(Matrix2F a, Matrix2F b, ref Matrix2F result)
		{
			result.M11 = a.M11 * b.M11 + a.M12 * b.M21;
			result.M12 = a.M11 * b.M12 + a.M12 * b.M22;
			result.M21 = a.M11 * b.M12 + a.M12 * b.M22;
			result.M22 = a.M21 * b.M12 + a.M22 * b.M22;
		}		
		/// <summary>
		/// Transforms a given vector by a matrix.
		/// </summary>
		/// <param name="matrix">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="vector">A <see cref="Vector2F"/> instance.</param>
		/// <returns>A new <see cref="Vector2F"/> instance containing the result.</returns>
		public static Vector2F Transform(Matrix2F matrix, Vector2F vector)
		{
			return new Vector2F(
				(matrix.M11 * vector.X) + (matrix.M12 * vector.Y),
				(matrix.M21 * vector.X) + (matrix.M22 * vector.Y));
		}
		/// <summary>
		/// Transforms a given vector by a matrix and put the result in a vector.
		/// </summary>
		/// <param name="matrix">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="vector">A <see cref="Vector2F"/> instance.</param>
		/// <param name="result">A <see cref="Vector2F"/> instance to hold the result.</param>
		public static void Transform(Matrix2F matrix, Vector2F vector, ref Vector2F result)
		{
			result.X = (matrix.M11 * vector.X) + (matrix.M12 * vector.Y);
			result.Y = (matrix.M21 * vector.X) + (matrix.M22 * vector.Y);
		}
		/// <summary>
		/// Transposes a matrix.
		/// </summary>
		/// <param name="m">A <see cref="Matrix2F"/> instance.</param>
		/// <returns>A new <see cref="Matrix2F"/> instance containing the transposed matrix.</returns>
		public static Matrix2F Transpose(Matrix2F m)
		{
			Matrix2F t = new Matrix2F(m);
			t.Transpose();
			return t;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the value of the [1,1] matrix element.
		/// </summary>
		public float M11
		{
			get { return _m11; }
			set { _m11 = value;}
		}
		/// <summary>
		/// Gets or sets the value of the [1,2] matrix element.
		/// </summary>
		public float M12
		{
			get { return _m12; }
			set { _m12 = value;}
		}
		/// <summary>
		/// Gets or sets the value of the [2,1] matrix element.
		/// </summary>
		public float M21
		{
			get { return _m21; }
			set { _m21 = value;}
		}
		/// <summary>
		/// Gets or sets the value of the [2,2] matrix element.
		/// </summary>
		public float M22
		{
			get { return _m22; }
			set { _m22 = value;}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return 
				_m11.GetHashCode() ^ _m12.GetHashCode() ^
				_m21.GetHashCode() ^ _m22.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Matrix2F"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Matrix2F)
			{
				Matrix2F m = (Matrix2F)obj;
				return 
					(_m11 == m.M11) && (_m12 == m.M12) &&
					(_m11 == m.M21) && (_m12 == m.M22);
			}
			return false;
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			s.Append(String.Format( "|{0}, {1}|\n", M11, M12));
			s.Append(String.Format( "|{0}, {1}|\n", M21, M22));

			return s.ToString();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Calculates the determinant value of the matrix.
		/// </summary>
		/// <returns>The determinant value of the matrix.</returns>
		public float Determinant()
		{
			return (_m11*_m22) - (_m12*_m21);
		}
		/// <summary>
		/// Calculates the trace the matrix which is the sum of its diagonal elements.
		/// </summary>
		/// <returns>Returns the trace value of the matrix.</returns>
		public float Trace()
		{
			return _m11 + _m22;
		}
		/// <summary>
		/// Transposes this matrix.
		/// </summary>
		public void Transpose()
		{
			MathFunctions.Swap(ref _m12, ref _m21);
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified matrices are equal.
		/// </summary>
		/// <param name="a">The left-hand matrix.</param>
		/// <param name="b">The right-hand matrix.</param>
		/// <returns><see langword="true"/> if the two matrices are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(Matrix2F a, Matrix2F b)
		{
			return ValueType.Equals(a,b);
		}
		/// <summary>
		/// Tests whether two specified matrices are not equal.
		/// </summary>
		/// <param name="a">The left-hand matrix.</param>
		/// <param name="b">The right-hand matrix.</param>
		/// <returns><see langword="true"/> if the two matrices are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(Matrix2F a, Matrix2F b)
		{
			return !ValueType.Equals(a,b);
		}
		#endregion

		#region Binary Operators
		/// <summary>
		/// Adds two matrices.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2F"/> instance.</param>
		/// <returns>A new <see cref="Matrix2F"/> instance containing the sum.</returns>
		public static Matrix2F operator+(Matrix2F a, Matrix2F b)
		{
			return Matrix2F.Add(a,b);
		}
		/// <summary>
		/// Adds a matrix and a scalar.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix2F"/> instance containing the sum.</returns>
		public static Matrix2F operator+(Matrix2F a, float s)
		{
			return Matrix2F.Add(a,s);
		}
		/// <summary>
		/// Adds a matrix and a scalar.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix2F"/> instance containing the sum.</returns>
		public static Matrix2F operator+(float s, Matrix2F a)
		{
			return Matrix2F.Add(a,s);
		}
		/// <summary>
		/// Subtracts a matrix from a matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2F"/> instance.</param>
		/// <returns>A new <see cref="Matrix2F"/> instance containing the difference.</returns>
		public static Matrix2F operator-(Matrix2F a, Matrix2F b)
		{
			return Matrix2F.Subtract(a,b);
		}
		/// <summary>
		/// Subtracts a scalar from a matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix2F"/> instance containing the difference.</returns>
		public static Matrix2F operator-(Matrix2F a, float s)
		{
			return Matrix2F.Subtract(a,s);
		}
		/// <summary>
		/// Multiplies two matrices.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2F"/> instance.</param>
		/// <returns>A new <see cref="Matrix2F"/> instance containing the result.</returns>
		public static Matrix2F operator*(Matrix2F a, Matrix2F b)
		{
			return Matrix2F.Multiply(a,b);
		}
		/// <summary>
		/// Transforms a given vector by a matrix.
		/// </summary>
		/// <param name="matrix">A <see cref="Matrix2F"/> instance.</param>
		/// <param name="vector">A <see cref="Vector2F"/> instance.</param>
		/// <returns>A new <see cref="Vector2F"/> instance containing the result.</returns>
		public static Vector2F operator*(Matrix2F matrix, Vector2F vector)
		{
			return Matrix2F.Transform(matrix, vector);
		}
		#endregion

		#region Indexing Operators
		/// <summary>
		/// Indexer allowing to access the matrix elements by an index
		/// where index = 2*row + column.
		/// </summary>
		public unsafe float this [int index] 
		{			
			get 
			{
				if (index < 0 || index >= 4)
					throw new IndexOutOfRangeException("Invalid matrix index!");

				fixed(float *f = &_m11) 
				{
					return *(f+index);
				}
			}
			set 
			{			
				if (index < 0 || index >= 4)
					throw new IndexOutOfRangeException("Invalid matrix index!");

				fixed(float *f = &_m11) 
				{
					*(f+index) = value;
				}
			}			
		}
		/// <summary>
		/// Indexer allowing to access the matrix elements by row and column.
		/// </summary>
		public float this[int row, int column]
		{
			get 
			{
				return this[ row *2 + column ];
			}
			set 
			{				
				this[ row *2 + column ] = value;
			}			
		}
		#endregion
	}
}
