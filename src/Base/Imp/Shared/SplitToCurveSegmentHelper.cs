using System.Collections.Generic;
using System.Linq;
using Fusee.Math.Core;

#if PLATFORM_DESKTOP
using Fusee.Base.Imp.Desktop.ExtentionMethodes;
namespace Fusee.Base.Imp.Desktop
#elif PLATFORM_WEB
using Fusee.Base.Imp.Web.ExtentionMethodes;
namespace Fusee.Base.Imp.Web
#endif
{
    static class SplitToCurveSegmentHelper
    {
        public static List<CurveSegment> SplitPartIntoSegments(CurvePart part)
        {
            byte[] linearPattern = { 1, 1 };
            byte[] conicPattern = { 1, 0, 1 };
            byte[] cubicPattern = { 1, 0, 0, 1 };
            byte[] conicVirtualPattern = { 1, 0, 0, 0 };

            var segments = new List<CurveSegment>();

            for (var i = 0; i < part.VertTags.Count; i++)
            {
                if (part.VertTags.SkipItems(i).TakeItems(linearPattern.Length).IEnumEqual(linearPattern))
                {
                    segments.Add(CreateCurveSegment(part, i, linearPattern, InterpolationMethod.LINEAR));
                }
                else if (part.VertTags.SkipItems(i).TakeItems(conicPattern.Length).IEnumEqual(conicPattern))
                {
                    segments.Add(CreateCurveSegment(part, i, conicPattern, InterpolationMethod.BEZIER_CONIC));
                    i = i + 1;
                }
                else if (part.VertTags.SkipItems(i).TakeItems(cubicPattern.Length).IEnumEqual(cubicPattern))
                {
                    segments.Add(CreateCurveSegment(part, i, cubicPattern, InterpolationMethod.BEZIER_CUBIC));
                    i = i + 2;
                }
                else if (part.VertTags.SkipItems(i).TakeItems(conicVirtualPattern.Length).IEnumEqual(conicVirtualPattern))
                {
                    var count = 0;
                    var cs = CreateCurveSegment(part, i, conicVirtualPattern, InterpolationMethod.BEZIER_CONIC);

                    i = i + 3;

                    for (var j = i + 1; j < part.VertTags.Count; j++)
                    {
                        cs.Vertices.Add(part.Vertices[j]);
                        if (part.VertTags[j].Equals(0)) continue;
                        count = j;
                        break;
                    }

                    //If count = 0 its the last segment of the curve. We need to add the first vertice in the list to the segment and leave i as it is. 
                    if (!count.Equals(0)) i = count - 1;
                    else cs.Vertices.Add(part.Vertices[0]);

                    //Create "on" points between two successive 0's
                    CreateOnPointsAndAddToList(cs.Vertices);

                    segments.Add(cs);
                }
                else
                {
                    //Only needed for "closed" CurveParts (like letters always are)
                    var lastSegment = new List<byte>();
                    lastSegment.AddRange(part.VertTags.SkipItems(i).TakeItems(part.VertTags.Count - i));
                    lastSegment.Add(part.VertTags[0]);
                    if (lastSegment.IEnumEqual(conicPattern))
                    {
                        segments.Add(CreateCurveSegment(part, i, conicPattern, InterpolationMethod.BEZIER_CONIC, part.Vertices[0]));
                    }
                    else if (lastSegment.IEnumEqual(cubicPattern))
                    {
                        segments.Add(CreateCurveSegment(part, i, cubicPattern, InterpolationMethod.BEZIER_CUBIC, part.Vertices[0]));
                    }
                    else if (lastSegment.IEnumEqual(conicVirtualPattern))
                    {
                        segments.Add(CreateCurveSegment(part, i, cubicPattern, InterpolationMethod.BEZIER_CUBIC, part.Vertices[0]));
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
                    //Insert at every second index (2,4,6 ...)
                    vertices.Insert((j + 1) * 2, vPoint);
                }
            }
        }

        public static CurveSegment CreateCurveSegment(CurvePart cp, int i, byte[] pattern, InterpolationMethod methode)
        {
            var segmentVerts = new List<float3>();
            segmentVerts.AddRange(cp.Vertices.SkipItems(i).TakeItems(pattern.Length));
            var cs = new CurveSegment
            {
                Interpolation = methode,
                Vertices = new List<float3>()
            };
            cs.Vertices = segmentVerts;
            return cs;
        }

        public static CurveSegment CreateCurveSegment(CurvePart cp, int i, byte[] pattern, InterpolationMethod methode, float3 startPoint)
        {
            var segmentVerts = new List<float3>();
            segmentVerts.AddRange(cp.Vertices.SkipItems(i).TakeItems(pattern.Length));
            segmentVerts.Add(startPoint);
            var cs = new CurveSegment
            {
                Interpolation = methode,
                Vertices = new List<float3>()
            };
            cs.Vertices = segmentVerts;
            return cs;
        }

        public static void CombineCurveSegmentsAndAddThemToCurvePart(List<CurveSegment> segments, CurvePart part)
        {
            //Combine segments that follow each other and have the same interpolation methode.
            for (var i = 0; i < segments.Count; i++)
            {
                //Constraint
                if (i + 1 >= segments.Count) break;

                //Check whether two successive segments have the same interpolation Methode, if so combine them.
                if (segments[i].Interpolation.Equals(segments[i + 1].Interpolation))
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
            part.CurveSegments = segments; //TODO: decide whether to delete duplicate points or just do not draw them
        }
    }

    namespace ExtentionMethodes
    {
        /// <summary>
        /// Extention methodes for IEnumerable, replacing LINQs Skip, Take, Last and SequenceEqual methodes.
        /// </summary>
        public static class CustomExtentions
        {
            /// <summary>
            /// Bypasses a given number of elements in a sequence and returns the remaining. Alternative to LINQs Skip().
            /// </summary>
            /// <param name="data"></param>
            /// <param name="count"></param>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public static IEnumerable<T> SkipItems<T>(this IEnumerable<T> data, int count)
            {
                var zwerg = new List<T>();
                zwerg.AddRange((List<T>)data);
                var i = 0;

                foreach (var t in data)
                {
                    if (i < count)
                    {
                        zwerg.RemoveAt(0);
                    }
                    else break;
                    i++;
                }
                return zwerg;
            }

            /// <summary>
            /// Returns a specified number of contiguous elements from the start of a sequence. Alternative to LINQs Take().
            /// </summary>
            /// <param name="data"></param>
            /// <param name="count"></param>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public static IEnumerable<T> TakeItems<T>(this IEnumerable<T> data, int count)
            {
                var zwerg = new List<T>();
                var i = 0;

                foreach (var t in data)
                {
                    if (i < count)
                    {
                        zwerg.Add(t);
                    }
                    else break;
                    i++;
                }
                return zwerg;
            }

            /// <summary>
            /// Returns the last item of a sequence. Alternative to LINQs Last()
            /// </summary>
            /// <param name="data"></param>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public static T LastItem<T>(this IEnumerable<T> data)
            {
                var temp = default(T);
                var iEnum = data.GetEnumerator();
                while (iEnum.MoveNext())
                {
                    temp = iEnum.Current;
                }

                return temp;
            }

            /// <summary>
            /// Determines whether two sequences are equal by comparing the elements. Alternative to LINQs SequenceEqual().
            /// </summary>
            /// <param name="source"></param>
            /// <param name="compObj"></param>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public static bool IEnumEqual<T>(this IEnumerable<T> source, IEnumerable<T> compObj)
            {
                var enum1 = source.GetEnumerator();
                var enum2 = compObj.GetEnumerator();
                var count1 = 0;
                var count2 = 0;

                while (enum1.MoveNext())
                {
                    count1++;
                }
                enum1.Reset();
                while (enum2.MoveNext())
                {
                    count2++;
                }
                enum2.Reset();

                if (count1 != count2) return false;

                while (enum1.MoveNext() && enum2.MoveNext())
                {
                    if (!enum1.Current.Equals(enum2.Current))
                        return false;
                }
                return true;
            }
        }
    }
}
