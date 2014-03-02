using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.KeyFrameAnimation
{
    /// <summary>
    /// This class is the Channel that stores Keyframes of any Type.
    /// </summary>
    /// <typeparam name="TValue">The type of Keyframes that the channel can store.</typeparam>
    public class Channel<TValue> : ChannelBase
    {

        /// <summary>
        /// A function that returns a generic Type. The Functions can be seen in Lerp.cs
        /// </summary>
        /// <typeparam name="TValue">The type of the Keyframe.</typeparam>
        /// <param name="firstVal">The first value.</param>
        /// <param name="secondVal">The second value.</param>
        /// <param name="time1">The time1.</param>
        /// <param name="time2">The time2.</param>
        /// <returns></returns>
        public delegate TValue LerpFunc<TValue>(TValue firstVal, TValue secondVal, float time1, float time2);
        /// <summary>
        /// A delegate function for setting a value.
        /// </summary>
        /// <param name="val">The value that will be set.</param>
        public delegate void SetChanelValue(TValue val);
        /// <summary>
        /// Occurs when [TimeChanged].
        /// </summary>
        public event SetChanelValue TimeChanged;
        /// <summary>
        /// The _timeline contains a List of Keyframes.
        /// </summary>
        private List<Keyframe<TValue>> _timeline = new List<Keyframe<TValue>>();
        /// <summary>
        /// The _lerpit
        /// </summary>
        private LerpFunc<TValue> _lerpIt;

        /// <summary>
        /// The comparer is needed for sorting the _timeline.
        /// </summary>
        IComparer<Keyframe<TValue>> comparer = new ListSort<TValue>();


        /// <summary>
        /// The _value at a certain time in the channel
        /// </summary>
        private TValue _value;
        /// <summary>
        /// Initializes a new instance of the <see cref="Channel{TValue}"/> class. Adds one defaul Keyframe and the right Lerpfunction.
        /// </summary>
        /// <param name="timeChanged">The time changed.</param>
        /// <param name="lerpFunc">The lerp function.</param>
        public Channel(SetChanelValue timeChanged, LerpFunc<TValue> lerpFunc)
        {
            TimeChanged += timeChanged;
            _lerpIt = lerpFunc;
            AddKeyframe(0, default(TValue));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Channel{TValue}"/> class. Adds the right Lerpfunction.
        /// </summary>
        /// <param name="lerpFunc">The right lerpfunction.</param>
        public Channel(LerpFunc<TValue> lerpFunc)
        {
            _lerpIt = lerpFunc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Channel{TValue}"/> class. Adds a Keyframe with a specific value and the right lerpFunction.
        /// </summary>
        /// <param name="lerpFunc">The right lerpfunction.</param>
        /// <param name="value">The value of the firs keyframe.</param>
        public Channel(LerpFunc<TValue> lerpFunc, TValue value)
        {
            _lerpIt = lerpFunc;
            AddKeyframe(0, value);
        }


        /// <summary>
        /// Overrides the Base Channel Class.
        /// </summary>
        /// <param name="time">The time.</param>
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

        /// <summary>
        /// Set's the maximum time of the channel in the baseclass if theres no keyframe the time is 0.
        /// </summary>
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
        /// <summary>
        /// Adds a keyframe to the channel.
        /// </summary>
        /// <param name="keyframe">The keyframe.</param>
        public void AddKeyframe(Keyframe<TValue> keyframe)
        {

            if (ContainsKey(keyframe))
            {
                RemoveKeyframe(keyframe.Time);
            }
            _timeline.Add(keyframe);
            _timeline.Sort(comparer);
        }

        /// <summary>
        /// Creates a new Keyframe and add's him to the channel.
        /// </summary>
        /// <param name="time">The time of the new keyframe.</param>
        /// <param name="value">The value of the new keyframe.</param>
        public void AddKeyframe(float time, TValue value)
        {

            if (ContainsKey(time))
            {
                RemoveKeyframe(time);
            }
            _timeline.Add(new Keyframe<TValue>(time, value));
            _timeline.Sort(comparer);

        }

        /// <summary>
        /// Removes a keyframe at a speciffic time (time = key).
        /// </summary>
        /// <param name="time">The time of the keyframe that has to be removed.</param>
        public void RemoveKeyframe(float time)
        {
            for (int i = 0; i < _timeline.Count; i++)
            {
                if(_timeline[i].Time == time)
                _timeline.RemoveAt(i);
                _timeline.Sort(comparer);
            }
            
        }

        /// <summary>
        /// Returns the value of a keyframe at a specific time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>The calculated value</returns>
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

        /// <summary>
        /// Determines whether the channel contains a keyframe that has the same key.
        /// </summary>
        /// <param name="keyframe">The keyframe that will be tested.</param>
        /// <returns>true = there is a keyframe with the same key | false = there no keyframe with the same key</returns>
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

        /// <summary>
        /// Determines whether the channel contains a keyframe that has the same time.
        /// </summary>
        /// <param name="time">The time that will be tested.</param>
        /// <returns>true = there is a keyframe with the same key | false = there no keyframe with the same key</returns>
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


        /// <summary>
        /// Gets the last calculated value.
        /// </summary>
        public TValue Value { get { return _value; } }
    }
}