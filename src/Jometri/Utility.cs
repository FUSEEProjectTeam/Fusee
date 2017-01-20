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

        public static float3 Reduce2D(this float3 vert, float3 normal)
        {
            //Set of orthonormal vectors
            normal.Normalize(); //new z axis
            var v2 = float3.Cross(normal, float3.UnitZ); //rotation axis - new x axis
            var v3 = float3.Cross(normal, v2); //new y axis

            //Calculate change-of-basis matrix (orthonormal matrix).
            var row1 = new float3(v2.x, v3.x, normal.x);
            var row2 = new float3(v2.y, v3.y, normal.y);
            var row3 = new float3(v2.z, v3.z, normal.z);

            var changeOfBasisMat = new float3x3(row1, row2, row3);

            //In an orthonomal matrix the inverse equals the transpose, therefor the transpose can be used to calculate vector in new basis.
            var transposeMat = new float3x3(changeOfBasisMat.Row0, changeOfBasisMat.Row1, changeOfBasisMat.Row2);
            transposeMat.Transpose();

            var vec = transposeMat * vert;
            var testvec = transposeMat * normal;

            //Round to get rid of potential exponent representation
            var vecX = System.Math.Round((decimal)vec.x, 5);
            var vecY = System.Math.Round((decimal)vec.y, 5);
            var vecZ = System.Math.Round((decimal)vec.z, 5);

            vec = new float3((float)vecX, (float)vecY, (float)vecZ);

            return vec;
        }

        public static float3 Reduce2D(this float3 vert, float3 normal, out float3x3 changeOfBasisMat)
        {
            //Set of orthonormal vectors
            normal.Normalize(); //new z axis
            var v2 = float3.Cross(normal, float3.UnitZ); //rotation axis - new x axis
            var v3 = float3.Cross(normal, v2); //new y axis

            //Calculate change-of-basis matrix (orthonormal matrix).
            var row1 = new float3(v2.x, v3.x, normal.x);
            var row2 = new float3(v2.y, v3.y, normal.y);
            var row3 = new float3(v2.z, v3.z, normal.z);

            changeOfBasisMat = new float3x3(row1, row2, row3);

            //In an orthonomal matrix the inverse equals the transpose, therefor the transpose can be used to calculate vector in new basis.
            var transposeMat = new float3x3(changeOfBasisMat.Row0, changeOfBasisMat.Row1, changeOfBasisMat.Row2);
            transposeMat.Transpose();

            var vec = transposeMat * vert;
            var testvec = transposeMat*normal;

            //Round to get rid of potential exponent representation
            var vecX = System.Math.Round((decimal)vec.x, 5);
            var vecY = System.Math.Round((decimal)vec.y, 5);
            var vecZ = System.Math.Round((decimal)vec.z, 5);

            vec = new float3((float)vecX, (float)vecY, (float)vecZ);

            return vec;
        }

        /// <summary>
        /// Calculates a face normal from three points. The points have to be coplanar and part of the face.
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="faceHandle">The reference of the face.</param>
        /// <returns></returns>
        public static float3 CalculateFaceNormal(this DCEL.Geometry geometry, int faceHandle)
        {
            var face = geometry.GetFaceByHandle(faceHandle);
            var outerHalfEdge = geometry.GetHalfEdgeByHandle(face.OuterHalfEdge);
            var nextHalfEdge = geometry.GetHalfEdgeByHandle(outerHalfEdge.NextHalfEdge);
            var prevHalfEdge = geometry.GetHalfEdgeByHandle(outerHalfEdge.PrevHalfEdge);

            var firstP = geometry.GetVertexByHandle(prevHalfEdge.OriginVertex).VertData.Pos;
            var secP = geometry.GetVertexByHandle(outerHalfEdge.OriginVertex).VertData.Pos;
            var thirdP = geometry.GetVertexByHandle(nextHalfEdge.OriginVertex).VertData.Pos;

            var a = secP - firstP;
            var b = thirdP - secP;

            var cross = float3.Cross(b, a);
            cross.Normalize();

            return cross;
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
