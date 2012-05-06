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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
	/// <summary>
	/// Represents a complex double-precision doubleing point number.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	[TypeConverter(typeof(ComplexDConverter))]
	public struct ComplexD : ICloneable, ISerializable
	{
		private double _real;
		private double _image;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ComplexD"/> class using given real and imaginary values.
		/// </summary>
		/// <param name="real">Real value.</param>
		/// <param name="imaginary">Imaginary value.</param>
		public ComplexD(double real, double imaginary)
		{
			_real = real;
			_image = imaginary;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ComplexD"/> class using values from a given complex instance.
		/// </summary>
		/// <param name="c">A complex number to get values from.</param>
		public ComplexD(ComplexD c)
		{
			_real = c.Real;
			_image = c.Imaginary;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ComplexD"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		private ComplexD(SerializationInfo info, StreamingContext context)
		{
			_real	= info.GetSingle("Real");
			_image  = info.GetSingle("Imaginary");
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the real value of the complex number.
		/// </summary>
		/// <value>The real value of this complex number.</value>
		public double Real
		{
			get { return _real; }
			set { _real = value;}
		}
		/// <summary>
		/// Gets or sets the imaginary value of the complex number.
		/// </summary>
		/// <value>The imaginary value of this complex number.</value>
		public double Imaginary
		{
			get { return _image; }
			set { _image = value;}
		}
		#endregion

		#region Constants
		/// <summary>
		///  A double-precision complex number that represents zero.
		/// </summary>
		public static readonly ComplexD Zero = new ComplexD(0,0);
		/// <summary>
		///  A double-precision complex number that represents one.
		/// </summary>
		public static readonly ComplexD One	= new ComplexD(1,0);
		/// <summary>
		///  A double-precision complex number that represents the squere root of (-1).
		/// </summary>
		public static readonly ComplexD I	= new ComplexD(0,1);
		#endregion
	
		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="ComplexD"/> object.
		/// </summary>
		/// <returns>The <see cref="ComplexD"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new ComplexD(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="ComplexD"/> object.
		/// </summary>
		/// <returns>The <see cref="ComplexD"/> object this method creates.</returns>
		public ComplexD Clone()
		{
			return new ComplexD(this);
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
			info.AddValue("Real", this.Real);
			info.AddValue("Imaginary", this.Imaginary);
		}
		#endregion

		#region Public Static Parse Methods
		/// <summary>
		/// Converts the specified string to its <see cref="ComplexD"/> equivalent.
		/// </summary>
		/// <param name="s">A string representation of a <see cref="ComplexD"/></param>
		/// <returns>A <see cref="ComplexD"/> that represents the vector specified by the <paramref name="s"/> parameter.</returns>
		public static ComplexD Parse(string s)
		{
			Regex r = new Regex(@"\((?<real>.*),(?<imaginary>.*)\)", RegexOptions.None);
			Match m = r.Match(s);
			if (m.Success)
			{
				return new ComplexD(
					double.Parse(m.Result("${real}")),
					double.Parse(m.Result("${imaginary}"))
					);
			}
			else
			{
				throw new ParseException("Unsuccessful Match.");
			}
		}
		#endregion

		#region Public Static Complex Arithmetics
		/// <summary>
		/// Adds two complex numbers.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the sum.</returns>
		public static ComplexD Add(ComplexD a, ComplexD b)
		{
			return new ComplexD(a.Real + b.Real, a.Imaginary + b.Imaginary);
		}
		/// <summary>
		/// Adds a complex number and a scalar.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the sum.</returns>
		public static ComplexD Add(ComplexD a, double s)
		{
			return new ComplexD(a.Real + s, a.Imaginary);
		}
		/// <summary>
		/// Adds two complex numbers and put the result in the third complex number.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <param name="result">A <see cref="ComplexD"/> instance to hold the result.</param>
		public static void Add(ComplexD a, ComplexD b, ref ComplexD result)
		{
			result.Real = a.Real + b.Real;
			result.Imaginary = a.Imaginary + b.Imaginary;
		}
		/// <summary>
		/// Adds a complex number and a scalar and put the result into another complex.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="result">A <see cref="ComplexD"/> instance to hold the result.</param>
		public static void Add(ComplexD a, double s, ref ComplexD result)
		{
			result.Real = a.Real + s;
			result.Imaginary = a.Imaginary;
		}

		/// <summary>
		/// Subtracts a complex from a complex.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the difference.</returns>
		public static ComplexD Subtract(ComplexD a, ComplexD b)
		{
			return new ComplexD(a.Real - b.Real, a.Imaginary - b.Imaginary);
		}
		/// <summary>
		/// Subtracts a scalar from a complex.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the difference.</returns>
		public static ComplexD Subtract(ComplexD a, double s)
		{
			return new ComplexD(a.Real - s, a.Imaginary);
		}
		/// <summary>
		/// Subtracts a complex from a scalar.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the difference.</returns>
		public static ComplexD Subtract(double s, ComplexD a)
		{
			return new ComplexD(s - a.Real, a.Imaginary);
		}
		/// <summary>
		/// Subtracts a complex from a complex and put the result in the third complex number.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <param name="result">A <see cref="ComplexD"/> instance to hold the result.</param>
		public static void Subtract(ComplexD a, ComplexD b, ref ComplexD result)
		{
			result.Real = a.Real - b.Real;
			result.Imaginary = a.Imaginary - b.Imaginary;
		}
		/// <summary>
		/// Subtracts a scalar from a complex and put the result into another complex.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="result">A <see cref="ComplexD"/> instance to hold the result.</param>
		public static void Subtract(ComplexD a, double s, ref ComplexD result)
		{
			result.Real = a.Real - s;
			result.Imaginary = a.Imaginary;
		}		
		/// <summary>
		/// Subtracts a complex from a scalar and put the result into another complex.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="result">A <see cref="ComplexD"/> instance to hold the result.</param>
		public static void Subtract(double s, ComplexD a, ref ComplexD result)
		{
			result.Real = s - a.Real;
			result.Imaginary = a.Imaginary;
		}		
		/// <summary>
		/// Multiplies two complex numbers.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the result.</returns>
		public static ComplexD Multiply(ComplexD a, ComplexD b)
		{
			// (x + yi)(u + vi) = (xu – yv) + (xv + yu)i. 
			double x = a.Real, y = a.Imaginary;
			double u = b.Real, v = b.Imaginary;
			
			return new ComplexD(x*u - y*v, x*v + y*u);
		}
		/// <summary>
		/// Multiplies a complex by a scalar.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the result.</returns>
		public static ComplexD Multiply(ComplexD a, double s)
		{
			return new ComplexD(a.Real * s, a.Imaginary * s);
		}
		/// <summary>
		/// Multiplies a complex by a scalar.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the result.</returns>
		public static ComplexD Multiply(double s, ComplexD a)
		{
			return new ComplexD(a.Real * s, a.Imaginary * s);
		}
		/// <summary>
		/// Multiplies two complex numbers and put the result in a third complex number.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <param name="result">A <see cref="ComplexD"/> instance to hold the result.</param>
		public static void Multiply(ComplexD a, ComplexD b, ref ComplexD result)
		{
			// (x + yi)(u + vi) = (xu – yv) + (xv + yu)i. 
			double x = a.Real, y = a.Imaginary;
			double u = b.Real, v = b.Imaginary;
			
			result.Real = x*u - y*v;
			result.Imaginary = x*v + y*u;
		}
		/// <summary>
		/// Multiplies a complex by a scalar and put the result into another complex number.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="result">A <see cref="ComplexD"/> instance to hold the result.</param>
		public static void Multiply(ComplexD a, double s, ref ComplexD result)
		{
			result.Real = a.Real * s;
			result.Imaginary = a.Imaginary * s;
		}
		/// <summary>
		/// Multiplies a complex by a scalar and put the result into another complex number.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="result">A <see cref="ComplexD"/> instance to hold the result.</param>
		public static void Multiply(double s, ComplexD a, ref ComplexD result)
		{
			result.Real = a.Real * s;
			result.Imaginary = a.Imaginary * s;

		}
		/// <summary>
		/// Divides a complex by a complex.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the result.</returns>
		public static ComplexD Divide(ComplexD a, ComplexD b)
		{
			double	x = a.Real,	y = a.Imaginary;
			double	u = b.Real,	v = b.Imaginary;
			double	modulusSquared = u*u + v*v;

			if( modulusSquared == 0 ) 
			{
				throw new DivideByZeroException();
			}

			double invModulusSquared = 1 / modulusSquared;

			return new ComplexD(
				( x*u + y*v ) * invModulusSquared,
				( y*u - x*v ) * invModulusSquared );
		}
		/// <summary>
		/// Divides a complex by a scalar.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the result.</returns>
		public static ComplexD Divide(ComplexD a, double s)
		{
			if( s == 0 ) 
			{
				throw new DivideByZeroException();
			}
			return new ComplexD(
				a.Real / s,
				a.Imaginary / s );
		}
		/// <summary>
		/// Divides a scalar by a complex.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the result.</returns>
		public static ComplexD Divide(double s, ComplexD a)
		{
			if ((a.Real == 0) || (a.Imaginary == 0))
			{
				throw new DivideByZeroException();
			}
			return new ComplexD(
				s / a.Real,
				s / a.Imaginary);
		}
		/// <summary>
		/// Divides a complex by a complex and put the result in a third complex number.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <param name="result">A <see cref="ComplexD"/> instance to hold the result.</param>
		public static void Divide(ComplexD a, ComplexD b, ref ComplexD result)
		{
			double	x = a.Real,	y = a.Imaginary;
			double	u = b.Real,	v = b.Imaginary;
			double	modulusSquared = u*u + v*v;

			if( modulusSquared == 0 ) 
			{
				throw new DivideByZeroException();
			}

			double invModulusSquared = 1 / modulusSquared;

			result.Real = ( x*u + y*v ) * invModulusSquared;
			result.Imaginary = ( y*u - x*v ) * invModulusSquared;
		}		
		/// <summary>
		/// Divides a complex by a scalar and put the result into another complex number.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="result">A <see cref="ComplexD"/> instance to hold the result.</param>
		public static void Divide(ComplexD a, double s, ref ComplexD result)
		{
			if( s == 0 ) 
			{
				throw new DivideByZeroException();
			}
			
			result.Real = a.Real / s;
			result.Imaginary = a.Imaginary / s;
		}
		/// <summary>
		/// Divides a scalar by a complex and put the result into another complex number.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="result">A <see cref="ComplexD"/> instance to hold the result.</param>
		public static void Divide(double s, ComplexD a, ref ComplexD result)
		{
			if ((a.Real == 0) || (a.Imaginary == 0))
			{
				throw new DivideByZeroException();
			}

			result.Real = s / a.Real;
			result.Imaginary = s / a.Imaginary;
		}
		/// <summary>
		/// Negates a complex number.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the negated values.</returns>
		public static ComplexD Negate(ComplexD a)
		{
			return new ComplexD(-a.Real, -a.Imaginary);
		}
		/// <summary>
		/// Tests whether two complex numbers are approximately equal using default tolerance value.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <returns><see langword="true"/> if the two vectors are approximately equal; otherwise, <see langword="false"/>.</returns>
		public static bool ApproxEqual(ComplexD a, ComplexD b)
		{
			return ApproxEqual(a,b, MathFunctions.EpsilonD);
		}
		/// <summary>
		/// Tests whether two complex numbers are approximately equal given a tolerance value.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <param name="tolerance">The tolerance value used to test approximate equality.</param>
		/// <returns><see langword="true"/> if the two vectors are approximately equal; otherwise, <see langword="false"/>.</returns>
		public static bool ApproxEqual(ComplexD a, ComplexD b, double tolerance)
		{
			return
				(
				(System.Math.Abs(a.Real - b.Real) <= tolerance) &&
				(System.Math.Abs(a.Imaginary - b.Imaginary) <= tolerance)
				);
		}
		#endregion

		#region Public Static Complex Special Functions
		public static ComplexD Sqrt(ComplexD a)
		{
			ComplexD result = ComplexD.Zero;

			if ((a.Real == 0.0) && (a.Imaginary == 0.0))
			{
				return result;
			}
			else if (a.Imaginary == 0.0)
			{
				result.Real = (a.Real > 0) ? System.Math.Sqrt(a.Real) : System.Math.Sqrt(-a.Real);
				result.Imaginary = 0.0;
			}
			else
			{
				double modulus = a.GetModulus();

				result.Real		= System.Math.Sqrt(0.5 * (modulus + a.Real));
				result.Imaginary= System.Math.Sqrt(0.5 * (modulus - a.Real));
				if (a.Imaginary < 0.0)
					result.Imaginary = -result.Imaginary;
			}

			return result;
		}
		public static ComplexD Log(ComplexD a)
		{
			ComplexD result = ComplexD.Zero;

			if ((a.Real > 0.0) && (a.Imaginary == 0.0))
			{
				result.Real = System.Math.Log(a.Real);
				result.Imaginary = 0.0;
			}
			else if (a.Real == 0.0)
			{
				if (a.Imaginary > 0.0)
				{
					result.Real = System.Math.Log(a.Imaginary);
					result.Imaginary = MathFunctions.HalfPI;
				}
				else
				{
					result.Real = System.Math.Log(-(a.Imaginary));
					result.Imaginary = -MathFunctions.HalfPI;
				}
			}
			else
			{
				result.Real = System.Math.Log(a.GetModulus());
				result.Imaginary = System.Math.Atan2(a.Imaginary, a.Real);
			}

			return result;
		}
		public static ComplexD Exp(ComplexD a)
		{
			ComplexD result = ComplexD.Zero;
			double r = System.Math.Exp(a.Real);
			result.Real		= r * System.Math.Cos(a.Imaginary);
			result.Imaginary= r * System.Math.Sin(a.Imaginary);

			return result;
		}
		#endregion

		#region Public Static Complex Trigonometry
		public static ComplexD Sin(ComplexD a)
		{
			ComplexD result = ComplexD.Zero;

			if (a.Imaginary == 0.0)
			{
				result.Real = System.Math.Sin(a.Real);
				result.Imaginary = 0.0;
			}
			else
			{
				result.Real		 = System.Math.Sin(a.Real) * System.Math.Cosh(a.Imaginary);
				result.Imaginary = System.Math.Cos(a.Real) * System.Math.Sinh(a.Imaginary);
			}

			return result;
		}
		public static ComplexD Cos(ComplexD a)
		{
			ComplexD result = ComplexD.Zero;

			if (a.Imaginary == 0.0)
			{
				result.Real = System.Math.Cos(a.Real);
				result.Imaginary = 0.0;
			}
			else
			{
				result.Real		 = System.Math.Cos(a.Real) * System.Math.Cosh(a.Imaginary);
				result.Imaginary = -System.Math.Sin(a.Real) * System.Math.Sinh(a.Imaginary);
			}

			return result;
		}
		public static ComplexD Tan(ComplexD a)
		{
			ComplexD result = ComplexD.Zero;

			if (a.Imaginary == 0.0)
			{
				result.Real = System.Math.Tan(a.Real);
				result.Imaginary = 0.0;
			}
			else
			{
				double real2 = 2*a.Real;
				double imag2 = 2*a.Imaginary;
				double denom = System.Math.Cos(real2) + System.Math.Cosh(real2);

				result.Real		 = System.Math.Sin(real2) / denom;
				result.Imaginary = System.Math.Sinh(imag2) / denom;
			}

			return result;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Gets the modulus of the complex number.
		/// </summary>
		/// <returns>
		/// The modulus of the complex number:  sqrt(real*real + imaginary*imaginary).
		/// </returns>
		public double GetModulus()
		{
			return System.Math.Sqrt( _real*_real + _image*_image );
		}
		/// <summary>
		/// Gets the squared modulus of the complex number.
		/// </summary>
		/// <returns>
		/// The squared modulus of the complex number:  (real*real + imaginary*imaginary).
		/// </returns>
		public double GetModulusSquared()
		{
			return (_real*_real + _image*_image);
		}
		/// <summary>
		/// Gets the conjugate of the complex number.
		/// </summary>
		/// <returns>
		/// The conjugate of the complex number.
		/// </returns>
		public ComplexD GetConjugate()
		{
			return new ComplexD(_real, -_image);
		}
		/// <summary>
		/// Scale the complex number to modulus 1.
		/// </summary>
		public void Normalize()
		{
			double modulus = this.GetModulus();
			if(modulus == 0) 
			{
				throw new DivideByZeroException( "Can not normalize a complex number that is zero." );
			}
			_real	= (double)(_real / modulus);
			_image	= (double)(_image / modulus);
		}

		#endregion

		#region Overrides
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return _real.GetHashCode() ^ _image.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="ComplexD"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			if(obj is ComplexD) 
			{
				ComplexD c = (ComplexD)obj;
				return (this.Real == c.Real) && (this.Imaginary == c.Imaginary);
			}
			return false;
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			return string.Format("({0}, {1})", _real, _image);
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified complex numbers are equal.
		/// </summary>
		/// <param name="u">The left-hand complex number.</param>
		/// <param name="v">The right-hand complex number.</param>
		/// <returns><see langword="true"/> if the two complex numbers are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(ComplexD u, ComplexD v)
		{
			return ValueType.Equals(u,v);
		}
		/// <summary>
		/// Tests whether two specified complex numbers are not equal.
		/// </summary>
		/// <param name="u">The left-hand complex number.</param>
		/// <param name="v">The right-hand complex number.</param>
		/// <returns><see langword="true"/> if the two complex numbers are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(ComplexD u, ComplexD v)
		{
			return !ValueType.Equals(u,v);
		}
		#endregion

		#region Unary Operators
		/// <summary>
		/// Negates the complex number.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the negated values.</returns>
		public static ComplexD operator-(ComplexD a)
		{
			return ComplexD.Negate(a);
		}
		#endregion

		#region Binary Operators
		/// <summary>
		/// Adds two complex numbers.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the sum.</returns>
		public static ComplexD operator+(ComplexD a, ComplexD b)
		{
			return ComplexD.Add(a,b);
		}
		/// <summary>
		/// Adds a complex number and a scalar.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the sum.</returns>
		public static ComplexD operator+(ComplexD a, double s)
		{
			return ComplexD.Add(a,s);
		}
		/// <summary>
		/// Adds a complex number and a scalar.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the sum.</returns>
		public static ComplexD operator+(double s, ComplexD a)
		{
			return ComplexD.Add(a,s);
		}
		/// <summary>
		/// Subtracts a complex from a complex.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the difference.</returns>
		public static ComplexD operator-(ComplexD a, ComplexD b)
		{
			return ComplexD.Subtract(a,b);
		}
		/// <summary>
		/// Subtracts a scalar from a complex.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the difference.</returns>
		public static ComplexD operator-(ComplexD a, double s)
		{
			return ComplexD.Subtract(a, s);
		}
		/// <summary>
		/// Subtracts a complex from a scalar.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the difference.</returns>
		public static ComplexD operator-(double s, ComplexD a)
		{
			return ComplexD.Subtract(s, a);
		}

		/// <summary>
		/// Multiplies two complex numbers.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the result.</returns>
		public static ComplexD operator*(ComplexD a, ComplexD b)
		{
			return ComplexD.Multiply(a,b);
		}
		/// <summary>
		/// Multiplies a complex by a scalar.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the result.</returns>
		public static ComplexD operator*(double s, ComplexD a)
		{
			return ComplexD.Multiply(a,s);
		}
		/// <summary>
		/// Multiplies a complex by a scalar.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the result.</returns>
		public static ComplexD operator*(ComplexD a, double s)
		{
			return ComplexD.Multiply(a,s);
		}
		/// <summary>
		/// Divides a complex by a complex.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="b">A <see cref="ComplexD"/> instance.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the result.</returns>
		public static ComplexD operator/(ComplexD a, ComplexD b)
		{
			return ComplexD.Divide(a,b);
		}
		/// <summary>
		/// Divides a complex by a scalar.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the result.</returns>
		public static ComplexD operator/(ComplexD a, double s)
		{
			return ComplexD.Divide(a,s);
		}
		/// <summary>
		/// Divides a scalar by a complex.
		/// </summary>
		/// <param name="a">A <see cref="ComplexD"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="ComplexD"/> instance containing the result.</returns>
		public static ComplexD operator/(double s, ComplexD a)
		{
			return ComplexD.Divide(s,a);
		}
		#endregion

		#region Conversion Operators
		/// <summary>
		/// Converts from a single-precision real number to a complex number. 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static explicit operator ComplexD(float value)
		{
			return new ComplexD((double)value, 0);
		}
		/// <summary>
		/// Converts from a double-precision real number to a complex number. 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static explicit operator ComplexD(double value)
		{
			return new ComplexD(value, 0);
		}
		#endregion
	}

	#region ComplexDConverter class
	/// <summary>
	/// Converts a <see cref="ComplexD"/> to and from string representation.
	/// </summary>
	public class ComplexDConverter : ExpandableObjectConverter
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
			if ((destinationType == typeof(string)) && (value is ComplexD))
			{
				ComplexD c = (ComplexD)value;
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
				return ComplexD.Parse((string)value);
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
				new StandardValuesCollection(new object[2] {ComplexD.Zero, ComplexD.One} );

			return svc;
		}
	}
	#endregion

}
