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
        public int GBufferHandle { get; set; } = -1; //TODO: Dispose framebuffer obj if it isn't needed anymore        

        /// <summary>
        /// Handle of the corresponding Depth Buffer (as renderbuffer). Used to dispose the object if it isn't needed anymore.
        /// </summary>
        public int DepthBufferHandle { get; set; } = -1; //TODO: Dispose framebuffer obj if it isn't needed anymore

        public TexRes TextureResolution { get; private set; }

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
            var posTexImageData = new ImageData(ColorFormat.fRGB, (int)TextureResolution, (int)TextureResolution);
            var posTex = new WritableTexture(posTexImageData);            
            RenderTextures[(int)RenderTargetTextures.G_POSITION] = posTex;
        }

        /// <summary>
        /// Generates a albedo and specular (alpha channel) texture.
        /// </summary>       
        public void CreateAlbedoSpecularTex()
        {
            var albedoTexImageData = new ImageData(ColorFormat.RGBA, (int)TextureResolution, (int)TextureResolution);
            var albedoTex = new WritableTexture(albedoTexImageData);
            RenderTextures[(int)RenderTargetTextures.G_ALBEDO_SPECULAR] = albedoTex;
        }

        /// <summary>
        /// Generates a normal texture.
        /// </summary>
        public void CreateNormalTex()
        {
            var normalTexImageData = new ImageData(ColorFormat.fRGB, (int)TextureResolution, (int)TextureResolution);
            var normalTex = new WritableTexture(normalTexImageData);
            RenderTextures[(int)RenderTargetTextures.G_NORMAL] = normalTex;
        }

        /// <summary>
        /// Generates a depth texture.
        /// </summary>
        public void CreateDepthTex()
        {
            var depthTexImageData = new ImageData(ColorFormat.RGB, (int)TextureResolution, (int)TextureResolution);
            var depthTex = new WritableTexture(depthTexImageData);
            RenderTextures[(int)RenderTargetTextures.G_DEPTH] = depthTex;
        }

        /// <summary>
        /// Generates a ssao texture.
        /// </summary>        
        //public void CreateStencilTex()
        //{
        //    var stenciltexImageData = new ImageData(ColorFormat.Stencil, (int)TextureResolution, (int)TextureResolution);
        //    var ssaoTex = new WritableTexture(stenciltexImageData);
        //    RenderTextures[(int)RenderTargetTextures.G_SSAO] = ssaoTex;
        //}


        /// <summary>
        /// Generates a ssao texture.
        /// </summary>        
        public void CreateSSAOTex()
        {
            var ssaoTexImageData = new ImageData(ColorFormat.RGB, (int)TextureResolution, (int)TextureResolution);
            var ssaoTex = new WritableTexture(ssaoTexImageData);
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
