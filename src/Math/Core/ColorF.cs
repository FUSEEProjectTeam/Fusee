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
	/// Represents an RGBA color.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ColorF : ISerializable, ICloneable
	{
		#region Private Fields
		private float _red,_green,_blue,_alpha;
		#endregion

		#region Constructores
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorF"/> class.
		/// </summary>
		/// <remarks>
		/// Default values are 1.0f for Alpha and 0.0f for the color channels.
		/// </remarks>
		public ColorF()
		{
			_red	= 0.0f;
			_green	= 0.0f;
			_blue	= 0.0f;
			_alpha	= 1.0f;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorF"/> class.
		/// </summary>
		/// <param name="red">Red channel value.</param>
		/// <param name="green">Green channel value.</param>
		/// <param name="blue">Blue channel value.</param>
		/// <remarks>The alpha channel value is set to 1.0f.</remarks>
		public ColorF(float red, float green, float blue)
		{
			_red	= red;
			_green	= green;
			_blue	= blue;
			_alpha	= 1.0f;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorF"/> class.
		/// </summary>
		/// <param name="red">Red channel value.</param>
		/// <param name="green">Green channel value.</param>
		/// <param name="blue">Blue channel value.</param>
		/// <param name="alpha">Alpha channel value.</param>
		public ColorF(float red, float green, float blue, float alpha)
		{
			_red	= red;
			_green	= green;
			_blue	= blue;
			_alpha	= alpha;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorF"/> class using values from another instance.
		/// </summary>
		/// <param name="color">A <see cref="ColorF"/> instance.</param>
		public ColorF(ColorF color)
		{
			_red	= color.Red;
			_green	= color.Green;
			_blue	= color.Blue;
			_alpha	= color.Alpha;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorF"/> class from a blend of two colors.
		/// </summary>
		/// <param name="source">The blend source color.</param>
		/// <param name="dest">The blend destination color.</param>
		/// <param name="opacity">The opacity value.</param>
		public ColorF(ColorF source, ColorF dest, float opacity)
		{
			_red	= MathFunctions.LinearInterpolation(source.Red, dest.Red, opacity);
			_green	= MathFunctions.LinearInterpolation(source.Green, dest.Green, opacity);
			_blue	= MathFunctions.LinearInterpolation(source.Blue, dest.Blue, opacity);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorF"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected ColorF(SerializationInfo info, StreamingContext context)
		{
			_red	= info.GetSingle("Red");
			_green	= info.GetSingle("Green");
			_blue	= info.GetSingle("Blue");
			_alpha	= info.GetSingle("Alpha");
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the red channel value.
		/// </summary>
		public float Red
		{
			get { return _red; }
			set { _red = value;}
		}
		/// <summary>
		/// Gets or sets the green channel value.
		/// </summary>
		public float Green
		{
			get { return _green; }
			set { _green = value;}
		}
		/// <summary>
		/// Gets or sets the blue channel value.
		/// </summary>
		public float Blue
		{
			get { return _blue; }
			set { _blue = value;}
		}
		/// <summary>
		/// Gets or sets the alpha channel value.
		/// </summary>
		public float Alpha
		{
			get { return _alpha; }
			set { _alpha = value;}
		}
		/// <summary>
		/// Gets the color's intensity.
		/// </summary>
		/// <remarks>
		/// Intensity = (R + G + B) / 3
		/// </remarks>
		public float Intensity
		{
			get { return (_red + _green + _blue) / 3.0f; }
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="ColorF"/> object.
		/// </summary>
		/// <returns>The <see cref="ColorF"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new ColorF(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="ColorF"/> object.
		/// </summary>
		/// <returns>The <see cref="ColorF"/> object this method creates.</returns>
		public ColorF Clone()
		{
			return new ColorF(this);
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
			info.AddValue("Red", _red);
			info.AddValue("Green", _green);
			info.AddValue("Blue", _blue);
			info.AddValue("Alpha", _alpha);
		}
		#endregion

		#region Public Static Parse Methods
		/// <summary>
		/// Converts the specified string to its <see cref="ColorF"/> equivalent.
		/// </summary>
		/// <param name="s">A string representation of a <see cref="ColorF"/></param>
		/// <returns>A <see cref="ColorF"/> that represents the vector specified by the <paramref name="s"/> parameter.</returns>
		public static ColorF Parse(string s)
		{
			Regex r = new Regex(@"\ColorF((?<r>.*),(?<g>.*),(?<b>.*),(?<a>.*)\)", RegexOptions.None);
			Match m = r.Match(s);
			if (m.Success)
			{
				return new ColorF(
					float.Parse(m.Result("${r}")),
					float.Parse(m.Result("${g}")),
					float.Parse(m.Result("${b}")),
					float.Parse(m.Result("${a}"))
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
		/// Clamp the RGBA value to [0, 1] range.
		/// </summary>
		/// <remarks>
		/// Values above 1.0f are clamped to 1.0f.
		/// Values below 0.0f are clamped to 0.0f.
		/// </remarks>
		public void Clamp()
		{
			if (_red < 0.0f) 
				_red = 0.0f;
			else if (_red > 1.0f) 
				_red = 1.0f;

			if (_green < 0.0f) 
				_green = 0.0f;
			else if (_green > 1.0f) 
				_green = 1.0f;

			if (_blue < 0.0f) 
				_blue = 0.0f;
			else if (_blue > 1.0f) 
				_blue = 1.0f;
		
			if (_alpha < 0.0f) 
				_alpha = 0.0f;
			else if (_alpha > 1.0f) 
				_alpha = 1.0f;
		}
		/// <summary>
		/// Calculates the color's HSV values.
		/// </summary>
		/// <param name="h">The Hue value.</param>
		/// <param name="s">The Saturation value.</param>
		/// <param name="v">The Value value.</param>
		public void ToHSV(out float h, out float s, out float v)
		{
			float min = MathFunctions.MinValue((Vector3F)this);
			float max = MathFunctions.MaxValue((Vector3F)this);
			v = max;

			float delta = max - min;
			if( max != 0.0f )
			{
				s = delta / max;
			}
			else 
			{
				// r = g = b = 0.0f --> s = 0, v is undefined
				s = 0.0f;
				h = 0.0f;
				return;
			}

			if(_red == max)
			{
				h = ( _green - _blue ) / delta;		// between yellow & magenta
			}
			else if(_green == max)
			{
				h = 2 + ( _blue - _red ) / delta;	// between cyan & yellow
			}
			else
			{
				h = 4 + ( _red - _green ) / delta;	// between magenta & cyan
			}

			h *= 60.0f; // degrees
			if( h < 0.0f )
				h += 360.0f;

		}
		#endregion

		#region Overrides
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return _red.GetHashCode() ^ _green.GetHashCode() ^ _blue.GetHashCode() ^ _alpha.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="ColorF"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			ColorF color = obj as ColorF;
			if (color != null)
				return (_red == color.Red) && (_green == color.Green) && (_blue == color.Blue) && (_alpha == color.Alpha);
			else 
				return false;
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			return string.Format("ColorF({0}, {1}, {2}, {3})", _red, _green, _blue, _alpha);
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified <see cref="ColorF"/> instances are equal.
		/// </summary>
		/// <param name="u">The left-hand <see cref="ColorF"/> instance.</param>
		/// <param name="v">The right-hand <see cref="ColorF"/> instance.</param>
		/// <returns><see langword="true"/> if the two <see cref="ColorF"/> instances are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(ColorF u, ColorF v)
		{
			return Object.Equals(u,v);
		}
		/// <summary>
		/// Tests whether two specified <see cref="ColorF"/> instances are not equal.
		/// </summary>
		/// <param name="u">The left-hand <see cref="ColorF"/> instance.</param>
		/// <param name="v">The right-hand <see cref="ColorF"/> instance.</param>
		/// <returns><see langword="true"/> if the two <see cref="ColorF"/> instances are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(ColorF u, ColorF v)
		{
			return !Object.Equals(u,v);
		}
		#endregion

		#region Conversion Operators
		/// <summary>
		/// Converts the color to a <see cref="Vector3F"/> instance.
		/// </summary>
		/// <param name="color">A <see cref="ColorF"/> instance.</param>
		/// <returns>An <see cref="Vector3F"/> instance.</returns>
		public static explicit operator Vector3F(ColorF color)
		{
			return new Vector3F(color.Red, color.Green, color.Blue);
		}
		/// <summary>
		/// Converts the color to a <see cref="Vector4F"/> instance.
		/// </summary>
		/// <param name="color">A <see cref="ColorF"/> instance.</param>
		/// <returns>An <see cref="Vector4F"/> instance.</returns>
		public static explicit operator Vector4F(ColorF color)
		{
			return new Vector4F(color.Red, color.Green, color.Blue, color.Alpha);
		}
		/// <summary>
		/// Converts the color structure to an array of single-precision floating point values.
		/// </summary>
		/// <param name="color">A <see cref="ColorF"/> instance.</param>
		/// <returns>An array of single-precision floating point values.</returns>
		public static explicit operator float[](ColorF color)
		{
			float[] array = new float[4];
			array[0] = color.Red;
			array[1] = color.Green;
			array[2] = color.Blue;
			array[3] = color.Alpha;
			return array;
		}
		/// <summary>
		/// Converts the color structure to an array of single-precision floating point values.
		/// </summary>
		/// <param name="color">A <see cref="ColorF"/> instance.</param>
		/// <returns>An array of single-precision floating point values.</returns>
		public static explicit operator FloatArrayList(ColorF color)
		{
			FloatArrayList array = new FloatArrayList(4);
			array[0] = color.Red;
			array[1] = color.Green;
			array[2] = color.Blue;
			array[3] = color.Alpha;
			return array;
		}

		#endregion

	}
}
