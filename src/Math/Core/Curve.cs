using System;
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
        /// The parts making up the Curve.
        /// </summary>
        public IList<CurvePart> CurveParts;

        /// <summary>
        /// Combines two Curves by creating a new one.
        /// </summary>
        /// <param name="a">The first curve to combine</param>
        /// <param name="b">The second curve to combine</param>
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
        /// <param name="curves">The curves to combine</param>
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
        /// Calculates a polyline, representing the curve itself.
        /// </summary>
        /// <param name="subdiv">The number of subdivisions per curve segment</param>
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
        /// Calculates a polyline, representing the curve itself.
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
        /// Returns a polyline, representing the curve segment. Intermediate points are calculated adaptively.
        /// </summary>
        /// <param name="area">Determines a maximal area size for the tringale created from start point random point and end point.</param>
        /// <returns></returns>
        public IEnumerable<float3> CalcAdaptivePolyline(float area)
        {
            foreach (var part in CurveParts)
            {
                foreach (var vert in part.CalcAdaptivePolyline(area))
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
        public bool Closed;

        /// <summary>
        /// This is the starting point of the CurvePart.
        /// </summary>
        public float3 StartPoint;

        /// <summary>
        /// The segments making up the CurvePart.
        /// </summary>
        public IList<CurveSegment> CurveSegments;

        /// <summary>
        /// Calculates a polyline, representing the curve part.
        /// </summary>
        /// <param name="subdiv">The number of subdivisions per curve segment</param>
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

                //If i == 0 sp = StartPoint, if not it's the last vert of the CurveSegment[i-1]s' list of vertices
                sp = i == 0 ? StartPoint : CurveSegments[i - 1].Vertices[CurveSegments[i - 1].Vertices.Count - 1];

                foreach (var vert in CurveSegments[i].CalcUniformPolyline(sp, subdiv, degree))
                {
                    yield return vert;
                }
            }
        }

        /// <summary>
        /// Calculates a polyline, representing the curve part.
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

                //If i == 0 sp = StartPoint, if not it's the last vert of the CurveSegment[i-1]s' list of vertices
                sp = i == 0 ? StartPoint : CurveSegments[i - 1].Vertices[CurveSegments[i - 1].Vertices.Count - 1];

                foreach (var vert in CurveSegments[i].CalcAdaptivePolyline(sp, angle, degree))
                {
                    yield return vert;
                }
            }
        }

        /// <summary>
        /// Returns a polyline, representing the curve segment. Intermediate points are calculated adaptively.
        /// </summary>
        /// <param name="area">Determines a maximal area size for the tringale created from start point random point and end point.</param>
        /// <returns></returns>
        public IEnumerable<float3> CalcAdaptivePolyline(float area)
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

                //If i == 0 sp = StartPoint, if not it's the last vert of the CurveSegment[i-1]s' list of vertices
                sp = i == 0 ? StartPoint : CurveSegments[i - 1].Vertices[CurveSegments[i - 1].Vertices.Count - 1];

                foreach (var vert in CurveSegments[i].CalcAdaptivePolyline(sp, area, degree))
                {
                    yield return vert;
                }
            }
        }
    }

    /// <summary>
    /// The base class for CurveSegments.
    /// A CurveSgment does not know its own start point. For the first CurveSegment in a sequence the start point is saved in the CurvePart belonging to the segment.
    /// The start point for the CurveSegment with index i is the last vertex in the CurveSegent[i-1]s' list of vertices.
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
        /// <param name="t">Beziér curves are polynominals of t. t is element of [0, 1]</param>
        /// <param name="vertices">All control points thet represent the curve, incl. start and end point</param>
        /// <returns></returns>
        public virtual float3 CalcPoint(float t, float3[] vertices)
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
        /// Splits a curve using De Casteljaus algorithm
        /// </summary>
        /// <param name="t">Beziér curves are polynominals of t. t is element of [0, 1]</param>
        /// <param name="vertices">All control points thet represent the curve, incl. start and end point.</param>
        /// <param name="leftCurve">The left new curve</param>
        /// <param name="rightCurve">The right new curve</param>
        public virtual void SplitCurve(float t, float3[] vertices, ref List<float3> leftCurve, ref List<float3> rightCurve)
        {
            if (vertices.Length == 1)
            {
                leftCurve.Add(vertices[0]);
                rightCurve.Add(vertices[0]);
                rightCurve.Reverse();
                return;
            }

            var newVerts = new float3[vertices.Length - 1];

            for (var i = 0; i < newVerts.Length; i++)
            {
                if (i == 0)
                {
                    leftCurve.Add(vertices[i]);
                }
                if (i == newVerts.Length - 1)
                {
                    rightCurve.Add(vertices[i + 1]);
                }
                //calculates a weighted average of vertices[i] and vertices[i + 1] for x,y,z --> point on line between vertices[i] and vertices[i + 1]
                var x = (1 - t) * vertices[i].x + t * vertices[i + 1].x;
                var y = (1 - t) * vertices[i].y + t * vertices[i + 1].y;
                var z = (1 - t) * vertices[i].z + t * vertices[i + 1].z;
                newVerts[i] = new float3(x, y, z);
            }
            SplitCurve(t, newVerts, ref leftCurve, ref rightCurve);
        }

        /// <summary>
        /// Returns a polyline, representing the curve segment. Intermediate points are calculated with uniform values of t.
        /// </summary>
        /// <param name="startPoint">The segments starting point</param>
        /// <param name="segmentsPerCurve">The number of segments per curve</param>
        /// <param name="degree">The degree of the curve: 1 for linear, 2 for conic, 3 for cubic</param>
        public virtual IEnumerable<float3> CalcUniformPolyline(float3 startPoint, int segmentsPerCurve, int degree)
        {
            var controlPoints = new List<float3> { startPoint };
            controlPoints.AddRange(Vertices);

            for (var i = 0; i < controlPoints.Count - degree; i += degree)
            {
                //Returns all points that already lie on the curve (i +=degree)
                yield return controlPoints[i];

                var verts = new float3[degree + 1];

                //After this loop verts represents a single Beziér curve  - not a Beziér path (= CurveSegment)
                for (var j = 0; j < verts.Length; j++)
                {
                    verts[j] = controlPoints[i + j];
                }

                //Calculate additional vertices for values of t
                for (var j = 1; j < segmentsPerCurve; j++)
                {
                    var t = j / (float)segmentsPerCurve;
                    var point = CalcPoint(t, verts);
                    yield return point;
                }
            }
            //Manually adds the last control point to maintain the order of the points
            yield return controlPoints[controlPoints.Count - 1];
        }

        /// <summary>
        /// Returns a polyline, representing the curve segment. Intermediate points are calculated adaptively.
        /// </summary>
        /// <param name="startPoint">The segments starting point</param>
        /// <param name="angle">Determines how far the angle between the two vectors(start point random point and random point end point) may vary from 180°</param>
        /// <param name="degree">The degree of the curve: 1 for linear, 2 for conic, 3 for cubic</param>
        /// <returns></returns>
        public virtual IEnumerable<float3> CalcAdaptivePolyline(float3 startPoint, int angle, int degree)
        {
            var controlPoints = new List<float3> { startPoint };
            controlPoints.AddRange(Vertices);

            for (var i = 0; i < controlPoints.Count - degree; i += degree)
            {
                //Returns all points that already lie on the curve (i +=degree)
                yield return controlPoints[i];

                var verts = new float3[degree + 1];

                //After this loop verts represents a single Beziér curve  - not a Beziér path (= CurveSegment)
                for (var j = 0; j < verts.Length; j++)
                {
                    verts[j] = controlPoints[i + j];
                }

                //Sample verts by performig a flatness test
                foreach (var float3 in AdaptiveSampling(verts, angle))
                {
                    yield return float3;
                }
            }
            //Manually adds the last control point to maintain the order of the points
            yield return controlPoints[controlPoints.Count - 1];
        }

        /// <summary>
        /// Returns a polyline, representing the curve segment. Intermediate points are calculated adaptively.
        /// </summary>
        /// <param name="startPoint">The segments starting point</param>
        /// <param name="area">Determines a maximal area size for the tringale created from start point random point and end point.</param>
        /// <param name="degree">The degree of the curve: 1 for linear, 2 for conic, 3 for cubic</param>
        /// <returns></returns>
        public virtual IEnumerable<float3> CalcAdaptivePolyline(float3 startPoint, float area, int degree)
        {
            var controlPoints = new List<float3> { startPoint };
            controlPoints.AddRange(Vertices);

            for (var i = 0; i < controlPoints.Count - degree; i += degree)
            {
                //Returns all points that already lie on the curve (i +=degree)
                yield return controlPoints[i];

                var verts = new float3[degree + 1];

                //After this loop verts represents a single Beziér curve  - not a Beziér path (= CurveSegment)
                for (var j = 0; j < verts.Length; j++)
                {
                    verts[j] = controlPoints[i + j];
                }

                //Sample verts by performig a flatness test
                foreach (var float3 in AdaptiveSampling(verts, area))
                {
                    yield return float3;
                }
            }
            //Manually adds the last control point to maintain the order of the points
            yield return controlPoints[controlPoints.Count - 1];
        }

        private IEnumerable<float3> AdaptiveSampling(float3[] verts, int angle)
        {
            var rnd = new Random();
            const double min = 0.45;
            const double max = 0.55;

            var a = verts[0];
            var b = verts[verts.Length - 1];

            var t = RandomT(rnd, min, max);
            var vertNearMiddle = CalcPoint((float)t, verts);

            if (!IsAngleFlatEnough(a, vertNearMiddle, b, angle))
            {
                yield return vertNearMiddle;

                List<float3> leftCurve = new List<float3>();
                List<float3> rightCurve = new List<float3>();
                SplitCurve((float)t, verts, ref leftCurve, ref rightCurve);

                foreach (var vert in AdaptiveSampling(leftCurve.ToArray(), angle))
                {
                    yield return vert;
                }
                foreach (var vert in AdaptiveSampling(rightCurve.ToArray(), angle))
                {
                    yield return vert;
                }
            }
        }

        private IEnumerable<float3> AdaptiveSampling(float3[] verts, float area)
        {
            var rnd = new Random();
            const double min = 0.45;
            const double max = 0.55;

            var a = verts[0];
            var b = verts[verts.Length - 1];

            var t = RandomT(rnd, min, max);
            var vertNearMiddle = CalcPoint((float)t, verts);

            if (!IsTriangleAreaSmallEnough(a, vertNearMiddle, b, area))
            {
                yield return vertNearMiddle;

                List<float3> leftCurve = new List<float3>();
                List<float3> rightCurve = new List<float3>();
                SplitCurve((float)t, verts, ref leftCurve, ref rightCurve);

                foreach (var vert in AdaptiveSampling(leftCurve.ToArray(), area))
                {
                    yield return vert;
                }
                foreach (var vert in AdaptiveSampling(rightCurve.ToArray(), area))
                {
                    yield return vert;
                }
            }
        }

        private double RandomT(Random rnd, double min, double max)
        {
            var rndT = min + rnd.NextDouble() * (max - min);
            rndT = System.Math.Round(rndT, 2);
            if (!rndT.Equals(0.50))
                return rndT;
            return RandomT(rnd, min, max);
        }

        private bool IsAngleFlatEnough(float3 a, float3 m, float3 b, float threshold)
        {
            var p = a - m;
            var q = b - m;
            var angle = CalcAngle(p, q);

            angle = M.RadiansToDegrees(angle);
            if (angle <= 180 && angle >= 180 - threshold)
                return true;
            return false;
        }

        private bool IsTriangleAreaSmallEnough(float3 a, float3 m, float3 b, float threshold)
        {
            var am = m - a;
            var ab = b - a;
            var alpha = CalcAngle(am, ab);
            var hc = am.Length * M.Sin(alpha);
            var area = ab.Length * hc / 2;

            if (area < threshold)
                return true;
            return false;
        }

        private float CalcAngle(float3 first, float3 second) //TODO: is already available als extension to float3 .. but this extension throws NaNs!
        {
            var nFirst = first / first.Length;
            var nSecond = second / second.Length;

            var dotP = float3.Dot(nFirst, nSecond);

            //Some values of dotP are not between -1 and 1 --> acos not defined --> NaN
            if (dotP < -1)
                dotP = -1;
            if (dotP > 1)
                dotP = 1;

            var angle = ((float)System.Math.Acos(dotP));

            return angle;
        }
    }

    /// <summary>
    /// Represents a linear segment of a CurvePart, using a list of float3.
    /// A CurveSgment does not know its own start point. For the first CurveSegment in a sequence the start point is saved in the CurvePart belonging to the segment.
    /// The start point for the CurveSegment with index i is the last vertex in the CurveSegent[i-1]s' list of vertices.
    /// </summary>
    public class LinearSegment : CurveSegment
    {
        /// <summary>
        /// Calculates a polyline, representing the curve segment.
        /// </summary>
        /// <param name="startPoint">The segments starting point</param>
        /// <param name="segmentsPerCurve">The number of segments per curve</param>
        /// <param name="degree">The degree of the curve: 1 for linear, 2 for conic, 3 for cubic</param>
        public override IEnumerable<float3> CalcUniformPolyline(float3 startPoint, int segmentsPerCurve, int degree)
        {
            var controlPoints = new List<float3> { startPoint };
            controlPoints.AddRange(Vertices);

            for (var i = 0; i < controlPoints.Count - degree; i += degree)
            {
                yield return controlPoints[i];

                var verts = new float3[degree + 1];

                for (var j = 0; j < verts.Length; j++)
                {
                    verts[j] = controlPoints[i + j];
                }

                for (var j = 1; j < segmentsPerCurve; j++)
                {
                    var t = j / (float)segmentsPerCurve;
                    yield return (1 - t) * verts[0] + t * verts[1];
                }
                yield return controlPoints[controlPoints.Count - 1];
            }
        }
    }

    /// <summary>
    /// Represents a conic beziér path of a CurvePart, using a list of float3.
    /// A CurveSgment does not know its own start point. For the first CurveSegment in a sequence the start point is saved in the CurvePart belonging to the segment.
    /// The start point for the CurveSegment with index i is the last vertex in the CurveSegent[i-1]s' list of vertices.
    /// </summary>
    public class BezierConicSegment : CurveSegment
    {

    }

    /// <summary>
    /// Represents a cubic beziér path of a CurvePart, using a list of float3.
    /// A CurveSgment does not know its own start point. For the first CurveSegment in a sequence the start point is saved in the CurvePart belonging to the segment.
    /// The start point for the CurveSegment with index i is the last vertex in the CurveSegent[i-1]s' list of vertices.
    /// </summary>
    public class BezierCubicSegment : CurveSegment
    {

    }

}
