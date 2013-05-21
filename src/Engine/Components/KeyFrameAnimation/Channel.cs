using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Math;
using Fusse.KeyFrameAnimation;


namespace Fusee.KeyFrameAnimation
{
    public class Channel<TValue> : ChannelBase
    {

        public delegate TValue LerpFunc<TValue>(TValue firstVal, TValue secondVal, float time1, float time2);
        public delegate void SetChanelValue(TValue  val);
        public event SetChanelValue TimeChanged;

        private SortedList<float, Keyframe<TValue>> _timeline = new SortedList<float, Keyframe<TValue>>();
        private LerpFunc<TValue> _lerpIt;


        public Channel()
        {
        }

        public Channel(SetChanelValue timeChanged, LerpFunc<TValue> lerpFunc)
        {
            TimeChanged += timeChanged;
            _lerpIt = lerpFunc;
        }

        public Channel(LerpFunc<TValue> lerpFunc)
        {
            _lerpIt = lerpFunc;
        }

        protected override void DoTick(float time)
        {
            if (TimeChanged != null)
            {
                TValue  currentValue = GetValueAt(time);
                TimeChanged(currentValue);
            }
        }

        
        //Add Keyframes 
        public void AddKeyframe(Keyframe<TValue> keyframe)
        {
            
            if (_timeline.ContainsKey(keyframe.Time))
            {
                RemoveKeyframe(keyframe.Time);
            }
            _timeline.Add(keyframe.Time, keyframe);
        }

        //Maybe we schould place here the current values of the animated Object as a startvalue, therefore we need them first.
        public void AddKeyframe(float time, TValue value)
        {

            if (_timeline.ContainsKey(time))
            {
                RemoveKeyframe(time);
            }
            _timeline.Add(time, new Keyframe<TValue>(time, value));
            
        }

        //Remove KEyframes 
        public void RemoveKeyframe(float time)
        {
            _timeline.Remove(time);
        }

        //Returns the value of a keyframe at a specific time
        public TValue GetValueAt(float time)
        {
            TValue keyValue;
           
            if (_timeline != null && _timeline.Count > 1)
            {
                keyValue = _timeline.ElementAt(0).Value.Value;

                for (int next = 1; next < _timeline.Count; next++)
                {
                    if (_timeline.ElementAt(next).Key > time && _timeline.ElementAt(next - 1).Key < time)
                    {
                        keyValue = _lerpIt(_timeline.ElementAt(next - 1).Value.Value, _timeline.ElementAt(next).Value.Value, _timeline.ElementAt(next).Value.Time - _timeline.ElementAt(next - 1).Value.Time, time - _timeline.ElementAt(next - 1).Value.Time );

                        break;
                    }
                }
            }
            else 
            {
                keyValue = _timeline.ElementAt(0).Value.Value;
            }

            return keyValue;
        }
    }
}