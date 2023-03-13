using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Imp.SharedAll;
using Fusee.Math.Core;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IRenderContextImp" /> interface for usage with OpenTK framework.
    /// </summary>
    public class RenderContextImp : IRenderContextImp
    {
        /// <summary>
        /// Constant id that describes the renderer. This can be used in shaders to do platform dependent things.
        /// </summary>
        public FuseePlatformId FuseePlatformId { get; } = FuseePlatformId.Desktop;

        private int _textureCountPerShader;
        private readonly Dictionary<int, int> _shaderParam2TexUnit;

        private BlendEquationMode _blendEquationAlpha;
        private BlendEquationMode _blendEquationRgb;
        private BlendingFactorSrc _blendSrcRgb;
        private BlendingFactorDest _blendDstRgb;
        private BlendingFactorSrc _blendSrcAlpha;
        private BlendingFactorDest _blendDstAlpha;

        private bool _isCullEnabled;
        private bool _isPtRenderingEnabled;
        private bool _isLineSmoothEnabled;
        private bool _isMultisampleEnabled;
        private int _lastBoundFbo;

#if DEBUG
        private static DebugProc _openGlDebugDelegate;
#endif

        /// <summary>
        /// Current workaround to differentiate between "Desktop" and "Mesa" (Software Rasterizer used for the RenderTests in the CI)
        /// </summary>
        /// <param name="renderCanvas"></param>
        /// <param name="platformID"></param>
        public RenderContextImp(IRenderCanvasImp renderCanvas, FuseePlatformId platformID) : this(renderCanvas)
        {
            FuseePlatformId = platformID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContextImp"/> class.
        /// </summary>
        /// <param name="renderCanvas">The render canvas interface.</param>
        public RenderContextImp(IRenderCanvasImp renderCanvas)
        {
            _textureCountPerShader = 0;
            _shaderParam2TexUnit = new Dictionary<int, int>();

#if DEBUG
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);

            _openGlDebugDelegate = new DebugProc(OpenGLDebugCallback);

            GL.DebugMessageCallback(_openGlDebugDelegate, IntPtr.Zero);
            GL.DebugMessageControl(DebugSourceControl.DontCare, DebugTypeControl.DontCare, DebugSeverityControl.DebugSeverityNotification, 0, Array.Empty<int>(), false);
#endif

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            //Needed for rendering more than one viewport.
            GL.Enable(EnableCap.ScissorTest);

            GL.GetInteger(GetPName.BlendSrcAlpha, out int blendSrcAlpha);
            GL.GetInteger(GetPName.BlendDstAlpha, out int blendDstAlpha);
            GL.GetInteger(GetPName.BlendDstRgb, out int blendDstRgb);
            GL.GetInteger(GetPName.BlendSrcRgb, out int blendSrcRgb);
            GL.GetInteger(GetPName.BlendEquationAlpha, out int blendEqA);
            GL.GetInteger(GetPName.BlendEquationRgb, out int blendEqRgb);

            _blendDstRgb = (BlendingFactorDest)blendDstRgb;
            _blendSrcRgb = (BlendingFactorSrc)blendSrcRgb;
            _blendSrcAlpha = (BlendingFactorSrc)blendSrcAlpha;
            _blendDstAlpha = (BlendingFactorDest)blendDstAlpha;
            _blendEquationAlpha = (BlendEquationMode)blendEqA;
            _blendEquationRgb = (BlendEquationMode)blendEqRgb;

            Diagnostics.Debug(GL.GetString(StringName.Vendor) + " - " + GL.GetString(StringName.Renderer) + " - " + GL.GetString(StringName.Version));
#if DEBUG
            var numExtensions = GL.GetInteger(GetPName.NumExtensions);
            var extensions = new string[numExtensions];

            for (int i = 0; i < numExtensions; i++)
            {
                extensions[i] = GL.GetString(StringNameIndexed.Extensions, i);
            }

            Diagnostics.Verbose(string.Join(';', extensions));
#endif
        }

#if DEBUG
        private static void OpenGLDebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            Diagnostics.Debug($"{Marshal.PtrToStringAnsi(message, length)}\n\tid:{id} severity:{severity} type:{type} source:{source}\n");
        }
