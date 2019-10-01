using System;
using System.Collections.Generic;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Math.Core;
using Fusee.Engine.Common;
using static Fusee.Engine.Imp.Graphics.WebAsm.WebGLRenderingContextBase;
using static Fusee.Engine.Imp.Graphics.WebAsm.WebGL2RenderingContextBase;
using WebAssembly.Core;
using System.Runtime.InteropServices;
using Fusee.Engine.Imp.WebAsm;
using System.Linq;

namespace Fusee.Engine.Imp.Graphics.WebAsm
{
    /// <summary>
    /// Implementation of the <see cref="IRenderContextImp" /> interface on top of WebGLDotNET.
    /// </summary>
    public class RenderContextImp : IRenderContextImp
    {
        #region Fields

        protected WebGLRenderingContextBase gl;
        protected WebGL2RenderingContextBase gl2;

        private int _currentTextureUnit;
        private readonly Dictionary<WebGLUniformLocation, int> _shaderParam2TexUnit;
 
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContextImp"/> class.
        /// </summary>
        /// <param name="renderCanvasImp">The platform specific render canvas implementation.</param>
        public RenderContextImp(IRenderCanvasImp renderCanvasImp)
        {
            _currentTextureUnit = 0;
            _shaderParam2TexUnit = new Dictionary<WebGLUniformLocation, int>();

            gl = ((RenderCanvasImp)renderCanvasImp)._gl;

            // Due to the right-handed nature of OpenGL and the left-handed design of FUSEE
            // the meaning of what's Front and Back of a face simply flips.
            // TODO - implement this in render states!!!

            gl.CullFace(BACK);
        }

        #endregion

        #region Image data related Members

