namespace Fusee.Engine.Common
{
    /// <summary>
    /// TODO: Write the actual VideoStream implementations.
    /// The interface for all VideoStream implementations.
    /// This interface contains all functions to control the playback of a video.
    /// </summary>
    public interface IVideoStreamImp
    {
        /// <summary>
        /// Gets the current frame.
        /// </summary>
        /// <returns></returns>
        ITexture GetCurrentFrame();
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