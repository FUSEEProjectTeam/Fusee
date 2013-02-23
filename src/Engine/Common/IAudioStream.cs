namespace Fusee.Engine
{
    /// <summary>
    /// </summary>
    public interface IAudioStream
    {
        float Volume { set; get; }
        bool Loop { set; get; }

        void Dispose();

        void Play(bool loop = false);
        void Pause();
        void Stop();
    }
}