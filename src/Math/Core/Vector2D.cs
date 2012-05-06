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
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
	/// <summary>
	/// Represents 2-Dimentional vector of double-precision floating point numbers.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	[TypeConverter(typeof(Vector2DConverter))]
	public struct Vector2D : ISerializable, ICloneable
	{
		#region Private fields
		private double _x;
		private double _y;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Vector2D"/> class with the specified coordinates.
		/// </summary>
		/// <param name="x">The vector's X coordinate.</param>
		/// <param name="y">The vector's Y coordinate.</param>
		public Vector2D(double x, double y)
		{
			_x = x;
			_y = y;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Vector2D"/> class with the specified coordinates.
		/// </summary>
		/// <param name="coordinates">An array containing the coordinate parameters.</param>
		public Vector2D(double[] coordinates)
		{
			Debug.Assert(coordinates != null);
			Debug.Assert(coordinates.Length >= 2);

			_x = coordinates[0];
			_y = coordinates[1];
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Vector2D"/> class with the specified coordinates.
		/// </summary>
		/// <param name="coordinates">An array containing the coordinate parameters.</param>
		public Vector2D(DoubleArrayList coordinates)
		{
			Debug.Assert(coordinates != null);
			Debug.Assert(coordinates.Count >= 2);

			_x = coordinates[0];
			_y = coordinates[1];
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Vector2D"/> class using coordinates from a given <see cref="Vector2D"/> instance.
		/// </summary>
		/// <param name="vector">A <see cref="Vector2D"/> to get the coordinates from.</param>
		public Vector2D(Vector2D vector)
		{
			_x = vector.X;
			_y = vector.Y;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Vector2D"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		private Vector2D(SerializationInfo info, StreamingContext context)
		{
			_x = info.GetSingle("X");
			_y = info.GetSingle("Y");
		}
		#endregion

		#region Constants
		/// <summary>
		/// 2-Dimentional double-precision floating point zero vector.
		/// </summary>
		public static readonly Vector2D Zero	= new Vector2D(0.0f, 0.0f);
		/// <summary>
		/// 2-Dimentional double-precision floating point X-Axis vector.
		/// </summary>
		public static readonly Vector2D XAxis	= new Vector2D(1.0f, 0.0f);
		/// <summary>
		/// 2-Dimentional double-precision floating point Y-Axis vector.
		/// </summary>
		public static readonly Vector2D YAxis	= new Vector2D(0.0f, 1.0f);
		#endregion

		#region Public properties
		/// <summery>
		/// Gets or sets the x-coordinate of this vector.
		/// </summery>
		/// <value>The x-coordinate of this vector.</value>
		public double X
		{
			get { return _x; }
			set { _x = value;}
		}
		/// <summery>
		/// Gets or sets the y-coordinate of this vector.
		/// </summery>
		/// <value>The y-coordinate of this vector.</value>
		public double Y
		{
			get { return _y; }
			set { _y = value;}
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Vector2D"/> object.
		/// </summary>
		/// <returns>The <see cref="Vector2D"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Vector2D(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Vector2D"/> object.
		/// </summary>
		/// <returns>The <see cref="Vector2D"/> object this method creates.</returns>
		public Vector2D Clone()
		{
			return new Vector2D(this);
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
			info.AddValue("X", _x);
			info.AddValue("Y", _y);
		}
		#endregion

		#region Public Static Parse Methods
		/// <summary>
		/// Converts the specified string to its <see cref="Vector2D"/> equivalent.
		/// </summary>
		/// <param name="s">A string representation of a <see cref="Vector2D"/></param>
		/// <returns>A <see cref="Vector2D"/> that represents the vector specified by the <paramref name="s"/> parameter.</returns>
		public static Vector2D Parse(string s)
		{
			Regex r = new Regex(@"\((?<x>.*),(?<y>.*)\)", RegexOptions.None);
			Match m = r.Match(s);
			if (m.Success)
			{
				return new Vector2D(
					double.Parse(m.Result("${x}")),
					double.Parse(m.Result("${y}"))
					);
			}
			else
			{
				throw new ParseException("Unsuccessful Match.");
			}
		}
		#endregion

		#region Public Static Vector Arithmetics
		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="w">A <see cref="Vector2D"/> instance.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the sum.</returns>
		public static Vector2D Add(Vector2D v, Vector2D w)
		{
			return new Vector2D(v.X + w.X, v.Y + w.Y);
		}
		/// <summary>
		/// Adds a vector and a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the sum.</returns>
		public static Vector2D Add(Vector2D v, double s)
		{
			return new Vector2D(v.X + s, v.Y + s);
		}
		/// <summary>
		/// Adds two vectors and put the result in the third vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="v">A <see cref="Vector2D"/> instance</param>
		/// <param name="w">A <see cref="Vector2D"/> instance to hold the result.</param>
		public static void Add(Vector2D u, Vector2D v, ref Vector2D w)
		{
			w.X = u.X + v.X;
			w.Y = u.Y + v.Y;
		}
		/// <summary>
		/// Adds a vector and a scalar and put the result into another vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="v">A <see cref="Vector2D"/> instance to hold the result.</param>
		public static void Add(Vector2D u, double s, ref Vector2D v)
		{
			v.X = u.X + s;
			v.Y = u.Y + s;
		}
		/// <summary>
		/// Subtracts a vector from a vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="w">A <see cref="Vector2D"/> instance.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the difference.</returns>
		/// <remarks>
		///	result[i] = v[i] - w[i].
		/// </remarks>
		public static Vector2D Subtract(Vector2D v, Vector2D w)
		{
			return new Vector2D(v.X - w.X, v.Y - w.Y);
		}
		/// <summary>
		/// Subtracts a scalar from a vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the difference.</returns>
		/// <remarks>
		/// result[i] = v[i] - s
		/// </remarks>
		public static Vector2D Subtract(Vector2D v, double s)
		{
			return new Vector2D(v.X - s, v.Y - s);
		}
		/// <summary>
		/// Subtracts a vector from a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the difference.</returns>
		/// <remarks>
		/// result[i] = s - v[i]
		/// </remarks>
		public static Vector2D Subtract(double s, Vector2D v)
		{
			return new Vector2D(s - v.X, s - v.Y);
		}
		/// <summary>
		/// Subtracts a vector from a second vector and puts the result into a third vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="v">A <see cref="Vector2D"/> instance</param>
		/// <param name="w">A <see cref="Vector2D"/> instance to hold the result.</param>
		/// <remarks>
		///	w[i] = v[i] - w[i].
		/// </remarks>
		public static void Subtract(Vector2D u, Vector2D v, ref Vector2D w)
		{
			w.X = u.X - v.X;
			w.Y = u.Y - v.Y;
		}
		/// <summary>
		/// Subtracts a vector from a scalar and put the result into another vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="v">A <see cref="Vector2D"/> instance to hold the result.</param>
		/// <remarks>
		/// v[i] = u[i] - s
		/// </remarks>
		public static void Subtract(Vector2D u, double s, ref Vector2D v)
		{
			v.X = u.X - s;
			v.Y = u.Y - s;
		}
		/// <summary>
		/// Subtracts a scalar from a vector and put the result into another vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="v">A <see cref="Vector2D"/> instance to hold the result.</param>
		/// <remarks>
		/// v[i] = s - u[i]
		/// </remarks>
		public static void Subtract(double s, Vector2D u, ref Vector2D v)
		{
			v.X = s - u.X;
			v.Y = s - u.Y;
		}
		/// <summary>
		/// Divides a vector by another vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <returns>A new <see cref="Vector2D"/> containing the quotient.</returns>
		/// <remarks>
		///	result[i] = u[i] / v[i].
		/// </remarks>
		public static Vector2D Divide(Vector2D u, Vector2D v)
		{
			return new Vector2D(u.X / v.X, u.Y / v.Y);
		}
		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar</param>
		/// <returns>A new <see cref="Vector2D"/> containing the quotient.</returns>
		/// <remarks>
		/// result[i] = v[i] / s;
		/// </remarks>
		public static Vector2D Divide(Vector2D v, double s)
		{
			return new Vector2D(v.X / s, v.Y / s);
		}
		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar</param>
		/// <returns>A new <see cref="Vector2D"/> containing the quotient.</returns>
		/// <remarks>
		/// result[i] = s / v[i]
		/// </remarks>
		public static Vector2D Divide(double s, Vector2D v)
		{
			return new Vector2D(s / v.X, s/ v.Y);
		}
		/// <summary>
		/// Divides a vector by another vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="w">A <see cref="Vector2D"/> instance to hold the result.</param>
		/// <remarks>
		/// w[i] = u[i] / v[i]
		/// </remarks>
		public static void Divide(Vector2D u, Vector2D v, ref Vector2D w)
		{
			w.X = u.X / v.X;
			w.Y = u.Y / v.Y;
		}
		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar</param>
		/// <param name="v">A <see cref="Vector2D"/> instance to hold the result.</param>
		/// <remarks>
		/// v[i] = u[i] / s
		/// </remarks>
		public static void Divide(Vector2D u, double s, ref Vector2D v)
		{
			v.X = u.X / s;
			v.Y = u.Y / s;
		}
		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar</param>
		/// <param name="v">A <see cref="Vector2D"/> instance to hold the result.</param>
		/// <remarks>
		/// v[i] = s / u[i]
		/// </remarks>
		public static void Divide(double s, Vector2D u, ref Vector2D v)
		{
			v.X = s / u.X;
			v.Y = s / u.Y;
		}
		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector2D"/> containing the result.</returns>
		public static Vector2D Multiply(Vector2D u, double s)
		{
			return new Vector2D(u.X * s, u.Y * s);
		}
		/// <summary>
		/// Multiplies a vector by a scalar and put the result in another vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="v">A <see cref="Vector2D"/> instance to hold the result.</param>
		public static void Multiply(Vector2D u, double s, ref Vector2D v)
		{
			v.X = u.X * s;
			v.Y = u.Y * s;
		}
		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <returns>The dot product value.</returns>
		public static double Dot(Vector2D u, Vector2D v)
		{
			return (u.X * v.X) + (u.Y * v.Y);
		}
		/// <summary>
		/// Calculates the Kross product of two vectors.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <returns>The Kross product value.</returns>
		/// <remarks>
		/// <p>
		/// The Kross product is defined as:
		/// Kross(u,v) = u.X*v.Y - u.Y*v.X.
		/// </p>
		/// <p>
		/// The operation is related to the cross product in 3D given by (x0, y0, 0) X (x1, y1, 0) = (0, 0, Kross((x0, y0), (x1, y1))).
		/// The operation has the property that Kross(u, v) = -Kross(v, u).
		/// </p>
		/// </remarks>
		public static double KrossProduct(Vector2D u, Vector2D v)
		{
			return u.X*v.Y - u.Y*v.X;
		}
		/// <summary>
		/// Negates a vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the negated values.</returns>
		public static Vector2D Negate(Vector2D v)
		{
			return new Vector2D(-v.X, -v.Y);
		}
		/// <summary>
		/// Tests whether two vectors are approximately equal using default tolerance value.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <returns><see langword="true"/> if the two vectors are approximately equal; otherwise, <see langword="false"/>.</returns>
		public static bool ApproxEqual(Vector2D v, Vector2D u)
		{
			return ApproxEqual(v,u, MathFunctions.EpsilonD);
		}
		/// <summary>
		/// Tests whether two vectors are approximately equal given a tolerance value.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="tolerance">The tolerance value used to test approximate equality.</param>
		/// <returns><see langword="true"/> if the two vectors are approximately equal; otherwise, <see langword="false"/>.</returns>
		public static bool ApproxEqual(Vector2D v, Vector2D u, double tolerance)
		{
			return
				(
				(System.Math.Abs(v.X - u.X) <= tolerance) &&
				(System.Math.Abs(v.Y - u.Y) <= tolerance)
				);
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Scale the vector so that its length is 1.
		/// </summary>
		public void Normalize()
		{
			double length = GetLength();
			if (length == 0)
			{
				throw new DivideByZeroException("Trying to normalize a vector with length of zero.");
			}

			_x /= length;
			_y /= length;

		}
		/// <summary>
		/// Returns the length of the vector.
		/// </summary>
		/// <returns>The length of the vector. (Sqrt(X*X + Y*Y))</returns>
		public double GetLength()
		{
			return System.Math.Sqrt(_x*_x + _y*_y);
		}
		/// <summary>
		/// Returns the squared length of the vector.
		/// </summary>
		/// <returns>The squared length of the vector. (X*X + Y*Y)</returns>
		public double GetLengthSquared()
		{
			return (_x*_x + _y*_y);
		}
		/// <summary>
		/// Returns a perpendicular vector.
		/// </summary>
		/// <returns>A <see cref="Vector2D"/> instance.</returns>
		/// <remarks>
		/// The return value is the vector rotated by 90 degrees clockwise.
		/// If the V=(x,y) then V.Perp()=(y,-x).
		/// </remarks>
		public Vector2D Perp()
		{
			return new Vector2D(_y, -_x);
		}
		/// <summary>
		/// Returns a normalized perpendicular vector.
		/// </summary>
		/// <returns>A <see cref="Vector2D"/> instance.</returns>
		/// <remarks>
		/// The return value is the vector rotated by 90 degrees clockwise.
		/// If the V=(x,y) then V.Perp()=(y,-x) normalized.
		/// </remarks>
		public Vector2D UnitPerp()
		{
			Vector2D perp = new Vector2D(_y, -_x);
			perp.Normalize();
			return perp;
		}
		/// <summary>
		/// Clamps vector values to zero using a given tolerance value.
		/// </summary>
		/// <param name="tolerance">The tolerance to use.</param>
		/// <remarks>
		/// The vector values that are close to zero within the given tolerance are set to zero.
		/// </remarks>
		public void ClampZero(double tolerance)
		{
			_x = MathFunctions.Clamp(_x, 0, tolerance);
			_y = MathFunctions.Clamp(_y, 0, tolerance);
		}
		/// <summary>
		/// Clamps vector values to zero using the default tolerance value.
		/// </summary>
		/// <remarks>
		/// The vector values that are close to zero within the given tolerance are set to zero.
		/// The tolerance value used is <see cref="MathFunctions.EpsilonD"/>
		/// </remarks>
		public void ClampZero()
		{
			_x = MathFunctions.Clamp(_x, 0);
			_y = MathFunctions.Clamp(_y, 0);
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return _x.GetHashCode() ^ _y.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Vector2D"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Vector2D)
			{
				Vector2D v = (Vector2D)obj;
				return (_x == v.X) && (_y == v.Y);
			}
			return false;
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			return string.Format("({0}, {1})", _x, _y);
		}
		#endregion
		
		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified vectors are equal.
		/// </summary>
		/// <param name="u">The left-hand vector.</param>
		/// <param name="v">The right-hand vector.</param>
		/// <returns><see langword="true"/> if the two vectors are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(Vector2D u, Vector2D v)
		{
			return ValueType.Equals(u,v);
		}
		/// <summary>
		/// Tests whether two specified vectors are not equal.
		/// </summary>
		/// <param name="u">The left-hand vector.</param>
		/// <param name="v">The right-hand vector.</param>
		/// <returns><see langword="true"/> if the two vectors are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(Vector2D u, Vector2D v)
		{
			return !ValueType.Equals(u,v);
		}

		/// <summary>
		/// Tests if a vector's components are greater than another vector's components.
		/// </summary>
		/// <param name="u">The left-hand vector.</param>
		/// <param name="v">The right-hand vector.</param>
		/// <returns><see langword="true"/> if the left-hand vector's components are greater than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
		public static bool operator>(Vector2D u, Vector2D v)
		{
			return (
				(u._x > v._x) && 
				(u._y > v._y));
		}
		/// <summary>
		/// Tests if a vector's components are smaller than another vector's components.
		/// </summary>
		/// <param name="u">The left-hand vector.</param>
		/// <param name="v">The right-hand vector.</param>
		/// <returns><see langword="true"/> if the left-hand vector's components are smaller than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
		public static bool operator<(Vector2D u, Vector2D v)
		{
			return (
				(u._x < v._x) && 
				(u._y < v._y));
		}
		/// <summary>
		/// Tests if a vector's components are greater or equal than another vector's components.
		/// </summary>
		/// <param name="u">The left-hand vector.</param>
		/// <param name="v">The right-hand vector.</param>
		/// <returns><see langword="true"/> if the left-hand vector's components are greater or equal than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
		public static bool operator>=(Vector2D u, Vector2D v)
		{
			return (
				(u._x >= v._x) && 
				(u._y >= v._y));
		}
		/// <summary>
		/// Tests if a vector's components are smaller or equal than another vector's components.
		/// </summary>
		/// <param name="u">The left-hand vector.</param>
		/// <param name="v">The right-hand vector.</param>
		/// <returns><see langword="true"/> if the left-hand vector's components are smaller or equal than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
		public static bool operator<=(Vector2D u, Vector2D v)
		{
			return (
				(u._x <= v._x) && 
				(u._y <= v._y));
		}
		#endregion

		#region Unary Operators
		/// <summary>
		/// Negates the values of the vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the negated values.</returns>
		public static Vector2D operator-(Vector2D v)
		{
			return Vector2D.Negate(v);
		}
		#endregion

		#region Binary Operators
		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the sum.</returns>
		public static Vector2D operator+(Vector2D u, Vector2D v)
		{
			return Vector2D.Add(u,v);
		}
		/// <summary>
		/// Adds a vector and a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the sum.</returns>
		public static Vector2D operator+(Vector2D v, double s)
		{
			return Vector2D.Add(v,s);
		}
		/// <summary>
		/// Adds a vector and a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the sum.</returns>
		public static Vector2D operator+(double s, Vector2D v)
		{
			return Vector2D.Add(v,s);
		}
		/// <summary>
		/// Subtracts a vector from a vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector2D"/> instance.</param>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the difference.</returns>
		/// <remarks>
		///	result[i] = v[i] - w[i].
		/// </remarks>
		public static Vector2D operator-(Vector2D u, Vector2D v)
		{
			return Vector2D.Subtract(u,v);
		}
		/// <summary>
		/// Subtracts a scalar from a vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the difference.</returns>
		/// <remarks>
		/// result[i] = v[i] - s
		/// </remarks>
		public static Vector2D operator-(Vector2D v, double s)
		{
			return Vector2D.Subtract(v, s);
		}
		/// <summary>
		/// Subtracts a vector from a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector2D"/> instance containing the difference.</returns>
		/// <remarks>
		/// result[i] = s - v[i]
		/// </remarks>
		public static Vector2D operator-(double s, Vector2D v)
		{
			return Vector2D.Subtract(s, v);
		}

		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector2D"/> containing the result.</returns>
		public static Vector2D operator*(Vector2D v, double s)
		{
			return Vector2D.Multiply(v,s);
		}
		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector2D"/> containing the result.</returns>
		public static Vector2D operator*(double s, Vector2D v)
		{
			return Vector2D.Multiply(v,s);
		}
		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar</param>
		/// <returns>A new <see cref="Vector2D"/> containing the quotient.</returns>
		/// <remarks>
		/// result[i] = v[i] / s;
		/// </remarks>
		public static Vector2D operator/(Vector2D v, double s)
		{
			return Vector2D.Divide(v,s);
		}
		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <param name="s">A scalar</param>
		/// <returns>A new <see cref="Vector2D"/> containing the quotient.</returns>
		/// <remarks>
		/// result[i] = s / v[i]
		/// </remarks>
		public static Vector2D operator/(double s, Vector2D v)
		{
			return Vector2D.Divide(s,v);
		}
		#endregion

		#region Array Indexing Operator
		/// <summary>
		/// Indexer ( [x, y] ).
		/// </summary>
		public double this[int index]
		{
			get	
			{
				switch( index ) 
				{
					case 0:
						return _x;
					case 1:
						return _y;
					default:
						throw new IndexOutOfRangeException();
				}
			}
			set 
			{
				switch( index ) 
				{
					case 0:
						_x = value;
						break;
					case 1:
						_y = value;
						break;
					default:
						throw new IndexOutOfRangeException();
				}
			}

		}

		#endregion

		#region Conversion Operators
		/// <summary>
		/// Converts the vector to an array of double-precision floating point values.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <returns>An array of double-precision floating point values.</returns>
		public static explicit operator double[](Vector2D v)
		{
			double[] array = new double[2];
			array[0] = v.X;
			array[1] = v.Y;
			return array;
		}
		/// <summary>
		/// Converts the vector to an array of double-precision floating point values.
		/// </summary>
		/// <param name="v">A <see cref="Vector2D"/> instance.</param>
		/// <returns>An array of double-precision floating point values.</returns>
		public static explicit operator DoubleArrayList(Vector2D v)
		{
			DoubleArrayList array = new DoubleArrayList(2);
			array.Add(v.X);
			array.Add(v.Y);

			return array;
		}
		#endregion
	}


	#region Vector2DConverter class
	/// <summary>
	/// Converts a <see cref="Vector2D"/> to and from string representation.
	/// </summary>
	public class Vector2DConverter : ExpandableObjectConverter
	{
		/// <summary>
		/// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
		/// <returns><b>true</b> if this converter can perform the conversion; otherwise, <b>false</b>.</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;

			return base.CanConvertFrom (context, sourceType);
		}
		/// <summary>
		/// Returns whether this converter can convert the object to the specified type, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="destinationType">A <see cref="Type"/> that represents the type you want to convert to.</param>
		/// <returns><b>true</b> if this converter can perform the conversion; otherwise, <b>false</b>.</returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;

			return base.CanConvertTo (context, destinationType);
		}
		/// <summary>
		/// Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">A <see cref="System.Globalization.CultureInfo"/> object. If a null reference (Nothing in Visual Basic) is passed, the current culture is assumed.</param>
		/// <param name="value">The <see cref="Object"/> to convert.</param>
		/// <param name="destinationType">The Type to convert the <paramref name="value"/> parameter to.</param>
		/// <returns>An <see cref="Object"/> that represents the converted value.</returns>
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if ((destinationType == typeof(string)) && (value is Vector2D))
			{
				Vector2D v = (Vector2D)value;
				return v.ToString();
			}

			return base.ConvertTo (context, culture, value, destinationType);
		}
		/// <summary>
		/// Converts the given object to the type of this converter, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture. </param>
		/// <param name="value">The <see cref="Object"/> to convert.</param>
		/// <returns>An <see cref="Object"/> that represents the converted value.</returns>
		/// <exception cref="ParseException">Failed parsing from string.</exception>
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value.GetType() == typeof(string))
			{
				return Vector2D.Parse((string)value);
			}

			return base.ConvertFrom (context, culture, value);
		}

		/// <summary>
		/// Returns whether this object supports a standard set of values that can be picked from a list.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <returns><b>true</b> if <see cref="GetStandardValues"/> should be called to find a common set of values the object supports; otherwise, <b>false</b>.</returns>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <summary>
		/// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be a null reference.</param>
		/// <returns>A <see cref="TypeConverter.StandardValuesCollection"/> that holds a standard set of valid values, or a null reference (Nothing in Visual Basic) if the data type does not support a standard set of values.</returns>
		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			StandardValuesCollection svc = 
				new StandardValuesCollection(new object[3] {Vector2D.Zero, Vector2D.XAxis, Vector2D.YAxis} );

			return svc;
		}
	}
	#endregion
	

}
