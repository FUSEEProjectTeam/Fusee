using System.Collections.Generic;

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

        static void CombineCurve(Curve a, Curve b) //TODO: for rendering a text a list of Curves would be the better choice?
        {
            
        }
    }


    /// <summary>
    /// Represents a open or Closed part of a curve, using a list of CurveSegments and its starting point.
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

        /// <summary>
        /// The vertices belonging to the CurvePart.
        /// </summary>
        public IList<float3> Vertices;
        /// <summary>
        ///The tags belonging to the Vertices. Those are importent to create CurveSegments and defining the interpolation methode.
        /// </summary>
        public IList<byte> VertTags;
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

        private void GetSegmentVertices(List<float3> vertices)
        {
            
        }
    }

    /// <summary>
    /// Contains the possible methodes to interpolate between vertices.
    /// </summary>
    public enum InterpolationMethod
    {
        /// <summary>
        /// A linear curve is described by two successive "on curve" points.
        /// </summary>
        LINEAR,
        /// <summary>
        /// A cubic curve is decribed by two successive "off curve" points between two "on curve" points.
        /// </summary>
        BEZIER_CUBIC,
        /// <summary>
        /// A conic curve is described by a "off curve" point between two "on curve" points.
        /// </summary>
        BEZIER_CONIC
    }

}