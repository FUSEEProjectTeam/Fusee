namespace Fusee.Engine
{
    /// <summary>
    /// </summary>
    public interface IAudioStream
    {
        float Volume { set; get; }
        bool Loop { set; get; }

        void Dispose();

        void Play();
        void Pause();
        void Stop();
    }
}