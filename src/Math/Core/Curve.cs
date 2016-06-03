using System.Collections.Generic;
using System.Net;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a curve, using a list of CurveParts
    /// </summary>
    public class Curve
    {
        public IList<CurvePart> CurveParts;

        /// <summary>
        /// Test purpose only, do not use this!
        /// </summary>
        public IDictionary<float3, int[]> VertNTag;

        /// <summary>
        /// Test purpose only, do not use this!
        /// </summary>
        public IList<float3> Vertices;

        static void CombineCurve(Curve a, Curve b)
        {
            
        }
    }


    /// <summary>
    /// Represents a open or closed part of a curve, using a list of CurveSegments and its starting point
    /// </summary>
    public class CurvePart
    {
        public bool closed;
        public float3 startPoint;
        public IList<CurveSegment> CurveSegments;

        /// <summary>
        /// Test purpose only, do not use this!
        /// </summary>
        public IList<float3> Vertices;
    }

    /// <summary>
    /// Represents a segment of a CurvePart, using a list of float3 and their interpolation methode
    /// </summary>
    public class CurveSegment
    {
        public IList<float3> Vertices;
        InterpolationMethod Interpolation;

        private void GetSegmentVertices(List<float3> vertices)
        {
            
        }
    }

    /// <summary>
    /// Contains the possible methodes to interpolate between vertices
    /// </summary>
    public enum InterpolationMethod
    {
        Linear,
        BezierCubic,
    }

}