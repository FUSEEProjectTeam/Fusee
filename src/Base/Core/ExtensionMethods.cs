using System.Collections.Generic;
using Fusee.Math.Core;

namespace Fusee.Base.Core
{
    /// <summary>
    /// A set of extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        #region IEnumerable extension methods

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
            var items = new List<T>();
            var i = 0;

            foreach (var t in data)
            {
                if (i < count)
                {
                    items.Add(t);
                }
                else break;
                i++;
            }
            return items;
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
        public static bool SequEqual<T>(this IEnumerable<T> source, IEnumerable<T> compObj)
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
        #endregion

        #region IList extension methodes

        /// <summary>
        /// Checks whether a polygon (parallel to the xy plane) has a ccw winding.
        /// </summary>
        /// <param name="source">The polygon, represented as list of float3</param>
        /// <param name="closed">Determines if the polygon is closed</param>
        /// <returns></returns>
        public static bool IsCounterClockwise(this IList<float3> source)
        {
            var sum = 0f;
            for (var i = 0; i < source.Count; i++)
            {
                var current = new float2(source[i].x, source[i].y);

                var next = new float2(source[(i + 1) % source.Count].x, source[(i + 1) % source.Count].y);

                sum += (next.x - current.x)*(next.y + current.y);
            }

            return sum < 0;
        }

        #endregion

    }
}
