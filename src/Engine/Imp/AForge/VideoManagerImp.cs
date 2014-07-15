using AForge.Video.FFMPEG;
using AForge.Video.DirectShow;

namespace Fusee.Engine
{
    public class VideoManagerImp : IVideoManagerImp
    {
        private VideoFileSource _source;
        private VideoCaptureDevice _videoCaptureDevice;

        //TODO VideoStreamFromFile, VideoStreamFromWebCam
        public IVideoStreamImp CreateVideoStreamImpFromFile(string filename)
        {          
                _source = new VideoFileSource(filename);
                VideoStreamImp stream = new VideoStreamImp(_source);
                return (IVideoStreamImp) stream;
        }

        public  IVideoStreamImp CreateVideoStreamImpFromCamera ()
        {
            FilterInfoCollection videosources = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _videoCaptureDevice = new VideoCaptureDevice(videosources[0].MonikerString);
            return new VideoStreamImp(_videoCaptureDevice);
        }
    }
}
