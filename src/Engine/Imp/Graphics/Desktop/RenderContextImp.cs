using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Imp.Shared;
using Fusee.Math.Core;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

        private BlendEquationModeEXT _blendEquationAlpha;
        private BlendEquationModeEXT _blendEquationRgb;
        private BlendingFactor _blendSrcRgb;
        private BlendingFactor _blendDstRgb;
        private BlendingFactor _blendSrcAlpha;
        private BlendingFactor _blendDstAlpha;

        private bool _isCullEnabled;
        private bool _isPtRenderingEnabled;
        private bool _isLineSmoothEnabled;

#if DEBUG
        private static GLDebugProc _openGlDebugDelegate;
#endif

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

            _openGlDebugDelegate = new GLDebugProc(OpenGLDebugCallback);

            GL.DebugMessageCallback(_openGlDebugDelegate, IntPtr.Zero);

            unsafe
            {
                GL.DebugMessageControl(DebugSource.DontCare, DebugType.DontCare, DebugSeverity.DebugSeverityNotification, 0, (uint*)IntPtr.Zero, Convert.ToByte(false));
            }
#endif

            // Due to the right-handed nature of OpenGL and the left-handed design of FUSEE
            // the meaning of what's Front and Back of a face simply flips.
            // TODO - implement this in render states!!!
            GL.CullFace(CullFaceMode.Back);

            //Needed for rendering more than one viewport.
            GL.Enable(EnableCap.ScissorTest);

            int blendSrcAlpha = 0;
            int blendDstAlpha = 0;
            int blendDstRgb = 0;
            int blendSrcRgb = 0;
            int blendEqA = 0;
            int blendEqRgb = 0;

            GL.GetInteger(GetPName.BlendSrcAlpha, ref blendSrcAlpha);
            GL.GetInteger(GetPName.BlendDstAlpha, ref blendDstAlpha);
            GL.GetInteger(GetPName.BlendDstRgb, ref blendDstRgb);
            GL.GetInteger(GetPName.BlendSrcRgb, ref blendSrcRgb);
            GL.GetInteger(GetPName.BlendEquationAlpha, ref blendEqA);
            GL.GetInteger(GetPName.BlendEquationRgb, ref blendEqRgb);

            _blendDstRgb = (BlendingFactor)blendDstRgb;
            _blendSrcRgb = (BlendingFactor)blendSrcRgb;
            _blendSrcAlpha = (BlendingFactor)blendSrcAlpha;
            _blendDstAlpha = (BlendingFactor)blendDstAlpha;
            _blendEquationAlpha = (BlendEquationModeEXT)blendEqA;
            _blendEquationRgb = (BlendEquationModeEXT)blendEqRgb;

            Diagnostics.Debug(GL.GetString(StringName.Vendor) + " - " + GL.GetString(StringName.Renderer) + " - " + GL.GetString(StringName.Version));
#if DEBUG
            int numExtensions = 0;
            GL.GetInteger(GetPName.NumExtensions, ref numExtensions);

            // OpenTK 4.x
            //var extensions = new string[numExtensions];
            //for (int i = 0; i < numExtensions; i++)
            //{
            //    extensions[i] = GL.GetString(StringName.Extensions, i);
            //}

            var extensions = GL.GetString(StringName.Extensions);

            Diagnostics.Verbose(string.Join(';', extensions));
#endif
        }