#endif

        #region Image data related Members

        private OpenTK.Graphics.OpenGL.TextureCompareMode GetTexComapreMode(Common.TextureCompareMode compareMode)
        {
            return compareMode switch
            {
                Common.TextureCompareMode.None => OpenTK.Graphics.OpenGL.TextureCompareMode.None,
                Common.TextureCompareMode.CompareRefToTexture => OpenTK.Graphics.OpenGL.TextureCompareMode.CompareRefToTexture,
                _ => throw new ArgumentException("Invalid compare mode."),
            };
        }

        private Tuple<TextureMinFilter, TextureMagFilter> GetMinMagFilter(TextureFilterMode filterMode)
        {
            TextureMinFilter minFilter;
            TextureMagFilter magFilter;

            switch (filterMode)
            {
                case TextureFilterMode.Nearest:
                    minFilter = TextureMinFilter.Nearest;
                    magFilter = TextureMagFilter.Nearest;
                    break;
                default:
                case TextureFilterMode.Linear:
                    minFilter = TextureMinFilter.Linear;
                    magFilter = TextureMagFilter.Linear;
                    break;
                case TextureFilterMode.NearestMipmapNearest:
                    minFilter = TextureMinFilter.NearestMipmapNearest;
                    magFilter = TextureMagFilter.Nearest;
                    break;
                case TextureFilterMode.LinearMipmapNearest:
                    minFilter = TextureMinFilter.LinearMipmapNearest;
                    magFilter = TextureMagFilter.Linear;
                    break;
                case TextureFilterMode.NearestMipmapLinear:
                    minFilter = TextureMinFilter.NearestMipmapLinear;
                    magFilter = TextureMagFilter.Nearest;
                    break;
                case TextureFilterMode.LinearMipmapLinear:
                    minFilter = TextureMinFilter.LinearMipmapLinear;
                    magFilter = TextureMagFilter.Linear;
                    break;
            }

            return new Tuple<TextureMinFilter, TextureMagFilter>(minFilter, magFilter);
        }

        private DepthFunction GetDepthCompareFunc(Compare compareFunc)
        {
            return compareFunc switch
            {
                Compare.Never => DepthFunction.Never,
                Compare.Less => DepthFunction.Less,
                Compare.Equal => DepthFunction.Equal,
                Compare.LessEqual => DepthFunction.Lequal,
                Compare.Greater => DepthFunction.Greater,
                Compare.NotEqual => DepthFunction.Notequal,
                Compare.GreaterEqual => DepthFunction.Gequal,
                Compare.Always => DepthFunction.Always,
                _ => throw new ArgumentOutOfRangeException("value"),
            };
        }

        private OpenTK.Graphics.OpenGL.TextureWrapMode GetWrapMode(Common.TextureWrapMode wrapMode)
        {
            return wrapMode switch
            {
                Common.TextureWrapMode.MirroredRepeat => OpenTK.Graphics.OpenGL.TextureWrapMode.MirroredRepeat,
                Common.TextureWrapMode.ClampToEdge => OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge,
                Common.TextureWrapMode.ClampToBorder => OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToBorder,
                _ => OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat,
            };
        }

        private SizedInternalFormat GetSizedInteralFormat(ImagePixelFormat format)
        {
            return format.ColorFormat switch
            {
                ColorFormat.RGBA => SizedInternalFormat.Rgba8,
                ColorFormat.fRGBA16 => SizedInternalFormat.Rgba16f,
                ColorFormat.fRGBA32 => SizedInternalFormat.Rgba32f,
                ColorFormat.iRGBA32 => SizedInternalFormat.Rgba32i,
                ColorFormat.fRGB16 => (SizedInternalFormat)All.Rgb16f,
                ColorFormat.fRGB32 => (SizedInternalFormat)All.Rgb32f,
                ColorFormat.Depth24 => (SizedInternalFormat)All.DepthComponent24,
                ColorFormat.Depth16 => (SizedInternalFormat)All.DepthComponent16,
                ColorFormat.RGB => (SizedInternalFormat)All.Rgb8,
                ColorFormat.Intensity => (SizedInternalFormat)All.R8,
                _ => throw new ArgumentOutOfRangeException("SizedInternalFormat not supported. Try to use a format with r,g,b and a components."),
            };
        }

        private TexturePixelInfo GetTexturePixelInfo(ImagePixelFormat pixelFormat)
        {
            PixelInternalFormat internalFormat;
            PixelFormat format;
            PixelType pxType;

            //The wrong row alignment will lead to malformed textures.
            //See https://www.khronos.org/opengl/wiki/Common_Mistakes#Texture_upload_and_pixel_reads
            //and https://www.khronos.org/opengl/wiki/Pixel_Transfer#Pixel_layout
            int rowAlignment = 4;

            switch (pixelFormat.ColorFormat)
            {
                case ColorFormat.RGBA:
                    internalFormat = PixelInternalFormat.Rgba;
                    format = PixelFormat.Rgba;
                    pxType = PixelType.UnsignedByte;
                    break;

                case ColorFormat.RGB:
                    internalFormat = PixelInternalFormat.Rgb;
                    format = PixelFormat.Rgb;
                    pxType = PixelType.UnsignedByte;
                    rowAlignment = 1;
                    break;

                // TODO: Handle Alpha-only / Intensity-only and AlphaIntensity correctly.
                case ColorFormat.Intensity:
                    internalFormat = PixelInternalFormat.R8;
                    format = PixelFormat.Red;
                    pxType = PixelType.UnsignedByte;
                    rowAlignment = 1;
                    break;

                case ColorFormat.Depth24:
                    internalFormat = PixelInternalFormat.DepthComponent24;
                    format = PixelFormat.DepthComponent;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.Depth16:
                    internalFormat = PixelInternalFormat.DepthComponent16;
                    format = PixelFormat.DepthComponent;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.uiRgb8:
                    internalFormat = PixelInternalFormat.Rgba8ui;
                    format = PixelFormat.RgbaInteger;
                    pxType = PixelType.UnsignedByte;
                    rowAlignment = 1;
                    break;

                case ColorFormat.fRGB32:
                    internalFormat = PixelInternalFormat.Rgb32f;
                    format = PixelFormat.Rgb;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.fRGB16:
                    internalFormat = PixelInternalFormat.Rgb16f;
                    format = PixelFormat.Rgb;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.fRGBA16:
                    internalFormat = PixelInternalFormat.Rgba16f;
                    format = PixelFormat.Rgba;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.fRGBA32:
                    internalFormat = PixelInternalFormat.Rgba32f;
                    format = PixelFormat.Rgba;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.iRGBA32:
                    internalFormat = PixelInternalFormat.Rgba32i;
                    format = PixelFormat.RgbaInteger;
                    pxType = PixelType.Int;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("CreateTexture: Image pixel format not supported");
            }

            return new TexturePixelInfo()
            {
                Format = format,
                InternalFormat = internalFormat,
                PxType = pxType,
                RowAlignment = rowAlignment
            };
        }


        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="tex">A given IWritableTexture object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableTexture tex)
        {
            if (tex is WritableTexture wt)
                return CreateTexture(wt);
            if (tex is WritableMultisampleTexture mswt)
                return CreateTexture(mswt);

            throw new NotImplementedException($"CreateTexture typeof({tex}) not found!");
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableArrayTexture img)
        {
            GL.CreateTextures(TextureTarget.Texture2DArray, 1, out int id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = (int)glMinMagFilter.Item1;
            var magFilter = (int)glMinMagFilter.Item2;
            var glWrapMode = (int)GetWrapMode(img.WrapMode);

            GL.TextureStorage3D(id, 1, GetSizedInteralFormat(img.PixelFormat), img.Width, img.Height, img.Layers);

            if (img.DoGenerateMipMaps)
                GL.GenerateTextureMipmap(id);

            var compareMode = (int)GetTexComapreMode(img.CompareMode);
            var compareFunc = (int)GetDepthCompareFunc(img.CompareFunc);
            GL.TextureParameterI(id, TextureParameterName.TextureCompareMode, ref compareMode);
            GL.TextureParameterI(id, TextureParameterName.TextureCompareFunc, ref compareFunc);
            GL.TextureParameterI(id, TextureParameterName.TextureMinFilter, ref minFilter);
            GL.TextureParameterI(id, TextureParameterName.TextureMagFilter, ref magFilter);
            GL.TextureParameterI(id, TextureParameterName.TextureWrapS, ref glWrapMode);
            GL.TextureParameterI(id, TextureParameterName.TextureWrapT, ref glWrapMode);

            ITextureHandle texID = new TextureHandle { TexId = id };

            return texID;
        }

        /// <summary>
        /// Creates a new CubeMap and binds it to the shader.
        /// </summary>
        /// <param name="img">A given IWritableCubeMap object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableCubeMap img)
        {
            GL.CreateTextures(TextureTarget.TextureCubeMap, 1, out int id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = (int)glMinMagFilter.Item1;
            var magFilter = (int)glMinMagFilter.Item2;

            var glWrapMode = (int)GetWrapMode(img.WrapMode);

            GL.TextureStorage2D(id, 1, GetSizedInteralFormat(img.PixelFormat), img.Width, img.Height);

            var compareMode = (int)GetTexComapreMode(img.CompareMode);
            var compareFunc = (int)GetDepthCompareFunc(img.CompareFunc);
            GL.TextureParameterI(id, TextureParameterName.TextureCompareMode, ref compareMode);
            GL.TextureParameterI(id, TextureParameterName.TextureCompareFunc, ref compareFunc);
            GL.TextureParameterI(id, TextureParameterName.TextureMagFilter, ref magFilter);
            GL.TextureParameterI(id, TextureParameterName.TextureMinFilter, ref minFilter);
            GL.TextureParameterI(id, TextureParameterName.TextureWrapS, ref glWrapMode);
            GL.TextureParameterI(id, TextureParameterName.TextureWrapT, ref glWrapMode);
            GL.TextureParameterI(id, TextureParameterName.TextureWrapR, ref glWrapMode);

            ITextureHandle texID = new TextureHandle { TexId = id };

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ITexture object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(ITexture img)
        {
            if (img is Texture1D wt1D)
                return CreateTexture(wt1D);
            else
                return CreateTexture((Texture)img);

            throw new ArgumentException($"{img} has an unknown texture type.");

        }

        private ITextureHandle CreateTexture(Texture img)
        {
            GL.CreateTextures(TextureTarget.Texture2D, 1, out int id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = (int)glMinMagFilter.Item1;
            var magFilter = (int)glMinMagFilter.Item2;

            var glWrapMode = (int)GetWrapMode(img.WrapMode);

            var pxInfo = GetTexturePixelInfo(img.ImageData.PixelFormat);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, pxInfo.RowAlignment);
            GL.TextureStorage2D(id, 1, GetSizedInteralFormat(img.ImageData.PixelFormat), img.ImageData.Width, img.ImageData.Height);
            GL.TextureSubImage2D(id, 0, 0, 0, img.ImageData.Width, img.ImageData.Height, pxInfo.Format, pxInfo.PxType, img.ImageData.PixelData);

            if (img.DoGenerateMipMaps)
                GL.GenerateTextureMipmap(id);

            GL.TextureParameterI(id, TextureParameterName.TextureMinFilter, ref minFilter);
            GL.TextureParameterI(id, TextureParameterName.TextureMagFilter, ref magFilter);
            GL.TextureParameterI(id, TextureParameterName.TextureWrapS, ref glWrapMode);
            GL.TextureParameterI(id, TextureParameterName.TextureWrapT, ref glWrapMode);
            GL.TextureParameterI(id, TextureParameterName.TextureWrapR, ref glWrapMode);

            ITextureHandle texID = new TextureHandle { TexId = id };

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="tex">A given IWritableTexture object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        private ITextureHandle CreateTexture(WritableTexture tex)
        {
            GL.CreateTextures(TextureTarget.Texture2D, 1, out int id);

            var glMinMagFilter = GetMinMagFilter(tex.FilterMode);
            var minFilter = (int)glMinMagFilter.Item1;
            var magFilter = (int)glMinMagFilter.Item2;
            var glWrapMode = (int)GetWrapMode(tex.WrapMode);

            GL.TextureStorage2D(id, 1, GetSizedInteralFormat(tex.PixelFormat), tex.Width, tex.Height);

            if (tex.DoGenerateMipMaps)
                GL.GenerateTextureMipmap(id);

            var compareMode = (int)GetTexComapreMode(tex.CompareMode);
            var compareFunc = (int)GetDepthCompareFunc(tex.CompareFunc);
            GL.TextureParameterI(id, TextureParameterName.TextureCompareMode, ref compareMode);
            GL.TextureParameterI(id, TextureParameterName.TextureCompareFunc, ref compareFunc);
            GL.TextureParameterI(id, TextureParameterName.TextureMinFilter, ref minFilter);
            GL.TextureParameterI(id, TextureParameterName.TextureMagFilter, ref magFilter);
            GL.TextureParameterI(id, TextureParameterName.TextureWrapS, ref glWrapMode);
            GL.TextureParameterI(id, TextureParameterName.TextureWrapT, ref glWrapMode);

            ITextureHandle texID = new TextureHandle { TexId = id };

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="tex">A given IWritableTexture object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(Texture1D tex)
        {
            GL.CreateTextures(TextureTarget.Texture1D, 1, out int id);

            var glMinMagFilter = GetMinMagFilter(tex.FilterMode);
            var minFilter = (int)glMinMagFilter.Item1;
            var magFilter = (int)glMinMagFilter.Item2;

            var glWrapMode = (int)GetWrapMode(tex.WrapMode);

            var pxInfo = GetTexturePixelInfo(tex.ImageData.PixelFormat);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, pxInfo.RowAlignment);
            GL.TextureStorage1D(id, 1, GetSizedInteralFormat(tex.ImageData.PixelFormat), tex.ImageData.Width);
            GL.TextureSubImage1D(id, 0, 0, tex.ImageData.Width, pxInfo.Format, pxInfo.PxType, tex.ImageData.PixelData);

            if (tex.DoGenerateMipMaps)
                GL.GenerateTextureMipmap(id);

            GL.TextureParameterI(id, TextureParameterName.TextureMinFilter, ref minFilter);
            GL.TextureParameterI(id, TextureParameterName.TextureMagFilter, ref magFilter);
            GL.TextureParameterI(id, TextureParameterName.TextureWrapS, ref glWrapMode);
            GL.TextureParameterI(id, TextureParameterName.TextureWrapT, ref glWrapMode);
            GL.TextureParameterI(id, TextureParameterName.TextureWrapR, ref glWrapMode);

            ITextureHandle texID = new TextureHandle { TexId = id };

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="tex">A given IWritableTexture object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        private ITextureHandle CreateTexture(WritableMultisampleTexture tex)
        {
            if (!_isMultisampleEnabled)
            {
                _isMultisampleEnabled = true;
                GL.Enable(EnableCap.Multisample);
            }

            GL.CreateTextures(TextureTarget.Texture2DMultisample, 1, out int id);
            GL.TextureStorage2DMultisample(id, tex.MultisampleFactor, GetSizedInteralFormat(tex.PixelFormat), tex.Width, tex.Height, true);

            var glMinMagFilter = GetMinMagFilter(tex.FilterMode);

            //Note: Multisample textures are not filtered and the calls below will generate a INVALID_ENUM error

            ITextureHandle texID = new TextureHandle { TexId = id };

            return texID;
        }

        /// <summary>
        /// Updates a specific rectangle of a texture.
        /// </summary>
        /// <param name="tex">The texture to which the ImageData is bound to.</param>
        /// <param name="img">The ImageData struct containing information about the image. </param>
        /// <param name="startX">The x-value of the upper left corner of th rectangle.</param>
        /// <param name="startY">The y-value of the upper left corner of th rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <remarks> /// <remarks>Look at the VideoTextureExample for further information.</remarks></remarks>
        public void UpdateTextureRegion(ITextureHandle tex, ITexture img, int startX, int startY, int width, int height)
        {
            var pxInfo = GetTexturePixelInfo(img.ImageData.PixelFormat);
            PixelFormat format = pxInfo.Format;

            // copy the bytes from img to GPU texture
            int bytesTotal = width * height * img.ImageData.PixelFormat.BytesPerPixel;
            var scanlines = img.ImageData.ScanLines(startX, startY, width, height);
            byte[] bytes = new byte[bytesTotal];
            int offset = 0;
            do
            {
                if (scanlines.Current != null)
                {
                    var lineBytes = scanlines.Current.GetScanLineBytes();
                    System.Buffer.BlockCopy(lineBytes, 0, bytes, offset, lineBytes.Length);
                    offset += lineBytes.Length;
                }

            } while (scanlines.MoveNext());

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, pxInfo.RowAlignment);
            GL.TextureSubImage2D(((TextureHandle)tex).TexId, 0, startX, startY, width, height, format, PixelType.UnsignedByte, bytes);
        }

        /// <summary>
        /// Sets the textures filter mode (<see cref="TextureFilterMode"/> at runtime.
        /// </summary>
        /// <param name="tex">The handle of the texture.</param>
        /// <param name="filterMode">The new filter mode.</param>
        public void SetTextureFilterMode(ITextureHandle tex, TextureFilterMode filterMode)
        {
            var toBindTexId = ((TextureHandle)tex).TexId;

            var minMag = GetMinMagFilter(filterMode);
            var glMinFilter = (int)minMag.Item1;
            var glMagFilter = (int)minMag.Item2;
            GL.TextureParameterI(toBindTexId, TextureParameterName.TextureMinFilter, ref glMinFilter);
            GL.TextureParameterI(toBindTexId, TextureParameterName.TextureMagFilter, ref glMagFilter);
        }

        /// <summary>
        /// Sets the textures filter mode (<see cref="Common.TextureWrapMode"/> at runtime.
        /// </summary>
        /// <param name="tex">The handle of the texture.</param>
        ///<param name="wrapMode">The new wrap mode.</param>
        public void SetTextureWrapMode(ITextureHandle tex, Common.TextureWrapMode wrapMode)
        {
            var toBindTexId = ((TextureHandle)tex).TexId;

            var glWrapMode = (int)GetWrapMode(wrapMode);
            GL.TextureParameterI(toBindTexId, TextureParameterName.TextureWrapS, ref glWrapMode);
            GL.TextureParameterI(toBindTexId, TextureParameterName.TextureWrapT, ref glWrapMode);
            GL.TextureParameterI(toBindTexId, TextureParameterName.TextureWrapR, ref glWrapMode);
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to a frame-buffer object.
        /// </summary>
        /// <param name="bh">The platform dependent abstraction of the gpu buffer handle.</param>
        public void DeleteFrameBuffer(IBufferHandle bh)
        {
            GL.DeleteFramebuffer(((FrameBufferHandle)bh).Handle);
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to a render-buffer object.
        /// </summary>
        /// <param name="bh">The platform dependent abstraction of the gpu buffer handle.</param>
        public void DeleteRenderBuffer(IBufferHandle bh)
        {
            GL.DeleteFramebuffer(((RenderBufferHandle)bh).Handle);
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to the given <see cref="ITextureHandle"/>.
        /// </summary>
        /// <param name="textureHandle">The <see cref="ITextureHandle"/> which gpu allocated memory will be freed.</param>
        public void RemoveTextureHandle(ITextureHandle textureHandle)
        {
            TextureHandle texHandle = (TextureHandle)textureHandle;

            if (texHandle.FrameBufferHandle != -1)
            {
                GL.DeleteFramebuffer(texHandle.FrameBufferHandle);
            }

            if (texHandle.DepthRenderBufferHandle != -1)
            {
                GL.DeleteRenderbuffer(texHandle.DepthRenderBufferHandle);
            }

            if (texHandle.TexId != -1)
            {
                GL.DeleteTexture(texHandle.TexId);
            }
        }
        #endregion

        #region Shader related Members

        /// <summary>
        /// Creates a shader object from compute shader source code.
        /// </summary>
        /// <param name="cs">A string containing the compute shader source.</param>
        /// <returns></returns>
        public IShaderHandle CreateShaderProgramCompute(string cs)
        {
            string info = string.Empty;
            int statusCode = -1;

            // Compile compute shader
            int computeObject = -1;
            if (!string.IsNullOrEmpty(cs))
            {
                computeObject = GL.CreateShader(ShaderType.ComputeShader);

                GL.ShaderSource(computeObject, cs);
                GL.CompileShader(computeObject);
                GL.GetShaderInfoLog(computeObject, out info);
                GL.GetShader(computeObject, ShaderParameter.CompileStatus, out statusCode);
            }

            if (statusCode != 1)
                throw new ApplicationException(info);

            int program = GL.CreateProgram();

            GL.AttachShader(program, computeObject);
            GL.LinkProgram(program); //Must be called AFTER BindAttribLocation
            GL.DetachShader(program, computeObject);
            GL.DeleteShader(computeObject);

            return new ShaderHandle { Handle = program };
        }

        /// <summary>
        /// Creates the shader program by using a valid GLSL vertex and fragment shader code. This code is compiled at runtime.
        /// Do not use this function in frequent updates.
        /// </summary>
        /// <param name="vs">The vertex shader code.</param>
        /// <param name="gs">The geometry shader code.</param>
        /// <param name="ps">The pixel(=fragment) shader code.</param>
        /// <returns>An instance of <see cref="IShaderHandle" />.</returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public IShaderHandle CreateShaderProgram(string vs, string ps, string gs = null)
        {
            int vertexObject = GL.CreateShader(ShaderType.VertexShader);
            int fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(vertexObject, vs);
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, out string info);
            GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out int statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            // Compile geometry shader
            int geometryObject = -1;
            if (!string.IsNullOrEmpty(gs))
            {
                geometryObject = GL.CreateShader(ShaderType.GeometryShader);

                GL.ShaderSource(geometryObject, gs);
                GL.CompileShader(geometryObject);
                GL.GetShaderInfoLog(geometryObject, out info);
                GL.GetShader(geometryObject, ShaderParameter.CompileStatus, out statusCode);
            }

            if (statusCode != 1)
                throw new ApplicationException(info);

            // Compile pixel shader
            GL.ShaderSource(fragmentObject, ps);
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(fragmentObject, out info);
            GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            int program = GL.CreateProgram();
            GL.AttachShader(program, fragmentObject);

            if (!string.IsNullOrEmpty(gs))
                GL.AttachShader(program, geometryObject);

            GL.AttachShader(program, vertexObject);

            // enable GLSL (ES) shaders to use fuVertex, fuColor and fuNormal attributes
            GL.BindAttribLocation(program, AttributeLocations.VertexAttribLocation, UniformNameDeclarations.Vertex);
            GL.BindAttribLocation(program, AttributeLocations.ColorAttribLocation, UniformNameDeclarations.VertexColor);
            GL.BindAttribLocation(program, AttributeLocations.Color1AttribLocation, UniformNameDeclarations.VertexColor1);
            GL.BindAttribLocation(program, AttributeLocations.Color2AttribLocation, UniformNameDeclarations.VertexColor2);
            GL.BindAttribLocation(program, AttributeLocations.UvAttribLocation, UniformNameDeclarations.TextureCoordinates);
            GL.BindAttribLocation(program, AttributeLocations.NormalAttribLocation, UniformNameDeclarations.Normal);
            GL.BindAttribLocation(program, AttributeLocations.TangentAttribLocation, UniformNameDeclarations.Tangent);
            GL.BindAttribLocation(program, AttributeLocations.BoneIndexAttribLocation, UniformNameDeclarations.BoneIndex);
            GL.BindAttribLocation(program, AttributeLocations.BoneWeightAttribLocation, UniformNameDeclarations.BoneWeight);
            GL.BindAttribLocation(program, AttributeLocations.BitangentAttribLocation, UniformNameDeclarations.Bitangent);
            GL.BindAttribLocation(program, AttributeLocations.InstancedColor, UniformNameDeclarations.InstanceColor);
            GL.BindAttribLocation(program, AttributeLocations.InstancedModelMat1, UniformNameDeclarations.InstanceModelMat);
            GL.BindAttribLocation(program, AttributeLocations.FlagsAttribLocation, UniformNameDeclarations.Flags);

            GL.LinkProgram(program); //Must be called AFTER BindAttribLocation

            GL.DetachShader(program, fragmentObject);
            GL.DetachShader(program, vertexObject);
            GL.DeleteShader(fragmentObject);
            GL.DeleteShader(vertexObject);

            return new ShaderHandle { Handle = program };
        }

        /// <inheritdoc />
        /// <summary>
        /// Removes shader from the GPU
        /// </summary>
        /// <param name="sp"></param>
        public void RemoveShader(IShaderHandle sp)
        {
            var program = ((ShaderHandle)sp).Handle;

            // wait for all threads to be finished
            GL.Finish();
            GL.Flush();

            GL.DeleteProgram(program);
        }


        /// <summary>
        /// Sets the shader program onto the GL Render context.
        /// </summary>
        /// <param name="program">The shader program.</param>
        public void SetShader(IShaderHandle program)
        {
            _textureCountPerShader = 0;
            _shaderParam2TexUnit.Clear();

            GL.UseProgram(((ShaderHandle)program).Handle);
        }

        /// <summary>
        /// Gets the shader parameter.
        /// The Shader parameter is used to bind values inside of shader programs that run on the graphics card.
        /// Do not use this function in frequent updates as it transfers information from graphics card to the cpu which takes time.
        /// </summary>
        /// <param name="shaderProgram">The shader program.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>The Shader parameter is returned if the name is found, otherwise null.</returns>
        public IUniformHandle GetShaderUniformParam(IShaderHandle shaderProgram, string paramName)
        {
            int h = GL.GetUniformLocation(((ShaderHandle)shaderProgram).Handle, paramName);
            return (h == -1) ? null : new UniformHandle { handle = h };
        }

        /// <summary>
        /// Gets the float parameter value inside a shader program by using a <see cref="IUniformHandle" /> as search reference.
        /// Do not use this function in frequent updates as it transfers information from graphics card to the cpu which takes time.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>A float number (default is 0).</returns>
        public float GetParamValue(IShaderHandle program, IUniformHandle param)
        {
            GL.GetUniform(((ShaderHandle)program).Handle, ((UniformHandle)param).handle, out float f);
            return f;
        }

        /// <summary>
        /// Returns a List of type <see cref="ActiveUniform"/> for all ShaderStorageBlocks
        /// </summary>
        /// <param name="shaderProgram">The shader program to query.</param>
        public IList<IActiveUniform> GetShaderStorageBufferList(IShaderHandle shaderProgram)
        {
            var paramList = new List<IActiveUniform>();
            var sProg = (ShaderHandle)shaderProgram;
            GL.GetProgramInterface(sProg.Handle, ProgramInterface.ShaderStorageBlock, ProgramInterfaceParameter.MaxNameLength, out int ssboMaxLen);
            GL.GetProgramInterface(sProg.Handle, ProgramInterface.ShaderStorageBlock, ProgramInterfaceParameter.ActiveResources, out int nParams);

            for (var i = 0; i < nParams; i++)
            {
                var param = new ActiveUniform
                {
                    HasValueChanged = true
                };
                GL.GetProgramResourceName(sProg.Handle, ProgramInterface.ShaderStorageBlock, i, ssboMaxLen, out _, out string name);
                param.Name = name;

                int h = GL.GetProgramResourceIndex(sProg.Handle, ProgramInterface.ShaderStorageBlock, name);
                param.Handle = (h == -1) ? null : new UniformHandle { handle = h };
                paramList.Add(param);
            }

            return paramList;
        }

        /// <summary>
        /// Gets the shader parameter list of a specific <see cref="IShaderHandle" />.
        /// </summary>
        /// <param name="shaderProgram">The shader program.</param>
        /// <returns>All Shader parameters of a shader program are returned.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IList<IActiveUniform> GetActiveUniformsList(IShaderHandle shaderProgram)
        {
            var sProg = (ShaderHandle)shaderProgram;
            var paramList = new List<IActiveUniform>();

            GL.GetProgram(sProg.Handle, GetProgramParameterName.ActiveUniforms, out int nParams);

            for (var i = 0; i < nParams; i++)
            {
                var paramInfo = new ActiveUniform
                {
                    Name = GL.GetActiveUniform(sProg.Handle, i, out var paramSize, out ActiveUniformType uType),
                    HasValueChanged = true
                };
                paramInfo.Handle = GetShaderUniformParam(sProg, paramInfo.Name);
                paramInfo.Size = paramSize;
                paramList.Add(paramInfo);
            }
            return paramList;
        }

        /// <summary>
        /// Specifies the rasterized width of both aliased and antialiased lines.
        /// </summary>
        /// <param name="width">The width in px.</param>
        public void SetLineWidth(float width)
        {
            GL.LineWidth(width);
        }

        /// <summary>
        /// Sets a float shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, float val)
        {
            GL.Uniform1(((UniformHandle)param).handle, val);
        }

        /// <summary>
        /// Sets a double shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, double val)
        {
            GL.Uniform1(((UniformHandle)param).handle, val);
        }

        /// <summary>
        /// Sets a <see cref="float2" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, float2 val)
        {
            GL.Uniform2(((UniformHandle)param).handle, val.x, val.y);
        }

        /// <summary>
        /// Sets a <see cref="float2" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IUniformHandle param, float2[] val)
        {
            fixed (float2* pFlt = &val[0])
                GL.Uniform2(((UniformHandle)param).handle, val.Length, (float*)pFlt);
        }

        /// <summary>
        /// Sets a <see cref="float3" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, float3 val)
        {
            GL.Uniform3(((UniformHandle)param).handle, val.x, val.y, val.z);
        }

        /// <summary>
        ///     Sets a <see cref="float3" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IUniformHandle param, float3[] val)
        {
            fixed (float3* pFlt = &val[0])
                GL.Uniform3(((UniformHandle)param).handle, val.Length, (float*)pFlt);
        }

        /// <summary>
        /// Sets a <see cref="float4" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, float4 val)
        {
            GL.Uniform4(((UniformHandle)param).handle, val.x, val.y, val.z, val.w);
        }

        /// <summary>
        /// Sets a <see cref="float4x4" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IUniformHandle param, float4x4 val)
        {
            fixed (void* pt = new byte[Marshal.SizeOf(val)])
            {
                var intPtr = new IntPtr(pt);
                Marshal.StructureToPtr(val, intPtr, true);
                GL.UniformMatrix4(((UniformHandle)param).handle, 1, true, (float*)intPtr);
            }
        }

        /// <summary>
        ///     Sets a <see cref="float4" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IUniformHandle param, float4[] val)
        {
            fixed (float4* pFlt = &val[0])
                GL.Uniform4(((UniformHandle)param).handle, val.Length, (float*)pFlt);
        }

        /// <summary>
        /// Sets a <see cref="int2" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, int2 val)
        {
            GL.Uniform2(((UniformHandle)param).handle, val.x, val.y);
        }

        /// <summary>
        /// Sets a <see cref="float4x4" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IUniformHandle param, float4x4[] val)
        {
            fixed (float4x4* pFlt = &val[0])
            {
                GL.UniformMatrix4(((UniformHandle)param).handle, val.Length, true, (float*)pFlt);
            }
        }

        /// <summary>
        /// Sets a int shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, int val)
        {
            GL.Uniform1(((UniformHandle)param).handle, val);
        }

        private void BindImage(TextureType texTarget, ITextureHandle texId, int texUint, TextureAccess access, SizedInternalFormat format)
        {
            switch (texTarget)
            {
                case TextureType.Image2D:
                    GL.BindImageTexture(texUint, ((TextureHandle)texId).TexId, 0, false, 0, access, format);
                    break;
                default:
                    throw new ArgumentException($"Unknown texture target: {texTarget}.");
            }
        }

        /// <summary>
        /// Sets a texture active and binds it.
        /// </summary>
        /// <param name="param">The shader parameter, associated with this texture.</param>
        /// <param name="texId">The texture handle.</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        public void SetActiveAndBindTexture(IUniformHandle param, ITextureHandle texId, TextureType texTarget)
        {
            int iParam = ((UniformHandle)param).handle;
            if (!_shaderParam2TexUnit.TryGetValue(iParam, out _))
            {
                _textureCountPerShader++;
                _shaderParam2TexUnit[iParam] = _textureCountPerShader;
                GL.BindTextureUnit(_textureCountPerShader, ((TextureHandle)texId).TexId);
            }
        }

        private void SetActiveAndBindImage(IUniformHandle param, ITextureHandle texId, TextureType texTarget, ImagePixelFormat format, TextureAccess access, out int texUnit)
        {
            int iParam = ((UniformHandle)param).handle;
            if (!_shaderParam2TexUnit.TryGetValue(iParam, out texUnit))
            {
                _textureCountPerShader++;
                texUnit = _textureCountPerShader;
                _shaderParam2TexUnit[iParam] = texUnit;

                var sizedIntFormat = GetSizedInteralFormat(format);

                GL.ActiveTexture(TextureUnit.Texture0 + texUnit);
                BindImage(texTarget, texId, texUnit, access, sizedIntFormat);
            }
        }

        /// <summary>
        /// Sets a texture active and binds it.
        /// </summary>
        /// <param name="param">The shader parameter, associated with this texture.</param>
        /// <param name="texId">The texture handle.</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        /// <param name="texUnit">The texture unit.</param>
        public void SetActiveAndBindTexture(IUniformHandle param, ITextureHandle texId, TextureType texTarget, out int texUnit)
        {
            int iParam = ((UniformHandle)param).handle;
            if (!_shaderParam2TexUnit.TryGetValue(iParam, out texUnit))
            {
                _textureCountPerShader++;
                _shaderParam2TexUnit[iParam] = _textureCountPerShader;
                texUnit = _textureCountPerShader;
                GL.BindTextureUnit(_textureCountPerShader, ((TextureHandle)texId).TexId);
            }
        }

        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texIds">An array of ITextureHandles returned from CreateTexture method or the ShaderEffectManager.</param>
        /// /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        public void SetActiveAndBindTextureArray(IUniformHandle param, ITextureHandle[] texIds, TextureType texTarget)
        {
            int iParam = ((UniformHandle)param).handle;

            if (!_shaderParam2TexUnit.TryGetValue(iParam, out _))
            {
                _textureCountPerShader++;
                _textureCountPerShader += texIds.Length;
                _shaderParam2TexUnit[iParam] = _textureCountPerShader;

                for (int i = 0; i < texIds.Length; i++)
                {
                    GL.BindTextureUnit(_textureCountPerShader + i, ((TextureHandle)texIds[i]).TexId);
                }
            }
        }

        /// <summary>
        /// Sets a texture active and binds it.
        /// </summary>
        /// <param name="param">The shader parameter, associated with this texture.</param>
        /// <param name="texIds">An array of ITextureHandles returned from CreateTexture method or the ShaderEffectManager.</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        /// <param name="texUnitArray">The texture units.</param>
        public void SetActiveAndBindTextureArray(IUniformHandle param, ITextureHandle[] texIds, TextureType texTarget, out int[] texUnitArray)
        {
            int iParam = ((UniformHandle)param).handle;
            texUnitArray = new int[texIds.Length];

            if (!_shaderParam2TexUnit.TryGetValue(iParam, out _))
            {
                _textureCountPerShader++;
                _textureCountPerShader += texIds.Length;
                _shaderParam2TexUnit[iParam] = _textureCountPerShader;

                for (int i = 0; i < texIds.Length; i++)
                {
                    texUnitArray[i] = _textureCountPerShader + i;
                    GL.BindTextureUnit(_textureCountPerShader + i, ((TextureHandle)texIds[i]).TexId);
                }
            }
        }

        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texId">An ITextureHandle probably returned from CreateTexture method</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        /// <param name="format">The internal sized format of the texture.</param>
        public void SetShaderParamImage(IUniformHandle param, ITextureHandle texId, TextureType texTarget, ImagePixelFormat format)
        {
            SetActiveAndBindImage(param, texId, texTarget, format, TextureAccess.ReadWrite, out int texUnit);
            GL.Uniform1(((UniformHandle)param).handle, texUnit);
        }

        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texId">An ITextureHandle probably returned from CreateTexture method</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        public void SetShaderParamTexture(IUniformHandle param, ITextureHandle texId, TextureType texTarget)
        {
            SetActiveAndBindTexture(param, texId, texTarget, out int texUnit);
            GL.Uniform1(((UniformHandle)param).handle, texUnit);
        }

        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texIds">An array of ITextureHandles probably returned from CreateTexture method</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        public unsafe void SetShaderParamTextureArray(IUniformHandle param, ITextureHandle[] texIds, TextureType texTarget)
        {
            SetActiveAndBindTextureArray(param, texIds, texTarget, out int[] texUnitArray);

            fixed (int* pFlt = &texUnitArray[0])
                GL.Uniform1(((UniformHandle)param).handle, texUnitArray.Length, pFlt);
        }

        #endregion

        #region Clear

        /// <summary>
        /// Clears the specified flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
        public void Clear(ClearFlags flags)
        {
            GL.Clear((ClearBufferMask)flags);
        }

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
                //GL.GetFloat(GetPName.ColorClearValue, out OpenTK.Mathematics.Vector4 ret);
                return _clearColor;
            }
            set
            {
                _clearColor = value;
                GL.ClearColor(value.x, value.y, value.z, value.w);
            }
        }
        private float4 _clearColor;

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
                GL.GetFloat(GetPName.DepthClearValue, out float ret);
                return ret;
            }
            set => GL.ClearDepth(value);
        }

        #endregion

        #region Rendering related Members

        /// <summary>
        /// Creates a <see cref="IRenderTarget"/> with the purpose of being used as CPU GBuffer representation.
        /// </summary>
        /// <param name="res">The texture resolution.</param>
        public IRenderTarget CreateGBufferTarget(TexRes res)
        {
            var gBufferRenderTarget = new RenderTarget(res);
            gBufferRenderTarget.SetPositionTex();
            gBufferRenderTarget.SetAlbedoTex();
            gBufferRenderTarget.SetNormalTex();
            gBufferRenderTarget.SetDepthTex();
            gBufferRenderTarget.SetSpecularTex();
            gBufferRenderTarget.SetEmissiveTex();
            gBufferRenderTarget.SetSubsurfaceTex();

            return gBufferRenderTarget;
        }

        /// <summary>
        /// The clipping behavior against the Z position of a vertex can be turned off by activating depth clamping.
        /// This is done with glEnable(GL_DEPTH_CLAMP). This will cause the clip-space Z to remain unclipped by the front and rear viewing volume.
        /// See: https://www.khronos.org/opengl/wiki/Vertex_Post-Processing#Depth_clamping
        /// </summary>
        public void EnableDepthClamp()
        {
            GL.Enable(EnableCap.DepthClamp);
        }

        /// <summary>
        /// Retrieve pixels from bound framebuffer
        /// </summary>
        /// <param name="x">x pixel position</param>
        /// <param name="y">y pixel position</param>
        /// <param name="pixelFormat">format to retrieve, this has to match the current bound FBO!</param>
        /// <param name="width">how many pixel in x direction</param>
        /// <param name="height">how many pixel in y direction</param>
        /// <returns><see cref="ReadOnlySpan{T}"/> with pixel content</returns>
        /// <remarks>Does usually not throw on error (e. g. wrong pixel format, out of bounds, etc), uses GL.GetError() to retrieve
        /// potential error</remarks>
        public ReadOnlySpan<byte> ReadPixels(int x, int y, ImagePixelFormat pixelFormat, int width, int height)
        {
            var format = GetTexturePixelInfo(pixelFormat);
            var data = new byte[width * height * pixelFormat.BytesPerPixel];

            GL.ReadPixels(x, y, 1, 1, format.Format, format.PxType, data);

            var err = GL.GetError();
            if (err != ErrorCode.NoError)
            {
                throw new Exception($"ReadPixel failed with error code {err}!");
            }

            return data;
        }

        /// <summary>
        /// Disables depths clamping. <seealso cref="EnableDepthClamp"/>
        /// </summary>
        public void DisableDepthClamp()
        {
            GL.Disable(EnableCap.DepthClamp);
        }

        /// <summary>
        /// Binds the VertexArrayObject onto the GL Render context and assigns its index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        public void SetVertexArrayObject(IMeshImp mr)
        {
            if (((MeshImp)mr).VertexArrayObject == 0)
            {
                GL.CreateVertexArrays(1, out int vao);
                ((MeshImp)mr).VertexArrayObject = vao;
            }
        }

        /// <summary>
        /// Creates or updates the instance transform buffer. Positions, scales and rotations become the instance model matrices.
        /// </summary>
        /// <param name="instanceImp">The <see cref="InstanceDataImp"/>.</param>
        /// <param name="instancePositions">The positions of the instances.</param>
        /// <param name="instanceRotations">The rotations of the instances.</param>
        /// <param name="instanceScales">The scales of the instances.</param>
        public void SetInstanceTransform(IInstanceDataImp instanceImp, float3[] instancePositions, float3[] instanceRotations, float3[] instanceScales)
        {
            var vao = ((InstanceDataImp)instanceImp).VertexArrayObject;
            if (vao == 0)
            {
                throw new ApplicationException("Create the VAO first!");
            }

            var sizeOfFloat4 = sizeof(float) * 4;
            var sizeOfMat = sizeOfFloat4 * 4;
            var amount = instancePositions.Length;
            int matBytes = amount * sizeOfMat;

            var posBufferData = new float4[amount * 4];

            var modelMats = new float4x4[amount];

            for (int i = 0; i < amount; i++)
            {
                var mat = float4x4.Identity;
                if (instanceScales != null)
                    mat = float4x4.CreateScale(instanceScales[i]);
                if (instanceRotations != null)
                    mat *= float4x4.CreateRotationZXY(instanceRotations[i]);
                mat *= float4x4.CreateTranslation(instancePositions[i]);
                modelMats[i] = mat;
            }

            for (var i = 0; i < modelMats.Length; i++)
            {
                posBufferData[i * 4] = modelMats[i].Column1;
                posBufferData[i * 4 + 1] = modelMats[i].Column2;
                posBufferData[i * 4 + 2] = modelMats[i].Column3;
                posBufferData[i * 4 + 3] = modelMats[i].Column4;
            }

            int instanceTransformBo = ((InstanceDataImp)instanceImp).InstanceTransformBufferObject;
            if (instanceTransformBo == 0)
            {
                GL.CreateBuffers(1, out instanceTransformBo);
                ((InstanceDataImp)instanceImp).InstanceTransformBufferObject = instanceTransformBo;
                GL.NamedBufferStorage(instanceTransformBo, matBytes, posBufferData, BufferStorageFlags.DynamicStorageBit);
            }
            else
            {
                GL.NamedBufferSubData(instanceTransformBo, IntPtr.Zero, matBytes, posBufferData);
                instanceTransformBo = ((InstanceDataImp)instanceImp).InstanceTransformBufferObject;
            }

            GL.VertexArrayVertexBuffer(vao, AttributeLocations.InstancedModelMatBindingIndex, instanceTransformBo, IntPtr.Zero, sizeOfMat);
            GL.GetNamedBufferParameter(instanceTransformBo, BufferParameterName.BufferSize, out int instancedPosBytes);
            if (instancedPosBytes != matBytes)
                throw new ApplicationException(string.Format("Problem uploading normal buffer to VBO. Tried to upload {0} bytes, uploaded {1}.", instancedPosBytes, matBytes));

            // set attribute pointers for matrix (4 times vec4)
            GL.VertexArrayAttribFormat(vao, AttributeLocations.InstancedModelMat1, 4, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(vao, AttributeLocations.InstancedModelMat1, AttributeLocations.InstancedModelMatBindingIndex);
            GL.VertexArrayAttribFormat(vao, AttributeLocations.InstancedModelMat2, 4, VertexAttribType.Float, false, 1 * sizeOfFloat4);
            GL.VertexArrayAttribBinding(vao, AttributeLocations.InstancedModelMat2, AttributeLocations.InstancedModelMatBindingIndex);
            GL.VertexArrayAttribFormat(vao, AttributeLocations.InstancedModelMat3, 4, VertexAttribType.Float, false, 2 * sizeOfFloat4);
            GL.VertexArrayAttribBinding(vao, AttributeLocations.InstancedModelMat3, AttributeLocations.InstancedModelMatBindingIndex);
            GL.VertexArrayAttribFormat(vao, AttributeLocations.InstancedModelMat4, 4, VertexAttribType.Float, false, 3 * sizeOfFloat4);
            GL.VertexArrayAttribBinding(vao, AttributeLocations.InstancedModelMat4, AttributeLocations.InstancedModelMatBindingIndex);

            GL.VertexArrayBindingDivisor(vao, AttributeLocations.InstancedModelMat1, 1);
            GL.VertexArrayBindingDivisor(vao, AttributeLocations.InstancedModelMat2, 1);
            GL.VertexArrayBindingDivisor(vao, AttributeLocations.InstancedModelMat3, 1);
            GL.VertexArrayBindingDivisor(vao, AttributeLocations.InstancedModelMat4, 1);
        }

        /// <summary>
        /// Creates or updates the instance color buffer..
        /// </summary>
        /// <param name="instanceImp">The <see cref="InstanceDataImp"/>.</param>
        /// <param name="instanceColors">The colors of the instances.</param>
        public void SetInstanceColor(IInstanceDataImp instanceImp, float4[] instanceColors)
        {
            if (instanceColors == null)
                return;

            var vao = ((InstanceDataImp)instanceImp).VertexArrayObject;
            if (vao == 0)
            {
                throw new ApplicationException("Create the VAO first!");
            }

            //TODO: can we use AttributeLocations.Color?
            int sizeOfCol = sizeof(float) * 4;
            int iColorBytes = instanceColors.Length * sizeOfCol;
            int instanceColorBo = ((InstanceDataImp)instanceImp).InstanceColorBufferObject;
            if (instanceColorBo == 0)
            {
                GL.CreateBuffers(1, out instanceColorBo);
                ((InstanceDataImp)instanceImp).InstanceColorBufferObject = instanceColorBo;
                GL.NamedBufferStorage(instanceColorBo, iColorBytes, instanceColors, BufferStorageFlags.DynamicStorageBit);
            }
            else
            {
                instanceColorBo = ((InstanceDataImp)instanceImp).InstanceColorBufferObject;
                GL.NamedBufferSubData(instanceColorBo, IntPtr.Zero, iColorBytes, instanceColors);
            }

            GL.VertexArrayVertexBuffer(vao, AttributeLocations.InstancedColorBindingIndex, instanceColorBo, IntPtr.Zero, sizeOfCol);
            GL.GetNamedBufferParameter(instanceColorBo, BufferParameterName.BufferSize, out int instancedColorBytes);
            if (instancedColorBytes != iColorBytes)
                throw new ApplicationException(string.Format("Problem uploading normal buffer to VBO. Tried to upload {0} bytes, uploaded {1}.", instancedColorBytes, iColorBytes));

            // set attribute pointers for matrix (4 times vec4)
            GL.VertexArrayAttribFormat(vao, AttributeLocations.InstancedColor, 4, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(vao, AttributeLocations.InstancedColor, AttributeLocations.InstancedColorBindingIndex);
            GL.VertexArrayBindingDivisor(vao, AttributeLocations.InstancedColor, 1);
        }

        /// <summary>
        /// Binds the vertices onto the GL Render context and assigns an VertexBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="vertices">The vertices.</param>
        /// <exception cref="ArgumentException">Vertices must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetVertices(IMeshImp mr, ReadOnlySpan<float3> vertices)
        {
            if (vertices == null || vertices.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            int sizeOfVert = 3 * sizeof(float); ;
            int vertsBytes = vertices.Length * sizeOfVert;
            int vbo;
            if (((MeshImp)mr).VertexBufferObject == 0)
            {
                GL.CreateBuffers(1, out vbo);
                ((MeshImp)mr).VertexBufferObject = vbo;

                var vao = ((MeshImp)mr).VertexArrayObject;
                if (vao == 0)
                {
                    throw new ApplicationException("Create the VAO first!");
                }
                GL.NamedBufferStorage(vbo, vertsBytes, ref MemoryMarshal.GetReference(vertices), BufferStorageFlags.DynamicStorageBit);
                GL.VertexArrayVertexBuffer(((MeshImp)mr).VertexArrayObject, AttributeLocations.VertexAttribBindingIndex, vbo, IntPtr.Zero, sizeOfVert);

                GL.VertexArrayAttribFormat(vao, AttributeLocations.VertexAttribLocation, 3, VertexAttribType.Float, false, 0);
                GL.VertexArrayAttribBinding(vao, AttributeLocations.VertexAttribLocation, AttributeLocations.VertexAttribBindingIndex);
            }
            else
            {
                vbo = ((MeshImp)mr).VertexBufferObject;
                GL.GetNamedBufferParameter(vbo, BufferParameterName.BufferSize, out int size);
                if (size < vertsBytes)
                {
                    GL.DeleteBuffer(vbo);
                    GL.CreateBuffers(1, out vbo);

                    var vao = ((MeshImp)mr).VertexArrayObject;
                    GL.NamedBufferStorage(vbo, vertsBytes, ref MemoryMarshal.GetReference(vertices), BufferStorageFlags.DynamicStorageBit);
                    GL.VertexArrayVertexBuffer(((MeshImp)mr).VertexArrayObject, AttributeLocations.VertexAttribBindingIndex, vbo, IntPtr.Zero, sizeOfVert);

                }
                else
                    GL.NamedBufferSubData(vbo, IntPtr.Zero, vertsBytes, ref MemoryMarshal.GetReference(vertices));
            }
#if DEBUG
            GL.GetNamedBufferParameter(vbo, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes < vertsBytes)
                throw new ApplicationException(string.Format("Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.", vertsBytes, vboBytes));
#endif
        }

        /// <summary>
        /// Binds the tangents onto the GL Render context and assigns an TangentBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="tangents">The tangents.</param>
        /// <exception cref="ArgumentException">Tangents must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetTangents(IMeshImp mr, ReadOnlySpan<float4> tangents)
        {
            if (tangents == null || tangents.Length == 0)
            {
                throw new ArgumentException("Tangents must not be null or empty");
            }

            int sizeOfTangent = 4 * sizeof(float);
            int tangentBytes = tangents.Length * sizeOfTangent;
            int tBo;
            if (((MeshImp)mr).TangentBufferObject == 0)
            {
                GL.CreateBuffers(1, out tBo);
                ((MeshImp)mr).TangentBufferObject = tBo;
                var vao = ((MeshImp)mr).VertexArrayObject;
                if (vao == 0)
                {
                    throw new ApplicationException("Create the VAO first!");
                }
                GL.NamedBufferStorage(tBo, tangentBytes, ref MemoryMarshal.GetReference(tangents), BufferStorageFlags.DynamicStorageBit);
                GL.VertexArrayVertexBuffer(vao, AttributeLocations.TangentAttribBindingIndex, tBo, IntPtr.Zero, sizeOfTangent);

                GL.VertexArrayAttribFormat(vao, AttributeLocations.TangentAttribLocation, 4, VertexAttribType.Float, false, 0);
                GL.VertexArrayAttribBinding(vao, AttributeLocations.TangentAttribLocation, AttributeLocations.TangentAttribBindingIndex);
            }
            else
            {
                tBo = ((MeshImp)mr).TangentBufferObject;

                GL.GetNamedBufferParameter(tBo, BufferParameterName.BufferSize, out int size);
                if (size < tangentBytes)
                {
                    GL.DeleteBuffer(tBo);
                    GL.CreateBuffers(1, out tBo);

                    var vao = ((MeshImp)mr).VertexArrayObject;
                    GL.NamedBufferStorage(tBo, tangentBytes, ref MemoryMarshal.GetReference(tangents), BufferStorageFlags.DynamicStorageBit);
                    GL.VertexArrayVertexBuffer(((MeshImp)mr).VertexArrayObject, AttributeLocations.TangentAttribLocation, tBo, IntPtr.Zero, sizeOfTangent);

                }
                else
                    GL.NamedBufferSubData(tBo, IntPtr.Zero, tangentBytes, ref MemoryMarshal.GetReference(tangents));
            }

#if DEBUG
            GL.GetNamedBufferParameter(tBo, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes < tangentBytes)
                throw new ApplicationException(string.Format("Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.", tangentBytes, vboBytes));
#endif
        }

        /// <summary>
        /// Binds the bitangents onto the GL Render context and assigns an BiTangentBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="bitangents">The BiTangents.</param>
        /// <exception cref="ArgumentException">BiTangents must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetBiTangents(IMeshImp mr, ReadOnlySpan<float3> bitangents)
        {
            if (bitangents == null || bitangents.Length == 0)
            {
                throw new ArgumentException("BiTangents must not be null or empty");
            }

            int sizeOfBiTangent = 3 * sizeof(float);
            int bitangentBytes = bitangents.Length * sizeOfBiTangent;
            int btBo;
            if (((MeshImp)mr).BitangentBufferObject == 0)
            {
                GL.CreateBuffers(1, out btBo);
                ((MeshImp)mr).BitangentBufferObject = btBo;

                var vao = ((MeshImp)mr).VertexArrayObject;
                if (vao == 0)
                {
                    throw new ApplicationException("Create the VAO first!");
                }
                GL.NamedBufferStorage(btBo, bitangentBytes, ref MemoryMarshal.GetReference(bitangents), BufferStorageFlags.DynamicStorageBit);
                GL.VertexArrayVertexBuffer(vao, AttributeLocations.BitangentAttribBindingIndex, btBo, IntPtr.Zero, sizeOfBiTangent);

                GL.VertexArrayAttribFormat(vao, AttributeLocations.BitangentAttribLocation, 3, VertexAttribType.Float, false, 0);
                GL.VertexArrayAttribBinding(vao, AttributeLocations.BitangentAttribLocation, AttributeLocations.BitangentAttribBindingIndex);
            }
            else
            {
                btBo = ((MeshImp)mr).BitangentBufferObject;

                GL.GetNamedBufferParameter(btBo, BufferParameterName.BufferSize, out int size);
                if (size < bitangentBytes)
                {
                    GL.DeleteBuffer(btBo);
                    GL.CreateBuffers(1, out btBo);

                    var vao = ((MeshImp)mr).VertexArrayObject;
                    GL.NamedBufferStorage(btBo, bitangentBytes, ref MemoryMarshal.GetReference(bitangents), BufferStorageFlags.DynamicStorageBit);
                    GL.VertexArrayVertexBuffer(((MeshImp)mr).VertexArrayObject, AttributeLocations.BitangentAttribLocation, btBo, IntPtr.Zero, sizeOfBiTangent);

                }
                else
                    GL.NamedBufferSubData(btBo, IntPtr.Zero, bitangentBytes, ref MemoryMarshal.GetReference(bitangents));
            }

#if DEBUG
            GL.GetNamedBufferParameter(btBo, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes < bitangentBytes)
                throw new ApplicationException(string.Format("Problem uploading vertex buffer to VBO (bitangents). Tried to upload {0} bytes, uploaded {1}.", bitangentBytes, vboBytes));
#endif
        }

        /// <summary>
        /// Binds the normals onto the GL Render context and assigns an NormalBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="normals">The normals.</param>
        /// <exception cref="ArgumentException">Normals must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetNormals(IMeshImp mr, ReadOnlySpan<float3> normals)
        {
            if (normals == null || normals.Length == 0)
            {
                throw new ArgumentException("Normals must not be null or empty");
            }

            int sizeOfNorm = 3 * sizeof(float);
            int normsBytes = normals.Length * sizeOfNorm;
            int nBo;
            if (((MeshImp)mr).NormalBufferObject == 0)
            {
                GL.CreateBuffers(1, out nBo);
                ((MeshImp)mr).NormalBufferObject = nBo;

                var vao = ((MeshImp)mr).VertexArrayObject;
                if (vao == 0)
                {
                    throw new ApplicationException("Create the VAO first!");
                }
                GL.NamedBufferStorage(nBo, normsBytes, ref MemoryMarshal.GetReference(normals), BufferStorageFlags.DynamicStorageBit);
                GL.VertexArrayVertexBuffer(vao, AttributeLocations.NormalAttribBindingIndex, nBo, IntPtr.Zero, sizeOfNorm);

                GL.VertexArrayAttribFormat(vao, AttributeLocations.NormalAttribLocation, 3, VertexAttribType.Float, false, 0);
                GL.VertexArrayAttribBinding(vao, AttributeLocations.NormalAttribLocation, AttributeLocations.NormalAttribBindingIndex);
            }
            else
            {
                nBo = ((MeshImp)mr).NormalBufferObject;

                GL.GetNamedBufferParameter(nBo, BufferParameterName.BufferSize, out int size);
                if (size < normsBytes)
                {
                    GL.DeleteBuffer(nBo);
                    GL.CreateBuffers(1, out nBo);

                    var vao = ((MeshImp)mr).VertexArrayObject;
                    GL.NamedBufferStorage(nBo, normsBytes, ref MemoryMarshal.GetReference(normals), BufferStorageFlags.DynamicStorageBit);
                    GL.VertexArrayVertexBuffer(((MeshImp)mr).VertexArrayObject, AttributeLocations.NormalAttribLocation, nBo, IntPtr.Zero, sizeOfNorm);

                }
                else
                    GL.NamedBufferSubData(nBo, IntPtr.Zero, normsBytes, ref MemoryMarshal.GetReference(normals));
            }

#if DEBUG
            GL.GetNamedBufferParameter(nBo, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes < normsBytes)
                throw new ApplicationException(string.Format("Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.", normsBytes, vboBytes));
#endif
        }

        /// <summary>
        /// Binds the bone indices onto the GL Render context and assigns an BoneIndexBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneIndices">The bone indices.</param>
        /// <exception cref="ArgumentException">BoneIndices must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetBoneIndices(IMeshImp mr, ReadOnlySpan<float4> boneIndices)
        {
            if (boneIndices == null || boneIndices.Length == 0)
            {
                throw new ArgumentException("BoneIndices must not be null or empty");
            }

            int sizeOfboneIndex = 4 * sizeof(float);
            int indicesBytes = boneIndices.Length * sizeOfboneIndex;
            int biBo;
            if (((MeshImp)mr).BoneIndexBufferObject == 0)
            {
                GL.CreateBuffers(1, out biBo);
                ((MeshImp)mr).BoneIndexBufferObject = biBo;

                var vao = ((MeshImp)mr).VertexArrayObject;
                if (vao == 0)
                {
                    throw new ApplicationException("Create the VAO first!");
                }
                GL.NamedBufferStorage(biBo, indicesBytes, ref MemoryMarshal.GetReference(boneIndices), BufferStorageFlags.DynamicStorageBit);
                GL.VertexArrayVertexBuffer(vao, AttributeLocations.BoneIndexAttribAttribBindingIndex, biBo, IntPtr.Zero, sizeOfboneIndex);

                GL.VertexArrayAttribFormat(vao, AttributeLocations.BoneIndexAttribLocation, 4, VertexAttribType.Float, false, 0);
                GL.VertexArrayAttribBinding(vao, AttributeLocations.BoneIndexAttribLocation, AttributeLocations.BoneIndexAttribAttribBindingIndex);
            }
            else
            {
                biBo = ((MeshImp)mr).BoneIndexBufferObject;
                GL.GetNamedBufferParameter(biBo, BufferParameterName.BufferSize, out int size);
                if (size < indicesBytes)
                {
                    GL.DeleteBuffer(biBo);
                    GL.CreateBuffers(1, out biBo);

                    var vao = ((MeshImp)mr).VertexArrayObject;
                    GL.NamedBufferStorage(biBo, indicesBytes, ref MemoryMarshal.GetReference(boneIndices), BufferStorageFlags.DynamicStorageBit);
                    GL.VertexArrayVertexBuffer(((MeshImp)mr).VertexArrayObject, AttributeLocations.BoneIndexAttribLocation, biBo, IntPtr.Zero, sizeOfboneIndex);

                }
                else
                    GL.NamedBufferSubData(biBo, IntPtr.Zero, indicesBytes, ref MemoryMarshal.GetReference(boneIndices));
            }

#if DEBUG
            GL.GetNamedBufferParameter(biBo, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes < indicesBytes)
                throw new ApplicationException(string.Format("Problem uploading bone indices buffer to VBO (bone indices). Tried to upload {0} bytes, uploaded {1}.", indicesBytes, vboBytes));
#endif
        }

        /// <summary>
        /// Binds the bone weights onto the GL Render context and assigns an BoneWeightBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneWeights">The bone weights.</param>
        /// <exception cref="ArgumentException">BoneWeights must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetBoneWeights(IMeshImp mr, ReadOnlySpan<float4> boneWeights)
        {
            if (boneWeights == null || boneWeights.Length == 0)
            {
                throw new ArgumentException("BoneWeights must not be null or empty");
            }

            int sizeOfBoneWeight = 4 * sizeof(float);
            int weightsBytes = boneWeights.Length * sizeOfBoneWeight;
            int wBo;
            if (((MeshImp)mr).BoneIndexBufferObject == 0)
            {
                GL.CreateBuffers(1, out wBo);
                ((MeshImp)mr).BoneIndexBufferObject = wBo;

                var vao = ((MeshImp)mr).VertexArrayObject;
                if (vao == 0)
                {
                    throw new ApplicationException("Create the VAO first!");
                }
                GL.NamedBufferStorage(wBo, weightsBytes, ref MemoryMarshal.GetReference(boneWeights), BufferStorageFlags.DynamicStorageBit);
                GL.VertexArrayVertexBuffer(vao, AttributeLocations.BoneIndexAttribAttribBindingIndex, wBo, IntPtr.Zero, sizeOfBoneWeight);

                GL.VertexArrayAttribFormat(vao, AttributeLocations.BoneIndexAttribLocation, 4, VertexAttribType.Float, false, 0);
                GL.VertexArrayAttribBinding(vao, AttributeLocations.BoneIndexAttribLocation, AttributeLocations.BoneIndexAttribAttribBindingIndex);
            }
            else
            {
                wBo = ((MeshImp)mr).BoneIndexBufferObject;
                GL.GetNamedBufferParameter(wBo, BufferParameterName.BufferSize, out int size);
                if (size < weightsBytes)
                {
                    GL.DeleteBuffer(wBo);
                    GL.CreateBuffers(1, out wBo);

                    var vao = ((MeshImp)mr).VertexArrayObject;
                    GL.NamedBufferStorage(wBo, weightsBytes, ref MemoryMarshal.GetReference(boneWeights), BufferStorageFlags.DynamicStorageBit);
                    GL.VertexArrayVertexBuffer(((MeshImp)mr).VertexArrayObject, AttributeLocations.BoneIndexAttribLocation, wBo, IntPtr.Zero, sizeOfBoneWeight);

                }
                else
                    GL.NamedBufferSubData(wBo, IntPtr.Zero, weightsBytes, ref MemoryMarshal.GetReference(boneWeights));
            }

#if DEBUG
            GL.GetNamedBufferParameter(wBo, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes < weightsBytes)
                throw new ApplicationException(string.Format("Problem uploading bone weights buffer to VBO (bone weights). Tried to upload {0} bytes, uploaded {1}.", weightsBytes, vboBytes));
#endif
        }

        /// <summary>
        /// Binds the UV coordinates onto the GL Render context and assigns an UVBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="uvs">The UV's.</param>
        /// <exception cref="ArgumentException">UVs must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetUVs(IMeshImp mr, ReadOnlySpan<float2> uvs)
        {
            if (uvs == null || uvs.Length == 0)
            {
                throw new ArgumentException("UVs must not be null or empty");
            }

            int sizeOfUv = 2 * sizeof(float);
            int uvsBytes = uvs.Length * sizeOfUv;
            int uvBo;
            if (((MeshImp)mr).UVBufferObject == 0)
            {
                GL.CreateBuffers(1, out uvBo);
                ((MeshImp)mr).UVBufferObject = uvBo;

                var vao = ((MeshImp)mr).VertexArrayObject;
                if (vao == 0)
                {
                    throw new ApplicationException("Create the VAO first!");
                }
                GL.NamedBufferStorage(uvBo, uvsBytes, ref MemoryMarshal.GetReference(uvs), BufferStorageFlags.DynamicStorageBit);
                GL.VertexArrayVertexBuffer(vao, AttributeLocations.UvAttribBindingIndex, uvBo, IntPtr.Zero, sizeOfUv);

                GL.VertexArrayAttribFormat(vao, AttributeLocations.UvAttribLocation, 2, VertexAttribType.Float, false, 0);
                GL.VertexArrayAttribBinding(vao, AttributeLocations.UvAttribLocation, AttributeLocations.UvAttribBindingIndex);
            }
            else
            {
                uvBo = ((MeshImp)mr).UVBufferObject;
                GL.GetNamedBufferParameter(uvBo, BufferParameterName.BufferSize, out int size);
                if (size < uvsBytes)
                {
                    GL.DeleteBuffer(uvBo);
                    GL.CreateBuffers(1, out uvBo);

                    var vao = ((MeshImp)mr).VertexArrayObject;
                    GL.NamedBufferStorage(uvBo, uvsBytes, ref MemoryMarshal.GetReference(uvs), BufferStorageFlags.DynamicStorageBit);
                    GL.VertexArrayVertexBuffer(((MeshImp)mr).VertexArrayObject, AttributeLocations.UvAttribLocation, uvBo, IntPtr.Zero, sizeOfUv);

                }
                else
                    GL.NamedBufferSubData(uvBo, IntPtr.Zero, uvsBytes, ref MemoryMarshal.GetReference(uvs));
            }

#if DEBUG
            GL.GetNamedBufferParameter(uvBo, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes < uvsBytes)
                throw new ApplicationException(string.Format("Problem uploading uv buffer to VBO (uvs). Tried to upload {0} bytes, uploaded {1}.", uvsBytes, vboBytes));
#endif
        }

        /// <summary>
        /// Binds the colors onto the GL Render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="ArgumentException">colors must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetColors(IMeshImp mr, ReadOnlySpan<uint> colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("colors must not be null or empty");
            }

            int sizeOfColor = sizeof(uint);
            int colorBytes = colors.Length * sizeOfColor;
            int cBo;
            if (((MeshImp)mr).ColorBufferObject == 0)
            {
                GL.CreateBuffers(1, out cBo);
                ((MeshImp)mr).ColorBufferObject = cBo;

                var vao = ((MeshImp)mr).VertexArrayObject;
                if (vao == 0)
                {
                    throw new ApplicationException("Create the VAO first!");
                }
                GL.NamedBufferStorage(cBo, colorBytes, ref MemoryMarshal.GetReference(colors), BufferStorageFlags.DynamicStorageBit);
                GL.VertexArrayVertexBuffer(vao, AttributeLocations.ColorAttribBindingIndex, cBo, IntPtr.Zero, sizeOfColor);

                GL.VertexArrayAttribFormat(vao, AttributeLocations.ColorAttribLocation, 4, VertexAttribType.UnsignedByte, true, 0);
                GL.VertexArrayAttribBinding(vao, AttributeLocations.ColorAttribLocation, AttributeLocations.ColorAttribBindingIndex);
            }
            else
            {
                cBo = ((MeshImp)mr).ColorBufferObject;
                GL.GetNamedBufferParameter(cBo, BufferParameterName.BufferSize, out int size);
                if (size < colorBytes)
                {
                    GL.DeleteBuffer(cBo);
                    GL.CreateBuffers(1, out cBo);

                    var vao = ((MeshImp)mr).VertexArrayObject;
                    GL.NamedBufferStorage(cBo, colorBytes, ref MemoryMarshal.GetReference(colors), BufferStorageFlags.DynamicStorageBit);
                    GL.VertexArrayVertexBuffer(((MeshImp)mr).VertexArrayObject, AttributeLocations.ColorAttribLocation, cBo, IntPtr.Zero, sizeOfColor);

                }
                else
                    GL.NamedBufferSubData(cBo, IntPtr.Zero, colorBytes, ref MemoryMarshal.GetReference(colors));
            }

#if DEBUG
            GL.GetNamedBufferParameter(cBo, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes < colorBytes)
                throw new ApplicationException(string.Format("Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.", colorBytes, vboBytes));
#endif
        }

        /// <summary>
        /// Binds the colors onto the GL Render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="ArgumentException">colors must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetColors1(IMeshImp mr, ReadOnlySpan<uint> colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("colors must not be null or empty");
            }

            int sizeOfColor = sizeof(uint);
            int colorBytes = colors.Length * sizeOfColor;
            int cBo;
            if (((MeshImp)mr).ColorBufferObject1 == 0)
            {
                GL.CreateBuffers(1, out cBo);
                ((MeshImp)mr).ColorBufferObject1 = cBo;

                var vao = ((MeshImp)mr).VertexArrayObject;
                if (vao == 0)
                {
                    throw new ApplicationException("Create the VAO first!");
                }
                GL.NamedBufferStorage(cBo, colorBytes, ref MemoryMarshal.GetReference(colors), BufferStorageFlags.DynamicStorageBit);
                GL.VertexArrayVertexBuffer(vao, AttributeLocations.ColorAttribBindingIndex, cBo, IntPtr.Zero, sizeOfColor);

                GL.VertexArrayAttribFormat(vao, AttributeLocations.Color1AttribLocation, 4, VertexAttribType.UnsignedByte, true, 0);
                GL.VertexArrayAttribBinding(vao, AttributeLocations.Color1AttribLocation, AttributeLocations.ColorAttribBindingIndex);
            }
            else
            {
                cBo = ((MeshImp)mr).ColorBufferObject;
                GL.GetNamedBufferParameter(cBo, BufferParameterName.BufferSize, out int size);
                if (size < colorBytes)
                {
                    GL.DeleteBuffer(cBo);
                    GL.CreateBuffers(1, out cBo);

                    var vao = ((MeshImp)mr).VertexArrayObject;
                    GL.NamedBufferStorage(cBo, colorBytes, ref MemoryMarshal.GetReference(colors), BufferStorageFlags.DynamicStorageBit);
                    GL.VertexArrayVertexBuffer(((MeshImp)mr).VertexArrayObject, AttributeLocations.Color1AttribBindingIndex, cBo, IntPtr.Zero, sizeOfColor);

                }
                else
                    GL.NamedBufferSubData(cBo, IntPtr.Zero, colorBytes, ref MemoryMarshal.GetReference(colors));
            }

#if DEBUG
            GL.GetNamedBufferParameter(cBo, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes < colorBytes)
                throw new ApplicationException(string.Format("Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.", colorBytes, vboBytes));
#endif
        }

        /// <summary>
        /// Binds the colors onto the GL Render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="ArgumentException">colors must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetColors2(IMeshImp mr, ReadOnlySpan<uint> colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("colors must not be null or empty");
            }

            int sizeOfColor = sizeof(uint);
            int colorBytes = colors.Length * sizeOfColor;
            int cBo;
            if (((MeshImp)mr).ColorBufferObject2 == 0)
            {
                GL.CreateBuffers(1, out cBo);
                ((MeshImp)mr).ColorBufferObject2 = cBo;

                var vao = ((MeshImp)mr).VertexArrayObject;
                if (vao == 0)
                {
                    throw new ApplicationException("Create the VAO first!");
                }
                GL.NamedBufferStorage(cBo, colorBytes, ref MemoryMarshal.GetReference(colors), BufferStorageFlags.DynamicStorageBit);
                GL.VertexArrayVertexBuffer(vao, AttributeLocations.Color2AttribBindingIndex, cBo, IntPtr.Zero, sizeOfColor);

                GL.VertexArrayAttribFormat(vao, AttributeLocations.Color2AttribLocation, 4, VertexAttribType.UnsignedByte, true, 0);
                GL.VertexArrayAttribBinding(vao, AttributeLocations.Color2AttribLocation, AttributeLocations.ColorAttribBindingIndex);
            }
            else
            {
                cBo = ((MeshImp)mr).ColorBufferObject;
                GL.GetNamedBufferParameter(cBo, BufferParameterName.BufferSize, out int size);
                if (size < colorBytes)
                {
                    GL.DeleteBuffer(cBo);
                    GL.CreateBuffers(1, out cBo);

                    var vao = ((MeshImp)mr).VertexArrayObject;
                    GL.NamedBufferStorage(cBo, colorBytes, ref MemoryMarshal.GetReference(colors), BufferStorageFlags.DynamicStorageBit);
                    GL.VertexArrayVertexBuffer(((MeshImp)mr).VertexArrayObject, AttributeLocations.Color2AttribBindingIndex, cBo, IntPtr.Zero, sizeOfColor);

                }
                else
                    GL.NamedBufferSubData(cBo, IntPtr.Zero, colorBytes, ref MemoryMarshal.GetReference(colors));
            }

