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

using Fusee.Math.Core;

namespace Fusee.Math.Analysis
{
	/// <summary>
	/// Represents a function that takes a single-precision floating point parameter.
	/// </summary>
	public sealed class FloatUnaryFunction
	{
		MathFunctions.FloatUnaryFunction _function;
		IDifferentiator _differentiator;
		IIntegrator _integrator;

		#region Static Properties
		private static IIntegrator _defaultIntegrator = new RombergIntegral();
		private static IDifferentiator _defaultDiff = null;

		/// <summary>
		/// The default integrator used by the <see cref="FloatUnaryFunction"/> instances.
		/// </summary>
		public static IIntegrator DefaultIntegrator
		{
			get { return _defaultIntegrator; }
			set 
			{ 
				if (value == null)
					throw new ArgumentException("DefaultIntegrator cannot be null.", "value");

				_defaultIntegrator = value;
			}
		}
		/// <summary>
		/// /// The default differentiator used by the <see cref="FloatUnaryFunction"/> instances.
		/// </summary>
		public static IDifferentiator DefaultDifferentiator
		{
			get { return _defaultDiff; }
			set
			{
				if (value == null)
					throw new ArgumentException("DefaultDifferentiator cannot be null.", "value");

				_defaultDiff = value;
			}
		}
			
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="FloatUnaryFunction"/> class.
		/// </summary>
		/// <param name="f">
		/// A function delegate that takes a float value as a parameter and returns a float value.
		/// </param>
		public FloatUnaryFunction(MathFunctions.FloatUnaryFunction f)
		{
			_function = f;
			_differentiator = null;
			_integrator = null;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="FloatUnaryFunction"/> class.
		/// </summary>
		/// <param name="f">A function delegate that takes a float value as a parameter and returns a float value.</param>
		/// <param name="d">The <see cref="IDifferentiator"/> to use.</param>
		/// <param name="i">The <see cref="IIntegrator"/> to use.</param>
		public FloatUnaryFunction(MathFunctions.FloatUnaryFunction f, IDifferentiator d, IIntegrator i)
		{
			_function = f;
			_differentiator = d;
			_integrator = i;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets the function encapsulated by this object.
		/// </summary>
		public MathFunctions.FloatUnaryFunction Function
		{
			get { return _function; }
		}
		/// <summary>
		/// Gets or sets the differentiator associated with this object.
		/// </summary>
		public IDifferentiator Differentiator
		{
			get { return _differentiator; }
			set { _differentiator = value;}
		}
		/// <summary>
		/// Gets or sets the integrator associated with this object.
		/// </summary>
		public IIntegrator Integrator
		{
			get { return _integrator; }
			set { _integrator = value;}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Computes the integral of the current function over the specified interval. 
		/// </summary>
		/// <param name="a">Lower integration limit.</param>
		/// <param name="b">Upper integration limit.</param>
		/// <returns>The integral value over the specified interval.</returns>
		/// <remarks>
		/// If an integrator was not explicitly set, the method uses the default integrator.
		/// </remarks>
		public float Integrate(float a, float b)
		{
			if (_integrator == null)
				return DefaultIntegrator.Integrate(_function, a, b);
			else
                return _integrator.Integrate(_function,a,b);
		}
		#endregion
	}
}