        /// <summary>
        /// Updates a texture with images obtained from a Video.
        /// </summary>
        /// <param name="stream">The Video from which the images are taken.</param>
        /// <param name="tex">The texture to which the ImageData is bound to.</param>
        /// <remarks>Look at the VideoTextureExample for further information.</remarks>
        public void UpdateTextureFromVideoStream(IVideoStreamImp stream, ITextureHandle tex)
        {
            ITexture img = stream.GetCurrentFrame();
            uint pixelFormat;
            switch (img.PixelFormat.ColorFormat)
            {
                case ColorFormat.RGBA:
                    pixelFormat = RGBA;
                    break;
                case ColorFormat.RGB:
                    pixelFormat = RGB;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (img.PixelData != null)
            {
                if (tex == null)
                    tex = CreateTexture(img);

                gl.BindTexture(TEXTURE_2D, ((TextureHandle)tex).Handle);

                var imageData = gl.CastNativeArray(img.PixelData);
                gl.TexSubImage2D(TEXTURE_2D, 0, 0, 0, img.Width, img.Height, pixelFormat, UNSIGNED_BYTE, imageData);
            }
        }

        /// <summary>
        /// Updates a specific rectangle of a texture.
        /// </summary>
        /// <param name="tex">The texture to which the ImageData is bound to.</param>
        /// <param name="img">The ImageData-Struct containing information about the image. </param>
        /// <param name="startX">The x-value of the upper left corner of th rectangle.</param>
        /// <param name="startY">The y-value of the upper left corner of th rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <remarks> /// <remarks>Look at the VideoTextureExample for further information.</remarks></remarks>
        public void UpdateTextureRegion(ITextureHandle tex, ITexture img, int startX, int startY, int width, int height)
        {
            uint pixelFormat;
            switch (img.PixelFormat.ColorFormat)
            {
                case ColorFormat.RGBA:
                    pixelFormat = RGBA;
                    break;
                case ColorFormat.RGB:
                    pixelFormat = RGB;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // copy the bytes from img to GPU texture
            int bytesTotal = width * height * img.PixelFormat.BytesPerPixel;
            var scanlines = img.ScanLines(startX, startY, width, height);
            byte[] bytes = new byte[bytesTotal];
            int offset = 0;
            do
            {
                if (scanlines.Current != null)
                {
                    var lineBytes = scanlines.Current.GetScanLineBytes();
                    Buffer.BlockCopy(lineBytes, 0, bytes, offset, lineBytes.Length);
                    offset += lineBytes.Length;
                }

            } while (scanlines.MoveNext());

            gl.BindTexture(TEXTURE_2D, ((TextureHandle)tex).Handle);
            var imageData = gl.CastNativeArray(bytes);
            gl.TexSubImage2D(TEXTURE_2D, 0, startX, startY, width, height, pixelFormat, UNSIGNED_BYTE, imageData);

            gl.GenerateMipmap(TEXTURE_2D);

            gl.TexParameteri(TEXTURE_2D, TEXTURE_MIN_FILTER, (int)LINEAR_MIPMAP_LINEAR);
            gl.TexParameteri(TEXTURE_2D, TEXTURE_MAG_FILTER, (int)LINEAR_MIPMAP_LINEAR);
        }

        private Tuple<int, int> GetMinMagFilter(TextureFilterMode filterMode)
        {
            int minFilter;
            int magFilter;


            switch (filterMode)
            {
                case TextureFilterMode.NEAREST:
                    minFilter = (int)NEAREST;
                    magFilter = (int)NEAREST;
                    break;
                default:
                case TextureFilterMode.LINEAR:
                    minFilter = (int)LINEAR;
                    magFilter = (int)LINEAR;
                    break;
                case TextureFilterMode.NEAREST_MIPMAP_NEAREST:
                    minFilter = (int)NEAREST_MIPMAP_NEAREST;
                    magFilter = (int)NEAREST_MIPMAP_NEAREST;
                    break;
                case TextureFilterMode.LINEAR_MIPMAP_NEAREST:
                    minFilter = (int)LINEAR_MIPMAP_NEAREST;
                    magFilter = (int)LINEAR_MIPMAP_NEAREST;
                    break;
                case TextureFilterMode.NEAREST_MIPMAP_LINEAR:
                    minFilter = (int)NEAREST_MIPMAP_LINEAR;
                    magFilter = (int)NEAREST_MIPMAP_LINEAR;
                    break;
                case TextureFilterMode.LINEAR_MIPMAP_LINEAR:
                    minFilter = (int)NEAREST_MIPMAP_LINEAR;
                    magFilter = (int)NEAREST_MIPMAP_LINEAR;
                    break;
            }

            return new Tuple<int, int>(minFilter, magFilter);
        }
        private int GetWrapMode(TextureWrapMode wrapMode)
        {
            switch (wrapMode)
            {
                default:
                case TextureWrapMode.REPEAT:
                    return (int)REPEAT;
                case TextureWrapMode.MIRRORED_REPEAT:
                    return (int)MIRRORED_REPEAT;
                case TextureWrapMode.CLAMP_TO_EDGE:
                    return (int)CLAMP_TO_EDGE;
                case TextureWrapMode.CLAMP_TO_BORDER:
                    {
                        #warning TextureWrapMode.CLAMP_TO_BORDER is not supported on Android. CLAMP_TO_EDGE is set instead.
                        return (int)CLAMP_TO_EDGE;
                    }
            }
        }


        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param> 
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(ITexture img)
        {
            uint internalFormat;
            uint format;

            var glWrapMode = GetWrapMode(img.WrapMode);
            var glFilterMode = GetMinMagFilter(img.FilterMode);

            switch (img.PixelFormat.ColorFormat)
            {
                case ColorFormat.RGBA:
                    internalFormat = RGBA;
                    format = RGBA;
                    break;
                case ColorFormat.RGB:
                    internalFormat = RGB;
                    format = RGB;
                    break;
                // TODO: Handle Alpha-only / Intensity-only and AlphaIntensity correctly.
                case ColorFormat.Intensity:
                    internalFormat = ALPHA;
                    format = ALPHA;
                    break;
                case ColorFormat.iRGBA:
                    internalFormat = RGB8I;
                    format = RGBA;
                    break;
                case ColorFormat.fRGB32:
                    internalFormat = RGB32I;
                    format = RGBA;
                    break;
                case ColorFormat.fRGB16:
                    internalFormat = RGB16I;
                    format = RGBA;
                    break;
                case ColorFormat.Depth:
                    internalFormat = DEPTH_COMPONENT16;
                    format = DEPTH_COMPONENT;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("CreateTexture: Image pixel format not supported");
            }            

            var id = gl.CreateTexture();

            gl.BindTexture(TEXTURE_2D, id);
            var imageData = gl.CastNativeArray(img.PixelData);
            gl.TexImage2D(TEXTURE_2D, 0, internalFormat, img.Width, img.Height, 0, format, UNSIGNED_BYTE, imageData);

            if(img.DoGenerateMipMaps)
                gl.GenerateMipmap(TEXTURE_2D);

            gl.TexParameteri(TEXTURE_2D, TEXTURE_MIN_FILTER, glFilterMode.Item1);
            gl.TexParameteri(TEXTURE_2D, TEXTURE_MAG_FILTER, glFilterMode.Item2);

            gl.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_S, glWrapMode);
            gl.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_T, glWrapMode);            

            ITextureHandle texID = new TextureHandle { Handle = id, TextureWidth = img.Width, TextureHeight = img.Height};

            return texID;
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to a framebuffer object.
        /// </summary>
        /// <param name="bh">The platform dependent abstraction of the gpu buffer handle.</param>
        public void DeleteFrameBuffer(IBufferHandle bh)
        {
            gl2.DeleteFramebuffer(((FrameBufferHandle)bh).FrameBuffer);
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to a renderbuffer object.
        /// </summary>
        /// <param name="bh">The platform dependent abstraction of the gpu buffer handle.</param>
        public void DeleteRenderBuffer(IBufferHandle bh)
        {
            gl2.DeleteRenderbuffer(((RenderBufferHandle)bh).RenderBuffer);
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to the given <see cref="ITextureHandle"/>.
        /// </summary>
        /// <param name="textureHandle">The <see cref="ITextureHandle"/> which gpu allocated memory will be freed.</param>
        public void RemoveTextureHandle(ITextureHandle textureHandle)
        {
            TextureHandle texHandle = (TextureHandle)textureHandle;

            if (texHandle.RenderToTextureBufferHandle != null)
            {
                gl.DeleteTexture(texHandle.RenderToTextureBufferHandle);
            }

            if (texHandle.FboHandle != null)
            {
                gl.DeleteFramebuffer(texHandle.FboHandle);
            }

            if (texHandle.IntermediateToTextureBufferHandle != null)
            {
                gl.DeleteTexture(texHandle.IntermediateToTextureBufferHandle);
            }

            if (texHandle.GBufferHandle != null)
            {
                gl.DeleteFramebuffer(texHandle.GBufferHandle);

                if (texHandle.GDepthRenderbufferHandle != null)
                {
                    gl.DeleteTexture(texHandle.GDepthRenderbufferHandle);
                }

                if (texHandle.GBufferAlbedoSpecTextureHandle != null)
                {
                    gl.DeleteTexture(texHandle.GBufferAlbedoSpecTextureHandle);
                }

                if (texHandle.GBufferDepthTextureHandle != null)
                {
                    gl.DeleteTexture(texHandle.GBufferDepthTextureHandle);
                }

                if (texHandle.GBufferNormalTextureHandle != null)
                {
                    gl.DeleteTexture(texHandle.GBufferNormalTextureHandle);
                }

                if (texHandle.GBufferPositionTextureHandle != null)
                {
                    gl.DeleteTexture(texHandle.GBufferPositionTextureHandle);
                }
            }

            // TODO: (dd) ?? TBD
            if (texHandle.DepthHandle != null)
            {

            }

            if (texHandle.Handle != null)
            {
                gl.DeleteTexture(texHandle.Handle);
            }
        }

        /// <summary>
        /// Creates a new writable texture and binds it to the shader.
        /// Creates also a framebufferobject and installs convenience methods for binding and reading.
        /// </summary>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateWritableTexture(int width, int height, WritableTextureFormat textureFormat)
        {
            TextureHandle returnTexture = null;

            try
            {
                switch (textureFormat)
                {
                    case WritableTextureFormat.Depth:
                        returnTexture = CreateDepthFramebuffer(width, height);
                        break;
                    case WritableTextureFormat.CubeMap:
                        returnTexture = CreateCubeMapFramebuffer(width, height);
                        break;
                    case WritableTextureFormat.RenderTargetTexture:
                        returnTexture = CreateRenderTargetTextureFramebuffer(width, height);
                        break;
                    case WritableTextureFormat.GBuffer:
                        returnTexture = CreateGBufferFramebuffer(width, height);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception e)
            {
                Diagnostics.Log($"Error creating writable Texture: {gl.GetError()}, {e}");
            }
            return returnTexture;
        }


        private TextureHandle CreateGBufferFramebuffer(int width, int height)
        {
            throw new NotImplementedException("TODO: use explicit WebGL version 2 context within WebAsm.RenderContextImp");
            /*
            // TODO: Add Speculardata
            // Set up G-Buffer
            // 4 textures:
            // 1. Positions (RGB)
            // 2. Normals (RGB)
            // 3. Color (RGB)
            // 4. Depth (DepthComponent24)

            WebGLFramebuffer gBufferHandle = null;
            WebGLTexture gBufferPositionTextureHandle = null;
            WebGLTexture gBufferNormalTextureHandle = null;
            WebGLTexture gBufferAlbedoTextureHandle = null;
            WebGLTexture gBufferDepthTextureHandle = null;

            // Renderbuffer
            WebGLTexture gDepthRenderbufferHandle = null;

            gBufferHandle = gl.CreateFramebuffer();
            gl.BindFramebuffer(FRAMEBUFFER, gBufferHandle);

            // Position color buffer - 16 or 32 bit float per component - high precision texture
            gBufferPositionTextureHandle = gl.CreateTexture();
            gl.BindTexture(TEXTURE_2D, gBufferPositionTextureHandle);
            gl.TexImage2D(TEXTURE_2D, 0, PixelInternalFormat.Rgb16f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            gl.FramebufferTexture2D(FRAMEBUFFER, FramebufferAttachment.ColorAttachment0, TEXTURE_2D, gBufferPositionTextureHandle, 0);

            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureWrapS, (float)(int)TextureWrapMode.ClampToEdge);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);


            // Normal color buffer - 16 or 32 bit float per component - high precision texture
            gBufferNormalTextureHandle = gl.CreateTexture();
            gl.BindTexture(TEXTURE_2D, gBufferNormalTextureHandle);
            gl.TexImage2D(TEXTURE_2D, 0, PixelInternalFormat.Rgb16f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            gl.FramebufferTexture2D(FRAMEBUFFER, FramebufferAttachment.ColorAttachment1, TEXTURE_2D, gBufferNormalTextureHandle, 0);

            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureWrapS, (float)(int)TextureWrapMode.ClampToEdge);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Color - default 8bit texture is enough
            gl.GenTextures(1, out gBufferAlbedoTextureHandle);
            gl.BindTexture(TEXTURE_2D, gBufferAlbedoTextureHandle);
            gl.TexImage2D(TEXTURE_2D, 0, PixelInternalFormat.Rgb32f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            gl.FramebufferTexture2D(FRAMEBUFFER, FramebufferAttachment.ColorAttachment2, TEXTURE_2D, gBufferAlbedoTextureHandle, 0);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureWrapS, (float)(int)TextureWrapMode.ClampToEdge);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Depth - default 32f texture is enough
            gl.GenTextures(1, out gBufferDepthTextureHandle);
            gl.BindTexture(TEXTURE_2D, gBufferDepthTextureHandle);
            gl.TexImage2D(TEXTURE_2D, 0, PixelInternalFormat.Rgb32f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            gl.FramebufferTexture2D(FRAMEBUFFER, FramebufferAttachment.ColorAttachment3, TEXTURE_2D, gBufferDepthTextureHandle, 0);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Tell OpenGL which color attachments we will use (of this framebuffer) for rendering:
            var attachements = new[]
            {
                DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1,
                DrawBuffersEnum.ColorAttachment2, DrawBuffersEnum.ColorAttachment3
            };

            gl.DrawBuffers(attachements.Length, attachements);

            // Create and attach depth buffer (renderbuffer)
            gl.GenRenderbuffers(1, out gDepthRenderbufferHandle);
            gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);
            gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, width, height);
            gl.FramebufferRenderbuffer(FRAMEBUFFER, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);

            // Bind normal buffer again
            gl.BindFramebuffer(FRAMEBUFFER, 0);

            // check if complete
            if (gl.CheckFramebufferStatus(FRAMEBUFFER) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception($"Error creating writable Texture: {gl.GetError()}, {gl.CheckFramebufferStatus(FRAMEBUFFER)}");
            }


            // Fill texture with all params
            return new TextureHandle
            {
                GBufferHandle = gBufferHandle,
                GBufferPositionTextureHandle = gBufferPositionTextureHandle,
                GBufferAlbedoSpecTextureHandle = gBufferAlbedoTextureHandle,
                GBufferNormalTextureHandle = gBufferNormalTextureHandle,
                GBufferDepthTextureHandle = gBufferDepthTextureHandle,
                GDepthRenderbufferHandle = gDepthRenderbufferHandle,

                TextureWidth = width,
                TextureHeight = height
            };
            */
        }

        // Creates a depth framebuffer
        private static TextureHandle CreateDepthFramebuffer(int width, int height)
        {
            throw new NotImplementedException("TODO: use explicit WebGL version 2 context within WebAsm.RenderContextImp");
            /*

            var textureHandle = 0;
            var fboHandle = 0;

            // Create a shadow texture
            gl.GenTextures(1, out textureHandle);

            gl.BindTexture(TEXTURE_2D, textureHandle);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToBorder);
            // everything outside the border will be white
            var borderColor = 1.0f;
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureBorderColor, borderColor);
            gl.TexImage2D(TEXTURE_2D, 0, PixelInternalFormat.DepthComponent16, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);

            // Create FBO
            gl.GenFramebuffers(1, out fboHandle);
            gl.BindFramebuffer(FRAMEBUFFER, fboHandle);
            gl.FramebufferTexture2D(FRAMEBUFFER, FramebufferAttachment.DepthAttachment, TEXTURE_2D, textureHandle, 0);

            // Disable writes to the color buffer
            gl.DrawBuffer(DrawBufferMode.None);
            gl.ReadBuffer(ReadBufferMode.None);

            if (gl.CheckFramebufferStatus(FRAMEBUFFER) != FramebufferErrorCode.FramebufferCompleteExt)
            {
                throw new Exception($"Error creating writable Texture: {gl.GetError()}, {gl.CheckFramebufferStatus(FRAMEBUFFER)}");
            }

            return new TextureHandle { Handle = textureHandle, FboHandle = fboHandle };
            */
        }

        private static TextureHandle CreateCubeMapFramebuffer(int width, int height)
        {
            throw new NotImplementedException("TODO: use explicit WebGL version 2 context within WebAsm.RenderContextImp");
            /*
            //throw new NotImplementedException("Currently not implemented!");

            var cubeMapTextureHandle = 0;
            var depthBuffer = 0;
            var framebuffer = 0;

            // Create a shadow texture
            gl.GenTextures(1, out cubeMapTextureHandle);
            gl.BindTexture(TextureTarget.TextureCubeMap, cubeMapTextureHandle);

            gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            // HDR Texture
            for (var i = 0; i < 6; i++)
                gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgb32f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);

            // create the fbo
            gl.GenFramebuffers(1, out framebuffer);
            gl.BindFramebuffer(FRAMEBUFFER, framebuffer);
            gl.FramebufferTexture2D(FRAMEBUFFER, FramebufferAttachment.ColorAttachment0, TextureTarget.TextureCubeMap, cubeMapTextureHandle, 0);

            // create the uniform depth buffer
            gl.GenRenderbuffers(1, out depthBuffer);
            gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
            gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, width, height);
            gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            // Bind normal buffer again
            gl.BindFramebuffer(FRAMEBUFFER, 0);
            //gl.BindTexture(TextureTarget.TextureCubeMap, 0);

            if (gl.CheckFramebufferStatus(FRAMEBUFFER) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception($"Error creating writable Texture: {gl.GetError()}, {gl.CheckFramebufferStatus(FRAMEBUFFER)}");
            }

            return new TextureHandle { Handle = cubeMapTextureHandle, FboHandle = framebuffer };
            */
        }

        private static TextureHandle CreateRenderTargetTextureFramebuffer(int width, int height)
        {
            throw new NotImplementedException("TODO: implement me.");
            /*
            // configure 4x MSAA framebuffer
            int msaa_level = 4;
            // --------------------------
            gl.GenFramebuffers(1, out int framebuffer);
            gl.BindFramebuffer(FRAMEBUFFER, framebuffer);

            // create a multisampled color attachment texture
            var textureColorBufferMultiSampled = gl.GenTexture();
            gl.BindTexture(TEXTURE_2DMultisample, textureColorBufferMultiSampled);
            gl.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, msaa_level, PixelInternalFormat.Rgba32f, width, height, true);
            gl.BindTexture(TEXTURE_2DMultisample, 0);
            gl.FramebufferTexture2D(FRAMEBUFFER, FramebufferAttachment.ColorAttachment0, TEXTURE_2DMultisample, textureColorBufferMultiSampled, 0);

            // create a (also multisampled) renderbuffer object for depth and stencil attachments
            gl.GenRenderbuffers(1, out int rbo);
            gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            gl.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, msaa_level, RenderbufferStorage.Depth24Stencil8, width, height);
            gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            gl.FramebufferRenderbuffer(FRAMEBUFFER, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbo);

            if (gl.CheckFramebufferStatus(FRAMEBUFFER) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception($"Error creating writable Texture: {gl.GetError()}, {gl.CheckFramebufferStatus(FRAMEBUFFER)}");
            }
            gl.BindFramebuffer(FRAMEBUFFER, 0);

            // configure second post-processing framebuffer
            //unsigned int intermediateFBO;
            gl.GenFramebuffers(1, out int intermediateFbo);
            gl.BindFramebuffer(FRAMEBUFFER, intermediateFbo);

            //// create a color attachment texture (multisampled texture will be blitted into screenTexture)
            int screenTexture = gl.GenTexture();
            gl.BindTexture(TEXTURE_2D, screenTexture);

            gl.TexImage2D(TEXTURE_2D, 0, PixelInternalFormat.Rgba32f, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            gl.TexParameter(TEXTURE_2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            gl.FramebufferTexture2D(FRAMEBUFFER, FramebufferAttachment.ColorAttachment0, TEXTURE_2D, screenTexture, 0);

            // Handle= the blitted texture that will be used as shaderparam sampler2D...
            return new TextureHandle { Handle = screenTexture, RenderToTextureBufferHandle = framebuffer, IntermediateToTextureBufferHandle = intermediateFbo, TextureWidth = width, TextureHeight = height };
            */
        }

        #endregion

        #region Shader related Members

        /// <summary>
        /// Gets the shader parameter.
        /// The Shader parameter is used to bind values inside of shaderprograms that run on the graphics card.
        /// Do not use this function in frequent updates as it transfers information from graphics card to the cpu which takes time.
        /// </summary>
        /// <param name="shaderProgram">The shader program.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>The Shader parameter is returned if the name is found, otherwise null.</returns>
        public IShaderParam GetShaderParam(IShaderProgramImp shaderProgram, string paramName)
        {
            WebGLUniformLocation h = gl.GetUniformLocation(((ShaderProgramImp)shaderProgram).Program, paramName);
            return (h == null) ? null : new ShaderParam { handle = h };
        }

        /// <summary>
        /// Gets the float parameter value inside a shaderprogram by using a <see cref="IShaderParam" /> as search reference.
        /// Do not use this function in frequent updates as it transfers information from the graphics card to the cpu which takes time.
        /// </summary>
        /// <param name="program">The shader program.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>The current parameter's value.</returns>
        public float GetParamValue(IShaderProgramImp program, IShaderParam param)
        {
            float f = (float) gl.GetUniform(((ShaderProgramImp)program).Program, ((ShaderParam)param).handle);
            return f;
        }

        /// <inheritdoc />
        /// <summary>
        /// Removes shader from the GPU
        /// </summary>
        /// <param name="sp"></param>
        public void RemoveShader(IShaderProgramImp sp)
        {
            if (sp == null) return; // if no RenderContext is available return otherwise memory read error

            var program = ((ShaderProgramImp)sp).Program;

            // wait for all threads to be finished
            gl.Finish();
            gl.Flush();

            // cleanup
            // gl.DeleteShader(program);
            gl.DeleteProgram(program);
        }

        public void SetLineWidth(float width)
        {
            gl.LineWidth(width);
        }

        /// <summary>
        /// Gets the shader parameter list of a specific <see cref="IShaderProgramImp" />. 
        /// </summary>
        /// <param name="shaderProgram">The shader program.</param>
        /// <returns>All Shader parameters of a shaderprogram are returned.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IList<ShaderParamInfo> GetShaderParamList(IShaderProgramImp shaderProgram)
        {
            var sProg = (ShaderProgramImp)shaderProgram;
            var paramList = new List<ShaderParamInfo>();

            int nParams;
            nParams = (int) gl.GetProgramParameter(sProg.Program, ACTIVE_UNIFORMS);

            for (uint i = 0; i < nParams; i++)
            {
                WebGLActiveInfo activeInfo = gl.GetActiveUniform(sProg.Program, i);

                var paramInfo = new ShaderParamInfo();
                paramInfo.Name = activeInfo.Name;
                paramInfo.Size = activeInfo.Size;
                uint uType = activeInfo.Type;//activeInfo.GlType;
                paramInfo.Handle = GetShaderParam(sProg, paramInfo.Name);

                //Diagnostics.Log($"Active Uniforms: {paramInfo.Name}");

                switch (uType)
                {
                    case INT:
                        paramInfo.Type = typeof(int);
                        break;

                    case FLOAT:
                        paramInfo.Type = typeof(float);
                        break;

                    case FLOAT_VEC2:
                        paramInfo.Type = typeof(float2);
                        break;

                    case FLOAT_VEC3:
                        paramInfo.Type = typeof(float3);
                        break;

                    case FLOAT_VEC4:
                        paramInfo.Type = typeof(float4);
                        break;

                    case FLOAT_MAT4:
                        paramInfo.Type = typeof(float4x4);
                        break;

                    case SAMPLER_2D:
                        paramInfo.Type = typeof(ITexture);
                        break;

                    case SAMPLER_CUBE:
                        paramInfo.Type = typeof(ITexture);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                paramList.Add(paramInfo);
            }
            return paramList;
        }

        /// <summary>
        /// Sets a float shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float val)
        {
            gl.Uniform1f(((ShaderParam)param).handle, val);
        }

        /// <summary>
        /// Sets a <see cref="float2" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float2 val)
        {
            gl.Uniform2f(((ShaderParam)param).handle, val.x, val.y);
        }

        /// <summary>
        /// Sets a <see cref="float3" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float3 val)
        {
            gl.Uniform3f(((ShaderParam)param).handle, val.x, val.y, val.z);
        }

        /// <summary>
        /// Sets a <see cref="float3" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IShaderParam param, float3[] val)
        {
            fixed (float3* pFlt = &val[0])
                gl.Uniform3fv(((ShaderParam)param).handle, new Span<float>((float*)pFlt, val.Length * 3));
        }

        /// <summary>
        /// Sets a <see cref="float4" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float4 val)
        {
            gl.Uniform4f(((ShaderParam)param).handle, val.x, val.y, val.z, val.w);
        }

        // TODO add vector implementations

        /// <summary>
        /// Sets a <see cref="float4x4" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
            gl.UniformMatrix4fv(((ShaderParam)param).handle, true, val.ToArray());
        }



        /// <summary>
        ///     Sets a <see cref="float4" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IShaderParam param, float4[] val)
        {
            fixed (float4* pFlt = &val[0])
                gl.Uniform4fv(((ShaderParam)param).handle, new Span<float>((float*)pFlt, val.Length*4));
        }

        /// <summary>
        /// Sets a <see cref="float4x4" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IShaderParam param, float4x4[] val)
        {
            var tmpArray = new float4[val.Length * 4];

            for (var i = 0; i < val.Length; i++)
            {
                tmpArray[i * 4] = val[i].Column0;
                tmpArray[i * 4 + 1] = val[i].Column1;
                tmpArray[i * 4 + 2] = val[i].Column2;
                tmpArray[i * 4 + 3] = val[i].Column3;
            }

            fixed (float4* pMtx = &tmpArray[0])
                gl.UniformMatrix4fv(((ShaderParam)param).handle, true, new Span<float>((float*)pMtx, val.Length * 16) );
        }

        /// <summary>
        /// Sets a int shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, int val)
        {
            gl.Uniform1i(((ShaderParam)param).handle, val);
        }

        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texId">An ITextureHandle probably returned from CreateTexture method</param>
        public void SetShaderParamTexture(IShaderParam param, ITextureHandle texId)
        {
            var hParam = ((ShaderParam)param).handle;
            int texUnit;
            if (!_shaderParam2TexUnit.TryGetValue(hParam, out texUnit))
            {
                texUnit = _currentTextureUnit++;
                _shaderParam2TexUnit[hParam] = texUnit;
            }
            gl.Uniform1i(hParam, texUnit);
            gl.ActiveTexture((uint)(TEXTURE0 + texUnit));
            //gl.BindTexture(TextureTarget.TextureCubeMap, ((Texture)texId).handle);
            gl.BindTexture(TEXTURE_2D, ((TextureHandle)texId).Handle);
        }

        public void SetShaderParamWritableTexture(IShaderParam param, int texId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texId">An ITextureHandle probably returned from CreateWritableTexture method</param>
        /// <param name="gHandle">The GBufferHandle</param>
        public void SetShaderParamTexture(IShaderParam param, ITextureHandle texId, GBufferHandle gHandle)
        {
            switch (gHandle)
            {
                case GBufferHandle.GPositionHandle:
                    ((TextureHandle)texId).Handle = ((TextureHandle)texId).GBufferPositionTextureHandle;
                    SetShaderParamTexture(param, texId);
                    break;
                case GBufferHandle.GNormalHandle:
                    ((TextureHandle)texId).Handle = ((TextureHandle)texId).GBufferNormalTextureHandle;
                    SetShaderParamTexture(param, texId);
                    break;
                case GBufferHandle.GAlbedoHandle:
                    ((TextureHandle)texId).Handle = ((TextureHandle)texId).GBufferAlbedoSpecTextureHandle;
                    SetShaderParamTexture(param, texId);
                    break;
                case GBufferHandle.GDepth:
                    ((TextureHandle)texId).Handle = ((TextureHandle)texId).GBufferDepthTextureHandle;
                    SetShaderParamTexture(param, texId);
                    break;
                case GBufferHandle.EnvMap:
                    ((TextureHandle)texId).Handle = ((TextureHandle)texId).Handle;
                    var hParam = ((ShaderParam)param).handle;
                    int texUnit;
                    if (!_shaderParam2TexUnit.TryGetValue(hParam, out texUnit))
                    {
                        texUnit = _currentTextureUnit++;
                        _shaderParam2TexUnit[hParam] = texUnit;
                    }
                    gl.Uniform1i(hParam, texUnit);
                    gl.ActiveTexture((uint)(TEXTURE0 + texUnit));
                    gl.BindTexture(TEXTURE_CUBE_MAP, ((TextureHandle)texId).Handle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gHandle), gHandle, null);
            }
        }

        #endregion

        #region Clear Fields

        /// <summary>
        /// Gets and sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the clear.
        /// </value>
        public float4 ClearColor
        {
            get
            {
                float[] ret = (float[]) gl.GetParameter(COLOR_CLEAR_VALUE);
                return new float4(ret[0], ret[1], ret[2], ret[3]);
            }
            set { gl.ClearColor(value.x, value.y, value.z, value.w); }
        }

        /// <summary>
        /// Gets and sets the clear depth value which is used to clear the depth buffer.
        /// </summary>
        /// <value>
        /// Specifies the depth value used when the depth buffer is cleared. The initial value is 1. This value is clamped to the range [0,1].
        /// </value>
        public float ClearDepth
        {
            get
            {
                return (float) gl.GetParameter(DEPTH_CLEAR_VALUE);
            }
            set { gl.ClearDepth(value); }
        }

        #endregion

        #region Rendering related Members

        /// <summary>
        /// The clipping behavior against the Z position of a vertex can be turned off by activating depth clamping. 
        /// This is done with glEnable(GL_DEPTH_CLAMP). This will cause the clip-space Z to remain unclipped by the front and rear viewing volume.
        /// See: https://www.khronos.org/opengl/wiki/Vertex_Post-Processing#Depth_clamping
        /// </summary>
        public void EnableDepthClamp()
        {            
            throw new NotImplementedException("Depth clamping isn't implemented yet!");
        }

        /// <summary>
        /// Disables depths clamping. <seealso cref="EnableDepthClamp"/>
        /// </summary>
        public void DisableDepthClamp()
        {
            throw new NotImplementedException("Depth clamping isn't implemented yet!");
        }

        /// <summary>
        /// Creates the shaderprogram by using a valid GLSL vertex and fragment shader code. This code is compiled at runtime.
        /// Do not use this function in frequent updates.
        /// </summary>
        /// <param name="vs">The vertex shader code.</param>
        /// <param name="ps">The pixel(=fragment) shader code.</param>
        /// <returns>An instance of <see cref="IShaderProgramImp" />.</returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public IShaderProgramImp CreateShader(string vs, string ps)
        {
            bool statusCode;
            string info;

            WebGLShader vertexObject = gl.CreateShader(VERTEX_SHADER);
            WebGLShader fragmentObject = gl.CreateShader(FRAGMENT_SHADER);

            // Compile vertex shader
            gl.ShaderSource(vertexObject, vs);
            gl.CompileShader(vertexObject);
            info = gl.GetShaderInfoLog(vertexObject);
            var statusCodeOb = gl.GetShaderParameter(vertexObject, COMPILE_STATUS);
            statusCode = (bool)statusCodeOb;

            if (!statusCode)
                throw new ApplicationException(info);

            // Compile pixel shader
            gl.ShaderSource(fragmentObject, ps);
            gl.CompileShader(fragmentObject);
            info = gl.GetShaderInfoLog(fragmentObject);
            statusCode = (bool) gl.GetShaderParameter(fragmentObject, COMPILE_STATUS);

            if (!statusCode)
                throw new ApplicationException(info);

            WebGLProgram program = gl.CreateProgram();
            gl.AttachShader(program, fragmentObject);
            gl.AttachShader(program, vertexObject);

            // enable GLSL (ES) shaders to use fuVertex, fuColor and fuNormal attributes
            gl.BindAttribLocation(program, (uint) Helper.VertexAttribLocation, Helper.VertexAttribName);
            gl.BindAttribLocation(program, (uint) Helper.ColorAttribLocation, Helper.ColorAttribName);
            gl.BindAttribLocation(program, (uint) Helper.UvAttribLocation, Helper.UvAttribName);
            gl.BindAttribLocation(program, (uint) Helper.NormalAttribLocation, Helper.NormalAttribName);
            gl.BindAttribLocation(program, (uint) Helper.TangentAttribLocation, Helper.TangentAttribName);
            gl.BindAttribLocation(program, (uint) Helper.BoneIndexAttribLocation, Helper.BoneIndexAttribName);
            gl.BindAttribLocation(program, (uint) Helper.BoneWeightAttribLocation, Helper.BoneWeightAttribName);
            gl.BindAttribLocation(program, (uint) Helper.BitangentAttribLocation, Helper.BitangentAttribName);

            gl.LinkProgram(program); // AAAARRRRRGGGGHHHH!!!! Must be called AFTER BindAttribLocation

            // mr: Detach Shader & delete
            //gl.DetachShader(program, fragmentObject);
            //gl.DetachShader(program, vertexObject);
            //gl.DeleteShader(fragmentObject);
            //gl.DeleteShader(vertexObject);

            return new ShaderProgramImp { Program = program };
        }

        /// <summary>
        /// Sets the shaderprogram onto the GL Rendercontext.
        /// </summary>
        /// <param name="program">The shaderprogram.</param>
        public void SetShader(IShaderProgramImp program)
        {
            _currentTextureUnit = 0;
            _shaderParam2TexUnit.Clear();

            gl.UseProgram(((ShaderProgramImp)program).Program);
        }

        /// <summary>
        /// Clears the specified flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
        public void Clear(ClearFlags flags)
        {
            // ACCUM is ignored in WebGL...
            var wglFlags = 
                  ((flags & ClearFlags.Depth) != 0 ? DEPTH_BUFFER_BIT : 0)
                | ((flags & ClearFlags.Stencil) != 0 ? STENCIL_BUFFER_BIT : 0)
                | ((flags & ClearFlags.Color) != 0 ? COLOR_BUFFER_BIT : 0);
            gl.Clear(wglFlags);
        }

        /// <summary>
        /// Create one single multi-purpose attribute buffer
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public IAttribImp CreateAttributeBuffer(float3[] attributes, string attributeName)
        {
            if (attributes == null || attributes.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            int vboBytes;
            int vertsBytes = attributes.Length * 3 * sizeof(float);
            var handle = gl.CreateBuffer();

            gl.BindBuffer(ARRAY_BUFFER, handle);
            gl.BufferData(ARRAY_BUFFER, attributes, STATIC_DRAW);
            vboBytes = (int) gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading attribute buffer to VBO ('{2}'). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes, attributeName));
            gl.BindBuffer(ARRAY_BUFFER, null);

            return new AttributeImp { AttributeBufferObject = handle };
        }

        /// <summary>
        /// Remove an attribute buffer previously created with <see cref="CreateAttributeBuffer"/> and release all associated resources
        /// allocated on the GPU.
        /// </summary>
        /// <param name="attribHandle">The han</param>
        public void DeleteAttributeBuffer(IAttribImp attribHandle)
        {
            if (attribHandle != null)
            {
                var handle = ((AttributeImp)attribHandle).AttributeBufferObject;
                if (handle != null)
                {
                    gl.DeleteBuffer(handle);
                    ((AttributeImp)attribHandle).AttributeBufferObject = null;
                }
            }
        }

        /// <summary>
        /// Binds the vertices onto the GL Rendercontext and assigns an VertexBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="vertices">The vertices.</param>
        /// <exception cref="ArgumentException">Vertices must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetVertices(IMeshImp mr, float3[] vertices)
        {
            Diagnostics.Log("[SetVertices]");
            if (vertices == null || vertices.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            int vboBytes;
            int vertsBytes = vertices.Length * 3 * sizeof(float);
            if (((MeshImp)mr).VertexBufferObject == null)
                ((MeshImp)mr).VertexBufferObject = gl.CreateBuffer();

            gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).VertexBufferObject);
            Diagnostics.Log("[Before gl.BufferData]");

            var verticesFlat = new float[vertices.Length * 3];
            unsafe
            {
                fixed (float3* pBytes = &vertices[0])
                {
                    Marshal.Copy((IntPtr)(pBytes), verticesFlat, 0, verticesFlat.Length);
                }
            }
            gl.BufferData(ARRAY_BUFFER, verticesFlat, STATIC_DRAW);

            Diagnostics.Log("[After gl.BufferData]");
            vboBytes = (int) gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format("Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.", vertsBytes, vboBytes));
            gl.BindBuffer(ARRAY_BUFFER, null);
        }

        /// <summary>
        /// Binds the tangents onto the GL Rendercontext and assigns an TangentBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="tangents">The tangents.</param>
        /// <exception cref="ArgumentException">Tangents must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetTangents(IMeshImp mr, float4[] tangents)
        {
            if (tangents == null || tangents.Length == 0)
            {
                throw new ArgumentException("Tangents must not be null or empty");
            }

            int vboBytes;
            int tangentBytes = tangents.Length * 4 * sizeof(float);
            if (((MeshImp)mr).TangentBufferObject == null)
                ((MeshImp)mr).TangentBufferObject = gl.CreateBuffer(); ;

            gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).TangentBufferObject);

