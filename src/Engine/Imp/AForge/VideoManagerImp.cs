using AForge.Video.FFMPEG;
using AForge.Video.DirectShow;

namespace Fusee.Engine
{
    /// <summary>
    /// This class provides all functions necessary to load video from a local file
    /// or an attached camera
    /// </summary>
    public class VideoManagerImp : IVideoManagerImp
    {
        private VideoFileSource _source;
        private VideoCaptureDevice _videoCaptureDevice;
        private VideoStreamImp _stream;

        
        /// <summary>
        /// Creates a video stream from a local video file.
        /// </summary>
        /// <param name="filename">The path to the video file.</param>
        /// <param name="loopVideo">Defines whether the video sould be looped.</param>
        /// <param name="useAudio">Defines whether the video should be played with sound. Note that this option only affects the web build because at the moment
        /// videos with sound are not supported yet in the native build.</param>
        /// <returns>An <see cref="IVideoStreamImp"/>.</returns>
        /// <remarks>If you want to create a web-build, please pay attention to the video-codec and format as not all
        /// formats are supported in HTML5. The recommended format is .webm as this container is supported in all common browsers.</remarks>
        public IVideoStreamImp CreateVideoStreamImpFromFile(string filename, bool loopVideo, bool useAudio)
        {          
                _source = new VideoFileSource(filename);
                return _stream = new VideoStreamImp(_source, loopVideo, useAudio);

        }

       /// <summary>
        /// Creates a video stream from an attached camera.
       /// </summary>
       /// <param name="cameraIndex">Specifies which camera should be used if multiple cameras are available</param>
        /// <param name="useAudio">Defines whether the video should be played with sound. Note that this option only affects the web build because at the moment
        /// videos with sound are not supported yet in the native build.</param>
        /// <returns>An <see cref="IVideoStreamImp"/>.</returns>
        /// <remarks>If you want to use thr audio input, please be aware that this might cause accusstic feedback if your camera has an integrated microphone.</remarks>
        public  IVideoStreamImp CreateVideoStreamImpFromCamera (int cameraIndex, bool useAudio)
        {
            FilterInfoCollection videosources = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _videoCaptureDevice = new VideoCaptureDevice(videosources[cameraIndex].MonikerString);
            return _stream = new VideoStreamImp(_videoCaptureDevice, useAudio);
        }
    }
}
