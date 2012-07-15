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
using System.Diagnostics;

using Fusee.Math.Core;

namespace Fusee.Math.Geometry3D
{
	/// <summary>
	/// Provides various distance computation methods.
	/// </summary>
	public sealed class DistanceMethods
	{
		#region Point-Point
		/// <summary>
		/// Calculates the squared distance between two points.
		/// </summary>
		/// <param name="point1">A <see cref="Vector3F"/> instance.</param>
		/// <param name="point2">A <see cref="Vector3F"/> instance.</param>
		/// <returns>The squared distance between between two points.</returns>
		public static float SquaredDistance(Vector3F point1, Vector3F point2)
		{
			Vector3F delta = point2 - point1;
			return delta.GetLength();
		}
		/// <summary>
		/// Calculates the distance between two points.
		/// </summary>
		/// <param name="point1">A <see cref="Vector3F"/> instance.</param>
		/// <param name="point2">A <see cref="Vector3F"/> instance.</param>
		/// <returns>The distance between between two points.</returns>
		public static float Distance(Vector3F point1, Vector3F point2)
		{
			return (float)System.Math.Sqrt(SquaredDistance(point1, point2));
		}
		#endregion

		#region Point-OBB
		/// <summary>
		/// Calculates the squared distance between a point and a solid oriented box.
		/// </summary>
		/// <param name="point">A <see cref="Vector3F"/> instance.</param>
		/// <param name="obb">An <see cref="OrientedBox"/> instance.</param>
		/// <param name="closestPoint">The closest point in box coordinates.</param>
		/// <returns>The squared distance between a point and a solid oriented box.</returns>
		/// <remarks>
		/// Treating the oriented box as solid means that any point inside the box has
		/// distance zero from the box.
		/// </remarks>
		public static float SquaredDistancePointSolidOrientedBox(Vector3F point, OrientedBox obb, out Vector3F closestPoint)
		{
			Vector3F diff = point - obb.Center;
			Vector3F closest = new Vector3F(
				Vector3F.Dot(diff, obb.Axis1),
				Vector3F.Dot(diff, obb.Axis2),
				Vector3F.Dot(diff, obb.Axis3));

			float sqrDist = 0.0f;
			float delta	  = 0.0f;

			if (closest.X < -obb.Extent1)
			{
				delta = closest.X + obb.Extent1;
				sqrDist += delta*delta;
				closest.X = -obb.Extent1;
			}
			else if (closest.X > obb.Extent1)
			{
				delta = closest.X - obb.Extent1;
				sqrDist += delta*delta;
				closest.X = obb.Extent1;
			}

			if (closest.Y < -obb.Extent2)
			{
				delta = closest.Y + obb.Extent2;
				sqrDist += delta*delta;
				closest.Y = -obb.Extent2;
			}
			else if (closest.Y > obb.Extent2)
			{
				delta = closest.Y - obb.Extent2;
				sqrDist += delta*delta;
				closest.Y = obb.Extent2;
			}

			if (closest.Z < -obb.Extent3)
			{
				delta = closest.Z + obb.Extent3;
				sqrDist += delta*delta;
				closest.Z = -obb.Extent3;
			}
			else if (closest.Z > obb.Extent3)
			{
				delta = closest.Z - obb.Extent3;
				sqrDist += delta*delta;
				closest.Z = obb.Extent3;
			}

			closestPoint = closest;

			return sqrDist;
		}
		/// <summary>
		/// Calculates the squared distance between a point and a solid oriented box.
		/// </summary>
		/// <param name="point">A <see cref="Vector3F"/> instance.</param>
		/// <param name="obb">An <see cref="OrientedBox"/> instance.</param>
		/// <returns>The squared distance between a point and a solid oriented box.</returns>
		/// <remarks>
		/// Treating the oriented box as solid means that any point inside the box has
		/// distance zero from the box.
		/// </remarks>
		public static float SquaredDistance(Vector3F point, OrientedBox obb)
		{
			Vector3F temp;
			return SquaredDistancePointSolidOrientedBox(point, obb, out temp);
		}

		/// <summary>
		/// Calculates the distance between a point and a solid oriented box.
		/// </summary>
		/// <param name="point">A <see cref="Vector3F"/> instance.</param>
		/// <param name="obb">An <see cref="OrientedBox"/> instance.</param>
		/// <returns>The distance between a point and a solid oriented box.</returns>
		/// <remarks>
		/// Treating the oriented box as solid means that any point inside the box has
		/// distance zero from the box.
		/// </remarks>
		public static float Distance(Vector3F point, OrientedBox obb)
		{
			return (float)System.Math.Sqrt(SquaredDistance(point, obb));
		}
		#endregion

		#region Point-Plane
		/// <summary>
		/// Calculates the distance between a point and a plane.
		/// </summary>
		/// <param name="point">A <see cref="Vector3F"/> instance.</param>
		/// <param name="plane">A <see cref="Plane"/> instance.</param>
		/// <returns>The distance between a point and a plane.</returns>
		/// <remarks>
		/// <p>
		///  A positive return value means teh point is on the positive side of the plane.
		///  A negative return value means teh point is on the negative side of the plane.
		///  A zero return value means the point is on the plane.
		/// </p>
		/// <p>
		///  The absolute value of the return value is the true distance only when the plane normal is
		///  a unit length vector. 
		/// </p>
		/// </remarks>
		public static float Distance(Vector3F point, Plane plane)
		{
			return Vector3F.Dot(plane.Normal, point) + plane.Constant;
		}
		#endregion

		#region Private Constructor
		private DistanceMethods()
		{
		}
		#endregion
	}
}
