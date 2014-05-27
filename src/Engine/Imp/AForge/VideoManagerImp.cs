using AForge.Video.FFMPEG;
using AForge.Video.DirectShow;

namespace Fusee.Engine
{
    public class VideoManagerImp : IVideoManagerImp
    {
        private VideoFileSource _source;
        private VideoCaptureDevice _videoCaptureDevice;

        public IVideoStreamImp CreateVideoStreamImp(string filename)
        {
            if (!filename.Equals("webcam"))
            {
                _source = new VideoFileSource(filename);
                return new VideoStreamImp(_source);
            }

            FilterInfoCollection videosources = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _videoCaptureDevice = new VideoCaptureDevice(videosources[0].MonikerString);
            return new VideoStreamImp(_videoCaptureDevice);

        }
    }
}
