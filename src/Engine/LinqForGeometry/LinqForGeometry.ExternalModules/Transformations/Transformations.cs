/*
	Author: Dominik Steffen
	E-Mail: dominik.steffen@hs-furtwangen.de, dominik.steffen@gmail.com
	Bachelor Thesis Summer Semester 2013
	'Computer Science in Media'
	Project: LinqForGeometry
	Professors:
	Mr. Prof. C. Müller
	Mr. Prof. W. Walter
*/

using System;
using System.Linq;
using Fusee.Math;
using LinqForGeometry.Core;

namespace LFG.ExternalModules.Transformations
{

    /// <summary>
    /// This class provides some basic geometry transformations to apply on a LFG geometry object.
    /// For now, this is included as demos:
    /// - Translation.
    /// - Rotation on global axis.
    /// - Scaling of the object itself.
    /// 
    /// You are free to extend this list as you wish.
    /// </summary>
    public static class Transformations
    {

        /// <summary>
        /// This method can transform the object with a special matrix provided by the user.
        /// This method is applied using a basic LINQ/Lambda expression.
        /// </summary>
        /// <param name="transformationMatrix">float4x4 transformation Matrix. A matrix provided by the user to apply on the geometry.</param>
        /// <param name="geo">Geometry geo. The geometry object you want to do the translations on.</param>
        /// <returns>Boolean - true if the operation was succesful, false if not.</returns>
        public static bool Transform(float4x4 transformationMatrix, ref Geometry geo)
        {
            try
            {
                geo._LvertexVal = geo._LvertexVal.Select(tmpVertValue => transformationMatrix * tmpVertValue).ToList();

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// This method can scale the object bigger or smaller dependent on the input parameters.
        /// This method is applied using a basic LINQ/Lambda expression.
        /// </summary>
        /// <param name="scalarX">Scalar amount for x.</param>
        /// <param name="scalarY">Scalar amount for y.</param>
        /// <param name="scalarZ">Scalar amount for z.</param>
        /// <param name="geo">Geometry geo. The geometry object you want to do the translations on.</param>
        /// <returns>Boolean - true if the operation was succesful, false if not.</returns>
        public static bool Scale(float scalarX, float scalarY, float scalarZ, ref Geometry geo)
        {
            try
            {
                var transfMatrix = new float4x4(
                    new float4(scalarX, 0f, 0f, 0f),
                    new float4(0f, scalarY, 0f, 0f),
                    new float4(0f, 0f, scalarZ, 0f),
                    new float4(0f, 0f, 0f, 1.0f)
                    );

                geo._LvertexVal = geo._LvertexVal.Select(tmpVertValue => transfMatrix * tmpVertValue).ToList();

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// This method translates the model to another position.
        /// This method is applied using a basic LINQ/Lambda expression.
        /// </summary>
        /// <param name="tX">float factor</param>
        /// <param name="tY">float factor</param>
        /// <param name="tZ">float factor</param>
        /// <param name="geo">Geometry geo. The geometry object you want to do the translations on.</param>
        /// <returns>bool - true when operation succeeded</returns>
        public static bool Translate(float tX, float tY, float tZ, ref Geometry geo)
        {
            try
            {
                float4x4 transfMatrix = new float4x4(
                        new float4(1f, 0f, 0f, tX),
                        new float4(0f, 1f, 0f, tY),
                        new float4(0f, 0f, 1f, tZ),
                        new float4(0f, 0f, 0f, 1f)
                    );

                geo._LvertexVal = geo._LvertexVal.Select(tmpVertValue => transfMatrix * tmpVertValue).ToList();

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// Rotates the object on the X-Axis.
        /// This method is applied using a basic LINQ/Lambda expression.
        /// </summary>
        /// <param name="alpha">a float value representing an angle.</param>
        /// <param name="geo">Geometry geo. The geometry object you want to do the translations on.</param>
        /// <returns>bool - true if the operation succeeded.</returns>
        public static bool RotateX(float alpha, ref Geometry geo)
        {
            try
            {
                double cos = System.Math.Cos((double)alpha);
                double sin = System.Math.Sin((double)alpha);

                float4x4 transfMatrix = new float4x4(
                        new float4(1f, 0f, 0f, 0f),
                        new float4(0f, (float)cos, (float)sin, 0f),
                        new float4(0f, (float)-sin, (float)cos, 0f),
                        new float4(0f, 0f, 0f, 1f)
                    );

                geo._LvertexVal = geo._LvertexVal.Select(tmpVertValue => transfMatrix * tmpVertValue).ToList();

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// Rotates the object on the Y-Axis.
        /// This method is applied using a basic LINQ/Lambda expression.
        /// </summary>
        /// <param name="alpha">a float value representing an angle.</param>
        /// <param name="geo">Geometry geo. The geometry object you want to do the translations on.</param>
        /// <returns>bool - true if the operation succeeded.</returns>
        public static bool RotateY(float alpha, ref Geometry geo)
        {
            try
            {
                double cos = System.Math.Cos((double)alpha);
                double sin = System.Math.Sin((double)alpha);

                float4x4 transfMatrix = new float4x4(
                        new float4((float)cos, 0f, (float)-sin, 0f),
                        new float4(0f, 1f, 0f, 0f),
                        new float4((float)sin, 0f, (float)cos, 0f),
                        new float4(0f, 0f, 0f, 1f)
                    );

                geo._LvertexVal = geo._LvertexVal.Select(tmpVertValue => transfMatrix * tmpVertValue).ToList();

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// Rotates the object on the Z-Axis.
        /// This method is applied using a basic LINQ/Lambda expression.
        /// </summary>
        /// <param name="alpha">a float value representing an angle.</param>
        /// <param name="geo">Geometry geo. The geometry object you want to do the translations on.</param>
        /// <returns>bool - true if the operation succeeded.</returns>
        public static bool RotateZ(float alpha, ref Geometry geo)
        {
            try
            {
                double cos = System.Math.Cos((double)alpha);
                double sin = System.Math.Sin((double)alpha);

                float4x4 transfMatrix = new float4x4(
                        new float4((float)cos, (float)sin, 0f, 0f),
                        new float4((float)-sin, (float)cos, 0f, 0f),
                        new float4(0f, 0f, 1f, 0f),
                        new float4(0f, 0f, 0f, 1f)
                    );

                geo._LvertexVal = geo._LvertexVal.Select(tmpVertValue => transfMatrix * tmpVertValue).ToList();
                
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
