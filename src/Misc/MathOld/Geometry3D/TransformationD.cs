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

namespace Fusee.Math.Geometry3D
{
	/// <summary>
	/// Contains utility functions to create double-precision transformations.
	/// </summary>
	public sealed class TransformationD
	{
		#region Rotation transformation
		/// <summary>
		/// Builds a matrix that rotates around an arbitrary axis.
		/// </summary>
		/// <param name="rotationAxis">A <see cref="Vector3D"/> instance specifying the axis to rotate around (vector has to be of unit length).</param>
		/// <param name="angle">
		/// Angle of rotation, in radians. Angles are measured clockwise when looking along the rotation axis from the origin.
		/// </param>
		/// <returns>A <see cref="Matrix4D"/> representing the rotation.</returns>
		public static Matrix4D RotationAxis(Vector3D rotationAxis, double angle)
		{
			double s = System.Math.Sin(angle);
			double c = System.Math.Cos(angle);
			double cInv = 1 - c;
			double squareAxisX = rotationAxis.X * rotationAxis.X;
			double squareAxisY = rotationAxis.Y * rotationAxis.Y;
			double squareAxisZ = rotationAxis.Z * rotationAxis.Z;
			double XY = rotationAxis.X * rotationAxis.Y;
			double XZ = rotationAxis.X * rotationAxis.Z;
			double YZ = rotationAxis.Y * rotationAxis.Z;

			return new Matrix4D(
				c+squareAxisX*cInv,			XY*cInv-(rotationAxis.Z*s), XZ*cInv + (rotationAxis.Y*s),	0,
				XY*cInv+(rotationAxis.Z*s),	c + squareAxisY*cInv,		YZ*cInv - rotationAxis.X * s,	0,
				XZ*cInv-(rotationAxis.Y*s),	YZ*cInv+(rotationAxis.X*s), c + squareAxisZ*cInv,			0,
				0,0,0,1);

		}
		/// <summary>
		/// Builds a rotation matrix from a quaternion.
		/// </summary>
		/// <param name="q">A <see cref="QuaternionF"/> instance.</param>
		/// <returns>A <see cref="Matrix4D"/> representing the rotation.</returns>
		public static Matrix4D RotationQuaternion(QuaternionD q)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Builds a matrix with a specified yaw, pitch, and roll.
		/// </summary>
		/// <param name="yaw">Yaw around the y-axis, in radians.</param>
		/// <param name="pitch">Pitch around the x-axis, in radians.</param>
		/// <param name="roll">Roll around the z-axis, in radians.</param>
		/// <returns>A <see cref="Matrix4D"/> representing the rotation.</returns>
		/// <remarks>
		/// The order of transformations is first roll, then pitch, then yaw. 
		/// Relative to the object's local coordinate axis, this is equivalent to 
		/// rotation around the z-axis, followed by rotation around the x-axis, 
		/// followed by rotation around the y-axis. 
		/// </remarks>
		public static Matrix4D RotationYawPitchRoll(double yaw, double pitch, double roll)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}
		/// <summary>
		/// Builds a matrix that rotates around the x-axis.
		/// </summary>
		/// <param name="angle">Angle of rotation, in radians.</param>
		/// <returns>A <see cref="Matrix4D"/> representing the rotation.</returns>
		public static Matrix4D RotateX(double angle)
		{
			double sin = System.Math.Sin(angle);
			double cos = System.Math.Cos(angle);

			return new Matrix4D(
				1,	0,	0,	 0,
				0,	cos,-sin,0,
				0,	sin, cos,0,
				0,	0,	 0,	 1);
		}
		/// <summary>
		/// Builds a matrix that rotates around the y-axis.
		/// </summary>
		/// <param name="angle">Angle of rotation, in radians.</param>
		/// <returns>A <see cref="Matrix4D"/> representing the rotation.</returns>
		public static Matrix4D RotateY(double angle)
		{
			double sin = System.Math.Sin(angle);
			double cos = System.Math.Cos(angle);

			return new Matrix4D(
				cos,	0,	sin,	0,
				0,		1,  0,		0,
				-sin,	0,  cos,	0,
				0,		0,	0,		1);
		}
		/// <summary>
		/// Builds a matrix that rotates around the z-axis.
		/// </summary>
		/// <param name="angle">Angle of rotation, in radians.</param>
		/// <returns>A <see cref="Matrix4D"/> representing the rotation.</returns>
		public static Matrix4D RotateZ(double angle)
		{
			double sin = System.Math.Sin(angle);
			double cos = System.Math.Cos(angle);

			return new Matrix4D(
				cos,	-sin,	0,	0,
				sin,	cos,	0,	0,
				0,		0,		1,	0,
				0,		0,		0,	1);
		}

		#endregion

