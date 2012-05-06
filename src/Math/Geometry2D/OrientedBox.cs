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
	/// Represents an oriented box in 2D space.
	/// </summary>
	/// <remarks>
	/// An oriented box is a box whose faces have normals that are all pairwise orthogonal-i.e., it is an axis aligned box arbitrarily rotated.
	/// A 2D oriented box is defined by a center point, two orthonormal axes that describe the side
	/// directions of the box, and their respective positive half-lengths extents.
	/// </remarks>
	[Serializable]
	[TypeConverter(typeof(OrientedBoxConverter))]
	public struct OrientedBox : ISerializable, ICloneable
	{
		#region Private Fields
		private Vector2F _center;
		private Vector2F _axis1;
		private Vector2F _axis2;
		private float _extent1;
		private float _extent2;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="OrientedBox"/> class using given center point, axes and extents.
		/// </summary>
		/// <param name="center">The center of the box.</param>
		/// <param name="axis1">The first axis.</param>
		/// <param name="axis2">The second axis.</param>
		/// <param name="extent1">The extent on the first axis.</param>
		/// <param name="extent2">The extent on the second axis.</param>
		public OrientedBox(Vector2F center, Vector2F axis1, Vector2F axis2, float extent1, float extent2)
		{
			_center = center;
			_axis1 = axis1;
			_axis2 = axis2;
			_extent1 = extent1;
			_extent2 = extent2;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="OrientedBox"/> class using given center point, axes and extents.
		/// </summary>
		/// <param name="center">The center of the box.</param>
		/// <param name="axes">The axes of the box.</param>
		/// <param name="extents">The extent values of the box..</param>
		public OrientedBox(Vector2F center, Vector2F[] axes, float[] extents)
		{
			Debug.Assert(axes.Length >= 2);
			Debug.Assert(extents.Length >= 2);

			_center = center;
			
			_axis1 = axes[0];
			_axis2 = axes[1];

			_extent1 = extents[0];
			_extent2 = extents[1];
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="OrientedBox"/> class using given values from another box instance.
		/// </summary>
		/// <param name="box">A <see cref="OrientedBox"/> instance to take values from.</param>
		public OrientedBox(OrientedBox box)
		{
			_center = box.Center;

			_axis1 = box.Axis1;
			_axis2 = box.Axis2;

			_extent1 = box.Extent1;
			_extent2 = box.Extent2;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="OrientedBox"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		private OrientedBox(SerializationInfo info, StreamingContext context)
		{
			_center = (Vector2F)info.GetValue("Center", typeof(Vector2F));

			_axis1 = (Vector2F)info.GetValue("Axis1", typeof(Vector2F));
			_axis2 = (Vector2F)info.GetValue("Axis2", typeof(Vector2F));

			_extent1 = info.GetSingle("Extent1");
			_extent2 = info.GetSingle("Extent2");
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the box's center point.
		/// </summary>
		public Vector2F Center
		{
			get { return _center; }
			set { _center = value;}
		}
		/// <summary>
		/// Gets or sets the box's first axis.
		/// </summary>
		public Vector2F Axis1
		{
			get { return _axis1; }
			set { _axis1 = value;}
		}

		/// <summary>
		/// Gets or sets the box's second axis.
		/// </summary>
		public Vector2F Axis2
		{
			get { return _axis2; }
			set { _axis2 = value;}
		}
		/// <summary>
		/// Gets or sets the box's first extent.
		/// </summary>
		public float Extent1
		{
			get { return _extent1; }
			set { _extent1 = value;}
		}
		/// <summary>
		/// Gets or sets the box's second extent.
		/// </summary>
		public float Extent2
		{
			get { return _extent2; }
			set { _extent2 = value;}
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="OrientedBox"/> object.
		/// </summary>
		/// <returns>The <see cref="OrientedBox"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new OrientedBox(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="OrientedBox"/> object.
		/// </summary>
		/// <returns>The <see cref="OrientedBox"/> object this method creates.</returns>
		public OrientedBox Clone()
		{
			return new OrientedBox(this);
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
			info.AddValue("Center", _center, typeof(Vector2F));

			info.AddValue("Axis1", _axis1, typeof(Vector2F));
			info.AddValue("Axis2", _axis2, typeof(Vector2F));

			info.AddValue("Extent1", _extent1, typeof(Vector2F));
			info.AddValue("Extent2", _extent2, typeof(Vector2F));
		}
		#endregion

		#region Public Static Parse Methods
		/// <summary>
		/// Converts the specified string to its <see cref="OrientedBox"/> equivalent.
		/// </summary>
		/// <param name="s">A string representation of a <see cref="OrientedBox"/></param>
		/// <returns>A <see cref="OrientedBox"/> that represents the vector specified by the <paramref name="s"/> parameter.</returns>
		public static OrientedBox Parse(string s)
		{
			Regex r = new Regex(@"OrientedBox\(Center=(?<center>\([^\)]*\)), Axis1=(?<axis1>\([^\)]*\)), Axis2=(?<axis2>\([^\)]*\)), Extent1=(?<extent1>.*), Extent2=(?<extent2>.*)\)", RegexOptions.None);
			Match m = r.Match(s);
			if (m.Success)
			{
				return new OrientedBox(
					Vector2F.Parse(m.Result("${center}")),
					Vector2F.Parse(m.Result("${axis1}")),
					Vector2F.Parse(m.Result("${axis2}")),
					float.Parse(m.Result("${extent1}")),
					float.Parse(m.Result("${extent2}"))
					);
			}
			else
			{
				throw new ParseException("Unsuccessful Match.");
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Computes the box vertices. 
		/// </summary>
		/// <returns>An array of <see cref="Vector2F"/> containing the box vertices.</returns>
		public Vector2F[] ComputeVertices()
		{
			Vector2F[] vertices = new Vector2F[4];
			Vector2F[] AxisExtents = new Vector2F[2]
				{
					Axis1*Extent1, Axis2*Extent2
				};


			vertices[0] = Center - AxisExtents[0] - AxisExtents[1];
			vertices[1] = Center + AxisExtents[0] - AxisExtents[1];
			vertices[2] = Center + AxisExtents[0] + AxisExtents[1];
			vertices[3] = Center - AxisExtents[0] + AxisExtents[1];

			return vertices;
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return _center.GetHashCode() ^ _axis1.GetHashCode() ^ _axis2.GetHashCode() ^ _extent1.GetHashCode() ^ _extent2.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Vector2F"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is OrientedBox)
			{
				OrientedBox b = (OrientedBox)obj;
				return 
					(_center == b.Center) && 
					(_axis1 == b.Axis1) && (_axis2 == b.Axis2) &&
					(_extent1 == b.Extent1) && (_extent2 == b.Extent2);
			}
			return false;
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			return string.Format( "OrientedBox(Center={0}, Axis1={1}, Axis2={2}, Extent1={3}, Extent2={4})", 
				Center, Axis1, Axis2, Extent1, Extent2);
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified boxes are equal.
		/// </summary>
		/// <param name="a">The left-hand box.</param>
		/// <param name="b">The right-hand box.</param>
		/// <returns><see langword="true"/> if the two vectors are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(OrientedBox a, OrientedBox b)
		{
			return ValueType.Equals(a,b);
		}
		/// <summary>
		/// Tests whether two specified boxes are not equal.
		/// </summary>
		/// <param name="a">The left-hand box.</param>
		/// <param name="b">The right-hand box.</param>
		/// <returns><see langword="true"/> if the two boxes are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(OrientedBox a, OrientedBox b)
		{
			return ValueType.Equals(a,b);
		}

		#endregion
	}

	#region OrientedBoxConverter class
	/// <summary>
	/// Converts a <see cref="OrientedBox"/> to and from string representation.
	/// </summary>
	public class OrientedBoxConverter : ExpandableObjectConverter
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
			if ((destinationType == typeof(string)) && (value is OrientedBox))
			{
				OrientedBox box = (OrientedBox)value;
				return box.ToString();
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
				return OrientedBox.Parse((string)value);
			}

			return base.ConvertFrom (context, culture, value);
		}
	}
	#endregion
}
