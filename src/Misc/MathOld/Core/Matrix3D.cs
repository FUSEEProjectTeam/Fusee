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
	/// Represents a 3-dimentional double-precision floating point matrix.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct Matrix3D : ISerializable, ICloneable
	{
		#region Private Fields
		private double _m11, _m12, _m13;
		private double _m21, _m22, _m23;
		private double _m31, _m32, _m33;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix3D"/> structure with the specified values.
		/// </summary>
		public Matrix3D(
			double m11, double m12, double m13,
			double m21, double m22, double m23,
			double m31, double m32, double m33
			)
		{
			_m11 = m11; _m12 = m12; _m13 = m13;
			_m21 = m21; _m22 = m22; _m23 = m23;
			_m31 = m31; _m32 = m32; _m33 = m33;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix3D"/> structure with the specified values.
		/// </summary>
		/// <param name="elements">An array containing the matrix values in row-major order.</param>
		public Matrix3D(double[] elements)
		{
			Debug.Assert(elements != null);
			Debug.Assert(elements.Length >= 9);

			_m11 = elements[0]; _m12 = elements[1]; _m13 = elements[2];
			_m21 = elements[3]; _m22 = elements[4]; _m23 = elements[5];
			_m31 = elements[6]; _m32 = elements[7]; _m33 = elements[8];
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix3F"/> structure with the specified values.
		/// </summary>
		/// <param name="elements">An array containing the matrix values in row-major order.</param>
		public Matrix3D(DoubleArrayList elements)
		{
			Debug.Assert(elements != null);
			Debug.Assert(elements.Count >= 9);

			_m11 = elements[0]; _m12 = elements[1]; _m13 = elements[2];
			_m21 = elements[3]; _m22 = elements[4]; _m23 = elements[5];
			_m31 = elements[6]; _m32 = elements[7]; _m33 = elements[8];
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix3D"/> structure with the specified values.
		/// </summary>
		/// <param name="column1">A <see cref="Vector2D"/> instance holding values for the first column.</param>
		/// <param name="column2">A <see cref="Vector2D"/> instance holding values for the second column.</param>
		/// <param name="column3">A <see cref="Vector2D"/> instance holding values for the third column.</param>
		public Matrix3D(Vector3D column1, Vector3D column2, Vector3D column3)
		{
			_m11 = column1.X; _m12 = column2.X; _m13 = column3.X;
			_m21 = column1.Y; _m22 = column2.Y; _m23 = column3.Y;
			_m31 = column1.Z; _m32 = column2.Z; _m33 = column3.Z;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix3D"/> class using a given matrix.
		/// </summary>
		public Matrix3D(Matrix3D m)
		{
			_m11 = m.M11; _m12 = m.M12; _m13 = m.M13;
			_m21 = m.M21; _m22 = m.M22; _m23 = m.M23;
			_m31 = m.M31; _m32 = m.M32; _m33 = m.M33;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix3D"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		private Matrix3D(SerializationInfo info, StreamingContext context)
		{
			// Get the first row
			_m11 = info.GetSingle("M11");
			_m12 = info.GetSingle("M12");
			_m13 = info.GetSingle("M13");

			// Get the second row
			_m21 = info.GetSingle("M21");
			_m22 = info.GetSingle("M22");
			_m23 = info.GetSingle("M23");

			// Get the third row
			_m31 = info.GetSingle("M31");
			_m32 = info.GetSingle("M32");
			_m33 = info.GetSingle("M33");
		}
		#endregion

		#region Constants
		/// <summary>
		/// 3-dimentional double-precision floating point zero matrix.
		/// </summary>
		public static readonly Matrix3D Zero = new Matrix3D(0,0,0,0,0,0,0,0,0);
		/// <summary>
		/// 3-dimentional double-precision floating point identity matrix.
		/// </summary>
		public static readonly Matrix3D Identity = new Matrix3D(
			1,0,0,
			0,1,0,
			0,0,1
			);
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Matrix3D"/> object.
		/// </summary>
		/// <returns>The <see cref="Matrix3D"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Matrix3D(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Matrix3D"/> object.
		/// </summary>
		/// <returns>The <see cref="Matrix3D"/> object this method creates.</returns>
		public Matrix3D Clone()
		{
			return new Matrix3D(this);
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
			info.AddValue("M13", _m13);

			// Second row
			info.AddValue("M21", _m21);
			info.AddValue("M22", _m22);
			info.AddValue("M23", _m23);

			// Third row
			info.AddValue("M31", _m31);
			info.AddValue("M32", _m32);
			info.AddValue("M33", _m33);
		}
		#endregion

		#region Public Static Matrix Arithmetics
		/// <summary>
		/// Adds two matrices.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix3D"/> instance.</param>
		/// <returns>A new <see cref="Matrix3D"/> instance containing the sum.</returns>
		public static Matrix3D Add(Matrix3D a, Matrix3D b)
		{
			return new Matrix3D(
				a.M11 + b.M11, a.M12 + b.M12, a.M13 + b.M13,
				a.M21 + b.M21, a.M22 + b.M22, a.M23 + b.M23,
				a.M31 + b.M31, a.M32 + b.M32, a.M33 + b.M33
				);
		}
		/// <summary>
		/// Adds a matrix and a scalar.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix3D"/> instance containing the sum.</returns>
		public static Matrix3D Add(Matrix3D a, double s)
		{
			return new Matrix3D(
				a.M11 + s, a.M12 + s, a.M13 + s, 
				a.M21 + s, a.M22 + s, a.M23 + s,
				a.M31 + s, a.M32 + s, a.M33 + s
				);
		}
		/// <summary>
		/// Adds two matrices and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="result">A <see cref="Matrix3D"/> instance to hold the result.</param>
		public static void Add(Matrix3D a, Matrix3D b, ref Matrix3D result)
		{
			result.M11 = a.M11 + b.M11;
			result.M12 = a.M12 + b.M12;
			result.M13 = a.M13 + b.M13;

			result.M21 = a.M21 + b.M21;
			result.M22 = a.M22 + b.M22;
			result.M23 = a.M23 + b.M23;

			result.M31 = a.M31 + b.M31;
			result.M32 = a.M32 + b.M32;
			result.M33 = a.M33 + b.M33;
		}
		/// <summary>
		/// Adds a matrix and a scalar and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="result">A <see cref="Matrix3D"/> instance to hold the result.</param>
		public static void Add(Matrix3D a, double s, ref Matrix3D result)
		{
			result.M11 = a.M11 + s;
			result.M12 = a.M12 + s;
			result.M13 = a.M13 + s;

			result.M21 = a.M21 + s;
			result.M22 = a.M22 + s;
			result.M23 = a.M23 + s;

			result.M31 = a.M31 + s;
			result.M32 = a.M32 + s;
			result.M33 = a.M33 + s;
		}
		/// <summary>
		/// Subtracts a matrix from a matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance to subtract from.</param>
		/// <param name="b">A <see cref="Matrix3D"/> instance to subtract.</param>
		/// <returns>A new <see cref="Matrix3D"/> instance containing the difference.</returns>
		/// <remarks>result[x][y] = a[x][y] - b[x][y]</remarks>
		public static Matrix3D Subtract(Matrix3D a, Matrix3D b)
		{
			return new Matrix3D(
				a.M11 - b.M11, a.M12 - b.M12, a.M13 - b.M13,
				a.M21 - b.M21, a.M22 - b.M22, a.M23 - b.M23,
				a.M31 - b.M31, a.M32 - b.M32, a.M33 - b.M33
				);
		}
		/// <summary>
		/// Subtracts a scalar from a matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix3D"/> instance containing the difference.</returns>
		public static Matrix3D Subtract(Matrix3D a, double s)
		{
			return new Matrix3D(
				a.M11 - s, a.M12 - s, a.M13 - s,
				a.M21 - s, a.M22 - s, a.M23 - s,
				a.M31 - s, a.M32 - s, a.M33 - s
				);
		}
		/// <summary>
		/// Subtracts a matrix from a matrix and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance to subtract from.</param>
		/// <param name="b">A <see cref="Matrix3D"/> instance to subtract.</param>
		/// <param name="result">A <see cref="Matrix3D"/> instance to hold the result.</param>
		/// <remarks>result[x][y] = a[x][y] - b[x][y]</remarks>
		public static void Subtract(Matrix3D a, Matrix3D b, ref Matrix3D result)
		{
			result.M11 = a.M11 - b.M11;
			result.M12 = a.M12 - b.M12;
			result.M13 = a.M13 - b.M13;

			result.M21 = a.M21 - b.M21;
			result.M22 = a.M22 - b.M22;
			result.M23 = a.M23 - b.M23;

			result.M31 = a.M31 - b.M31;
			result.M32 = a.M32 - b.M32;
			result.M33 = a.M33 - b.M33;
		}
		/// <summary>
		/// Subtracts a scalar from a matrix and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="result">A <see cref="Matrix3D"/> instance to hold the result.</param>
		public static void Subtract(Matrix3D a, double s, ref Matrix3D result)
		{
			result.M11 = a.M11 - s;
			result.M12 = a.M12 - s;
			result.M13 = a.M13 - s;

			result.M21 = a.M21 - s;
			result.M22 = a.M22 - s;
			result.M23 = a.M23 - s;

			result.M31 = a.M31 - s;
			result.M32 = a.M32 - s;
			result.M33 = a.M33 - s;
		}
		/// <summary>
		/// Multiplies two matrices.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix3D"/> instance.</param>
		/// <returns>A new <see cref="Matrix3D"/> instance containing the result.</returns>
		public static Matrix3D Multiply(Matrix3D a, Matrix3D b)
		{
			return new Matrix3D(
				a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31,
				a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32,
				a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33,

				a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31,
				a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32,
				a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33,

				a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31,
				a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32, 
				a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33
				);
		}
		/// <summary>
		/// Multiplies two matrices and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="result">A <see cref="Matrix3D"/> instance to hold the result.</param>
		public static void Multiply(Matrix3D a, Matrix3D b, ref Matrix3D result)
		{
			result.M11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
			result.M12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
			result.M13 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;

			result.M21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
			result.M22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
			result.M23 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;

			result.M31 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
			result.M32 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32; 
			result.M33 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33;
		}		
		/// <summary>
		/// Transforms a given vector by a matrix.
		/// </summary>
		/// <param name="matrix">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="vector">A <see cref="Vector3D"/> instance.</param>
		/// <returns>A new <see cref="Vector3D"/> instance containing the result.</returns>
		public static Vector3D Transform(Matrix3D matrix, Vector3D vector)
		{
			return new Vector3D(
				(matrix.M11 * vector.X) + (matrix.M12 * vector.Y) + (matrix.M13 * vector.Z),
				(matrix.M21 * vector.X) + (matrix.M22 * vector.Y) + (matrix.M23 * vector.Z),
				(matrix.M31 * vector.X) + (matrix.M32 * vector.Y) + (matrix.M33 * vector.Z));
		}
		/// <summary>
		/// Transforms a given vector by a matrix and put the result in a vector.
		/// </summary>
		/// <param name="matrix">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="vector">A <see cref="Vector3D"/> instance.</param>
		/// <param name="result">A <see cref="Vector3D"/> instance to hold the result.</param>
		public static void Transform(Matrix3D matrix, Vector3D vector, ref Vector3D result)
		{
			result.X = (matrix.M11 * vector.X) + (matrix.M12 * vector.Y) + (matrix.M13 * vector.Z);
			result.Y = (matrix.M21 * vector.X) + (matrix.M22 * vector.Y) + (matrix.M23 * vector.Z);
			result.Z = (matrix.M31 * vector.X) + (matrix.M32 * vector.Y) + (matrix.M33 * vector.Z);
		}
		/// <summary>
		/// Transposes a matrix.
		/// </summary>
		/// <param name="m">A <see cref="Matrix3D"/> instance.</param>
		/// <returns>A new <see cref="Matrix3D"/> instance containing the transposed matrix.</returns>
		public static Matrix3D Transpose(Matrix3D m)
		{
			Matrix3D t = new Matrix3D(m);
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
		/// Gets or sets the value of the [1,3] matrix element.
		/// </summary>
		public double M13
		{
			get { return _m13; }
			set { _m13 = value;}
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
		/// <summary>
		/// Gets or sets the value of the [2,3] matrix element.
		/// </summary>
		public double M23
		{
			get { return _m23; }
			set { _m23 = value;}
		}


		/// <summary>
		/// Gets or sets the value of the [3,1] matrix element.
		/// </summary>
		public double M31
		{
			get { return _m31; }
			set { _m31 = value;}
		}
		/// <summary>
		/// Gets or sets the value of the [3,2] matrix element.
		/// </summary>
		public double M32
		{
			get { return _m32; }
			set { _m32 = value;}
		}
		/// <summary>
		/// Gets or sets the value of the [3,3] matrix element.
		/// </summary>
		public double M33
		{
			get { return _m33; }
			set { _m33 = value;}
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
				_m11.GetHashCode() ^ _m12.GetHashCode() ^ _m13.GetHashCode() ^
				_m21.GetHashCode() ^ _m22.GetHashCode() ^ _m23.GetHashCode() ^
				_m31.GetHashCode() ^ _m32.GetHashCode() ^ _m33.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Matrix3D"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Matrix3D)
			{
				Matrix3D m = (Matrix3D)obj;
				return 
					(_m11 == m.M11) && (_m12 == m.M12) && (_m13 == m.M13) &&
					(_m21 == m.M21) && (_m22 == m.M22) && (_m23 == m.M23) &&
					(_m31 == m.M31) && (_m32 == m.M32) && (_m33 == m.M33);
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
			s.Append(String.Format( "|{0}, {1}, {2}|\n", _m11, _m12, _m13));
			s.Append(String.Format( "|{0}, {1}, {2}|\n", _m21, _m22, _m23));
			s.Append(String.Format( "|{0}, {1}, {2}|\n", _m31, _m32, _m33));

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
			// rule of Sarrus
			return 
				_m11 * _m22 * _m33 + _m12 * _m23 * _m31 + _m13 * _m21 * _m32 -
				_m13 * _m22 * _m31 - _m11 * _m23 * _m32 - _m12 * _m21 * _m33;
		}
		/*
		/// <summary>
		/// Calculates the adjoint of the matrix.
		/// </summary>
		/// <returns>A <see cref="Matrix3D"/> instance containing the adjoint of the matrix.</returns>
		public Matrix3D Adjoint() 
		{
			Matrix3D result = new Matrix3D();
			for (int row = 0; row < 4; row++)
			{
				for (int col = 0; col < 4; col++) 
				{
					if (((col+row) % 2) == 0)
						result[row, col] = Minor(col, row).Determinant();
					else
						result[row, col] = -Minor(col, row).Determinant();
				}
			}

			return result;
		}
		/// <summary>
		/// Build a 3x3 matrix from from the current matrix without the given row and column.
		/// </summary>
		/// <param name="row">The row to remove.</param>
		/// <param name="column">The column to remove.</param>
		/// <returns>A <see cref="Matrix3D"/> instance containing the result Minor.</returns>
		public Matrix3D Minor(int row, int column) 
		{
			int r = 0;
			Matrix3D result = new Matrix3D();
			for (int row = 0; row < 4; row++) 
			{
				int c = 0;
				if (row != row) 
				{
					for (int column = 0; column < 4; column++) 
					{
						if (column != column) 
						{
							result[r,c] = this[row, column];
							c++;
						}
					}
					r++;
				}
			}
			return result;
		}
		*/

		/// <summary>
		/// Calculates the trace the matrix which is the sum of its diagonal elements.
		/// </summary>
		/// <returns>Returns the trace value of the matrix.</returns>
		public double Trace()
		{
			return _m11 + _m22 + _m33;
		}
		/// <summary>
		/// Transposes this matrix.
		/// </summary>
		public void Transpose()
		{
			MathFunctions.Swap(ref _m12, ref _m21);
			MathFunctions.Swap(ref _m13, ref _m31);
			MathFunctions.Swap(ref _m23, ref _m32);
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified matrices are equal.
		/// </summary>
		/// <param name="a">The left-hand matrix.</param>
		/// <param name="b">The right-hand matrix.</param>
		/// <returns><see langword="true"/> if the two matrices are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(Matrix3D a, Matrix3D b)
		{
			return ValueType.Equals(a,b);
		}
		/// <summary>
		/// Tests whether two specified matrices are not equal.
		/// </summary>
		/// <param name="a">The left-hand matrix.</param>
		/// <param name="b">The right-hand matrix.</param>
		/// <returns><see langword="true"/> if the two matrices are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(Matrix3D a, Matrix3D b)
		{
			return !ValueType.Equals(a,b);
		}
		#endregion

		#region Binary Operators
		/// <summary>
		/// Adds two matrices.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix3D"/> instance.</param>
		/// <returns>A new <see cref="Matrix3D"/> instance containing the sum.</returns>
		public static Matrix3D operator+(Matrix3D a, Matrix3D b)
		{
			return Matrix3D.Add(a,b);
		}
		/// <summary>
		/// Adds a matrix and a scalar.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix3D"/> instance containing the sum.</returns>
		public static Matrix3D operator+(Matrix3D a, double s)
		{
			return Matrix3D.Add(a,s);
		}
		/// <summary>
		/// Adds a matrix and a scalar.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix3D"/> instance containing the sum.</returns>
		public static Matrix3D operator+(double s, Matrix3D a)
		{
			return Matrix3D.Add(a,s);
		}
		/// <summary>
		/// Subtracts a matrix from a matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix3D"/> instance.</param>
		/// <returns>A new <see cref="Matrix3D"/> instance containing the difference.</returns>
		public static Matrix3D operator-(Matrix3D a, Matrix3D b)
		{
			return Matrix3D.Subtract(a,b);
		}
		/// <summary>
		/// Subtracts a scalar from a matrix.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Matrix3D"/> instance containing the difference.</returns>
		public static Matrix3D operator-(Matrix3D a, double s)
		{
			return Matrix3D.Subtract(a,s);
		}
		/// <summary>
		/// Multiplies two matrices.
		/// </summary>
		/// <param name="a">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="b">A <see cref="Matrix3D"/> instance.</param>
		/// <returns>A new <see cref="Matrix3D"/> instance containing the result.</returns>
		public static Matrix3D operator*(Matrix3D a, Matrix3D b)
		{
			return Matrix3D.Multiply(a,b);
		}
		/// <summary>
		/// Transforms a given vector by a matrix.
		/// </summary>
		/// <param name="matrix">A <see cref="Matrix3D"/> instance.</param>
		/// <param name="vector">A <see cref="Vector3D"/> instance.</param>
		/// <returns>A new <see cref="Vector3D"/> instance containing the result.</returns>
		public static Vector3D operator*(Matrix3D matrix, Vector3D vector)
		{
			return Matrix3D.Transform(matrix, vector);
		}
		#endregion

		#region Indexing Operators
		/// <summary>
		/// Indexer allowing to access the matrix elements by an index
		/// where index = 3*row + column.
		/// </summary>
		public unsafe double this [int index] 
		{			
			get 
			{
				if (index < 0 || index >= 16)
					throw new IndexOutOfRangeException("Invalid matrix index!");

				fixed(double *f = &_m11) 
				{
					return *(f+index);
				}
			}
			set 
			{			
				if (index < 0 || index >= 16)
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
				return this[ row *3 + column ];
			}
			set 
			{				
				this[ row *3 + column ] = value;
			}			
		}
		#endregion
	}
}
