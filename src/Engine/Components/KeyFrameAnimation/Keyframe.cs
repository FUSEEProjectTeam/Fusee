namespace Fusee.KeyFrameAnimation
{
    /// <summary>
    /// This Class represents a Keyframe it has a time and a Value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class Keyframe<TValue>
    {
        private TValue _value;
        private float _time;

        /// <summary>
        /// Initializes a new instance of the <see cref="Keyframe{TValue}"/> class.
        /// </summary>
        /// <param name="time">The time of the keyframe.</param>
        /// <param name="value">The value of the keyframe.</param>
        public Keyframe(float time, TValue value)
        {
            _value = value;
            _time = (float)System.Math.Round(time, 5);
        }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The new time.
        /// </value>
        public float Time
        {
            get { return _time; }
            set
            {
                _time = (float)System.Math.Round(value, 5);
            }
        }


        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The new value.
        /// </value>
        public TValue Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