#if DEBUG
            GL.GetNamedBufferParameter(cBo, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes < colorBytes)
                throw new ApplicationException(string.Format("Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.", colorBytes, vboBytes));
#endif
        }

        /// <summary>
        /// Binds the flags onto the GL Render context and assigns a buffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="flags">The flags.</param>
        /// <exception cref="ArgumentException">Flags must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetFlags(IMeshImp mr, ReadOnlySpan<uint> flags)
        {
            if (flags == null || flags.Length == 0)
            {
                throw new ArgumentException("Flags must not be null or empty");
            }

            int sizeOfFlag = sizeof(uint);
            int flagsBytes = flags.Length * sizeOfFlag;
            int fBo;
            if (((MeshImp)mr).FlagsBufferObject == 0)
            {
                GL.CreateBuffers(1, out fBo);
                ((MeshImp)mr).FlagsBufferObject = fBo;

                var vao = ((MeshImp)mr).VertexArrayObject;
                if (vao == 0)
                {
                    throw new ApplicationException("Create the VAO first!");
                }
                GL.NamedBufferStorage(fBo, flagsBytes, ref MemoryMarshal.GetReference(flags), BufferStorageFlags.DynamicStorageBit);
                GL.VertexArrayVertexBuffer(vao, AttributeLocations.FlagsBindingIndex, fBo, IntPtr.Zero, sizeOfFlag);

                GL.VertexArrayAttribIFormat(vao, AttributeLocations.FlagsAttribLocation, 1, VertexAttribIntegerType.UnsignedInt, 0);
                GL.VertexArrayAttribBinding(vao, AttributeLocations.FlagsAttribLocation, AttributeLocations.FlagsBindingIndex);
            }
            else
            {
                fBo = ((MeshImp)mr).FlagsBufferObject;
                GL.GetNamedBufferParameter(fBo, BufferParameterName.BufferSize, out int size);
                if (size < flagsBytes)
                {
                    GL.DeleteBuffer(fBo);
                    GL.CreateBuffers(1, out fBo);

                    var vao = ((MeshImp)mr).VertexArrayObject;
                    GL.NamedBufferStorage(fBo, flagsBytes, ref MemoryMarshal.GetReference(flags), BufferStorageFlags.DynamicStorageBit);
                    GL.VertexArrayVertexBuffer(((MeshImp)mr).VertexArrayObject, AttributeLocations.FlagsBindingIndex, fBo, IntPtr.Zero, sizeOfFlag);

                }
                else
                    GL.NamedBufferSubData(fBo, IntPtr.Zero, flagsBytes, ref MemoryMarshal.GetReference(flags));
            }

#if DEBUG
            GL.GetNamedBufferParameter(fBo, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes < flagsBytes)
                throw new ApplicationException(string.Format("Problem uploading flags buffer to VBO (flags). Tried to upload {0} bytes, uploaded {1}.", flagsBytes, vboBytes));
#endif
        }

        /// <summary>
        /// Binds the triangles onto the GL Render context and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="triangleIndices">The triangle indices.</param>
        /// <exception cref="ArgumentException">triangleIndices must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetTriangles(IMeshImp mr, ReadOnlySpan<uint> triangleIndices)
        {
            if (triangleIndices == null || triangleIndices.Length == 0)
            {
                throw new ArgumentException("triangleIndices must not be null or empty");
            }
            ((MeshImp)mr).NElements = triangleIndices.Length;

            int sizeOfTri = sizeof(uint);
            int trisBytes = triangleIndices.Length * sizeOfTri;

            int ibo;
            if (((MeshImp)mr).ElementBufferObject == 0)
            {
                GL.CreateBuffers(1, out ibo);
                ((MeshImp)mr).ElementBufferObject = ibo;
                int vao = ((MeshImp)mr).VertexArrayObject;
                if (vao == 0)
                {
                    throw new ApplicationException("Create the VAO first!");
                }
                // Upload the index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
                GL.NamedBufferStorage(ibo, trisBytes, ref MemoryMarshal.GetReference(triangleIndices), BufferStorageFlags.DynamicStorageBit);
                GL.VertexArrayElementBuffer(vao, ibo);
            }
            else
            {
                ibo = ((MeshImp)mr).ElementBufferObject;
                GL.GetNamedBufferParameter(ibo, BufferParameterName.BufferSize, out int size);
                if (size < trisBytes)
                {
                    GL.DeleteBuffer(ibo);
                    GL.CreateBuffers(1, out ibo);

                    var vao = ((MeshImp)mr).VertexArrayObject;
                    GL.NamedBufferStorage(ibo, trisBytes, ref MemoryMarshal.GetReference(triangleIndices), BufferStorageFlags.DynamicStorageBit);
                    GL.VertexArrayElementBuffer(vao, ibo);

                }
                else
                    GL.NamedBufferSubData(ibo, IntPtr.Zero, trisBytes, ref MemoryMarshal.GetReference(triangleIndices));
            }

#if DEBUG
            GL.GetNamedBufferParameter(ibo, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes < trisBytes)
                throw new ApplicationException(string.Format("Problem uploading vertex buffer to VBO (offsets). Tried to upload {0} bytes, uploaded {1}.", trisBytes, vboBytes));
#endif
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh whose buffers respectively GPU memory should be deleted.</param>
        public void RemoveVertices(IMeshImp mr)
        {
            GL.DeleteVertexArray(((MeshImp)mr).VertexArrayObject);
            GL.DeleteBuffer(((MeshImp)mr).VertexBufferObject);
            ((MeshImp)mr).InvalidateVertices();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="instanceImp">The instance data whose buffers are to be deleted.</param>
        public void RemoveInstanceData(IInstanceDataImp instanceImp)
        {
            GL.DeleteBuffer(((InstanceDataImp)instanceImp).InstanceTransformBufferObject);
            GL.DeleteBuffer(((InstanceDataImp)instanceImp).InstanceColorBufferObject);
            ((InstanceDataImp)instanceImp).InstanceTransformBufferObject = 0;
            ((InstanceDataImp)instanceImp).InstanceColorBufferObject = 0;
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveNormals(IMeshImp mr)
        {
            GL.DeleteBuffer(((MeshImp)mr).NormalBufferObject);
            ((MeshImp)mr).InvalidateNormals();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveColors(IMeshImp mr)
        {
            GL.DeleteBuffer(((MeshImp)mr).ColorBufferObject);
            ((MeshImp)mr).InvalidateColors();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveColors1(IMeshImp mr)
        {
            int bufferObj = ((MeshImp)mr).ColorBufferObject1;
            GL.DeleteBuffers(1, ref bufferObj);
            ((MeshImp)mr).InvalidateColors1();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveColors2(IMeshImp mr)
        {
            int bufferObj = ((MeshImp)mr).ColorBufferObject2;
            GL.DeleteBuffers(1, ref bufferObj);
            ((MeshImp)mr).InvalidateColors2();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveUVs(IMeshImp mr)
        {
            GL.DeleteBuffer(((MeshImp)mr).UVBufferObject);
            ((MeshImp)mr).InvalidateUVs();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveTriangles(IMeshImp mr)
        {
            GL.DeleteBuffer(((MeshImp)mr).ElementBufferObject);
            ((MeshImp)mr).InvalidateTriangles();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBoneWeights(IMeshImp mr)
        {
            GL.DeleteBuffer(((MeshImp)mr).BoneWeightBufferObject);
            ((MeshImp)mr).InvalidateBoneWeights();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBoneIndices(IMeshImp mr)
        {
            GL.DeleteBuffer(((MeshImp)mr).BoneIndexBufferObject);
            ((MeshImp)mr).InvalidateBoneIndices();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveTangents(IMeshImp mr)
        {
            GL.DeleteBuffer(((MeshImp)mr).TangentBufferObject);
            ((MeshImp)mr).InvalidateTangents();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBiTangents(IMeshImp mr)
        {
            GL.DeleteBuffer(((MeshImp)mr).BitangentBufferObject);
            ((MeshImp)mr).InvalidateBiTangents();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveFlags(IMeshImp mr)
        {
            int bufferObj = ((MeshImp)mr).FlagsBufferObject;
            GL.DeleteBuffers(1, ref bufferObj);
            ((MeshImp)mr).InvalidateFlags();
        }

        /// <summary>
        /// Defines a barrier ordering memory transactions. At the moment it will insert all supported barriers.
        /// </summary>
        public void MemoryBarrier()
        {
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        }

        /// <summary>
        /// Launch the bound Compute Shader Program.
        /// </summary>
        /// <param name="kernelIndex"></param>
        /// <param name="threadGroupsX">The number of work groups to be launched in the X dimension.</param>
        /// <param name="threadGroupsY">The number of work groups to be launched in the Y dimension.</param>
        /// <param name="threadGroupsZ">he number of work groups to be launched in the Z dimension.</param>
        public void DispatchCompute(int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ)
        {
            GL.DispatchCompute(threadGroupsX, threadGroupsY, threadGroupsZ);
        }

        /// <summary>
        /// Renders the specified <see cref="IMeshImp" />.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="instanceData">Optional parameter for using instance rendering.</param>
        public void Render(IMeshImp mr, IInstanceDataImp instanceData = null)
        {
            var vao = ((MeshImp)mr).VertexArrayObject;
            GL.BindVertexArray(vao);

            if (((MeshImp)mr).VertexBufferObject != 0)
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.VertexAttribLocation);
            if (((MeshImp)mr).ColorBufferObject != 0)
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.ColorAttribLocation);
            if (((MeshImp)mr).ColorBufferObject1 != 0)
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.Color1AttribLocation);
            if (((MeshImp)mr).ColorBufferObject2 != 0)
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.Color2AttribLocation);
            if (((MeshImp)mr).UVBufferObject != 0)
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.UvAttribLocation);
            if (((MeshImp)mr).NormalBufferObject != 0)
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.NormalAttribLocation);
            if (((MeshImp)mr).TangentBufferObject != 0)
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.TangentAttribLocation);
            if (((MeshImp)mr).BitangentBufferObject != 0)
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.BitangentAttribLocation);
            if (((MeshImp)mr).BoneIndexBufferObject != 0)
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.BoneIndexAttribLocation);
            if (((MeshImp)mr).BoneWeightBufferObject != 0)
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.BoneWeightAttribLocation);
            if (((MeshImp)mr).FlagsBufferObject != 0)
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.FlagsAttribLocation);

            if (((MeshImp)mr).ElementBufferObject == 0) throw new ApplicationException("Element/Index buffer not initialized!");
            var oglPrimitiveType = ((MeshImp)mr).MeshType switch
            {
                Common.PrimitiveType.Points => OpenTK.Graphics.OpenGL.PrimitiveType.Points,
                Common.PrimitiveType.Lines => OpenTK.Graphics.OpenGL.PrimitiveType.Lines,
                Common.PrimitiveType.LineLoop => OpenTK.Graphics.OpenGL.PrimitiveType.LineLoop,
                Common.PrimitiveType.LineStrip => OpenTK.Graphics.OpenGL.PrimitiveType.LineStrip,
                Common.PrimitiveType.Patches => OpenTK.Graphics.OpenGL.PrimitiveType.Patches,
                Common.PrimitiveType.QuadStrip => OpenTK.Graphics.OpenGL.PrimitiveType.QuadStrip,
                Common.PrimitiveType.TriangleFan => OpenTK.Graphics.OpenGL.PrimitiveType.TriangleFan,
                Common.PrimitiveType.TriangleStrip => OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip,
                Common.PrimitiveType.Quads => OpenTK.Graphics.OpenGL.PrimitiveType.Quads,
                Common.PrimitiveType.LineAdjacency => OpenTK.Graphics.OpenGL.PrimitiveType.LinesAdjacency,
                _ => OpenTK.Graphics.OpenGL.PrimitiveType.Triangles,
            };

            if (instanceData != null)
            {
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.InstancedModelMat1);
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.InstancedModelMat2);
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.InstancedModelMat3);
                GL.EnableVertexArrayAttrib(vao, AttributeLocations.InstancedModelMat4);

                if (((InstanceDataImp)instanceData).InstanceColorBufferObject != 0)
                {
                    GL.EnableVertexArrayAttrib(vao, AttributeLocations.InstancedColor);
                    //Needed in case of one Mesh / VBO used for more than one InstanceData / InstanceTransformBufferObject -> reset pointer
                    GL.VertexArrayVertexBuffer(vao, AttributeLocations.InstancedColorBindingIndex, ((InstanceDataImp)instanceData).InstanceColorBufferObject, IntPtr.Zero, 4 * sizeof(float));
                }

                //Needed in case of one Mesh / VBO used for more than one InstanceData / InstanceTransformBufferObject -> reset pointer
                GL.VertexArrayVertexBuffer(vao, AttributeLocations.InstancedModelMatBindingIndex, ((InstanceDataImp)instanceData).InstanceTransformBufferObject, IntPtr.Zero, 16 * sizeof(float));

                GL.DrawElementsInstanced(oglPrimitiveType, ((MeshImp)mr).NElements, DrawElementsType.UnsignedInt, IntPtr.Zero, instanceData.Amount);

                GL.DisableVertexArrayAttrib(vao, AttributeLocations.InstancedModelMat1);
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.InstancedModelMat2);
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.InstancedModelMat3);
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.InstancedModelMat4);
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.InstancedColor);
            }
            else
                GL.DrawElements(oglPrimitiveType, ((MeshImp)mr).NElements, DrawElementsType.UnsignedInt, IntPtr.Zero);

            if (((MeshImp)mr).VertexBufferObject != 0)
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.VertexAttribLocation);
            if (((MeshImp)mr).ColorBufferObject != 0)
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.ColorAttribLocation);
            if (((MeshImp)mr).ColorBufferObject1 != 0)
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.Color1AttribLocation);
            if (((MeshImp)mr).ColorBufferObject2 != 0)
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.Color2AttribLocation);
            if (((MeshImp)mr).UVBufferObject != 0)
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.UvAttribLocation);
            if (((MeshImp)mr).NormalBufferObject != 0)
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.NormalAttribLocation);
            if (((MeshImp)mr).TangentBufferObject != 0)
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.TangentAttribLocation);
            if (((MeshImp)mr).BitangentBufferObject != 0)
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.BitangentAttribLocation);
            if (((MeshImp)mr).BoneIndexBufferObject != 0)
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.BoneIndexAttribLocation);
            if (((MeshImp)mr).BoneWeightBufferObject != 0)
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.BoneWeightAttribLocation);
            if (((MeshImp)mr).FlagsBufferObject != 0)
                GL.DisableVertexArrayAttrib(vao, AttributeLocations.FlagsAttribLocation);

            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Gets the content of the buffer.
        /// </summary>
        /// <param name="quad">The Rectangle where the content is draw into.</param>
        /// <param name="texId">The texture identifier.</param>
        public void GetBufferContent(Common.Rectangle quad, ITextureHandle texId)
        {
            GL.BindTexture(TextureTarget.Texture2D, ((TextureHandle)texId).TexId);
            GL.CopyTexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, quad.Left, quad.Top, quad.Width, quad.Height, 0);
        }

        /// <summary>
        /// Creates the mesh implementation.
        /// </summary>
        /// <returns>The <see cref="IMeshImp" /> instance.</returns>
        public IMeshImp CreateMeshImp()
        {
            return new MeshImp();
        }

        /// <summary>
        /// Creates the instance data implementation.
        /// </summary>
        /// <returns>The <see cref="IInstanceDataImp" /> instance.</returns>
        public IInstanceDataImp CreateInstanceDataImp(IMeshImp meshImp)
        {
            var instanceImp = new InstanceDataImp
            {
                VertexArrayObject = ((MeshImp)meshImp).VertexArrayObject
            };
            return instanceImp;
        }

        internal static BlendEquationMode BlendOperationToOgl(BlendOperation bo)
        {
            return bo switch
            {
                BlendOperation.Add => BlendEquationMode.FuncAdd,
                BlendOperation.Subtract => BlendEquationMode.FuncSubtract,
                BlendOperation.ReverseSubtract => BlendEquationMode.FuncReverseSubtract,
                BlendOperation.Minimum => BlendEquationMode.Min,
                BlendOperation.Maximum => BlendEquationMode.Max,
                _ => throw new ArgumentOutOfRangeException($"Invalid argument: {bo}"),
            };
        }

        internal static BlendOperation BlendOperationFromOgl(BlendEquationMode bom)
        {
            return bom switch
            {
                BlendEquationMode.FuncAdd => BlendOperation.Add,
                BlendEquationMode.Min => BlendOperation.Minimum,
                BlendEquationMode.Max => BlendOperation.Maximum,
                BlendEquationMode.FuncSubtract => BlendOperation.Subtract,
                BlendEquationMode.FuncReverseSubtract => BlendOperation.ReverseSubtract,
                _ => throw new ArgumentOutOfRangeException($"Invalid argument: {bom}"),
            };
        }

        internal static int BlendToOgl(Blend blend, bool isForBlendFactorAlpha = false)
        {
            return blend switch
            {
                Blend.Zero => (int)BlendingFactorSrc.Zero,
                Blend.One => (int)BlendingFactorSrc.One,
                Blend.SourceColor => (int)BlendingFactorDest.SrcColor,
                Blend.InverseSourceColor => (int)BlendingFactorDest.OneMinusSrcColor,
                Blend.SourceAlpha => (int)BlendingFactorSrc.SrcAlpha,
                Blend.InverseSourceAlpha => (int)BlendingFactorSrc.OneMinusSrcAlpha,
                Blend.DestinationAlpha => (int)BlendingFactorSrc.DstAlpha,
                Blend.InverseDestinationAlpha => (int)BlendingFactorSrc.OneMinusDstAlpha,
                Blend.DestinationColor => (int)BlendingFactorSrc.DstColor,
                Blend.InverseDestinationColor => (int)BlendingFactorSrc.OneMinusDstColor,
                Blend.BlendFactor => (int)((isForBlendFactorAlpha) ? BlendingFactorSrc.ConstantAlpha : BlendingFactorSrc.ConstantColor),
                Blend.InverseBlendFactor => (int)((isForBlendFactorAlpha) ? BlendingFactorSrc.OneMinusConstantAlpha : BlendingFactorSrc.OneMinusConstantColor),
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
                _ => throw new ArgumentOutOfRangeException(nameof(blend)),
            };
        }

        internal static Blend BlendFromOgl(int bf)
        {
            return bf switch
            {
                (int)BlendingFactorSrc.Zero => Blend.Zero,
                (int)BlendingFactorSrc.One => Blend.One,
                (int)BlendingFactorDest.SrcColor => Blend.SourceColor,
                (int)BlendingFactorDest.OneMinusSrcColor => Blend.InverseSourceColor,
                (int)BlendingFactorSrc.SrcAlpha => Blend.SourceAlpha,
                (int)BlendingFactorSrc.OneMinusSrcAlpha => Blend.InverseSourceAlpha,
                (int)BlendingFactorSrc.DstAlpha => Blend.DestinationAlpha,
                (int)BlendingFactorSrc.OneMinusDstAlpha => Blend.InverseDestinationAlpha,
                (int)BlendingFactorSrc.DstColor => Blend.DestinationColor,
                (int)BlendingFactorSrc.OneMinusDstColor => Blend.InverseDestinationColor,
                (int)BlendingFactorSrc.ConstantAlpha or (int)BlendingFactorSrc.ConstantColor => Blend.BlendFactor,
                (int)BlendingFactorSrc.OneMinusConstantAlpha or (int)BlendingFactorSrc.OneMinusConstantColor => Blend.InverseBlendFactor,
                _ => throw new ArgumentOutOfRangeException("blend"),
            };
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
                        PolygonMode pm;

                        switch ((FillMode)value)
                        {
                            case FillMode.Point:
                                pm = PolygonMode.Point;
                                //Needed for rendering points
                                if (!_isPtRenderingEnabled)
                                {
                                    GL.Enable(EnableCap.ProgramPointSize);
                                    GL.Enable(EnableCap.VertexProgramPointSize);
                                    _isPtRenderingEnabled = true;
                                }
                                break;
                            case FillMode.Wireframe:
                                pm = PolygonMode.Line;
                                if (!_isLineSmoothEnabled)
                                {
                                    //Needed for rendering smooth lines
                                    GL.Enable(EnableCap.LineSmooth);
                                    _isLineSmoothEnabled = true;
                                }
                                break;
                            case FillMode.Solid:
                                pm = PolygonMode.Fill;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(value));
                        }
                        GL.PolygonMode(MaterialFace.FrontAndBack, pm);
                        return;
                    }
                case RenderState.CullMode:
                    {
                        switch ((Cull)value)
                        {
                            case Cull.None:
                                if (_isCullEnabled)
                                {
                                    _isCullEnabled = false;
                                    GL.Disable(EnableCap.CullFace);
                                }
                                GL.FrontFace(FrontFaceDirection.Ccw);
                                break;
                            case Cull.Clockwise:
                                if (!_isCullEnabled)
                                {
                                    _isCullEnabled = true;
                                    GL.Enable(EnableCap.CullFace);
                                }
                                GL.FrontFace(FrontFaceDirection.Cw);
                                break;
                            case Cull.Counterclockwise:
                                if (!_isCullEnabled)
                                {
                                    _isCullEnabled = true;
                                    GL.Enable(EnableCap.CullFace);
                                }
                                GL.FrontFace(FrontFaceDirection.Ccw);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(value));
                        }
                    }
                    break;
                case RenderState.Clipping:
                    // clipping is always on in OpenGL - This state is simply ignored
                    break;
                case RenderState.ZFunc:
                    {
                        DepthFunction df = GetDepthCompareFunc((Compare)value);
                        GL.DepthFunc(df);
                    }
                    break;
                case RenderState.ZEnable:
                    if (value == 0)
                        GL.Disable(EnableCap.DepthTest);
                    else
                        GL.Enable(EnableCap.DepthTest);
                    break;
                case RenderState.ZWriteEnable:
                    GL.DepthMask(value != 0);
                    break;
                case RenderState.AlphaBlendEnable:
                    if (value == 0)
                        GL.Disable(EnableCap.Blend);
                    else
                        GL.Enable(EnableCap.Blend);
                    break;
                case RenderState.BlendOperation:
                    {
                        _blendEquationRgb = BlendOperationToOgl((BlendOperation)value);
                        GL.BlendEquationSeparate(_blendEquationRgb, _blendEquationAlpha);
                    }
                    break;

                case RenderState.BlendOperationAlpha:
                    {
                        _blendEquationAlpha = BlendOperationToOgl((BlendOperation)value);
                        GL.BlendEquationSeparate(_blendEquationRgb, _blendEquationAlpha);
                    }
                    break;
                case RenderState.SourceBlend:
                    {
                        _blendSrcRgb = (BlendingFactorSrc)BlendToOgl((Blend)value);
                        GL.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.DestinationBlend:
                    {
                        _blendDstRgb = (BlendingFactorDest)BlendToOgl((Blend)value);
                        GL.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.SourceBlendAlpha:
                    {
                        _blendSrcAlpha = (BlendingFactorSrc)BlendToOgl((Blend)value);
                        GL.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.DestinationBlendAlpha:
                    {
                        _blendDstAlpha = (BlendingFactorDest)BlendToOgl((Blend)value);
                        GL.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.BlendFactor:
                    GL.BlendColor(System.Drawing.Color.FromArgb((int)value));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderState));
            }
        }

        /// <summary>
        /// Gets the current RenderState that is applied to the current OpenGL based RenderContext.
        /// </summary>
        /// <param name="renderState">State of the render. See <see cref="RenderState"/> for further information.</param>
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
                        GL.GetInteger(GetPName.PolygonMode, out int pm);
                        var ret = (PolygonMode)pm switch
                        {
                            PolygonMode.Point => FillMode.Point,
                            PolygonMode.Line => FillMode.Wireframe,
                            PolygonMode.Fill => FillMode.Solid,
                            _ => throw new ArgumentOutOfRangeException("pm", "Value " + ((PolygonMode)pm) + " not handled"),
                        };
                        return (uint)ret;
                    }
                case RenderState.CullMode:
                    {
                        GL.GetInteger(GetPName.CullFace, out int cullFace);
                        if (cullFace == 0)
                            return (uint)Cull.None;
                        GL.GetInteger(GetPName.FrontFace, out int frontFace);
                        if (frontFace == (int)FrontFaceDirection.Cw)
                            return (uint)Cull.Clockwise;
                        return (uint)Cull.Counterclockwise;
                    }
                case RenderState.Clipping:
                    // clipping is always on in OpenGL - This state is simply ignored
                    return 1; // == true
                case RenderState.ZFunc:
                    {
                        GL.GetInteger(GetPName.DepthFunc, out int depFunc);
                        var ret = (DepthFunction)depFunc switch
                        {
                            DepthFunction.Never => Compare.Never,
                            DepthFunction.Less => Compare.Less,
                            DepthFunction.Equal => Compare.Equal,
                            DepthFunction.Lequal => Compare.LessEqual,
                            DepthFunction.Greater => Compare.Greater,
                            DepthFunction.Notequal => Compare.NotEqual,
                            DepthFunction.Gequal => Compare.GreaterEqual,
                            DepthFunction.Always => Compare.Always,
                            _ => throw new ArgumentOutOfRangeException("depFunc", "Value " + ((DepthFunction)depFunc) + " not handled"),
                        };
                        return (uint)ret;
                    }
                case RenderState.ZEnable:
                    {
                        GL.GetInteger(GetPName.DepthTest, out int depTest);
                        return (uint)(depTest);
                    }
                case RenderState.ZWriteEnable:
                    {
                        GL.GetInteger(GetPName.DepthWritemask, out int depWriteMask);
                        return (uint)(depWriteMask);
                    }
                case RenderState.AlphaBlendEnable:
                    {
                        GL.GetInteger(GetPName.Blend, out int blendEnable);
                        return (uint)(blendEnable);
                    }
                case RenderState.BlendOperation:
                    {
                        GL.GetInteger(GetPName.BlendEquationRgb, out int rgbMode);
                        return (uint)BlendOperationFromOgl((BlendEquationMode)rgbMode);
                    }
                case RenderState.BlendOperationAlpha:
                    {
                        GL.GetInteger(GetPName.BlendEquationAlpha, out int alphaMode);
                        return (uint)BlendOperationFromOgl((BlendEquationMode)alphaMode);
                    }
                case RenderState.SourceBlend:
                    {
                        GL.GetInteger(GetPName.BlendSrcRgb, out int rgbSrc);
                        return (uint)BlendFromOgl(rgbSrc);
                    }
                case RenderState.DestinationBlend:
                    {
                        GL.GetInteger(GetPName.BlendSrcRgb, out int rgbDst);
                        return (uint)BlendFromOgl(rgbDst);
                    }
                case RenderState.SourceBlendAlpha:
                    {
                        GL.GetInteger(GetPName.BlendSrcAlpha, out int alphaSrc);
                        return (uint)BlendFromOgl(alphaSrc);
                    }
                case RenderState.DestinationBlendAlpha:
                    {
                        GL.GetInteger(GetPName.BlendDstAlpha, out int alphaDst);
                        return (uint)BlendFromOgl(alphaDst);
                    }
                case RenderState.BlendFactor:
                    int col;
                    GL.GetInteger(GetPName.BlendColorExt, out col);
                    return (uint)col;
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderState));
            }
        }

        /// <summary>
        /// Takes a <see cref="WritableMultisampleTexture"/> and blits the result of all samples into an
        /// existing <see cref="WritableTexture"/> for further use (e. g. bind and use as Albedo texture)
        /// </summary>
        /// <param name="input">WritableMultisampleTexture</param>
        /// <param name="output">WritableTexture</param>
        public void BlitMultisample2DTextureToTexture(IWritableTexture input, IWritableTexture output)
        {
            if (input == null || output == null) return;

            if (((TextureHandle)output.TextureHandle).FrameBufferHandle == -1)
            {
                var texHandle = output.TextureHandle;
                GL.CreateFramebuffers(1, out int fBuffer);
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;

                ((TextureHandle)texHandle).DepthRenderBufferHandle = CreateDepthRenderBuffer(output.Width, output.Height, fBuffer);
                GL.NamedFramebufferTexture(fBuffer, FramebufferAttachment.ColorAttachment0, ((TextureHandle)texHandle).TexId, 0);

#if DEBUG
                if (GL.CheckNamedFramebufferStatus(fBuffer, FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
                    throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckNamedFramebufferStatus(fBuffer, FramebufferTarget.Framebuffer)}");
#endif

                //throw new ArgumentOutOfRangeException("Output Framebuffer is uninitialized");
            }
            GL.BlitNamedFramebuffer(((TextureHandle)((WritableMultisampleTexture)input).InternalTextureHandle).FrameBufferHandle, ((TextureHandle)output.TextureHandle).FrameBufferHandle, 0, 0, input.Width, input.Height, 0, 0, input.Width, input.Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
        }

        /// <summary>
        /// Renders into the given texture.
        /// </summary>
        /// <param name="tex">The texture.</param>
        /// <param name="texHandle">The texture handle, associated with the given texture. Should be created by the TextureManager in the RenderContext.</param>
        public void SetRenderTarget(IWritableTexture tex, ITextureHandle texHandle)
        {
            int fBuffer;
            if (((TextureHandle)texHandle).FrameBufferHandle == -1)
            {
                GL.CreateFramebuffers(1, out fBuffer);
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    if (tex.GetType() == typeof(WritableMultisampleTexture))
                        ((TextureHandle)texHandle).DepthRenderBufferHandle = CreateDepthRenderBufferMultisample(tex.Width, tex.Height, ((WritableMultisampleTexture)tex).MultisampleFactor, fBuffer);
                    else
                        ((TextureHandle)texHandle).DepthRenderBufferHandle = CreateDepthRenderBuffer(tex.Width, tex.Height, fBuffer);
                    GL.NamedFramebufferTexture(fBuffer, FramebufferAttachment.ColorAttachment0, ((TextureHandle)texHandle).TexId, 0);
                }
                else
                {
                    GL.NamedFramebufferTexture(fBuffer, FramebufferAttachment.DepthAttachment, ((TextureHandle)texHandle).TexId, 0);
                }
            }
            else
            {
                fBuffer = ((TextureHandle)texHandle).FrameBufferHandle;
            }
#if DEBUG
            if (GL.CheckNamedFramebufferStatus(fBuffer, FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckNamedFramebufferStatus(fBuffer, FramebufferTarget.Framebuffer)}");
#endif

            if (_lastBoundFbo != fBuffer)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fBuffer);
                _lastBoundFbo = fBuffer;
            }
        }

        /// <summary>
        /// Renders into the given cube map.
        /// </summary>
        /// <param name="tex">The texture.</param>
        /// <param name="texHandle">The texture handle, associated with the given cube map. Should be created by the TextureManager in the RenderContext.</param>
        public void SetRenderTarget(IWritableCubeMap tex, ITextureHandle texHandle)
        {
            int fBuffer;
            if (((TextureHandle)texHandle).FrameBufferHandle == -1)
            {
                GL.CreateFramebuffers(1, out fBuffer);
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    ((TextureHandle)texHandle).DepthRenderBufferHandle = CreateDepthRenderBuffer(tex.Width, tex.Height, fBuffer);
                    GL.NamedFramebufferTexture(fBuffer, FramebufferAttachment.ColorAttachment0, ((TextureHandle)texHandle).TexId, 0);
                }
                else
                {
                    GL.NamedFramebufferTexture(fBuffer, FramebufferAttachment.DepthAttachment, ((TextureHandle)texHandle).TexId, 0);
                }
            }
            else
            {
                fBuffer = ((TextureHandle)texHandle).FrameBufferHandle;
            }
