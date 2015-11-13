namespace Fusee.Engine
{
    /// <summary>
    /// The interface for all VideoStream implementations.
    /// This interface should contain all functions to control the playback of a video.
    /// </summary>
    public interface IVideoStreamImp
    {
        /// <summary>
        /// Gets the current frame.
        /// </summary>
        /// <returns></returns>
        ImageData GetCurrentFrame();
        /// <summary>
        /// Starts this stream.
        /// </summary>
        void Start();
        /// <summary>
        /// Stops this stream.
        /// </summary>
        void Stop();
        
    }
}
