namespace Fusee.KeyFrameAnimation
{
    /// <summary>
    /// 
    /// </summary>
    public class ChannelBase
    {
        /// <summary>
        /// The _time in the BaseChannel
        /// </summary>
        private float _time;

        /// <summary>
        /// Get's or set's the time of the Channel.
        /// </summary>
        public float Time { get { return _time; } set { _time = value; } }

        protected virtual void DoTick(float time)
        {

        }

        protected virtual void DemandTime()
        {

        }


        /// <summary>
        /// Executes DoTick.
        /// </summary>
        /// <param name="time">The time will be pasted to DoTick.</param>
        public void SetTick(float time)
        {
            DoTick(time);
        }
        /// <summary>
        /// Executes DemandTime.
        /// </summary>
        public void NeedTime()
        {
            DemandTime();
        }
    }
}