#if DEBUG
            if (GL.CheckNamedFramebufferStatus(fBuffer, FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckNamedFramebufferStatus(fBuffer, FramebufferTarget.Framebuffer)}");
#endif
            if (_lastBoundFbo != fBuffer)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fBuffer);
                _lastBoundFbo = fBuffer;
            }
        }

        /// <summary>
        /// Renders into the given layer of the array texture.
        /// </summary>
        /// <param name="tex">The array texture.</param>
        /// <param name="layer">The layer to render to.</param>
        /// <param name="texHandle">The texture handle, associated with the given texture. Should be created by the TextureManager in the RenderContext.</param>
        public void SetRenderTarget(IWritableArrayTexture tex, int layer, ITextureHandle texHandle)
        {
            int fBuffer;
            if (((TextureHandle)texHandle).FrameBufferHandle == -1)
            {
                GL.CreateFramebuffers(1, out fBuffer);
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    ((TextureHandle)texHandle).DepthRenderBufferHandle = CreateDepthRenderBuffer(tex.Width, tex.Height, fBuffer);
                    GL.NamedFramebufferTextureLayer(fBuffer, FramebufferAttachment.ColorAttachment0, ((TextureHandle)texHandle).TexId, 0, layer);
                }
                else
                {
                    fBuffer = ((TextureHandle)texHandle).FrameBufferHandle;
                    GL.NamedFramebufferTextureLayer(fBuffer, FramebufferAttachment.DepthAttachment, ((TextureHandle)texHandle).TexId, 0, layer);
                }
            }
            else
            {
                fBuffer = ((TextureHandle)texHandle).FrameBufferHandle;
                GL.NamedFramebufferTextureLayer(fBuffer, FramebufferAttachment.DepthAttachment, ((TextureHandle)texHandle).TexId, 0, layer);
            }
