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
	/// Represents a 2-dimentional double-precision floating point matrix.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct Matrix2D : ISerializable, ICloneable
	{
		#region Private Fields
		private double _m11, _m12;
		private double _m21, _m22;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix2D"/> structure with the specified values.
		/// </summary>
		public Matrix2D(
			double m11, double m12,
			double m21, double m22
			)
		{
			_m11 = m11; _m12 = m12;
			_m21 = m21; _m22 = m22;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix2D"/> structure with the specified values.
		/// </summary>
		/// <param name="elements">An array containing the matrix values in row-major order.</param>
		public Matrix2D(double[] elements)
		{
			Debug.Assert(elements != null);
			Debug.Assert(elements.Length >= 4);

			_m11 = elements[0]; _m12 = elements[1];
			_m21 = elements[2]; _m22 = elements[3];
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix2D"/> structure with the specified values.
		/// </summary>
		/// <param name="elements">An array containing the matrix values in row-major order.</param>
		public Matrix2D(DoubleArrayList elements)
		{
			Debug.Assert(elements != null);
			Debug.Assert(elements.Count >= 4);

			_m11 = elements[0]; _m12 = elements[1];
			_m21 = elements[2]; _m22 = elements[3];
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix2D"/> structure with the specified values.
		/// </summary>
		/// <param name="column1">A <see cref="Vector2D"/> instance holding values for the first column.</param>
		/// <param name="column2">A <see cref="Vector2D"/> instance holding values for the second column.</param>
		public Matrix2D(Vector2D column1, Vector2D column2)
		{
			_m11 = column1.X; _m12 = column2.X;
			_m21 = column1.Y; _m22 = column2.Y;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix2D"/> class using a given matrix.
		/// </summary>
		public Matrix2D(Matrix2D m)
		{
			_m11 = m.M11; _m12 = m.M12;
			_m21 = m.M21; _m22 = m.M22;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix2D"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		private Matrix2D(SerializationInfo info, StreamingContext context)
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
		/// 2-dimentional double-precision floating point zero matrix.
		/// </summary>
		public static readonly Matrix2D Zero = new Matrix2D(0,0,0,0);
		/// <summary>
		/// 2-dimentional double-precision floating point identity matrix.
		/// </summary>
		public static readonly Matrix2D Identity = new Matrix2D(1,0,0,1);
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Matrix2D"/> object.
		/// </summary>
		/// <returns>The <see cref="Matrix2D"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Matrix2D(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Matrix2D"/> object.
		/// </summary>
		/// <returns>The <see cref="Matrix2D"/> object this method creates.</returns>
		public Matrix2D Clone()
		{
			return new Matrix2D(this);
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
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2D"/> instance.</param>
		/// <returns>A new <see cref="Matrix2D"/> instance containing the sum.</returns>
		public static Matrix2D Add(Matrix2D a, Matrix2D b)
		{
			return new Matrix2D(
				a.M11 + b.M11, a.M12 + b.M12,
				a.M21 + b.M21, a.M22 + b.M22
				);
		}
		/// <summary>
		/// Adds a matrix and a scalar.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix2D"/> instance containing the sum.</returns>
		public static Matrix2D Add(Matrix2D a, double s)
		{
			return new Matrix2D(
				a.M11 + s, a.M12 + s,
				a.M21 + s, a.M22 + s
				);
		}
		/// <summary>
		/// Adds two matrices and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="result">A <see cref="Matrix2D"/> instance to hold the result.</param>
		public static void Add(Matrix2D a, Matrix2D b, ref Matrix2D result)
		{
			result.M11 = a.M11 + b.M11;
			result.M12 = a.M12 + b.M12;

			result.M21 = a.M21 + b.M21;
			result.M22 = a.M22 + b.M22;
		}
		/// <summary>
		/// Adds a matrix and a scalar and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="result">A <see cref="Matrix2D"/> instance to hold the result.</param>
		public static void Add(Matrix2D a, double s, ref Matrix2D result)
		{
			result.M11 = a.M11 + s;
			result.M12 = a.M12 + s;

			result.M21 = a.M21 + s;
			result.M22 = a.M22 + s;
		}
		/// <summary>
		/// Subtracts a matrix from a matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance to subtract from.</param>
		/// <param name="b">A <see cref="Matrix2D"/> instance to subtract.</param>
		/// <returns>A new <see cref="Matrix2D"/> instance containing the difference.</returns>
		/// <remarks>result[x][y] = a[x][y] - b[x][y]</remarks>
		public static Matrix2D Subtract(Matrix2D a, Matrix2D b)
		{
			return new Matrix2D(
				a.M11 - b.M11, a.M12 - b.M12,
				a.M21 - b.M21, a.M22 - b.M22
				);
		}
		/// <summary>
		/// Subtracts a scalar from a matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix2D"/> instance containing the difference.</returns>
		public static Matrix2D Subtract(Matrix2D a, double s)
		{
			return new Matrix2D(
				a.M11 - s, a.M12 - s,
				a.M21 - s, a.M22 - s
				);
		}
		/// <summary>
		/// Subtracts a matrix from a matrix and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance to subtract from.</param>
		/// <param name="b">A <see cref="Matrix2D"/> instance to subtract.</param>
		/// <param name="result">A <see cref="Matrix2D"/> instance to hold the result.</param>
		/// <remarks>result[x][y] = a[x][y] - b[x][y]</remarks>
		public static void Subtract(Matrix2D a, Matrix2D b, ref Matrix2D result)
		{
			result.M11 = a.M11 - b.M11;
			result.M12 = a.M12 - b.M12;

			result.M21 = a.M21 - b.M21;
			result.M22 = a.M22 - b.M22;
		}
		/// <summary>
		/// Subtracts a scalar from a matrix and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="result">A <see cref="Matrix2D"/> instance to hold the result.</param>
		public static void Subtract(Matrix2D a, double s, ref Matrix2D result)
		{
			result.M11 = a.M11 - s;
			result.M12 = a.M12 - s;

			result.M21 = a.M21 - s;
			result.M22 = a.M22 - s;
		}
		/// <summary>
		/// Multiplies two matrices.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2D"/> instance.</param>
		/// <returns>A new <see cref="Matrix2D"/> instance containing the result.</returns>
		public static Matrix2D Multiply(Matrix2D a, Matrix2D b)
		{
			return new Matrix2D(
				a.M11 * b.M11 + a.M12 * b.M21,
				a.M11 * b.M12 + a.M12 * b.M22,
				a.M11 * b.M12 + a.M12 * b.M22,
				a.M21 * b.M12 + a.M22 * b.M22
				);
		}
		/// <summary>
		/// Multiplies two matrices and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="result">A <see cref="Matrix2D"/> instance to hold the result.</param>
		public static void Multiply(Matrix2D a, Matrix2D b, ref Matrix2D result)
		{
			result.M11 = a.M11 * b.M11 + a.M12 * b.M21;
			result.M12 = a.M11 * b.M12 + a.M12 * b.M22;
			result.M21 = a.M11 * b.M12 + a.M12 * b.M22;
			result.M22 = a.M21 * b.M12 + a.M22 * b.M22;
		}		
		/// <summary>
		/// Transforms a given vector by a matrix.
		/// </summary>
		/// <param name="matrix">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="vector">A <see cref="Vector2D"/> instance.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the result.</returns>
		public static Vector2D Transform(Matrix2D matrix, Vector2D vector)
		{
			return new Vector2D(
				(matrix.M11 * vector.X) + (matrix.M12 * vector.Y),
				(matrix.M21 * vector.X) + (matrix.M22 * vector.Y));
		}
		/// <summary>
		/// Transforms a given vector by a matrix and put the result in a vector.
		/// </summary>
		/// <param name="matrix">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="vector">A <see cref="Vector2D"/> instance.</param>
		/// <param name="result">A <see cref="Vector2D"/> instance to hold the result.</param>
		public static void Transform(Matrix2D matrix, Vector2D vector, ref Vector2D result)
		{
			result.X = (matrix.M11 * vector.X) + (matrix.M12 * vector.Y);
			result.Y = (matrix.M21 * vector.X) + (matrix.M22 * vector.Y);
		}
		/// <summary>
		/// Transposes a matrix.
		/// </summary>
		/// <param name="m">A <see cref="Matrix2D"/> instance.</param>
		/// <returns>A new <see cref="Matrix2D"/> instance containing the transposed matrix.</returns>
		public static Matrix2D Transpose(Matrix2D m)
		{
			Matrix2D t = new Matrix2D(m);
			t.Transpose();
			return t;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the value of the [1,1] matrix element.
		/// </summary>
		public double M11
		{
			get { return _m11; }
			set { _m11 = value;}
		}
		/// <summary>
		/// Gets or sets the value of the [1,2] matrix element.
		/// </summary>
		public double M12
		{
			get { return _m12; }
			set { _m12 = value;}
		}
		/// <summary>
		/// Gets or sets the value of the [2,1] matrix element.
		/// </summary>
		public double M21
		{
			get { return _m21; }
			set { _m21 = value;}
		}
		/// <summary>
		/// Gets or sets the value of the [2,2] matrix element.
		/// </summary>
		public double M22
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
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Matrix2D"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Matrix2D)
			{
				Matrix2D m = (Matrix2D)obj;
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
		public double Determinant()
		{
			return (_m11*_m22) - (_m12*_m21);
		}
		/// <summary>
		/// Calculates the trace the matrix which is the sum of its diagonal elements.
		/// </summary>
		/// <returns>Returns the trace value of the matrix.</returns>
		public double Trace()
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
		public static bool operator==(Matrix2D a, Matrix2D b)
		{
			return ValueType.Equals(a,b);
		}
		/// <summary>
		/// Tests whether two specified matrices are not equal.
		/// </summary>
		/// <param name="a">The left-hand matrix.</param>
		/// <param name="b">The right-hand matrix.</param>
		/// <returns><see langword="true"/> if the two matrices are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(Matrix2D a, Matrix2D b)
		{
			return !ValueType.Equals(a,b);
		}
		#endregion

		#region Binary Operators
		/// <summary>
		/// Adds two matrices.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2D"/> instance.</param>
		/// <returns>A new <see cref="Matrix2D"/> instance containing the sum.</returns>
		public static Matrix2D operator+(Matrix2D a, Matrix2D b)
		{
			return Matrix2D.Add(a,b);
		}
		/// <summary>
		/// Adds a matrix and a scalar.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix2D"/> instance containing the sum.</returns>
		public static Matrix2D operator+(Matrix2D a, double s)
		{
			return Matrix2D.Add(a,s);
		}
		/// <summary>
		/// Adds a matrix and a scalar.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix2D"/> instance containing the sum.</returns>
		public static Matrix2D operator+(double s, Matrix2D a)
		{
			return Matrix2D.Add(a,s);
		}
		/// <summary>
		/// Subtracts a matrix from a matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2D"/> instance.</param>
		/// <returns>A new <see cref="Matrix2D"/> instance containing the difference.</returns>
		public static Matrix2D operator-(Matrix2D a, Matrix2D b)
		{
			return Matrix2D.Subtract(a,b);
		}
		/// <summary>
		/// Subtracts a scalar from a matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix2D"/> instance containing the difference.</returns>
		public static Matrix2D operator-(Matrix2D a, double s)
		{
			return Matrix2D.Subtract(a,s);
		}
		/// <summary>
		/// Multiplies two matrices.
		/// </summary>
		/// <param name="a">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix2D"/> instance.</param>
		/// <returns>A new <see cref="Matrix2D"/> instance containing the result.</returns>
		public static Matrix2D operator*(Matrix2D a, Matrix2D b)
		{
			return Matrix2D.Multiply(a,b);
		}
		/// <summary>
		/// Transforms a given vector by a matrix.
		/// </summary>
		/// <param name="matrix">A <see cref="Matrix2D"/> instance.</param>
		/// <param name="vector">A <see cref="Vector2D"/> instance.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the result.</returns>
		public static Vector2D operator*(Matrix2D matrix, Vector2D vector)
		{
			return Matrix2D.Transform(matrix, vector);
		}
		#endregion

		#region Indexing Operators
		/// <summary>
		/// Indexer allowing to access the matrix elements by an index
		/// where index = 2*row + column.
		/// </summary>
		public unsafe double this [int index] 
		{			
			get 
			{
				if (index < 0 || index >= 4)
					throw new IndexOutOfRangeException("Invalid matrix index!");

				fixed(double *f = &_m11) 
				{
					return *(f+index);
				}
			}
			set 
			{			
				if (index < 0 || index >= 4)
					throw new IndexOutOfRangeException("Invalid matrix index!");

				fixed(double *f = &_m11) 
				{
					*(f+index) = value;
				}
			}			
		}
		/// <summary>
		/// Indexer allowing to access the matrix elements by row and column.
		/// </summary>
		public double this[int row, int column]
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
