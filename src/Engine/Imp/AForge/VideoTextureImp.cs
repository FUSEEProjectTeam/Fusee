using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video;
using AForge.Video.FFMPEG;


namespace Fusee.Engine
{
    public class VideoTexture : IVideoTextureImp
    {
        private VideoFileSource _source;
        private Bitmap _videoTexture;

        public void CreateVideoTexture(String filename)
        {
            _source = new VideoFileSource(filename);
            _source.Start();
            _source.NewFrame += new AForge.Video.NewFrameEventHandler(SourceNewFrame);
        }



        private void SourceNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            _videoTexture = (Bitmap)eventArgs.Frame.Clone();

        }

        public Bitmap GetNewFrame()
        {
            return _videoTexture;
        }
    }
}
