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

namespace Fusee.Math.Geometry2D
{
	/// <summary>
	/// Represents a circle in 2D space.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(CircleConverter))]
	public struct Circle : ICloneable, ISerializable
	{
		private Vector2F center;
		private float radius;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Circle"/> class using center and radius values.
		/// </summary>
		/// <param name="center">The circle's center point.</param>
		/// <param name="radius">The circle's radius.</param>
		public Circle(Vector2F center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Circle"/> class using values from another circle instance.
		/// </summary>
		/// <param name="circle">A <see cref="Circle"/> instance to take values from.</param>
		public Circle(Circle circle)
		{
			this.center = circle.center;
			this.radius = circle.radius;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Vector2F"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		private Circle(SerializationInfo info, StreamingContext context)
		{
			this.center = (Vector2F)info.GetValue("Center", typeof(Vector2F));
			this.radius = info.GetSingle("Radius");
		}
		#endregion

		#region Constants
		/// <summary>
		/// Unit sphere.
		/// </summary>
		public static readonly Circle UnitCircle	= new Circle(new Vector2F(0.0f,0.0f), 1.0f);
		#endregion

		#region Public Properties
		/// <summary>
		/// The circle's center.
		/// </summary>
		public Vector2F Center
		{
			get
			{
				return this.center;
			}
			set
			{
				this.center = value;
			}
		}
		/// <summary>
		/// The circle's radius.
		/// </summary>
		public float Radius
		{
			get
			{
				return this.radius;
			}
			set
			{
				this.radius = value;
			}
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
			info.AddValue("Center", this.center, typeof(Vector2F));
			info.AddValue("Radius", this.radius);
		}
		#endregion
	
		#region ICloneable Members
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		object ICloneable.Clone()
		{
			return new Circle(this);
		}
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public Circle Clone()
		{
			return new Circle(this);
		}
		#endregion

		#region Public Static Parse Methods
		/// <summary>
		/// Converts the specified string to its <see cref="Circle"/> equivalent.
		/// </summary>
		/// <param name="s">A string representation of a <see cref="Circle"/></param>
		/// <returns>A <see cref="Circle"/> that represents the vector specified by the <paramref name="s"/> parameter.</returns>
		public static Circle Parse(string s)
		{
			Regex r = new Regex(@"Circle\(Center=(?<center>\([^\)]*\)), Radius=(?<radius>.*)\)", RegexOptions.None);
			Match m = r.Match(s);
			if (m.Success)
			{
				return new Circle(
					Vector2F.Parse(m.Result("${center}")),
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
		/// Get the hashcode for this vector instance.
		/// </summary>
		/// <returns>Returns the hash code for this vector instance.</returns>
		public override int	GetHashCode() 
		{
			return this.center.GetHashCode() ^ this.radius.GetHashCode();
		}
		/// <summary>
		/// Checks if a given <see cref="Circle"/> equals to self.
		/// </summary>
		/// <param name="o">Object to check if equal to.</param>
		/// <returns></returns>
		public override bool Equals(object o) 
		{
			if( o is Circle ) 
			{
				Circle c = (Circle) o;
				return 
					(this.center == c.Center) && (this.radius == c.radius);
			}
			return false;
		}
		/// <summary>
		/// Convert <see cref="Circle"/> to a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString() 
		{
			return	string.Format( "Circle(Center={0}, Radius={1})", this.center, this.radius);
		}
		#endregion

		#region Operators
		/// <summary>
		/// Checks if the two given circles are equal.
		/// </summary>
		/// <param name="a">The first of two 2D circles to compare.</param>
		/// <param name="b">The second of two 2D circles to compare.</param>
		/// <returns><b>true</b> if the circles are equal; otherwise, <b>false</b>.</returns>
		public static bool operator==(Circle a, Circle b) 
		{
			return ValueType.Equals(a,b);
		}

		/// <summary>
		/// Checks if the two given circles are not equal.
		/// </summary>
		/// <param name="a">The first of two 2D circles to compare.</param>
		/// <param name="b">The second of two 2D circles to compare.</param>
		/// <returns><b>true</b> if the circles are not equal; otherwise, <b>false</b>.</returns>
		public static bool operator!=(Circle a, Circle b) 
		{
			return !ValueType.Equals(a,b);
		}
		#endregion

		/// <summary>
		/// Tests if a ray intersects the sphere.
		/// </summary>
		/// <param name="ray">The ray to test.</param>
		/// <returns>Returns True if the ray intersects the sphere. otherwise, <see langword="false"/>.</returns>
		public bool TestIntersection(Ray ray)
		{
			float squaredDistance = DistanceMethods.SquaredDistance(this.center, ray);
			return (squaredDistance <= this.radius*this.radius);
		}

		/// <summary>
		/// Find the intersection of a ray and a sphere.
		/// Only works with unit rays (normalized direction)!!!
		/// </summary>
		/// <remarks>
		/// This is the optimized Ray-Sphere intersection algorithms described in "Real-Time Rendering".
		/// </remarks>
		/// <param name="ray">The ray to test.</param>
		/// <param name="t">
		/// If intersection accurs, the function outputs the distance from the ray's origin 
		/// to the closest intersection point to this parameter.
		/// </param>
		/// <returns>Returns True if the ray intersects the sphere. otherwise, <see langword="false"/>.</returns>
		public bool FindIntersections(Ray ray, ref float t)
		{
			// Only gives correct result for unit rays.
			//Debug.Assert(MathUtils.ApproxEquals(1.0f, ray.Direction.GetLength()), "Ray direction should be normalized!");

			// Calculates a vector from the ray origin to the sphere center.
			Vector2F diff		= this.center - ray.Origin;
			// Compute the projection of diff onto the ray direction
			float d = Vector2F.Dot(diff, ray.Direction);
		
			float diffSquared	= diff.GetLengthSquared();
			float radiusSquared = this.radius * this.radius;

			// First rejection test : 
			// if d<0 and the ray origin is outside the sphere than the sphere is behind the ray
			if ((d < 0.0f) && (diffSquared > radiusSquared))
			{
				return false;
			}

			// Compute the distance from the sphere center to the projection
			float mSquared = diffSquared - d*d;

			// Second rejection test:
			// if mSquared > radiusSquared than the ray misses the sphere
			if (mSquared > radiusSquared)
			{
				return false;
			}

			float q = (float)System.Math.Sqrt(radiusSquared - mSquared);

			// We are interested only in the first intersection point:
			if (diffSquared > radiusSquared)
			{
				// If the origin is outside the sphere t = d - q is the first intersection point
				t = d - q;
			}
			else 
			{
				// If the origin is inside the sphere t = d + q is the first intersection point
				t = d + q;
			}
			return true;
		}
	}

	#region CircleConverter class
	/// <summary>
	/// Converts a <see cref="Circle"/> to and from string representation.
	/// </summary>
	public class CircleConverter : ExpandableObjectConverter
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
			if ((destinationType == typeof(string)) && (value is Circle))
			{
				Circle c = (Circle)value;
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
				return Circle.Parse((string)value);
			}

			return base.ConvertFrom (context, culture, value);
		}
	}
	#endregion
}
