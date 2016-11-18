using System.Collections.Generic;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri
{
    /// <summary>
    /// Provides utility methodes, used in the Jometri project
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Reduces a vertex of a face to "2D" by calculating the face normal and rotating the face until its normal lies on z axis
        /// </summary>
        /// <param name="src">Input vertex</param>
        public static float3 Reduce2D(this float3 src)
        {
            //calculate face normal - rotate face until normal lies on z axis - buffer the normal for this polygon (?)
            //retrun new value of src

            //Dummy
            return new float3(src.x, src.y, 0);
        }

        /// <summary>
        /// Reduces a vertex of a face to "2D" by calculating the face normal and rotating the face until its normal lies on z axis
        /// </summary>
        /// <param name="src">Input vertex</param>
        /// <param name="face">The face the vertex belongs to</param>
        public static float3 Reduce2D(this float3 src, FaceHandle face)
        {
            //calculate face normal - rotate face until normal lies on z axis - - buffer the normal for this polygon (?)
            //retrun new value of src

            //Dummy
            return new float3(src.x, src.y, 0);
        }

        /// <summary>
        /// Reduces the vertices of a face to "2D" by calculating the face normal and rotating the face until its normal lies on z axis
        /// </summary>
        /// <param name="src">Input vertex</param>
        /// <param name="face"></param>
        public static IEnumerable<float3> Reduce2D(this IEnumerable<float3> src, FaceHandle face)
        {
            //calculate face normal - rotate face until normal lies on z axis - retrun new value of src

            //Dummy
            foreach (var coord in src)
            {
                yield return new float3(coord.x, coord.y, 0);
            }
        }

        //For an explanation if this algorythm see: http://blog.element84.com/polygon-winding.html
        /// <summary>
        /// Checks whether a polygon, parallel to the xy plane, has a ccw winding.
        /// This methode does NOT support polygons parallel to the yz or xz plane!
        /// To guarantee a correct output make sure the polygon does not degenerate when the z coordinates are ignored.
        /// </summary>
        /// <param name="source">The polygon, represented as list of float3</param>
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
    }
}
