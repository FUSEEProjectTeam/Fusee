using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class VideoTexture
    {
        private IVideoTextureImp _videoTextureImp;


        public void CreateVideoTexture(String filename, ITexture iTex, RenderContext renderContext)
        {
            _videoTextureImp.CreateVideoTexture(filename, iTex, (IRenderContextImp)renderContext);
        }
    }
}
