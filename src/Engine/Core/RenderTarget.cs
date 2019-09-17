using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using System;

namespace Fusee.Engine.Core
{
    
    
    /// <summary>
    /// Render to a target if you want to render to a texture and use them in an other pass.
    /// Use the "Create__Tex"-Methods to generate the textures you need. The order of the textures in the RenderTextures array is given by the <see cref="RenderTargetTextures"/> enum.
    /// </summary>
    public class RenderTarget : IRenderTarget
    {
        ///Order of textures in RenderTextures array is given by the corresponding enum.
        public IWritableTexture[] RenderTextures { get; private set; }

        /// <summary>
        /// Handle of the corresponding G-Buffer. Used to dispose the object if it isn't needed anymore.
        /// </summary>
        public IBufferHandle GBufferHandle { get; set; } //TODO: Dispose framebuffer obj if it isn't needed anymore        

        /// <summary>
        /// Handle of the corresponding Depth Buffer (as renderbuffer). Used to dispose the object if it isn't needed anymore.
        /// </summary>
        public IBufferHandle DepthBufferHandle { get; set; } //TODO: Dispose framebuffer obj if it isn't needed anymore

        /// <summary>
        /// Sets the resolution of the render textures.
        /// </summary>
        public TexRes TextureResolution { get; private set; }

        /// <summary>
        /// Sets a RenderTexture from another RenderTarget at the correct position in the RenderTexure array.
        /// </summary>
        /// <param name="src">The source RenderTarget.</param>
        /// <param name="tex">The type of the texture.</param>
        public void SetTextureFromRenderTarget(RenderTarget src, RenderTargetTextures tex)
        {
            var srcTex = src.RenderTextures[(int)tex];
            RenderTextures[(int)tex] = srcTex ?? throw new ArgumentException("Texture from source target is null!");
        }

        /// <summary>
        /// Creates a new instance of type "RenderTarget".
        /// </summary>
        /// <param name="texRes">Resolution of the created Textures.</param>
        public RenderTarget(TexRes texRes)
        {
            RenderTextures = new WritableTexture[Enum.GetNames(typeof(RenderTargetTextures)).Length];
            TextureResolution = texRes;
        }

        /// <summary>
        /// Generates a position texture.
        /// </summary>
        public void CreatePositionTex()
        {            
            var posTex = new WritableTexture(new ImagePixelFormat(ColorFormat.fRGB32), (int)TextureResolution, (int)TextureResolution, false, TextureFilterMode.NEAREST);            
            RenderTextures[(int)RenderTargetTextures.G_POSITION] = posTex;
        }

        /// <summary>
        /// Generates a albedo and specular (alpha channel) texture.
        /// </summary>       
        public void CreateAlbedoSpecularTex()
        {            
            var albedoTex = new WritableTexture(new ImagePixelFormat(ColorFormat.RGBA), (int)TextureResolution, (int)TextureResolution, false);
            RenderTextures[(int)RenderTargetTextures.G_ALBEDO_SPECULAR] = albedoTex;
        }

        /// <summary>
        /// Generates a normal texture.
        /// </summary>
        public void CreateNormalTex()
        {          
            var normalTex = new WritableTexture(new ImagePixelFormat(ColorFormat.fRGB16), (int)TextureResolution, (int)TextureResolution, false, TextureFilterMode.NEAREST);
            RenderTextures[(int)RenderTargetTextures.G_NORMAL] = normalTex;
        }

        /// <summary>
        /// Generates a depth texture.
        /// </summary>
        public void CreateDepthTex()
        {           
            var depthTex = new WritableTexture(new ImagePixelFormat(ColorFormat.fRGB16), (int)TextureResolution, (int)TextureResolution, false, TextureFilterMode.NEAREST);
            RenderTextures[(int)RenderTargetTextures.G_DEPTH] = depthTex;
        }

        /// <summary>
        /// Generates a ssao texture.
        /// </summary>        
        public void CreateSSAOTex()
        {           
            var ssaoTex = new WritableTexture(new ImagePixelFormat(ColorFormat.fRGB16), (int)TextureResolution, (int)TextureResolution, false, TextureFilterMode.NEAREST);
            RenderTextures[(int)RenderTargetTextures.G_SSAO] = ssaoTex;
        }

        //public void Dispose()
        //{
        //    Dispose(true);
        //    // This object will be cleaned up by the Dispose method.
        //    // Therefore, you should call GC.SupressFinalize to
        //    // take this object off the finalization queue
        //    // and prevent finalization code for this object
        //    // from executing a second time.
        //    GC.SuppressFinalize(this);
        //}

        //// Dispose(bool disposing) executes in two distinct scenarios.
        //// If disposing equals true, the method has been called directly
        //// or indirectly by a user's code. Managed and unmanaged resources
        //// can be disposed.
        //// If disposing equals false, the method has been called by the
        //// runtime from inside the finalizer and you should not reference
        //// other objects. Only unmanaged resources can be disposed.
        //protected virtual void Dispose(bool disposing)
        //{
        //    // Check to see if Dispose has already been called.
        //    if (!disposed)
        //    {
        //        //foreach (var tex in RenderTextures)
        //        //{
        //        //    if (tex == null) continue;
        //        //    ((WritableTexture)tex).Dispose();
        //        //}

        //        // Note disposing has been done.
        //        disposed = true;

        //    }
        //}


        //~RenderTarget()
        //{
        //    // Do not re-create Dispose clean-up code here.
        //    // Calling Dispose(false) is optimal in terms of
        //    // readability and maintainability.
        //    Dispose(false);
        //}

    }
}