		#region Scaling transformation
		/// <summary>
		/// Builds a matrix that scales along the x-axis, y-axis, and z-axis.
		/// </summary>
		/// <param name="v">A <see cref="Vector3D"/> structure containing three values that represent the scaling factors applied along the x-axis, y-axis, and z-axis.</param>
		/// <returns>A <see cref="Matrix4D"/> representing the scaling transformation.</returns>
		public static Matrix4D Scaling(Vector3D v)
		{
			return new Matrix4D(
				v.X,	0,		0,		0,
				0,		v.Y,	0,		0,
				0,		0,		v.Z,	0,
				0,		0,		0,		1);
		}
		/// <summary>
		/// Builds a matrix that scales along the x-axis, y-axis, and z-axis.
		/// </summary>
		/// <param name="x">Scaling factor that is applied along the x-axis.</param>
		/// <param name="y">Scaling factor that is applied along the y-axis.</param>
		/// <param name="z">Scaling factor that is applied along the z-axis.</param>
		/// <returns>A <see cref="Matrix4D"/> representing the scaling transformation.</returns>
		public static Matrix4D Scaling(double x, double y, double z)
		{
			return new Matrix4D(
				x,	0,	0,	0,
				0,	y,	0,	0,
				0,	0,	z,	0,
				0,	0,	0,	1);
		}
		#endregion

		#region Translation transformation
		/// <summary>
		/// Builds a translation matrix.
		/// </summary>
		/// <param name="x">Offset on the x-axis.</param>
		/// <param name="y">Offset on the y-axis.</param>
		/// <param name="z">Offset on the z-axis.</param>
		/// <returns>A <see cref="Matrix4D"/> representing the translation transformation.</returns>
		public static Matrix4D Translation(double x, double y, double z)
		{
			return new Matrix4D(
				1, 0, 0, x,
				0, 1, 0, y,
				0, 0, 1, z,
				0, 0, 0, 1);
		}
		/// <summary>
		/// Builds a translation matrix.
		/// </summary>
		/// <param name="v">A <see cref="Vector3D"/> structure containing three values that represent the offsets on the x-axis, y-axis, and z-axis.</param>
		/// <returns>A <see cref="Matrix4D"/> representing the translation transformation.</returns>
		public static Matrix4D Translation(Vector3D v)
		{
			return new Matrix4D(
				1, 0, 0, v.X,
				0, 1, 0, v.Y,
				0, 0, 1, v.Z,
				0, 0, 0, 1);
		}
		#endregion
	
		/// <summary>
		/// Transform the vectors in the given array.
		/// </summary>
		/// <param name="vectors">An array of vectors to transform.</param>
		/// <param name="transformation">The transformation.</param>
		/// <remarks>
		/// This method changes the vector values in the <paramref name="vectors"/> array.
		/// </remarks>
		public static void TransformArray(Vector4DArrayList vectors, Matrix4D transformation)
		{
			for (int i = 0; i < vectors.Count; i++)
			{
				vectors[i] = transformation * vectors[i];
			}
		}
		/// <summary>
		/// Transform the vectors in the given array and put the result in another array.
		/// </summary>
		/// <param name="vectors">An array of vectors to transform.</param>
		/// <param name="transformation">The transformation.</param>
		/// <param name="result">An array of vectors to put the transformation results in (should be empty).</param>
		public static void TransformArray(Vector4DArrayList vectors, Matrix4D transformation, Vector4DArrayList result)
		{
			for (int i = 0; i < vectors.Count; i++)
			{
				result.Add(transformation * vectors[i]);
			}
		}


		/// <summary>
		/// Transform the vectors in the given array.
		/// </summary>
		/// <param name="vectors">An array of vectors to transform.</param>
		/// <param name="transformation">The transformation.</param>
		/// <remarks>
		/// This method changes the vector values in the <paramref name="vectors"/> array.
		/// </remarks>
		public static void TransformArray(Vector3DArrayList vectors, Matrix4D transformation)
		{
			for (int i = 0; i < vectors.Count; i++)
			{
				vectors[i] = Matrix4D.Transform(transformation, vectors[i]);
			}
		}
		/// <summary>
		/// Transform the vectors in the given array and put the result in another array.
		/// </summary>
		/// <param name="vectors">An array of vectors to transform.</param>
		/// <param name="transformation">The transformation.</param>
		/// <param name="result">An array of vectors to put the transformation results in (should be empty).</param>
		public static void TransformArray(Vector3DArrayList vectors, Matrix4D transformation, Vector3DArrayList result)
		{
			for (int i = 0; i < vectors.Count; i++)
			{
				result.Add(Matrix4D.Transform(transformation, vectors[i]));
			}
		}


		#region Private Constructor
		private TransformationD()
		{}
		#endregion
	}
}