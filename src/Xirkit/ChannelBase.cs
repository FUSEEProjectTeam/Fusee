namespace Fusee.Xirkit
{
    /// <summary>
    /// Base class containing functionality common to all Channel types, no matter 
    /// of the type of the values of the containing key frames.
    /// </summary>
    public abstract class ChannelBase
    {
        /// <summary>
        /// The _time in the BaseChannel
        /// </summary>
        private float _time;

        /// <summary>
        /// Gets and sets the time of the Channel.
        /// </summary>
        public float Time { get { return _time; } /* set { _time = value; } */  }

        /// <summary>
        /// Increments the current time and advances between the best fitting keyframes.
        /// </summary>
        /// <param name="time">The time to advance this channel about.</param>
        public abstract void SetTick(float time);

        /// <summary>
        /// Retrieves the overall duration of this channel.
        /// </summary>
        public abstract float Duration { get; }
    }
}