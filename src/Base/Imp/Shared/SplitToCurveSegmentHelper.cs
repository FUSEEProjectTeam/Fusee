using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Math.Core;

#if PLATFORM_DESKTOP
namespace Fusee.Base.Imp.Desktop
#elif PLATFORM_WEB
namespace Fusee.Base.Imp.Web
#endif
{
    static class SplitToCurveSegmentHelper
    {
        internal enum SegmentType
        {
            LINEAR,
            CUBIC,
            CONIC
        }
        private static SegmentType Type { get; set; }

        public static List<CurveSegment> SplitPartIntoSegments(CurvePart part, List<byte> partTags, List<float3> partVerts)
        {
            byte[] linearPattern = { 1, 1 };
            byte[] conicPattern = { 1, 0, 1 };
            byte[] cubicPattern = { 1, 0, 0, 1 };
            byte[] conicVirtualPattern = { 1, 0, 0, 0 };

            var segments = new List<CurveSegment>();

            for (var i = 0; i < partTags.Count; i++)
            {
                if (partTags.SkipItems(i).TakeItems(linearPattern.Length).SequEqual(linearPattern))
                {
                    Type = SegmentType.LINEAR;
                    segments.Add(CreateCurveSegment(part, i, linearPattern, partVerts));
                }
                else if (partTags.SkipItems(i).TakeItems(conicPattern.Length).SequEqual(conicPattern))
                {
                    Type = SegmentType.CONIC;
                    segments.Add(CreateCurveSegment(part, i, conicPattern, partVerts));
                    i += 1;
                }
                else if (partTags.SkipItems(i).TakeItems(cubicPattern.Length).SequEqual(cubicPattern))
                {
                    Type = SegmentType.CUBIC;
                    segments.Add(CreateCurveSegment(part, i, cubicPattern, partVerts));
                    i += 2;
                }
                else if (partTags.SkipItems(i).TakeItems(conicVirtualPattern.Length).SequEqual(conicVirtualPattern))
                {
                    Type = SegmentType.CONIC;
                    var count = 0;
                    var cs = CreateCurveSegment(part, i, conicVirtualPattern, partVerts);

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
                    lastSegment.AddRange(partTags.SkipItems(i).TakeItems(partTags.Count - i));
                    lastSegment.Add(partTags[0]);
                    if (lastSegment.SequEqual(conicPattern))
                    {
                        segments.Add(CreateCurveSegment(part, i, conicPattern, partVerts[0], partVerts));
                    }
                    else if (lastSegment.SequEqual(cubicPattern))
                    {
                        segments.Add(CreateCurveSegment(part, i, cubicPattern, partVerts[0], partVerts));
                    }
                    else if (lastSegment.SequEqual(conicVirtualPattern))
                    {
                        segments.Add(CreateCurveSegment(part, i, cubicPattern, partVerts[0], partVerts));
                    }
                }
            }
            return segments;
        }

        public static void CreateOnPointsAndAddToList(IList<float3> vertices)
        {
            var zeroes = new List<float3>(vertices);
            zeroes.Remove(zeroes.First());
            zeroes.Remove(zeroes.LastItem());
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

        public static CurveSegment CreateCurveSegment(CurvePart cp, int i, byte[] pattern, List<float3> verts)
        {
            var segmentVerts = new List<float3>();
            segmentVerts.AddRange(verts.SkipItems(i).TakeItems(pattern.Length));

            CurveSegment segment;
            switch (Type)
            {
                case SegmentType.LINEAR:
                    segment = new LinearSegment
                    {
                        Vertices = new List<float3>()
                    };
                    segment.Vertices = segmentVerts;
                    break;
                case SegmentType.CONIC:
                    segment = new BezierConicSegment
                    {
                        Vertices = new List<float3>()
                    };
                    segment.Vertices = segmentVerts;
                    break;
                case SegmentType.CUBIC:
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

        public static CurveSegment CreateCurveSegment(CurvePart cp, int i, byte[] pattern, float3 startPoint, List<float3> verts)
        {
            var segmentVerts = new List<float3>();
            segmentVerts.AddRange(verts.SkipItems(i).TakeItems(pattern.Length));
            segmentVerts.Add(startPoint);
            CurveSegment segment;
            switch (Type)
            {
                case SegmentType.LINEAR:
                    segment = new LinearSegment
                    {
                        Vertices = new List<float3>()
                    };
                    segment.Vertices = segmentVerts;
                    break;
                case SegmentType.CONIC:
                    segment = new BezierConicSegment
                    {
                        Vertices = new List<float3>()
                    };
                    segment.Vertices = segmentVerts;
                    break;
                case SegmentType.CUBIC:
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

        public static void CombineCurveSegmentsAndAddThemToCurvePart(List<CurveSegment> segments, CurvePart part)
        {
            //Combine segments that follow each other and have the same interpolation methode.
            for (var i = 0; i < segments.Count; i++)
            {
                //Constraint
                if (i + 1 >= segments.Count) break;

                //Check whether two successive segments have the same interpolation Methode, if so combine them.
                if (segments[i].GetType() == segments[i + 1].GetType())
                {
                    foreach (var vertex in segments[i + 1].Vertices)
                    {
                        if (vertex.Equals(segments[i + 1].Vertices[0])) continue;
                        segments[i].Vertices.Add(vertex);
                    }
                    segments.RemoveAt(i + 1);
                    //Set the for loop one step back, to check the "new" CurvePart with its follower
                    if (i >= 0)
                        i = i - 1;
                }
            }
            FixSegments(segments);

            part.CurveSegments = segments;
        }

        //Remove redundant points and set the last point of the last segment in each part to the parts starting point
        public static void FixSegments(List<CurveSegment> segments)
        {
            var firstPoint = segments.First().Vertices.First();

            foreach (var seg in segments)
            {
                seg.Vertices.Remove(seg.Vertices.First());
            }

            if (firstPoint == (segments.LastItem().Vertices.LastItem())) return;

            var lastSegmentinPart = new LinearSegment { Vertices = new List<float3>() };
            lastSegmentinPart.Vertices.Add(firstPoint);
            segments.Add(lastSegmentinPart);

        }
    }
}
