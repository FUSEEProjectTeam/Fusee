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
#region C-program for MT19937 Copyright
//	A C-program for MT19937, with initialization improved 2002/1/26.
//	Coded by Takuji Nishimura and Makoto Matsumoto.
//
//	Before using, initialize the state by using init_genrand(seed)  
//	or init_by_array(init_key, key_length).
//
//	Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
//	All rights reserved.                          
//
//	Redistribution and use in source and binary forms, with or without
//	modification, are permitted provided that the following conditions
//	are met:
//
//	1. Redistributions of source code must retain the above copyright
//	notice, this list of conditions and the following disclaimer.
//
//	2. Redistributions in binary form must reproduce the above copyright
//	notice, this list of conditions and the following disclaimer in the
//	documentation and/or other materials provided with the distribution.
//
//	3. The names of its contributors may not be used to endorse or promote 
//	products derived from this software without specific prior written 
//	permission.
//
//	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//	"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//	LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//	A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//	CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//	EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//	PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//	PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//	LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//	NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//	SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//
//	Any feedback is very welcome.
//	http://www.math.keio.ac.jp/matumoto/emt.html
//	email: matumoto@math.keio.ac.jp
#endregion
using System;
using System.ComponentModel;
using System.Diagnostics;


using Fusee.Math.Core;

namespace Fusee.Math.Random
{
	/// <summary>
	/// Generates random numbers from a uniform distribution using the Mersenne Twister algorithm.
	/// </summary>
	public class RandGenMT : IFloatRandomNumberGenerator, IDoubleRandomNumberGenerator, IRandomArrayFiller
	{
		#region Mersenne Twister constants
		// Period parameters.
		private const int N = 624;
		private const int M = 397;
		private const uint MATRIX_A = 0x9908b0dfU;   // constant vector a
		private const uint UPPER_MASK = 0x80000000U; // most significant w-r bits
		private const uint LOWER_MASK = 0x7fffffffU; // least significant r bits
		private const int MAX_RAND_INT = 0x7fffffff;
		#endregion

		#region Private Fields
		// mag01[x] = x * MATRIX_A  for x=0,1
		private uint[] mag01 = {0x0U, MATRIX_A};

		// the array for the state vector
		private uint[] mt = new uint[N];

		// mti==N+1 means mt[N] is not initialized
		private int    mti = N+1;
		#endregion