#if DEBUG
        private static void OpenGLDebugCallback(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            Diagnostics.Debug($"{System.Runtime.InteropServices.Marshal.PtrToStringAnsi(message, length)}\n\tid:{id} severity:{severity} type:{type} source:{source}\n");
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

        private InternalFormat GetInteralFormat(ImagePixelFormat format)
        {
            return format.ColorFormat switch
            {
                ColorFormat.RGBA => InternalFormat.Rgba,
                ColorFormat.fRGBA16 => InternalFormat.Rgba16f,
                ColorFormat.fRGBA32 => InternalFormat.Rgba32f,
                ColorFormat.iRGBA32 => InternalFormat.Rgba32i,
                _ => throw new ArgumentOutOfRangeException("SizedInternalFormat not supported. Try to use a format with r,g,b and a components."),
            };
        }

        private TexturePixelInfo GetTexturePixelInfo(ITextureBase tex)
        {
            InternalFormat internalFormat;
            PixelFormat format;
            PixelType pxType;

            switch (tex.PixelFormat.ColorFormat)
            {
                case ColorFormat.RGBA:
                    internalFormat = InternalFormat.Rgba;
                    format = PixelFormat.Bgra;
                    pxType = PixelType.UnsignedByte;

                    break;
                case ColorFormat.RGB:
                    internalFormat = InternalFormat.Rgb;
                    format = PixelFormat.Bgr;
                    pxType = PixelType.UnsignedByte;

                    break;
                // TODO: Handle Alpha-only / Intensity-only and AlphaIntensity correctly.
                case ColorFormat.Intensity:
                    internalFormat = InternalFormat.R8;
                    format = PixelFormat.Red;
                    pxType = PixelType.UnsignedByte;

                    break;
                case ColorFormat.Depth24:
                    internalFormat = InternalFormat.DepthComponent24;
                    format = PixelFormat.DepthComponent;
                    pxType = PixelType.Float;

                    break;
                case ColorFormat.Depth16:
                    internalFormat = InternalFormat.DepthComponent16;
                    format = PixelFormat.DepthComponent;
                    pxType = PixelType.Float;

                    break;
                case ColorFormat.uiRgb8:
                    internalFormat = InternalFormat.Rgba8ui;
                    format = PixelFormat.RgbaInteger;
                    pxType = PixelType.UnsignedByte;

                    break;
                case ColorFormat.fRGB32:
                    internalFormat = InternalFormat.Rgb32f;
                    format = PixelFormat.Rgb;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.fRGB16:
                    internalFormat = InternalFormat.Rgb16f;
                    format = PixelFormat.Rgb;
                    pxType = PixelType.Float;
                    break;
                case ColorFormat.fRGBA16:
                    internalFormat = InternalFormat.Rgba16f;
                    format = PixelFormat.Rgba;
                    pxType = PixelType.Float;
                    break;
                case ColorFormat.fRGBA32:
                    internalFormat = InternalFormat.Rgba32f;
                    format = PixelFormat.Rgba;
                    pxType = PixelType.Float;
                    break;

                case ColorFormat.iRGBA32:
                    internalFormat = InternalFormat.Rgba32i;
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
                PxType = pxType
            };
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableArrayTexture img)
        {
            OpenTK.Graphics.TextureHandle handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2dArray, handle);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;
            var glWrapMode = GetWrapMode(img.WrapMode);
            var pxInfo = GetTexturePixelInfo(img);

            GL.TexImage3D(TextureTarget.Texture2dArray, 0, (int)pxInfo.InternalFormat, img.Width, img.Height, img.Layers, 0, pxInfo.Format, pxInfo.PxType, IntPtr.Zero);

            if (img.DoGenerateMipMaps)
                GL.GenerateMipmap(TextureTarget.Texture2d);

            GL.TexParameteri(TextureTarget.Texture2dArray, TextureParameterName.TextureCompareMode, (int)GetTexComapreMode(img.CompareMode));
            GL.TexParameteri(TextureTarget.Texture2dArray, TextureParameterName.TextureCompareFunc, (int)GetDepthCompareFunc(img.CompareFunc));
            GL.TexParameteri(TextureTarget.Texture2dArray, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameteri(TextureTarget.Texture2dArray, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameteri(TextureTarget.Texture2dArray, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameteri(TextureTarget.Texture2dArray, TextureParameterName.TextureWrapT, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = handle };

            return texID;
        }

        /// <summary>
        /// Creates a new CubeMap and binds it to the shader.
        /// </summary>
        /// <param name="img">A given IWritableCubeMap object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableCubeMap img)
        {
            OpenTK.Graphics.TextureHandle handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, handle);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;

            var glWrapMode = GetWrapMode(img.WrapMode);
            var pxInfo = GetTexturePixelInfo(img);

            for (uint i = 0; i < 6; i++)
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, (int)pxInfo.InternalFormat, img.Width, img.Height, 0, pxInfo.Format, pxInfo.PxType, IntPtr.Zero);

            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureCompareMode, (int)GetTexComapreMode(img.CompareMode));
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureCompareFunc, (int)GetDepthCompareFunc(img.CompareFunc));
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)glWrapMode);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = handle };

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ITexture object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(ITexture img)
        {
            OpenTK.Graphics.TextureHandle handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2d, handle);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;

            var glWrapMode = GetWrapMode(img.WrapMode);

            var pxInfo = GetTexturePixelInfo(img);
            unsafe
            {
                using var pxDataMem = img.PixelData.AsMemory().Pin();
                GL.TexImage2D(TextureTarget.Texture2d, 0, (int)pxInfo.InternalFormat, img.Width, img.Height, 0, pxInfo.Format, pxInfo.PxType, pxDataMem.Pointer);
            }

            if (img.DoGenerateMipMaps)
                GL.GenerateMipmap(TextureTarget.Texture2d);

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)glWrapMode);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapR, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = handle };

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given IWritableTexture object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableTexture img)
        {
            OpenTK.Graphics.TextureHandle handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2d, handle);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;
            var glWrapMode = GetWrapMode(img.WrapMode);
            var pxInfo = GetTexturePixelInfo(img);

            GL.TexImage2D(TextureTarget.Texture2d, 0, (int)pxInfo.InternalFormat, img.Width, img.Height, 0, pxInfo.Format, pxInfo.PxType, IntPtr.Zero);

            if (img.DoGenerateMipMaps)
                GL.GenerateMipmap(TextureTarget.Texture2d);

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureCompareMode, (int)GetTexComapreMode(img.CompareMode));
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureCompareFunc, (int)GetDepthCompareFunc(img.CompareFunc));
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = handle };

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
            PixelFormat format = GetTexturePixelInfo(img).Format;

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
                    System.Buffer.BlockCopy(lineBytes, 0, bytes, offset, lineBytes.Length);
                    offset += lineBytes.Length;
                }

            } while (scanlines.MoveNext());

            GL.BindTexture(TextureTarget.Texture2d, ((TextureHandle)tex).TexHandle);

            unsafe
            {
                using var byteMem = bytes.AsMemory().Pin();
                GL.TexSubImage2D(TextureTarget.Texture2d, 0, startX, startY, width, height, format, PixelType.UnsignedByte, byteMem.Pointer);
            }

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

        /// <summary>
        /// Sets the textures filter mode (<see cref="TextureFilterMode"/> at runtime.
        /// </summary>
        /// <param name="tex">The handle of the texture.</param>
        /// <param name="filterMode">The new filter mode.</param>
        public void SetTextureFilterMode(ITextureHandle tex, TextureFilterMode filterMode)
        {
            GL.BindTexture(TextureTarget.Texture2d, ((TextureHandle)tex).TexHandle);
            var glMinMagFilter = GetMinMagFilter(filterMode);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)glMinMagFilter.Item1);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)glMinMagFilter.Item2);
        }

        /// <summary>
        /// Sets the textures filter mode (<see cref="Common.TextureWrapMode"/> at runtime.
        /// </summary>
        /// <param name="tex">The handle of the texture.</param>
        ///<param name="wrapMode">The new wrap mode.</param>
        public void SetTextureWrapMode(ITextureHandle tex, Common.TextureWrapMode wrapMode)
        {
            GL.BindTexture(TextureTarget.Texture2d, ((TextureHandle)tex).TexHandle);
            var glWrapMode = GetWrapMode(wrapMode);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)glWrapMode);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapR, (int)glWrapMode);
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
        /// Free all allocated gpu memory that belong to a render buffer object.
        /// </summary>
        /// <param name="bh">The platform dependent abstraction of the gpu buffer handle.</param>
        public void DeleteRenderBuffer(IBufferHandle bh)
        {
            GL.DeleteRenderbuffer(((RenderBufferHandle)bh).Handle);
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to the given <see cref="ITextureHandle"/>.
        /// </summary>
        /// <param name="textureHandle">The <see cref="ITextureHandle"/> which gpu allocated memory will be freed.</param>
        public void RemoveTextureHandle(ITextureHandle textureHandle)
        {
            TextureHandle texHandle = (TextureHandle)textureHandle;

            if (texHandle.FrameBufferHandle.Handle != -1)
            {
                GL.DeleteFramebuffer(texHandle.FrameBufferHandle);
            }

            if (texHandle.DepthRenderBufferHandle.Handle != -1)
            {
                GL.DeleteRenderbuffer(texHandle.DepthRenderBufferHandle);
            }

            if (texHandle.TexHandle.Handle != -1)
            {
                GL.DeleteTexture(texHandle.TexHandle);
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
            ShaderHandle computeObject = new(-1);
            if (!string.IsNullOrEmpty(cs))
            {
                computeObject = GL.CreateShader(ShaderType.ComputeShader);

                GL.ShaderSource(computeObject, cs);
                GL.CompileShader(computeObject);
                GL.GetShaderInfoLog(computeObject, out info);
                GL.GetShaderi(computeObject, ShaderParameterName.CompileStatus, ref statusCode);
            }

            if (statusCode != 1)
                throw new ApplicationException(info);

            ProgramHandle program = GL.CreateProgram();

            GL.AttachShader(program, computeObject);
            GL.LinkProgram(program); //Must be called AFTER BindAttribLocation
            GL.DetachShader(program, computeObject);
            GL.DeleteShader(computeObject);

            return new ShaderHandleImp { Handle = program };
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
            ShaderHandle vertexObject = GL.CreateShader(ShaderType.VertexShader);
            ShaderHandle fragmentObject = GL.CreateShader(ShaderType.FragmentShader);
            int statusCode = -1;

            // Compile vertex shader
            GL.ShaderSource(vertexObject, vs);
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, out string info);
            GL.GetShaderi(vertexObject, ShaderParameterName.CompileStatus, ref statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            // Compile geometry shader
            ShaderHandle geometryObject = new(-1);
            if (!string.IsNullOrEmpty(gs))
            {
                geometryObject = GL.CreateShader(ShaderType.GeometryShader);

                GL.ShaderSource(geometryObject, gs);
                GL.CompileShader(geometryObject);
                GL.GetShaderInfoLog(geometryObject, out info);
                GL.GetShaderi(geometryObject, ShaderParameterName.CompileStatus, ref statusCode);
            }

            if (statusCode != 1)
                throw new ApplicationException(info);

            // Compile pixel shader
            GL.ShaderSource(fragmentObject, ps);
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(fragmentObject, out info);
            GL.GetShaderi(fragmentObject, ShaderParameterName.CompileStatus, ref statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            ProgramHandle program = GL.CreateProgram();
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
            GL.BindAttribLocation(program, AttributeLocations.FuseePlatformIdLocation, UniformNameDeclarations.FuseePlatformId);

            GL.LinkProgram(program); //Must be called AFTER BindAttribLocation

            GL.DetachShader(program, fragmentObject);
            GL.DetachShader(program, vertexObject);
            GL.DeleteShader(fragmentObject);
            GL.DeleteShader(vertexObject);

            return new ShaderHandleImp { Handle = program };
        }

        /// <inheritdoc />
        /// <summary>
        /// Removes shader from the GPU
        /// </summary>
        /// <param name="sp"></param>
        public void RemoveShader(IShaderHandle sp)
        {
            // wait for all threads to be finished
            GL.Finish();
            GL.Flush();

            GL.DeleteProgram(((ShaderHandleImp)sp).Handle);
        }


        /// <summary>
        /// Sets the shader program onto the GL Render context.
        /// </summary>
        /// <param name="program">The shader program.</param>
        public void SetShader(IShaderHandle program)
        {
            _textureCountPerShader = 0;
            _shaderParam2TexUnit.Clear();

            GL.UseProgram(((ShaderHandleImp)program).Handle);
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
            int h = GL.GetUniformLocation(((ShaderHandleImp)shaderProgram).Handle, paramName);
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
            float f = 0;
            GL.GetUniformf(((ShaderHandleImp)program).Handle, (int)((ShaderParam)param).handle, ref f);
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
            int ssboMaxLen = 0;
            int nParams = 0;

            GL.GetProgramInterfacei(sProg.Handle, ProgramInterface.ShaderStorageBlock, ProgramInterfacePName.MaxNameLength, ref ssboMaxLen);
            GL.GetProgramInterfacei(sProg.Handle, ProgramInterface.ShaderStorageBlock, ProgramInterfacePName.ActiveResources, ref nParams);

            for (uint i = 0; i < nParams; i++)
            {
                var paramInfo = new ShaderParamInfo();
                int length = 0;
                GL.GetProgramResourceName(sProg.Handle, ProgramInterface.ShaderStorageBlock, i, ssboMaxLen, ref length, out string name);
                paramInfo.Name = name;

                uint h = GL.GetProgramResourceIndex(sProg.Handle, ProgramInterface.ShaderStorageBlock, name);
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

            int nParams = 0;
            GL.GetProgrami(sProg.Handle, ProgramPropertyARB.ActiveUniforms, ref nParams);

            for (var i = 0; i < nParams; i++)
            {
                var paramInfo = new ShaderParamInfo();
                UniformType uType = UniformType.Bool;
                
                int length = 0;
                int size = 0;
                //TODO: (int)ProgramInterfacePName.MaxNameLength may not be the correct value here
                GL.GetActiveUniform(sProg.Handle, (uint)i, (int)ProgramInterfacePName.MaxNameLength, ref length, ref size, ref uType, out var name);
               
                paramInfo.Name = name;
                paramInfo.Handle = GetShaderUniformParam(sProg, paramInfo.Name);

                //TODO: remove cast to uint when UniformType.Image2d = 36941 is available
                paramInfo.Type = (uint)uType switch
                {
                    (uint)UniformType.Int => typeof(int),
                    (uint)UniformType.Bool => typeof(bool),
                    (uint)UniformType.Float => typeof(float),
                    (uint)UniformType.Double => typeof(double),
                    (uint)UniformType.IntVec2 => typeof(float2),
                    (uint)UniformType.FloatVec2 => typeof(float2),
                    (uint)UniformType.FloatVec3 => typeof(float3),
                    (uint)UniformType.FloatVec4 => typeof(float4),
                    (uint)UniformType.FloatMat4 => typeof(float4x4),
                    (uint)UniformType.Sampler2d or (uint)UniformType.UnsignedIntSampler2d or (uint)UniformType.IntSampler2d or (uint)UniformType.Sampler2dShadow /*or UniformType.Image2d*/or 36941 => typeof(ITextureBase),
                    (uint)UniformType.SamplerCube or (uint)UniformType.SamplerCubeShadow => typeof(IWritableCubeMap),
                    (uint)UniformType.Sampler2dArray or (uint)UniformType.Sampler2dArrayShadow => typeof(IWritableArrayTexture),
                    _ => throw new ArgumentOutOfRangeException($"UniformType {uType} unknown."),
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
            GL.Uniform1f((int)((ShaderParam)param).handle, val);
        }

        /// <summary>
        /// Sets a double shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, double val)
        {
            GL.Uniform1d((int)((ShaderParam)param).handle, val);
        }

        /// <summary>
        /// Sets a <see cref="float2" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float2 val)
        {
            GL.Uniform2f((int)((ShaderParam)param).handle, val.x, val.y);
        }

        /// <summary>
        /// Sets a <see cref="float2" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IShaderParam param, float2[] val)
        {
            foreach (var vec in val)
            {
                GL.Uniform2f((int)((ShaderParam)param).handle, vec.x, vec.y);
            }
        }

        /// <summary>
        /// Sets a <see cref="float3" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float3 val)
        {
            GL.Uniform3f(((ShaderParam)param).handle, val.x, val.y, val.z);
        }

        /// <summary>
        /// Sets a <see cref="float3" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IShaderParam param, float3[] val)
        {
            foreach (var vec in val)
            {
                GL.Uniform3f((int)((ShaderParam)param).handle, vec.x, vec.y, vec.z);
            }
        }

        /// <summary>
        /// Sets a <see cref="float4" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float4 val)
        {
            GL.Uniform4f(((ShaderParam)param).handle, val.x, val.y, val.z, val.w);
        }

        /// <summary>
        /// Sets a <see cref="float4x4" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
            GL.UniformMatrix4f(((ShaderParam)param).handle, 1, true, val.ToArray());
        }

        /// <summary>
        ///     Sets a <see cref="float4" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IShaderParam param, float4[] val)
        {
            foreach (var vec in val)
            {
                GL.Uniform4f(((ShaderParam)param).handle, vec.x, vec.y, vec.z, vec.w);
            }
        }

        /// <summary>
        /// Sets a <see cref="float4x4" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IShaderParam param, float4x4[] val)
        {
            var tmpArray = new float[val.Length * 4 * 4];
            for (var i = 0; i < val.Length; i++)
            {
               val[i].ToArray().CopyTo(tmpArray, i * 16);
            }
            GL.UniformMatrix4f(((ShaderParam)param).handle, val.Length, true, tmpArray);

            //var tmpArray = new float4[val.Length * 4];

            //for (var i = 0; i < val.Length; i++)
            //{
            //    tmpArray[i * 4] = val[i].Column1;
            //    tmpArray[i * 4 + 1] = val[i].Column2;
            //    tmpArray[i * 4 + 2] = val[i].Column3;
            //    tmpArray[i * 4 + 3] = val[i].Column4;
            //}

            //fixed (float4* pMtx = &tmpArray[0])
            //    GL.UniformMatrix4f((int)((ShaderParam)param).handle, val.Length, false, (float*)pMtx);
        }

        /// <summary>
        /// Sets a int shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, int val)
        {
            GL.Uniform1i(((ShaderParam)param).handle, val);
        }

        private void BindImage(TextureType texTarget, ITextureHandle texId, uint texUint, BufferAccessARB access, InternalFormat format)
        {
            switch (texTarget)
            {
                case TextureType.Image2D:
                    GL.BindImageTexture(texUint, ((TextureHandle)texId).TexHandle, 0, false, 0, access, format);
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
                    GL.BindTexture(TextureTarget.Texture1d, ((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.Texture2D:
                    GL.BindTexture(TextureTarget.Texture2d, ((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.Texture3D:
                    GL.BindTexture(TextureTarget.Texture3d, ((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.TextureCubeMap:
                    GL.BindTexture(TextureTarget.TextureCubeMap, ((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.ArrayTexture:
                    GL.BindTexture(TextureTarget.Texture2dArray, ((TextureHandle)texId).TexHandle);
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

            GL.ActiveTexture(TextureUnit.Texture0 + (uint)texUnit);
            BindTextureByTarget(texId, texTarget);
        }

        private void SetActiveAndBindImage(IShaderParam param, ITextureHandle texId, TextureType texTarget, ImagePixelFormat format, BufferAccessARB access, out int texUnit)
        {
            int iParam = ((ShaderParam)param).handle;
            if (!_shaderParam2TexUnit.TryGetValue(iParam, out texUnit))
            {
                _textureCountPerShader++;
                texUnit = (int)_textureCountPerShader;
                _shaderParam2TexUnit[iParam] = texUnit;
            }

            GL.ActiveTexture(TextureUnit.Texture0 + (uint)texUnit);
            BindImage(texTarget, texId, (uint)texUnit, access, GetInteralFormat(format));
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

            GL.ActiveTexture(TextureUnit.Texture0 + (uint)texUnit);
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

                GL.ActiveTexture(TextureUnit.Texture0 + (uint)firstTexUnit + (uint)i);
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

                GL.ActiveTexture(TextureUnit.Texture0 + (uint)firstTexUnit + (uint)i);
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
            SetActiveAndBindImage(param, texId, texTarget, format, BufferAccessARB.ReadWrite, out int texUnit);
            GL.Uniform1i(((ShaderParam)param).handle, texUnit);
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
            GL.Uniform1i(((ShaderParam)param).handle, texUnit);
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
            GL.Uniform1i(((ShaderParam)param).handle, texUnitArray.Length, new Span<int>(texUnitArray));
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
                float r = 0;
                float g = 0;
                float b = 0;
                float a = 0;

                GL.GetFloat(GetPName.ColorClearValue, 0, ref r);
                GL.GetFloat(GetPName.ColorClearValue, 1, ref g);
                GL.GetFloat(GetPName.ColorClearValue, 2, ref b);
                GL.GetFloat(GetPName.ColorClearValue, 3, ref a);
                return new float4(r, g, b, a);
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
                float ret = 0;
                GL.GetFloat(GetPName.DepthClearValue, ref ret);
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
            var handle = GL.GenBuffer();

            unsafe
            {
                using var data = attributes.AsMemory().Pin();
                GL.BufferData(BufferTargetARB.ArrayBuffer, vertsBytes, data.Pointer, BufferUsageARB.StaticDraw);
            }

            int vboBytes = 0;
            GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(string.Format(
                    "Problem uploading attribute buffer to VBO ('{2}'). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes, attributeName));

            return new AttributeImp { AttributeBufferObject = handle };
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
                var handle = ((AttributeImp)attribHandle).AttributeBufferObject;
                if (handle.Handle != 0)
                {
                    GL.DeleteBuffer(in handle);
                    ((AttributeImp)attribHandle).AttributeBufferObject.Handle = 0;
                }
            }
        }

        /// <summary>
        /// Binds the VertexArrayObject onto the GL Render context and assigns its index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        public void SetVertexArrayObject(IMeshImp mr)
        {
            if (((MeshImp)mr).VertexArrayObject.Handle == 0)
                ((MeshImp)mr).VertexArrayObject = GL.GenVertexArray();

            GL.BindVertexArray(((MeshImp)mr).VertexArrayObject);
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
            if (((MeshImp)mr).VertexBufferObject.Handle == 0)
                ((MeshImp)mr).VertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
            unsafe
            {
                using var dataMemHandle = vertices.AsMemory().Pin();
                GL.BufferData(BufferTargetARB.ArrayBuffer, vertsBytes, dataMemHandle.Pointer, BufferUsageARB.StaticDraw);
            }
            int vboBytes = 0;
            GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref vboBytes);
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
            if (((MeshImp)mr).TangentBufferObject.Handle == 0)
                ((MeshImp)mr).TangentBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).TangentBufferObject);
            unsafe
            {
                using var dataMemHandle = tangents.AsMemory().Pin();
                GL.BufferData(BufferTargetARB.ArrayBuffer, tangentBytes, dataMemHandle.Pointer, BufferUsageARB.StaticDraw);
            }
            int vboBytes = 0;
            GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref vboBytes);
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
            if (((MeshImp)mr).BitangentBufferObject.Handle == 0)
                ((MeshImp)mr).BitangentBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).BitangentBufferObject);
            unsafe
            {
                using var dataMemHandle = bitangents.AsMemory().Pin();
                GL.BufferData(BufferTargetARB.ArrayBuffer, bitangentBytes, dataMemHandle.Pointer, BufferUsageARB.StaticDraw);
            }
            int vboBytes = 0;
            GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref vboBytes);
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
            if (((MeshImp)mr).NormalBufferObject.Handle == 0)
                ((MeshImp)mr).NormalBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
            unsafe
            {
                using var dataMemHandle = normals.AsMemory().Pin();
                GL.BufferData(BufferTargetARB.ArrayBuffer, normsBytes, dataMemHandle.Pointer, BufferUsageARB.StaticDraw);
            }
            int vboBytes = 0;
            GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref vboBytes);
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
            if (((MeshImp)mr).BoneIndexBufferObject.Handle == 0)
                ((MeshImp)mr).BoneIndexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).BoneIndexBufferObject);
            unsafe
            {
                using var dataMemHandle = boneIndices.AsMemory().Pin();
                GL.BufferData(BufferTargetARB.ArrayBuffer, indicesBytes, dataMemHandle.Pointer, BufferUsageARB.StaticDraw);
            }
            int vboBytes = 0;
            GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref vboBytes);
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
            if (((MeshImp)mr).BoneWeightBufferObject.Handle == 0)
                ((MeshImp)mr).BoneWeightBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).BoneWeightBufferObject);
            unsafe
            {
                using var dataMemHandle = boneWeights.AsMemory().Pin();
                GL.BufferData(BufferTargetARB.ArrayBuffer, weightsBytes, dataMemHandle.Pointer, BufferUsageARB.StaticDraw);
            }
            int vboBytes = 0;
            GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref vboBytes);
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
            if (((MeshImp)mr).UVBufferObject.Handle == 0)
                ((MeshImp)mr).UVBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).UVBufferObject);
            unsafe
            {
                using var dataMemHandle = uvs.AsMemory().Pin();
                GL.BufferData(BufferTargetARB.ArrayBuffer, uvsBytes, dataMemHandle.Pointer, BufferUsageARB.StaticDraw);
            }
            int vboBytes = 0;
            GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref vboBytes);
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
            if (((MeshImp)mr).ColorBufferObject.Handle == 0)
                ((MeshImp)mr).ColorBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).ColorBufferObject);
            unsafe
            {
                using var dataMemHandle = colors.AsMemory().Pin();
                GL.BufferData(BufferTargetARB.ArrayBuffer, colsBytes, dataMemHandle.Pointer, BufferUsageARB.StaticDraw);
            }
            int vboBytes = 0;
            GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref vboBytes);
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
            if (((MeshImp)mr).ColorBufferObject1.Handle == 0)
                ((MeshImp)mr).ColorBufferObject1 = GL.GenBuffer();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).ColorBufferObject1);
            unsafe
            {
                using var dataMemHandle = colors.AsMemory().Pin();
                GL.BufferData(BufferTargetARB.ArrayBuffer, colsBytes, dataMemHandle.Pointer, BufferUsageARB.StaticDraw);
            }
            int vboBytes = 0;
            GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref vboBytes);
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
            if (((MeshImp)mr).ColorBufferObject2.Handle == 0)
                ((MeshImp)mr).ColorBufferObject2 = GL.GenBuffer();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).ColorBufferObject2);

            unsafe
            {
                using var dataMemHandle = colors.AsMemory().Pin();
                GL.BufferData(BufferTargetARB.ArrayBuffer, colsBytes, dataMemHandle.Pointer, BufferUsageARB.StaticDraw);
            }

            int vboBytes = 0;
            GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref vboBytes);
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

            if (((MeshImp)mr).ElementBufferObject.Handle == 0)
                ((MeshImp)mr).ElementBufferObject = GL.GenBuffer();
            // Upload the index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
            GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);
            unsafe
            {
                using var dataMemHandle = triangleIndices.AsMemory().Pin();
                GL.BufferData(BufferTargetARB.ElementArrayBuffer, trisBytes, dataMemHandle.Pointer, BufferUsageARB.StaticDraw);
            }

            int vboBytes = 0;
            GL.GetBufferParameteri(BufferTargetARB.ElementArrayBuffer, BufferPNameARB.BufferSize, ref vboBytes);
            if (vboBytes != trisBytes)
                throw new ApplicationException(string.Format("Problem uploading vertex buffer to VBO (offsets). Tried to upload {0} bytes, uploaded {1}.", trisBytes, vboBytes));
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveVertices(IMeshImp mr)
        {
            GL.DeleteVertexArray(((MeshImp)mr).VertexArrayObject);
            GL.DeleteBuffer(((MeshImp)mr).VertexBufferObject);
            ((MeshImp)mr).InvalidateVertices();
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
            GL.DeleteBuffer(((MeshImp)mr).ColorBufferObject1);
            ((MeshImp)mr).InvalidateColors1();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveColors2(IMeshImp mr)
        {
            GL.DeleteBuffer(((MeshImp)mr).ColorBufferObject2);
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
        public void DispatchCompute(int kernelIndex, uint threadGroupsX, uint threadGroupsY, uint threadGroupsZ)
        {
            GL.DispatchCompute(threadGroupsX, threadGroupsY, threadGroupsZ);
        }

        /// <summary>
        /// Renders the specified <see cref="IMeshImp" />.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        public void Render(IMeshImp mr)
        {
            GL.BindVertexArray(((MeshImp)mr).VertexArrayObject);

            if (((MeshImp)mr).VertexBufferObject.Handle != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.VertexAttribLocation);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
                GL.VertexAttribPointer(AttributeLocations.VertexAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).ColorBufferObject.Handle != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.ColorAttribLocation);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).ColorBufferObject);
                GL.VertexAttribPointer(AttributeLocations.ColorAttribLocation, 4, VertexAttribPointerType.UnsignedByte, true, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).ColorBufferObject1.Handle != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.Color1AttribLocation);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).ColorBufferObject1);
                GL.VertexAttribPointer(AttributeLocations.Color1AttribLocation, 4, VertexAttribPointerType.UnsignedByte, true, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).ColorBufferObject2.Handle != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.Color2AttribLocation);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).ColorBufferObject2);
                GL.VertexAttribPointer(AttributeLocations.Color2AttribLocation, 4, VertexAttribPointerType.UnsignedByte, true, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).UVBufferObject.Handle != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.UvAttribLocation);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).UVBufferObject);
                GL.VertexAttribPointer(AttributeLocations.UvAttribLocation, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).NormalBufferObject.Handle != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.NormalAttribLocation);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
                GL.VertexAttribPointer(AttributeLocations.NormalAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).TangentBufferObject.Handle != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.TangentAttribLocation);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).TangentBufferObject);
                GL.VertexAttribPointer(AttributeLocations.TangentAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).BitangentBufferObject.Handle != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.BitangentAttribLocation);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).BitangentBufferObject);
                GL.VertexAttribPointer(AttributeLocations.BitangentAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).BoneIndexBufferObject.Handle != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.BoneIndexAttribLocation);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).BoneIndexBufferObject);
                GL.VertexAttribPointer(AttributeLocations.BoneIndexAttribLocation, 4, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).BoneWeightBufferObject.Handle != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.BoneWeightAttribLocation);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, ((MeshImp)mr).BoneWeightBufferObject);
                GL.VertexAttribPointer(AttributeLocations.BoneWeightAttribLocation, 4, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).ElementBufferObject.Handle != 0)
            {
                GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);

                switch (((MeshImp)mr).MeshType)
                {
                    case OpenGLPrimitiveType.Triangles:
                    default:
                        GL.DrawElements(PrimitiveType.Triangles, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.Points:
                        // enable gl_PointSize to set the point size
                        if (!_isPtRenderingEnabled)
                        {
                            _isPtRenderingEnabled = true;
                            GL.Enable(EnableCap.ProgramPointSize);
                            //GL.Enable(EnableCap.PointSprite);
                            //GL.Enable(EnableCap.VertexProgramPointSize);
                        }
                        GL.DrawElements(PrimitiveType.Points, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.Lines:
                        if (!_isLineSmoothEnabled)
                        {
                            GL.Enable(EnableCap.LineSmooth);
                            _isLineSmoothEnabled = true;
                        }
                        GL.DrawElements(PrimitiveType.Lines, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.LineLoop:
                        if (!_isLineSmoothEnabled)
                        {
                            GL.Enable(EnableCap.LineSmooth);
                            _isLineSmoothEnabled = true;
                        }
                        GL.DrawElements(PrimitiveType.LineLoop, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.LineStrip:
                        if (!_isLineSmoothEnabled)
                        {
                            GL.Enable(EnableCap.LineSmooth);
                            _isLineSmoothEnabled = true;
                        }
                        GL.DrawElements(PrimitiveType.LineStrip, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.Patches:
                        GL.DrawElements(PrimitiveType.Patches, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.QuadStrip:
                        GL.DrawElements(PrimitiveType.QuadStrip, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.TriangleFan:
                        GL.DrawElements(PrimitiveType.TriangleFan, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.TriangleStrip:
                        GL.DrawElements(PrimitiveType.TriangleStrip, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                }
            }

            if (((MeshImp)mr).VertexBufferObject.Handle != 0)
                GL.DisableVertexAttribArray(AttributeLocations.VertexAttribLocation);
            if (((MeshImp)mr).ColorBufferObject.Handle != 0)
                GL.DisableVertexAttribArray(AttributeLocations.ColorAttribLocation);
            if (((MeshImp)mr).NormalBufferObject.Handle != 0)
                GL.DisableVertexAttribArray(AttributeLocations.NormalAttribLocation);
            if (((MeshImp)mr).UVBufferObject.Handle != 0)
                GL.DisableVertexAttribArray(AttributeLocations.UvAttribLocation);
            if (((MeshImp)mr).TangentBufferObject.Handle != 0)
                GL.DisableVertexAttribArray(AttributeLocations.TangentAttribLocation);
            if (((MeshImp)mr).BitangentBufferObject.Handle != 0)
                GL.DisableVertexAttribArray(AttributeLocations.TangentAttribLocation);

            GL.BindVertexArray(new VertexArrayHandle(0));
        }

        /// <summary>
        /// Gets the content of the buffer.
        /// </summary>
        /// <param name="quad">The Rectangle where the content is draw into.</param>
        /// <param name="texId">The texture identifier.</param>
        public void GetBufferContent(Common.Rectangle quad, ITextureHandle texId)
        {
            GL.BindTexture(TextureTarget.Texture2d, ((TextureHandle)texId).TexHandle);
            GL.CopyTexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, quad.Left, quad.Top, quad.Width, quad.Height, 0);
        }

        /// <summary>
        /// Creates the mesh implementation.
        /// </summary>
        /// <returns>The <see cref="IMeshImp" /> instance.</returns>
        public IMeshImp CreateMeshImp()
        {
            return new MeshImp();
        }

        internal static BlendEquationModeEXT BlendOperationToOgl(BlendOperation bo)
        {
            return bo switch
            {
                BlendOperation.Add => BlendEquationModeEXT.FuncAdd,
                BlendOperation.Subtract => BlendEquationModeEXT.FuncSubtract,
                BlendOperation.ReverseSubtract => BlendEquationModeEXT.FuncReverseSubtract,
                BlendOperation.Minimum => BlendEquationModeEXT.Min,
                BlendOperation.Maximum => BlendEquationModeEXT.Max,
                _ => throw new ArgumentOutOfRangeException($"Invalid argument: {bo}"),
            };
        }

        internal static BlendOperation BlendOperationFromOgl(BlendEquationModeEXT bom)
        {
            return bom switch
            {
                BlendEquationModeEXT.FuncAdd => BlendOperation.Add,
                BlendEquationModeEXT.Min => BlendOperation.Minimum,
                BlendEquationModeEXT.Max => BlendOperation.Maximum,
                BlendEquationModeEXT.FuncSubtract => BlendOperation.Subtract,
                BlendEquationModeEXT.FuncReverseSubtract => BlendOperation.ReverseSubtract,
                _ => throw new ArgumentOutOfRangeException($"Invalid argument: {bom}"),
            };
        }

        internal static int BlendToOgl(Blend blend, bool isForBlendFactorAlpha = false)
        {
            return blend switch
            {
                Blend.Zero => (int)BlendingFactor.Zero,
                Blend.One => (int)BlendingFactor.One,
                Blend.SourceColor => (int)BlendingFactor.SrcColor,
                Blend.InverseSourceColor => (int)BlendingFactor.OneMinusSrcColor,
                Blend.SourceAlpha => (int)BlendingFactor.SrcAlpha,
                Blend.InverseSourceAlpha => (int)BlendingFactor.OneMinusSrcAlpha,
                Blend.DestinationAlpha => (int)BlendingFactor.DstAlpha,
                Blend.InverseDestinationAlpha => (int)BlendingFactor.OneMinusDstAlpha,
                Blend.DestinationColor => (int)BlendingFactor.DstColor,
                Blend.InverseDestinationColor => (int)BlendingFactor.OneMinusDstColor,
                Blend.BlendFactor => (int)((isForBlendFactorAlpha) ? BlendingFactor.ConstantAlpha : BlendingFactor.ConstantColor),
                Blend.InverseBlendFactor => (int)((isForBlendFactorAlpha) ? BlendingFactor.OneMinusConstantAlpha : BlendingFactor.OneMinusConstantColor),
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
                (int)BlendingFactor.Zero => Blend.Zero,
                (int)BlendingFactor.One => Blend.One,
                (int)BlendingFactor.SrcColor => Blend.SourceColor,
                (int)BlendingFactor.OneMinusSrcColor => Blend.InverseSourceColor,
                (int)BlendingFactor.SrcAlpha => Blend.SourceAlpha,
                (int)BlendingFactor.OneMinusSrcAlpha => Blend.InverseSourceAlpha,
                (int)BlendingFactor.DstAlpha => Blend.DestinationAlpha,
                (int)BlendingFactor.OneMinusDstAlpha => Blend.InverseDestinationAlpha,
                (int)BlendingFactor.DstColor => Blend.DestinationColor,
                (int)BlendingFactor.OneMinusDstColor => Blend.InverseDestinationColor,
                (int)BlendingFactor.ConstantAlpha or (int)BlendingFactor.ConstantColor => Blend.BlendFactor,
                (int)BlendingFactor.OneMinusConstantAlpha or (int)BlendingFactor.OneMinusConstantColor => Blend.InverseBlendFactor,
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
                        _blendSrcRgb = (BlendingFactor)BlendToOgl((Blend)value);
                        GL.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.DestinationBlend:
                    {
                        _blendDstRgb = (BlendingFactor)BlendToOgl((Blend)value);
                        GL.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.SourceBlendAlpha:
                    {
                        _blendSrcAlpha = (BlendingFactor)BlendToOgl((Blend)value);
                        GL.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.DestinationBlendAlpha:
                    {
                        _blendDstAlpha = (BlendingFactor)BlendToOgl((Blend)value);
                        GL.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.BlendFactor:
                    var col = Color.FromArgb((int)value);
                    GL.BlendColor(col.R, col.G, col.B, col.A);
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
                        int pm = 0;
                        GL.GetInteger(GetPName.PolygonMode, ref pm);
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
                        int cullFace = 0;
                        int frontFace = 0;
                        GL.GetInteger(GetPName.CullFace, ref cullFace);
                        if (cullFace == 0)
                            return (uint)Cull.None;
                        GL.GetInteger(GetPName.FrontFace, ref frontFace);
                        if (frontFace == (int)FrontFaceDirection.Cw)
                            return (uint)Cull.Clockwise;
                        return (uint)Cull.Counterclockwise;
                    }
                case RenderState.Clipping:
                    // clipping is always on in OpenGL - This state is simply ignored
                    return 1; // == true
                case RenderState.ZFunc:
                    {
                        int depFunc = 0;
                        GL.GetInteger(GetPName.DepthFunc, ref depFunc);
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
                        int depTest = 0;
                        GL.GetInteger(GetPName.DepthTest, ref depTest);
                        return (uint)(depTest);
                    }
                case RenderState.ZWriteEnable:
                    {
                        int depWriteMask = 0;
                        GL.GetInteger(GetPName.DepthWritemask, ref depWriteMask);
                        return (uint)(depWriteMask);
                    }
                case RenderState.AlphaBlendEnable:
                    {
                        int blendEnable = 0;
                        GL.GetInteger(GetPName.Blend, ref blendEnable);
                        return (uint)(blendEnable);
                    }
                case RenderState.BlendOperation:
                    {
                        int rgbMode = 0;
                        GL.GetInteger(GetPName.BlendEquationRgb, ref rgbMode);
                        return (uint)BlendOperationFromOgl((BlendEquationModeEXT)rgbMode);
                    }
                case RenderState.BlendOperationAlpha:
                    {
                        int alphaMode = 0;
                        GL.GetInteger(GetPName.BlendEquationAlpha, ref alphaMode);
                        return (uint)BlendOperationFromOgl((BlendEquationModeEXT)alphaMode);
                    }
                case RenderState.SourceBlend:
                    {
                        int rgbSrc = 0;
                        GL.GetInteger(GetPName.BlendSrcRgb, ref rgbSrc);
                        return (uint)BlendFromOgl(rgbSrc);
                    }
                case RenderState.DestinationBlend:
                    {
                        int rgbDst = 0;
                        GL.GetInteger(GetPName.BlendSrcRgb, ref rgbDst);
                        return (uint)BlendFromOgl(rgbDst);
                    }
                case RenderState.SourceBlendAlpha:
                    {
                        int alphaSrc = 0;
                        GL.GetInteger(GetPName.BlendSrcAlpha, ref alphaSrc);
                        return (uint)BlendFromOgl(alphaSrc);
                    }
                case RenderState.DestinationBlendAlpha:
                    {
                        int alphaDst = 0;
                        GL.GetInteger(GetPName.BlendDstAlpha, ref alphaDst);
                        return (uint)BlendFromOgl(alphaDst);
                    }
                case RenderState.BlendFactor:
                    int col = 0;
                    GL.GetInteger(GetPName.BlendColorExt, ref col);
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
            if (((TextureHandle)texHandle).FrameBufferHandle.Handle == -1)
            {
                var fBuffer = GL.GenFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fBuffer);

                GL.BindTexture(TextureTarget.Texture2d, ((TextureHandle)texHandle).TexHandle);

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    CreateDepthRenderBuffer(tex.Width, tex.Height);
                    GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, ((TextureHandle)texHandle).TexHandle, 0);
                    GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
                }
                else
                {
                    GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, ((TextureHandle)texHandle).TexHandle, 0);
                    GL.DrawBuffer(DrawBufferMode.None);
                    GL.ReadBuffer(ReadBufferMode.None);
                }
            }
            else
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, ((TextureHandle)texHandle).FrameBufferHandle);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");

            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
        }

        /// <summary>
        /// Renders into the given cube map.
        /// </summary>
        /// <param name="tex">The texture.</param>
        /// <param name="texHandle">The texture handle, associated with the given cube map. Should be created by the TextureManager in the RenderContext.</param>
        public void SetRenderTarget(IWritableCubeMap tex, ITextureHandle texHandle)
        {
            if (((TextureHandle)texHandle).FrameBufferHandle.Handle == -1)
            {
                var fBuffer = GL.GenFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fBuffer);

                GL.BindTexture(TextureTarget.TextureCubeMap, ((TextureHandle)texHandle).TexHandle);

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    CreateDepthRenderBuffer(tex.Width, tex.Height);
                    GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, ((TextureHandle)texHandle).TexHandle, 0);
                    GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
                }
                else
                {
                    GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, ((TextureHandle)texHandle).TexHandle, 0);
                    GL.DrawBuffer(DrawBufferMode.None);
                    GL.ReadBuffer(ReadBufferMode.None);
                }
            }
            else
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, ((TextureHandle)texHandle).FrameBufferHandle);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");


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
            if (((TextureHandle)texHandle).FrameBufferHandle.Handle == -1)
            {
                var fBuffer = GL.GenFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fBuffer);

                GL.BindTexture(TextureTarget.Texture2dArray, ((TextureHandle)texHandle).TexHandle);

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    CreateDepthRenderBuffer(tex.Width, tex.Height);
                    GL.FramebufferTextureLayer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, ((TextureHandle)texHandle).TexHandle, 0, layer);
                    GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
                }
                else
                {
                    GL.FramebufferTextureLayer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, ((TextureHandle)texHandle).TexHandle, 0, layer);
                    GL.DrawBuffer(DrawBufferMode.None);
                    GL.ReadBuffer(ReadBufferMode.None);
                }
            }
            else
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, ((TextureHandle)texHandle).FrameBufferHandle);
                GL.BindTexture(TextureTarget.Texture2dArray, ((TextureHandle)texHandle).TexHandle);
                GL.FramebufferTextureLayer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, ((TextureHandle)texHandle).TexHandle, 0, layer);
            }

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");

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
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, new FramebufferHandle(0));
                return;
            }

            FramebufferHandle gBuffer;

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

            if (renderTarget.RenderTextures[(int)RenderTargetTextureTypes.Depth] == null && !renderTarget.IsDepthOnly)
            {
                RenderbufferHandle gDepthRenderbufferHandle;
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
                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);
                }
            }

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
            {
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");
            }

            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
        }

        private RenderbufferHandle CreateDepthRenderBuffer(int width, int height)
        {
            GL.Enable(EnableCap.DepthTest);

            var gDepthRenderbufferHandle = GL.GenRenderbuffer();
            //((FrameBufferHandle)renderTarget.DepthBufferHandle).Handle = gDepthRenderbufferHandle;
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);
            return gDepthRenderbufferHandle;
        }

        private FramebufferHandle CreateFrameBuffer(IRenderTarget renderTarget, ITextureHandle[] texHandles)
        {
            var gBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);

            int depthCnt = 0;

            var depthTexPos = (int)RenderTargetTextureTypes.Depth;

            if (!renderTarget.IsDepthOnly)
            {
                var attachments = new List<DrawBufferMode>();

                //Textures
                for (uint i = 0; i < texHandles.Length; i++)
                {
                    attachments.Add(DrawBufferMode.ColorAttachment0 + i);

                    var texHandle = texHandles[i];
                    if (texHandle == null) continue;

                    if (i == depthTexPos)
                    {
                        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment + (uint)(depthCnt), TextureTarget.Texture2d, ((TextureHandle)texHandle).TexHandle, 0);
                        depthCnt++;
                    }
                    else
                        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + (uint)(i - depthCnt), TextureTarget.Texture2d, ((TextureHandle)texHandle).TexHandle, 0);
                }
                GL.DrawBuffers(attachments.ToArray());
            }
            else //If a frame-buffer only has a depth texture we don't need draw buffers
            {
                var texHandle = texHandles[depthTexPos];

                if (texHandle != null)
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2d, ((TextureHandle)texHandle).TexHandle, 0);
                else
                    throw new NullReferenceException("Texture handle is null!");

                GL.DrawBuffer(DrawBufferMode.None);
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
            ChangeFramebufferTexture2d(renderTarget, attachment, new OpenTK.Graphics.TextureHandle(0), isDepthTex);
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
            ChangeFramebufferTexture2d(renderTarget, attachment, ((TextureHandle)texHandle).TexHandle, isDepthTex);
        }

        private void ChangeFramebufferTexture2d(IRenderTarget renderTarget, int attachment, OpenTK.Graphics.TextureHandle handle, bool isDepth)
        {
            int boundFbo = 0;
            GL.GetInteger(GetPName.DrawFramebufferBinding, ref boundFbo);
            var rtFbo = ((FrameBufferHandle)renderTarget.GBufferHandle).Handle;

            var isCurrentFbo = true;

            if (boundFbo != rtFbo.Handle)
            {
                isCurrentFbo = false;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, rtFbo);
            }

            if (!isDepth)
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + (uint)attachment, TextureTarget.Texture2d, handle, 0);
            else
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2d, handle, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");

            if (!isCurrentFbo)
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, new FramebufferHandle(boundFbo));
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
            var shaderProgram = ((ShaderHandleImp)currentProgram).Handle;
            var resInx = GL.GetProgramResourceIndex(shaderProgram, ProgramInterface.ShaderStorageBlock, ssboName);
            GL.ShaderStorageBlockBinding(shaderProgram, resInx, buffer.BindingIndex);
            GL.BindBufferBase(BufferTargetARB.ShaderStorageBuffer, buffer.BindingIndex, ((StorageBufferHandle)buffer.BufferHandle).Handle);
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
            if (bufferHandle.Handle.Handle == -1)
            {
                bufferHandle.Handle = GL.GenBuffer();
            }

            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Data must not be null or empty");
            }

            GL.BindBuffer(BufferTargetARB.ShaderStorageBuffer, bufferHandle.Handle);

            unsafe
            {
                using var dataMemHandle = data.AsMemory().Pin();
                GL.BufferData(BufferTargetARB.ShaderStorageBuffer, dataBytes, dataMemHandle.Pointer, BufferUsageARB.StaticDraw);
            }

            int bufferBytes = 0;
            GL.GetBufferParameteri(BufferTargetARB.ShaderStorageBuffer, BufferPNameARB.BufferSize, ref bufferBytes);
            if (bufferBytes != dataBytes)
                throw new ApplicationException(string.Format("Problem uploading bone indices buffer to SSBO. Tried to upload {0} bytes, uploaded {1}.", bufferBytes, dataBytes));

            GL.BindBuffer(BufferTargetARB.ShaderStorageBuffer, new BufferHandle(0));
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
            unsafe
            {
                using var data = image.PixelData.AsMemory().Pin();
                GL.ReadPixels(x, y, w, h, PixelFormat.Bgr, PixelType.UnsignedByte, data.Pointer);
            }
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