            var tangentsFlat = new float[tangents.Length * 4];
            unsafe
            {
                fixed (float4* pBytes = &tangents[0])
                {
                    Marshal.Copy((IntPtr)(pBytes), tangentsFlat, 0, tangentsFlat.Length);
                }
            }

            gl.BufferData(ARRAY_BUFFER, tangentsFlat, STATIC_DRAW);
            vboBytes = (int) gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != tangentBytes)
                throw new ApplicationException(String.Format("Problem uploading vertex buffer to VBO (tangents). Tried to upload {0} bytes, uploaded {1}.", tangentBytes, vboBytes));
            gl.BindBuffer(ARRAY_BUFFER, null);
        }

        /// <summary>
        /// Binds the bitangents onto the GL Rendercontext and assigns an BiTangentBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="tangents">The BiTangents.</param>
        /// <exception cref="ArgumentException">BiTangents must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetBiTangents(IMeshImp mr, float3[] bitangents)
        {
            if (bitangents == null || bitangents.Length == 0)
            {
                throw new ArgumentException("BiTangents must not be null or empty");
            }

            int vboBytes;
            int bitangentBytes = bitangents.Length * 3 * sizeof(float);
            if (((MeshImp)mr).BitangentBufferObject == null)
                ((MeshImp)mr).BitangentBufferObject = gl.CreateBuffer(); 

            gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).BitangentBufferObject);

            var bitangentsFlat = new float[bitangents.Length * 3];
            unsafe
            {
                fixed (float3* pBytes = &bitangents[0])
                {
                    Marshal.Copy((IntPtr)(pBytes), bitangentsFlat, 0, bitangentsFlat.Length);
                }
            }

            gl.BufferData(ARRAY_BUFFER, bitangentsFlat, STATIC_DRAW);
            vboBytes = (int) gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != bitangentBytes)
                throw new ApplicationException(String.Format("Problem uploading vertex buffer to VBO (bitangents). Tried to upload {0} bytes, uploaded {1}.", bitangentBytes, vboBytes));
            gl.BindBuffer(ARRAY_BUFFER, null);
        }

        /// <summary>
        /// Binds the normals onto the GL Rendercontext and assigns an NormalBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="normals">The normals.</param>
        /// <exception cref="ArgumentException">Normals must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetNormals(IMeshImp mr, float3[] normals)
        {
            if (normals == null || normals.Length == 0)
            {
                throw new ArgumentException("Normals must not be null or empty");
            }

            int vboBytes;
            int normsBytes = normals.Length * 3 * sizeof(float);
            if (((MeshImp)mr).NormalBufferObject == null)
                ((MeshImp)mr).NormalBufferObject = gl.CreateBuffer();

            gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).NormalBufferObject);

            var normalsFlat = new float[normals.Length * 3];
            unsafe
            {
                fixed (float3* pBytes = &normals[0])
                {
                    Marshal.Copy((IntPtr)(pBytes), normalsFlat, 0, normalsFlat.Length);
                }
            }

            gl.BufferData(ARRAY_BUFFER, normalsFlat, STATIC_DRAW);

            vboBytes = (int) gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != normsBytes)
                throw new ApplicationException(String.Format("Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.", normsBytes, vboBytes));
            gl.BindBuffer(ARRAY_BUFFER, null);
        }

        /// <summary>
        /// Binds the boneindices onto the GL Rendercontext and assigns an BondeIndexBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneIndices">The boneindices.</param>
        /// <exception cref="ArgumentException">BoneIndices must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetBoneIndices(IMeshImp mr, float4[] boneIndices)
        {
            if (boneIndices == null || boneIndices.Length == 0)
            {
                throw new ArgumentException("BoneIndices must not be null or empty");
            }

            int vboBytes;
            int indicesBytes = boneIndices.Length * 4 * sizeof(float);
            if (((MeshImp)mr).BoneIndexBufferObject == null)
                ((MeshImp)mr).BoneIndexBufferObject = gl.CreateBuffer();

            gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).BoneIndexBufferObject);

            var boneIndicesFlat = new float[boneIndices.Length * 4];
            unsafe
            {
                fixed (float4* pBytes = &boneIndices[0])
                {
                    Marshal.Copy((IntPtr)(pBytes), boneIndicesFlat, 0, boneIndicesFlat.Length);
                }
            }

            gl.BufferData(ARRAY_BUFFER, boneIndicesFlat, STATIC_DRAW);
            vboBytes = (int) gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != indicesBytes)
                throw new ApplicationException(String.Format("Problem uploading boneindices buffer to VBO (boneindices). Tried to upload {0} bytes, uploaded {1}.", indicesBytes, vboBytes));
            gl.BindBuffer(ARRAY_BUFFER, null);
        }

        /// <summary>
        /// Binds the boneweights onto the GL Rendercontext and assigns an BondeWeightBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneWeights">The boneweights.</param>
        /// <exception cref="ArgumentException">BoneWeights must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetBoneWeights(IMeshImp mr, float4[] boneWeights)
        {
            if (boneWeights == null || boneWeights.Length == 0)
            {
                throw new ArgumentException("BoneWeights must not be null or empty");
            }

            int vboBytes;
            int weightsBytes = boneWeights.Length * 4 * sizeof(float);
            if (((MeshImp)mr).BoneWeightBufferObject == null)
                ((MeshImp)mr).BoneWeightBufferObject = gl.CreateBuffer();

            gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).BoneWeightBufferObject);

            var boneWeightsFlat = new float[boneWeights.Length * 4];
            unsafe
            {
                fixed (float4* pBytes = &boneWeights[0])
                {
                    Marshal.Copy((IntPtr)(pBytes), boneWeightsFlat, 0, boneWeightsFlat.Length);
                }
            }

            gl.BufferData(ARRAY_BUFFER, boneWeightsFlat, STATIC_DRAW);
            vboBytes = (int) gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != weightsBytes)
                throw new ApplicationException(String.Format("Problem uploading boneweights buffer to VBO (boneweights). Tried to upload {0} bytes, uploaded {1}.", weightsBytes, vboBytes));
            gl.BindBuffer(ARRAY_BUFFER, null);
        }

        /// <summary>
        /// Binds the UV coordinates onto the GL Rendercontext and assigns an UVBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="uvs">The UV's.</param>
        /// <exception cref="ArgumentException">UVs must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetUVs(IMeshImp mr, float2[] uvs)
        {
            if (uvs == null || uvs.Length == 0)
            {
                throw new ArgumentException("UVs must not be null or empty");
            }

            int vboBytes;
            int uvsBytes = uvs.Length * 2 * sizeof(float);
            if (((MeshImp)mr).UVBufferObject == null)
                ((MeshImp)mr).UVBufferObject = gl.CreateBuffer();

            gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).UVBufferObject);

            var uvsFlat = new float[uvs.Length * 2];
            unsafe
            {
                fixed (float2* pBytes = &uvs[0])
                {
                    Marshal.Copy((IntPtr)(pBytes), uvsFlat, 0, uvsFlat.Length);
                }
            }

            gl.BufferData(ARRAY_BUFFER, uvsFlat, STATIC_DRAW);
            vboBytes = (int) gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != uvsBytes)
                throw new ApplicationException(String.Format("Problem uploading uv buffer to VBO (uvs). Tried to upload {0} bytes, uploaded {1}.", uvsBytes, vboBytes));
            gl.BindBuffer(ARRAY_BUFFER, null);
        }

        /// <summary>
        /// Binds the colors onto the GL Rendercontext and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="ArgumentException">colors must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetColors(IMeshImp mr, uint[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("colors must not be null or empty");
            }

            int vboBytes;
            int colsBytes = colors.Length * sizeof(uint);
            if (((MeshImp)mr).ColorBufferObject == null)
                ((MeshImp)mr).ColorBufferObject = gl.CreateBuffer();

            gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).ColorBufferObject);
            gl.BufferData(ARRAY_BUFFER, colors, STATIC_DRAW);
            vboBytes = (int) gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != colsBytes)
                throw new ApplicationException(String.Format("Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.", colsBytes, vboBytes));
            gl.BindBuffer(ARRAY_BUFFER, null);
        }

        /// <summary>
        /// Binds the triangles onto the GL Rendercontext and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="triangleIndices">The triangle indices.</param>
        /// <exception cref="ArgumentException">triangleIndices must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetTriangles(IMeshImp mr, ushort[] triangleIndices)
        {
            if (triangleIndices == null || triangleIndices.Length == 0)
            {
                throw new ArgumentException("triangleIndices must not be null or empty");
            }
            ((MeshImp)mr).NElements = triangleIndices.Length;
            int vboBytes;
            int trisBytes = triangleIndices.Length * sizeof(short);

            if (((MeshImp)mr).ElementBufferObject == null)
                ((MeshImp)mr).ElementBufferObject = gl.CreateBuffer();
            // Upload the index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
            gl.BindBuffer(ELEMENT_ARRAY_BUFFER, ((MeshImp)mr).ElementBufferObject);
            gl.BufferData(ELEMENT_ARRAY_BUFFER, triangleIndices, STATIC_DRAW);
            vboBytes = (int)gl.GetBufferParameter(ELEMENT_ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != trisBytes)
                throw new ApplicationException(String.Format("Problem uploading vertex buffer to VBO (offsets). Tried to upload {0} bytes, uploaded {1}.", trisBytes, vboBytes));
            gl.BindBuffer(ARRAY_BUFFER, null);
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveVertices(IMeshImp mr)
        {
            gl.DeleteBuffer(((MeshImp)mr).VertexBufferObject);
            ((MeshImp)mr).InvalidateVertices();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveNormals(IMeshImp mr)
        {
            gl.DeleteBuffer(((MeshImp)mr).NormalBufferObject);
            ((MeshImp)mr).InvalidateNormals();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveColors(IMeshImp mr)
        {
            gl.DeleteBuffer(((MeshImp)mr).ColorBufferObject);
            ((MeshImp)mr).InvalidateColors();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveUVs(IMeshImp mr)
        {
            gl.DeleteBuffer(((MeshImp)mr).UVBufferObject);
            ((MeshImp)mr).InvalidateUVs();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveTriangles(IMeshImp mr)
        {
            gl.DeleteBuffer(((MeshImp)mr).ElementBufferObject);
            ((MeshImp)mr).InvalidateTriangles();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBoneWeights(IMeshImp mr)
        {
            gl.DeleteBuffer(((MeshImp)mr).BoneWeightBufferObject);
            ((MeshImp)mr).InvalidateBoneWeights();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBoneIndices(IMeshImp mr)
        {
            gl.DeleteBuffer(((MeshImp)mr).BoneIndexBufferObject);
            ((MeshImp)mr).InvalidateBoneIndices();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveTangents(IMeshImp mr)
        {
            gl.DeleteBuffer(((MeshImp)mr).TangentBufferObject);
            ((MeshImp)mr).InvalidateTangents();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBiTangents(IMeshImp mr)
        {
            gl.DeleteBuffer(((MeshImp)mr).BitangentBufferObject);
            ((MeshImp)mr).InvalidateBiTangents();
        }
        /// <summary>
        /// Renders the specified <see cref="IMeshImp" />.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        public void Render(IMeshImp mr)
        {
            if (((MeshImp)mr).VertexBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)Helper.VertexAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).VertexBufferObject);
                gl.VertexAttribPointer((uint)Helper.VertexAttribLocation, 3, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).ColorBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)Helper.ColorAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).ColorBufferObject);
                gl.VertexAttribPointer((uint)Helper.ColorAttribLocation, 4, UNSIGNED_BYTE, true, 0, 0);
            }

            if (((MeshImp)mr).UVBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)Helper.UvAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).UVBufferObject);
                gl.VertexAttribPointer((uint)Helper.UvAttribLocation, 2, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).NormalBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)Helper.NormalAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).NormalBufferObject);
                gl.VertexAttribPointer((uint)Helper.NormalAttribLocation, 3, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).TangentBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)Helper.TangentAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).TangentBufferObject);
                gl.VertexAttribPointer((uint)Helper.TangentAttribLocation, 3, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).BitangentBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)Helper.BitangentAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).BitangentBufferObject);
                gl.VertexAttribPointer((uint)Helper.BitangentAttribLocation, 3, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).BoneIndexBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)Helper.BoneIndexAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).BoneIndexBufferObject);
                gl.VertexAttribPointer((uint)Helper.BoneIndexAttribLocation, 4, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).BoneWeightBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)Helper.BoneWeightAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).BoneWeightBufferObject);
                gl.VertexAttribPointer((uint)Helper.BoneWeightAttribLocation, 4, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).ElementBufferObject != null)
            {
                gl.BindBuffer(ELEMENT_ARRAY_BUFFER, ((MeshImp)mr).ElementBufferObject);
                gl.DrawElements(TRIANGLES, ((MeshImp)mr).NElements, UNSIGNED_SHORT, 0);
                //gl.DrawArrays(gl.Enums.BeginMode.POINTS, 0, shape.Vertices.Length);
            }


            if (((MeshImp)mr).VertexBufferObject != null)
            {
                gl.BindBuffer(ARRAY_BUFFER, null);
                gl.DisableVertexAttribArray((uint)Helper.VertexAttribLocation);
            }
            if (((MeshImp)mr).ColorBufferObject != null)
            {
                gl.BindBuffer(ARRAY_BUFFER, null);
                gl.DisableVertexAttribArray((uint)Helper.ColorAttribLocation);
            }
            if (((MeshImp)mr).NormalBufferObject != null)
            {
                gl.BindBuffer(ARRAY_BUFFER, null);
                gl.DisableVertexAttribArray((uint)Helper.NormalAttribLocation);
            }
            if (((MeshImp)mr).UVBufferObject != null)
            {
                gl.BindBuffer(ARRAY_BUFFER, null);
                gl.DisableVertexAttribArray((uint)Helper.UvAttribLocation);
            }
            if (((MeshImp)mr).TangentBufferObject != null)
            {
                gl.BindBuffer(ARRAY_BUFFER, null);
                gl.DisableVertexAttribArray((uint)Helper.TangentAttribLocation);
            }
            if (((MeshImp)mr).BitangentBufferObject != null)
            {
                gl.BindBuffer(ARRAY_BUFFER, null);
                gl.DisableVertexAttribArray((uint)Helper.TangentAttribLocation);
            }
        }

        /// <summary>
        /// Draws a Debug Line in 3D Space by using a start and end point (float3).
        /// </summary>
        /// <param name="start">The startpoint of the DebugLine.</param>
        /// <param name="end">The endpoint of the DebugLine.</param>
        /// <param name="color">The color of the DebugLine.</param>
        public void DebugLine(float3 start, float3 end, float4 color)
        {
            var vertices = new float3[]
            {
                new float3(start.x, start.y, start.z),
                new float3(end.x, end.y, end.z),
            };

            var itemSize = 3;
            var numItems = 2;
            var posBuffer = gl.CreateBuffer();

            gl.EnableVertexAttribArray((uint) Helper.VertexAttribLocation);
            gl.BindBuffer(ARRAY_BUFFER, posBuffer);
            gl.BufferData(ARRAY_BUFFER, vertices, STATIC_DRAW);
            gl.VertexAttribPointer((uint) Helper.VertexAttribLocation, itemSize, FLOAT, false, 0, 0);

            gl.DrawArrays(LINE_STRIP, 0, numItems);
            gl.DisableVertexAttribArray((uint) Helper.VertexAttribLocation);
        }

        /// <summary>
        /// Gets the content of the buffer.
        /// </summary>
        /// <param name="quad">The Rectangle where the content is draw into.</param>
        /// <param name="texId">The tex identifier.</param>
        public void GetBufferContent(Common.Rectangle quad, ITextureHandle texId)
        {
            gl.BindTexture(TEXTURE_2D, ((TextureHandle)texId).Handle);
            gl.CopyTexImage2D(TEXTURE_2D, 0, RGBA, quad.Left, quad.Top, quad.Width, quad.Height, 0);
        }

        /// <summary>
        /// Creates the mesh implementation.
        /// </summary>
        /// <returns>The <see cref="IMeshImp" /> instance.</returns>
        public IMeshImp CreateMeshImp()
        {
            return new MeshImp();
        }

        internal static uint BlendOperationToOgl(BlendOperation bo)
        {
            switch (bo)
            {
                case BlendOperation.Add:
                    return FUNC_ADD;
                case BlendOperation.Subtract:
                    return FUNC_SUBTRACT;
                case BlendOperation.ReverseSubtract:
                    return FUNC_REVERSE_SUBTRACT;
                case BlendOperation.Minimum:
                    throw new NotSupportedException("MIN blending mode not supported in WebGL!");
                case BlendOperation.Maximum:
                    throw new NotSupportedException("MAX blending mode not supported in WebGL!");
                default:
                    throw new ArgumentOutOfRangeException("bo");
            }
        }

        internal static BlendOperation BlendOperationFromOgl(uint bom)
        {
            switch (bom)
            {
                case FUNC_ADD:
                    return BlendOperation.Add;
                case FUNC_SUBTRACT:
                    return BlendOperation.Subtract;
                case FUNC_REVERSE_SUBTRACT:
                    return BlendOperation.ReverseSubtract;
                default:
                    throw new ArgumentOutOfRangeException("bom");
            }
        }

        internal static uint BlendToOgl(Blend blend, bool isForAlpha = false)
        {
            switch (blend)
            {
                case Blend.Zero:
                    return (uint)ZERO;
                case Blend.One:
                    return (uint)ONE;
                case Blend.SourceColor:
                    return (uint)SRC_COLOR;
                case Blend.InverseSourceColor:
                    return (uint)ONE_MINUS_SRC_COLOR;
                case Blend.SourceAlpha:
                    return (uint)SRC_ALPHA;
                case Blend.InverseSourceAlpha:
                    return (uint)ONE_MINUS_SRC_ALPHA;
                case Blend.DestinationAlpha:
                    return (uint)DST_ALPHA;
                case Blend.InverseDestinationAlpha:
                    return (uint)ONE_MINUS_DST_ALPHA;
                case Blend.DestinationColor:
                    return (uint)DST_COLOR;
                case Blend.InverseDestinationColor:
                    return (uint)ONE_MINUS_DST_COLOR;
                case Blend.BlendFactor:
                    return (uint)((isForAlpha) ? CONSTANT_ALPHA : CONSTANT_COLOR);
                case Blend.InverseBlendFactor:
                    return (uint)((isForAlpha) ? ONE_MINUS_CONSTANT_ALPHA : ONE_MINUS_CONSTANT_COLOR);
                // Ignored...
                // case Blend.SourceAlphaSaturated:
                //     break;
                //case Blend.Bothsrcalpha:
                //    break;
                //case Blend.BothInverseSourceAlpha:
                //    break;
                //case Blend.SourceColor2:
                //    break;
                //case Blend.InverseSourceColor2:
                //    break;
                default:
                    throw new ArgumentOutOfRangeException("blend");
            }
        }

        internal static Blend BlendFromOgl(uint bf)
        {
            switch (bf)
            {
                case ZERO:
                    return Blend.Zero;
                case ONE:
                    return Blend.One;
                case SRC_COLOR:
                    return Blend.SourceColor;
                case ONE_MINUS_SRC_COLOR:
                    return Blend.InverseSourceColor;
                case SRC_ALPHA:
                    return Blend.SourceAlpha;
                case ONE_MINUS_SRC_ALPHA:
                    return Blend.InverseSourceAlpha;
                case DST_ALPHA:
                    return Blend.DestinationAlpha;
                case ONE_MINUS_DST_ALPHA:
                    return Blend.InverseDestinationAlpha;
                case DST_COLOR:
                    return Blend.DestinationColor;
                case ONE_MINUS_DST_COLOR:
                    return Blend.InverseDestinationColor;
                case CONSTANT_COLOR:
                case CONSTANT_ALPHA:
                    return Blend.BlendFactor;
                case ONE_MINUS_CONSTANT_COLOR:
                case ONE_MINUS_CONSTANT_ALPHA:
                    return Blend.InverseBlendFactor;
                default:
                    throw new ArgumentOutOfRangeException("blend");
            }
        }

        /// <summary>
        /// Sets the RenderState object onto the current OpenGL based RenderContext.
        /// </summary>
        /// <param name="renderState">State of the render(enum).</param>
        /// <param name="value">The value. See <see cref="RenderState"/> for detailed information. </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// value
        /// or
        /// value
        /// or
        /// value
        /// or
        /// renderState
        /// </exception>
        public void SetRenderState(RenderState renderState, uint value)
        {
            switch (renderState)
            {
                case RenderState.FillMode:
                    {
                        if (value != (uint) FillMode.Solid)
                            throw new NotSupportedException("Line or Point fill mode (glPolygonMode) not supported in WebGL!");
                    }
                    break;
                case RenderState.CullMode:
                    {
                        switch ((Cull)value)
                        {
                            case Cull.None:
                                gl.Disable(CULL_FACE);
                                gl.FrontFace(NONE);
                                break;
                            case Cull.Clockwise:
                                gl.Enable(CULL_FACE);
                                gl.FrontFace(CW);
                                break;
                            case Cull.Counterclockwise:
                                gl.Enable(CULL_FACE);
                                gl.FrontFace(CCW);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("value");
                        }
                    }
                    break;
                case RenderState.Clipping:
                    // clipping is always on in OpenGL - This state is simply ignored
                    break;
                case RenderState.ZFunc:
                    {
                        uint df;
                        switch ((Compare)value)
                        {
                            case Compare.Never:
                                df = NEVER;
                                break;
                            case Compare.Less:
                                df = LESS;
                                break;
                            case Compare.Equal:
                                df = EQUAL;
                                break;
                            case Compare.LessEqual:
                                df = LEQUAL;
                                break;
                            case Compare.Greater:
                                df = GREATER;
                                break;
                            case Compare.NotEqual:
                                df = NOTEQUAL;
                                break;
                            case Compare.GreaterEqual:
                                df = GEQUAL;
                                break;
                            case Compare.Always:
                                df = ALWAYS;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("value");
                        }
                        gl.DepthFunc(df);
                    }
                    break;
                case RenderState.ZEnable:
                    if (value == 0)
                        gl.Disable(DEPTH_TEST);
                    else
                        gl.Enable(DEPTH_TEST);
                    break;
                case RenderState.ZWriteEnable:
                    gl.DepthMask(value != 0);
                    break;
                case RenderState.AlphaBlendEnable:
                    if (value == 0)
                        gl.Disable(BLEND);
                    else
                        gl.Enable(BLEND);
                    break;
                case RenderState.BlendOperation:
                    uint alphaMode;
                    alphaMode = (uint) gl.GetParameter(BLEND_EQUATION_ALPHA);
                    gl.BlendEquationSeparate(BlendOperationToOgl((BlendOperation)value), alphaMode);
                    break;
                case RenderState.BlendOperationAlpha:
                    uint rgbMode;
                    rgbMode = (uint) gl.GetParameter(BLEND_EQUATION_RGB);
                    gl.BlendEquationSeparate(rgbMode, BlendOperationToOgl((BlendOperation)value));
                    break;
                case RenderState.SourceBlend:
                    {
                        uint rgbDst, alphaSrc, alphaDst;
                        rgbDst = (uint) gl.GetParameter(BLEND_DST_RGB);
                        alphaSrc = (uint) gl.GetParameter(BLEND_SRC_ALPHA);
                        alphaDst = (uint) gl.GetParameter(BLEND_DST_ALPHA);
                        gl.BlendFuncSeparate(BlendToOgl((Blend)value), rgbDst, alphaSrc, alphaDst);
                    }
                    break;
                case RenderState.DestinationBlend:
                    {
                        uint rgbSrc, alphaSrc, alphaDst;
                        rgbSrc = (uint) gl.GetParameter(BLEND_SRC_RGB);
                        alphaSrc = (uint) gl.GetParameter(BLEND_SRC_ALPHA);
                        alphaDst = (uint) gl.GetParameter(BLEND_DST_ALPHA);
                        gl.BlendFuncSeparate(rgbSrc, BlendToOgl((Blend)value), alphaSrc, alphaDst);
                    }
                    break;
                case RenderState.SourceBlendAlpha:
                    {
                        uint rgbSrc, rgbDst, alphaDst;
                        rgbSrc = (uint) gl.GetParameter(BLEND_SRC_RGB);
                        rgbDst = (uint) gl.GetParameter(BLEND_DST_RGB);
                        alphaDst = (uint) gl.GetParameter(BLEND_DST_ALPHA);
                        gl.BlendFuncSeparate(rgbSrc, rgbDst, BlendToOgl((Blend)value, true), alphaDst);
                    }
                    break;
                case RenderState.DestinationBlendAlpha:
                    {
                        uint rgbSrc, rgbDst, alphaSrc;
                        rgbSrc = (uint) gl.GetParameter(BLEND_SRC_RGB);
                        rgbDst = (uint) gl.GetParameter(BLEND_DST_RGB);
                        alphaSrc = (uint) gl.GetParameter(BLEND_SRC_ALPHA);
                        gl.BlendFuncSeparate(rgbSrc, rgbDst, alphaSrc, BlendToOgl((Blend)value, true));
                    }
                    break;
                case RenderState.BlendFactor:
                    var blendcolor =  ColorUint.Tofloat4((ColorUint) value);
                    gl.BlendColor(blendcolor.r, blendcolor.g, blendcolor.b, blendcolor.a);
                    break;
                /* TODO: Implement texture wrapping rather as a texture property than a "global" render state. This is most
                 * convenient to implment with OpenGL/TK and easier to mimic in DirectX than the other way round.
                case RenderState.Wrap0:
                    break;
                case RenderState.Wrap1:
                    break;
                case RenderState.Wrap2:
                    break;
                case RenderState.Wrap3:
                    break;
                */
            default:
                    throw new ArgumentOutOfRangeException("renderState");
            }           
        }

        /// <summary>
        /// Retrieves the current value for the given RenderState that is applied to the current WebGL based RenderContext.
        /// </summary>
        /// <param name="renderState">The RenderState setting to be retrieved. See <see cref="RenderState"/> for further information.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// pm;Value  + ((PolygonMode)pm) +  not handled
        /// or
        /// depFunc;Value  + ((DepthFunction)depFunc) +  not handled
        /// or
        /// renderState
        /// </exception>
        public uint GetRenderState(RenderState renderState)
        {
            switch (renderState)
            {
                case RenderState.FillMode:
                    {
                        // Only solid polygon fill is supported by WebGL
                        return (uint) FillMode.Solid;
                    }
                case RenderState.CullMode:
                    {
                        uint cullFace;
                        cullFace = (uint) gl.GetParameter(CULL_FACE);
                        if (cullFace == 0)
                            return (uint)Cull.None;
                        uint frontFace;
                        frontFace = (uint) gl.GetParameter(FRONT_FACE);
                        if (frontFace == CW)
                            return (uint) Cull.Clockwise;
                        return (uint) Cull.Counterclockwise;
                    }
                case RenderState.Clipping:
                    // clipping is always on in OpenGL - This state is simply ignored
                    return 1; // == true
                case RenderState.ZFunc:
                    {
                        uint depFunc;
                        depFunc = (uint) gl.GetParameter(DEPTH_FUNC);
                        Compare ret;
                        switch (depFunc)
                        {
                            case NEVER:
                                ret = Compare.Never;
                                break;
                            case LESS:
                                ret = Compare.Less;
                                break;
                            case EQUAL:
                                ret = Compare.Equal;
                                break;
                            case LEQUAL:
                                ret = Compare.LessEqual;
                                break;
                            case GREATER:
                                ret = Compare.Greater;
                                break;
                            case NOTEQUAL:
                                ret = Compare.NotEqual;
                                break;
                            case GEQUAL:
                                ret = Compare.GreaterEqual;
                                break;
                            case ALWAYS:
                                ret = Compare.Always;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("depFunc", "Value " + depFunc + " not handled");
                        }
                        return (uint) ret;
                    }
                case RenderState.ZEnable:
                    {
                        uint depTest;
                        depTest = (uint) gl.GetParameter(DEPTH_TEST);
                        return (uint)(depTest);
                    }
                case RenderState.ZWriteEnable:
                    {
                        uint depWriteMask;
                        depWriteMask = (uint) gl.GetParameter(DEPTH_WRITEMASK);
                        return (uint)(depWriteMask);
                    }
                case RenderState.AlphaBlendEnable:
                    {
                        uint blendEnable;
                        blendEnable  = (uint) gl.GetParameter(BLEND);
                        return (uint)(blendEnable);
                    }
                case RenderState.BlendOperation:
                    {
                        uint rgbMode;
                        rgbMode  = (uint) gl.GetParameter(BLEND_EQUATION_RGB);
                        return (uint)BlendOperationFromOgl(rgbMode);
                    }
                case RenderState.BlendOperationAlpha:
                    {
                        uint alphaMode;
                        alphaMode  = (uint) gl.GetParameter(BLEND_EQUATION_ALPHA);
                        return (uint)BlendOperationFromOgl(alphaMode);
                    }
                case RenderState.SourceBlend:
                    {
                        uint rgbSrc;
                        rgbSrc  = (uint) gl.GetParameter(BLEND_SRC_RGB);
                        return (uint)BlendFromOgl(rgbSrc);
                    }
                case RenderState.DestinationBlend:
                    {
                        uint rgbDst;
                        rgbDst  = (uint) gl.GetParameter(BLEND_DST_RGB);
                        return (uint)BlendFromOgl(rgbDst);
                    }
                case RenderState.SourceBlendAlpha:
                    {
                        uint alphaSrc;
                        alphaSrc  = (uint) gl.GetParameter(BLEND_SRC_ALPHA );
                        return (uint)BlendFromOgl(alphaSrc);
                    }
                case RenderState.DestinationBlendAlpha:
                    {
                        uint alphaDst;
                        alphaDst  = (uint) gl.GetParameter(BLEND_DST_ALPHA);
                        return (uint)BlendFromOgl(alphaDst);
                    }
                case RenderState.BlendFactor:
                    {
                        var col = (float[])gl.GetParameter(BLEND_COLOR);
                        var uintCol = new ColorUint(col);
                        return (uint)uintCol.ToRgba();
                    }
                default:
                    throw new ArgumentOutOfRangeException("renderState");
            }
        }

        /// <summary>
        /// Copies Depthbuffer from a deferred buffer texture
        /// </summary>
        /// <param name="texture"></param>
        public void CopyDepthBufferFromDeferredBuffer(ITextureHandle texture)
        {
            /*
            var textureImp = (TextureHandle)texture;

            gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, textureImp.GDepthRenderbufferHandle);
            gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0); // Write to default framebuffer
                                                                      // copy depth
            var width = textureImp.TextureWidth;
            var height = textureImp.TextureHeight;
            gl.BlitFramebuffer(0, 0, width, height, 0, 0, width, height, ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
            */
            throw new NotImplementedException("TODO: See if WebGLDotNET supports required operations.");
        }        
       

        public void SetRenderTarget(IRenderTarget renderTarget)
        {
            if (renderTarget == null || renderTarget.RenderTextures.All(x => x == null))
            {
                gl2.BindFramebuffer(FRAMEBUFFER, null);
                return;
            }

            WebGLFramebuffer gBuffer;            

            if (renderTarget.GBufferHandle == null)
            {
                renderTarget.GBufferHandle = new FrameBufferHandle();
                gBuffer = CreateFrameBuffer(renderTarget);
                ((FrameBufferHandle)renderTarget.GBufferHandle).FrameBuffer = gBuffer;
            }
            else
            {
                gBuffer = ((FrameBufferHandle)renderTarget.GBufferHandle).FrameBuffer;
                gl2.BindFramebuffer(FRAMEBUFFER, gBuffer);
            }

            WebGLRenderbuffer gDepthRenderbuffer;
            if (renderTarget.DepthBufferHandle == null)
            {
                renderTarget.DepthBufferHandle = new RenderBufferHandle();
                // Create and attach depth buffer (renderbuffer)
                gDepthRenderbuffer = CreateDepthRenderBuffer(renderTarget);
                ((RenderBufferHandle)renderTarget.DepthBufferHandle).RenderBuffer = gDepthRenderbuffer;
            }
            else
            {
                gDepthRenderbuffer = ((RenderBufferHandle)renderTarget.DepthBufferHandle).RenderBuffer;
                gl2.BindRenderbuffer(RENDERBUFFER, gDepthRenderbuffer);
            }

            Clear(ClearFlags.Color);
            Clear(ClearFlags.Depth);            

            if (gl2.CheckFramebufferStatus(FRAMEBUFFER) != FRAMEBUFFER_COMPLETE)
            {
                throw new Exception($"Error creating RenderTarget: {gl.GetError()}, {gl.CheckFramebufferStatus(FRAMEBUFFER)}");
            }
        }

        private WebGLRenderbuffer CreateDepthRenderBuffer(IRenderTarget renderTarget)
        {
            gl2.Enable(DEPTH_TEST);

            var gDepthRenderbuffer = gl2.CreateRenderbuffer();            
            gl2.BindRenderbuffer(RENDERBUFFER, gDepthRenderbuffer);
            gl2.RenderbufferStorage(RENDERBUFFER, DEPTH_COMPONENT24, (int)renderTarget.TextureResolution, (int)renderTarget.TextureResolution);
            gl2.FramebufferRenderbuffer(FRAMEBUFFER, DEPTH_ATTACHMENT, RENDERBUFFER, gDepthRenderbuffer);
            return gDepthRenderbuffer;
        }


        private WebGLFramebuffer CreateFrameBuffer(IRenderTarget renderTarget)
        {
            var gBuffer = gl2.CreateFramebuffer();
            gl2.BindFramebuffer(FRAMEBUFFER, gBuffer);

            var attachements = new List<uint>();

            for (int i = 0; i < renderTarget.RenderTextures.Length; i++)
            {
                var tex = renderTarget.RenderTextures[i];
                if (tex == null) continue;

                if (tex.TextureHandle == null)
                {
                    var iTexHandle = CreateTexture(tex);
                    tex.TextureHandle = new TextureHandle();
                    ((TextureHandle)tex.TextureHandle).Handle = ((TextureHandle)iTexHandle).Handle;
                }

                gl2.FramebufferTexture2D(FRAMEBUFFER, COLOR_ATTACHMENT0 + (uint)i, TEXTURE_2D, ((TextureHandle)tex.TextureHandle).Handle, 0);
                attachements.Add(COLOR_ATTACHMENT0 + (uint)i);
            }

            gl2.DrawBuffers(attachements.ToArray());

            return gBuffer;
        }


        /// <summary>
        /// Set the Viewport of the rendering output window by x,y position and width,height parameters. 
        /// The Viewport is the portion of the final image window.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void Viewport(int x, int y, int width, int height)
        {
            gl.Viewport(x, y, width, height);
        }

        /// <summary>
        /// Enable or disable Color channels to be written to the frame buffer (final image).
        /// Use this function as a color channel filter for the final image.
        /// </summary>
        /// <param name="red">if set to <c>true</c> [red].</param>
        /// <param name="green">if set to <c>true</c> [green].</param>
        /// <param name="blue">if set to <c>true</c> [blue].</param>
        /// <param name="alpha">if set to <c>true</c> [alpha].</param>
        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            gl.ColorMask(red, green, blue, alpha);
        }

        /// <summary>
        /// Returns the capabilities of the underlying graphics hardware
        /// </summary>
        /// <param name="capability"></param>
        /// <returns>uint</returns>
        public uint GetHardwareCapabilities(HardwareCapability capability)
        {
            switch (capability)
            {
                case HardwareCapability.DefferedPossible:
                    return 0U;
                case HardwareCapability.Buffersize:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(capability), capability, null);
            }
        }

        #endregion

        #region Picking related Members

        /// <summary>
        /// Retrieves a sub-image of the given region.
        /// </summary>
        /// <param name="x">The x value of the start of the region.</param>
        /// <param name="y">The y value of the start of the region.</param>
        /// <param name="w">The width to copy.</param>
        /// <param name="h">The height to copy.</param>
        /// <returns>The specified sub-image</returns>
        public IImageData GetPixelColor(int x, int y, int w = 1, int h = 1)
        {
            Fusee.Base.Core.ImageData image = Fusee.Base.Core.ImageData.CreateImage(w, h, ColorUint.Black);
            var pixelDataTA = Uint8Array.From(image.PixelData);
            gl.ReadPixels(x, y, w, h, RGB /* yuk, yuk ??? */, UNSIGNED_BYTE, pixelDataTA);
            return image;
        }

        /// <summary>
        /// Retrieves the Z-value at the given pixel position.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <returns>The Z value at (x, y).</returns>
        public float GetPixelDepth(int x, int y)
        {
            float[] depth = new float[1];
            var depthTA = Float32Array.From(depth);
            gl.ReadPixels(x, y, 1, 1, DEPTH_COMPONENT, UNSIGNED_BYTE, depthTA);

            return depth[0];
        }

        #endregion
    }
}