namespace Fusee.Engine
{
    /// <summary>
    /// </summary>
    public interface IAudioStream
    {
        /// <summary>
        /// 
        /// </summary>
        float Volume { set; get; }

        /// <summary>
        /// 
        /// </summary>
        bool Loop { set; get; }

        /// <summary>
        /// 
        /// </summary>
        float Panning { set; get; }

        /// <summary>
        /// 
        /// </summary>
        void Play();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loop"></param>
        void Play(bool loop);

        /// <summary>
        /// 
        /// </summary>
        void Pause();

        /// <summary>
        /// 
        /// </summary>
        void Stop();
    }
}