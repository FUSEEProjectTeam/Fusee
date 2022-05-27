using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Imp.Shared;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Silk.NET.OpenGL;
using Silk.NET.Core;
using Silk.NET.OpenGL.Extensions.ARB;

namespace Fusee.Engine.Imp.Graphics.SilkDesktop
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

        private GLEnum _blendEquationAlpha;
        private GLEnum _blendEquationRgb;
        private GLEnum _blendSrcRgb;
        private GLEnum _blendDstRgb;
        private GLEnum _blendSrcAlpha;
        private GLEnum _blendDstAlpha;

        private bool _isCullEnabled;
        private bool _isPtRenderingEnabled;
        private bool _isLineSmoothEnabled;

#if DEBUG
        private static DebugProc _openGlDebugDelegate;
#endif

        private static GL GL;

        private IRenderCanvasImp canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContextImp"/> class.
        /// </summary>
        /// <param name="renderCanvas">The render canvas interface.</param>
        public RenderContextImp(IRenderCanvasImp renderCanvas)
        {
            _textureCountPerShader = 0;
            _shaderParam2TexUnit = new Dictionary<int, int>();
            canvas = renderCanvas;

            ((RenderCanvasImp)canvas).Init += (s, e) =>
            {
                GL = ((RenderCanvasImp)canvas)._gameWindow.GL;
                Init();
            };
        }

        public void Init()
        {
            

#if DEBUG
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);

            _openGlDebugDelegate = new DebugProc(OpenGLDebugCallback);

            GL.DebugMessageCallback(_openGlDebugDelegate, IntPtr.Zero);
            GL.DebugMessageControl(GLEnum.DontCare, GLEnum.DontCare, GLEnum.DebugSeverityNotification, 0, 0, false);
#endif

            // Due to the right-handed nature of OpenGL and the left-handed design of FUSEE
            // the meaning of what's Front and Back of a face simply flips.
            // TODO - implement this in render states!!!
            GL.CullFace(CullFaceMode.Back);

            //Needed for rendering more than one viewport.
            GL.Enable(EnableCap.ScissorTest);

            GL.GetInteger(GetPName.BlendSrcAlpha, out int blendSrcAlpha);
            GL.GetInteger(GetPName.BlendDstAlpha, out int blendDstAlpha);
            GL.GetInteger(GetPName.BlendDstRgb, out int blendDstRgb);
            GL.GetInteger(GetPName.BlendSrcRgb, out int blendSrcRgb);
            GL.GetInteger(GetPName.BlendEquationAlpha, out int blendEqA);
            GL.GetInteger(GetPName.BlendEquationRgb, out int blendEqRgb);

            _blendDstRgb = (GLEnum)blendDstRgb;
            _blendSrcRgb = (GLEnum)blendSrcRgb;
            _blendSrcAlpha = (GLEnum)blendSrcAlpha;
            _blendDstAlpha = (GLEnum)blendDstAlpha;
            _blendEquationAlpha = (GLEnum)blendEqA;
            _blendEquationRgb = (GLEnum)blendEqRgb;

            //Diagnostics.Debug(GL.GetString(StringName.Vendor) + " - " + GL.GetString(StringName.Renderer) + " - " + GL.GetString(StringName.Version));
#if DEBUG
            var numExtensions = GL.GetInteger(GLEnum.NumExtensions);
            var extensions = new string[numExtensions];

            for (int i = 0; i < numExtensions; i++)
            {
                //extensions[i] = GL.GetString(StringNameIndexed.Extensions, i);
            }

            Diagnostics.Verbose(string.Join(';', extensions));
#endif
        }

#if DEBUG
        private static void OpenGLDebugCallback(GLEnum source, GLEnum type, int id, GLEnum severity, int length, nint message, nint userParam)
        {
            Diagnostics.Debug($"{System.Runtime.InteropServices.Marshal.PtrToStringAnsi(message, length)}\n\tid:{id} severity:{severity} type:{type} source:{source}\n");
        }
