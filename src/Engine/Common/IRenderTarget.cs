using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Common
{
    public interface IRenderTarget
    {

        int GBufferHandle { get; set; }
        int DepthBufferHandle { get; set; }

        IWritableTexture[] RenderTextures { get; }

        TexRes TextureResolution { get; }
    }
}