#if DEBUG
            if (GL.CheckNamedFramebufferStatus(fBuffer, FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckNamedFramebufferStatus(fBuffer, FramebufferTarget.Framebuffer)}");
#endif

            if (_lastBoundFbo != fBuffer)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fBuffer);
                _lastBoundFbo = fBuffer;
            }
        }

        /// <summary>
        /// Renders into the given textures of the RenderTarget.
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="texHandles">The texture handles, associated with the given textures. Each handle should be created by the TextureManager in the RenderContext.</param>
        public void SetRenderTarget(IRenderTarget renderTarget, ITextureHandle[] texHandles)
        {
            if (renderTarget == null || (renderTarget.RenderTextures.All(x => x == null)))
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                _lastBoundFbo = 0;
                return;
            }

            int gBuffer;

            if (renderTarget.GBufferHandle == null)
            {
                renderTarget.GBufferHandle = new FrameBufferHandle();
                gBuffer = CreateFrameBuffer(renderTarget, texHandles);
                ((FrameBufferHandle)renderTarget.GBufferHandle).Handle = gBuffer;
            }
            else
            {
                gBuffer = ((FrameBufferHandle)renderTarget.GBufferHandle).Handle;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);
                _lastBoundFbo = gBuffer;
            }

            if (renderTarget.RenderTextures[(int)RenderTargetTextureTypes.Depth] == null && !renderTarget.IsDepthOnly)
            {
                int gDepthRenderbufferHandle;
                if (renderTarget.DepthBufferHandle == null)
                {
                    renderTarget.DepthBufferHandle = new RenderBufferHandle();
                    // Create and attach depth buffer (renderbuffer)
                    gDepthRenderbufferHandle = CreateDepthRenderBuffer((int)renderTarget.TextureResolution, (int)renderTarget.TextureResolution, gBuffer);
                    ((RenderBufferHandle)renderTarget.DepthBufferHandle).Handle = gDepthRenderbufferHandle;
                }
                else
                {
                    gDepthRenderbufferHandle = ((RenderBufferHandle)renderTarget.DepthBufferHandle).Handle;
                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);
                }
            }
