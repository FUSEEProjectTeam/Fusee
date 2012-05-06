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

namespace Fusee.Math.Random
{
	/// <summary>
	/// Contains various utility and helper methods.
	/// </summary>
	public sealed class RandomUtils
	{
		public static ComplexDArrayList CreateRandomComplexDArray(int count, IDoubleRandomNumberGenerator r)
		{
			ComplexDArrayList result = new ComplexDArrayList(count);
			for(int i = 0; i < count; i++)
			{
				result.Add(new ComplexD(r.NextDouble(),r.NextDouble()));
			}

			return result;
		}
		public static QuaternionDArrayList CreateRandomQuaternionDArray(int count, IDoubleRandomNumberGenerator r)
		{
			QuaternionDArrayList result  = new QuaternionDArrayList(count);
			for(int i = 0; i < count; i++)
			{
				result.Add(new QuaternionD(r.NextDouble(),r.NextDouble(), r.NextDouble(),r.NextDouble()));
			}
			return result;
		}
		public static Vector2DArrayList CreateRandomVector2DArray(int count, IDoubleRandomNumberGenerator r)
		{
			Vector2DArrayList result  = new Vector2DArrayList(count);
			for(int i = 0; i < count; i++)
			{
				result.Add(new Vector2D(r.NextDouble(),r.NextDouble()));
			}
			return result;
		}
		public static Vector3DArrayList CreateRandomVector3DArray(int count, IDoubleRandomNumberGenerator r)
		{
			Vector3DArrayList result  = new Vector3DArrayList(count);
			for(int i = 0; i < count; i++)
			{
				result.Add(new Vector3D(r.NextDouble(),r.NextDouble(), r.NextDouble()));
			}
			return result;
		}
		public static Vector4DArrayList CreateRandomVector4DArray(int count, IDoubleRandomNumberGenerator r)
		{
			Vector4DArrayList result  = new Vector4DArrayList(count);
			for(int i = 0; i < count; i++)
			{
				result.Add(new Vector4D(r.NextDouble(),r.NextDouble(), r.NextDouble(),r.NextDouble()));
			}
			return result;
		}


		public static ComplexFArrayList CreateRandomComplexFArray(int count, IFloatRandomNumberGenerator r)
		{
			ComplexFArrayList result = new ComplexFArrayList(count);
			for(int i = 0; i < count; i++)
			{
				result.Add(new ComplexF(r.NextFloat(),r.NextFloat()));
			}
			return result;
		}

		public static QuaternionFArrayList CreateRandomQuaternionFArray(int count, IFloatRandomNumberGenerator r)
		{
			QuaternionFArrayList result  = new QuaternionFArrayList(count);
			for(int i = 0; i < count; i++)
			{
				result.Add(new QuaternionF(r.NextFloat(),r.NextFloat(), r.NextFloat(),r.NextFloat()));
			}
			return result;
		}
		public static Vector2FArrayList CreateRandomVector2FArray(int count, IFloatRandomNumberGenerator r)
		{
			Vector2FArrayList result  = new Vector2FArrayList(count);
			for(int i = 0; i < count; i++)
			{
				result.Add(new Vector2F(r.NextFloat(),r.NextFloat()));
			}
			return result;
		}
		public static Vector3FArrayList CreateRandomVector3FArray(int count, IFloatRandomNumberGenerator r)
		{
			Vector3FArrayList result  = new Vector3FArrayList(count);
			for(int i = 0; i < count; i++)
			{
				result.Add(new Vector3F(r.NextFloat(),r.NextFloat(), r.NextFloat()));
			}
			return result;
		}
		public static Vector4FArrayList CreateRandomVector4FArray(int count, IFloatRandomNumberGenerator r)
		{
			Vector4FArrayList result  = new Vector4FArrayList(count);
			for(int i = 0; i < count; i++)
			{
				result.Add(new Vector4F(r.NextFloat(),r.NextFloat(), r.NextFloat(),r.NextFloat()));
			}
			return result;
		}


		#region Private Constructor
		private  RandomUtils()
		{
		}
		#endregion
	}
}
