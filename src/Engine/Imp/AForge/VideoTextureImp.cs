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
        private ITexture _iTex;
        private IRenderContextImp _renderContextImp;

        public void CreateVideoTexture(String filename, ITexture iTex, IRenderContextImp renderContext)
        {
            _source = new VideoFileSource(filename);
            _source.Start();
            _source.NewFrame += SourceNewFrame;
            _iTex = iTex;
            _renderContextImp = renderContext;
        }



        private void SourceNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            ImageData imgData = _renderContextImp.LoadVideoTexture(bitmap);
            _iTex = _renderContextImp.CreateTexture(imgData);

        }
    }
}