#endif

        #region Image data related Members

        private Silk.NET.OpenGL.TextureCompareMode GetTexComapreMode(Common.TextureCompareMode compareMode)
        {
            return compareMode switch
            {
                Common.TextureCompareMode.None => Silk.NET.OpenGL.TextureCompareMode.None,
                Common.TextureCompareMode.CompareRefToTexture => Silk.NET.OpenGL.TextureCompareMode.CompareRefToTexture,
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

        private Silk.NET.OpenGL.TextureWrapMode GetWrapMode(Common.TextureWrapMode wrapMode)
        {
            return wrapMode switch
            {
                Common.TextureWrapMode.MirroredRepeat => Silk.NET.OpenGL.TextureWrapMode.MirroredRepeat,
                Common.TextureWrapMode.ClampToEdge => Silk.NET.OpenGL.TextureWrapMode.ClampToEdge,
                Common.TextureWrapMode.ClampToBorder => Silk.NET.OpenGL.TextureWrapMode.ClampToBorder,
                _ => Silk.NET.OpenGL.TextureWrapMode.Repeat,
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
                _ => throw new ArgumentOutOfRangeException("SizedInternalFormat not supported. Try to use a format with r,g,b and a components."),
            };
        }

        private TexturePixelInfo GetTexturePixelInfo(ImagePixelFormat pixelFormat)
        {
            GLEnum internalFormat;
            PixelFormat format;
            PixelType pxType;

            //The wrong row alignment will lead to malformed textures.
            //See https://www.khronos.org/opengl/wiki/Common_Mistakes#Texture_upload_and_pixel_reads
            //and https://www.khronos.org/opengl/wiki/Pixel_Transfer#Pixel_layout
            int rowAlignment = 4;

            switch (pixelFormat.ColorFormat)
            {
                case ColorFormat.RGBA:
                    internalFormat = GLEnum.Rgba;
                    format = PixelFormat.Rgba;
                    pxType = PixelType.UnsignedByte;
                    break;

                case ColorFormat.RGB:
                    internalFormat = GLEnum.Rgb;
                    format = PixelFormat.Rgb;
                    pxType = PixelType.UnsignedByte;
                    rowAlignment = 1;
                    break;

                // TODO: Handle Alpha-only / Intensity-only and AlphaIntensity correctly.
                case ColorFormat.Intensity:
                    internalFormat = GLEnum.R8;
                    format = PixelFormat.Red;
                    pxType = PixelType.UnsignedByte;
                    rowAlignment = 1;
                    break;

                case ColorFormat.Depth24:
                    internalFormat = GLEnum.DepthComponent24;
                    format = PixelFormat.DepthComponent;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.Depth16:
                    internalFormat = GLEnum.DepthComponent16;
                    format = PixelFormat.DepthComponent;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.uiRgb8:
                    internalFormat = GLEnum.Rgba8ui;
                    format = PixelFormat.RgbaInteger;
                    pxType = PixelType.UnsignedByte;
                    rowAlignment = 1;
                    break;

                case ColorFormat.fRGB32:
                    internalFormat = GLEnum.Rgb32f;
                    format = PixelFormat.Rgb;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.fRGB16:
                    internalFormat = GLEnum.Rgb16f;
                    format = PixelFormat.Rgb;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.fRGBA16:
                    internalFormat = GLEnum.Rgba16f;
                    format = PixelFormat.Rgba;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.fRGBA32:
                    internalFormat = GLEnum.Rgba32f;
                    format = PixelFormat.Rgba;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.iRGBA32:
                    internalFormat = GLEnum.Rgba32i;
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
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableArrayTexture img)
        {
            uint id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;
            var glWrapMode = GetWrapMode(img.WrapMode);
            var pxInfo = GetTexturePixelInfo(img.PixelFormat);

            GL.TexImage3D((GLEnum)TextureTarget.Texture2DArray, 0, (int)pxInfo.InternalFormat, (uint)img.Width, (uint)img.Height, (uint)img.Layers, 0, (GLEnum)pxInfo.Format, (GLEnum)pxInfo.PxType, 0);


            if (img.DoGenerateMipMaps)
                GL.GenerateMipmap(GLEnum.Texture2D);

            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureCompareMode, (int)GetTexComapreMode(img.CompareMode));
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureCompareFunc, (int)GetDepthCompareFunc(img.CompareFunc));
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = (int)id };

            return texID;
        }

        /// <summary>
        /// Creates a new CubeMap and binds it to the shader.
        /// </summary>
        /// <param name="img">A given IWritableCubeMap object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableCubeMap img)
        {
            var id = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;

            var glWrapMode = GetWrapMode(img.WrapMode);
            var pxInfo = GetTexturePixelInfo(img.PixelFormat);

            for (int i = 0; i < 6; i++)
                GL.TexImage2D((GLEnum)TextureTarget.TextureCubeMapPositiveX + i, 0, (int)pxInfo.InternalFormat, (uint)img.Width, (uint)img.Height, 0, (GLEnum)pxInfo.Format, (GLEnum)pxInfo.PxType, 0);

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureCompareMode, (int)GetTexComapreMode(img.CompareMode));
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureCompareFunc, (int)GetDepthCompareFunc(img.CompareFunc));
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)glWrapMode);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = (int)id };

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ITexture object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(ITexture img)
        {
            var id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;

            var glWrapMode = GetWrapMode(img.WrapMode);

            var pxInfo = GetTexturePixelInfo(img.ImageData.PixelFormat);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, pxInfo.RowAlignment);
            GL.TexImage2D<byte>((GLEnum)TextureTarget.Texture2D, 0, (int)pxInfo.InternalFormat, (uint)img.ImageData.Width, (uint)img.ImageData.Height, 0, (GLEnum)pxInfo.Format, (GLEnum)pxInfo.PxType, img.ImageData.PixelData);

            if (img.DoGenerateMipMaps)
                GL.GenerateMipmap(GLEnum.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = (int)id };

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="tex">A given IWritableTexture object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableTexture tex)
        {
            var id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            var glMinMagFilter = GetMinMagFilter(tex.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;
            var glWrapMode = GetWrapMode(tex.WrapMode);
            var pxInfo = GetTexturePixelInfo(tex.PixelFormat);

            GL.TexImage2D((GLEnum)TextureTarget.Texture2D, 0, (int)pxInfo.InternalFormat, (uint)tex.Width, (uint)tex.Height, 0, (GLEnum)pxInfo.Format, (GLEnum)pxInfo.PxType, 0);

            if (tex.DoGenerateMipMaps)
                GL.GenerateMipmap(GLEnum.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)GetTexComapreMode(tex.CompareMode));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)GetDepthCompareFunc(tex.CompareFunc));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = (int)id };

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

            GL.PixelStore(PixelStoreParameter.PackAlignment, pxInfo.RowAlignment);
            GL.BindTexture((GLEnum)TextureTarget.Texture2D, (uint)((TextureHandle)tex).TexHandle);
            GL.TexSubImage2D<byte>((GLEnum)TextureTarget.Texture2D, 0, startX, startY, (uint)width, (uint)height, (GLEnum)format, GLEnum.UnsignedByte, bytes);

            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

        /// <summary>
        /// Sets the textures filter mode (<see cref="TextureFilterMode"/> at runtime.
        /// </summary>
        /// <param name="tex">The handle of the texture.</param>
        /// <param name="filterMode">The new filter mode.</param>
        public void SetTextureFilterMode(ITextureHandle tex, TextureFilterMode filterMode)
        {
            GL.BindTexture(TextureTarget.Texture2D, (uint)((TextureHandle)tex).TexHandle);
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
            GL.BindTexture(TextureTarget.Texture2D, (uint)((TextureHandle)tex).TexHandle);
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
            GL.DeleteFramebuffer((uint)((FrameBufferHandle)bh).Handle);
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to a render-buffer object.
        /// </summary>
        /// <param name="bh">The platform dependent abstraction of the gpu buffer handle.</param>
        public void DeleteRenderBuffer(IBufferHandle bh)
        {
            GL.DeleteFramebuffer((uint)((RenderBufferHandle)bh).Handle);
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
                GL.DeleteFramebuffer((uint)texHandle.FrameBufferHandle);
            }

            if (texHandle.DepthRenderBufferHandle != -1)
            {
                GL.DeleteRenderbuffer((uint)texHandle.DepthRenderBufferHandle);
            }

            if (texHandle.TexHandle != -1)
            {
                GL.DeleteTexture((uint)texHandle.TexHandle);
                _textureCountPerShader--;
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
            uint computeObject = 0;
            if (!string.IsNullOrEmpty(cs))
            {
                computeObject = GL.CreateShader(ShaderType.ComputeShader);

                GL.ShaderSource(computeObject, cs);
                GL.CompileShader(computeObject);
                GL.GetShaderInfoLog(computeObject, out info);
                GL.GetShader(computeObject, GLEnum.CompileStatus, out statusCode);
            }

            if (statusCode != 1)
                throw new ApplicationException(info);

            uint program = GL.CreateProgram();

            GL.AttachShader(program, computeObject);
            GL.LinkProgram(program); //Must be called AFTER BindAttribLocation
            GL.DetachShader(program, computeObject);
            GL.DeleteShader(computeObject);

            return new ShaderHandleImp { Handle = (int)program };
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
            uint vertexObject = GL.CreateShader(ShaderType.VertexShader);
            uint fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(vertexObject, vs);
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, out string info);
            GL.GetShader(vertexObject, GLEnum.CompileStatus, out int statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            // Compile geometry shader
            uint geometryObject = 0;
            if (!string.IsNullOrEmpty(gs))
            {
                geometryObject = GL.CreateShader(ShaderType.GeometryShader);

                GL.ShaderSource(geometryObject, gs);
                GL.CompileShader(geometryObject);
                GL.GetShaderInfoLog(geometryObject, out info);
                GL.GetShader(geometryObject, GLEnum.CompileStatus, out statusCode);
            }

            if (statusCode != 1)
                throw new ApplicationException(info);

            // Compile pixel shader
            GL.ShaderSource(fragmentObject, ps);
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(fragmentObject, out info);
            GL.GetShader(fragmentObject, GLEnum.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            uint program = GL.CreateProgram();
            GL.AttachShader(program, fragmentObject);

            if (!string.IsNullOrEmpty(gs))
                GL.AttachShader(program, geometryObject);

            GL.AttachShader(program, vertexObject);

            // enable GLSL (ES) shaders to use fuVertex, fuColor and fuNormal attributes
            GL.BindAttribLocation(program, (uint)AttributeLocations.VertexAttribLocation, UniformNameDeclarations.Vertex);
            GL.BindAttribLocation(program, (uint)AttributeLocations.ColorAttribLocation, UniformNameDeclarations.VertexColor);
            GL.BindAttribLocation(program, (uint)AttributeLocations.Color1AttribLocation, UniformNameDeclarations.VertexColor1);
            GL.BindAttribLocation(program, (uint)AttributeLocations.Color2AttribLocation, UniformNameDeclarations.VertexColor2);
            GL.BindAttribLocation(program, (uint)AttributeLocations.UvAttribLocation, UniformNameDeclarations.TextureCoordinates);
            GL.BindAttribLocation(program, (uint)AttributeLocations.NormalAttribLocation, UniformNameDeclarations.Normal);
            GL.BindAttribLocation(program, (uint)AttributeLocations.TangentAttribLocation, UniformNameDeclarations.Tangent);
            GL.BindAttribLocation(program, (uint)AttributeLocations.BoneIndexAttribLocation, UniformNameDeclarations.BoneIndex);
            GL.BindAttribLocation(program, (uint)AttributeLocations.BoneWeightAttribLocation, UniformNameDeclarations.BoneWeight);
            GL.BindAttribLocation(program, (uint)AttributeLocations.BitangentAttribLocation, UniformNameDeclarations.Bitangent);
            GL.BindAttribLocation(program, (uint)AttributeLocations.FuseePlatformIdLocation, UniformNameDeclarations.FuseePlatformId);

            GL.LinkProgram(program); //Must be called AFTER BindAttribLocation

            GL.DetachShader(program, fragmentObject);
            GL.DetachShader(program, vertexObject);
            GL.DeleteShader(fragmentObject);
            GL.DeleteShader(vertexObject);

            return new ShaderHandleImp { Handle = (int)program };
        }

        /// <inheritdoc />
        /// <summary>
        /// Removes shader from the GPU
        /// </summary>
        /// <param name="sp"></param>
        public void RemoveShader(IShaderHandle sp)
        {
            var program = ((ShaderHandleImp)sp).Handle;

            // wait for all threads to be finished
            GL.Finish();
            GL.Flush();

            GL.DeleteProgram((uint)program);
        }


        /// <summary>
        /// Sets the shader program onto the GL Render context.
        /// </summary>
        /// <param name="program">The shader program.</param>
        public void SetShader(IShaderHandle program)
        {
            _textureCountPerShader = 0;
            _shaderParam2TexUnit.Clear();

            GL.UseProgram((uint)((ShaderHandleImp)program).Handle);
        }

        /// <summary>
        /// Gets the shader parameter.
        /// The Shader parameter is used to bind values inside of shader programs that run on the graphics card.
        /// Do not use this function in frequent updates as it transfers information from graphics card to the cpu which takes time.
        /// </summary>
        /// <param name="shaderProgram">The shader program.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>The Shader parameter is returned if the name is found, otherwise null.</returns>
        public IShaderParam GetShaderUniformParam(IShaderHandle shaderProgram, string paramName)
        {
            int h = GL.GetUniformLocation((uint)((ShaderHandleImp)shaderProgram).Handle, paramName);
            return (h == -1) ? null : new ShaderParam { handle = h };
        }

        /// <summary>
        /// Gets the float parameter value inside a shader program by using a <see cref="IShaderParam" /> as search reference.
        /// Do not use this function in frequent updates as it transfers information from graphics card to the cpu which takes time.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>A float number (default is 0).</returns>
        public float GetParamValue(IShaderHandle program, IShaderParam param)
        {
            GL.GetUniform((uint)((ShaderHandleImp)program).Handle, ((ShaderParam)param).handle, out float f);
            return f;
        }

        /// <summary>
        /// Returns a List of type <see cref="ShaderParamInfo"/> for all ShaderStorageBlocks
        /// </summary>
        /// <param name="shaderProgram">The shader program to query.</param>
        public IList<ShaderParamInfo> GetShaderStorageBufferList(IShaderHandle shaderProgram)
        {
            var paramList = new List<ShaderParamInfo>();
            var sProg = (ShaderHandleImp)shaderProgram;
            GL.GetProgramInterface((uint)sProg.Handle, GLEnum.ShaderStorageBlock, GLEnum.MaxNameLength, out int ssboMaxLen);
            GL.GetProgramInterface((uint)sProg.Handle, GLEnum.ShaderStorageBlock, GLEnum.ActiveResources, out int nParams);

            for (var i = 0; i < nParams; i++)
            {
                var paramInfo = new ShaderParamInfo();
                GL.GetProgramResourceName((uint)sProg.Handle, GLEnum.ShaderStorageBlock, (uint)i, (uint)ssboMaxLen, out _, out string name);
                paramInfo.Name = name;

                uint h = GL.GetProgramResourceIndex((uint)sProg.Handle, GLEnum.ShaderStorageBlock, name);
                paramInfo.Handle = (h == -1) ? null : new ShaderParam { handle = (int)h };
                paramList.Add(paramInfo);
            }

            return paramList;
        }

        /// <summary>
        /// Gets the shader parameter list of a specific <see cref="IShaderHandle" />. 
        /// </summary>
        /// <param name="shaderProgram">The shader program.</param>
        /// <returns>All Shader parameters of a shader program are returned.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IList<ShaderParamInfo> GetActiveUniformsList(IShaderHandle shaderProgram)
        {
            var sProg = (ShaderHandleImp)shaderProgram;
            var paramList = new List<ShaderParamInfo>();

            GL.GetProgram((uint)sProg.Handle, GLEnum.ActiveUniforms, out int nParams);

            for (var i = 0; i < nParams; i++)
            {
                var paramInfo = new ShaderParamInfo();
                paramInfo.Name = GL.GetActiveUniform((uint)sProg.Handle, (uint)i, out paramInfo.Size, out UniformType uType);
                paramInfo.Handle = GetShaderUniformParam(sProg, paramInfo.Name);

                //Diagnostics.Log($"Active Uniforms: {paramInfo.Name}");

                paramInfo.Type = uType switch
                {
                    UniformType.Int => typeof(int),
                    UniformType.Bool => typeof(bool),
                    UniformType.Float => typeof(float),
                    UniformType.Double => typeof(double),
                    UniformType.IntVec2 => typeof(float2),
                    UniformType.FloatVec2 => typeof(float2),
                    UniformType.FloatVec3 => typeof(float3),
                    UniformType.FloatVec4 => typeof(float4),
                    UniformType.FloatMat4 => typeof(float4x4),
                    UniformType.Sampler2D or UniformType.UnsignedIntSampler2D or UniformType.IntSampler2D or UniformType.Sampler2DShadow /*or UniformType.Image2D*/ => typeof(ITextureBase),
                    UniformType.SamplerCube or UniformType.SamplerCubeShadow => typeof(IWritableCubeMap),
                    UniformType.Sampler2DArray or UniformType.Sampler2DArrayShadow => typeof(IWritableArrayTexture),
                    _ => throw new ArgumentOutOfRangeException($"ActiveUniformType {uType} unknown."),
                };
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
        public void SetShaderParam(IShaderParam param, float val)
        {
            GL.Uniform1(((ShaderParam)param).handle, val);
        }

        /// <summary>
        /// Sets a double shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, double val)
        {
            GL.Uniform1(((ShaderParam)param).handle, val);
        }

        /// <summary>
        /// Sets a <see cref="float2" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float2 val)
        {
            GL.Uniform2(((ShaderParam)param).handle, val.x, val.y);
        }

        /// <summary>
        /// Sets a <see cref="float2" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IShaderParam param, float2[] val)
        {
            fixed (float2* pFlt = &val[0])
                GL.Uniform2(((ShaderParam)param).handle, (uint)val.Length, (float*)pFlt);
        }

        /// <summary>
        /// Sets a <see cref="float3" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float3 val)
        {
            GL.Uniform3(((ShaderParam)param).handle, val.x, val.y, val.z);
        }

        /// <summary>
        ///     Sets a <see cref="float3" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IShaderParam param, float3[] val)
        {
            fixed (float3* pFlt = &val[0])
                GL.Uniform3(((ShaderParam)param).handle, (uint)val.Length, (float*)pFlt);
        }

        /// <summary>
        /// Sets a <see cref="float4" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float4 val)
        {
            GL.Uniform4(((ShaderParam)param).handle, val.x, val.y, val.z, val.w);
        }

        /// <summary>
        /// Sets a <see cref="float4x4" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
            unsafe
            {
                var mF = (float*)(&val);
                // Row order notation
                // GL.UniformMatrix4(((ShaderParam) param).handle, 1, false, mF);

                // Column order notation
                GL.UniformMatrix4(((ShaderParam)param).handle, 1, true, mF);
            }
        }

        /// <summary>
        ///     Sets a <see cref="float4" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IShaderParam param, float4[] val)
        {
            fixed (float4* pFlt = &val[0])
                GL.Uniform4(((ShaderParam)param).handle, (uint)val.Length, (float*)pFlt);
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
                tmpArray[i * 4] = val[i].Column1;
                tmpArray[i * 4 + 1] = val[i].Column2;
                tmpArray[i * 4 + 2] = val[i].Column3;
                tmpArray[i * 4 + 3] = val[i].Column4;
            }

            fixed (float4* pMtx = &tmpArray[0])
                GL.UniformMatrix4(((ShaderParam)param).handle, (uint)val.Length, false, (float*)pMtx);
        }

        /// <summary>
        /// Sets a int shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, int val)
        {
            GL.Uniform1(((ShaderParam)param).handle, val);
        }

        private void BindImage(TextureType texTarget, ITextureHandle texId, int texUint, GLEnum access, SizedInternalFormat format)
        {
            switch (texTarget)
            {
                case TextureType.Image2D:
                    GL.BindImageTexture((uint)texUint, (uint)((TextureHandle)texId).TexHandle, 0, false, 0, access, (GLEnum)format);
                    break;
                default:
                    throw new ArgumentException($"Unknown texture target: {texTarget}.");
            }
        }

        private void BindTextureByTarget(ITextureHandle texId, TextureType texTarget)
        {
            switch (texTarget)
            {
                case TextureType.Texture1D:
                    GL.BindTexture(TextureTarget.Texture1D, (uint)((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.Texture2D:
                    GL.BindTexture(TextureTarget.Texture2D, (uint)((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.Texture3D:
                    GL.BindTexture(TextureTarget.Texture3D, (uint)((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.TextureCubeMap:
                    GL.BindTexture(TextureTarget.TextureCubeMap, (uint)((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.ArrayTexture:
                    GL.BindTexture(TextureTarget.Texture2DArray, (uint)((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.Image2D:
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
        public void SetActiveAndBindTexture(IShaderParam param, ITextureHandle texId, TextureType texTarget)
        {
            int iParam = ((ShaderParam)param).handle;
            if (!_shaderParam2TexUnit.TryGetValue(iParam, out int texUnit))
            {
                _textureCountPerShader++;
                texUnit = _textureCountPerShader;
                _shaderParam2TexUnit[iParam] = texUnit;
            }

            GL.ActiveTexture(TextureUnit.Texture0 + texUnit);
            BindTextureByTarget(texId, texTarget);
        }

        private void SetActiveAndBindImage(IShaderParam param, ITextureHandle texId, TextureType texTarget, ImagePixelFormat format, GLEnum access, out int texUnit)
        {
            int iParam = ((ShaderParam)param).handle;
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
        /// Sets a texture active and binds it.
        /// </summary>
        /// <param name="param">The shader parameter, associated with this texture.</param>
        /// <param name="texId">The texture handle.</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        /// <param name="texUnit">The texture unit.</param>
        public void SetActiveAndBindTexture(IShaderParam param, ITextureHandle texId, TextureType texTarget, out int texUnit)
        {
            int iParam = ((ShaderParam)param).handle;
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
        public void SetActiveAndBindTextureArray(IShaderParam param, ITextureHandle[] texIds, TextureType texTarget)
        {
            int iParam = ((ShaderParam)param).handle;
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
        public void SetActiveAndBindTextureArray(IShaderParam param, ITextureHandle[] texIds, TextureType texTarget, out int[] texUnitArray)
        {
            int iParam = ((ShaderParam)param).handle;
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
        public void SetShaderParamImage(IShaderParam param, ITextureHandle texId, TextureType texTarget, ImagePixelFormat format)
        {
            SetActiveAndBindImage(param, texId, texTarget, format, GLEnum.ReadWrite, out int texUnit);
            GL.Uniform1(((ShaderParam)param).handle, texUnit);
        }

        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texId">An ITextureHandle probably returned from CreateTexture method</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        public void SetShaderParamTexture(IShaderParam param, ITextureHandle texId, TextureType texTarget)
        {
            SetActiveAndBindTexture(param, texId, texTarget, out int texUnit);
            GL.Uniform1(((ShaderParam)param).handle, texUnit);
        }

        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texIds">An array of ITextureHandles probably returned from CreateTexture method</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        public unsafe void SetShaderParamTextureArray(IShaderParam param, ITextureHandle[] texIds, TextureType texTarget)
        {
            SetActiveAndBindTextureArray(param, texIds, texTarget, out int[] texUnitArray);

            fixed (int* pFlt = &texUnitArray[0])
                GL.Uniform1(((ShaderParam)param).handle, (uint)texUnitArray.Length, pFlt);
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
                var ret = new Span<float>(new float[4]);
                GL.GetFloat(GetPName.ColorClearValue, ret);
                return new float4(ret[0], ret[1], ret[2], ret[3]);
            }
            set => GL.ClearColor(value.x, value.y, value.z, value.w);
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
        /// Disables depths clamping. <seealso cref="EnableDepthClamp"/>
        /// </summary>
        public void DisableDepthClamp()
        {
            GL.Disable(EnableCap.DepthClamp);
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

            int vertsBytes = attributes.Length * 3 * sizeof(float);
            GL.GenBuffers(1, out uint handle);

            GL.BindBuffer(GLEnum.ArrayBuffer, (uint)handle);

            GL.BufferData(GLEnum.ArrayBuffer, (nuint)vertsBytes, new ReadOnlySpan<float3>(attributes), GLEnum.StaticDraw);
            GL.GetBufferParameter(GLEnum.ArrayBuffer, GLEnum.BufferSize, out int vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(string.Format(
                    "Problem uploading attribute buffer to VBO ('{2}'). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes, attributeName));

            return new AttributeImp { AttributeBufferObject = (int)handle };
        }

        /// <summary>
        /// Remove an attribute buffer previously created with <see cref="CreateAttributeBuffer"/> and release all associated resources
        /// allocated on the GPU.
        /// </summary>
        /// <param name="attribHandle">The attribute handle</param>
        public void DeleteAttributeBuffer(IAttribImp attribHandle)
        {
            if (attribHandle != null)
            {
                int handle = ((AttributeImp)attribHandle).AttributeBufferObject;
                if (handle != 0)
                {
                    GL.DeleteBuffer((uint)handle);
                    ((AttributeImp)attribHandle).AttributeBufferObject = 0;
                }
            }
        }

        /// <summary>
        /// Binds the VertexArrayObject onto the GL Render context and assigns its index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        public void SetVertexArrayObject(IMeshImp mr)
        {
            if (((MeshImp)mr).VertexArrayObject == 0)
                ((MeshImp)mr).VertexArrayObject = (int)GL.GenVertexArray();

            GL.BindVertexArray((uint)((MeshImp)mr).VertexArrayObject);
        }

        /// <summary>
        /// Binds the vertices onto the GL Render context and assigns an VertexBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="vertices">The vertices.</param>
        /// <exception cref="ArgumentException">Vertices must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetVertices(IMeshImp mr, float3[] vertices)
        {
            if (vertices == null || vertices.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            int vertsBytes = vertices.Length * 3 * sizeof(float);
            if (((MeshImp)mr).VertexBufferObject == 0)
            {
                GL.GenBuffers(1, out uint bufferObj);
                ((MeshImp)mr).VertexBufferObject = (int)bufferObj;
            }

            GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).VertexBufferObject);
            GL.BufferData(GLEnum.ArrayBuffer, (nuint)(vertsBytes), new ReadOnlySpan<float3>(vertices), GLEnum.StaticDraw);
            GL.GetBufferParameter(GLEnum.ArrayBuffer, GLEnum.BufferSize, out int vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(string.Format("Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.", vertsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the tangents onto the GL Render context and assigns an TangentBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int tangentBytes = tangents.Length * 4 * sizeof(float);
            if (((MeshImp)mr).TangentBufferObject == 0)
            {
                GL.GenBuffers(1, out uint bufferObj);
                ((MeshImp)mr).TangentBufferObject = (int)bufferObj;
            }

            GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).TangentBufferObject);
            GL.BufferData(GLEnum.ArrayBuffer, (nuint)(tangentBytes), new ReadOnlySpan<float4>(tangents), GLEnum.StaticDraw);
            GL.GetBufferParameter(GLEnum.ArrayBuffer, GLEnum.BufferSize, out int vboBytes);
            if (vboBytes != tangentBytes)
                throw new ApplicationException(string.Format("Problem uploading vertex buffer to VBO (tangents). Tried to upload {0} bytes, uploaded {1}.", tangentBytes, vboBytes));
        }

        /// <summary>
        /// Binds the bitangents onto the GL Render context and assigns an BiTangentBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="bitangents">The BiTangents.</param>
        /// <exception cref="ArgumentException">BiTangents must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetBiTangents(IMeshImp mr, float3[] bitangents)
        {
            if (bitangents == null || bitangents.Length == 0)
            {
                throw new ArgumentException("BiTangents must not be null or empty");
            }

            int bitangentBytes = bitangents.Length * 3 * sizeof(float);
            if (((MeshImp)mr).BitangentBufferObject == 0)
            {
                GL.GenBuffers(1, out uint bufferObj);
                ((MeshImp)mr).BitangentBufferObject = (int)bufferObj;
            }

            GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).BitangentBufferObject);
            GL.BufferData(GLEnum.ArrayBuffer, (nuint)(bitangentBytes), new ReadOnlySpan<float3>(bitangents), GLEnum.StaticDraw);
            GL.GetBufferParameter(GLEnum.ArrayBuffer, GLEnum.BufferSize, out int vboBytes);
            if (vboBytes != bitangentBytes)
                throw new ApplicationException(string.Format("Problem uploading vertex buffer to VBO (bitangents). Tried to upload {0} bytes, uploaded {1}.", bitangentBytes, vboBytes));
        }

        /// <summary>
        /// Binds the normals onto the GL Render context and assigns an NormalBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int normsBytes = normals.Length * 3 * sizeof(float);
            if (((MeshImp)mr).NormalBufferObject == 0)
            {
                GL.GenBuffers(1, out uint bufferObj);
                ((MeshImp)mr).NormalBufferObject = (int)bufferObj;
            }

            GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).NormalBufferObject);
            GL.BufferData(GLEnum.ArrayBuffer, (nuint)(normsBytes), new ReadOnlySpan<float3>(normals), GLEnum.StaticDraw);
            GL.GetBufferParameter(GLEnum.ArrayBuffer, GLEnum.BufferSize, out int vboBytes);
            if (vboBytes != normsBytes)
                throw new ApplicationException(string.Format("Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.", normsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the bone indices onto the GL Render context and assigns an BondeIndexBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneIndices">The bone indices.</param>
        /// <exception cref="ArgumentException">BoneIndices must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetBoneIndices(IMeshImp mr, float4[] boneIndices)
        {
            if (boneIndices == null || boneIndices.Length == 0)
            {
                throw new ArgumentException("BoneIndices must not be null or empty");
            }

            int indicesBytes = boneIndices.Length * 4 * sizeof(float);
            if (((MeshImp)mr).BoneIndexBufferObject == 0)
            {
                GL.GenBuffers(1, out uint bufferObj);
                ((MeshImp)mr).BoneIndexBufferObject = (int)bufferObj;
            }

            GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).BoneIndexBufferObject);
            GL.BufferData(GLEnum.ArrayBuffer, (nuint)(indicesBytes), new ReadOnlySpan<float4>(boneIndices), GLEnum.StaticDraw);
            GL.GetBufferParameter(GLEnum.ArrayBuffer, GLEnum.BufferSize, out int vboBytes);
            if (vboBytes != indicesBytes)
                throw new ApplicationException(string.Format("Problem uploading bone indices buffer to VBO (bone indices). Tried to upload {0} bytes, uploaded {1}.", indicesBytes, vboBytes));
        }

        /// <summary>
        /// Binds the bone weights onto the GL Render context and assigns an BondeWeightBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneWeights">The bone weights.</param>
        /// <exception cref="ArgumentException">BoneWeights must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetBoneWeights(IMeshImp mr, float4[] boneWeights)
        {
            if (boneWeights == null || boneWeights.Length == 0)
            {
                throw new ArgumentException("BoneWeights must not be null or empty");
            }

            int weightsBytes = boneWeights.Length * 4 * sizeof(float);
            if (((MeshImp)mr).BoneWeightBufferObject == 0)
            {
                GL.GenBuffers(1, out uint bufferObj);
                ((MeshImp)mr).BoneWeightBufferObject = (int)bufferObj;
            }

            GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).BoneWeightBufferObject);
            GL.BufferData(GLEnum.ArrayBuffer, (nuint)(weightsBytes), new ReadOnlySpan<float4>(boneWeights), GLEnum.StaticDraw);
            GL.GetBufferParameter(GLEnum.ArrayBuffer, GLEnum.BufferSize, out int vboBytes);
            if (vboBytes != weightsBytes)
                throw new ApplicationException(string.Format("Problem uploading bone weights buffer to VBO (bone weights). Tried to upload {0} bytes, uploaded {1}.", weightsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the UV coordinates onto the GL Render context and assigns an UVBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int uvsBytes = uvs.Length * 2 * sizeof(float);
            if (((MeshImp)mr).UVBufferObject == 0)
            {
                GL.GenBuffers(1, out uint bufferObj);
                ((MeshImp)mr).UVBufferObject = (int)bufferObj;
            }

            GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).UVBufferObject);
            GL.BufferData(GLEnum.ArrayBuffer, (nuint)(uvsBytes), new ReadOnlySpan<float2>(uvs), GLEnum.StaticDraw);
            GL.GetBufferParameter(GLEnum.ArrayBuffer, GLEnum.BufferSize, out int vboBytes);
            if (vboBytes != uvsBytes)
                throw new ApplicationException(string.Format("Problem uploading uv buffer to VBO (uvs). Tried to upload {0} bytes, uploaded {1}.", uvsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the colors onto the GL Render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int colsBytes = colors.Length * sizeof(uint);
            if (((MeshImp)mr).ColorBufferObject == 0)
            {
                GL.GenBuffers(1, out uint bufferObj);
                ((MeshImp)mr).ColorBufferObject = (int)bufferObj;
            }

            GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).ColorBufferObject);
            GL.BufferData(GLEnum.ArrayBuffer, (nuint)(colsBytes), new ReadOnlySpan<uint>(colors), GLEnum.StaticDraw);
            GL.GetBufferParameter(GLEnum.ArrayBuffer, GLEnum.BufferSize, out int vboBytes);
            if (vboBytes != colsBytes)
                throw new ApplicationException(string.Format("Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.", colsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the colors onto the GL Render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="ArgumentException">colors must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetColors1(IMeshImp mr, uint[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("colors must not be null or empty");
            }

            int colsBytes = colors.Length * sizeof(uint);
            if (((MeshImp)mr).ColorBufferObject1 == 0)
            {
                GL.GenBuffers(1, out uint bufferObj);
                ((MeshImp)mr).ColorBufferObject1 = (int)bufferObj;
            }

            GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).ColorBufferObject1);
            GL.BufferData(GLEnum.ArrayBuffer, (nuint)(colsBytes), new ReadOnlySpan<uint>(colors), GLEnum.StaticDraw);
            GL.GetBufferParameter(GLEnum.ArrayBuffer, GLEnum.BufferSize, out int vboBytes);
            if (vboBytes != colsBytes)
                throw new ApplicationException(string.Format("Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.", colsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the colors onto the GL Render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="ArgumentException">colors must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetColors2(IMeshImp mr, uint[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("colors must not be null or empty");
            }

            int colsBytes = colors.Length * sizeof(uint);
            if (((MeshImp)mr).ColorBufferObject2 == 0)
            {
                GL.GenBuffers(1, out uint bufferObj);
                ((MeshImp)mr).ColorBufferObject2 = (int)bufferObj;
            }

            GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).ColorBufferObject2);
            GL.BufferData(GLEnum.ArrayBuffer, (nuint)(colsBytes), new ReadOnlySpan<uint>(colors), GLEnum.StaticDraw);
            GL.GetBufferParameter(GLEnum.ArrayBuffer, GLEnum.BufferSize, out int vboBytes);
            if (vboBytes != colsBytes)
                throw new ApplicationException(string.Format("Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.", colsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the triangles onto the GL Render context and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
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
            int trisBytes = triangleIndices.Length * sizeof(short);

            if (((MeshImp)mr).ElementBufferObject == 0)
            {
                GL.GenBuffers(1, out uint bufferObj);
                ((MeshImp)mr).ElementBufferObject = (int)bufferObj;
            }
            // Upload the index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
            GL.BindBuffer(GLEnum.ElementArrayBuffer, (uint)((MeshImp)mr).ElementBufferObject);
            GL.BufferData(GLEnum.ElementArrayBuffer, (nuint)(trisBytes), new ReadOnlySpan<ushort>(triangleIndices), GLEnum.StaticDraw);
            GL.GetBufferParameter(GLEnum.ElementArrayBuffer, GLEnum.BufferSize, out int vboBytes);
            if (vboBytes != trisBytes)
                throw new ApplicationException(string.Format("Problem uploading vertex buffer to VBO (offsets). Tried to upload {0} bytes, uploaded {1}.", trisBytes, vboBytes));
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveVertices(IMeshImp mr)
        {
            GL.DeleteVertexArray((uint)((MeshImp)mr).VertexArrayObject);
            GL.DeleteBuffer((uint)((MeshImp)mr).VertexBufferObject);
            ((MeshImp)mr).InvalidateVertices();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveNormals(IMeshImp mr)
        {
            GL.DeleteBuffer((uint)((MeshImp)mr).NormalBufferObject);
            ((MeshImp)mr).InvalidateNormals();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveColors(IMeshImp mr)
        {
            GL.DeleteBuffer((uint)((MeshImp)mr).ColorBufferObject);
            ((MeshImp)mr).InvalidateColors();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveColors1(IMeshImp mr)
        {
            var bufferObj = (uint)((MeshImp)mr).ColorBufferObject1;
            GL.DeleteBuffers(1, bufferObj);
            ((MeshImp)mr).InvalidateColors1();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveColors2(IMeshImp mr)
        {
            var bufferObj = (uint)((MeshImp)mr).ColorBufferObject2;
            GL.DeleteBuffers(1, bufferObj);
            ((MeshImp)mr).InvalidateColors2();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveUVs(IMeshImp mr)
        {
            GL.DeleteBuffer((uint)((MeshImp)mr).UVBufferObject);
            ((MeshImp)mr).InvalidateUVs();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveTriangles(IMeshImp mr)
        {
            GL.DeleteBuffer((uint)((MeshImp)mr).ElementBufferObject);
            ((MeshImp)mr).InvalidateTriangles();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBoneWeights(IMeshImp mr)
        {
            GL.DeleteBuffer((uint)((MeshImp)mr).BoneWeightBufferObject);
            ((MeshImp)mr).InvalidateBoneWeights();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBoneIndices(IMeshImp mr)
        {
            GL.DeleteBuffer((uint)((MeshImp)mr).BoneIndexBufferObject);
            ((MeshImp)mr).InvalidateBoneIndices();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveTangents(IMeshImp mr)
        {
            GL.DeleteBuffer((uint)((MeshImp)mr).TangentBufferObject);
            ((MeshImp)mr).InvalidateTangents();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBiTangents(IMeshImp mr)
        {
            GL.DeleteBuffer((uint)((MeshImp)mr).BitangentBufferObject);
            ((MeshImp)mr).InvalidateBiTangents();
        }

        /// <summary>
        /// Defines a barrier ordering memory transactions. At the moment it will insert all supported barriers.
        /// </summary>
        public void MemoryBarrier()
        {
            unchecked // (uint) as -1
            {
                GL.MemoryBarrier((uint)GLEnum.AllBarrierBits);
            }
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
            GL.DispatchCompute((uint)threadGroupsX, (uint)threadGroupsY, (uint)threadGroupsZ);
        }

        /// <summary>
        /// Renders the specified <see cref="IMeshImp" />.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        public unsafe void Render(IMeshImp mr)
        {
            GL.BindVertexArray((uint)((MeshImp)mr).VertexArrayObject);

            if (((MeshImp)mr).VertexBufferObject != 0)
            {
                GL.EnableVertexAttribArray((uint)AttributeLocations.VertexAttribLocation);
                GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).VertexBufferObject);
                GL.VertexAttribPointer((uint)AttributeLocations.VertexAttribLocation, 3, GLEnum.Float, false, 0, IntPtr.Zero.ToPointer());
            }
            if (((MeshImp)mr).ColorBufferObject != 0)
            {
                GL.EnableVertexAttribArray((uint)AttributeLocations.ColorAttribLocation);
                GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).ColorBufferObject);
                GL.VertexAttribPointer((uint)AttributeLocations.ColorAttribLocation, 4, GLEnum.UnsignedByte, true, 0, IntPtr.Zero.ToPointer());
            }
            if (((MeshImp)mr).ColorBufferObject1 != 0)
            {
                GL.EnableVertexAttribArray((uint)AttributeLocations.Color1AttribLocation);
                GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).ColorBufferObject1);
                GL.VertexAttribPointer((uint)AttributeLocations.Color1AttribLocation, 4, GLEnum.UnsignedByte, true, 0, IntPtr.Zero.ToPointer());
            }
            if (((MeshImp)mr).ColorBufferObject2 != 0)
            {
                GL.EnableVertexAttribArray((uint)AttributeLocations.Color2AttribLocation);
                GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).ColorBufferObject2);
                GL.VertexAttribPointer((uint)AttributeLocations.Color2AttribLocation, 4, GLEnum.UnsignedByte, true, 0, IntPtr.Zero.ToPointer());
            }
            if (((MeshImp)mr).UVBufferObject != 0)
            {
                GL.EnableVertexAttribArray((uint)AttributeLocations.UvAttribLocation);
                GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).UVBufferObject);
                GL.VertexAttribPointer((uint)AttributeLocations.UvAttribLocation, 2, GLEnum.Float, false, 0, IntPtr.Zero.ToPointer());
            }
            if (((MeshImp)mr).NormalBufferObject != 0)
            {
                GL.EnableVertexAttribArray((uint)AttributeLocations.NormalAttribLocation);
                GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).NormalBufferObject);
                GL.VertexAttribPointer((uint)AttributeLocations.NormalAttribLocation, 3, GLEnum.Float, false, 0, IntPtr.Zero.ToPointer());
            }
            if (((MeshImp)mr).TangentBufferObject != 0)
            {
                GL.EnableVertexAttribArray((uint)AttributeLocations.TangentAttribLocation);
                GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).TangentBufferObject);
                GL.VertexAttribPointer((uint)AttributeLocations.TangentAttribLocation, 3, GLEnum.Float, false, 0, IntPtr.Zero.ToPointer());
            }
            if (((MeshImp)mr).BitangentBufferObject != 0)
            {
                GL.EnableVertexAttribArray((uint)AttributeLocations.BitangentAttribLocation);
                GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).BitangentBufferObject);
                GL.VertexAttribPointer((uint)AttributeLocations.BitangentAttribLocation, 3, GLEnum.Float, false, 0, IntPtr.Zero.ToPointer());
            }
            if (((MeshImp)mr).BoneIndexBufferObject != 0)
            {
                GL.EnableVertexAttribArray((uint)AttributeLocations.BoneIndexAttribLocation);
                GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).BoneIndexBufferObject);
                GL.VertexAttribPointer((uint)AttributeLocations.BoneIndexAttribLocation, 4, GLEnum.Float, false, 0, IntPtr.Zero.ToPointer());
            }
            if (((MeshImp)mr).BoneWeightBufferObject != 0)
            {
                GL.EnableVertexAttribArray((uint)AttributeLocations.BoneWeightAttribLocation);
                GL.BindBuffer(GLEnum.ArrayBuffer, (uint)((MeshImp)mr).BoneWeightBufferObject);
                GL.VertexAttribPointer((uint)AttributeLocations.BoneWeightAttribLocation, 4, GLEnum.Float, false, 0, IntPtr.Zero.ToPointer());
            }
            if (((MeshImp)mr).ElementBufferObject != 0)
            {
                GL.BindBuffer(GLEnum.ElementArrayBuffer, (uint)((MeshImp)mr).ElementBufferObject);

                switch (((MeshImp)mr).MeshType)
                {
                    case Common.PrimitiveType.Triangles:
                    default:
                        GL.DrawElements(GLEnum.Triangles, (uint)((MeshImp)mr).NElements, GLEnum.UnsignedShort, IntPtr.Zero.ToPointer());
                        break;
                    case Common.PrimitiveType.Points:
                        // enable gl_PointSize to set the point size
                        if (!_isPtRenderingEnabled)
                        {
                            _isPtRenderingEnabled = true;
                            GL.Enable(EnableCap.ProgramPointSize);
                            //GL.Enable(EnableCap.PointSprite);
                            GL.Enable(GLEnum.VertexProgramPointSize);
                        }
                        GL.DrawElements(GLEnum.Points, (uint)((MeshImp)mr).NElements, GLEnum.UnsignedShort, IntPtr.Zero.ToPointer());
                        break;
                    case Common.PrimitiveType.Lines:
                        if (!_isLineSmoothEnabled)
                        {
                            GL.Enable(EnableCap.LineSmooth);
                            _isLineSmoothEnabled = true;
                        }
                        GL.DrawElements(GLEnum.Lines, (uint)((MeshImp)mr).NElements, GLEnum.UnsignedShort, IntPtr.Zero.ToPointer());
                        break;
                    case Common.PrimitiveType.LineLoop:
                        if (!_isLineSmoothEnabled)
                        {
                            GL.Enable(EnableCap.LineSmooth);
                            _isLineSmoothEnabled = true;
                        }
                        GL.DrawElements(GLEnum.LineLoop, (uint)((MeshImp)mr).NElements, GLEnum.UnsignedShort, IntPtr.Zero.ToPointer());
                        break;
                    case Common.PrimitiveType.LineStrip:
                        if (!_isLineSmoothEnabled)
                        {
                            GL.Enable(EnableCap.LineSmooth);
                            _isLineSmoothEnabled = true;
                        }
                        GL.DrawElements(GLEnum.LineStrip, (uint)((MeshImp)mr).NElements, GLEnum.UnsignedShort, IntPtr.Zero.ToPointer());
                        break;
                    case Common.PrimitiveType.Patches:
                        GL.DrawElements(GLEnum.Patches, (uint)((MeshImp)mr).NElements, GLEnum.UnsignedShort, IntPtr.Zero.ToPointer());
                        break;
                    case Common.PrimitiveType.QuadStrip:
                        //GL.DrawElements(GLEnum.QuadStrip, (uint)((MeshImp)mr).NElements, GLEnum.UnsignedShort, IntPtr.Zero.ToPointer());
                        break;
                    case Common.PrimitiveType.TriangleFan:
                        GL.DrawElements(GLEnum.TriangleFan, (uint)((MeshImp)mr).NElements, GLEnum.UnsignedShort, IntPtr.Zero.ToPointer());
                        break;
                    case Common.PrimitiveType.TriangleStrip:
                        GL.DrawElements(GLEnum.TriangleStrip, (uint)((MeshImp)mr).NElements, GLEnum.UnsignedShort, IntPtr.Zero.ToPointer());
                        break;
                }
            }

            if (((MeshImp)mr).VertexBufferObject != 0)
                GL.DisableVertexAttribArray((uint)AttributeLocations.VertexAttribLocation);
            if (((MeshImp)mr).ColorBufferObject != 0)
                GL.DisableVertexAttribArray((uint)AttributeLocations.ColorAttribLocation);
            if (((MeshImp)mr).NormalBufferObject != 0)
                GL.DisableVertexAttribArray((uint)AttributeLocations.NormalAttribLocation);
            if (((MeshImp)mr).UVBufferObject != 0)
                GL.DisableVertexAttribArray((uint)AttributeLocations.UvAttribLocation);
            if (((MeshImp)mr).TangentBufferObject != 0)
                GL.DisableVertexAttribArray((uint)AttributeLocations.TangentAttribLocation);
            if (((MeshImp)mr).BitangentBufferObject != 0)
                GL.DisableVertexAttribArray((uint)AttributeLocations.TangentAttribLocation);

            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Gets the content of the buffer.
        /// </summary>
        /// <param name="quad">The Rectangle where the content is draw into.</param>
        /// <param name="texId">The texture identifier.</param>
        public void GetBufferContent(Common.Rectangle quad, ITextureHandle texId)
        {
            GL.BindTexture(TextureTarget.Texture2D, (uint)((TextureHandle)texId).TexHandle);
            GL.CopyTexImage2D((GLEnum)TextureTarget.Texture2D, 0, GLEnum.Rgba, quad.Left, quad.Top, (uint)quad.Width, (uint)quad.Height, 0);
        }

        /// <summary>
        /// Creates the mesh implementation.
        /// </summary>
        /// <returns>The <see cref="IMeshImp" /> instance.</returns>
        public IMeshImp CreateMeshImp()
        {
            return new MeshImp();
        }

        internal static GLEnum BlendOperationToOgl(BlendOperation bo)
        {
            return bo switch
            {
                BlendOperation.Add => GLEnum.FuncAdd,
                BlendOperation.Subtract => GLEnum.FuncSubtract,
                BlendOperation.ReverseSubtract => GLEnum.FuncReverseSubtract,
                BlendOperation.Minimum => GLEnum.Min,
                BlendOperation.Maximum => GLEnum.Max,
                _ => throw new ArgumentOutOfRangeException($"Invalid argument: {bo}"),
            };
        }

        internal static BlendOperation BlendOperationFromOgl(GLEnum bom)
        {
            return bom switch
            {
                GLEnum.FuncAdd => BlendOperation.Add,
                GLEnum.Min => BlendOperation.Minimum,
                GLEnum.Max => BlendOperation.Maximum,
                GLEnum.FuncSubtract => BlendOperation.Subtract,
                GLEnum.FuncReverseSubtract => BlendOperation.ReverseSubtract,
                _ => throw new ArgumentOutOfRangeException($"Invalid argument: {bom}"),
            };
        }

        internal static GLEnum BlendToOgl(Blend blend, bool isForBlendFactorAlpha = false)
        {
            return blend switch
            {
                Blend.Zero => GLEnum.Zero,
                Blend.One => GLEnum.One,
                Blend.SourceColor => GLEnum.SrcColor,
                Blend.InverseSourceColor => GLEnum.OneMinusSrcColor,
                Blend.SourceAlpha => GLEnum.SrcAlpha,
                Blend.InverseSourceAlpha => GLEnum.OneMinusSrcAlpha,
                Blend.DestinationAlpha => GLEnum.DstAlpha,
                Blend.InverseDestinationAlpha => GLEnum.OneMinusDstAlpha,
                Blend.DestinationColor => GLEnum.DstColor,
                Blend.InverseDestinationColor => GLEnum.OneMinusDstColor,
                Blend.BlendFactor => ((isForBlendFactorAlpha) ? GLEnum.ConstantAlpha : GLEnum.ConstantColor),
                Blend.InverseBlendFactor => ((isForBlendFactorAlpha) ? GLEnum.OneMinusConstantAlpha : GLEnum.OneMinusConstantColor),
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
                (int)GLEnum.Zero => Blend.Zero,
                (int)GLEnum.One => Blend.One,
                (int)GLEnum.SrcColor => Blend.SourceColor,
                (int)GLEnum.OneMinusSrcColor => Blend.InverseSourceColor,
                (int)GLEnum.SrcAlpha => Blend.SourceAlpha,
                (int)GLEnum.OneMinusSrcAlpha => Blend.InverseSourceAlpha,
                (int)GLEnum.DstAlpha => Blend.DestinationAlpha,
                (int)GLEnum.OneMinusDstAlpha => Blend.InverseDestinationAlpha,
                (int)GLEnum.DstColor => Blend.DestinationColor,
                (int)GLEnum.OneMinusDstColor => Blend.InverseDestinationColor,
                (int)GLEnum.ConstantAlpha or (int)GLEnum.ConstantColor => Blend.BlendFactor,
                (int)GLEnum.OneMinusConstantAlpha or (int)GLEnum.OneMinusConstantColor => Blend.InverseBlendFactor,
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
                        var pm = (FillMode)value switch
                        {
                            FillMode.Point => PolygonMode.Point,
                            FillMode.Wireframe => PolygonMode.Line,
                            FillMode.Solid => PolygonMode.Fill,
                            _ => throw new ArgumentOutOfRangeException(nameof(value)),
                        };
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
                                GL.FrontFace(FrontFaceDirection.CW);
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
                        GL.BlendEquationSeparate(_blendEquationRgb, (GLEnum)_blendEquationAlpha);
                    }
                    break;
                case RenderState.SourceBlend:
                    {
                        _blendSrcRgb = BlendToOgl((Blend)value);
                        GL.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.DestinationBlend:
                    {
                        _blendDstRgb = BlendToOgl((Blend)value);
                        GL.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.SourceBlendAlpha:
                    {
                        _blendSrcAlpha = BlendToOgl((Blend)value);
                        GL.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.DestinationBlendAlpha:
                    {
                        _blendDstAlpha = BlendToOgl((Blend)value);
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
                        if (frontFace == (int)FrontFaceDirection.CW)
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
                        return (uint)BlendOperationFromOgl((GLEnum)rgbMode);
                    }
                case RenderState.BlendOperationAlpha:
                    {
                        GL.GetInteger(GetPName.BlendEquationAlpha, out int alphaMode);
                        return (uint)BlendOperationFromOgl((GLEnum)alphaMode);
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
        /// Renders into the given texture.
        /// </summary>
        /// <param name="tex">The texture.</param>
        /// <param name="texHandle">The texture handle, associated with the given texture. Should be created by the TextureManager in the RenderContext.</param>
        public void SetRenderTarget(IWritableTexture tex, ITextureHandle texHandle)
        {
            if (((TextureHandle)texHandle).FrameBufferHandle == -1)
            {
                var fBuffer = GL.GenFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = (int)fBuffer;
                GL.BindFramebuffer(GLEnum.Framebuffer, fBuffer);

                GL.BindTexture(TextureTarget.Texture2D, (uint)((TextureHandle)texHandle).TexHandle);

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    CreateDepthRenderBuffer(tex.Width, tex.Height);
                    GL.FramebufferTexture(GLEnum.Framebuffer, GLEnum.ColorAttachment0, (uint)((TextureHandle)texHandle).TexHandle, 0);
                    GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
                }
                else
                {
                    GL.FramebufferTexture(GLEnum.Framebuffer, GLEnum.DepthAttachment, (uint)((TextureHandle)texHandle).TexHandle, 0);
                    GL.DrawBuffer(DrawBufferMode.None);
                    GL.ReadBuffer(ReadBufferMode.None);
                }
            }
            else
                GL.BindFramebuffer(GLEnum.Framebuffer, (uint)((TextureHandle)texHandle).FrameBufferHandle);

            if (GL.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckFramebufferStatus(GLEnum.Framebuffer)}");

            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
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
                var fBuffer = GL.GenFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = (int)fBuffer;
                GL.BindFramebuffer(GLEnum.Framebuffer, fBuffer);

                GL.BindTexture(TextureTarget.TextureCubeMap, (uint)((TextureHandle)texHandle).TexHandle);

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    CreateDepthRenderBuffer(tex.Width, tex.Height);
                    GL.FramebufferTexture(GLEnum.Framebuffer, GLEnum.ColorAttachment0, (uint)((TextureHandle)texHandle).TexHandle, 0);
                    GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
                }
                else
                {
                    GL.FramebufferTexture(GLEnum.Framebuffer, GLEnum.DepthAttachment, (uint)((TextureHandle)texHandle).TexHandle, 0);
                    GL.DrawBuffer(DrawBufferMode.None);
                    GL.ReadBuffer(ReadBufferMode.None);
                }
            }
            else
                GL.BindFramebuffer(GLEnum.Framebuffer, (uint)((TextureHandle)texHandle).FrameBufferHandle);

            if (GL.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckFramebufferStatus(GLEnum.Framebuffer)}");


            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
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
                var fBuffer = GL.GenFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = (int)fBuffer;
                GL.BindFramebuffer(GLEnum.Framebuffer, fBuffer);

                GL.BindTexture(TextureTarget.Texture2DArray, (uint)((TextureHandle)texHandle).TexHandle);

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    CreateDepthRenderBuffer(tex.Width, tex.Height);
                    GL.FramebufferTextureLayer(GLEnum.Framebuffer, GLEnum.ColorAttachment0, (uint)((TextureHandle)texHandle).TexHandle, 0, layer);
                    GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
                }
                else
                {
                    GL.FramebufferTextureLayer(GLEnum.Framebuffer, GLEnum.DepthAttachment, (uint)((TextureHandle)texHandle).TexHandle, 0, layer);
                    GL.DrawBuffer(DrawBufferMode.None);
                    GL.ReadBuffer(ReadBufferMode.None);
                }
            }
            else
            {
                GL.BindFramebuffer(GLEnum.Framebuffer, (uint)((TextureHandle)texHandle).FrameBufferHandle);
                GL.BindTexture(TextureTarget.Texture2DArray, (uint)((TextureHandle)texHandle).TexHandle);
                GL.FramebufferTextureLayer(GLEnum.Framebuffer, GLEnum.DepthAttachment, (uint)((TextureHandle)texHandle).TexHandle, 0, layer);
            }

            if (GL.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckFramebufferStatus(GLEnum.Framebuffer)}");

            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
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
                GL.BindFramebuffer(GLEnum.Framebuffer, 0);
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
                GL.BindFramebuffer(GLEnum.Framebuffer, (uint)gBuffer);
            }

            if (renderTarget.RenderTextures[(int)RenderTargetTextureTypes.Depth] == null && !renderTarget.IsDepthOnly)
            {
                int gDepthRenderbufferHandle;
                if (renderTarget.DepthBufferHandle == null)
                {
                    renderTarget.DepthBufferHandle = new RenderBufferHandle();
                    // Create and attach depth buffer (renderbuffer)
                    gDepthRenderbufferHandle = CreateDepthRenderBuffer((int)renderTarget.TextureResolution, (int)renderTarget.TextureResolution);
                    ((RenderBufferHandle)renderTarget.DepthBufferHandle).Handle = gDepthRenderbufferHandle;
                }
                else
                {
                    gDepthRenderbufferHandle = ((RenderBufferHandle)renderTarget.DepthBufferHandle).Handle;
                    GL.BindRenderbuffer(GLEnum.Renderbuffer, (uint)gDepthRenderbufferHandle);
                }
            }

            if (GL.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete)
            {
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckFramebufferStatus(GLEnum.Framebuffer)}");
            }

            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
        }

        private int CreateDepthRenderBuffer(int width, int height)
        {
            GL.Enable(EnableCap.DepthTest);

            GL.GenRenderbuffers(1, out uint gDepthRenderbufferHandle);
            //((FrameBufferHandle)renderTarget.DepthBufferHandle).Handle = gDepthRenderbufferHandle;
            GL.BindRenderbuffer(GLEnum.Renderbuffer, gDepthRenderbufferHandle);
            GL.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.DepthComponent24, (uint)width, (uint)height);
            GL.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthAttachment, GLEnum.Renderbuffer, gDepthRenderbufferHandle);
            return (int)gDepthRenderbufferHandle;
        }

        private int CreateFrameBuffer(IRenderTarget renderTarget, ITextureHandle[] texHandles)
        {
            var gBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(GLEnum.Framebuffer, gBuffer);

            int depthCnt = 0;

            var depthTexPos = (int)RenderTargetTextureTypes.Depth;

            if (!renderTarget.IsDepthOnly)
            {
                var attachments = new List<GLEnum>();

                //Textures
                for (int i = 0; i < texHandles.Length; i++)
                {
                    attachments.Add(GLEnum.ColorAttachment0 + i);

                    var texHandle = texHandles[i];
                    if (texHandle == null) continue;

                    if (i == depthTexPos)
                    {
                        GL.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.DepthAttachment + (depthCnt), TextureTarget.Texture2D, (uint)((TextureHandle)texHandle).TexHandle, 0);
                        depthCnt++;
                    }
                    else
                        GL.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0 + (i - depthCnt), TextureTarget.Texture2D, (uint)((TextureHandle)texHandle).TexHandle, 0);
                }
                GL.DrawBuffers((uint)attachments.Count, attachments.ToArray());
            }
            else //If a frame-buffer only has a depth texture we don't need draw buffers
            {
                var texHandle = texHandles[depthTexPos];

                if (texHandle != null)
                    GL.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.DepthAttachment, TextureTarget.Texture2D, (uint)((TextureHandle)texHandle).TexHandle, 0);
                else
                    throw new NullReferenceException("Texture handle is null!");

                GL.DrawBuffer(DrawBufferMode.None);
                GL.ReadBuffer(ReadBufferMode.None);
            }

            return (int)gBuffer;
        }

        /// <summary>
        /// Detaches a texture from the frame buffer object, associated with the given render target.
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="attachment">Number of the fbo attachment. For example: attachment = 1 will detach the texture currently associated with <see cref="GLEnum.ColorAttachment1"/>.</param>
        /// <param name="isDepthTex">Determines if the texture is a depth texture. In this case the texture currently associated with <see cref="GLEnum.DepthAttachment"/> will be detached.</param>       
        public void DetachTextureFromFbo(IRenderTarget renderTarget, bool isDepthTex, int attachment = 0)
        {
            ChangeFramebufferTexture2D(renderTarget, attachment, 0, isDepthTex);
        }


        /// <summary>
        /// Attaches a texture to the frame buffer object, associated with the given render target.
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="attachment">Number of the fbo attachment. For example: attachment = 1 will attach the texture to <see cref="GLEnum.ColorAttachment1"/>.</param>
        /// <param name="isDepthTex">Determines if the texture is a depth texture. In this case the texture is attached to <see cref="GLEnum.DepthAttachment"/>.</param>        
        /// <param name="texHandle">The gpu handle of the texture.</param>
        public void AttacheTextureToFbo(IRenderTarget renderTarget, bool isDepthTex, ITextureHandle texHandle, int attachment = 0)
        {
            ChangeFramebufferTexture2D(renderTarget, attachment, ((TextureHandle)texHandle).TexHandle, isDepthTex);
        }

        private void ChangeFramebufferTexture2D(IRenderTarget renderTarget, int attachment, int handle, bool isDepth)
        {
            var boundFbo = GL.GetInteger(GLEnum.FramebufferBinding);
            var rtFbo = ((FrameBufferHandle)renderTarget.GBufferHandle).Handle;

            var isCurrentFbo = true;

            if (boundFbo != rtFbo)
            {
                isCurrentFbo = false;
                GL.BindFramebuffer(GLEnum.Framebuffer, (uint)rtFbo);
            }

            if (!isDepth)
                GL.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0 + attachment, TextureTarget.Texture2D, (uint)handle, 0);
            else
                GL.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.DepthAttachment, TextureTarget.Texture2D, (uint)handle, 0);

            if (GL.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckFramebufferStatus(GLEnum.Framebuffer)}");

            if (!isCurrentFbo)
                GL.BindFramebuffer(GLEnum.Framebuffer, (uint)boundFbo);
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
            GL.Scissor(x, y, (uint)width, (uint)height);
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
            GL.Viewport(x, y, (uint)width, (uint)height);
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
                HardwareCapability.CanRenderDeferred => 1U, //!GL.GetString(StringName.Extensions).Contains("EXT_framebuffer_object") ? 0U : 1U,
                HardwareCapability.CanUseGeometryShaders => 1U,
                _ => throw new ArgumentOutOfRangeException(nameof(capability), capability, null),
            };
        }

        /// <summary> 
        /// Returns a human readable description of the underlying graphics hardware. This implementation reports GL_VENDOR, GL_RENDERER, GL_VERSION and GL_EXTENSIONS.
        /// </summary> 
        /// <returns></returns> 
        public string GetHardwareDescription()
        {
            return "";
            //return "Vendor: " + GL.GetString(StringName.Vendor) + "\nRenderer: " + GL.GetString(StringName.Renderer) + "\nVersion: " + GL.GetString(StringName.Version) + "\nExtensions: " + GL.GetString(StringName.Extensions);
        }

        /// <summary>
        /// Draws a Debug Line in 3D Space by using a start and end point (float3).
        /// </summary>
        /// <param name="start">The starting point of the DebugLine.</param>
        /// <param name="end">The endpoint of the DebugLine.</param>
        /// <param name="color">The color of the DebugLine.</param>
        public void DebugLine(float3 start, float3 end, float4 color)
        {
            //GL.Begin(GLEnum.Lines);
            //GL.Color4(color.x, color.y, color.z, color.w);
            //GL.Vertex3(start.x, start.y, start.z);
            //GL.Color4(color.x, color.y, color.z, color.w);
            //GL.Vertex3(end.x, end.y, end.z);
            //GL.End();
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
            var shaderProgram = (uint)((ShaderHandleImp)currentProgram).Handle;
            var resInx = GL.GetProgramResourceIndex(shaderProgram, ProgramInterface.ShaderStorageBlock, ssboName);
            GL.ShaderStorageBlockBinding(shaderProgram, resInx, (uint)buffer.BindingIndex);
            GL.BindBufferBase(GLEnum.ShaderStorageBuffer, (uint)buffer.BindingIndex, (uint)((StorageBufferHandle)buffer.BufferHandle).Handle);
        }

        /// <summary>
        /// Uploads the given data to the SSBO. If the buffer is not created on the GPU by no it will be.
        /// </summary>
        /// <typeparam name="T">The data type.</typeparam>
        /// <param name="storageBuffer">The Storage Buffer Object on the CPU.</param>
        /// <param name="data">The data that will be uploaded.</param>
        public unsafe void StorageBufferSetData<T>(IStorageBuffer storageBuffer, T[] data) where T : struct
        {
            throw new NotImplementedException();

            if (storageBuffer.BufferHandle == null)
                storageBuffer.BufferHandle = new StorageBufferHandle();
            var bufferHandle = (StorageBufferHandle)storageBuffer.BufferHandle;
            int dataBytes = storageBuffer.Count * storageBuffer.Size;

            //1. Generate Buffer and or set the data
            if (bufferHandle.Handle == -1)
            {
                bufferHandle.Handle = (int)GL.GenBuffer();
            }

            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Data must not be null or empty");
            }


            GL.BindBuffer(GLEnum.ShaderStorageBuffer, (uint)bufferHandle.Handle);
            if (data != null && data.GetType().IsValueType)
            {
                // GL.BufferData(GLEnum.ShaderStorageBuffer, (nuint)dataBytes, in data, GLEnum.DynamicCopy);
            }

            GL.GetBufferParameter(GLEnum.ShaderStorageBuffer, GLEnum.BufferSize, out int bufferBytes);
            if (bufferBytes != dataBytes)
                throw new ApplicationException(string.Format("Problem uploading bone indices buffer to SSBO. Tried to upload {0} bytes, uploaded {1}.", bufferBytes, dataBytes));

            GL.BindBuffer(GLEnum.ShaderStorageBuffer, 0);
        }

        /// <summary>
        /// Deletes the shader storage buffer on the GPU.
        /// </summary>
        /// <param name="storageBufferHandle">The buffer object.</param>
        public void DeleteStorageBuffer(IBufferHandle storageBufferHandle)
        {
            GL.DeleteBuffer((uint)((StorageBufferHandle)storageBufferHandle).Handle);
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
            GL.ReadPixels(x, y, (uint)w, (uint)h, GLEnum.Rgb, GLEnum.UnsignedByte, new Span<byte>(image.PixelData));
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
            GL.ReadPixels(x, y, 1, 1, GLEnum.DepthComponent, GLEnum.UnsignedByte, out float depth);
            return depth;
        }



        #endregion
    }
}