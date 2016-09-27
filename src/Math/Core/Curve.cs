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

        static Curve CombineCurve(Curve a, Curve b)
        {
            //Concat returns a new list, without modifying the original
            var combinedCurve = new Curve { CurveParts = (IList<CurvePart>)a.CurveParts.Concat(b.CurveParts) };
            return combinedCurve;
        }

        static Curve CombineCurve(IEnumerable<Curve> curves)
        {
            var combinedCurve = new Curve {CurveParts = new List<CurvePart>()};
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
    /// Represents a segment of a CurvePart, using a list of float3 and their interpolation methode.
    /// </summary>
    public class CurveSegment
    {
        /// <summary>
        /// The Vertices, representet as float3, of a CurveSegment.
        /// </summary>
        public IList<float3> Vertices;
        /// <summary>
        /// Defines which kinde of curve the segment belongs to.
        /// </summary>
        public InterpolationMethod Interpolation;
    }

    /// <summary>
    /// Contains the possible methodes to interpolate between vertices.
    /// </summary>
    public enum InterpolationMethod
    {
        /// <summary>
        /// A line is described by two successive "on curve" points.
        /// </summary>
        LINEAR,
        /// <summary>
        /// A cubic curve is decribed by two successive control points between two "on curve" points.
        /// </summary>
        BEZIER_CUBIC,
        /// <summary>
        /// A conic curve is described by one control point between two "on curve" points.
        /// </summary>
        BEZIER_CONIC
    }

}