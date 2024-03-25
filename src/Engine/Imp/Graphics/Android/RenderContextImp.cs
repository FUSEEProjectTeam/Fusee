using Android.Content;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Imp.SharedAll;
using Fusee.Math.Core;
using OpenTK.Graphics.ES31;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Fusee.Engine.Imp.Graphics.Android
{
    /// <summary>
    /// Implementation of the <see cref="IRenderContextImp" /> interface for usage with OpenTK framework.
    /// </summary>
    public class RenderContextImp : IRenderContextImp
    {
        /// <summary>
        /// Constant id that describes the renderer. This can be used in shaders to do platform dependent things.
        /// </summary>
        public FuseePlatformId FuseePlatformId { get; } = FuseePlatformId.Android;

        private int _textureCountPerShader;
        private readonly Dictionary<int, int> _shaderParam2TexUnit;
        private readonly Context _androidContext;

        private BlendEquationMode _blendEquationAlpha;
        private BlendEquationMode _blendEquationRgb;
        private BlendingFactorDest _blendDstRgb;
        private BlendingFactorSrc _blendSrcRgb;
        private BlendingFactorSrc _blendSrcAlpha;
        private BlendingFactorDest _blendDstAlpha;

        private bool _isCullEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContextImp"/> class.
        /// </summary>
        /// <param name="renderCanvas">The render canvas interface.</param>
        /// <param name="androidContext">The android <see cref="Context"/>.</param>
        public RenderContextImp(IRenderCanvasImp renderCanvas, Context androidContext)
        {
            _textureCountPerShader = 0;
            _shaderParam2TexUnit = new Dictionary<int, int>();

            _androidContext = androidContext;

            // Due to the right-handed nature of OpenGL and the left-handed design of FUSEE
            // the meaning of what's Front and Back of a face simply flips.
            // TODO - implement this in render states!!!
            GL.CullFace(CullFaceMode.Back);

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

            Diagnostics.Debug(GetHardwareDescription());
        }

        #region Image data related Members

        private TextureCompareMode GetTexComapreMode(Common.TextureCompareMode compareMode)
        {
            return compareMode switch
            {
                TextureCompareMode.None => TextureCompareMode.None,
                Common.TextureCompareMode.CompareRefToTexture => TextureCompareMode.CompareRefToTexture,
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

        private OpenTK.Graphics.ES30.TextureWrapMode GetWrapMode(Common.TextureWrapMode wrapMode)
        {
            switch (wrapMode)
            {
                default:
                case Common.TextureWrapMode.Repeat:
                    return OpenTK.Graphics.ES30.TextureWrapMode.Repeat;

                case Common.TextureWrapMode.MirroredRepeat:
                    return OpenTK.Graphics.ES30.TextureWrapMode.MirroredRepeat;

                case Common.TextureWrapMode.ClampToEdge:
                    return OpenTK.Graphics.ES30.TextureWrapMode.ClampToEdge;

                case Common.TextureWrapMode.ClampToBorder:
                    {
                        Diagnostics.Warn("TextureWrapMode.ClampToBorder is not supported on Android. OpenTK.Graphics.ES30.TextureWrapMode.ClampToEdge is set instead.");
                        return OpenTK.Graphics.ES30.TextureWrapMode.ClampToEdge;
                    }
            }
        }

        private SizedInternalFormat GetSizedInteralFormat(ImagePixelFormat format)
        {
            return format.ColorFormat switch
            {
                ColorFormat.RGBA => SizedInternalFormat.Rgba8,
                ColorFormat.fRGBA16 => SizedInternalFormat.Rgba16f,
                ColorFormat.fRGBA32 => SizedInternalFormat.Rgba32f,
                ColorFormat.iRGBA32 => SizedInternalFormat.Rgba32i,
                _ => throw new ArgumentOutOfRangeException("SizedInternalFormat not supported. Try to use a format with r,g,b and a components."),
            };
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

        private TextureComponentCount GetTexTureComponentCount(ImagePixelFormat format)
        {
            return format.ColorFormat switch
            {
                ColorFormat.RGBA => TextureComponentCount.Rgba,
                ColorFormat.RGB => TextureComponentCount.Rgb,
                ColorFormat.Intensity => TextureComponentCount.Alpha,
                ColorFormat.uiRgb8 => TextureComponentCount.Rgb8ui,
                ColorFormat.fRGB32 => TextureComponentCount.Rgb32f,
                ColorFormat.fRGB16 => TextureComponentCount.Rgb16f,
                ColorFormat.fRGBA16 => TextureComponentCount.Rgba16f,
                ColorFormat.Depth16 => TextureComponentCount.DepthComponent16,
                ColorFormat.Depth24 => TextureComponentCount.DepthComponent24,
                _ => throw new ArgumentException("Unsupported color format!"),
            };
        }

        /*TODO: OpenTK 30ES does not seem to support other PixelInternalFormats other than Rgba, Rgb, Alpha, Luminance,
        even though OpenGL 30es seems to do so (https://www.khronos.org/registry/OpenGL-Refpages/es3.0/html/glTexImage2D.xhtml).
        After some research it seems the OpenTK 30es branch suffers due to the development of OpenTK 40es....
        Furthermore it doesn't seem possible to attach a depth texture to a framebuffer (DEPTH_ATTACHMENT), therefore we need to render depth into a COLOR_ATTACHMENT and create a Depth render buffer.
        This is bound to create a overhead.*/
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
                    internalFormat = PixelInternalFormat.Alpha;
                    format = PixelFormat.Alpha;
                    pxType = PixelType.UnsignedByte;
                    rowAlignment = 1;
                    break;

                case ColorFormat.Depth24:
                case ColorFormat.Depth16:
                    internalFormat = PixelInternalFormat.Rgb;
                    format = PixelFormat.Rgb;
                    pxType = PixelType.UnsignedByte;
                    break;

                case ColorFormat.uiRgb8:
                    // SHOULD:
                    //internalFormat = (PixelInternalFormat)SizedInternalFormat.Rgba8ui;
                    //format = PixelFormat.RgbaInteger;
                    //pxType = PixelType.UnsignedByte;
                    internalFormat = PixelInternalFormat.Rgb;
                    format = PixelFormat.Rgb;
                    pxType = PixelType.UnsignedByte;
                    rowAlignment = 1;
                    break;

                case ColorFormat.fRGB32:
                    // SHOULD:
                    //internalFormat = (PixelInternalFormat)SizedInternalFormat.Rgb32f;
                    //format = PixelFormat.Rgb;
                    //pxType = PixelType.Float;
                    internalFormat = PixelInternalFormat.Rgb;
                    format = PixelFormat.Rgb;
                    pxType = PixelType.UnsignedByte;
                    break;

                case ColorFormat.fRGB16:
                    // SHOULD:
                    //internalFormat = (PixelInternalFormat)SizedInternalFormat.Rgb16f;
                    //format = PixelFormat.Rgb;
                    //pxType = PixelType.Float;
                    internalFormat = PixelInternalFormat.Rgb;
                    format = PixelFormat.Rgb;
                    pxType = PixelType.UnsignedByte;
                    break;
                case ColorFormat.fRGBA16:
                    // SHOULD:
                    //internalFormat = (PixelInternalFormat)SizedInternalFormat.Rgb16f;
                    //format = PixelFormat.Rgb;
                    //pxType = PixelType.Float;
                    internalFormat = PixelInternalFormat.Rgba;
                    format = PixelFormat.Rgba;
                    pxType = PixelType.UnsignedByte;
                    break;
                case ColorFormat.fRGBA32:
                    // SHOULD:
                    //internalFormat = PixelInternalFormat.Rgba32f;
                    //format = PixelFormat.Rgba;
                    //pxType = PixelType.Float;
                    internalFormat = PixelInternalFormat.Rgba;
                    format = PixelFormat.Rgba;
                    pxType = PixelType.UnsignedByte;
                    break;
                case ColorFormat.iRGBA32:
                    // SHOULD:
                    //internalFormat = PixelInternalFormat.Rgba32f;
                    //format = PixelFormat.Rgba;
                    //pxType = PixelType.Float;
                    internalFormat = PixelInternalFormat.Rgba;
                    format = PixelFormat.Rgba;
                    pxType = PixelType.UnsignedByte;
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
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableArrayTexture img)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;
            var glWrapMode = GetWrapMode(img.WrapMode);
            var pxInfo = GetTexturePixelInfo(img.PixelFormat);

            GL.TexImage3D(TextureTarget3D.Texture2DArray, 0, GetTexTureComponentCount(img.PixelFormat), img.Width, img.Height, img.Layers, 0, pxInfo.Format, pxInfo.PxType, IntPtr.Zero);

            if (img.DoGenerateMipMaps)
                GL.GenerateMipmap(TextureTarget.Texture2DArray);

            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureCompareMode, (int)GetTexComapreMode(img.CompareMode));
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureCompareFunc, (int)GetDepthCompareFunc(img.CompareFunc));
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexId = id };

            return texID;
        }

        /// <summary>
        /// Creates a new CubeMap and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableCubeMap img)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;

            var glWrapMode = GetWrapMode(img.WrapMode);

            var pxInfo = GetTexturePixelInfo(img.PixelFormat);

            for (int i = 0; i < 6; i++)
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, pxInfo.InternalFormat, img.Width, img.Height, 0, pxInfo.Format, pxInfo.PxType, IntPtr.Zero);

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)glWrapMode);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexId = id };

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(ITexture img)
        {
            if (img is Texture1D)
                throw new ArgumentException($"Texture1D isn't supported on this platform.");
            else
                return CreateTexture((Texture)img);

            throw new ArgumentException($"{img} has an unknown texture type.");
        }

        private ITextureHandle CreateTexture(Texture tex)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            var glMinMagFilter = GetMinMagFilter(tex.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;

            var glWrapMode = GetWrapMode(tex.WrapMode);

            var pxInfo = GetTexturePixelInfo(tex.ImageData.PixelFormat);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, pxInfo.RowAlignment);
            GL.TexImage2D(TextureTarget.Texture2D, 0, pxInfo.InternalFormat, tex.ImageData.Width, tex.ImageData.Height, 0, pxInfo.Format, pxInfo.PxType, tex.ImageData.PixelData);

            if (tex.DoGenerateMipMaps)
                GL.GenerateMipmap(TextureTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexId = id };

            //Diagnostics.Debug(GL.GetErrorCode());

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableTexture img)
        {
            if (img is WritableTexture wt)
                return CreateTexture(wt);
            if (img is WritableMultisampleTexture)
                throw new NotSupportedException("Android has no MultisampleWritableTexture support!");

            throw new NotImplementedException($"CreateTexture typeof({img}) not found!");
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        private ITextureHandle CreateTexture(WritableTexture img)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;

            var glWrapMode = GetWrapMode(img.WrapMode);

            var pxInfo = GetTexturePixelInfo(img.PixelFormat);

            GL.TexImage2D(TextureTarget.Texture2D, 0, pxInfo.InternalFormat, img.Width, img.Height, 0, pxInfo.Format, pxInfo.PxType, IntPtr.Zero);

            if (img.DoGenerateMipMaps)
                GL.GenerateMipmap(TextureTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)GetTexComapreMode(img.CompareMode));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)GetDepthCompareFunc(img.CompareFunc));

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexId = id };

            //Diagnostics.Debug(GL.GetErrorCode());

            return texID;
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
            var pxInfo = GetTexturePixelInfo(img.ImageData.PixelFormat);

            GL.BindTexture(TextureTarget.Texture2D, ((TextureHandle)tex).TexId);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, pxInfo.RowAlignment);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, startX, startY, width, height,
                pxInfo.Format, PixelType.UnsignedByte, img.ImageData.PixelData);
        }

        /// <summary>
        /// Sets the textures filter mode (<see cref="TextureFilterMode"/> at runtime.
        /// </summary>
        /// <param name="tex">The handle of the texture.</param>
        /// <param name="filterMode">The new filter mode.</param>
        public void SetTextureFilterMode(ITextureHandle tex, TextureFilterMode filterMode)
        {
            GL.BindTexture(TextureTarget.Texture2D, ((TextureHandle)tex).TexId);
            var glMinMagFilter = GetMinMagFilter(filterMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)glMinMagFilter.Item1);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)glMinMagFilter.Item2);
        }

        /// <summary>
        /// Sets the textures filter mode (<see cref="Common.TextureWrapMode"/> at runtime.
        /// </summary>
        /// <param name="tex">The handle of the texture.</param>
        ///<param name="wrapMode">The new wrap mode.</param>
        public void SetTextureWrapMode(ITextureHandle tex, Common.TextureWrapMode wrapMode)
        {
            GL.BindTexture(TextureTarget.Texture2D, ((TextureHandle)tex).TexId);
            var glWrapMode = GetWrapMode(wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)glWrapMode);
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to a frame-buffer object.
        /// </summary>
        /// <param name="bh">The platform dependent abstraction of the gpu buffer handle.</param>
        public void DeleteFrameBuffer(IBufferHandle bh)
        {
            GL.DeleteFramebuffers(1, ref ((FrameBufferHandle)bh).Handle);
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to a render-buffer object.
        /// </summary>
        /// <param name="bh">The platform dependent abstraction of the gpu buffer handle.</param>
        public void DeleteRenderBuffer(IBufferHandle bh)
        {
            GL.DeleteFramebuffers(1, ref ((RenderBufferHandle)bh).Handle);
        }

        /// <summary>
        /// Removes the TextureHandle's buffers and textures from the graphics card's memory
        /// </summary>
        /// <remarks>
        /// Method should be called after an TextureHandle is no longer required by the application.
        /// </remarks>
        /// <param name="textureHandle">An TextureHandle object, containing necessary information for the upload to the graphics card.</param>
        public void RemoveTextureHandle(ITextureHandle textureHandle)
        {
            TextureHandle texHandle = (TextureHandle)textureHandle;

            if (texHandle.FrameBufferHandle != -1)
            {
                GL.DeleteFramebuffers(1, ref texHandle.FrameBufferHandle);
            }

            if (texHandle.DepthRenderBufferHandle != -1)
            {
                GL.DeleteRenderbuffers(1, ref texHandle.DepthRenderBufferHandle);
            }

            if (texHandle.TexId != -1)
            {
                var texId = texHandle.TexId;
                GL.DeleteTexture(texId);
                _textureCountPerShader--;
            }
        }

        #endregion Image data related Members

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
        /// <param name="ps">The pixel(=fragment) shader code.</param>
        /// <param name="gs">The geometry shader code. Caution: must be null because geometry shaders are unsupported by Android right now.</param>
        /// <returns>An instance of <see cref="IShaderHandle" />.</returns>
        /// <exception cref="System.ApplicationException">
        /// </exception>
        public IShaderHandle CreateShaderProgram(string vs, string ps, string gs = null)
        {
            if (!string.IsNullOrEmpty(gs))
                Diagnostics.Warn("Geometry Shaders are unsupported");

            StringBuilder info = new(512);

            int vertexObject = GL.CreateShader(ShaderType.VertexShader);
            int fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(vertexObject, 1, new[] { vs }, new[] { vs.Length });
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, 512, out _, info);
            GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out int statusCode);

            if (statusCode != 1)
            {
                _ = info.ToString();
                throw new ApplicationException(info.ToString());
            }

            // Compile pixel shader
            GL.ShaderSource(fragmentObject, 1, new[] { ps }, new[] { ps.Length });
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(vertexObject, 512, out _, info);
            GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info.ToString());

            int program = GL.CreateProgram();
            GL.AttachShader(program, fragmentObject);
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

            GL.LinkProgram(program);
            return new ShaderHandle { Handle = program };
        }

        /// <inheritdoc />
        /// <summary>
        /// Removes shader from the GPU
        /// </summary>
        /// <param name="sp"></param>
        public void RemoveShader(IShaderHandle sp)
        {
            if (_androidContext == null) return; // if no RenderContext is available return - otherwise memory read error

            var program = ((ShaderHandle)sp).Handle;

            // wait for all threads to be finished
            GL.Finish();
            GL.Flush();

            // cleanup
            GL.DeleteShader(program);
            GL.DeleteProgram(program);
        }

        /// <summary>
        /// Sets the shader program onto the GL render context.
        /// </summary>
        /// <param name="program">The shader program.</param>
        public void SetShader(IShaderHandle program)
        {
            _textureCountPerShader = 0;
            _shaderParam2TexUnit.Clear();

            GL.UseProgram(((ShaderHandle)program).Handle);
        }

        /// <summary>
        /// Sets the line width when drawing a mesh with primitive mode line
        /// </summary>
        /// <param name="width">The width of the line.</param>
        public void SetLineWidth(float width)
        {
            GL.LineWidth(width);
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
            StringBuilder sbParamName = new(paramName);
            int h = GL.GetUniformLocation(((ShaderHandle)shaderProgram).Handle, sbParamName.ToString());
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
            GL.GetProgramInterface(sProg.Handle, All.ShaderStorageBlock, All.MaxNameLength, out int ssboMaxLen);
            GL.GetProgramInterface(sProg.Handle, All.ShaderStorageBlock, All.ActiveResources, out int nParams);

            for (var i = 0; i < nParams; i++)
            {
                var param = new ActiveUniform
                {
                    HasValueChanged = true
                };

                var name = new StringBuilder();
                GL.GetProgramResourceName(sProg.Handle, All.ShaderStorageBlock, i, ssboMaxLen, out _, name);
                param.Name = name.ToString();

                int h = GL.GetProgramResourceIndex(sProg.Handle, All.ShaderStorageBlock, name);
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
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public IList<IActiveUniform> GetActiveUniformsList(IShaderHandle shaderProgram)
        {
            var sProg = (ShaderHandle)shaderProgram;
            var paramList = new List<IActiveUniform>();

            GL.GetProgram(sProg.Handle, ProgramParameter.ActiveUniforms, out int nParams);

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
            Diagnostics.Warn("No support for double values - it is cast to float!");
            GL.Uniform1(((UniformHandle)param).handle, (float)val);
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
                GL.Uniform4(((UniformHandle)param).handle, val.Length, (float*)pFlt);
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

            //unsafe
            //{
            //    var mF = (float*)(&val);

            //    // Column order notation
            //    GL.UniformMatrix4(((UniformHandle)param).handle, 1, true, mF);
            //}
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
            var tmpArray = new float4[val.Length * 4];

            for (var i = 0; i < val.Length; i++)
            {
                tmpArray[i * 4] = val[i].Column1;
                tmpArray[i * 4 + 1] = val[i].Column2;
                tmpArray[i * 4 + 2] = val[i].Column3;
                tmpArray[i * 4 + 3] = val[i].Column4;
            }

            fixed (float4* pMtx = &tmpArray[0])
                GL.UniformMatrix4(((UniformHandle)param).handle, val.Length, false, (float*)pMtx);
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

        private void BindImage(TextureType texTarget, ITextureHandle texId, int texUint, All access, SizedInternalFormat format)
        {
            Diagnostics.Warn("SizedInternalFormat not supported by BindImageTexture!");
            //switch (texTarget)
            //{
            //    case TextureType.Image2D:
            //        GL.BindImageTexture(texUint, ((TextureHandle)texId).TexHandle, 0, false, 0, access, format);
            //        break;
            //    default:
            //        throw new ArgumentException($"Unknown texture target: {texTarget}.");
            //}
        }

        private void BindTextureByTarget(ITextureHandle texId, TextureType texTarget)
        {
            switch (texTarget)
            {
                case TextureType.Texture1D:
                    Diagnostics.Error("Xamarin OpenTK ES31 does not support Texture1D.");
                    break;
                case TextureType.Texture2D:
                    GL.BindTexture(TextureTarget.Texture2D, ((TextureHandle)texId).TexId);
                    break;
                case TextureType.Texture3D:
                    GL.BindTexture(TextureTarget.Texture3D, ((TextureHandle)texId).TexId);
                    break;
                case TextureType.TextureCubeMap:
                    GL.BindTexture(TextureTarget.TextureCubeMap, ((TextureHandle)texId).TexId);
                    break;
                case TextureType.ArrayTexture:
                    GL.BindTexture(TextureTarget.Texture2DArray, ((TextureHandle)texId).TexId);
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
            if (!_shaderParam2TexUnit.TryGetValue(iParam, out int texUnit))
            {
                _textureCountPerShader++;
                texUnit = _textureCountPerShader;
                _shaderParam2TexUnit[iParam] = texUnit;
            }

            GL.ActiveTexture(TextureUnit.Texture0 + texUnit);
            BindTextureByTarget(texId, texTarget);
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
                texUnit = _textureCountPerShader;
                _shaderParam2TexUnit[iParam] = texUnit;
            }

            GL.ActiveTexture(TextureUnit.Texture0 + texUnit);
            BindTextureByTarget(texId, texTarget);
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
            int[] texUnitArray = new int[texIds.Length];

            if (!_shaderParam2TexUnit.TryGetValue(iParam, out int firstTexUnit))
            {
                _textureCountPerShader++;
                firstTexUnit = _textureCountPerShader;
                _textureCountPerShader += texIds.Length;
                _shaderParam2TexUnit[iParam] = firstTexUnit;
            }

            for (int i = 0; i < texIds.Length; i++)
            {
                texUnitArray[i] = firstTexUnit + i;

                GL.ActiveTexture(TextureUnit.Texture0 + firstTexUnit + i);
                BindTextureByTarget(texIds[i], texTarget);
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

            if (!_shaderParam2TexUnit.TryGetValue(iParam, out int firstTexUnit))
            {
                _textureCountPerShader++;
                firstTexUnit = _textureCountPerShader;
                _textureCountPerShader += texIds.Length;
                _shaderParam2TexUnit[iParam] = firstTexUnit;
            }

            for (int i = 0; i < texIds.Length; i++)
            {
                texUnitArray[i] = firstTexUnit + i;

                GL.ActiveTexture(TextureUnit.Texture0 + firstTexUnit + i);
                BindTextureByTarget(texIds[i], texTarget);
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
            SetActiveAndBindImage(param, texId, texTarget, format, All.ReadWrite, out int texUnit);
            GL.Uniform1(((UniformHandle)param).handle, texUnit);
        }

        private void SetActiveAndBindImage(IUniformHandle param, ITextureHandle texId, TextureType texTarget, ImagePixelFormat format, All access, out int texUnit)
        {
            int iParam = ((UniformHandle)param).handle;
            if (!_shaderParam2TexUnit.TryGetValue(iParam, out texUnit))
            {
                _textureCountPerShader++;
                texUnit = _textureCountPerShader;
                _shaderParam2TexUnit[iParam] = texUnit;
            }

            var sizedIntFormat = GetSizedInteralFormat(format);

            GL.ActiveTexture(TextureUnit.Texture0 + texUnit);
            BindImage(texTarget, texId, texUnit, access, sizedIntFormat);
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

        #endregion Shader related Members

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
                float[] ret = new float[4];
                GL.GetFloat(GetPName.ColorClearValue, ret);
                return new float4(ret[0], ret[1], ret[2], ret[3]);
            }
            set { GL.ClearColor(value.x, value.y, value.z, value.w); }
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
                GL.GetFloat(GetPName.DepthClearValue, out float ret);
                return ret;
            }
            set { GL.ClearDepth(value); }
        }

        #endregion Clear

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
        /// Only pixels that lie within the scissor box can be modified by drawing commands.
        /// Note that the Scissor test must be enabled for this to work.
        /// </summary>
        /// <param name="x">X Coordinate of the lower left point of the scissor box.</param>
        /// <param name="y">Y Coordinate of the lower left point of the scissor box.</param>
        /// <param name="width">Width of the scissor box.</param>
        /// <param name="height">Height of the scissor box.</param>
        public void Scissor(int x, int y, int width, int height)
        {
            GL.Scissor(x, y, width, height);
        }

        /// <summary>
        /// The clipping behavior against the Z position of a vertex can be turned off by activating depth clamping.
        /// This is done with glEnable(GL_DEPTH_CLAMP). This will cause the clip-space Z to remain unclipped by the front and rear viewing volume.
        /// See: https://www.khronos.org/opengl/wiki/Vertex_Post-Processing#Depth_clamping
        /// </summary>
        public void EnableDepthClamp()
        {
            //throw new NotImplementedException("Depth clamping isn't implemented yet!");
        }

        /// <summary>
        /// Disables depths clamping. <seealso cref="EnableDepthClamp"/>
        /// </summary>
        public void DisableDepthClamp()
        {
            //throw new NotImplementedException("Depth clamping isn't implemented yet!");
        }

        /// <summary>
        /// Binds the VertexArrayObject onto the GL Render context and assigns its index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        public void SetVertexArrayObject(IMeshImp mr)
        {
            if (((MeshImp)mr).VertexArrayObject == 0)
            {
                GL.GenVertexArrays(1, out int vao);
                ((MeshImp)mr).VertexArrayObject = vao;
            }
        }

        /// <summary>
        /// Creates and binds the instance model matrices onto the GL render context and assigns an buffer object index to the passed <see cref="IInstanceDataImp" /> instance.
        /// </summary>
        /// <param name="instanceImp">The <see cref="IInstanceDataImp"/> instance.</param>
        /// <param name="instancePositions">The instance positions.</param>
        /// <param name="instanceRotations">The instance rotations.</param>
        /// <param name="instanceScales">The instance scale values.</param>
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
                modelMats[i] = float4x4.CreateTranslation(instancePositions[i]) * float4x4.CreateRotationZXY(instanceRotations[i]) * float4x4.CreateScale(instanceScales[i]);
            }

            for (var i = 0; i < modelMats.Length; i++)
            {
                posBufferData[i * 4] = modelMats[i].Column1;
                posBufferData[i * 4 + 1] = modelMats[i].Column2;
                posBufferData[i * 4 + 2] = modelMats[i].Column3;
                posBufferData[i * 4 + 3] = modelMats[i].Column4;
            }

            if (((InstanceDataImp)instanceImp).InstanceTransformBufferObject == 0)
            {
                GL.GenBuffers(1, out int instanceTransformBo);
                ((InstanceDataImp)instanceImp).InstanceTransformBufferObject = instanceTransformBo;
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((InstanceDataImp)instanceImp).InstanceTransformBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)matBytes, modelMats, BufferUsage.DynamicDraw);
            }
            else
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((InstanceDataImp)instanceImp).InstanceTransformBufferObject);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)matBytes, modelMats);
            }

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int instancedPosBytes);
            if (instancedPosBytes != matBytes)
                throw new ApplicationException(string.Format("Problem uploading normal buffer to VBO. Tried to upload {0} bytes, uploaded {1}.", instancedPosBytes, matBytes));

            GL.BindVertexArray(vao);
            GL.EnableVertexAttribArray(AttributeLocations.InstancedModelMat1);
            GL.VertexAttribPointer(AttributeLocations.InstancedModelMat1, 4, VertexAttribPointerType.Float, false, sizeOfMat, 0);
            GL.EnableVertexAttribArray(AttributeLocations.InstancedModelMat2);
            GL.VertexAttribPointer(AttributeLocations.InstancedModelMat2, 4, VertexAttribPointerType.Float, false, sizeOfMat, 1 * sizeOfFloat4);
            GL.EnableVertexAttribArray(AttributeLocations.InstancedModelMat3);
            GL.VertexAttribPointer(AttributeLocations.InstancedModelMat3, 4, VertexAttribPointerType.Float, false, sizeOfMat, 2 * sizeOfFloat4);
            GL.EnableVertexAttribArray(AttributeLocations.InstancedModelMat4);
            GL.VertexAttribPointer(AttributeLocations.InstancedModelMat4, 4, VertexAttribPointerType.Float, false, sizeOfMat, 3 * sizeOfFloat4);

            GL.VertexAttribDivisor(AttributeLocations.InstancedModelMat1, 1);
            GL.VertexAttribDivisor(AttributeLocations.InstancedModelMat2, 1);
            GL.VertexAttribDivisor(AttributeLocations.InstancedModelMat3, 1);
            GL.VertexAttribDivisor(AttributeLocations.InstancedModelMat4, 1);
        }

        /// <summary>
        /// Binds the instance colors onto the GL render context and assigns an buffer object index to the passed <see cref="IInstanceDataImp" /> instance.
        /// </summary>
        /// <param name="instanceImp">The <see cref="IInstanceDataImp"/> instance.</param>
        /// <param name="instanceColors">The instance colors.</param>
        public void SetInstanceColor(IInstanceDataImp instanceImp, uint[] instanceColors)
        {
            if (instanceColors == null)
                return;

            var vao = ((InstanceDataImp)instanceImp).VertexArrayObject;
            if (vao == 0)
            {
                throw new ApplicationException("Create the VAO first!");
            }

            //TODO: can we use AttributeLocations.Color?
            int sizeOfCol = sizeof(uint);
            int iColorBytes = instanceColors.Length * sizeOfCol;
            int instanceColorBo = ((InstanceDataImp)instanceImp).InstanceColorBufferObject;
            if (instanceColorBo == 0)
            {
                GL.GenBuffers(1, out instanceColorBo);
                ((InstanceDataImp)instanceImp).InstanceColorBufferObject = instanceColorBo;
                GL.BindBuffer(BufferTarget.ArrayBuffer, instanceColorBo);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)iColorBytes, instanceColors, BufferUsage.StaticDraw);
            }
            else
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, instanceColorBo);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)iColorBytes, instanceColors);
            }

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int instancedColorBytes);
            if (instancedColorBytes != iColorBytes)
                throw new ApplicationException(string.Format("Problem uploading normal buffer to VBO. Tried to upload {0} bytes, uploaded {1}.", instancedColorBytes, iColorBytes));

            GL.BindVertexArray(vao);
            // set attribute pointers for matrix (4 times vec4)
            GL.EnableVertexAttribArray(AttributeLocations.InstancedColor);
            GL.VertexAttribPointer(AttributeLocations.InstancedColor, 4, VertexAttribPointerType.Float, false, sizeOfCol, 0);
            GL.VertexAttribDivisor(AttributeLocations.InstancedColor, 1);
        }


        /// <summary>
        /// Binds the vertices onto the GL render context and assigns an VertexBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="vertices">The vertices.</param>
        /// <exception cref="System.ArgumentException">Vertices must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetVertices(IMeshImp mr, ReadOnlySpan<float3> vertices)
        {
            if (vertices == null || vertices.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            int vertsBytes = vertices.Length * 3 * sizeof(float);
            if (((MeshImp)mr).VertexBufferObject == 0)
            {
                GL.GenBuffers(1, out int bufferIdx);
                ((MeshImp)mr).VertexBufferObject = bufferIdx;
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
            GL.VertexAttribPointer(AttributeLocations.VertexAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertsBytes), ref MemoryMarshal.GetReference(vertices), BufferUsage.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(string.Format(
                    "Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes));

        }

        /// <summary>
        /// Binds the tangents onto the GL render context and assigns an TangentBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="tangents">The tangents.</param>
        /// <exception cref="System.ArgumentException">Tangents must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetTangents(IMeshImp mr, ReadOnlySpan<float4> tangents)
        {
            if (tangents == null || tangents.Length == 0)
            {
                throw new ArgumentException("Tangents must not be null or empty");
            }

            int vertsBytes = tangents.Length * 4 * sizeof(float);
            if (((MeshImp)mr).TangentBufferObject == 0)
            {
                GL.GenBuffers(1, out int bufferIdx);
                ((MeshImp)mr).TangentBufferObject = bufferIdx;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).TangentBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertsBytes), ref MemoryMarshal.GetReference(tangents), BufferUsage.StaticDraw);
            GL.VertexAttribPointer(AttributeLocations.TangentAttribLocation, 4, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (tangents). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the bitangents onto the GL render context and assigns an BiTangentBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name = "bitangents">The bitangents.</param>
        /// <exception cref="System.ArgumentException">BiTangents must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetBiTangents(IMeshImp mr, ReadOnlySpan<float3> bitangents)
        {
            if (bitangents == null || bitangents.Length == 0)
            {
                throw new ArgumentException("Tangents must not be null or empty");
            }

            int vertsBytes = bitangents.Length * 3 * sizeof(float);
            if (((MeshImp)mr).BitangentBufferObject == 0)
            {
                GL.GenBuffers(1, out int bufferIdx);
                ((MeshImp)mr).BitangentBufferObject = bufferIdx;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).BitangentBufferObject);
            GL.VertexAttribPointer(AttributeLocations.BitangentAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertsBytes), ref MemoryMarshal.GetReference(bitangents), BufferUsage.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (bitangents). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the flags onto the GL render context and assigns an buffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name = "flags">The bitangents.</param>
        /// <exception cref="ArgumentException">Flags must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetFlags(IMeshImp mr, ReadOnlySpan<uint> flags)
        {
            if (flags == null || flags.Length == 0)
            {
                throw new ArgumentException("Flags must not be null or empty");
            }

            int flagsBytes = flags.Length;
            if (((MeshImp)mr).FlagsBufferObject == 0)
            {
                GL.GenBuffers(1, out int bufferIdx);
                ((MeshImp)mr).FlagsBufferObject = bufferIdx;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).FlagsBufferObject);
            GL.VertexAttribIPointer(AttributeLocations.FlagsAttribLocation, 1, VertexAttribIntegerType.UnsignedInt, 0, IntPtr.Zero);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(flagsBytes), ref MemoryMarshal.GetReference(flags), BufferUsage.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != flagsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading flags buffer to VBO (bitangents). Tried to upload {0} bytes, uploaded {1}.",
                    flagsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the normals onto the GL Render context and assigns an NormalBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="normals">The normals.</param>
        /// <exception cref="System.ArgumentException">Normals must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetNormals(IMeshImp mr, ReadOnlySpan<float3> normals)
        {
            if (normals == null || normals.Length == 0)
            {
                throw new ArgumentException("Normals must not be null or empty");
            }

            int normsBytes = normals.Length * 3 * sizeof(float);
            if (((MeshImp)mr).NormalBufferObject == 0)
            {
                GL.GenBuffers(1, out int bufferIdx);
                ((MeshImp)mr).NormalBufferObject = bufferIdx;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
            GL.VertexAttribPointer(AttributeLocations.NormalAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normsBytes), ref MemoryMarshal.GetReference(normals), BufferUsage.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != normsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.",
                    normsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the bone indices onto the GL render context and assigns an BondeIndexBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneIndices">The bone indices.</param>
        /// <exception cref="System.ArgumentException">BoneIndices must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetBoneIndices(IMeshImp mr, ReadOnlySpan<float4> boneIndices)
        {
            if (boneIndices == null || boneIndices.Length == 0)
            {
                throw new ArgumentException("BoneIndices must not be null or empty");
            }

            int indicesBytes = boneIndices.Length * 4 * sizeof(float);
            if (((MeshImp)mr).BoneIndexBufferObject == 0)
            {
                GL.GenBuffers(1, out int bufferIdx);
                ((MeshImp)mr).BoneIndexBufferObject = bufferIdx;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).BoneIndexBufferObject);
            GL.VertexAttribPointer(AttributeLocations.BoneIndexAttribLocation, 4, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(indicesBytes), ref MemoryMarshal.GetReference(boneIndices), BufferUsage.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != indicesBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading bone indices buffer to VBO (bone indices). Tried to upload {0} bytes, uploaded {1}.",
                    indicesBytes, vboBytes));
        }

        /// <summary>
        /// Binds the bone weights onto the GL render context and assigns an BondeWeightBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneWeights">The bone weights.</param>
        /// <exception cref="System.ArgumentException">BoneWeights must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetBoneWeights(IMeshImp mr, ReadOnlySpan<float4> boneWeights)
        {
            if (boneWeights == null || boneWeights.Length == 0)
            {
                throw new ArgumentException("BoneWeights must not be null or empty");
            }

            int weightsBytes = boneWeights.Length * 4 * sizeof(float);
            if (((MeshImp)mr).BoneWeightBufferObject == 0)
            {
                GL.GenBuffers(1, out int bufferIdx);
                ((MeshImp)mr).BoneWeightBufferObject = bufferIdx;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).BoneWeightBufferObject);
            GL.VertexAttribPointer(AttributeLocations.BoneWeightAttribLocation, 4, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(weightsBytes), ref MemoryMarshal.GetReference(boneWeights), BufferUsage.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != weightsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading bone weights buffer to VBO (bone weights). Tried to upload {0} bytes, uploaded {1}.",
                    weightsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the UV coordinates onto the GL render context and assigns an UVBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="uvs">The UV's.</param>
        /// <exception cref="System.ArgumentException">UVs must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetUVs(IMeshImp mr, ReadOnlySpan<float2> uvs)
        {
            if (uvs == null || uvs.Length == 0)
            {
                throw new ArgumentException("UVs must not be null or empty");
            }

            int uvsBytes = uvs.Length * 2 * sizeof(float);
            if (((MeshImp)mr).UVBufferObject == 0)
            {
                GL.GenBuffers(1, out int bufferIdx);
                ((MeshImp)mr).UVBufferObject = bufferIdx;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).UVBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uvsBytes), ref MemoryMarshal.GetReference(uvs), BufferUsage.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != uvsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading uv buffer to VBO (uvs). Tried to upload {0} bytes, uploaded {1}.",
                    uvsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the colors onto the GL render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="System.ArgumentException">colors must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetColors(IMeshImp mr, ReadOnlySpan<uint> colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("colors must not be null or empty");
            }

            int colsBytes = colors.Length * sizeof(uint);
            if (((MeshImp)mr).ColorBufferObject == 0)
            {
                GL.GenBuffers(1, out int bufferIdx);
                ((MeshImp)mr).ColorBufferObject = bufferIdx;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).ColorBufferObject);
            GL.VertexAttribPointer(AttributeLocations.ColorAttribLocation, 4, VertexAttribPointerType.UnsignedByte, true, 0, IntPtr.Zero);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colsBytes), ref MemoryMarshal.GetReference(colors), BufferUsage.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != colsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.",
                    colsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the colors onto the GL render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="System.ArgumentException">colors must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetColors1(IMeshImp mr, ReadOnlySpan<uint> colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("colors must not be null or empty");
            }

            int colsBytes = colors.Length * sizeof(uint);
            if (((MeshImp)mr).ColorBufferObject1 == 0)
            {
                GL.GenBuffers(1, out int bufferIdx);
                ((MeshImp)mr).ColorBufferObject1 = bufferIdx;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).ColorBufferObject1);
            GL.VertexAttribPointer(AttributeLocations.Color1AttribLocation, 4, VertexAttribPointerType.UnsignedByte, true, 0, IntPtr.Zero);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colsBytes), ref MemoryMarshal.GetReference(colors), BufferUsage.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != colsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.",
                    colsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the colors onto the GL render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="System.ArgumentException">colors must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetColors2(IMeshImp mr, ReadOnlySpan<uint> colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("colors must not be null or empty");
            }

            int colsBytes = colors.Length * sizeof(uint);
            if (((MeshImp)mr).ColorBufferObject2 == 0)
            {
                GL.GenBuffers(1, out int bufferIdx);
                ((MeshImp)mr).ColorBufferObject2 = bufferIdx;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).ColorBufferObject2);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colsBytes), ref MemoryMarshal.GetReference(colors), BufferUsage.StaticDraw);
            GL.VertexAttribPointer(AttributeLocations.Color2AttribLocation, 4, VertexAttribPointerType.UnsignedByte, true, 0, IntPtr.Zero);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != colsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.",
                    colsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the triangles onto the GL render context and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="triangleIndices">The triangle indices.</param>
        /// <exception cref="System.ArgumentException">triangleIndices must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetTriangles(IMeshImp mr, ReadOnlySpan<uint> triangleIndices)
        {
            if (triangleIndices == null || triangleIndices.Length == 0)
            {
                throw new ArgumentException("triangleIndices must not be null or empty");
            }
            ((MeshImp)mr).NElements = triangleIndices.Length;
            int trisBytes = triangleIndices.Length * sizeof(uint);

            if (((MeshImp)mr).ElementBufferObject == 0)
            {
                GL.GenBuffers(1, out int bufferIdx);
                ((MeshImp)mr).ElementBufferObject = bufferIdx;
            }
            // Upload the index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(trisBytes), ref MemoryMarshal.GetReference(triangleIndices),
                BufferUsage.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != trisBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (offsets). Tried to upload {0} bytes, uploaded {1}.",
                    trisBytes, vboBytes));
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveVertices(IMeshImp mr)
        {
            int bufferObj = ((MeshImp)mr).VertexBufferObject;
            GL.DeleteBuffers(1, ref bufferObj);
            ((MeshImp)mr).InvalidateVertices();
        }

        /// <summary>
        /// Deletes the buffers associated with the instance data implementation.
        /// </summary>
        /// <param name="instanceDataImp">The instance data for which to delete the GPU buffers.</param>
        public void RemoveInstanceData(IInstanceDataImp instanceDataImp)
        {
            var posBo = ((InstanceDataImp)instanceDataImp).InstanceTransformBufferObject;
            var colBo = ((InstanceDataImp)instanceDataImp).InstanceColorBufferObject;

            GL.DeleteBuffers(1, ref posBo);
            GL.DeleteBuffers(1, ref colBo);
            ((InstanceDataImp)instanceDataImp).InstanceTransformBufferObject = 0;
            ((InstanceDataImp)instanceDataImp).InstanceColorBufferObject = 0;
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveNormals(IMeshImp mr)
        {
            int bufferObj = ((MeshImp)mr).NormalBufferObject;
            GL.DeleteBuffers(1, ref bufferObj);
            ((MeshImp)mr).InvalidateNormals();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveColors(IMeshImp mr)
        {
            int bufferObj = ((MeshImp)mr).ColorBufferObject;
            GL.DeleteBuffers(1, ref bufferObj);
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
            int bufferObj = ((MeshImp)mr).UVBufferObject;
            GL.DeleteBuffers(1, ref bufferObj);
            ((MeshImp)mr).InvalidateUVs();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveTriangles(IMeshImp mr)
        {
            int bufferObj = ((MeshImp)mr).ElementBufferObject;
            GL.DeleteBuffers(1, ref bufferObj);
            ((MeshImp)mr).InvalidateTriangles();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBoneWeights(IMeshImp mr)
        {
            int bufferObj = ((MeshImp)mr).BoneWeightBufferObject;
            GL.DeleteBuffers(1, ref bufferObj);
            ((MeshImp)mr).InvalidateBoneWeights();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBoneIndices(IMeshImp mr)
        {
            int bufferObj = ((MeshImp)mr).BoneIndexBufferObject;
            GL.DeleteBuffers(1, ref bufferObj);
            ((MeshImp)mr).InvalidateBoneIndices();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveTangents(IMeshImp mr)
        {
            int bufferObj = ((MeshImp)mr).TangentBufferObject;
            GL.DeleteBuffers(1, ref bufferObj);
            ((MeshImp)mr).InvalidateTangents();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBiTangents(IMeshImp mr)
        {
            int bufferObj = ((MeshImp)mr).BitangentBufferObject;
            GL.DeleteBuffers(1, ref bufferObj);
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
            GL.MemoryBarrier(MemoryBarrierMask.AllBarrierBits);
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
        /// <param name="instanceData">Contains the buffers that need to be bound when using instanced rendering.</param>
        public void Render(IMeshImp mr, IInstanceDataImp instanceData = null)
        {
            GL.BindVertexArray(((MeshImp)mr).VertexArrayObject);

            if (((MeshImp)mr).VertexBufferObject != 0)
                GL.EnableVertexAttribArray(AttributeLocations.VertexAttribLocation);
            if (((MeshImp)mr).ColorBufferObject != 0)
                GL.EnableVertexAttribArray(AttributeLocations.ColorAttribLocation);
            if (((MeshImp)mr).ColorBufferObject1 != 0)
                GL.EnableVertexAttribArray(AttributeLocations.Color1AttribLocation);
            if (((MeshImp)mr).ColorBufferObject2 != 0)
                GL.EnableVertexAttribArray(AttributeLocations.Color2AttribLocation);
            if (((MeshImp)mr).UVBufferObject != 0)
                GL.EnableVertexAttribArray(AttributeLocations.UvAttribLocation);
            if (((MeshImp)mr).NormalBufferObject != 0)
                GL.EnableVertexAttribArray(AttributeLocations.NormalAttribLocation);
            if (((MeshImp)mr).TangentBufferObject != 0)
                GL.EnableVertexAttribArray(AttributeLocations.TangentAttribLocation);
            if (((MeshImp)mr).BitangentBufferObject != 0)
                GL.EnableVertexAttribArray(AttributeLocations.BitangentAttribLocation);
            if (((MeshImp)mr).BoneIndexBufferObject != 0)
                GL.EnableVertexAttribArray(AttributeLocations.BoneIndexAttribLocation);
            if (((MeshImp)mr).BoneWeightBufferObject != 0)
                GL.EnableVertexAttribArray(AttributeLocations.BoneWeightAttribLocation);
            if (((MeshImp)mr).FlagsBufferObject != 0)
                GL.EnableVertexAttribArray(AttributeLocations.FlagsAttribLocation);


            if (((MeshImp)mr).ElementBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);

                var oglPrimitiveType = ((MeshImp)mr).MeshType switch
                {
                    Common.PrimitiveType.Points => OpenTK.Graphics.ES31.PrimitiveType.Points,
                    Common.PrimitiveType.Lines => OpenTK.Graphics.ES31.PrimitiveType.Lines,
                    Common.PrimitiveType.LineLoop => OpenTK.Graphics.ES31.PrimitiveType.LineLoop,
                    Common.PrimitiveType.LineStrip => OpenTK.Graphics.ES31.PrimitiveType.LineStrip,
                    Common.PrimitiveType.Patches => throw new NotSupportedException("Patches aren't supported."),
                    Common.PrimitiveType.QuadStrip => throw new NotSupportedException("QuadStrips aren't supported."),
                    Common.PrimitiveType.TriangleFan => OpenTK.Graphics.ES31.PrimitiveType.TriangleFan,
                    Common.PrimitiveType.TriangleStrip => OpenTK.Graphics.ES31.PrimitiveType.TriangleStrip,
                    Common.PrimitiveType.Quads => throw new NotSupportedException("Quads aren't supported."),
                    _ => OpenTK.Graphics.ES31.PrimitiveType.Triangles,
                };

                if (instanceData != null)
                {
                    var sizeOfFloat4 = sizeof(float) * 4;
                    var sizeOfMat = sizeOfFloat4 * 4;
                    if (((InstanceDataImp)instanceData).InstanceColorBufferObject != 0)
                    {
                        GL.BindBuffer(BufferTarget.ArrayBuffer, ((InstanceDataImp)instanceData).InstanceColorBufferObject);
                        GL.EnableVertexAttribArray(AttributeLocations.InstancedColor);
                        //Needed in case of one Mesh / VBO used for more than one InstanceData / InstanceTransformBufferObject -> reset pointer
                        GL.VertexAttribPointer(AttributeLocations.InstancedColor, 4, VertexAttribPointerType.Float, false, sizeOfFloat4, 0);
                    }

                    GL.BindBuffer(BufferTarget.ArrayBuffer, ((InstanceDataImp)instanceData).InstanceTransformBufferObject);

                    // set attribute pointers for matrix (4 times vec4)
                    GL.EnableVertexAttribArray(AttributeLocations.InstancedModelMat1);
                    GL.EnableVertexAttribArray(AttributeLocations.InstancedModelMat2);
                    GL.EnableVertexAttribArray(AttributeLocations.InstancedModelMat3);
                    GL.EnableVertexAttribArray(AttributeLocations.InstancedModelMat4);

                    //Needed in case of one Mesh / VBO used for more than one InstanceData / InstanceTransformBufferObject -> reset pointer
                    GL.VertexAttribPointer(AttributeLocations.InstancedModelMat1, 4, VertexAttribPointerType.Float, false, sizeOfMat, 0);
                    GL.VertexAttribPointer(AttributeLocations.InstancedModelMat2, 4, VertexAttribPointerType.Float, false, sizeOfMat, (1 * sizeOfFloat4));
                    GL.VertexAttribPointer(AttributeLocations.InstancedModelMat3, 4, VertexAttribPointerType.Float, false, sizeOfMat, (2 * sizeOfFloat4));
                    GL.VertexAttribPointer(AttributeLocations.InstancedModelMat4, 4, VertexAttribPointerType.Float, false, sizeOfMat, (3 * sizeOfFloat4));

                    GL.DrawElementsInstanced(oglPrimitiveType, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero, instanceData.Amount);

                    GL.DisableVertexAttribArray(AttributeLocations.InstancedModelMat1);
                    GL.DisableVertexAttribArray(AttributeLocations.InstancedModelMat2);
                    GL.DisableVertexAttribArray(AttributeLocations.InstancedModelMat3);
                    GL.DisableVertexAttribArray(AttributeLocations.InstancedModelMat4);
                    GL.DisableVertexAttribArray(AttributeLocations.InstancedColor);
                }
                else
                    GL.DrawElements((BeginMode)oglPrimitiveType, ((MeshImp)mr).NElements, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }

            if (((MeshImp)mr).VertexBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.VertexAttribLocation);
            if (((MeshImp)mr).ColorBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.ColorAttribLocation);
            if (((MeshImp)mr).ColorBufferObject1 != 0)
                GL.DisableVertexAttribArray(AttributeLocations.Color1AttribLocation);
            if (((MeshImp)mr).ColorBufferObject2 != 0)
                GL.DisableVertexAttribArray(AttributeLocations.Color2AttribLocation);
            if (((MeshImp)mr).UVBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.UvAttribLocation);
            if (((MeshImp)mr).NormalBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.NormalAttribLocation);
            if (((MeshImp)mr).TangentBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.TangentAttribLocation);
            if (((MeshImp)mr).BitangentBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.BitangentAttribLocation);
            if (((MeshImp)mr).BoneIndexBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.BoneIndexAttribLocation);
            if (((MeshImp)mr).BoneWeightBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.BoneWeightAttribLocation);
            if (((MeshImp)mr).FlagsBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.FlagsAttribLocation);

            GL.BindVertexArray(0);

        }

        /// <summary>
        /// Gets the content of the buffer.
        /// </summary>
        /// <param name="quad">The Rectangle where the content is draw into.</param>
        /// <param name="texId">The texture identifier.</param>
        public void GetBufferContent(Rectangle quad, ITextureHandle texId)
        {
            GL.BindTexture(TextureTarget.Texture2D, ((TextureHandle)texId).TexId);
            GL.CopyTexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, quad.Left, quad.Top, quad.Width,
                quad.Height, 0);
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
                _ => throw new ArgumentOutOfRangeException("bo"),
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

        internal static int BlendToOgl(Blend blend, bool isForAlpha = false)
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
                Blend.BlendFactor => (int)((isForAlpha) ? BlendingFactorSrc.ConstantAlpha : BlendingFactorSrc.ConstantColor),
                Blend.InverseBlendFactor => (int)((isForAlpha) ? BlendingFactorSrc.OneMinusConstantAlpha : BlendingFactorSrc.OneMinusConstantColor),
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
                _ => throw new ArgumentOutOfRangeException("blend"),
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
        /// <exception cref="System.ArgumentOutOfRangeException">
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
            GL.Enable(EnableCap.ScissorTest);

            switch (renderState)
            {
                case RenderState.FillMode:
                    {
                        switch ((FillMode)value)
                        {
                            case FillMode.Point:
                            case FillMode.Wireframe:
                                Diagnostics.Warn("SetRenderState(RenderState.FillMode): Trying to set unsupported FillMode (PolygonMode) on Android. Not supported by OpenGL ES 3.0.");
                                break;

                            case FillMode.Solid:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(value));
                        }
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
                                throw new ArgumentOutOfRangeException("value");
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
                    float4 col = (float4)ColorUint.FromRgba(value);
                    GL.BlendColor(col.r, col.g, col.b, col.a);
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
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// pm;Value  + ((PolygonMode)pm) +  not handled
        /// or
        /// depFunc;Value  + ((All)depFunc) +  not handled
        /// or
        /// renderState
        /// </exception>
        public uint GetRenderState(RenderState renderState)
        {
            switch (renderState)
            {
                case RenderState.FillMode:
                    {
                        Diagnostics.Warn("GetRenderState(RenderState.FillMode): FillMode (PolygonMode) on Android is not supported by OpenGL ES 3.0. Returning FillMode.Solid.");
                        return (uint)FillMode.Solid;
                    }
                case RenderState.CullMode:
                    {
                        GL.GetInteger(GetPName.CullFace, out int cullFace);
                        if (cullFace == 0)
                            return (uint)Cull.None;
                        GL.GetInteger(GetPName.FrontFace, out int frontFace);
                        if (frontFace == (int)All.Cw)
                            return (uint)Cull.Clockwise;
                        return (uint)Cull.Counterclockwise;
                    }
                case RenderState.Clipping:
                    // clipping is always on in OpenGL - This state is simply ignored
                    return 1; // == true
                case RenderState.ZFunc:
                    {
                        GL.GetInteger(GetPName.DepthFunc, out int depFunc);
                        var ret = (All)depFunc switch
                        {
                            All.Never => Compare.Never,
                            All.Less => Compare.Less,
                            All.Equal => Compare.Equal,
                            All.Lequal => Compare.LessEqual,
                            All.Greater => Compare.Greater,
                            All.Notequal => Compare.NotEqual,
                            All.Gequal => Compare.GreaterEqual,
                            All.Always => Compare.Always,
                            _ => throw new ArgumentOutOfRangeException("depFunc", "Value " + ((All)depFunc) + " not handled"),
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
                        GL.GetInteger(GetPName.BlendEquation, out int rgbMode);
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
                    GL.GetInteger(GetPName.BlendColor, out col);
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
            throw new NotSupportedException("Android has no MultisampleWritableTexture support!");
        }

        /// <summary>
        /// Renders into the given texture.
        /// </summary>
        /// <param name="tex">The texture.</param>
        /// <param name="texHandle">The texture handle, associated with the given texture. Should be created by the TextureManager in the RenderContext.</param>
        public void SetRenderTarget(IWritableTexture tex, ITextureHandle texHandle)
        {
            if (tex is not WritableTexture wt)
            {
                throw new NotSupportedException("Android has no MultisampleWritableTexture support!");
            }

            SetRenderTarget(wt, texHandle);
        }

        /// <summary>
        /// Renders into the given texture.
        /// </summary>
        /// <param name="tex">The texture.</param>
        /// <param name="texHandle">The texture handle, associated with the given texture. Should be created by the TextureManager in the RenderContext.</param>
        public void SetRenderTarget(WritableTexture tex, ITextureHandle texHandle)
        {
            if (((TextureHandle)texHandle).FrameBufferHandle == -1)
            {
                GL.GenFramebuffers(1, out int fBuffer);
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fBuffer);

                GL.BindTexture(TextureTarget.Texture2D, ((TextureHandle)texHandle).TexId);

                CreateDepthRenderBuffer(tex.Width, tex.Height);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, TextureTarget.Texture2D, ((TextureHandle)texHandle).TexId, 0);
                GL.DrawBuffers(1, new DrawBufferMode[1] { DrawBufferMode.ColorAttachment0 });
            }
            else
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, ((TextureHandle)texHandle).FrameBufferHandle);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetErrorCode()}, {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");
        }

        /// <summary>
        /// Renders into the given cube map.
        /// </summary>
        /// <param name="tex">The texture.</param>
        /// <param name="texHandle">The texture handle, associated with the given cube map. Should be created by the TextureManager in the RenderContext.</param>
        public void SetRenderTarget(IWritableCubeMap tex, ITextureHandle texHandle)
        {
            if (((TextureHandle)texHandle).FrameBufferHandle == -1)
            {
                GL.GenFramebuffers(1, out int fBuffer);
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fBuffer);

                GL.BindTexture(TextureTarget.TextureCubeMap, ((TextureHandle)texHandle).TexId);

                CreateDepthRenderBuffer(tex.Width, tex.Height);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, TextureTarget.TextureCubeMap, ((TextureHandle)texHandle).TexId, 0);
                GL.DrawBuffers(1, new DrawBufferMode[1] { DrawBufferMode.ColorAttachment0 });
            }
            else
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, ((TextureHandle)texHandle).FrameBufferHandle);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetErrorCode()}, {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");
        }

        /// <summary>
        /// Renders into the given layer of the array texture.
        /// </summary>
        /// <param name="tex">The array texture.</param>
        /// <param name="layer">The layer to render to.</param>
        /// <param name="texHandle">The texture handle, associated with the given texture. Should be created by the TextureManager in the RenderContext.</param>
        public void SetRenderTarget(IWritableArrayTexture tex, int layer, ITextureHandle texHandle)
        {
            if (((TextureHandle)texHandle).FrameBufferHandle == -1)
            {
                GL.GenFramebuffers(1, out int fBuffer);
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fBuffer);

                GL.BindTexture(TextureTarget.Texture2DArray, ((TextureHandle)texHandle).TexId);

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    CreateDepthRenderBuffer(tex.Width, tex.Height);
                    GL.FramebufferTextureLayer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, ((TextureHandle)texHandle).TexId, 0, layer);
                    GL.DrawBuffers(1, new DrawBufferMode[1] { DrawBufferMode.ColorAttachment0 });
                }
                else
                {
                    GL.FramebufferTextureLayer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, ((TextureHandle)texHandle).TexId, 0, layer);
                    GL.DrawBuffers(1, new DrawBufferMode[1] { DrawBufferMode.None });
                    GL.ReadBuffer(ReadBufferMode.None);
                }
            }
            else
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, ((TextureHandle)texHandle).FrameBufferHandle);
                GL.BindTexture(TextureTarget.Texture2DArray, ((TextureHandle)texHandle).TexId);
                GL.FramebufferTextureLayer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, ((TextureHandle)texHandle).TexId, 0, layer);
            }

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetErrorCode()}, {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");
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
            }

            int gDepthRenderbufferHandle;
            if (renderTarget.DepthBufferHandle == null)
            {
                renderTarget.DepthBufferHandle = new RenderBufferHandle();
                // Create and attach depth buffer (render buffer)
                gDepthRenderbufferHandle = CreateDepthRenderBuffer((int)renderTarget.TextureResolution, (int)renderTarget.TextureResolution);
                ((RenderBufferHandle)renderTarget.DepthBufferHandle).Handle = gDepthRenderbufferHandle;
            }
            else
            {
                gDepthRenderbufferHandle = ((RenderBufferHandle)renderTarget.DepthBufferHandle).Handle;
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);
            }

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetErrorCode()}, {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");
        }

        private int CreateDepthRenderBuffer(int width, int height)
        {
            GL.Enable(EnableCap.DepthTest);

            GL.GenRenderbuffers(1, out int gDepthRenderbufferHandle);
            //((FrameBufferHandle)renderTarget.DepthBufferHandle).Handle = gDepthRenderbufferHandle;
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferInternalFormat.DepthComponent24, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferSlot.DepthAttachment, RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);
            return gDepthRenderbufferHandle;
        }

        private int CreateFrameBuffer(IRenderTarget renderTarget, ITextureHandle[] texHandles)
        {
            GL.GenBuffers(1, out int gBuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);

            int depthCnt = 0;

            var depthTexPos = (int)RenderTargetTextureTypes.Depth;

            if (!renderTarget.IsDepthOnly)
            {
                var attachments = new List<DrawBufferMode>();

                //Textures
                for (int i = 0; i < texHandles.Length; i++)
                {
                    attachments.Add(DrawBufferMode.ColorAttachment0 + i);

                    var texHandle = texHandles[i];
                    if (texHandle == null) continue;

                    if (i == depthTexPos)
                    {
                        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.DepthAttachment + (depthCnt), TextureTarget.Texture2D, ((TextureHandle)texHandle).TexId, 0);
                        depthCnt++;
                    }
                    else
                        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0 + (i - depthCnt), TextureTarget.Texture2D, ((TextureHandle)texHandle).TexId, 0);
                }
                GL.DrawBuffers(attachments.Count, attachments.ToArray());
            }
            else //If a frame-buffer only has a depth texture we don't need draw buffers
            {
                var texHandle = texHandles[depthTexPos];

                if (texHandle != null)
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.DepthAttachment, TextureTarget.Texture2D, ((TextureHandle)texHandle).TexId, 0);
                else
                    throw new NullReferenceException("Texture handle is null!");

                GL.ColorMask(false, false, false, false);
                //GL.DrawBuffers(0, new DrawBufferMode[1] { DrawBufferMode.None }); //TODO: Correct call? GL.DrawBuffer(DrawBufferMode.None) does not exist.
                GL.ReadBuffer(ReadBufferMode.None);
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
            GL.GetInteger(GetPName.FramebufferBinding, out int boundFbo);
            var rtFbo = ((FrameBufferHandle)renderTarget.GBufferHandle).Handle;

            var isCurrentFbo = true;

            if (boundFbo != rtFbo)
            {
                isCurrentFbo = false;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, rtFbo);
            }

            if (!isDepth)
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0 + attachment, TextureTarget.Texture2D, handle, 0);
            else
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.DepthAttachment, TextureTarget.Texture2D, handle, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetErrorCode()}, {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");

            if (!isCurrentFbo)
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, boundFbo);
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
                HardwareCapability.CanUseGeometryShaders => 0U,//Android uses OpenGL es, where no geometry shaders can be used.
                HardwareCapability.MaxSamples => 0U, // not supported
                _ => throw new ArgumentOutOfRangeException(nameof(capability), capability, null),
            };
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
        /// <param name="start">The start point of the DebugLine.</param>
        /// <param name="end">The endpoint of the DebugLine.</param>
        /// <param name="color">The color of the DebugLine.</param>
        public void DebugLine(float3 start, float3 end, float4 color)
        {
            throw new NotImplementedException("TODO: Implement RenderContextImp.DebugLine on Android");
            /*
            GL.Begin(All.Lines);
            GL.Vertex3(start.x, start.y, start.z);
            GL.Vertex3(end.x, end.y, end.z);
            GL.End();
            */
        }

        #endregion Rendering related Members

        #region Shader Storage Buffer

        /// <summary>
        /// Connects the given SSBO to the currently active shader program.
        /// </summary>
        /// <param name="currentProgram">The handle of the current shader program.</param>
        /// <param name="buffer">The Storage Buffer object on the CPU.</param>
        /// <param name="ssboName">The SSBO's name.</param>
        public void ConnectBufferToShaderStorage(IShaderHandle currentProgram, IStorageBuffer buffer, string ssboName)
        {
            Diagnostics.Warn("GL does not contain a definition for ShaderStorageBlockBinding!");
            //var shaderProgram = ((ShaderHandleImp)currentProgram).Handle;
            //var resInx = GL.GetProgramResourceIndex(shaderProgram, All.ShaderStorageBlock, new StringBuilder(ssboName));
            //GL.ShaderStorageBlockBinding(shaderProgram, resInx, buffer.BindingIndex);
            //GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, buffer.BindingIndex, ((StorageBufferHandle)buffer.BufferHandle).Handle);
        }

        /// <summary>
        /// Uploads the given data to the SSBO. If the buffer is not created on the GPU by no it will be.
        /// </summary>
        /// <typeparam name="T">The data type.</typeparam>
        /// <param name="storageBuffer">The Storage Buffer Object on the CPU.</param>
        /// <param name="data">The data that will be uploaded.</param>
        public void StorageBufferSetData<T>(IStorageBuffer storageBuffer, T[] data) where T : struct
        {
            Diagnostics.Warn("The GL enums BufferTarget.ShaderStorageBuffer and BufferUsage.DynamicCopy do not exist!");

            //if (storageBuffer.BufferHandle == null)
            //    storageBuffer.BufferHandle = new StorageBufferHandle();
            //var bufferHandle = (StorageBufferHandle)storageBuffer.BufferHandle;
            //int dataBytes = storageBuffer.Count * storageBuffer.Size;

            //if (bufferHandle.Handle == -1)
            //{
            //    GL.GenBuffers(1, out bufferHandle.Handle);
            //}

            //if (data == null || data.Length == 0)
            //{
            //    throw new ArgumentException("Data must not be null or empty");
            //}

            //GL.BindBuffer(BufferTarget.ShaderStorageBuffer, bufferHandle.Handle);
            //GL.BufferData(BufferTarget.ShaderStorageBuffer, dataBytes, data, BufferUsage.DynamicCopy);

            //GL.GetBufferParameter(BufferTarget.ShaderStorageBuffer, BufferParameterName.BufferSize, out int bufferBytes);
            //if (bufferBytes != dataBytes)
            //    throw new ApplicationException(string.Format("Problem uploading bone indices buffer to SSBO. Tried to upload {0} bytes, uploaded {1}.", bufferBytes, dataBytes));

            //GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        }

        /// <summary>
        /// Deletes the shader storage buffer on the GPU.
        /// </summary>
        /// <param name="storageBufferHandle">The buffer object.</param>
        public void DeleteStorageBuffer(IBufferHandle storageBufferHandle)
        {
            GL.DeleteBuffers(1, ref ((StorageBufferHandle)storageBufferHandle).Handle);
        }
        #endregion

        #region Picking related Members

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

            var err = GL.GetErrorCode();
            if (err != ErrorCode.NoError)
            {
                throw new Exception($"ReadPixel failed with error code {err}!");
            }

            return data;
        }

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
    }
    #endregion
}