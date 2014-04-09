using System;
using System.Drawing;

namespace Fusee.Engine
{
    public interface IVideoTextureImp
    {
        void CreateVideoTexture (String filename);

        Bitmap GetNewFrame();
    }
}
