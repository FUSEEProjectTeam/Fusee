using System.Collections.Generic;
using System.Linq;

namespace Fusee.Xirkit
{
    /// <summary>
    /// Generic Channel implementation capable of storing keyframes of the given Type.
    /// </summary>
    /// <typeparam name="TValue">The type of keyframes' values that the channel should store.</typeparam>
    public class Channel<TValue> : ChannelBase
    {

        /// <summary>
        /// A function that returns a generic Type. The Functions can be seen in Lerp.cs
        /// </summary>
        /// <param name="firstVal">The first value.</param>
        /// <param name="secondVal">The second value.</param>
        /// <param name="time1">The time1.</param>
        /// <param name="time2">The time2.</param>
        /// <returns></returns>
        public delegate TValue LerpFunc(TValue firstVal, TValue secondVal, float time1, float time2);
        /// <summary>
        /// A delegate function for setting a value.
        /// </summary>
        /// <param name="val">The value that will be set.</param>
        public delegate void SetChanelValue(TValue val);
        /// <summary>
        /// Occurs when the channel time changed.
        /// </summary>
        public event SetChanelValue TimeChanged;

        // The _timeline contains a List of Keyframes.
        private readonly List<Keyframe<TValue>> _timeline = new();

        private readonly LerpFunc _lerpIt;

        // The comparer is needed for sorting the _timeline.
        readonly IComparer<Keyframe<TValue>> _comparer = new ListSort<TValue>();


        // The _value at a certain time in the channel
        private TValue _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Channel{TValue}"/> class. Adds one default Keyframe and the right Lerpfunction.
        /// </summary>
        /// <param name="timeChanged">The time changed.</param>
        /// <param name="lerpFunc">The lerp function.</param>
        public Channel(SetChanelValue timeChanged, LerpFunc lerpFunc)
        {
            TimeChanged += timeChanged;
            _lerpIt = lerpFunc;
            AddKeyframe(0, default);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Channel{TValue}"/> class. Adds the right Lerpfunction.
        /// </summary>
        /// <param name="lerpFunc">The right lerpfunction.</param>
        public Channel(LerpFunc lerpFunc)
        {
            _lerpIt = lerpFunc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Channel{TValue}"/> class. Adds a Keyframe with a specific value and the right lerpFunction.
        /// </summary>
        /// <param name="lerpFunc">The right lerpfunction.</param>
        /// <param name="value">The value of the firs keyframe.</param>
        public Channel(LerpFunc lerpFunc, TValue value)
        {
            _lerpIt = lerpFunc;
            AddKeyframe(0, value);
        }

        /// <summary>
        /// Increments the current time and advances between the best fitting keyframes.
        /// </summary>
        /// <param name="time">The time to advance this channel about.</param>
        public override void SetTick(float time)
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
        /// Retrieves the overall duration of this channel.
        /// </summary>
        public override float Duration
        {
            get
            {
                return (_timeline.Count > 0) ? _timeline.ElementAt(_timeline.Count - 1).Time : 0.0f;
            }
        }

        /// <summary>
        /// Adds a keyframe to this channel.
        /// </summary>
        /// <param name="keyframe">The keyframe.</param>
        public void AddKeyframe(Keyframe<TValue> keyframe)
        {

            if (ContainsKey(keyframe))
            {
                RemoveKeyframe(keyframe.Time);
            }
            _timeline.Add(keyframe);
            _timeline.Sort(_comparer);
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
            _timeline.Sort(_comparer);

        }

        /// <summary>
        /// Removes a keyframe at a specific time (time = key).
        /// </summary>
        /// <param name="time">The time of the keyframe that has to be removed.</param>
        public void RemoveKeyframe(float time)
        {
            for (int i = 0; i < _timeline.Count; i++)
            {
                if (_timeline[i].Time == time)
                    _timeline.RemoveAt(i);
                _timeline.Sort(_comparer);
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
                    if (_timeline.ElementAt(next - 1).Time <= time && time < _timeline.ElementAt(next).Time)
                    {
                        keyValue = _lerpIt(_timeline.ElementAt(next - 1).Value, _timeline.ElementAt(next).Value, _timeline.ElementAt(next).Time - _timeline.ElementAt(next - 1).Time, time - _timeline.ElementAt(next - 1).Time);
                        break;
                    }
                }
            }
            else
            {
                // Diagnostics.Log("Timeline is empty. Using default value");
                keyValue = default;
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
            for (int i = 0; i < _timeline.Count; i++)
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
            return _timeline.Any(t => time == t.Time);
        }


        /// <summary>
        /// Gets the last calculated value.
        /// </summary>
        public TValue Value { get { return _value; } }
    }
}