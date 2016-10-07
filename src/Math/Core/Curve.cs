using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a curve, using a list of CurveParts.
    /// </summary>
    public class Curve
    {
        /// <summary>
        /// The parts making up the Curve.
        /// </summary>
        public IList<CurvePart> CurveParts;

        /// <summary>
        /// Combines two Curves by creating a new one.
        /// </summary>
        /// <param name="a">The first curve to combine</param>
        /// <param name="b">The second curve to combine</param>
        /// <returns></returns>
        public static Curve CombineCurve(Curve a, Curve b)
        {
            //Concat returns a new list, without modifying the original
            var combinedCurve = new Curve { CurveParts = (IList<CurvePart>)a.CurveParts.Concat(b.CurveParts) };
            return combinedCurve;
        }

        /// <summary>
        /// Combines a list of Curves by creating a new Curve out of the list.
        /// </summary>
        /// <param name="curves">The curves to combine</param>
        /// <returns></returns>
        public static Curve CombineCurve(IEnumerable<Curve> curves)
        {
            var combinedCurve = new Curve { CurveParts = new List<CurvePart>() };
            foreach (var curve in curves)
            {
                foreach (var part in curve.CurveParts)
                {
                    combinedCurve.CurveParts.Add(part);
                }
            }
            return combinedCurve;
        }
    }


    /// <summary>
    /// Represents a open or closed part of a curve, using a list of CurveSegments and its starting point.
    /// </summary>
    public class CurvePart
    {
        /// <summary>
        /// A CurvePart can be closed or open.
        /// </summary>
        public bool Closed;

        /// <summary>
        /// This is the starting point of the CurvePart.
        /// </summary>
        public float3 StartPoint;

        /// <summary>
        /// The segments making up the CurveParts.
        /// </summary>
        public IList<CurveSegment> CurveSegments;
    }

    /// <summary>
    /// The base class for CurveSegments.
    /// Represents a segment of a CurvePart, using a list of float3.
    /// </summary>
    public abstract class CurveSegment
    {
        /// <summary>
        /// The Vertices, representet as float3, of a CurveSegment.
        /// </summary>
        public IList<float3> Vertices;

        /// <summary>
        /// Calculates a point on a beziér curve using De Casteljaus algorithm.
        /// </summary>
        /// <param name="t">Beziér curves are polynominals of t. t is element of [0, 1] </param>
        /// <param name="vertices">All control points, incl. start and end point</param>
        /// <returns></returns>
        public float3 CalcPoint(float t, float3[] vertices)
        {
            if (vertices.Length == 1)
                return vertices[0];

            var newVerts = new float3[vertices.Length - 1];

            for (var i = 0; i < newVerts.Length; i++)
            {
                //calculates a weighted average of vertices[i] and vertices[i + 1] for x,y,z --> point on line between vertices[i] and vertices[i + 1]
                var x = (1 - t) * vertices[i].x + t * vertices[i + 1].x;
                var y = (1 - t) * vertices[i].y + t * vertices[i + 1].y;
                var z = (1 - t) * vertices[i].z + t * vertices[i + 1].z;
                newVerts[i] = new float3(x, y, z);
            }
            return CalcPoint(t, newVerts);
        }


        /// <summary>
        /// Calculates a polyline, representing the segment
        /// </summary>
        /// <param name="startPoint">The segments starting point</param>
        /// <param name="segmentsPerCurve">The number of segments per curve</param>
        /// <returns></returns>
        public abstract IEnumerable<float3> CalculateUniformPolyline(float3 startPoint, int segmentsPerCurve);

    }

    /// <summary>
    /// Represents a linear segment of a CurvePart, using a list of float3.
    /// </summary>
    public class LinearSegment : CurveSegment
    {
        /// <summary>
        /// Calculates a polyline, representing the segment
        /// </summary>
        /// <param name="startPoint">The segments starting point</param>
        /// <param name="segmentsPerCurve">The number of segments per curve</param>
        /// <returns></returns>
        public override IEnumerable<float3> CalculateUniformPolyline(float3 startPoint, int segmentsPerCurve)
        {

            var zwerg = new List<float3>();
            zwerg.Add(startPoint);
            zwerg.AddRange(Vertices);


            for (var i = 0; i < zwerg.Count - 1; i += 1)
            {
                var verts = new float3[2];
                if (i == 0)
                {
                    yield return startPoint;

                }
                verts[0] = zwerg[i];
                verts[1] = zwerg[i + 1];

                for (var j = 1; j < segmentsPerCurve; j++)
                {
                    var t = j / (float)segmentsPerCurve;
                    yield return (1 - t) * verts[0] + t * verts[1];
                }
                yield return zwerg.Last();
            }
        }
    }

    /// <summary>
    /// Represents a cubic beziér path of a CurvePart, using a list of float3.
    /// </summary>
    public class BezierCubicSegment : CurveSegment
    {

        private float3 CalculateCubicPoint(float t, IList<float3> vertices)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            float3 p = uuu * vertices[0]; //first term
            p += 3 * uu * t * vertices[1]; //second term
            p += 3 * u * tt * vertices[2]; //third term
            p += ttt * vertices[3]; //fourth term

            return p;
        }

        /// <summary>
        /// Calculates a polyline, representing the segment
        /// </summary>
        /// <param name="startPoint">The segments starting point</param>
        /// <param name="segmentsPerCurve">The number of segments per curve</param>
        /// <returns></returns>
        public override IEnumerable<float3> CalculateUniformPolyline(float3 startPoint, int segmentsPerCurve)
        {
            var zwerg = new List<float3>();
            zwerg.Add(startPoint);
            zwerg.AddRange(Vertices);

            for (var i = 0; i < zwerg.Count - 3; i += 3)
            {

                yield return zwerg[i];
                var verts = new float3[4];
                
                verts[0] = zwerg[i];
                verts[1] = zwerg[i + 1];
                verts[2] = zwerg[i + 2];
                verts[3] = zwerg[i + 3];

                for (var j = 1; j < segmentsPerCurve; j++)
                {
                    var t = j / (float)segmentsPerCurve;
                    var point = CalcPoint(t, verts);
                    yield return point;
                }
                yield return zwerg.Last();
            }

        }
    }

    /// <summary>
    /// Represents a conic beziér path of a CurvePart, using a list of float3.
    /// </summary>
    public class BezierConicSegment : CurveSegment
    {

        private float3 CalculateConicPoint(float t, IList<float3> vertices)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            float3 p = uu * vertices[0]; //first term
            p += 2 * u * t * vertices[1]; //second term
            p += tt * vertices[2]; //third term

            return p;
        }

        /// <summary>
        /// Calculates a polyline, representing the segment
        /// </summary>
        /// <param name="startPoint">The segments starting point</param>
        /// <param name="segmentsPerCurve">The number of segments per curve</param>
        /// <returns></returns>
        public override IEnumerable<float3> CalculateUniformPolyline(float3 startPoint, int segmentsPerCurve)
        {
            var zwerg = new List<float3>();
            zwerg.Add(startPoint);
            zwerg.AddRange(Vertices);

            for (var i = 0; i < zwerg.Count - 2; i += 2)
            {
                yield return zwerg[i];
                var verts = new float3[3];

                verts[0] = zwerg[i];
                verts[1] = zwerg[i + 1];
                verts[2] = zwerg[i + 2];

                for (var j = 1; j < segmentsPerCurve; j++)
                {
                    var t = j / (float)segmentsPerCurve;
                    var point = CalcPoint(t, verts);
                    yield return point;
                }
            }
            yield return zwerg.Last();
        }
    }

}
