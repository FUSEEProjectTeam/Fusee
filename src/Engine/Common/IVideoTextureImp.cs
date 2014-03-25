using System;

namespace Fusee.Engine
{
    public interface IVideoTextureImp
    {
        void CreateVideoTexture (String filename, ITexture iText, IRenderContextImp renderContext);
    }
}
