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
	/// Approximates integrals of functions over a given interal using the Romberg integration method.
	/// </summary>
	public class RombergIntegral : IIntegrator
	{
		#region Private Fields
		private int _order = 5;
		private double[,] _romD = null;
		private float[,] _romF = null;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="RombergIntegral"/> class using default order value.
		/// </summary>
		/// <remarks>
		/// The new instance is created with the default order value 5.
		/// </remarks>
		public RombergIntegral()
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="RombergIntegral"/> class.
		/// </summary>
		/// <param name="order">The order value to use.</param>
		public RombergIntegral(int order)
		{
			_order = order;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="RombergIntegral"/> class using the order value from another instance.
		/// </summary>
		/// <param name="r">A <see cref="RombergIntegral"/> instance.</param>
		public RombergIntegral(RombergIntegral r)
		{
			_order = r.Order;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the order of the Romberg approximation.
		/// </summary>
		public int Order
		{
			get { return _order;  }
			set 
			{ 
				if (_order != value)
				{
					_order = value;
					_romF = null;
					_romD = null;
				}
			}
		}
		#endregion
	
		#region IIntegrator Members
		/// <summary>
		/// Integrates a given function within the given integral.
		/// </summary>
		/// <param name="f">The function to integrate.</param>
		/// <param name="a">The lower limit.</param>
		/// <param name="b">The higher limit.</param>
		/// <returns>
		/// The integral of <paramref name="function"/> over the interval from <paramref name="a"/> to <paramref name="b"/>
		/// </returns>
		public double Integrate(MathFunctions.DoubleUnaryFunction f, double a, double b)
		{
			// Check the _romD field is initialized correctly.
			if ((_romD == null) || (_romD.GetLength(1) == _order))
			{
				_romD = new double[1, _order];
			}

			if (a > b) return Integrate(f, b, a);

			double h = (b-a);

			_romD[0,0] = 0.5*h*(f(a) + f(b));
			for (int i = 2, ipower = 1; i <= _order; i++, ipower *= 2, h /= 2)
			{
				// Approximation using the trapezoid rule.
				double sum = 0;
				for (int j = 1; j <= ipower; j++)
				{
					sum += f(a + h*(j-0.5));
				}

				// Richardson extrapolation
				_romD[1,0] = 0.5 * (_romD[0,0] + (h*sum));
				for (int k = 1, kpower = 4; k < i; k++, kpower *= 4)
				{
					_romD[1,k] = (kpower*_romD[1, k-1] - _romD[0,k-1]) / (kpower-1);
				}

				// Save the extrapolated values for the next iteration
				for (int j = 0; j < i; j++)
				{
					_romD[0, j] = _romD[1, j];
				}
			}

			return _romD[0,_order-1];
		}

		/// <summary>
		/// Integrates a given function within the given integral.
		/// </summary>
		/// <param name="f">The function to integrate.</param>
		/// <param name="a">The lower limit.</param>
		/// <param name="b">The higher limit.</param>
		/// <returns>
		/// The integral of <paramref name="function"/> over the interval from <paramref name="a"/> to <paramref name="b"/>
		/// </returns>
		public float Integrate(MathFunctions.FloatUnaryFunction f, float a, float b)
		{
			// Check the _romD field is initialized correctly.
			if ((_romF == null) || (_romF.GetLength(1) == _order))
			{
				_romF = new float[1, _order];
			}

			if (a > b) return Integrate(f, b, a);

			float h = (b-a);

			_romF[0,0] = 0.5f*h*(f(a) + f(b));
			for (int i = 2, ipower = 1; i <= _order; i++, ipower *= 2, h /= 2)
			{
				// Approximation using the trapezoid rule.
				float sum = 0;
				for (int j = 1; j <= ipower; j++)
				{
					sum += f(a + h*(j-0.5f));
				}

				// Richardson extrapolation
				_romF[1,0] = 0.5f * (_romF[0,0] + (h*sum));
				for (int k = 1, kpower = 4; i < i; k++, kpower *= 4)
				{
					_romF[1,k] = (kpower*_romF[1, k-1] - _romF[0,k-1]) / (kpower-1);
				}

				// Save the extrapolated values for the next iteration
				for (int j = 0; j < i; j++)
				{
					_romF[0, j] = _romF[1, j];
				}
			}

			return _romF[0,_order-1];
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="RombergIntegral"/> object.
		/// </summary>
		/// <returns>The <see cref="RombergIntegral"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new RombergIntegral(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="RombergIntegral"/> object.
		/// </summary>
		/// <returns>The <see cref="RombergIntegral"/> object this method creates.</returns>
		public RombergIntegral Clone()
		{
			return new RombergIntegral(this);
		}
		#endregion
	}
}
