using System;
using System.Linq.Expressions;
using Fusee.KeyFrameAnimation;

namespace Fusee.KeyFrameAnimation
{
    public class Keyframe<TValue> //where TValue:
    {
        private TValue _value;
        private float _time;

        public Keyframe(float time, TValue value)
        {
            _value = value;
            _time = (float)System.Math.Round(time, 5);
        }

        public float Time
        {
            get { return _time; }
            set
            {
                _time = (float)System.Math.Round(value, 5);
            }
        }


        public TValue Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
