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
	/// Contains utility functions to create single-precision transformations.
	/// </summary>
	public sealed class TransformationF
	{
		/// <summary>
		/// Builds a 3-D affine transformation matrix.
		/// </summary>
		/// <param name="scaling">The scaling vactor. Use zero to specify no scaling.</param>
		/// <param name="rotationCenter">A <see cref="Vector3F"/> instance specifying the center of the rotation.</param>
		/// <param name="rotation">A <see cref="QuaternionF"/> instance specifying the rotation. use <see cref="QuaternionF.Identity"/> to specify no rotation.</param>
		/// <param name="translation">A <see cref="Vector3F"/> specifying the translation. Use <see cref="Vector3F.Zero"/> to specify no translation.</param>
		/// <returns>A <see cref="Matrix4F"/> representing the affine transformation.</returns>
		/// <remarks>
		/// <p>
		/// Calculates the transoformation matrix using the following formula:
		/// Mout = Ms * (Mrc)^-1 * Mr * Mrc * Mt
		/// </p>
		/// <p>
		/// where:
		/// <list type="bullet">
		///		<item>
		///			<term>Ms</term>
		///			<description>Scaling transformation.</description>
		///		</item>
		///		<item>
		///			<term>Mrc</term>
		///			<description>Rotation center transformation.</description>
		///		</item>
		///		<item>
		///			<term>Mr</term>
		///			<description>Rotation transformation.</description>
		///		</item>
		///		<item>
		///			<term>Mt</term>
		///			<description>Translation transformation.</description>
		///		</item>
		/// </list>
		/// </p>
		/// </remarks>
		public static Matrix4F AffineTransformation(float scaling, Vector3F rotationCenter, QuaternionF rotation, Vector3F translation)
		{
			Matrix4F Ms = TransformationF.Scaling(scaling, scaling, scaling);
			Matrix4F Mt = TransformationF.Translation(translation.X, translation.Y, translation.Z);


			// TODO: Implement this.
			throw new NotImplementedException();
		}


		#region Rotation transformation
		/// <summary>
		/// Builds a matrix that rotates around an arbitrary axis.
		/// </summary>
		/// <param name="rotationAxis">A <see cref="Vector3F"/> instance specifying the axis to rotate around (vector has to be of unit length).</param>
		/// <param name="angle">
		/// Angle of rotation, in radians. Angles are measured clockwise when looking along the rotation axis from the origin.
		/// </param>
		/// <returns>A <see cref="Matrix4F"/> representing the rotation.</returns>
		public static Matrix4F RotationAxis(Vector3F rotationAxis, float angle)
		{
			float s = (float)System.Math.Sin(angle);
			float c = (float)System.Math.Cos(angle);
			float cInv = 1.0f - c;
			float squareAxisX = rotationAxis.X * rotationAxis.X;
			float squareAxisY = rotationAxis.Y * rotationAxis.Y;
			float squareAxisZ = rotationAxis.Z * rotationAxis.Z;
			float XY = rotationAxis.X * rotationAxis.Y;
			float XZ = rotationAxis.X * rotationAxis.Z;
			float YZ = rotationAxis.Y * rotationAxis.Z;

			return new Matrix4F(
				c+squareAxisX*cInv,			XY*cInv-(rotationAxis.Z*s), XZ*cInv + (rotationAxis.Y*s),	0.0f,
				XY*cInv+(rotationAxis.Z*s),	c + squareAxisY*cInv,		YZ*cInv - rotationAxis.X * s,	0.0f,
				XZ*cInv-(rotationAxis.Y*s),	YZ*cInv+(rotationAxis.X*s), c + squareAxisZ*cInv,			0.0f,
				0.0f,0.0f,0.0f,1.0f);

		}
		/// <summary>
		/// Builds a rotation matrix from a quaternion.
		/// </summary>
		/// <param name="q">A <see cref="QuaternionF"/> instance.</param>
		/// <returns>A <see cref="Matrix4F"/> representing the rotation.</returns>
		public static Matrix4F RotationQuaternion(QuaternionF q)
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
		/// <returns>A <see cref="Matrix4F"/> representing the rotation.</returns>
		/// <remarks>
		/// The order of transformations is first roll, then pitch, then yaw. 
		/// Relative to the object's local coordinate axis, this is equivalent to 
		/// rotation around the z-axis, followed by rotation around the x-axis, 
		/// followed by rotation around the y-axis. 
		/// </remarks>
		public static Matrix4F RotationYawPitchRoll(float yaw, float pitch, float roll)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}
		/// <summary>
		/// Builds a matrix that rotates around the x-axis.
		/// </summary>
		/// <param name="angle">Angle of rotation, in radians.</param>
		/// <returns>A <see cref="Matrix4F"/> representing the rotation.</returns>
		public static Matrix4F RotateX(float angle)
		{
			float sin = (float)System.Math.Sin(angle);
			float cos = (float)System.Math.Cos(angle);

			return new Matrix4F(
				1.0f,	0.0f,	0.0f,	0.0f,
				0.0f,	cos,	-sin,	0.0f,
				0.0f,	sin,	cos,	0.0f,
				0.0f,	0.0f,	0.0f,	1.0f);
		}
		/// <summary>
		/// Builds a matrix that rotates around the y-axis.
		/// </summary>
		/// <param name="angle">Angle of rotation, in radians.</param>
		/// <returns>A <see cref="Matrix4F"/> representing the rotation.</returns>
		public static Matrix4F RotateY(float angle)
		{
			float sin = (float)System.Math.Sin(angle);
			float cos = (float)System.Math.Cos(angle);

			return new Matrix4F(
				cos,	0.0f,	sin,	0.0f,
				0.0f,	1.0f,	0.0f,	0.0f,
				-sin,	0.0f,	cos,	0.0f,
				0.0f,	0.0f,	0.0f,	1.0f);
		}
		/// <summary>
		/// Builds a matrix that rotates around the z-axis.
		/// </summary>
		/// <param name="angle">Angle of rotation, in radians.</param>
		/// <returns>A <see cref="Matrix4F"/> representing the rotation.</returns>
		public static Matrix4F RotateZ(float angle)
		{
			float sin = (float)System.Math.Sin(angle);
			float cos = (float)System.Math.Cos(angle);

			return new Matrix4F(
				cos,	-sin,	0.0f,	0.0f,
				sin,	cos,	0.0f,	0.0f,
				0.0f,	0.0f,	1.0f,	0.0f,
				0.0f,	0.0f,	0.0f,	1.0f);
		}

		#endregion

		#region Scaling transformation
		/// <summary>
		/// Builds a matrix that scales along the x-axis, y-axis, and z-axis.
		/// </summary>
		/// <param name="v">A <see cref="Vector3F"/> structure containing three values that represent the scaling factors applied along the x-axis, y-axis, and z-axis.</param>
		/// <returns>A <see cref="Matrix4F"/> representing the scaling transformation.</returns>
		public static Matrix4F Scaling(Vector3F v)
		{
			return new Matrix4F(
				v.X,	0.0f,   0.0f,   0.0f,
				0.0f,	v.Y,	0.0f,	0.0f,
				0.0f,   0.0f,	v.Z,	0.0f,
				0.0f,   0.0f,   0.0f,   1.0f);
		}
		/// <summary>
		/// Builds a matrix that scales along the x-axis, y-axis, and z-axis.
		/// </summary>
		/// <param name="x">Scaling factor that is applied along the x-axis.</param>
		/// <param name="y">Scaling factor that is applied along the y-axis.</param>
		/// <param name="z">Scaling factor that is applied along the z-axis.</param>
		/// <returns>A <see cref="Matrix4F"/> representing the scaling transformation.</returns>
		public static Matrix4F Scaling(float x, float y, float z)
		{
			return new Matrix4F(
				x,		0.0f,	0.0f,	0.0f,
				0.0f,	y,		0.0f,	0.0f,
				0.0f,	0.0f,	z,		0.0f,
				0.0f,	0.0f,	0.0f,	1.0f);
		}
		#endregion

		#region Translation transformation
		/// <summary>
		/// Builds a translation matrix.
		/// </summary>
		/// <param name="x">Offset on the x-axis.</param>
		/// <param name="y">Offset on the y-axis.</param>
		/// <param name="z">Offset on the z-axis.</param>
		/// <returns>A <see cref="Matrix4F"/> representing the translation transformation.</returns>
		public static Matrix4F Translation(float x, float y, float z)
		{
			return new Matrix4F(
				1.0f,	0.0f,   0.0f,   x,
				0.0f,	1.0f,	0.0f,	y,
				0.0f,   0.0f,	1.0f,	z,
				0.0f,   0.0f,   0.0f,   1.0f);
		}
		/// <summary>
		/// Builds a translation matrix.
		/// </summary>
		/// <param name="v">A <see cref="Vector3F"/> structure containing three values that represent the offsets on the x-axis, y-axis, and z-axis.</param>
		/// <returns>A <see cref="Matrix4F"/> representing the translation transformation.</returns>
		public static Matrix4F Translation(Vector3F v)
		{
			return new Matrix4F(
				1.0f,	0.0f,   0.0f,   v.X,
				0.0f,	1.0f,	0.0f,	v.Y,
				0.0f,   0.0f,	1.0f,	v.Z,
				0.0f,   0.0f,   0.0f,   1.0f);
		}
		#endregion

		/// <summary>
		/// Builds a matrix that reflects the coordinate system about a plane.
		/// </summary>
		/// <param name="p">A <see cref="Plane"/> instance.</param>
		/// <returns>A <see cref="Matrix4F"/> representing the reflection transformation.</returns>
		/// <remarks>
		/// This method normalizes the plane's equation before creating the reflection matrix.
		/// </remarks>
		public static Matrix4F Reflect(Plane p)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Builds a matrix that flattens geometry into a plane.
		/// </summary>
		/// <param name="lightPosition">A <see cref="Vector4F"/> instance representing the light source position.</param>
		/// <param name="p">A <see cref="Plane"/> instance.</param>
		/// <returns>A <see cref="Matrix4F"/> representing the shadow transformation.</returns>
		/// <remarks>
		/// This method flattens geometry into a plane, as if it were casting a shadow from a light.
		/// </remarks>
		public static Matrix4F Shadow(Vector4F lightPosition, Plane p)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}


		/// <summary>
		/// Transform the vectors in the given array.
		/// </summary>
		/// <param name="vectors">An array of vectors to transform.</param>
		/// <param name="transformation">The transformation.</param>
		/// <remarks>
		/// This method changes the vector values in the <paramref name="vectors"/> array.
		/// </remarks>
		public static void TransformArray(Vector4FArrayList vectors, Matrix4F transformation)
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
		public static void TransformArray(Vector4FArrayList vectors, Matrix4F transformation, Vector4FArrayList result)
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
		public static void TransformArray(Vector3FArrayList vectors, Matrix4F transformation)
		{
			for (int i = 0; i < vectors.Count; i++)
			{
				vectors[i] = Matrix4F.Transform(transformation, vectors[i]);
			}
		}
		/// <summary>
		/// Transform the vectors in the given array and put the result in another array.
		/// </summary>
		/// <param name="vectors">An array of vectors to transform.</param>
		/// <param name="transformation">The transformation.</param>
		/// <param name="result">An array of vectors to put the transformation results in (should be empty).</param>
		public static void TransformArray(Vector3FArrayList vectors, Matrix4F transformation, Vector3FArrayList result)
		{
			for (int i = 0; i < vectors.Count; i++)
			{
				result.Add(Matrix4F.Transform(transformation, vectors[i]));
			}
		}

		#region Private Constructor
		private TransformationF()
		{}
		#endregion
	}

}
