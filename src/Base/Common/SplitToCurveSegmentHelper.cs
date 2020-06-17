using System;
using System.Collections.Generic;
using System.Linq;

using Fusee.Math.Core;

namespace Fusee.Base.Common
{
    /// <summary>
    /// Contains methods to spit a CurvePart into CurveSegments.
    /// </summary>
    public static class SplitToCurveSegmentHelper
    {
        internal enum SegmentType
        {
            Linear,
            Cubic,
            Conic
        }
        private static SegmentType Type { get; set; }

        /// <summary>
        /// Splits a CurvePart into CurveSegments by reading the byte pattern from a tag array.
        /// </summary>
        /// <param name="part">The CurvePart to be split into CurveSegments.</param>
        /// <param name="partTags">Tags of the CurvePart (on curve point, off curve point).</param>
        /// <param name="partVerts">Vertices of the CurvePart</param>
        /// <returns></returns>
        public static List<CurveSegment> SplitPartIntoSegments(CurvePart part, List<byte> partTags, List<float3> partVerts)
        {
            byte[] linearPattern = { 1, 1 };
            byte[] conicPattern = { 1, 0, 1 };
            byte[] cubicPattern = { 1, 0, 0, 1 };
            byte[] conicVirtualPattern = { 1, 0, 0, 0 };

            var segments = new List<CurveSegment>();

            for (var i = 0; i < partTags.Count; i++)
            {
                if (partTags.Skip(i).Take(linearPattern.Length).SequenceEqual(linearPattern))
                {
                    Type = SegmentType.Linear;
                    segments.Add(CreateCurveSegment(i, linearPattern, partVerts));
                }
                else if (partTags.Skip(i).Take(conicPattern.Length).SequenceEqual(conicPattern))
                {
                    Type = SegmentType.Conic;
                    segments.Add(CreateCurveSegment(i, conicPattern, partVerts));
                    i += 1;
                }
                else if (partTags.Skip(i).Take(cubicPattern.Length).SequenceEqual(cubicPattern))
                {
                    Type = SegmentType.Cubic;
                    segments.Add(CreateCurveSegment(i, cubicPattern, partVerts));
                    i += 2;
                }
                else if (partTags.Skip(i).Take(conicVirtualPattern.Length).SequenceEqual(conicVirtualPattern))
                {
                    Type = SegmentType.Conic;
                    var count = 0;
                    var cs = CreateCurveSegment(i, conicVirtualPattern, partVerts);

                    i += 3;

                    for (var j = i + 1; j < partTags.Count; j++)
                    {
                        cs.Vertices.Add(partVerts[j]);
                        if (partTags[j].Equals(0)) continue;
                        count = j;
                        break;
                    }

                    //If count = 0 it is the last segment of the curve. We need to add the first vertex in the list to the segment and leave it as it is. 
                    if (!count.Equals(0)) i = count - 1;
                    else cs.Vertices.Add(partVerts[0]);

                    //Create "on" points between two successive "off points.
                    CreateOnPointsAndAddToList(cs.Vertices);

                    segments.Add(cs);
                }
                else
                {
                    //Is needed only for "closed" CurveParts.
                    var lastSegment = new List<byte>();
                    lastSegment.AddRange(partTags.Skip(i).Take(partTags.Count - i));
                    lastSegment.Add(partTags[0]);
                    if (lastSegment.SequenceEqual(conicPattern))
                    {
                        segments.Add(CreateCurveSegment(i, conicPattern, partVerts[0], partVerts));
                    }
                    else if (lastSegment.SequenceEqual(cubicPattern))
                    {
                        segments.Add(CreateCurveSegment(i, cubicPattern, partVerts[0], partVerts));
                    }
                    else if (lastSegment.SequenceEqual(conicVirtualPattern))
                    {
                        segments.Add(CreateCurveSegment(i, cubicPattern, partVerts[0], partVerts));
                    }
                }
            }
            return segments;
        }

