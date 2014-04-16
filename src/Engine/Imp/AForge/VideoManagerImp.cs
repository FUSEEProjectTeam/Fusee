using AForge.Video.FFMPEG;

namespace Fusee.Engine
{
    public class VideoManagerImp : IVideoManagerImp
    {
        private VideoFileSource _source;

        public IVideoStreamImp CreateVideoStreamImp (string filename)
        {
            _source = new VideoFileSource(filename);
            return new VideoStreamImp(_source);
        }
    }
}
