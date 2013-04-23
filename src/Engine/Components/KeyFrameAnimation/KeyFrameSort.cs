using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;

namespace Fusee.KeyFrameAnimation
{
    public class KeyFrameSort : IComparer<Keyframe>
    {
        public int Compare(Keyframe x, Keyframe y)
        {
            int _time = x.Time.CompareTo(y.Time);
            if(_time == 0)
            {
                return x.Time .CompareTo(y.Time); 
            }

            return _time;
        }
    }
}