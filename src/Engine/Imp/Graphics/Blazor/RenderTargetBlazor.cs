using Fusee.Base.Common;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Imp.SharedAll;
using System;

namespace Fusee.Engine.Imp.Graphics.Blazor
{
    /// <summary>
    /// Provides Blazor-specific overrides to the <see cref="RenderTarget"/>s Set[..]Tex() methods.
    /// </summary>
    public class RenderTargetBlazor : RenderTarget
    {
        public RenderTargetBlazor(TexRes texRes) : base(texRes)
        {
        }

        /// <summary>
        /// Sets a RenderTexture into the correct position in the RederTexture array.
        /// </summary>
        /// <param name="src">The source RenderTexture.</param>
        /// <param name="tex">The type of the texture.</param>
        public override void SetTexture(IWritableTexture src, RenderTargetTextureTypes tex)
        {
            RenderTextures[(int)tex] = src ?? throw new ArgumentException("Texture from source target is null!");
        }

        /// <summary>
        /// Generates a position texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        public override void SetPositionTex()
        {
            RenderTextures[(int)RenderTargetTextureTypes.Position] = WritableTexture.CreatePosTex((int)TextureResolution, (int)TextureResolution, new ImagePixelFormat(ColorFormat.fRGBA32));
        }

        /// <summary>
        /// Generates a albedo and specular (alpha channel) texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>       
        public override void SetAlbedoTex()
        {
            RenderTextures[(int)RenderTargetTextureTypes.Albedo] = WritableTexture.CreateAlbedoTex((int)TextureResolution, (int)TextureResolution, new ImagePixelFormat(ColorFormat.RGBA));
        }

        /// <summary>
        /// Generates a normal texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        public override void SetNormalTex()
        {
            RenderTextures[(int)RenderTargetTextureTypes.Normal] = WritableTexture.CreateNormalTex((int)TextureResolution, (int)TextureResolution, new ImagePixelFormat(ColorFormat.fRGBA32));
        }

        /// <summary>
        /// Generates a depth texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        public override void SetDepthTex(TextureCompareMode compareMode = TextureCompareMode.None, Compare compareFunc = Compare.Less)
        {
            RenderTextures[(int)RenderTargetTextureTypes.Depth] = WritableTexture.CreateDepthTex((int)TextureResolution, (int)TextureResolution, new ImagePixelFormat(ColorFormat.Depth24), compareMode, compareFunc);
        }

        /// <summary>
        /// Generates a SSAO texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        public override void SetSSAOTex()
        {
            RenderTextures[(int)RenderTargetTextureTypes.Ssao] = WritableTexture.CreateSSAOTex((int)TextureResolution, (int)TextureResolution, new ImagePixelFormat(ColorFormat.fRGBA32));
        }

        /// <summary>
        /// Generates a specular texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        public override void SetSpecularTex()
        {
            RenderTextures[(int)RenderTargetTextureTypes.Specular] = WritableTexture.CreateSpecularTex((int)TextureResolution, (int)TextureResolution, new ImagePixelFormat(ColorFormat.fRGBA32));
        }

        /// <summary>
        /// Generates a emissive texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        public override void SetEmissiveTex()
        {
            RenderTextures[(int)RenderTargetTextureTypes.Emission] = WritableTexture.CreateEmissionTex((int)TextureResolution, (int)TextureResolution, new ImagePixelFormat(ColorFormat.RGBA));
        }

        /// <summary>
        /// Generates a subsurface texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        public override void SetSubsurfaceTex()
        {
            RenderTextures[(int)RenderTargetTextureTypes.Subsurface] = WritableTexture.CreateSubsurfaceTex((int)TextureResolution, (int)TextureResolution, new ImagePixelFormat(ColorFormat.RGBA));
        }
    }
}
