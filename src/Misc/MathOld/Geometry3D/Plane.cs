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
	/// Represents a plane in 3D space.
	/// </summary>
	/// <remarks>
	/// The plane is described by a normal and a constant (N,D) which 
	/// denotes that the plane is consisting of points Q that
	/// satisfies (N dot Q)+D = 0.
	/// </remarks>
	[Serializable]
	[TypeConverter(typeof(PlaneConverter))]
	public struct Plane : ISerializable, ICloneable
	{
		#region Private Fields
		private Vector3F _normal;
		private float _const;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Plane"/> class using given normal and constant values.
		/// </summary>
		/// <param name="normal">The plane's normal vector.</param>
		/// <param name="constant">The plane's constant value.</param>
		public Plane(Vector3F normal, float constant)
		{
			_normal	= normal;
			_const	= constant;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Plane"/> class using given normal and a point.
		/// </summary>
		/// <param name="normal">The plane's normal vector.</param>
		/// <param name="point">A point on the plane in 3D space.</param>
		public Plane(Vector3F normal, Vector3F point)
		{
			_normal = normal;
			_const	= Vector3F.Dot(normal, point);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Plane"/> class using 3 given points.
		/// </summary>
		/// <param name="p0">A point on the plane in 3D space.</param>
		/// <param name="p1">A point on the plane in 3D space.</param>
		/// <param name="p2">A point on the plane in 3D space.</param>
		public Plane(Vector3F p0, Vector3F p1, Vector3F p2)
		{
			_normal = Vector3F.CrossProduct(p2-p1, p0-p1);
			_normal.Normalize();
			_const	= Vector3F.Dot(_normal, p0);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Plane"/> class using given a plane to assign values from.
		/// </summary>
		/// <param name="p">A 3D plane to assign values from.</param>
		public Plane(Plane p)
		{
			_normal	= p.Normal;
			_const	= p.Constant;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Plane"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		private Plane(SerializationInfo info, StreamingContext context)
		{
			_normal = (Vector3F)info.GetValue("Normal", typeof(Vector3F));
			_const	= info.GetSingle("Constant");
		}
		#endregion

		#region Constants
		/// <summary>
		/// Plane on the X axis.
		/// </summary>
		public static readonly Plane XPlane = new Plane(Vector3F.XAxis, Vector3F.Zero);
		/// <summary>
		/// Plane on the Y axis.
		/// </summary>
		public static readonly Plane YPlane = new Plane(Vector3F.YAxis, Vector3F.Zero);
		/// <summary>
		/// Plane on the Z axis.
		/// </summary>
		public static readonly Plane ZPlane = new Plane(Vector3F.ZAxis, Vector3F.Zero);
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the plane's normal vector.
		/// </summary>
		public Vector3F Normal
		{
			get { return _normal; }
			set { _normal = value;}
		}
		/// <summary>
		/// Gets or sets the plane's constant value.
		/// </summary>
		public float Constant
		{
			get { return _const; }
			set { _const = value;}
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Plane"/> object.
		/// </summary>
		/// <returns>The <see cref="Plane"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Plane(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Plane"/> object.
		/// </summary>
		/// <returns>The <see cref="Plane"/> object this method creates.</returns>
		public Plane Clone()
		{
			return new Plane(this);
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
			info.AddValue("Normal", _normal, typeof(Vector3F));
			info.AddValue("Constant", _const);
		}
		#endregion

		#region Public Static Parse Methods
		/// <summary>
		/// Converts the specified string to its <see cref="Plane"/> equivalent.
		/// </summary>
		/// <param name="s">A string representation of a <see cref="Plane"/></param>
		/// <returns>A <see cref="Plane"/> that represents the vector specified by the <paramref name="s"/> parameter.</returns>
		public static Plane Parse(string s)
		{
			Regex r = new Regex(@"Plane\(n=(?<normal>\([^\)]*\)), c=(?<const>.*)\)", RegexOptions.None);
			Match m = r.Match(s);
			if (m.Success)
			{
				return new Plane(
					Vector3F.Parse(m.Result("${normal}")),
					float.Parse(m.Result("${const}"))
					);
			}
			else
			{
				throw new ParseException("Unsuccessful Match.");
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Flip the plane.
		/// </summary>
		public void Flip()
		{
			_normal = -_normal;
		}
		/// <summary>
		/// Creates a new flipped plane (-normal, constant).
		/// </summary>
		/// <returns>A new <see cref="Plane"/> instance.</returns>
		public Plane GetFlipped()
		{
			return new Plane(-_normal, _const);
		}
		/// <summary>
		/// Returns the points's position relative to the plane itself (i.e Front/Back/On)
		/// </summary>
		/// <param name="p">A point in 3D space.</param>
		/// <returns>A <see cref="MathFunctions.Sign"/>.</returns>
		public MathFunctions.Sign GetSign(Vector3F p) 
		{
			return MathFunctions.GetSign(DistanceMethods.Distance(p,this));
		}
		/// <summary>
		/// Returns the points's position relative to the plane itself (i.e Front/Back/On)
		/// </summary>
		/// <param name="p">A point in 3D space.</param>
		/// <param name="tolerance">The tolerance value to use.</param>
		/// <returns>A <see cref="MathFunctions.Sign"/>.</returns>
		/// <remarks>
		/// If the point's distance from the plane is withon the [-tolerance, tolerance] range, the point is considered to be on the plane.
		/// </remarks>
		public MathFunctions.Sign GetSign(Vector3F p, float tolerance) 
		{
			return MathFunctions.GetSign(DistanceMethods.Distance(p,this), tolerance);
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return _normal.GetHashCode() ^ _const.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Vector2D"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			if(obj is Plane) 
			{
				Plane p = (Plane)obj;
				return (_normal == p.Normal) && (_const == p.Constant);
			}
			return false;
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			return string.Format("Plane[n={0}, c={1}]", _normal, _const);
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified planes are equal.
		/// </summary>
		/// <param name="a">The left-hand plane.</param>
		/// <param name="b">The right-hand plane.</param>
		/// <returns><see langword="true"/> if the two planes are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(Plane a, Plane b)
		{
			return ValueType.Equals(a,b);
		}
		/// <summary>
		/// Tests whether two specified planes are not equal.
		/// </summary>
		/// <param name="a">The left-hand plane.</param>
		/// <param name="b">The right-hand plane.</param>
		/// <returns><see langword="true"/> if the two planes are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(Plane a, Plane b)
		{
			return !ValueType.Equals(a,b);
		}
		#endregion
	}

	#region PlaneConverter class
	/// <summary>
	/// Converts a <see cref="Plane"/> to and from string representation.
	/// </summary>
	public class PlaneConverter : ExpandableObjectConverter
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
			if ((destinationType == typeof(string)) && (value is Plane))
			{
				Plane c = (Plane)value;
				return c.ToString();
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
				return Plane.Parse((string)value);
			}

			return base.ConvertFrom (context, culture, value);
		}
	}
	#endregion
}
