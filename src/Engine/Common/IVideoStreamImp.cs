namespace Fusee.Engine
{
    /// <summary>
    /// The interface for all VideoStream implementations.
    /// This interface should contain all functions to control the playback of a video.
    /// </summary>
    public interface IVideoStreamImp
    {
        ImageData GetCurrentFrame();
        void Start();
        void Stop();
        
    }
}
