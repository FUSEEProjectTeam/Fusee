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
	/// Approximates integrals of functions over a given interal using the Simpson integration method.
	/// </summary>
	public sealed class SimpsonIntegral : IIntegrator
	{
		#region Private Fields
		private int _stepsNumber = 100;
		#endregion

		#region Constructors
		/// <summary>
		/// Initialize a new instance of the <see cref="SimpsonIntegral"/> class.
		/// </summary>
		public SimpsonIntegral()
		{
		}
		/// <summary>
		/// Initialize a new instance of the <see cref="SimpsonIntegral"/> class.
		/// </summary>
		/// <param name="stepsNumber">The number of steps to use for the integration.</param>
		public SimpsonIntegral(int stepsNumber)
		{
			_stepsNumber = stepsNumber;
		}
		/// <summary>
		/// Initialize a new instance of the <see cref="SimpsonIntegral"/> class using values from another <see cref="SimpsonIntegral"/> instance.
		/// </summary>
		/// <param name="integrator">A <see cref="SimpsonIntegral"/> instance.</param>
		public SimpsonIntegral(SimpsonIntegral integrator)
		{
			_stepsNumber = integrator._stepsNumber;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets a value indicating the number of steps used for the integration.
		/// </summary>
		public int StepsNumber
		{
			get { return _stepsNumber; }
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
			if (a > b)  return -Integrate(f, b, a);

			double sum = 0;
			double stepSize = (b-a)/_stepsNumber;
			double stepSizeDiv3 = stepSize/3;
			for (int i = 0; i < _stepsNumber; i = i+2)
			{
				sum += (f(a + i*stepSize) + 4*f(a+(i+1)*stepSize) + f(a+(i+2)*stepSize))*stepSizeDiv3;
			}

			return sum;
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
			if (a > b)  return -Integrate(f, b, a);

			float sum = 0;
			float stepSize = (float)((b-a)/_stepsNumber);
			float stepSizeDiv3 = stepSize/3.0f;
			for (int i = 0; i < _stepsNumber; i = i+2)
			{
				sum += (f(a + i*stepSize) + 4.0f*f(a+(i+1)*stepSize) + f(a+(i+2)*stepSize))*stepSizeDiv3;
			}

			return sum;
		}

		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="SimpsonIntegral"/> object.
		/// </summary>
		/// <returns>The <see cref="SimpsonIntegral"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new SimpsonIntegral(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="SimpsonIntegral"/> object.
		/// </summary>
		/// <returns>The <see cref="SimpsonIntegral"/> object this method creates.</returns>
		public SimpsonIntegral Clone()
		{
			return new SimpsonIntegral(this);
		}
		#endregion
	}
}
