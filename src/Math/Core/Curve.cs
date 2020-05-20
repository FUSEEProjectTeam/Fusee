﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a curve, using a list of CurveParts.
    /// </summary>
    public class Curve
    {
        /// <summary>
        /// The parts forming the curve.
        /// </summary>
        public IList<CurvePart> CurveParts = new List<CurvePart>();

        /// <summary>
        /// Combines two Curves by creating a new one.
        /// </summary>
        /// <param name="a">The first curve to be combined.</param>
        /// <param name="b">The first curve to be combined.</param>
        /// <returns></returns>
        public static Curve CombineCurves(Curve a, Curve b)
        {
            //Concat returns a new list, without modifying the original
            var combinedCurve = new Curve { CurveParts = (IList<CurvePart>)a.CurveParts.Concat(b.CurveParts) };
            return combinedCurve;
        }

        /// <summary>
        /// Combines a list of Curves by creating a new Curve out of the list.
        /// </summary>
        /// <param name="curves">The curves to be combined.</param>
        /// <returns></returns>
        public static Curve CombineCurves(IEnumerable<Curve> curves)
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

        /// <summary>
        /// Calculates a polygonal chain, representing the curve.
        /// </summary>
        /// <param name="subdiv">The number of subdivisions per curve segment.</param>
        /// <returns></returns>
        public IEnumerable<float3> CalcUniformPolyline(int subdiv)
        {
            foreach (var part in CurveParts)
            {
                foreach (var vert in part.CalcUniformPolyline(subdiv))
                {
                    yield return vert;
                }
            }
        }

        /// <summary>
        /// Calculates a polygonal chain, representing the curve.
        /// </summary>
        /// <param name="angle">Determines how far the angle may vary from 180°.</param>
        /// <returns></returns>
        public IEnumerable<float3> CalcAdaptivePolyline(int angle)
        {
            foreach (var part in CurveParts)
            {
                foreach (var vert in part.CalcAdaptivePolyline(angle))
                {
                    yield return vert;
                }
            }
        }

        /// <summary>
        /// Returns a polygonal chain, representing the curve segment. Intermediate points are calculated adaptively.
        /// </summary>
        /// <param name="acreage">Determines the maximum acreage for the triangle consisting out of start point, random point and end point.</param>
        /// <returns></returns>
        public IEnumerable<float3> CalcAdaptivePolyline(float acreage)
        {
            foreach (var part in CurveParts)
            {
                foreach (var vert in part.CalcAdaptivePolyline(acreage))
                {
                    yield return vert;
                }
            }
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
        public bool IsClosed;

        /// <summary>
        /// The starting point of the CurvePart.
        /// </summary>
        public float3 StartPoint;

        /// <summary>
        /// The segments making up the CurvePart.
        /// </summary>
        public IList<CurveSegment> CurveSegments = new List<CurveSegment>();

        /// <summary>
        /// Calculates a polygonal chain, representing the CurvePart.
        /// </summary>
        /// <param name="subdiv">The number of subdivisions per CurveSegment.</param>
        /// <returns></returns>
        public IEnumerable<float3> CalcUniformPolyline(int subdiv)
        {
            for (var i = 0; i < CurveSegments.Count; i++)
            {
                float3 sp;
                var degree = 0;

                if (CurveSegments[i].GetType() == typeof(LinearSegment))
                {
                    degree = 1;
                }
                else if (CurveSegments[i].GetType() == typeof(BezierConicSegment))
                {
                    degree = 2;
                }
                else if (CurveSegments[i].GetType() == typeof(BezierCubicSegment))
                {
                    degree = 3;
                }

                //If i == 0 sp = StartPoint, if not it's the last vertex of the CurveSegment[i-1]'s list of vertices.
                sp = i == 0 ? StartPoint : CurveSegments[i - 1].Vertices[CurveSegments[i - 1].Vertices.Count - 1];

                foreach (var vert in CurveSegments[i].CalcUniformPolyline(sp, subdiv, degree))
                {
                    yield return vert;
                }
            }
        }

        /// <summary>
        /// Calculates a polygonal chain, representing the curve part.
        /// </summary>
        /// <param name="angle">Determines how far the angle may vary from 180°.</param>
        /// <returns></returns>
        public IEnumerable<float3> CalcAdaptivePolyline(int angle)
        {
            for (var i = 0; i < CurveSegments.Count; i++)
            {
                float3 sp;
                var degree = 0;

                if (CurveSegments[i].GetType() == typeof(LinearSegment))
                {
                    degree = 1;
                }
                else if (CurveSegments[i].GetType() == typeof(BezierConicSegment))
                {
                    degree = 2;
                }
                else if (CurveSegments[i].GetType() == typeof(BezierCubicSegment))
                {
                    degree = 3;
                }

                //If i == 0 sp = StartPoint, if not it's the last vertex of the CurveSegment[i-1]'s list of vertices.
                sp = i == 0 ? StartPoint : CurveSegments[i - 1].Vertices[CurveSegments[i - 1].Vertices.Count - 1];

                foreach (var vert in CurveSegments[i].CalcAdaptivePolylineWAngle(sp, angle, degree))
                {
                    yield return vert;
                }
            }
        }

        /// <summary>
        /// Returns a polygonal chain, representing the curve segment. Intermediate points are calculated adaptively.
        /// </summary>
        /// <param name="acreage">Determines a maximal acreage for the triangle created from start point random point and end point.</param>
        /// <returns></returns>
        public IEnumerable<float3> CalcAdaptivePolyline(float acreage)
        {
            for (var i = 0; i < CurveSegments.Count; i++)
            {
                float3 sp;
                var degree = 0;

                if (CurveSegments[i].GetType() == typeof(LinearSegment))
                {
                    degree = 1;
                }
                else if (CurveSegments[i].GetType() == typeof(BezierConicSegment))
                {
                    degree = 2;
                }
                else if (CurveSegments[i].GetType() == typeof(BezierCubicSegment))
                {
                    degree = 3;
                }

                //If i == 0 sp = StartPoint, if not it's the last vertex of the CurveSegment[i-1]'s list of vertices.
                sp = i == 0 ? StartPoint : CurveSegments[i - 1].Vertices[CurveSegments[i - 1].Vertices.Count - 1];

                foreach (var vert in CurveSegments[i].CalcAdaptivePolylineWArcreage(sp, acreage, degree))
                {
                    yield return vert;
                }
            }
        }
    }

    /// <summary>
    /// The base class for CurveSegments.
    /// A CurveSgment does not know its own start point. For the first CurveSegment in a sequence the start point is saved in the CurvePart belonging to the segment.
    /// The start point for the CurveSegment with index i is the last vertex in the CurveSegent[i-1]'s list of vertices.
    /// </summary>
    public abstract class CurveSegment
    {
        /// <summary>
        ///The vertices of a CurveSegment represented by float3s.
        /// </summary>
        public IList<float3> Vertices = new List<float3>();

        /// <summary>
        /// Calculates a point on a bézier curve using De Casteljau's algorithm.
        /// </summary>
        /// <param name="t">Bézier curves are polynominals of t. t is element of [0, 1]</param>
        /// <param name="vertices">All control points that represent the curve, incl. start and end point</param>
        /// <returns></returns>
        public float3 CalcPoint(float t, float3[] vertices)
        {
            if (vertices.Length == 1)
                return vertices[0];

            var newVerts = new float3[vertices.Length - 1];

            for (var i = 0; i < newVerts.Length; i++)
            {
                //Calculates a weighted average of vertices[i] and vertices[i + 1] for x,y,z --> point on line between vertices[i] and vertices[i + 1]
                var point = (1 - t) * vertices[i] + t * vertices[i + 1];
                newVerts[i] = point;
            }
            return CalcPoint(t, newVerts);
        }

        /// <summary>
        /// Splits a curve using De Casteljau's algorithm.
        /// </summary>
        /// <param name="t">Bézier curves are polynominals of t. t is element of [0, 1].</param>
        /// <param name="vertices">All control points that represent the curve, incl. start and end point.</param>
        /// <param name="leftCurve">The left new curve.</param>
        /// <param name="rightCurve">The right new curve.</param>
        public void SplitCurve(float t, float3[] vertices, ref List<float3> leftCurve, ref List<float3> rightCurve)
        {
            if (vertices.Length == 1)
            {
                leftCurve.Add(vertices[0]); //Third position.
                rightCurve.Add(vertices[0]);//First position.
                rightCurve.Reverse(); //Maintain the winding of the curve.
                return;
            }

            var newVerts = new float3[vertices.Length - 1];

            for (var i = 0; i < newVerts.Length; i++)
            {
                if (i == 0)
                {
                    leftCurve.Add(vertices[0]);
                }
                if (i == newVerts.Length - 1)
                {
                    rightCurve.Add(vertices[i + 1]);
                }
                //Calculates a weighted average of vertices[i] and vertices[i + 1] for x,y,z --> point on line between vertices[i] and vertices[i + 1].
                var newVert = (1 - t) * vertices[i] + t * vertices[i + 1];

                newVerts[i] = newVert;
            }
            SplitCurve(t, newVerts, ref leftCurve, ref rightCurve);
        }

        /// <summary>
        /// Returns a polygonal chain, representing the curve segment. Intermediate points are calculated with uniform values of t.
        /// </summary>
        /// <param name="startPoint">The segment's starting point.</param>
        /// <param name="segmentsPerCurve">The number of segments per curve.</param>
        /// <param name="degree">The degree of the curve: 1 for linear, 2 for conic, 3 for cubic.</param>
        public virtual IEnumerable<float3> CalcUniformPolyline(float3 startPoint, int segmentsPerCurve, int degree)
        {
            var controlPoints = new List<float3> { startPoint };
            controlPoints.AddRange(Vertices);

            for (var i = 0; i < controlPoints.Count - degree; i += degree)
            {
                //Returns all points that already lie on the curve (i +=degree).
                yield return controlPoints[i];

                var verts = new float3[degree + 1];

                //After this loop vertices represent a single Bézier curve  - not a Bézier path (= CurveSegment).
                for (var j = 0; j < verts.Length; j++)
                {
                    verts[j] = controlPoints[i + j];
                }

                //Calculates additional vertices for values of t.
                for (var j = 1; j < segmentsPerCurve; j++)
                {
                    var t = j / (float)segmentsPerCurve;
                    var point = CalcPoint(t, verts);
                    yield return point;
                }
            }
            //Adds the last control point to maintain the order of the points.
            yield return controlPoints[controlPoints.Count - 1];
        }

        /// <summary>
        /// Returns a polygonal chain, representing the curve segment. Intermediate points are calculated adaptively.
        /// </summary>
        /// <param name="startPoint">The segment's starting point</param>
        /// <param name="angle">Determines how far the angle between the two vectors(start point, random point and end point) may vary from 180°</param>
        /// <param name="degree">The degree of the curve: 1 for linear, 2 for conic, 3 for cubic</param>
        /// <returns></returns>
        public IEnumerable<float3> CalcAdaptivePolylineWAngle(float3 startPoint, int angle, int degree)
        {
            var controlPoints = new List<float3> { startPoint };
            controlPoints.AddRange(Vertices);

            //Splits a Bézier path into a single Bézier curve, by dividing the list at every "on curve" point.
            for (var i = 0; i < controlPoints.Count - degree; i += degree)
            {
                var verts = new float3[degree + 1];

                //After this loop vertices represent a single Bézier curve  - not a Bézier path (= CurveSegment).
                for (var j = 0; j < verts.Length; j++)
                {
                    verts[j] = controlPoints[i + j];
                }

                var vertList = new List<float3>();

                //Sample vertices by performing a flatness test.
                AdaptiveSamplingWAngle(verts, angle, ref vertList);

                foreach (var vert in vertList)
                {
                    yield return vert;
                }
            }
        }

        /// <summary>
        /// Returns a polygonal chain, representing the curve segment. Intermediate points are calculated adaptively.
        /// </summary>
        /// <param name="startPoint">The segment's starting point</param>
        /// <param name="acreage">Determines a maximal acreage for the triangle created from start point random point and end point.</param>
        /// <param name="degree">The degree of the curve: 1 for linear, 2 for conic, 3 for cubic</param>
        /// <returns></returns>
        public IEnumerable<float3> CalcAdaptivePolylineWArcreage(float3 startPoint, float acreage, int degree)
        {
            var controlPoints = new List<float3> { startPoint };
            controlPoints.AddRange(Vertices);

            for (var i = 0; i < controlPoints.Count - degree; i += degree)
            {
                var verts = new float3[degree + 1];

                //After this loop vertices represent a single Bézier curve  - not a Bézier path (= CurveSegment).
                for (var j = 0; j < verts.Length; j++)
                {
                    verts[j] = controlPoints[i + j];
                }

                var vertList = new List<float3>();

                //Sample vertices by performing a flatness test.
                AdaptiveSamplingWArcreage(verts, acreage, ref vertList);

                foreach (var vert in vertList)
                {
                    yield return vert;
                }
            }
        }

        private void AdaptiveSamplingWAngle(float3[] verts, int angle, ref List<float3> vertList)
        {
            var rnd = new Random();
            const double min = 0.45;
            const double max = 0.55;

            var a = verts[0];
            var b = verts[verts.Length - 1];

            var t = RandomT(rnd, min, max);
            var vertNearMiddle = CalcPoint((float)t, verts);

            if (IsAngleFlatEnough(a, vertNearMiddle, b, angle))
            {
                vertList.Add(b);
            }
            else
            {
                var leftCurve = new List<float3>();
                var rightCurve = new List<float3>();
                SplitCurve((float)t, verts, ref leftCurve, ref rightCurve);

                AdaptiveSamplingWAngle(leftCurve.ToArray(), angle, ref vertList);
                AdaptiveSamplingWAngle(rightCurve.ToArray(), angle, ref vertList);
            }
        }

        private void AdaptiveSamplingWArcreage(float3[] verts, float acreage, ref List<float3> vertList)
        {
            var rnd = new Random();
            const double min = 0.45;
            const double max = 0.55;

            var a = verts[0];
            var b = verts[verts.Length - 1];

            var t = RandomT(rnd, min, max);
            var vertNearMiddle = CalcPoint((float)t, verts);

            if (IsArcreageSmallEnough(a, vertNearMiddle, b, acreage))
            {
                vertList.Add(b);
            }
            else
            {
                var leftCurve = new List<float3>();
                var rightCurve = new List<float3>();
                SplitCurve((float)t, verts, ref leftCurve, ref rightCurve);

                AdaptiveSamplingWArcreage(leftCurve.ToArray(), acreage, ref vertList);
                AdaptiveSamplingWArcreage(rightCurve.ToArray(), acreage, ref vertList);
            }
        }

        private static double RandomT(Random rnd, double min, double max)
        {
            var rndT = min + rnd.NextDouble() * (max - min);
            rndT = System.Math.Round(rndT, 2);
            if (!rndT.Equals(0.50))
                return rndT;
            return RandomT(rnd, min, max);
        }

        private static bool IsAngleFlatEnough(float3 a, float3 m, float3 b, float threshold)
        {
            var p = a - m;
            var q = b - m;
            var angle = float3.CalculateAngle(p, q);

            angle = M.RadiansToDegrees(angle);
            if (angle <= 180 && angle >= 180 - threshold)
                return true;
            return false;
        }

        private static bool IsArcreageSmallEnough(float3 a, float3 m, float3 b, float threshold)
        {
            //equals the formula "0.5 * float3.Cross(a-m, a-b).Length", but without square roots
            var det = new float3x3
                (1, 1, 1,
                m.x - a.x, m.y - a.y, m.z - a.z,
                b.x - a.x, b.y - a.y, b.z - a.z)
                .Determinant;

            var area = 0.5 * det;
            if (area < 1) area = area * -1;

            if (area < threshold)
                return true;
            return false;
        }
    }

    /// <summary>
    /// Represents a linear segment of a CurvePart by using a list of float3s.
    /// A CurveSgment does not know its own start point. For the first CurveSegment in a sequence the start point is saved in the CurvePart belonging to the segment.
    /// The start point for the CurveSegment with index i is the last vertex in the CurveSegent[i-1]'s list of vertices.
    /// </summary>
    public class LinearSegment : CurveSegment
    {
        /// <summary>
        /// Calculates a polygonal chain, representing the curve segment.
        /// </summary>
        /// <param name="startPoint">The segment's starting point</param>
        /// <param name="segmentsPerCurve">The number of segments per curve</param>
        /// <param name="degree">The degree of the curve: 1 for linear, 2 for conic, 3 for cubic</param>
        public override IEnumerable<float3> CalcUniformPolyline(float3 startPoint, int segmentsPerCurve, int degree)
        {
            var controlPoints = new List<float3> { startPoint };
            controlPoints.AddRange(Vertices);

            for (var i = 0; i < controlPoints.Count - degree; i += degree)
            {
                //Returns all points that already lie on the curve (i +=degree)
                yield return controlPoints[i];

                var verts = new float3[degree + 1];

                for (var j = 0; j < verts.Length; j++)
                {
                    verts[j] = controlPoints[i + j];
                }

                //Calculate additional vertices for values of t
                for (var j = 1; j < segmentsPerCurve; j++)
                {
                    var t = j / (float)segmentsPerCurve;
                    var point = (1 - t) * verts[0] + t * verts[1];
                    yield return point;
                }
            }
            //Manually adds the last control point to maintain the order of the points
            yield return controlPoints[controlPoints.Count - 1];
        }
    }

    /// <summary>
    /// Represents a conic bézier path of a CurvePart by using a list of float3s.
    /// A CurveSgment does not know its own start point. For the first CurveSegment in a sequence the start point is saved in the CurvePart belonging to the segment.
    /// The start point for the CurveSegment with index i is the last vertex in the CurveSegent[i-1]'s list of vertices.
    /// </summary>
    public class BezierConicSegment : CurveSegment
    {
    }

    /// <summary>
    /// Represents a cubic bézier path of a CurvePart by using a list of float3s.
    /// A CurveSgment does not know its own start point. For the first CurveSegment in a sequence the start point is saved in the CurvePart belonging to the segment.
    /// The start point for the CurveSegment with index i is the last vertex in the CurveSegent[i-1]'s list of vertices.
    /// </summary>
    public class BezierCubicSegment : CurveSegment
    {
    }
}