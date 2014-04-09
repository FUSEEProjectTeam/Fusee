using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class VideoTexture
    {
        private IVideoTextureImp _videoTextureImp;

        internal IVideoTextureImp VideoTextureImp
        {
            set
            {
                _videoTextureImp = value;
            }
        }



        public void CreateVideoTexture(String filename)
        {
            _videoTextureImp.CreateVideoTexture(filename);
        }

        public Bitmap GetNewFrame()
        {
            return _videoTextureImp.GetNewFrame();
        }
    }
}
