using System.Collections.Generic;
using Fusee.Math.Core;

namespace Fusee.Jometri
{
    /// <summary>
    /// Provides utility methodes used in the Jometri project.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Reduces a vertex of a face to "2D" by calculating the face normal and rotating the face until its normal lies on z axis.
        /// </summary>
        /// <param name="src">Input vertex.</param>
        public static float3 Reduce2D(this float3 src)
        {
            //calculate face normal - rotate face until normal lies on z axis - buffer the normal for this polygon (?)
            //retrun new value of src

            //Dummy
            return new float3(src.x, src.y, 0);
        }

        /// <summary>
        /// Reduces a vertex of a face to "2D" by calculating the face normal and rotating the face until its normal lies on z axis.
        /// </summary>
        /// <param name="src">Input vertex.</param>
        /// <param name="faceHandle">The face the vertex belongs to.</param>
        public static float3 Reduce2D(this float3 src, int faceHandle)
        {
            //calculate face normal - rotate face until normal lies on z axis - buffer the normal for this polygon (?)
            //retrun new value of src

            //Dummy
            return new float3(src.x, src.y, 0);
        }

        /// <summary>
        /// Reduces the vertices of a face to "2D" by calculating the face normal and rotating the face until its normal lies on z axis.
        /// </summary>
        /// <param name="src">Input vertex.</param>
        /// <param name="faceHandle"></param>
        public static IEnumerable<float3> Reduce2D(this IEnumerable<float3> src, int faceHandle)
        {
            //calculate face normal - rotate face until normal lies on z axis - retrun new value of src

            //Dummy
            foreach (var coord in src)
            {
                yield return new float3(coord.x, coord.y, 0);
            }
        }

        //For an explanation of this algorythm see: http://blog.element84.com/polygon-winding.html
        /// <summary>
        /// Checks whether a polygon, parallel to the xy plane, has a ccw winding.
        /// This methode does NOT support polygons parallel to the yz or xz plane!
        /// To guarantee a correct output make sure the polygon does not degenerate when the z coordinates are ignored.
        /// </summary>
        /// <param name="source">The polygon, represented as list of float3.</param>
        /// <returns></returns>
        public static bool IsCounterClockwise(this IList<float3> source)
        {
            var sum = 0f;

            for (var i = 0; i < source.Count; i++)
            {
                var current = source[i].Reduce2D(); //new float2(source[i].x, source[i].y);
                var next = source[(i + 1) % source.Count].Reduce2D(); //new float2(source[(i + 1) % source.Count].x, source[(i + 1) % source.Count].y);

                sum += (next.x - current.x) * (next.y + current.y);
            }
            return sum < 0;
        }

        //See: Antionio, Franklin - Faster line intersection (1992)
        //Points need to be reduced to 2D!
        //UNTESTED!!
        /// <summary>
        /// Checks if two lines intersect.
        /// </summary>
        /// <param name="p1">First control point of the first line</param>
        /// <param name="p2">Second control point of the first line</param>
        /// <param name="p3">First point of the second line</param>
        /// <param name="p4">Second point of the secornd line</param>
        /// <returns></returns>
        public static bool AreLinesIntersecting(float3 p1, float3 p2, float3 p3, float3 p4)
        {
            p1 = p1.Reduce2D();
            p2 = p2.Reduce2D();
            p3 = p3.Reduce2D();
            p4 = p4.Reduce2D();

            var a = p2 - p1;
            var b = p3 - p4;
            var c = p1 - p3;

            var tNumerator = b.y * b.x - b.x * c.y;
            var iNumerator = a.x * c.y - a.y * c.x;

            var denominator = a.y * b.x - a.x * b.y;

            if (denominator > 0)
            {
                if (tNumerator < 0 || tNumerator > denominator)
                    return false;
            }
            else
            {
                if (tNumerator > 0 || tNumerator < denominator)
                    return false;
            }

            if (denominator > 0)
            {
                if (iNumerator < 0 || iNumerator > denominator)
                    return false;
            }
            else
            {
                if (iNumerator > 0 || iNumerator < denominator)
                    return false;
            }

            return true;
        }
    }
}