#if DEBUG
            if (GL.CheckNamedFramebufferStatus(gBuffer, FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
            {
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckNamedFramebufferStatus(gBuffer, FramebufferTarget.Framebuffer)}");
            }
#endif
        }

        private static int CreateDepthRenderBuffer(int width, int height, int framebufferHandle)
        {
            GL.Enable(EnableCap.DepthTest);

            GL.CreateRenderbuffers(1, out int gDepthRenderbufferHandle);

            GL.NamedRenderbufferStorage(gDepthRenderbufferHandle, RenderbufferStorage.DepthComponent24, width, height);
            GL.NamedFramebufferRenderbuffer(framebufferHandle, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);
            return gDepthRenderbufferHandle;
        }

        private static int CreateDepthRenderBufferMultisample(int width, int height, int samples, int framebufferHandle)
        {
            GL.Enable(EnableCap.DepthTest);

            GL.CreateRenderbuffers(1, out int gDepthRenderbufferHandle);

            GL.NamedRenderbufferStorageMultisample(gDepthRenderbufferHandle, samples, RenderbufferStorage.DepthComponent24, width, height);
            GL.NamedFramebufferRenderbuffer(framebufferHandle, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);
            return gDepthRenderbufferHandle;
        }

        private static int CreateFrameBuffer(IRenderTarget renderTarget, ITextureHandle[] texHandles)
        {
            GL.CreateFramebuffers(1, out int gBuffer);

            int depthCnt = 0;

            var depthTexPos = (int)RenderTargetTextureTypes.Depth;

            if (!renderTarget.IsDepthOnly)
            {
                var attachments = new List<DrawBuffersEnum>();

                //Textures
                for (int i = 0; i < texHandles.Length; i++)
                {
                    attachments.Add(DrawBuffersEnum.ColorAttachment0 + i);

                    var texHandle = texHandles[i];
                    if (texHandle == null) continue;

                    if (i == depthTexPos)
                    {
                        GL.NamedFramebufferTexture(gBuffer, FramebufferAttachment.DepthAttachment + (depthCnt), ((TextureHandle)texHandle).TexId, 0);
                        depthCnt++;
                    }
                    else
                        GL.NamedFramebufferTexture(gBuffer, FramebufferAttachment.ColorAttachment0 + (i - depthCnt), ((TextureHandle)texHandle).TexId, 0);
                }
                GL.NamedFramebufferDrawBuffers(gBuffer, attachments.Count, attachments.ToArray());
            }
            else //If a frame-buffer only has a depth texture we don't need draw buffers
            {
                var texHandle = texHandles[depthTexPos];

                if (texHandle != null)
                    GL.NamedFramebufferTexture(gBuffer, FramebufferAttachment.DepthAttachment, ((TextureHandle)texHandle).TexId, 0);
                else
                    throw new NullReferenceException("Texture handle is null!");

                GL.NamedFramebufferDrawBuffer(gBuffer, DrawBufferMode.None);
                GL.NamedFramebufferReadBuffer(gBuffer, ReadBufferMode.None);
            }

            return gBuffer;
        }

        /// <summary>
        /// Detaches a texture from the frame buffer object, associated with the given render target.
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="attachment">Number of the fbo attachment. For example: attachment = 1 will detach the texture currently associated with <see cref="FramebufferAttachment.ColorAttachment1"/>.</param>
        /// <param name="isDepthTex">Determines if the texture is a depth texture. In this case the texture currently associated with <see cref="FramebufferAttachment.DepthAttachment"/> will be detached.</param>
        public void DetachTextureFromFbo(IRenderTarget renderTarget, bool isDepthTex, int attachment = 0)
        {
            ChangeFramebufferTexture2D(renderTarget, attachment, 0, isDepthTex);
        }


        /// <summary>
        /// Attaches a texture to the frame buffer object, associated with the given render target.
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="attachment">Number of the fbo attachment. For example: attachment = 1 will attach the texture to <see cref="FramebufferAttachment.ColorAttachment1"/>.</param>
        /// <param name="isDepthTex">Determines if the texture is a depth texture. In this case the texture is attached to <see cref="FramebufferAttachment.DepthAttachment"/>.</param>
        /// <param name="texHandle">The gpu handle of the texture.</param>
        public void AttacheTextureToFbo(IRenderTarget renderTarget, bool isDepthTex, ITextureHandle texHandle, int attachment = 0)
        {
            ChangeFramebufferTexture2D(renderTarget, attachment, ((TextureHandle)texHandle).TexId, isDepthTex);
        }

        private void ChangeFramebufferTexture2D(IRenderTarget renderTarget, int attachment, int handle, bool isDepth)
        {
            var rtFbo = ((FrameBufferHandle)renderTarget.GBufferHandle).Handle;

            if (!isDepth)
                GL.NamedFramebufferTexture(rtFbo, FramebufferAttachment.ColorAttachment0 + attachment, handle, 0);
            else
                GL.NamedFramebufferTexture(rtFbo, FramebufferAttachment.DepthAttachment, handle, 0);
#if DEBUG
            if (GL.CheckNamedFramebufferStatus(rtFbo, FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckNamedFramebufferStatus(rtFbo, FramebufferTarget.Framebuffer)}");
#endif
        }


        /// <summary>
        /// Only pixels that lie within the scissor box can be modified by drawing commands.
        /// Note that the Scissor test must be enabled for this to work.
        /// </summary>
        /// <param name="x">X Coordinate of the lower left point of the scissor box.</param>
        /// <param name="y">Y Coordinate of the lower left point of the scissor box.</param>
        /// <param name="width">Width of the scissor box.</param>
        /// <param name="height">Height of the scissor box.</param>
        public void Scissor(int x, int y, int width, int height)
        {
            //Should be enabled per default - but WPF seems to disable it...
            if (!GL.IsEnabled(EnableCap.ScissorTest))
                GL.Enable(EnableCap.ScissorTest);
            GL.Scissor(x, y, width, height);
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
            GL.Viewport(x, y, width, height);
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
            GL.ColorMask(red, green, blue, alpha);
        }

        /// <summary>
        /// Returns the capabilities of the underlying graphics hardware
        /// </summary>
        /// <param name="capability"></param>
        /// <returns>uint</returns>
        public uint GetHardwareCapabilities(HardwareCapability capability)
        {
            return capability switch
            {
                HardwareCapability.CanRenderDeferred => !GL.GetString(StringName.Extensions).Contains("EXT_framebuffer_object") ? 0U : 1U,
                HardwareCapability.CanUseGeometryShaders => 1U,
                HardwareCapability.MaxSamples => GetSampleSize(),
                _ => throw new ArgumentOutOfRangeException(nameof(capability), capability, null),
            };
        }

        private uint GetSampleSize()
        {
            GL.GetInternalformat(ImageTarget.Texture2DMultisample, GetSizedInteralFormat(new ImagePixelFormat(ColorFormat.RGBA)), InternalFormatParameter.Samples, 32, out int sampleSize);
            return (uint)sampleSize;
        }

        /// <summary>
        /// Returns a human readable description of the underlying graphics hardware. This implementation reports GL_VENDOR, GL_RENDERER, GL_VERSION and GL_EXTENSIONS.
        /// </summary>
        /// <returns></returns>
        public string GetHardwareDescription()
        {
            return "Vendor: " + GL.GetString(StringName.Vendor) + "\nRenderer: " + GL.GetString(StringName.Renderer) + "\nVersion: " + GL.GetString(StringName.Version) + "\nExtensions: " + GL.GetString(StringName.Extensions);
        }

        /// <summary>
        /// Draws a Debug Line in 3D Space by using a start and end point (float3).
        /// </summary>
        /// <param name="start">The starting point of the DebugLine.</param>
        /// <param name="end">The endpoint of the DebugLine.</param>
        /// <param name="color">The color of the DebugLine.</param>
        public void DebugLine(float3 start, float3 end, float4 color)
        {
            GL.Begin(OpenTK.Graphics.OpenGL.PrimitiveType.Lines);
            GL.Color4(color.x, color.y, color.z, color.w);
            GL.Vertex3(start.x, start.y, start.z);
            GL.Color4(color.x, color.y, color.z, color.w);
            GL.Vertex3(end.x, end.y, end.z);
            GL.End();
        }

        #endregion

        #region Shader Storage Buffer

        /// <summary>
        /// Connects the given SSBO to the currently active shader program.
        /// </summary>
        /// <param name="currentProgram">The handle of the current shader program.</param>
        /// <param name="buffer">The Storage Buffer object on the CPU.</param>
        /// <param name="ssboName">The SSBO's name.</param>
        public void ConnectBufferToShaderStorage(IShaderHandle currentProgram, IStorageBuffer buffer, string ssboName)
        {
            var shaderProgram = ((ShaderHandle)currentProgram).Handle;
            var resInx = GL.GetProgramResourceIndex(shaderProgram, ProgramInterface.ShaderStorageBlock, ssboName);
            GL.ShaderStorageBlockBinding(shaderProgram, resInx, buffer.BindingIndex);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, buffer.BindingIndex, ((StorageBufferHandle)buffer.BufferHandle).Handle);
        }

        /// <summary>
        /// Uploads the given data to the SSBO. If the buffer is not created on the GPU by no it will be.
        /// </summary>
        /// <typeparam name="T">The data type.</typeparam>
        /// <param name="storageBuffer">The Storage Buffer Object on the CPU.</param>
        /// <param name="data">The data that will be uploaded.</param>
        public void StorageBufferSetData<T>(IStorageBuffer storageBuffer, T[] data) where T : struct
        {
            if (storageBuffer.BufferHandle == null)
                storageBuffer.BufferHandle = new StorageBufferHandle();
            var bufferHandle = (StorageBufferHandle)storageBuffer.BufferHandle;
            int dataBytes = storageBuffer.Count * storageBuffer.Size;

            //1. Generate Buffer and or set the data
            if (bufferHandle.Handle == -1)
            {
                GL.CreateBuffers(1, out bufferHandle.Handle);
            }

            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Data must not be null or empty");
            }

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, bufferHandle.Handle);
            GL.NamedBufferData(bufferHandle.Handle, dataBytes, data, BufferUsageHint.DynamicCopy);

            GL.GetNamedBufferParameter(bufferHandle.Handle, BufferParameterName.BufferSize, out int bufferBytes);
            if (bufferBytes != dataBytes)
                throw new ApplicationException(string.Format("Problem uploading bone indices buffer to SSBO. Tried to upload {0} bytes, uploaded {1}.", bufferBytes, dataBytes));

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        }

        /// <summary>
        /// Deletes the shader storage buffer on the GPU.
        /// </summary>
        /// <param name="storageBufferHandle">The buffer object.</param>
        public void DeleteStorageBuffer(IBufferHandle storageBufferHandle)
        {
            GL.DeleteBuffer(((StorageBufferHandle)storageBufferHandle).Handle);
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
            ImageData image = ImageData.CreateImage(w, h, ColorUint.Black);
            GL.ReadPixels(x, y, w, h, PixelFormat.Rgb, PixelType.UnsignedByte, image.PixelData);
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
            float depth = 0;
            GL.ReadPixels(x, y, 1, 1, PixelFormat.DepthComponent, PixelType.UnsignedByte, ref depth);

            return depth;
        }

        #endregion
    }
}