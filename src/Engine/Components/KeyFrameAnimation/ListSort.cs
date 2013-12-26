using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.KeyFrameAnimation;

namespace Fusse.KeyFrameAnimation
{
    public class ListSort<TValue> : IComparer<Keyframe<TValue>>
    {


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
