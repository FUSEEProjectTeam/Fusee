using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Implements the video manager.
    /// </summary>
    public class VideoManager
    {
        private static VideoManager _instance;
        private IVideoManagerImp _videoManagerImp;

        internal IVideoManagerImp VideoManagerImp
        {
            set { _videoManagerImp = value; }
        }
        /// <summary>
        /// Loads a video file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="loopVideo"></param>
        /// <param name="useAudio"></param>
        /// <returns></returns>
        public IVideoStreamImp LoadVideoFromFile(string filename, bool loopVideo, bool useAudio = true)
        {
            return _videoManagerImp.CreateVideoStreamImpFromFile(filename, loopVideo, useAudio);
        }
        /// <summary>
        /// Loads video directly of a camera.
        /// </summary>
        /// <param name="cameraIndex"></param>
        /// <param name="useAudio"></param>
        /// <returns></returns>
        public IVideoStreamImp LoadVideoFromCamera(int cameraIndex = 0, bool useAudio = false)
        {
            return _videoManagerImp.CreateVideoStreamImpFromCamera(cameraIndex, useAudio);
        }
        /// <summary>
        /// Creates an instance of <see cref="VideoManager"/>.
        /// </summary>
        public static VideoManager Instance
        {
            get { return _instance ?? (_instance = new VideoManager()); }
        }
    }
}