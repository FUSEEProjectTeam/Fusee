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
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Security.Permissions;

using Fusee.Math.Core;

namespace Fusee.Math.Geometry3D
{
	/// <summary>
	/// Represents a triangle in 3D space.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(TriangleConverter))]
	public struct Triangle : ICloneable, ISerializable
	{
		#region Private Fields
		private Vector3F _p0, _p1, _p2;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Triangle"/> class.
		/// </summary>
		/// <param name="p0">A <see cref="Vector3F"/> instance.</param>
		/// <param name="p1">A <see cref="Vector3F"/> instance.</param>
		/// <param name="p2">A <see cref="Vector3F"/> instance.</param>
		public Triangle(Vector3F p0, Vector3F p1, Vector3F p2)
		{
			_p0 = p0;
			_p1 = p1;
			_p2 = p2;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Triangle"/> class using a given Triangle.
		/// </summary>
		/// <param name="t">A <see cref="Triangle"/> instance.</param>
		public Triangle(Triangle t)
		{
			_p0 = t._p0;
			_p1 = t._p1;
			_p2 = t._p2;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Triangle"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		private Triangle(SerializationInfo info, StreamingContext context)
		{
			_p0 = (Vector3F)info.GetValue("P0", typeof(Vector3F));
			_p1 = (Vector3F)info.GetValue("P1", typeof(Vector3F));
			_p2 = (Vector3F)info.GetValue("P2", typeof(Vector3F));
		}

		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Triangle"/> object.
		/// </summary>
		/// <returns>The <see cref="Triangle"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Triangle(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Triangle"/> object.
		/// </summary>
		/// <returns>The <see cref="Triangle"/> object this method creates.</returns>
		public Triangle Clone()
		{
			return new Triangle(this);
		}
		#endregion

		#region ISerializable Members
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data. </param>
		/// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
		//[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("P0", _p0, typeof(Vector3F));
			info.AddValue("P1", _p1, typeof(Vector3F));
			info.AddValue("P2", _p2, typeof(Vector3F));
		}
		#endregion

		#region Public Static Parse Methods
		/// <summary>
		/// Converts the specified string to its <see cref="Triangle"/> equivalent.
		/// </summary>
		/// <param name="s">A string representation of a <see cref="Triangle"/></param>
		/// <returns>A <see cref="Triangle"/> that represents the vector specified by the <paramref name="s"/> parameter.</returns>
		public static Triangle Parse(string s)
		{
			Regex r = new Regex(@"Triangle\((?<p1>\([^\)]*\)), (?<p2>\([^\)]*\)), (?<p3>\([^\)]*\))\)", RegexOptions.None);
			Match m = r.Match(s);
			if (m.Success)
			{
				return new Triangle(
					Vector3F.Parse(m.Result("${p1}")),
					Vector3F.Parse(m.Result("${p2}")),
					Vector3F.Parse(m.Result("${p3}"))
					);
			}
			else
			{
				throw new ParseException("Unsuccessful Match.");
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Get the hashcode for this vector instance.
		/// </summary>
		/// <returns>Returns the hash code for this vector instance.</returns>
		public override int	GetHashCode() 
		{
			return	_p0.GetHashCode() ^ _p1.GetHashCode() ^ _p2.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Vector3F"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj) 
		{
			if(obj is Triangle) 
			{
				Triangle t = (Triangle)obj;
				return (_p0 == t.Point0) && (_p1 == t.Point1) && (_p2 == t.Point2);
			}
			return false;
		}
		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString() 
		{
			return string .Format("Triangle({0}, {1}, {2})", _p0, _p1, _p2);
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// The first triangle vertex.
		/// </summary>
		/// <value>A <see cref="Vector3F"/> instance.</value>
		public Vector3F Point0
		{
			get { return _p0; }
			set { _p0 = value;}
		}
		/// <summary>
		/// The second triangle vertex.
		/// </summary>
		/// <value>A <see cref="Vector3F"/> instance.</value>
		public Vector3F Point1
		{
			get { return _p1; }
			set { _p1 = value;}
		}
		/// <summary>
		/// The third triangle vertex.
		/// </summary>
		/// <value>A <see cref="Vector3F"/> instance.</value>
		public Vector3F Point2
		{
			get { return _p2; }
			set { _p2 = value;}
		}
		
		/// <summary>
		/// Computes the normal for this triangle.
		/// </summary>
		/// <value>A <see cref="Vector3F"/> instance.</value>
		public Vector3F Normal
		{
			get
			{
				Vector3F normal = Vector3F.CrossProduct(_p1 - _p0, _p2 - _p0);
				normal.Normalize();
				return normal;
			}
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified triangles are equal.
		/// </summary>
		/// <param name="a">The left-hand triangle.</param>
		/// <param name="b">The right-hand triangle.</param>
		/// <returns><see langword="true"/> if the two triangles are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(Triangle a, Triangle b)
		{
			return ValueType.Equals(a,b);
		}
		/// <summary>
		/// Tests whether two specified triangles are not equal.
		/// </summary>
		/// <param name="a">The left-hand triangle.</param>
		/// <param name="b">The right-hand triangle.</param>
		/// <returns><see langword="true"/> if the two triangles are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(Triangle a, Triangle b)
		{
			return !ValueType.Equals(a,b);
		}
		#endregion

		#region Array Indexing Operator
		/// <summary>
		/// Indexer ( [P0, P1, P2] ).
		/// </summary>
		public Vector3F this[int index]
		{
			get
			{
				switch(index)
				{
					case 0 : return _p0;
					case 1 : return _p1;
					case 2 : return _p2;
					default:
						throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch(index)
				{
					case 0 : _p0 = value; break;
					case 1 : _p1 = value; break;
					case 2 : _p2 = value; break;
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}
		#endregion

		/// <summary>
		/// Calculates a point in the triangle  from its barycentric coordinates.
		/// </summary>
		/// <param name="u">A single-precision floating point coordinate value.</param>
		/// <param name="v">A single-precision floating point coordinate value.</param>
		/// <returns>Returns a point inside the trianlge.</returns>
		public Vector3F FromBarycentric(float u, float v)
		{
			return ((1-u-v)*_p0)+(u*_p1)+(v*_p2);
		}
	}

	#region TriangleConverter class
	/// <summary>
	/// Converts a <see cref="Triangle"/> to and from string representation.
	/// </summary>
	public class TriangleConverter : ExpandableObjectConverter
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
			if ((destinationType == typeof(string)) && (value is Triangle))
			{
				Triangle r = (Triangle)value;
				return r.ToString();
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
				return Triangle.Parse((string)value);
			}

			return base.ConvertFrom (context, culture, value);
		}
	}
	#endregion
}
