using AForge.Video.FFMPEG;
using AForge.Video.DirectShow;

namespace Fusee.Engine
{
    public class VideoManagerImp : IVideoManagerImp
    {
        private VideoFileSource _source;
        private VideoCaptureDevice _videoCaptureDevice;
        private VideoStreamImp _stream;


        //TODO VideoStreamFromFile, VideoStreamFromWebCam
        public IVideoStreamImp CreateVideoStreamImpFromFile(string filename, bool loopVideo, bool useAudio)
        {          
                _source = new VideoFileSource(filename);
                return _stream = new VideoStreamImp(_source, loopVideo, useAudio);

        }

        public  IVideoStreamImp CreateVideoStreamImpFromCamera (int cameraIndex, bool useAudio)
        {
            FilterInfoCollection videosources = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _videoCaptureDevice = new VideoCaptureDevice(videosources[cameraIndex].MonikerString);
            return _stream = new VideoStreamImp(_videoCaptureDevice, useAudio);
        }
    }
}
