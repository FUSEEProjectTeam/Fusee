using System.Collections.Generic;

namespace Fusee.Xirkit
{
    /// <summary>
    /// This is needed to sort the List in the Channel Class.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class ListSort<TValue> : IComparer<Keyframe<TValue>>
    {
        /// <summary>
        /// Compares 2 Objects and shows through the returned value, whether an Object is smaller, bigger or equals the other Objects.
        /// </summary>
        /// <param name="x">The first Object that shall be compared.</param>
        /// <param name="y">The second Object that shall be compared.</param>
        public int Compare(Keyframe<TValue> x, Keyframe<TValue> y)
        {
            int compare = x.Time.CompareTo(y.Time);
            if (compare == 0)
            {
                return x.Time.CompareTo(y.Time);
            }
            return compare;
        }
    }
}