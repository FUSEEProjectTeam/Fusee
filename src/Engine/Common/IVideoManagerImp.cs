namespace Fusee.Engine.Common
{
    /// <summary>
    /// TODO: Write the actual VideoManager implementations.
    /// The interface for VideoManager implementations. This interface should contain all functions
    /// to load a video.
    /// </summary>
    public interface IVideoManagerImp
    {
        /// <summary>
        /// Creates the video stream imp from file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="loopVideo">if set to <c>true</c> [loop video].</param>
        /// <param name="useAudio">if set to <c>true</c> [use audio].</param>
        /// <returns></returns>
        IVideoStreamImp CreateVideoStreamImpFromFile(string filename, bool loopVideo, bool useAudio);
        /// <summary>
        /// Creates the video stream imp from camera.
        /// </summary>
        /// <param name="cameraIndex">Index of the camera.</param>
        /// <param name="useAudio">if set to <c>true</c> [use audio].</param>
        /// <returns></returns>
        IVideoStreamImp CreateVideoStreamImpFromCamera(int cameraIndex, bool useAudio);
    }
}