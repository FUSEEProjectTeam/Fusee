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
	/// Represents an axis aligned box in 3D space.
	/// </summary>
	/// <remarks>
	/// An axis-aligned box is a box whose faces coincide with the standard basis axes.
	/// </remarks>
	[Serializable]
	[TypeConverter(typeof(AxisAlignedBoxConverter))]
	public struct AxisAlignedBox : ISerializable, ICloneable
	{
		#region Private Fields
		private Vector3F _min;
		private Vector3F _max;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="AxisAlignedBox"/> class using given minimum and maximum points.
		/// </summary>
		/// <param name="min">A <see cref="Vector3F"/> instance representing the minimum point.</param>
		/// <param name="max">A <see cref="Vector3F"/> instance representing the maximum point.</param>
		public AxisAlignedBox(Vector3F min, Vector3F max)
		{
			_min = min;
			_max = max;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="AxisAlignedBox"/> class using given values from another box instance.
		/// </summary>
		/// <param name="box">A <see cref="AxisAlignedBox"/> instance to take values from.</param>
		public AxisAlignedBox(AxisAlignedBox box)
		{
			_min = box.Min;
			_max = box.Max;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="AxisAlignedBox"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		private AxisAlignedBox(SerializationInfo info, StreamingContext context)
		{
			_min = (Vector3F)info.GetValue("Min", typeof(Vector3F));
			_max = (Vector3F)info.GetValue("Max", typeof(Vector3F));
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the minimum point which is the box's minimum X and Y coordinates.
		/// </summary>
		public Vector3F Min
		{
			get { return _min; }
			set { _min = value;}
		}
		/// <summary>
		/// Gets or sets the maximum point which is the box's maximum X and Y coordinates.
		/// </summary>
		public Vector3F Max
		{
			get { return _max; }
			set { _max = value;}
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
			info.AddValue("Max", _max, typeof(Vector3F));
			info.AddValue("Min", _min, typeof(Vector3F));
		}
		#endregion
	
		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="AxisAlignedBox"/> object.
		/// </summary>
		/// <returns>The <see cref="AxisAlignedBox"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new AxisAlignedBox(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="AxisAlignedBox"/> object.
		/// </summary>
		/// <returns>The <see cref="AxisAlignedBox"/> object this method creates.</returns>
		public AxisAlignedBox Clone()
		{
			return new AxisAlignedBox(this);
		}
		#endregion

		#region Public Static Parse Methods
		/// <summary>
		/// Converts the specified string to its <see cref="AxisAlignedBox"/> equivalent.
		/// </summary>
		/// <param name="s">A string representation of a <see cref="AxisAlignedBox"/></param>
		/// <returns>A <see cref="AxisAlignedBox"/> that represents the vector specified by the <paramref name="s"/> parameter.</returns>
		public static AxisAlignedBox Parse(string s)
		{
			Regex r = new Regex(@"AxisAlignedBox\(Min=(?<min>\([^\)]*\)), Max=(?<max>\([^\)]*\))\)", RegexOptions.None);
			Match m = r.Match(s);
			if (m.Success)
			{
				return new AxisAlignedBox(
					Vector3F.Parse(m.Result("${min}")),
					Vector3F.Parse(m.Result("${max}"))
					);
			}
			else
			{
				throw new ParseException("Unsuccessful Match.");
			}
		}
		#endregion

		#region Public Static Bounding Methods
		public static AxisAlignedBox BoudningAABB(Sphere s)
		{
			Vector3F max = s.Center + s.Radius;
			Vector3F min = s.Center - s.Radius;
			return new AxisAlignedBox(min, max);
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Computes the box vertices. 
		/// </summary>
		/// <returns>An array of <see cref="Vector3F"/> containing the box vertices.</returns>
		public Vector3F[] ComputeVertices()
		{
			Vector3F[] vertices = new Vector3F[8];

			vertices[0] = _min;
			vertices[1] = new Vector3F(_max.X, _min.Y, _min.Z);
			vertices[2] = new Vector3F(_max.X, _max.Y, _min.Z);
			vertices[4] = new Vector3F(_min.X, _max.Y, _min.Z);

			vertices[5] = new Vector3F(_min.X, _min.Y, _max.Z);
			vertices[6] = new Vector3F(_max.X, _min.Y, _max.Z);
			vertices[7] = _max;
			vertices[8] = new Vector3F(_min.X, _max.Y, _max.Z);

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
			return _min.GetHashCode() ^ _max.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Vector3F"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is AxisAlignedBox)
			{
				AxisAlignedBox box = (AxisAlignedBox)obj;
				return (_min == box.Min) && (_max == box.Max);
			}
			return false;
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			return string.Format("AxisAlignedBox(Min={0}, Max={1})", _min, _max);
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Checks if the two given boxes are equal.
		/// </summary>
		/// <param name="a">The first of two boxes to compare.</param>
		/// <param name="b">The second of two boxes to compare.</param>
		/// <returns><b>true</b> if the boxes are equal; otherwise, <b>false</b>.</returns>
		public static bool operator==(AxisAlignedBox a, AxisAlignedBox b) 
		{
			return ValueType.Equals(a,b);
		}

		/// <summary>
		/// Checks if the two given boxes are not equal.
		/// </summary>
		/// <param name="a">The first of two boxes to compare.</param>
		/// <param name="b">The second of two boxes to compare.</param>
		/// <returns><b>true</b> if the vectors are not equal; otherwise, <b>false</b>.</returns>
		public static bool operator!=(AxisAlignedBox a, AxisAlignedBox b) 
		{
			return !ValueType.Equals(a,b);
		}
		#endregion	
	}

	#region AxisAlignedBoxConverter class
	/// <summary>
	/// Converts a <see cref="AxisAlignedBox"/> to and from string representation.
	/// </summary>
	public class AxisAlignedBoxConverter : ExpandableObjectConverter
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
			if ((destinationType == typeof(string)) && (value is AxisAlignedBox))
			{
				AxisAlignedBox box = (AxisAlignedBox)value;
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
				return AxisAlignedBox.Parse((string)value);
			}

			return base.ConvertFrom (context, culture, value);
		}
	}
	#endregion
}
