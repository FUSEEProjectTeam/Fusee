using Fusee.Base.Common;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Implements the video stream.
    /// </summary>
    public class VideoStream
    {
        /// <summary>
        /// The video stream.
        /// </summary>
        public IVideoStreamImp _imp;
        /// <summary>
        /// Returns the current frame.
        /// </summary>
        /// <returns></returns>
        public IImageData GetCurrentFrame()
        {
            return _imp.GetCurrentFrame();
        }
        /// <summary>
        /// Starts the video stream.
        /// </summary>
        public void Start()
        {
            _imp.Start();
        }
        /// <summary>
        /// Stops the video stream.
        /// </summary>
        public void Stop()
        {
            _imp.Stop();
        }

    }
}