		#region Public static properties
		/// <summary>
		/// Gets the maximum random integer value. All random integers generated
		/// by instances of this class are less than or equal to this value. This
		/// value is <c>0x7fffffff</c> (<c>2,147,483,647</c>).
		/// </summary>
		public static int MaxRandomInt
		{
			get
			{
				return MAX_RAND_INT;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="RandGenMT"/> class using the time of day in milliseconds as the seed.
		/// </summary>
		public RandGenMT()
		{
			init_genrand((uint)DateTime.Now.Millisecond);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RandGenMT"/> class using a given seed.
		/// </summary>
		/// <param name="seed">The seed.</param>
		public RandGenMT(int seed)
		{
			init_genrand((uint)seed);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RandGenMT"/> class using a given array.
		/// </summary>
		/// <param name="init">The array for initializing keys.</param>
		public RandGenMT(int[] init)
		{
			uint[] initArray = new uint[init.Length];
			for (int i = 0; i < init.Length; ++i)
				initArray[i] = (uint)init[i];

			init_by_array(initArray, (uint)initArray.Length);
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Returns a random integer greater than or equal to zero and
		/// less than or equal to <c>MaxRandomInt</c>. 
		/// </summary>
		/// <returns>The next random integer.</returns>
		public int Next()
		{
			return genrand_int31();
		}

		/// <summary>
		/// Returns a positive random integer less than the specified maximum.
		/// </summary>
		/// <param name="maxValue">The maximum value. Must be greater than zero.</param>
		/// <returns>A positive random integer less than or equal to <c>maxValue</c>.</returns>
		public int Next(int maxValue)
		{
			return Next(0, maxValue);
		}

		/// <summary>
		/// Returns a random integer within the specified range.
		/// </summary>
		/// <param name="minValue">The lower bound.</param>
		/// <param name="maxValue">The upper bound.</param>
		/// <returns>A random integer greater than or equal to <c>minValue</c>, and less than
		/// or equal to <c>maxValue</c>.</returns>
		public int Next(int minValue, int maxValue)
		{
			if (minValue > maxValue)
			{
				MathFunctions.Swap(ref minValue, ref maxValue);
			}

			return (int)(System.Math.Floor((maxValue-minValue+1)*genrand_real1() + minValue));
		}

		/// <summary>
		/// Returns a random unsigned integer greater than or equal to zero and
		/// less than or equal to <c>MaxRandomInt</c>. 
		/// </summary>
		/// <returns>The next random unsigned integer.</returns>
		[CLSCompliant(false)]
		public uint NextUInt()
		{
			return genrand_int32();
		}

		/// <summary>
		/// Returns a random number between, 0.0 and 1.0.
		/// </summary>
		/// <returns>A single-precision floating point number greater than or equal to, 0.0, 
		/// and less than 1.0.</returns>
		public float NextFloat()
		{
			return (float) genrand_real2();
		}

		/// <summary>
		/// Returns a random number greater than or equal to zero, and either strictly
		/// less than one, or less than or equal to one, depending on the value of the
		/// given boolean parameter.
		/// </summary>
		/// <param name="includeOne">
		/// If <c>true</c>, the random number returned will be 
		/// less than or equal to one; otherwise, the random number returned will
		/// be strictly less than one.
		/// </param>
		/// <returns>
		/// If <c>includeOne</c> is <c>true</c>, this method returns a
		/// single-precision random number greater than or equal to zero, and less
		/// than or equal to one. If <c>includeOne</c> is <c>false</c>, this method
		/// returns a single-precision random number greater than or equal to zero and
		/// strictly less than one.
		/// </returns>
		public float NextFloat(bool includeOne)
		{
			if (includeOne)
			{
				return (float) genrand_real1();
			}
			return (float) genrand_real2();
		}

		/// <summary>
		/// Returns a random number greater than, 0.0 and less than 1.0.
		/// </summary>
		/// <returns>A random number greater than, 0.0 and less than 1.0.</returns>
		public float NextFloatPositive()
		{
			return (float) genrand_real3();
		}
    

		/// <summary>
		/// Returns a random number between, 0.0 and 1.0.
		/// </summary>
		/// <returns>A double-precision floating point number greater than or equal to, 0.0, 
		/// and less than 1.0.</returns>
		public double NextDouble()
		{
			return genrand_real2();
		}

		/// <summary>
		/// Returns a random number greater than or equal to zero, and either strictly
		/// less than one, or less than or equal to one, depending on the value of the
		/// given boolean parameter.
		/// </summary>
		/// <param name="includeOne">
		/// If <c>true</c>, the random number returned will be 
		/// less than or equal to one; otherwise, the random number returned will
		/// be strictly less than one.
		/// </param>
		/// <returns>
		/// If <c>includeOne</c> is <c>true</c>, this method returns a
		/// single-precision random number greater than or equal to zero, and less
		/// than or equal to one. If <c>includeOne</c> is <c>false</c>, this method
		/// returns a single-precision random number greater than or equal to zero and
		/// strictly less than one.
		/// </returns>
		public double NextDouble(bool includeOne)
		{
			if (includeOne)
			{
				return genrand_real1();
			}
			return genrand_real2();
		}

		/// <summary>
		/// Returns a random number greater than, 0.0 and less than 1.0.
		/// </summary>
		/// <returns>A random number greater than, 0.0 and less than 1.0.</returns>
		public double NextDoublePositive()
		{
			return genrand_real3();
		}
    

		/// <summary>
		/// Generates a random number on <c>[0,1)</c> with 53-bit resolution.
		/// </summary>
		/// <returns>A random number on <c>[0,1)</c> with 53-bit resolution</returns>
		public double Next53BitRes()
		{
			return genrand_res53();
		}


		/// <summary>
		/// Reinitializes the random number generator using the time of day in
		/// milliseconds as the seed.
		/// </summary>
		public void Initialize()
		{
			init_genrand((uint)DateTime.Now.Millisecond);
		}

		/// <summary>
		/// Reinitializes the random number generator with the given seed.
		/// </summary>
		/// <param name="seed">The seed.</param>
		public void Initialize(int seed)
		{
			init_genrand((uint)seed);
		}

		/// <summary>
		/// Reinitializes the random number generator with the given array.
		/// </summary>
		/// <param name="init">The array for initializing keys.</param>
		public void Initialize(int[] init)
		{
			uint[] initArray = new uint[init.Length];
			for (int i = 0; i < init.Length; ++i)
				initArray[i] = (uint)init[i];

			init_by_array(initArray, (uint)initArray.Length);
		}

   
		#endregion Member Functions

		#region IRandomArrayFiller Members
		/// <summary>
		/// Fills the specified array of doubles with random numbers in the interval [0,1).
		/// </summary>
		/// <param name="array">An array of integers.</param>
		public void Fill(int[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Next();
			}
		}
		/// <summary>
		/// Fills the specified array of doubles with random numbers in the interval [0,1).
		/// </summary>
		/// <param name="array">An array of integers.</param>
		public void Fill(IntArrayList array)
		{
			for (int i = 0; i < array.Count; i++)
			{
				array[i] = Next();
			}
		}

		/// <summary>
		/// Fills the specified array of doubles with random numbers in the interval [0,1).
		/// </summary>
		/// <param name="array">An array of double-precision floating point numbers.</param>
		public void Fill(double[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = NextDouble();
			}
		}
		/// <summary>
		/// Fills the specified array of doubles with random numbers in the interval [0,1).
		/// </summary>
		/// <param name="array">An array of double-precision floating point numbers.</param>
		public void Fill(DoubleArrayList array)
		{
			for (int i = 0; i < array.Count; i++)
			{
				array[i] = NextDouble();
			}
		}

		/// <summary>
		/// Fills the specified array of doubles with random numbers in the interval [0,1).
		/// </summary>
		/// <param name="array">An array of single-precision floating point numbers.</param>
		public void Fill(float[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = NextFloat();
			}
		}
		/// <summary>
		/// Fills the specified array of doubles with random numbers in the interval [0,1).
		/// </summary>
		/// <param name="array">An array of single-precision floating point numbers.</param>
		public void Fill(FloatArrayList array)
		{
			for (int i = 0; i < array.Count; i++)
			{
				array[i] = NextFloat();
			}
		}

		#endregion

		#region Ported from C
		// initializes mt[N] with a seed
		private void init_genrand(uint s)
		{
			mt[0]= s & 0xffffffffU;
			for (mti=1; mti<N; mti++) 
			{
				mt[mti] = 
					(uint)(1812433253U * (mt[mti-1] ^ (mt[mti-1] >> 30)) + mti); 
				// See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. 
				// In the previous versions, MSBs of the seed affect   
				// only MSBs of the array mt[].                        
				// 2002/01/09 modified by Makoto Matsumoto             
				mt[mti] &= 0xffffffffU;
				// for >32 bit machines
			}
		}

		// initialize by an array with array-length
		// init_key is the array for initializing keys 
		// key_length is its length
		private void init_by_array(uint[] init_key, uint key_length)
		{
			int i, j, k;
			init_genrand(19650218U);
			i=1; j=0;
			k = (int)(N>key_length ? N : key_length);
			for (; k>0; k--) 
			{
				mt[i] = (uint)((uint)(mt[i] ^ ((mt[i-1] ^ (mt[i-1] >> 30)) * 1664525U)) + init_key[j] + j); /* non linear */
				mt[i] &= 0xffffffffU; // for WORDSIZE > 32 machines
				i++; j++;
				if (i>=N) { mt[0] = mt[N-1]; i=1; }
				if (j>=key_length) j=0;
			}
			for (k=N-1; k>0; k--) 
			{
				mt[i] = (uint)((uint)(mt[i] ^ ((mt[i-1] ^ (mt[i-1] >> 30)) * 1566083941U))- i); /* non linear */
				mt[i] &= 0xffffffffU; // for WORDSIZE > 32 machines
				i++;
				if (i>=N) { mt[0] = mt[N-1]; i=1; }
			}

			mt[0] = 0x80000000U; // MSB is 1; assuring non-zero initial array
		}

		// generates a random number on [0,0xffffffff]-interval
		private uint genrand_int32()
		{
			uint y;
			if (mti >= N) 
			{ /* generate N words at one time */
				int kk;

				if (mti == N+1)   /* if init_genrand() has not been called, */
					init_genrand(5489U); /* a default initial seed is used */

				for (kk=0;kk<N-M;kk++) 
				{
					y = (mt[kk]&UPPER_MASK)|(mt[kk+1]&LOWER_MASK);
					mt[kk] = mt[kk+M] ^ (y >> 1) ^ mag01[y & 0x1U];
				}
				for (;kk<N-1;kk++) 
				{
					y = (mt[kk]&UPPER_MASK)|(mt[kk+1]&LOWER_MASK);
					mt[kk] = mt[kk+(M-N)] ^ (y >> 1) ^ mag01[y & 0x1U];
				}
				y = (mt[N-1]&UPPER_MASK)|(mt[0]&LOWER_MASK);
				mt[N-1] = mt[M-1] ^ (y >> 1) ^ mag01[y & 0x1U];

				mti = 0;
			}
  
			y = mt[mti++];

			// Tempering
			y ^= (y >> 11);
			y ^= (y << 7) & 0x9d2c5680U;
			y ^= (y << 15) & 0xefc60000U;
			y ^= (y >> 18);

			return y;
		}

		// generates a random number on [0,0x7fffffff]-interval
		private int genrand_int31()
		{
			return (int)(genrand_int32()>>1);
		}
    
		// generates a random number on [0,1]-real-interval
		private double genrand_real1()
		{
			return genrand_int32()*(1.0/4294967295.0); 
			// divided by 2^32-1
		}

		// generates a random number on [0,1)-real-interval
		private double genrand_real2()
		{
			return genrand_int32()*(1.0/4294967296.0); 
			// divided by 2^32
		}

		// generates a random number on (0,1)-real-interval
		private double genrand_real3()
		{
			// divided by 2^32
			return (((double)genrand_int32()) + 0.5)*(1.0/4294967296.0); 
		}

		// generates a random number on [0,1) with 53-bit resolution
		private double genrand_res53() 
		{ 
			uint a=genrand_int32()>>5, b=genrand_int32()>>6; 
			return(a*67108864.0+b)*(1.0/9007199254740992.0); 
		} 
		#endregion
	}
}
