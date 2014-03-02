using System.Collections.Generic;

namespace Fusee.KeyFrameAnimation
{
    /// <summary>
    /// This is needed to sort the List in the Channel Class.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class ListSort<TValue> : IComparer<Keyframe<TValue>>
    {


        /// <summary>
        /// Compares 2 Objekts and shows through the returned value, wheater an Objekt is smaller, bigger or equals the other Objekts.
        /// </summary>
        /// <param name="x">The first Objekt that shall be compared.</param>
        /// <param name="y">The second Objekt that shall be compared.</param>
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
