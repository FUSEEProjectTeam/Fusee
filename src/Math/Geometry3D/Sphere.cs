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
	/// Represents a sphere in 3D space.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(SphereConverter))]
	public struct Sphere : ISerializable, ICloneable
	{
		#region Private Fields
		private Vector3F _center;
		private float _radius;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Sphere"/> class using center and radius values.
		/// </summary>
		/// <param name="center">The sphere center point.</param>
		/// <param name="radius">The sphere radius.</param>
		public Sphere(Vector3F center, float radius)
		{
			_center = center;
			_radius = radius;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Sphere"/> class using values from another sphere instance.
		/// </summary>
		/// <param name="sphere">A <see cref="Sphere"/> instance to take values from.</param>
		public Sphere(Sphere sphere)
		{
			_center = sphere.Center;
			_radius = sphere.Radius;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Vector3F"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		private Sphere(SerializationInfo info, StreamingContext context)
		{
			_center = (Vector3F)info.GetValue("Center", typeof(Vector3F));
			_radius = info.GetSingle("Radius");
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the sphere's center.
		/// </summary>
		public Vector3F Center
		{
			get { return _center; }
			set { _center = value;}
		}
		/// <summary>
		/// Gets or sets the sphere's radius.
		/// </summary>
		public float Radius
		{
			get { return _radius; }
			set { _radius = value;}
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
			info.AddValue("Center", _center, typeof(Vector3F));
			info.AddValue("Radius", _radius);
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Sphere"/> object.
		/// </summary>
		/// <returns>The <see cref="Sphere"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Sphere(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Sphere"/> object.
		/// </summary>
		/// <returns>The <see cref="Sphere"/> object this method creates.</returns>
		public Sphere Clone()
		{
			return new Sphere(this);
		}
		#endregion

		#region Public Static Parse Methods
		/// <summary>
		/// Converts the specified string to its <see cref="Sphere"/> equivalent.
		/// </summary>
		/// <param name="s">A string representation of a <see cref="Sphere"/></param>
		/// <returns>A <see cref="Sphere"/> that represents the vector specified by the <paramref name="s"/> parameter.</returns>
		public static Sphere Parse(string s)
		{
			Regex r = new Regex(@"Sphere\(Center=(?<center>\([^\)]*\)), Radius=(?<radius>.*)\)", RegexOptions.None);
			Match m = r.Match(s);
			if (m.Success)
			{
				return new Sphere(
					Vector3F.Parse(m.Result("${center}")),
					float.Parse(m.Result("${radius}"))
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
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return _center.GetHashCode() ^ _radius.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Vector2D"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			if(obj is Sphere) 
			{
				Sphere s = (Sphere)obj;
				return 
					(_center == s.Center) && (_radius == s.Radius);
			}
			return false;
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			return string.Format( "Sphere(Center={0}, Radius={1})", _center, _radius);
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified spheres are equal.
		/// </summary>
		/// <param name="a">The left-hand sphere.</param>
		/// <param name="b">The right-hand sphere.</param>
		/// <returns><see langword="true"/> if the two spheres are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(Sphere a, Sphere b) 
		{
			return ValueType.Equals(a,b);
		}
		/// <summary>
		/// Tests whether two specified spheres are not equal.
		/// </summary>
		/// <param name="a">The left-hand sphere.</param>
		/// <param name="b">The right-hand sphere.</param>
		/// <returns><see langword="true"/> if the two spheres are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(Sphere a, Sphere b) 
		{
			return !ValueType.Equals(a,b);
		}
		#endregion
	}

	#region SphereConverter class
	/// <summary>
	/// Converts a <see cref="Sphere"/> to and from string representation.
	/// </summary>
	public class SphereConverter : ExpandableObjectConverter
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
			if ((destinationType == typeof(string)) && (value is Sphere))
			{
				Sphere c = (Sphere)value;
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
				return Sphere.Parse((string)value);
			}

			return base.ConvertFrom (context, culture, value);
		}
	}
	#endregion
}
