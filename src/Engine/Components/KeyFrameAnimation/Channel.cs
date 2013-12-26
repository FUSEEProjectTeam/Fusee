using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Engine;
using Fusee.Math;
using Fusse.KeyFrameAnimation;


namespace Fusee.KeyFrameAnimation
{
    public class Channel<TValue> : ChannelBase
    {

        public delegate TValue LerpFunc<TValue>(TValue firstVal, TValue secondVal, float time1, float time2);
        public delegate void SetChanelValue(TValue val);
        public event SetChanelValue TimeChanged;
        private List<Keyframe<TValue>> _timeline = new List<Keyframe<TValue>>();
        private LerpFunc<TValue> _lerpIt;

        IComparer<Keyframe<TValue>> comparer = new ListSort<TValue>();


        private TValue _value;
        public Channel(SetChanelValue timeChanged, LerpFunc<TValue> lerpFunc)
        {
            TimeChanged += timeChanged;
            _lerpIt = lerpFunc;
            AddKeyframe(0, default(TValue));
        }

        public Channel(LerpFunc<TValue> lerpFunc)
        {
            _lerpIt = lerpFunc;
        }

        public Channel(LerpFunc<TValue> lerpFunc, TValue value)
        {
            _lerpIt = lerpFunc;
            AddKeyframe(0, value);
        }


        protected override void DoTick(float time)
        {
            if (TimeChanged != null)
            {
                TValue currentValue = GetValueAt(time);
                TimeChanged(currentValue);
            }
            else
            {
                GetValueAt(time);
            }
        }

        protected override void DemandTime()
        {

            try 
            {
                base.Time = _timeline.ElementAt(_timeline.Count - 1).Time;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                base.Time = 0;
            }
           
        }



        //Add Keyframes 
        public void AddKeyframe(Keyframe<TValue> keyframe)
        {

            if (ContainsKey(keyframe))
            {
                RemoveKeyframe(keyframe.Time);
            }
            _timeline.Add(keyframe);
            _timeline.Sort(comparer);
        }

        public void AddKeyframe(float time, TValue value)
        {

            if (ContainsKey(time))
            {
                RemoveKeyframe(time);
            }
            _timeline.Add(new Keyframe<TValue>(time, value));
            _timeline.Sort(comparer);

        }

        //Remove Keyframes 
        public void RemoveKeyframe(float time)
        {
            for (int i = 0; i < _timeline.Count; i++)
            {
                if(_timeline[i].Time == time)
                _timeline.RemoveAt(i);
                _timeline.Sort(comparer);
            }
            
        }


        //Returns the value of a keyframe at a specific time
        public TValue GetValueAt(float time)
        {

            TValue keyValue;
            if (_timeline.Count > 1)
            {
                keyValue = _timeline.ElementAt(0).Value;

                for (int next = 1; next < _timeline.Count; next++)
                {
                    if (_timeline.ElementAt(next).Time > time && _timeline.ElementAt(next - 1).Time < time)
                    {
                        keyValue = _lerpIt(_timeline.ElementAt(next - 1).Value, _timeline.ElementAt(next).Value, _timeline.ElementAt(next).Time - _timeline.ElementAt(next - 1).Time, time - _timeline.ElementAt(next - 1).Time);

                        break;
                    }
                }
            }
            else 
            {
                try
                {
                keyValue = _timeline.ElementAt(0).Value;
                }
                catch(System.ArgumentOutOfRangeException )
                {
                    Console.WriteLine("There are no keyframes in the timeline, a standart value will be set.");
                     keyValue = default(TValue);
                }
            }
            _value = keyValue;
            return keyValue;
        }

        private bool ContainsKey(Keyframe<TValue> keyframe)
        {
            for (int i = 0;i<_timeline.Count;i++)
            {
                if (keyframe.Time == _timeline[i].Time)
                {
                    return true;
                }
            }
            return false;
        }

        private bool ContainsKey(float time)
        {
            for (int i = 0; i < _timeline.Count; i++)
            {
                if (time == _timeline[i].Time)
                {
                    return true;
                }
            }
            return false;
        }


        public TValue Value { get { return _value; } }
    }
}