        private static void CreateOnPointsAndAddToList(IList<float3> vertices)
        {
            var zeroes = new List<float3>(vertices);
            zeroes.Remove(zeroes.First());
            zeroes.Remove(zeroes.Last());
            for (var j = 0; j < zeroes.Count; j++)
            {
                if (j + 1 >= zeroes.Count) break;
                var vPoint = new float3((zeroes[j].x + zeroes[j + 1].x) / 2, (zeroes[j].y + zeroes[j + 1].y) / 2,
                    (zeroes[j].z + zeroes[j + 1].z) / 2);

                if (j.Equals(0))
                {
                    vertices.Insert(2, vPoint);
                }
                else
                {
                    //Insert at every second index (2,4,6 ...).
                    vertices.Insert((j + 1) * 2, vPoint);
                }
            }
        }

        private static CurveSegment CreateCurveSegment(int i, ICollection<byte> pattern, List<float3> verts)
        {
            var segmentVerts = new List<float3>();
            segmentVerts.AddRange(verts.Skip(i).Take(pattern.Count));

            CurveSegment segment;
            switch (Type)
            {
                case SegmentType.Linear:
                    segment = new LinearSegment
                    {
                        Vertices = new List<float3>()
                    };
                    segment.Vertices = segmentVerts;
                    break;
                case SegmentType.Conic:
                    segment = new BezierConicSegment
                    {
                        Vertices = new List<float3>()
                    };
                    segment.Vertices = segmentVerts;
                    break;
                case SegmentType.Cubic:
                    segment = new BezierCubicSegment
                    {
                        Vertices = new List<float3>()
                    };
                    segment.Vertices = segmentVerts;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return segment;
        }

        private static CurveSegment CreateCurveSegment(int i, ICollection<byte> pattern, float3 startPoint, List<float3> verts)
        {
            var segmentVerts = new List<float3>();
            segmentVerts.AddRange(verts.Skip(i).Take(pattern.Count));
            segmentVerts.Add(startPoint);
            CurveSegment segment;
            switch (Type)
            {
                case SegmentType.Linear:
                    segment = new LinearSegment
                    {
                        Vertices = new List<float3>()
                    };
                    segment.Vertices = segmentVerts;
                    break;
                case SegmentType.Conic:
                    segment = new BezierConicSegment
                    {
                        Vertices = new List<float3>()
                    };
                    segment.Vertices = segmentVerts;
                    break;
                case SegmentType.Cubic:
                    segment = new BezierCubicSegment
                    {
                        Vertices = new List<float3>()
                    };
                    segment.Vertices = segmentVerts;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            segment.Vertices = segmentVerts;
            return segment;
        }

        /// <summary>
        /// Combines CurveSegments with the same type and adds them to a CurvePart.
        /// </summary>
        /// <param name="segments">List of CurveSegments - segments of the same type are combined.</param>
        /// <param name="part">Curve part to which the combined segments belong.</param>
        public static void CombineCurveSegmentsAndAddThemToCurvePart(List<CurveSegment> segments, CurvePart part)
        {
            //Combines segments that follow each other and have the same interpolation method.
            for (var i = 0; i < segments.Count; i++)
            {
                //Constraint
                if (i + 1 >= segments.Count) break;

                //Checks whether two successive segments have the same interpolation method, if so combine them.
                if (segments[i].GetType() == segments[i + 1].GetType())
                {
                    foreach (var vertex in segments[i + 1].Vertices)
                    {
                        if (vertex.Equals(segments[i + 1].Vertices[0])) continue;
                        segments[i].Vertices.Add(vertex);
                    }
                    segments.RemoveAt(i + 1);
                    //Sets the for loop one step back, to check the "new" CurvePart with its follower.
                    if (i >= 0)
                        i = i - 1;
                }
            }
            FixSegments(segments);

            part.CurveSegments = segments;
        }

        //Remove redundant points and set the last point of the last segment in each part to the parts starting point.
        private static void FixSegments(ICollection<CurveSegment> segments)
        {
            var firstPoint = segments.First().Vertices.First();

            foreach (var seg in segments)
            {
                seg.Vertices.Remove(seg.Vertices.First());
            }

            if (firstPoint == segments.Last().Vertices.Last()) return;

            var lastSegmentinPart = new LinearSegment { Vertices = new List<float3>() };
            lastSegmentinPart.Vertices.Add(firstPoint);
            segments.Add(lastSegmentinPart);

        }
    }
}