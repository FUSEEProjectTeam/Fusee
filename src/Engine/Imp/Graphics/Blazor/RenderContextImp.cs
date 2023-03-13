using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Imp.Blazor;
using Fusee.Engine.Imp.SharedAll;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using static Fusee.Engine.Imp.Graphics.Blazor.WebGL2RenderingContextBase;
using static Fusee.Engine.Imp.Graphics.Blazor.WebGLRenderingContextBase;

namespace Fusee.Engine.Imp.Graphics.Blazor
{
    /// <summary>
    /// Implementation of the <see cref="IRenderContextImp" /> interface on top of WebGLDotNET.
    /// </summary>
    public class RenderContextImp : IRenderContextImp
    {
        /// <summary>
        /// The WebGL rendering context base.
        /// </summary>
        protected WebGLRenderingContextBase gl;

        /// <summary>
        /// The WebGL2 rendering context base.
        /// </summary>
        protected WebGL2RenderingContextBase gl2;

        private int _textureCountPerShader;
        private readonly Dictionary<WebGLUniformLocation, int> _shaderParam2TexUnit;

        private uint _blendEquationAlpha;
        private uint _blendEquationRgb;
        private uint _blendDstRgb;
        private uint _blendSrcRgb;
        private uint _blendSrcAlpha;
        private uint _blendDstAlpha;

        private bool _isCullEnabled;
        private WebGLFramebuffer _lastBoundFbo;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContextImp"/> class.
        /// </summary>
        /// <param name="renderCanvasImp">The platform specific render canvas implementation.</param>
        public RenderContextImp(IRenderCanvasImp renderCanvasImp)
        {
            _textureCountPerShader = 0;
            _shaderParam2TexUnit = new Dictionary<WebGLUniformLocation, int>();

            gl2 = ((RenderCanvasImp)renderCanvasImp)._gl;

            gl2.Enable(CULL_FACE);
            gl2.CullFace(BACK);

            gl2.Enable(DEPTH_TEST);
            gl2.Enable(SCISSOR_TEST);
        }

        #region Image data related Members

        private uint GetTexCompareMode(TextureCompareMode compareMode)
        {
            return compareMode switch
            {
                TextureCompareMode.None => (int)NONE,
                TextureCompareMode.CompareRefToTexture => (int)COMPARE_REF_TO_TEXTURE,
                _ => throw new ArgumentException("Invalid compare mode."),
            };
        }

        private Tuple<int, int> GetMinMagFilter(TextureFilterMode filterMode)
        {
            int minFilter;
            int magFilter;

            switch (filterMode)
            {
                case TextureFilterMode.Nearest:
                    minFilter = (int)NEAREST;
                    magFilter = (int)NEAREST;
                    break;
                case TextureFilterMode.NearestMipmapNearest:
                    minFilter = (int)NEAREST_MIPMAP_NEAREST;
                    magFilter = (int)LINEAR;
                    break;
                case TextureFilterMode.LinearMipmapNearest:
                    minFilter = (int)LINEAR_MIPMAP_NEAREST;
                    magFilter = (int)LINEAR;
                    break;
                case TextureFilterMode.NearestMipmapLinear:
                    minFilter = (int)LINEAR;// TODO(mr): NEAREST_MIPMAP_LINEAR; breaks everything with invalid enum, don't know WHY?
                    magFilter = (int)LINEAR;
                    break;
                case TextureFilterMode.LinearMipmapLinear:
                    minFilter = (int)NEAREST_MIPMAP_LINEAR;
                    magFilter = (int)LINEAR;
                    break;
                case TextureFilterMode.Linear:
                default:
                    minFilter = (int)LINEAR;
                    magFilter = (int)LINEAR;
                    break;
            }

            return new Tuple<int, int>(minFilter, magFilter);
        }

        private int GetWrapMode(TextureWrapMode wrapMode)
        {
            return wrapMode switch
            {
                TextureWrapMode.MirroredRepeat => (int)MIRRORED_REPEAT,
                TextureWrapMode.ClampToBorder or TextureWrapMode.ClampToEdge => (int)CLAMP_TO_EDGE,
                // case TextureWrapMode.Repeat:
                _ => (int)REPEAT,
            };
        }

        private uint GetDepthCompareFunc(Compare compareFunc)
        {
            return compareFunc switch
            {
                Compare.Never => NEVER,
                Compare.Less => LESS,
                Compare.Equal => EQUAL,
                Compare.LessEqual => LEQUAL,
                Compare.Greater => GREATER,
                Compare.NotEqual => NOTEQUAL,
                Compare.GreaterEqual => GEQUAL,
                Compare.Always => ALWAYS,
                _ => throw new ArgumentOutOfRangeException("value"),
            };
        }

        private static TexturePixelInfo GetTexturePixelInfo(ImagePixelFormat pxFormat)
        {
            uint internalFormat;
            uint format;
            uint pxType;

            //The wrong row alignment will lead to malformed textures.
            //See https://www.khronos.org/opengl/wiki/Common_Mistakes#Texture_upload_and_pixel_reads
            //and https://www.khronos.org/opengl/wiki/Pixel_Transfer#Pixel_layout
            int rowAlignment = 4;

            switch (pxFormat.ColorFormat)
            {
                case ColorFormat.RGBA:
                    internalFormat = RGBA;
                    format = RGBA;
                    pxType = UNSIGNED_BYTE;
                    break;
                case ColorFormat.RGB:
                    internalFormat = RGB;
                    format = RGB;
                    pxType = UNSIGNED_BYTE;
                    rowAlignment = 1;
                    break;
                case ColorFormat.Intensity:
                    internalFormat = R8;
                    format = RED;
                    pxType = UNSIGNED_BYTE;
                    rowAlignment = 1;
                    break;
                case ColorFormat.Depth24:
                    internalFormat = DEPTH_COMPONENT24;
                    format = DEPTH_COMPONENT;
                    pxType = UNSIGNED_INT;
                    break;
                case ColorFormat.Depth16:
                    internalFormat = DEPTH_COMPONENT16;
                    format = DEPTH_COMPONENT;
                    pxType = UNSIGNED_INT;
                    break;
                case ColorFormat.uiRgb8:
                    internalFormat = RGBA8UI;
                    format = RGBA;
                    pxType = UNSIGNED_INT;
                    rowAlignment = 1;
                    break;
                case ColorFormat.fRGB32:
                    throw new NotSupportedException("WebGL 2.0: fRGB32 not supported");
                //internalFormat = RGB32F;
                //format = RGB;
                //pxType = FLOAT;
                case ColorFormat.fRGB16:
                    throw new NotSupportedException("WebGL 2.0: fRGB16 not supported");
                //internalFormat = RGB16F;
                //format = RGB;
                //pxType = HALF_FLOAT;
                case ColorFormat.fRGBA16:
                    internalFormat = RGBA16F;
                    format = RGBA;
                    pxType = HALF_FLOAT;
                    break;
                case ColorFormat.fRGBA32:
                    internalFormat = RGBA32F;
                    format = RGBA;
                    pxType = FLOAT;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"CreateTexture: Image pixel format not supported {pxFormat.ColorFormat}");
            }

            return new TexturePixelInfo()
            {
                Format = format,
                InternalFormat = internalFormat,
                PxType = pxType,
                RowAlignment = rowAlignment

            };
        }

        private static Array GetEmptyArray(IWritableTexture tex)
        {
            return tex.PixelFormat.ColorFormat switch
            {
                ColorFormat.RGBA => new int[tex.Width * tex.Height * 4],
                ColorFormat.RGB => new int[tex.Width * tex.Height * 3],
                // TODO: Handle Alpha-only / Intensity-only and AlphaIntensity correctly.
                ColorFormat.Intensity => new int[tex.Width * tex.Height],
                ColorFormat.Depth24 => new uint[tex.Width * tex.Height],
                ColorFormat.Depth16 => new int[tex.Width * tex.Height],
                ColorFormat.uiRgb8 => new int[tex.Width * tex.Height * 4],
                ColorFormat.fRGB32 or ColorFormat.fRGB16 => throw new NotSupportedException("WebGL 2.0: fRGB32 or fRGB16 not supported"),
                ColorFormat.fRGBA16 or ColorFormat.fRGBA32 => new float[tex.Width * tex.Height * 4],
                _ => throw new ArgumentOutOfRangeException($"GetEmptyArray: Image pixel format not supported {tex.PixelFormat.ColorFormat}"),
            };
        }

        /// <summary>
        /// Creates a new CubeMap and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableCubeMap img)
        {
            WebGLTexture id = gl2.CreateTexture();
            gl2.BindTexture(TEXTURE_CUBE_MAP, id);

            Tuple<int, int> glMinMagFilter = GetMinMagFilter(img.FilterMode);
            int minFilter = glMinMagFilter.Item1;
            int magFilter = glMinMagFilter.Item2;

            int glWrapMode = GetWrapMode(img.WrapMode);
            TexturePixelInfo pxInfo = GetTexturePixelInfo(img.PixelFormat);

            var data = GetEmptyArray(img);

            for (int i = 0; i < 6; i++)
            {
                gl2.TexImage2D(TEXTURE_CUBE_MAP_POSITIVE_X + (uint)i, 0, (int)pxInfo.InternalFormat, img.Width, img.Height, 0, pxInfo.Format, pxInfo.PxType, data);
            }

            gl2.TexParameteri(TEXTURE_CUBE_MAP, TEXTURE_MAG_FILTER, magFilter);
            gl2.TexParameteri(TEXTURE_CUBE_MAP, TEXTURE_MIN_FILTER, minFilter);
            gl2.TexParameteri(TEXTURE_CUBE_MAP, TEXTURE_WRAP_S, glWrapMode);
            gl2.TexParameteri(TEXTURE_CUBE_MAP, TEXTURE_WRAP_T, glWrapMode);
            gl2.TexParameteri(TEXTURE_CUBE_MAP, TEXTURE_WRAP_R, glWrapMode);

            uint err = gl2.GetError();
            if (err != 0)
                throw new ArgumentException($"Create Texture gl2 error {err}, Format {img.PixelFormat.ColorFormat}, BPP {img.PixelFormat.BytesPerPixel}, {pxInfo.InternalFormat}");


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
            WebGLTexture id = gl2.CreateTexture();

            gl2.BindTexture(TEXTURE_2D, id);

            Tuple<int, int> glMinMagFilter = GetMinMagFilter(tex.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var maxFilter = glMinMagFilter.Item2;

            int glWrapMode = GetWrapMode(tex.WrapMode);
            TexturePixelInfo pxInfo = GetTexturePixelInfo(tex.ImageData.PixelFormat);

            gl2.PixelStorei(UNPACK_ALIGNMENT, pxInfo.RowAlignment);
            gl2.TexImage2D(TEXTURE_2D, 0, (int)pxInfo.InternalFormat, tex.ImageData.Width, tex.ImageData.Height, 0, pxInfo.Format, pxInfo.PxType, tex.ImageData.PixelData);

            if (tex.DoGenerateMipMaps && tex.ImageData.PixelFormat.ColorFormat != ColorFormat.Intensity)
                gl2.GenerateMipmap(TEXTURE_2D);

            gl2.TexParameteri(TEXTURE_2D, TEXTURE_MAG_FILTER, minFilter);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_MIN_FILTER, maxFilter);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_S, glWrapMode);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_T, glWrapMode);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_R, glWrapMode);

            uint err = gl2.GetError();
            if (err != 0)
                throw new ArgumentException($"Create Texture ITexture gl2 error {err}, Format {tex.ImageData.PixelFormat.ColorFormat}, BPP {tex.ImageData.PixelFormat.BytesPerPixel}, {pxInfo.InternalFormat}");


            ITextureHandle texID = new TextureHandle { TexId = id };
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
            WebGLTexture id = gl2.CreateTexture();
            gl2.BindTexture(TEXTURE_2D, id);

            Tuple<int, int> glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;

            int glWrapMode = GetWrapMode(img.WrapMode);
            TexturePixelInfo pxInfo = GetTexturePixelInfo(img.PixelFormat);

            Array imgData = GetEmptyArray(img);

            gl2.TexImage2D(TEXTURE_2D, 0, (int)pxInfo.InternalFormat, img.Width, img.Height, 0, pxInfo.Format, pxInfo.PxType, imgData);

            if (img.DoGenerateMipMaps)
                gl2.GenerateMipmap(TEXTURE_2D);

            gl2.TexParameteri(TEXTURE_2D, TEXTURE_COMPARE_MODE, (int)GetTexCompareMode(img.CompareMode));
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_COMPARE_FUNC, (int)GetDepthCompareFunc(img.CompareFunc));
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_MAG_FILTER, minFilter);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_MIN_FILTER, magFilter);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_S, glWrapMode);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_T, glWrapMode);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_R, glWrapMode);

            ITextureHandle texID = new TextureHandle { TexId = id };

            uint err = gl2.GetError();
            if (err != 0)
                throw new ArgumentException($"Create Texture IWritableTexture gl2 error {err}, Format {img.PixelFormat.ColorFormat}, BPP {img.PixelFormat.BytesPerPixel}, {pxInfo.InternalFormat}, {img.Height}, {img.Width}");

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
            TexturePixelInfo info = GetTexturePixelInfo(img.ImageData.PixelFormat);
            uint format = info.Format;
            uint pxType = info.PxType;

            // copy the bytes from image to GPU texture
            int bytesTotal = width * height * img.ImageData.PixelFormat.BytesPerPixel;
            IEnumerator<ScanLine> scanlines = img.ImageData.ScanLines(startX, startY, width, height);
            byte[] bytes = new byte[bytesTotal];
            int offset = 0;
            do
            {
                if (scanlines.Current != null)
                {
                    byte[] lineBytes = scanlines.Current.GetScanLineBytes();
                    System.Buffer.BlockCopy(lineBytes, 0, bytes, offset, lineBytes.Length);
                    offset += lineBytes.Length;
                }

            } while (scanlines.MoveNext());

            TexturePixelInfo pxInfo = GetTexturePixelInfo(img.ImageData.PixelFormat);
            gl2.PixelStorei(UNPACK_ALIGNMENT, pxInfo.RowAlignment);
            gl2.BindTexture(TEXTURE_2D, ((TextureHandle)tex).TexId);
            gl2.TexSubImage2D(TEXTURE_2D, 0, startX, startY, width, height, format, pxType, bytes);

            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            gl2.TexParameteri(TEXTURE_2D, TEXTURE_MIN_FILTER, (int)NEAREST);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_MAG_FILTER, (int)NEAREST);
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to a frame-buffer object.
        /// </summary>
        /// <param name="bh">The platform dependent abstraction of the gpu buffer handle.</param>
        public void DeleteFrameBuffer(IBufferHandle bh)
        {
            gl2.DeleteFramebuffer(((FrameBufferHandle)bh).Handle);
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to a render-buffer object.
        /// </summary>
        /// <param name="bh">The platform dependent abstraction of the gpu buffer handle.</param>
        public void DeleteRenderBuffer(IBufferHandle bh)
        {
            gl2.DeleteRenderbuffer(((RenderBufferHandle)bh).Handle);
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to the given <see cref="ITextureHandle"/>.
        /// </summary>
        /// <param name="textureHandle">The <see cref="ITextureHandle"/> which gpu allocated memory will be freed.</param>
        public void RemoveTextureHandle(ITextureHandle textureHandle)
        {
            TextureHandle texHandle = (TextureHandle)textureHandle;

            if (texHandle.FrameBufferHandle != null)
            {
                gl2.DeleteFramebuffer(texHandle.FrameBufferHandle);
            }

            if (texHandle.DepthRenderBufferHandle != null)
            {
                gl2.DeleteRenderbuffer(texHandle.DepthRenderBufferHandle);
            }

            if (texHandle.TexId != null)
            {
                gl2.DeleteTexture(texHandle.TexId);
            }
        }

        #endregion

        #region Shader related Members

        /// <summary>
        /// Creates the shader program by using a valid GLSL vertex and fragment shader code. This code is compiled at runtime.
        /// Do not use this function in frequent updates.
        /// </summary>
        /// <param name="vs">The vertex shader code.</param>
        /// <param name="gs">The vertex shader code.</param>
        /// <param name="ps">The pixel(=fragment) shader code.</param>
        /// <returns>An instance of <see cref="IShaderHandle" />.</returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public IShaderHandle CreateShaderProgram(string vs, string ps, string gs = null)
        {
            if (gs != null)
                Diagnostics.Warn("Geometry Shaders are unsupported");

            if (vs == null || ps == null)
            {
                Diagnostics.Error("Pixel or vertex shader empty");
                throw new ArgumentException("Pixel or vertex shader empty");
            }
            string info;

            WebGLShader vertexObject = gl2.CreateShader(VERTEX_SHADER);
            WebGLShader fragmentObject = gl2.CreateShader(FRAGMENT_SHADER);

            // Compile vertex shader
            gl2.ShaderSource(vertexObject, vs);
            gl2.CompileShader(vertexObject);
            info = gl2.GetShaderInfoLog(vertexObject);

            if (info != string.Empty)
                throw new ApplicationException(info);

            // Compile pixel shader
            gl2.ShaderSource(fragmentObject, ps);
            gl2.CompileShader(fragmentObject);
            info = gl2.GetShaderInfoLog(fragmentObject);

            if (info != string.Empty)
                throw new ApplicationException(info);

            WebGLProgram program = gl2.CreateProgram();
            gl2.AttachShader(program, fragmentObject);
            gl2.AttachShader(program, vertexObject);

            // enable GLSL (ES) shaders to use fuVertex, fuColor and fuNormal attributes
            gl2.BindAttribLocation(program, (uint)AttributeLocations.VertexAttribLocation, UniformNameDeclarations.Vertex);
            gl2.BindAttribLocation(program, (uint)AttributeLocations.ColorAttribLocation, UniformNameDeclarations.VertexColor);
            gl2.BindAttribLocation(program, (uint)AttributeLocations.Color1AttribLocation, UniformNameDeclarations.VertexColor1);
            gl2.BindAttribLocation(program, (uint)AttributeLocations.Color2AttribLocation, UniformNameDeclarations.VertexColor2);
            gl2.BindAttribLocation(program, (uint)AttributeLocations.UvAttribLocation, UniformNameDeclarations.TextureCoordinates);
            gl2.BindAttribLocation(program, (uint)AttributeLocations.NormalAttribLocation, UniformNameDeclarations.Normal);
            gl2.BindAttribLocation(program, (uint)AttributeLocations.TangentAttribLocation, UniformNameDeclarations.Tangent);
            gl2.BindAttribLocation(program, (uint)AttributeLocations.BoneIndexAttribLocation, UniformNameDeclarations.BoneIndex);
            gl2.BindAttribLocation(program, (uint)AttributeLocations.BoneWeightAttribLocation, UniformNameDeclarations.BoneWeight);
            gl2.BindAttribLocation(program, (uint)AttributeLocations.BitangentAttribLocation, UniformNameDeclarations.Bitangent);
            gl2.BindAttribLocation(program, (uint)AttributeLocations.InstancedColor, UniformNameDeclarations.InstanceColor);
            gl2.BindAttribLocation(program, (uint)AttributeLocations.InstancedModelMat1, UniformNameDeclarations.InstanceModelMat);
            gl2.BindAttribLocation(program, (uint)AttributeLocations.FlagsAttribLocation, UniformNameDeclarations.Flags);

            gl2.LinkProgram(program);

            return new ShaderHandle { Handle = program };
        }

        /// <inheritdoc />
        /// <summary>
        /// Removes shader from the GPU
        /// </summary>
        /// <param name="sp"></param>
        public void RemoveShader(IShaderHandle sp)
        {
            if (sp == null) return;

            WebGLProgram program = ((ShaderHandle)sp).Handle;

            // wait for all threads to be finished
            gl2.Finish();
            gl2.Flush();

            // cleanup
            // gl2.DeleteShader(program);
            gl2.DeleteProgram(program);
        }

        /// <summary>
        /// Sets the shader program onto the GL render context.
        /// </summary>
        /// <param name="program">The shader program.</param>
        public void SetShader(IShaderHandle program)
        {
            _textureCountPerShader = 0;
            _shaderParam2TexUnit.Clear();

            gl2.UseProgram(((ShaderHandle)program).Handle);
        }

        /// <summary>
        /// Set the width of line primitives.
        /// </summary>
        /// <param name="width"></param>
        public void SetLineWidth(float width)
        {
            gl2.LineWidth(width);
        }

        /// <summary>
        /// Gets the shader parameter.
        /// The Shader parameter is used to bind values inside of shader programs that run on the graphics card.
        /// Do not use this function in frequent updates as it transfers information from graphics card to the cpu which takes time.
        /// </summary>
        /// <param name="shaderProgram">The shader program.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>The Shader parameter is returned if the name is found, otherwise null.</returns>
        public IUniformHandle GetShaderParam(IShaderHandle shaderProgram, string paramName)
        {
            WebGLUniformLocation h = gl2.GetUniformLocation(((ShaderHandle)shaderProgram).Handle, paramName);
            return (h == null) ? null : new UniformHandle { handle = h };
        }

        /// <summary>
        /// Gets the float parameter value inside a shader program by using a <see cref="IUniformHandle" /> as search reference.
        /// Do not use this function in frequent updates as it transfers information from the graphics card to the cpu which takes time.
        /// </summary>
        /// <param name="program">The shader program.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>The current parameter's value.</returns>
        public float GetParamValue(IShaderHandle program, IUniformHandle param)
        {
            float f = (float)gl2.GetUniform(((ShaderHandle)program).Handle, ((UniformHandle)param).handle);
            return f;
        }


        /// <summary>
        /// Gets the shader parameter list of a specific <see cref="IShaderHandle" />.
        /// </summary>
        /// <param name="shaderProgram">The shader program.</param>
        /// <returns>All Shader parameters of a shader program are returned.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IList<IActiveUniform> GetActiveUniformsList(IShaderHandle shaderProgram)
        {
            ShaderHandle sProg = (ShaderHandle)shaderProgram;
            List<IActiveUniform> paramList = new();

            int nParams = gl2.GetProgramParameter(sProg.Handle, ACTIVE_UNIFORMS);

            for (uint i = 0; i < nParams; i++)
            {
                WebGLActiveInfo activeInfo = gl2.GetActiveUniform(sProg.Handle, i);

                var paramInfo = new ActiveUniform()
                {
                    Name = activeInfo.Name,
                    Size = activeInfo.Size,
                    HasValueChanged = true
                };
                //uint uType = activeInfo.Type;//activeInfo.GlType;
                paramInfo.Handle = GetShaderParam(sProg, paramInfo.Name);
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
            gl2.Uniform1f(((UniformHandle)param).handle, val);
        }

        /// <summary>
        /// Sets a <see cref="float2" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, float2 val)
        {
            gl2.Uniform2f(((UniformHandle)param).handle, val.x, val.y);
        }


        /// <summary>
        /// Sets a <see cref="float2" /> array as shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, float2[] val)
        {
            var res = new float[val.Length * 2];

            for (var i = 0; i < val.Length; i++)
            {
                res[i * 2] = val[i].x;
                res[i * 2 + 1] = val[i].y;
            }

            gl2.Uniform2fv(((UniformHandle)param).handle, res, 0, (uint)res.Length);
        }


        /// <summary>
        /// Sets a <see cref="float3" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, float3 val)
        {
            gl2.Uniform3f(((UniformHandle)param).handle, val.x, val.y, val.z);
        }

        /// <summary>
        /// Sets a <see cref="float3" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, float3[] val)
        {
            var res = new float[val.Length * 3];
            for (var i = 0; i < val.Length; i++)
            {
                res[i] = val[i].x;
                res[i + 1] = val[i].y;
                res[i + 2] = val[i].z;
            }
            gl2.Uniform3fv(((UniformHandle)param).handle, res, 0, (uint)res.Length);
        }

        /// <summary>
        /// Sets a <see cref="float4" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, float4 val)
        {
            gl2.Uniform4f(((UniformHandle)param).handle, val.x, val.y, val.z, val.w);
        }


        /// <summary>
        /// Sets a <see cref="float4x4" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, float4x4 val)
        {
            gl2.UniformMatrix4fv(((UniformHandle)param).handle, true, val.ToArray());

        }

        /// <summary>
        ///     Sets a <see cref="float4" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, float4[] val)
        {
            var res = new float[val.Length * 4];

            for (var i = 0; i < val.Length; i++)
            {
                res[i * 4] = val[i].x;
                res[i * 4 + 1] = val[i].y;
                res[i * 4 + 2] = val[i].z;
                res[i * 4 + 3] = val[i].w;
            }

            gl2.Uniform4fv(((UniformHandle)param).handle, res, 0, (uint)res.Length);
        }

        /// <summary>
        /// Sets a <see cref="float4x4" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, float4x4[] val)
        {
            float4[] tmpArray = new float4[val.Length * 4];

            for (int i = 0; i < val.Length; i++)
            {
                tmpArray[i * 4] = val[i].Column1;
                tmpArray[i * 4 + 1] = val[i].Column2;
                tmpArray[i * 4 + 2] = val[i].Column3;
                tmpArray[i * 4 + 3] = val[i].Column4;
            }

            var res = new float[tmpArray.Length * 4];

            for (var i = 0; i < tmpArray.Length; i++)
            {
                res[i * 4] = tmpArray[i].x;
                res[i * 4 + 1] = tmpArray[i].y;
                res[i * 4 + 2] = tmpArray[i].z;
                res[i * 4 + 3] = tmpArray[i].w;
            }

            gl2.UniformMatrix4fv(((UniformHandle)param).handle, false, res.ToArray(), 0, (uint)res.Length);
        }

        /// <summary>
        /// Sets a <see cref="int2" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, int2 val)
        {
            gl2.Uniform2i(((UniformHandle)param).handle, val.x, val.y);
        }

        /// <summary>
        /// Sets a int shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IUniformHandle param, int val)
        {
            gl2.Uniform1i(((UniformHandle)param).handle, val);
        }

        private void BindTextureByTarget(ITextureHandle texId, TextureType texTarget)
        {
            switch (texTarget)
            {
                case TextureType.Texture1D:
                    Diagnostics.Error("OpenTK ES31 does not support Texture1D.");
                    break;
                case TextureType.Texture2D:
                    gl2.BindTexture(TEXTURE_2D, ((TextureHandle)texId).TexId);
                    break;
                case TextureType.Texture3D:
                    gl2.BindTexture(TEXTURE_3D, ((TextureHandle)texId).TexId);
                    break;
                case TextureType.TextureCubeMap:
                    gl2.BindTexture(TEXTURE_CUBE_MAP, ((TextureHandle)texId).TexId);
                    break;
                case TextureType.ArrayTexture:
                    //Console.WriteLine("Binding array tex");
                    gl2.BindTexture(TEXTURE_2D_ARRAY, ((TextureHandle)texId).TexId);
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
        public void SetActiveAndBindTexture(IUniformHandle param, ITextureHandle texId, TextureType texTarget)
        {

            WebGLUniformLocation iParam = ((UniformHandle)param).handle;
            if (!_shaderParam2TexUnit.TryGetValue(iParam, out int texUnit))
            {
                _textureCountPerShader++;
                texUnit = _textureCountPerShader;
                _shaderParam2TexUnit[iParam] = texUnit;
            }

            gl2.ActiveTexture((uint)(TEXTURE0 + texUnit));
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
            WebGLUniformLocation iParam = ((UniformHandle)param).handle;
            if (!_shaderParam2TexUnit.TryGetValue(iParam, out texUnit))
            {
                _textureCountPerShader++;
                texUnit = _textureCountPerShader;
                _shaderParam2TexUnit[iParam] = texUnit;
            }

            gl2.ActiveTexture((uint)(TEXTURE0 + texUnit));
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
            WebGLUniformLocation iParam = ((UniformHandle)param).handle;
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

                gl2.ActiveTexture((uint)(TEXTURE0 + firstTexUnit + i));
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
            WebGLUniformLocation iParam = ((UniformHandle)param).handle;
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

                gl2.ActiveTexture((uint)(TEXTURE0 + firstTexUnit + i));

                BindTextureByTarget(texIds[i], texTarget);
            }

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
            gl2.Uniform1i(((UniformHandle)param).handle, texUnit);
        }

        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texIds">An array of ITextureHandles probably returned from CreateTexture method</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        public void SetShaderParamTextureArray(IUniformHandle param, ITextureHandle[] texIds, TextureType texTarget)
        {
            SetActiveAndBindTextureArray(param, texIds, texTarget, out int[] texUnitArray);
            gl2.Uniform1i(((UniformHandle)param).handle, texUnitArray[0]);
        }

        #endregion

        #region Clear

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
                float[] retArr = gl2.GetClearColor();
                return new float4(retArr[0], retArr[1], retArr[2], retArr[3]);
            }
            set => gl2.ClearColor(value.x, value.y, value.z, value.w);
        }

        /// <summary>
        /// Gets and sets the clear depth value which is used to clear the depth buffer.
        /// </summary>
        /// <value>
        /// Specifies the depth value used when the depth buffer is cleared. The initial value is 1. This value is clamped to the range [0,1].
        /// </value>
        public float ClearDepth
        {
            get => gl2.GetParameter(DEPTH_CLEAR_VALUE);
            set => gl2.ClearDepth(value);
        }

        /// <summary>
        /// Returns the current underlying rendering platform
        /// </summary>
        public FuseePlatformId FuseePlatformId => FuseePlatformId.Blazor;

        /// <summary>
        /// Clears the specified flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
        public void Clear(ClearFlags flags)
        {
            // ACCUM is ignored in Webgl2...
            uint wglFlags =
                  ((flags & ClearFlags.Depth) != 0 ? DEPTH_BUFFER_BIT : 0)
                | ((flags & ClearFlags.Stencil) != 0 ? STENCIL_BUFFER_BIT : 0)
                | ((flags & ClearFlags.Color) != 0 ? COLOR_BUFFER_BIT : 0);
            gl2.Clear(wglFlags);
        }

        #endregion

        #region Rendering related Members

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
            gl2.Scissor(x, y, width, height);
        }

        /// <summary>
        /// The clipping behavior against the Z position of a vertex can be turned off by activating depth clamping.
        /// This is done with glEnable(GL_DEPTH_CLAMP). This will cause the clip-space Z to remain unclipped by the front and rear viewing volume.
        /// See: https://www.khronos.org/opengl/wiki/Vertex_Post-Processing#Depth_clamping
        /// </summary>
        public void EnableDepthClamp()
        {
            //gl2.Enable(DEPTH_RANGE)
            //throw new NotImplementedException("Depth clamping isn't implemented yet!");
        }

        /// <summary>
        /// Disables depths clamping. <seealso cref="EnableDepthClamp"/>
        /// </summary>
        public void DisableDepthClamp()
        {
            throw new NotImplementedException("Depth clamping isn't implemented yet!");
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
            WebGLBuffer handle = gl2.CreateBuffer();

            float[] attribFlat = new float[attributes.Length * 3];
            int i = 0;
            foreach (float3 v in attributes)
            {
                attribFlat[i] = v.x;
                attribFlat[i + 1] = v.y;
                attribFlat[i + 2] = v.z;
                i += 3;
            }

            gl2.BindBuffer(ARRAY_BUFFER, handle);
            gl2.BufferData(ARRAY_BUFFER, attribFlat, STATIC_DRAW);
            vboBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != vertsBytes)
            {
                throw new ApplicationException(string.Format(
                    "Problem uploading attribute buffer to VBO ('{2}'). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes, attributeName));
            }

            gl2.BindBuffer(ARRAY_BUFFER, null);

            return new AttributeImp { AttributeBufferObject = handle };
        }

        /// <summary>
        /// Remove an attribute buffer previously created with <see cref="CreateAttributeBuffer"/> and release all associated resources
        /// allocated on the GPU.
        /// </summary>
        /// <param name="attribHandle">The attribute handle.</param>
        public void DeleteAttributeBuffer(IAttribImp attribHandle)
        {
            if (attribHandle != null)
            {
                WebGLBuffer handle = ((AttributeImp)attribHandle).AttributeBufferObject;
                if (handle != null)
                {
                    gl2.DeleteBuffer(handle);
                    ((AttributeImp)attribHandle).AttributeBufferObject = null;
                }
            }
        }

        /// <summary>
        /// Binds the vertices onto the GL render context and assigns an VertexBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int vboBytes;
            int vertsBytes = vertices.Length * 3 * sizeof(float);
            if (((MeshImp)mr).VertexBufferObject == null)
                ((MeshImp)mr).VertexBufferObject = gl2.CreateBuffer();

            gl2.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).VertexBufferObject);
            gl2.VertexAttribPointer((uint)AttributeLocations.VertexAttribLocation, 3, FLOAT, false, 0, 0);

            float[] verticesFlat = new float[vertices.Length * 3];
            int i = 0;
            foreach (float3 v in vertices)
            {
                verticesFlat[i] = v.x;
                verticesFlat[i + 1] = v.y;
                verticesFlat[i + 2] = v.z;
                i += 3;
            }

            gl2.BufferData(ARRAY_BUFFER, verticesFlat, STATIC_DRAW);

            vboBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(string.Format("Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.", vertsBytes, vboBytes));

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
            if (vao == null)
            {
                throw new ApplicationException("Create the VAO first!");
            }

            var sizeOfFloat4 = sizeof(float) * 4;
            var sizeOfMat = sizeOfFloat4 * 4;
            var amount = instanceImp.Amount;
            int matBytes = amount * sizeOfMat;

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

            var posBufferData = new float[amount * 16];
            for (var i = 0; i < modelMats.Length; i++)
            {
                posBufferData[i * 16] = modelMats[i].M11;
                posBufferData[i * 16 + 1] = modelMats[i].M21;
                posBufferData[i * 16 + 2] = modelMats[i].M31;
                posBufferData[i * 16 + 3] = modelMats[i].M41;

                posBufferData[i * 16 + 4] = modelMats[i].M12;
                posBufferData[i * 16 + 5] = modelMats[i].M22;
                posBufferData[i * 16 + 6] = modelMats[i].M32;
                posBufferData[i * 16 + 7] = modelMats[i].M42;

                posBufferData[i * 16 + 8] = modelMats[i].M13;
                posBufferData[i * 16 + 9] = modelMats[i].M23;
                posBufferData[i * 16 + 10] = modelMats[i].M33;
                posBufferData[i * 16 + 11] = modelMats[i].M43;

                posBufferData[i * 16 + 12] = modelMats[i].M14;
                posBufferData[i * 16 + 13] = modelMats[i].M24;
                posBufferData[i * 16 + 14] = modelMats[i].M34;
                posBufferData[i * 16 + 15] = modelMats[i].M44;
            }

            WebGLBuffer instanceTransformBo;
            if (((InstanceDataImp)instanceImp).InstanceTransformBufferObject == null)
            {
                instanceTransformBo = gl2.CreateBuffer();
                ((InstanceDataImp)instanceImp).InstanceTransformBufferObject = instanceTransformBo;
                gl2.BindBuffer(ARRAY_BUFFER, instanceTransformBo);
                gl2.BufferData(ARRAY_BUFFER, posBufferData, DYNAMIC_DRAW);
            }
            else
            {
                instanceTransformBo = ((InstanceDataImp)instanceImp).InstanceTransformBufferObject;
                gl2.BindBuffer(ARRAY_BUFFER, instanceTransformBo);
                gl2.BufferSubData(ARRAY_BUFFER, IntPtr.Zero, posBufferData);
            }

            var instancedPosBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (instancedPosBytes != matBytes)
                throw new ApplicationException(string.Format("Problem uploading normal buffer to VBO. Tried to upload {0} bytes, uploaded {1}.", instancedPosBytes, matBytes));

            gl2.BindVertexArray(((InstanceDataImp)instanceImp).VertexArrayObject);
            // set attribute pointers for matrix (4 times vec4)
            gl2.EnableVertexAttribArray((uint)AttributeLocations.InstancedModelMat1);
            gl2.VertexAttribPointer((uint)AttributeLocations.InstancedModelMat1, 4, FLOAT, false, sizeOfMat, 0);
            gl2.EnableVertexAttribArray((uint)AttributeLocations.InstancedModelMat2);
            gl2.VertexAttribPointer((uint)AttributeLocations.InstancedModelMat2, 4, FLOAT, false, sizeOfMat, (uint)(1 * sizeOfFloat4));
            gl2.EnableVertexAttribArray((uint)AttributeLocations.InstancedModelMat3);
            gl2.VertexAttribPointer((uint)AttributeLocations.InstancedModelMat3, 4, FLOAT, false, sizeOfMat, (uint)(2 * sizeOfFloat4));
            gl2.EnableVertexAttribArray((uint)AttributeLocations.InstancedModelMat4);
            gl2.VertexAttribPointer((uint)AttributeLocations.InstancedModelMat4, 4, FLOAT, false, sizeOfMat, (uint)(3 * sizeOfFloat4));

            gl2.VertexAttribDivisor((uint)AttributeLocations.InstancedModelMat1, 1);
            gl2.VertexAttribDivisor((uint)AttributeLocations.InstancedModelMat2, 1);
            gl2.VertexAttribDivisor((uint)AttributeLocations.InstancedModelMat3, 1);
            gl2.VertexAttribDivisor((uint)AttributeLocations.InstancedModelMat4, 1);
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

            float[] colorsFlat = new float[instanceColors.Length * 4];
            int i = 0;
            foreach (float4 v in instanceColors)
            {
                colorsFlat[i] = v.x;
                colorsFlat[i + 1] = v.y;
                colorsFlat[i + 2] = v.z;
                colorsFlat[i + 3] = v.w;
                i += 4;
            }

            int vboBytes;
            int colsBytes = instanceColors.Length * 4 * sizeof(float);
            if (((InstanceDataImp)instanceImp).InstanceColorBufferObject == null)
            {
                ((InstanceDataImp)instanceImp).InstanceColorBufferObject = gl2.CreateBuffer();
                gl2.BindBuffer(ARRAY_BUFFER, ((InstanceDataImp)instanceImp).InstanceColorBufferObject);
                gl2.BufferData(ARRAY_BUFFER, colorsFlat, DYNAMIC_DRAW);
            }
            else
            {
                gl2.BindBuffer(ARRAY_BUFFER, ((InstanceDataImp)instanceImp).InstanceColorBufferObject);
                gl2.BufferSubData(ARRAY_BUFFER, IntPtr.Zero, colorsFlat);
            }

            vboBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != colsBytes)
                throw new ApplicationException(string.Format("Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.", colsBytes, vboBytes));

            gl2.BindVertexArray(((InstanceDataImp)instanceImp).VertexArrayObject);
            // set attribute pointers for matrix (4 times vec4)
            gl2.EnableVertexAttribArray((uint)AttributeLocations.InstancedColor);
            gl2.VertexAttribPointer((uint)AttributeLocations.InstancedColor, 4, FLOAT, false, 4 * sizeof(float), 0);
            gl2.VertexAttribDivisor((uint)AttributeLocations.InstancedColor, 1);
        }

        /// <summary>
        /// Binds the tangents onto the GL render context and assigns an TangentBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int vboBytes;
            int tangentBytes = tangents.Length * 4 * sizeof(float);
            if (((MeshImp)mr).TangentBufferObject == null)
                ((MeshImp)mr).TangentBufferObject = gl2.CreateBuffer(); ;

            gl2.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).TangentBufferObject);
            gl2.VertexAttribPointer((uint)AttributeLocations.TangentAttribLocation, 3, FLOAT, false, 0, 0);

            float[] tangentsFlat = new float[tangents.Length * 4];

            int i = 0;
            foreach (float4 v in tangents)
            {
                tangentsFlat[i] = v.x;
                tangentsFlat[i + 1] = v.y;
                tangentsFlat[i + 2] = v.z;
                tangentsFlat[i + 3] = v.w;
                i += 4;
            }


            gl2.BufferData(ARRAY_BUFFER, tangentsFlat, STATIC_DRAW);
            vboBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != tangentBytes)
                throw new ApplicationException(string.Format("Problem uploading vertex buffer to VBO (tangents). Tried to upload {0} bytes, uploaded {1}.", tangentBytes, vboBytes));

        }

        /// <summary>
        /// Binds the bitangents onto the GL render context and assigns an BiTangentBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int vboBytes;
            int bitangentBytes = bitangents.Length * 3 * sizeof(float);
            if (((MeshImp)mr).BitangentBufferObject == null)
                ((MeshImp)mr).BitangentBufferObject = gl2.CreateBuffer();

            gl2.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).BitangentBufferObject);
            gl2.VertexAttribPointer((uint)AttributeLocations.BitangentAttribLocation, 3, FLOAT, false, 0, 0);

            float[] bitangentsFlat = new float[bitangents.Length * 3];

            //{
            ////fixed(float3* pBytes = &bitangents[0])
            //{
            //Marshal.Copy((IntPtr)(pBytes), bitangentsFlat, 0, bitangentsFlat.Length);
            //}
            //}

            int i = 0;
            foreach (float3 v in bitangents)
            {
                bitangentsFlat[i] = v.x;
                bitangentsFlat[i + 1] = v.y;
                bitangentsFlat[i + 2] = v.z;
                i += 3;
            }

            gl2.BufferData(ARRAY_BUFFER, bitangentsFlat, STATIC_DRAW);
            vboBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != bitangentBytes)
                throw new ApplicationException(string.Format("Problem uploading vertex buffer to VBO (bitangents). Tried to upload {0} bytes, uploaded {1}.", bitangentBytes, vboBytes));
        }

        /// <summary>
        /// Binds the normals onto the GL render context and assigns an NormalBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int vboBytes;
            int normsBytes = normals.Length * 3 * sizeof(float);
            if (((MeshImp)mr).NormalBufferObject == null)
                ((MeshImp)mr).NormalBufferObject = gl2.CreateBuffer();

            gl2.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).NormalBufferObject);
            gl2.VertexAttribPointer((uint)AttributeLocations.NormalAttribLocation, 3, FLOAT, false, 0, 0);

            float[] normalsFlat = new float[normals.Length * 3];

            //{
            ////fixed(float3* pBytes = &normals[0])
            //{
            //Marshal.Copy((IntPtr)(pBytes), normalsFlat, 0, normalsFlat.Length);
            //}
            //}

            int i = 0;
            foreach (float3 v in normals)
            {
                normalsFlat[i] = v.x;
                normalsFlat[i + 1] = v.y;
                normalsFlat[i + 2] = v.z;
                i += 3;
            }

            gl2.BufferData(ARRAY_BUFFER, normalsFlat, STATIC_DRAW);

            vboBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != normsBytes)
                throw new ApplicationException(string.Format("Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.", normsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the bone indices onto the GL render context and assigns an BondeIndexBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int vboBytes;
            int indicesBytes = boneIndices.Length * 4 * sizeof(float);
            if (((MeshImp)mr).BoneIndexBufferObject == null)
                ((MeshImp)mr).BoneIndexBufferObject = gl2.CreateBuffer();

            gl2.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).BoneIndexBufferObject);
            gl2.VertexAttribPointer((uint)AttributeLocations.BoneIndexAttribLocation, 4, FLOAT, false, 0, 0);

            float[] boneIndicesFlat = new float[boneIndices.Length * 4];

            //{
            ////fixed(float4* pBytes = &boneIndices[0])
            //{
            //Marshal.Copy((IntPtr)(pBytes), boneIndicesFlat, 0, boneIndicesFlat.Length);
            //}
            //}

            int i = 0;
            foreach (float4 v in boneIndices)
            {
                boneIndicesFlat[i] = v.x;
                boneIndicesFlat[i + 1] = v.y;
                boneIndicesFlat[i + 2] = v.z;
                boneIndicesFlat[i + 3] = v.w;
                i += 4;
            }

            gl2.BufferData(ARRAY_BUFFER, boneIndicesFlat, STATIC_DRAW);
            vboBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != indicesBytes)
                throw new ApplicationException(string.Format("Problem uploading bone indices buffer to VBO (bone indices). Tried to upload {0} bytes, uploaded {1}.", indicesBytes, vboBytes));
        }

        /// <summary>
        /// Binds the bone weights onto the GL render context and assigns an BondeWeightBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int vboBytes;
            int weightsBytes = boneWeights.Length * 4 * sizeof(float);
            if (((MeshImp)mr).BoneWeightBufferObject == null)
                ((MeshImp)mr).BoneWeightBufferObject = gl2.CreateBuffer();

            gl2.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).BoneWeightBufferObject);
            gl2.VertexAttribPointer((uint)AttributeLocations.BoneWeightAttribLocation, 4, FLOAT, false, 0, 0);

            float[] boneWeightsFlat = new float[boneWeights.Length * 4];

            //{
            ////fixed(float4* pBytes = &boneWeights[0])
            //{
            //Marshal.Copy((IntPtr)(pBytes), boneWeightsFlat, 0, boneWeightsFlat.Length);
            //}
            //}

            int i = 0;
            foreach (float4 v in boneWeights)
            {
                boneWeightsFlat[i] = v.x;
                boneWeightsFlat[i + 1] = v.y;
                boneWeightsFlat[i + 2] = v.z;
                boneWeightsFlat[i + 3] = v.w;
                i += 4;
            }

            gl2.BufferData(ARRAY_BUFFER, boneWeightsFlat, STATIC_DRAW);
            vboBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != weightsBytes)
                throw new ApplicationException(string.Format("Problem uploading bone weights buffer to VBO (bone weights). Tried to upload {0} bytes, uploaded {1}.", weightsBytes, vboBytes));

        }

        /// <summary>
        /// Binds the UV coordinates onto the GL render context and assigns an UVBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int vboBytes;
            int uvsBytes = uvs.Length * 2 * sizeof(float);
            if (((MeshImp)mr).UVBufferObject == null)
                ((MeshImp)mr).UVBufferObject = gl2.CreateBuffer();

            gl2.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).UVBufferObject);
            gl2.VertexAttribPointer((uint)AttributeLocations.UvAttribLocation, 2, FLOAT, false, 0, 0);

            float[] uvsFlat = new float[uvs.Length * 2];

            //{
            ////fixed(float2* pBytes = &uvs[0])
            //{
            //Marshal.Copy((IntPtr)(pBytes), uvsFlat, 0, uvsFlat.Length);
            //}
            //}

            int i = 0;
            foreach (float2 v in uvs)
            {
                uvsFlat[i] = v.x;
                uvsFlat[i + 1] = v.y;
                i += 2;
            }

            gl2.BufferData(ARRAY_BUFFER, uvsFlat, STATIC_DRAW);
            vboBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != uvsBytes)
                throw new ApplicationException(string.Format("Problem uploading uv buffer to VBO (uvs). Tried to upload {0} bytes, uploaded {1}.", uvsBytes, vboBytes));

        }

        /// <summary>
        /// Binds the colors onto the GL render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int vboBytes;
            int colsBytes = colors.Length * sizeof(uint);
            if (((MeshImp)mr).ColorBufferObject == null)
                ((MeshImp)mr).ColorBufferObject = gl2.CreateBuffer();

            gl2.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).ColorBufferObject);
            gl2.VertexAttribPointer((uint)AttributeLocations.ColorAttribLocation, 4, UNSIGNED_BYTE, true, 0, 0);
            gl2.BufferData(ARRAY_BUFFER, colors.ToArray(), STATIC_DRAW);
            vboBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != colsBytes)
                throw new ApplicationException(string.Format("Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.", colsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the second set of colors onto the GL render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int vboBytes;
            int colsBytes = colors.Length * sizeof(uint);
            if (((MeshImp)mr).Color1BufferObject == null)
                ((MeshImp)mr).Color1BufferObject = gl2.CreateBuffer();

            gl2.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).Color1BufferObject);
            gl2.VertexAttribPointer((uint)AttributeLocations.Color1AttribLocation, 4, UNSIGNED_BYTE, true, 0, 0);
            gl2.BufferData(ARRAY_BUFFER, colors.ToArray(), STATIC_DRAW);
            vboBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != colsBytes)
                throw new ApplicationException(string.Format("Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.", colsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the third set of colors onto the GL render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            int vboBytes;
            int colsBytes = colors.Length * sizeof(uint);
            if (((MeshImp)mr).Color2BufferObject == null)
                ((MeshImp)mr).Color2BufferObject = gl2.CreateBuffer();

            gl2.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).Color2BufferObject);
            gl2.VertexAttribPointer((uint)AttributeLocations.Color2AttribLocation, 4, UNSIGNED_BYTE, true, 0, 0);
            gl2.BufferData(ARRAY_BUFFER, colors.ToArray(), STATIC_DRAW);
            vboBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != colsBytes)
                throw new ApplicationException(string.Format("Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.", colsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the lags onto the GL render context and assigns an buffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="flags">The flags.</param>
        /// <exception cref="ArgumentException">colors must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetFlags(IMeshImp mr, ReadOnlySpan<uint> flags)
        {
            if (flags == null || flags.Length == 0)
            {
                throw new ArgumentException("Flags must not be null or empty");
            }

            int vboBytes;
            int flagsBytes = flags.Length * sizeof(uint);
            if (((MeshImp)mr).FlagsBufferObject == null)
                ((MeshImp)mr).FlagsBufferObject = gl2.CreateBuffer();

            gl2.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).FlagsBufferObject);
            gl2.VertexAttribIPointer((uint)AttributeLocations.FlagsAttribLocation, 1, UNSIGNED_INT, 0, 0);
            gl2.BufferData(ARRAY_BUFFER, flags.ToArray(), STATIC_DRAW);
            vboBytes = (int)gl2.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != flagsBytes)
                throw new ApplicationException(string.Format("Problem uploading flags buffer to VBO (flags). Tried to upload {0} bytes, uploaded {1}.", flagsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the triangles onto the GL render context and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
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
            int vboBytes;
            int trisBytes = triangleIndices.Length * sizeof(uint);

            if (((MeshImp)mr).ElementBufferObject == null)
                ((MeshImp)mr).ElementBufferObject = gl2.CreateBuffer();
            // Upload the index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
            gl2.BindBuffer(ELEMENT_ARRAY_BUFFER, ((MeshImp)mr).ElementBufferObject);
            gl2.BufferData(ELEMENT_ARRAY_BUFFER, triangleIndices.ToArray(), STATIC_DRAW);
            vboBytes = (int)gl2.GetBufferParameter(ELEMENT_ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != trisBytes)
                throw new ApplicationException(string.Format("Problem uploading vertex buffer to VBO (offsets). Tried to upload {0} bytes, uploaded {1}.", trisBytes, vboBytes));
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveVertices(IMeshImp mr)
        {
            gl2.DeleteBuffer(((MeshImp)mr).VertexBufferObject);
            ((MeshImp)mr).InvalidateVertices();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="instanceImp">The instance data whose buffers are to be deleted.</param>
        public void RemoveInstanceData(IInstanceDataImp instanceImp)
        {
            gl2.DeleteBuffer(((InstanceDataImp)instanceImp).InstanceTransformBufferObject);
            gl2.DeleteBuffer(((InstanceDataImp)instanceImp).InstanceColorBufferObject);
            ((InstanceDataImp)instanceImp).InstanceTransformBufferObject = null;
            ((InstanceDataImp)instanceImp).InstanceColorBufferObject = null;
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveNormals(IMeshImp mr)
        {
            gl2.DeleteBuffer(((MeshImp)mr).NormalBufferObject);
            ((MeshImp)mr).InvalidateNormals();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveColors(IMeshImp mr)
        {
            gl2.DeleteBuffer(((MeshImp)mr).ColorBufferObject);
            ((MeshImp)mr).InvalidateColors();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mesh">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveColors1(IMeshImp mesh)
        {
            gl2.DeleteBuffer(((MeshImp)mesh).Color1BufferObject);
            ((MeshImp)mesh).InvalidateColors1();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mesh">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveColors2(IMeshImp mesh)
        {
            gl2.DeleteBuffer(((MeshImp)mesh).Color2BufferObject);
            ((MeshImp)mesh).InvalidateColors2();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveUVs(IMeshImp mr)
        {
            gl2.DeleteBuffer(((MeshImp)mr).UVBufferObject);
            ((MeshImp)mr).InvalidateUVs();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveTriangles(IMeshImp mr)
        {
            gl2.DeleteBuffer(((MeshImp)mr).ElementBufferObject);
            ((MeshImp)mr).InvalidateTriangles();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBoneWeights(IMeshImp mr)
        {
            gl2.DeleteBuffer(((MeshImp)mr).BoneWeightBufferObject);
            ((MeshImp)mr).InvalidateBoneWeights();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBoneIndices(IMeshImp mr)
        {
            gl2.DeleteBuffer(((MeshImp)mr).BoneIndexBufferObject);
            ((MeshImp)mr).InvalidateBoneIndices();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveTangents(IMeshImp mr)
        {
            gl2.DeleteBuffer(((MeshImp)mr).TangentBufferObject);
            ((MeshImp)mr).InvalidateTangents();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveBiTangents(IMeshImp mr)
        {
            gl2.DeleteBuffer(((MeshImp)mr).BitangentBufferObject);
            ((MeshImp)mr).InvalidateBiTangents();
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveFlags(IMeshImp mr)
        {
            gl2.DeleteBuffer(((MeshImp)mr).FlagsBufferObject);
            ((MeshImp)mr).InvalidateFlags();
        }

        /// <summary>
        /// Renders the specified <see cref="IMeshImp" />.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="instanceData">The <see cref="IInstanceDataImp" /></param>
        public void Render(IMeshImp mr, IInstanceDataImp instanceData = null)
        {
            gl2.BindVertexArray(((MeshImp)mr).VertexArrayObject);

            if (((MeshImp)mr).VertexBufferObject != null)
                gl2.EnableVertexAttribArray((uint)AttributeLocations.VertexAttribLocation);

            if (((MeshImp)mr).ColorBufferObject != null)
                gl2.EnableVertexAttribArray((uint)AttributeLocations.ColorAttribLocation);

            if (((MeshImp)mr).Color1BufferObject != null)
                gl2.EnableVertexAttribArray((uint)AttributeLocations.ColorAttribLocation);

            if (((MeshImp)mr).Color2BufferObject != null)
                gl2.EnableVertexAttribArray((uint)AttributeLocations.ColorAttribLocation);

            if (((MeshImp)mr).UVBufferObject != null)
                gl2.EnableVertexAttribArray((uint)AttributeLocations.UvAttribLocation);

            if (((MeshImp)mr).NormalBufferObject != null)
                gl2.EnableVertexAttribArray((uint)AttributeLocations.NormalAttribLocation);

            if (((MeshImp)mr).TangentBufferObject != null)
                gl2.EnableVertexAttribArray((uint)AttributeLocations.TangentAttribLocation);

            if (((MeshImp)mr).BitangentBufferObject != null)
                gl2.EnableVertexAttribArray((uint)AttributeLocations.BitangentAttribLocation);

            if (((MeshImp)mr).BoneIndexBufferObject != null)
                gl2.EnableVertexAttribArray((uint)AttributeLocations.BoneIndexAttribLocation);

            if (((MeshImp)mr).BoneWeightBufferObject != null)
                gl2.EnableVertexAttribArray((uint)AttributeLocations.BoneWeightAttribLocation);

            if (((MeshImp)mr).FlagsBufferObject != null)
                gl2.EnableVertexAttribArray((uint)AttributeLocations.FlagsAttribLocation);

            if (((MeshImp)mr).ElementBufferObject != null)
            {
                gl2.BindBuffer(ELEMENT_ARRAY_BUFFER, ((MeshImp)mr).ElementBufferObject);

                var oglPrimitiveType = ((MeshImp)mr).MeshType switch
                {
                    PrimitiveType.Points => POINTS,
                    PrimitiveType.Lines => LINES,
                    PrimitiveType.LineLoop => LINE_LOOP,
                    PrimitiveType.LineStrip => LINE_STRIP,
                    PrimitiveType.Patches => throw new NotSupportedException("Patches aren't supported."),
                    PrimitiveType.QuadStrip => throw new NotSupportedException("QuadStrips aren't supported."),
                    PrimitiveType.TriangleFan => TRIANGLE_FAN,
                    PrimitiveType.TriangleStrip => TRIANGLE_STRIP,
                    PrimitiveType.Quads => throw new NotSupportedException("Quads aren't supported."),
                    _ => TRIANGLES,
                };

                if (instanceData != null)
                {
                    var sizeOfFloat4 = sizeof(float) * 4;
                    var sizeOfMat = sizeOfFloat4 * 4;
                    if (((InstanceDataImp)instanceData).InstanceColorBufferObject != null)
                    {
                        gl2.BindBuffer(ARRAY_BUFFER, ((InstanceDataImp)instanceData).InstanceColorBufferObject);
                        gl2.EnableVertexAttribArray((uint)AttributeLocations.InstancedColor);
                        //Needed in case of one Mesh / VBO used for more than one InstanceData / InstanceTransformBufferObject -> reset pointer
                        gl2.VertexAttribPointer((uint)AttributeLocations.InstancedColor, 4, FLOAT, false, 4 * sizeof(float), 0);
                    }

                    gl2.BindBuffer(ARRAY_BUFFER, ((InstanceDataImp)instanceData).InstanceTransformBufferObject);

                    // set attribute pointers for matrix (4 times vec4)
                    gl2.EnableVertexAttribArray((uint)AttributeLocations.InstancedModelMat1);
                    gl2.EnableVertexAttribArray((uint)AttributeLocations.InstancedModelMat2);
                    gl2.EnableVertexAttribArray((uint)AttributeLocations.InstancedModelMat3);
                    gl2.EnableVertexAttribArray((uint)AttributeLocations.InstancedModelMat4);

                    //Needed in case of one Mesh / VBO used for more than one InstanceData / InstanceTransformBufferObject -> reset pointer
                    gl2.VertexAttribPointer((uint)AttributeLocations.InstancedModelMat1, 4, FLOAT, false, sizeOfMat, 0);
                    gl2.VertexAttribPointer((uint)AttributeLocations.InstancedModelMat2, 4, FLOAT, false, sizeOfMat, (uint)(1 * sizeOfFloat4));
                    gl2.VertexAttribPointer((uint)AttributeLocations.InstancedModelMat3, 4, FLOAT, false, sizeOfMat, (uint)(2 * sizeOfFloat4));
                    gl2.VertexAttribPointer((uint)AttributeLocations.InstancedModelMat4, 4, FLOAT, false, sizeOfMat, (uint)(3 * sizeOfFloat4));

                    gl2.DrawElementsInstanced(oglPrimitiveType, ((MeshImp)mr).NElements, UNSIGNED_BYTE, 0U, instanceData.Amount);

                    gl2.DisableVertexAttribArray((uint)AttributeLocations.InstancedModelMat1);
                    gl2.DisableVertexAttribArray((uint)AttributeLocations.InstancedModelMat2);
                    gl2.DisableVertexAttribArray((uint)AttributeLocations.InstancedModelMat3);
                    gl2.DisableVertexAttribArray((uint)AttributeLocations.InstancedModelMat4);
                    gl2.DisableVertexAttribArray((uint)AttributeLocations.InstancedColor);
                }
                else
                    gl2.DrawElements(oglPrimitiveType, ((MeshImp)mr).NElements, UNSIGNED_INT, 0);
            }

            gl2.BindBuffer(ARRAY_BUFFER, null);
            if (((MeshImp)mr).VertexBufferObject != null)
                gl2.DisableVertexAttribArray((uint)AttributeLocations.VertexAttribLocation);

            if (((MeshImp)mr).ColorBufferObject != null)
                gl2.DisableVertexAttribArray((uint)AttributeLocations.ColorAttribLocation);

            if (((MeshImp)mr).Color1BufferObject != null)
                gl2.DisableVertexAttribArray((uint)AttributeLocations.Color1AttribLocation);

            if (((MeshImp)mr).Color2BufferObject != null)
                gl2.DisableVertexAttribArray((uint)AttributeLocations.Color2AttribLocation);

            if (((MeshImp)mr).NormalBufferObject != null)
                gl2.DisableVertexAttribArray((uint)AttributeLocations.NormalAttribLocation);

            if (((MeshImp)mr).UVBufferObject != null)
                gl2.DisableVertexAttribArray((uint)AttributeLocations.UvAttribLocation);

            if (((MeshImp)mr).TangentBufferObject != null)
                gl2.DisableVertexAttribArray((uint)AttributeLocations.TangentAttribLocation);

            if (((MeshImp)mr).BitangentBufferObject != null)
                gl2.DisableVertexAttribArray((uint)AttributeLocations.BitangentAttribLocation);

            if (((MeshImp)mr).FlagsBufferObject != null)
                gl2.DisableVertexAttribArray((uint)AttributeLocations.FlagsAttribLocation);

            gl2.BindVertexArray(null);

        }

        /// <summary>
        /// Gets the content of the buffer.
        /// </summary>
        /// <param name="quad">The Rectangle where the content is draw into.</param>
        /// <param name="texId">The texture identifier.</param>
        public void GetBufferContent(Rectangle quad, ITextureHandle texId)
        {
            gl2.BindTexture(TEXTURE_2D, ((TextureHandle)texId).TexId);
            gl2.CopyTexImage2D(TEXTURE_2D, 0, RGBA, quad.Left, quad.Top, quad.Width, quad.Height, 0);
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

        internal static uint BlendOperationToOgl(BlendOperation bo)
        {
            return bo switch
            {
                BlendOperation.Add => FUNC_ADD,
                BlendOperation.Subtract => FUNC_SUBTRACT,
                BlendOperation.ReverseSubtract => FUNC_REVERSE_SUBTRACT,
                BlendOperation.Minimum => throw new NotSupportedException("MIN blending mode not supported in WebGL!"),
                BlendOperation.Maximum => throw new NotSupportedException("MAX blending mode not supported in WebGL!"),
                _ => throw new ArgumentOutOfRangeException($"Invalid argument: {bo}"),
            };
        }

        internal static BlendOperation BlendOperationFromOgl(uint bom)
        {
            return bom switch
            {
                FUNC_ADD => BlendOperation.Add,
                FUNC_SUBTRACT => BlendOperation.Subtract,
                FUNC_REVERSE_SUBTRACT => BlendOperation.ReverseSubtract,
                _ => throw new ArgumentOutOfRangeException($"Invalid argument: {bom}"),
            };
        }

        internal static uint BlendToOgl(Blend blend, bool isForAlpha = false)
        {
            return blend switch
            {
                Blend.Zero => (int)ZERO,
                Blend.One => (int)ONE,
                Blend.SourceColor => (int)SRC_COLOR,
                Blend.InverseSourceColor => (int)ONE_MINUS_SRC_COLOR,
                Blend.SourceAlpha => (int)SRC_ALPHA,
                Blend.InverseSourceAlpha => (int)ONE_MINUS_SRC_ALPHA,
                Blend.DestinationAlpha => (int)DST_ALPHA,
                Blend.InverseDestinationAlpha => (int)ONE_MINUS_DST_ALPHA,
                Blend.DestinationColor => (int)DST_COLOR,
                Blend.InverseDestinationColor => (int)ONE_MINUS_DST_COLOR,
                Blend.BlendFactor => (uint)(int)((isForAlpha) ? CONSTANT_ALPHA : CONSTANT_COLOR),
                Blend.InverseBlendFactor => (uint)(int)((isForAlpha) ? ONE_MINUS_CONSTANT_ALPHA : ONE_MINUS_CONSTANT_COLOR),
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
                _ => throw new ArgumentOutOfRangeException($"Blend mode {blend} not supported!"),
            };
        }

        internal static Blend BlendFromOgl(uint bf)
        {
            return bf switch
            {
                ZERO => Blend.Zero,
                ONE => Blend.One,
                SRC_COLOR => Blend.SourceColor,
                ONE_MINUS_SRC_COLOR => Blend.InverseSourceColor,
                SRC_ALPHA => Blend.SourceAlpha,
                ONE_MINUS_SRC_ALPHA => Blend.InverseSourceAlpha,
                DST_ALPHA => Blend.DestinationAlpha,
                ONE_MINUS_DST_ALPHA => Blend.InverseDestinationAlpha,
                DST_COLOR => Blend.DestinationColor,
                ONE_MINUS_DST_COLOR => Blend.InverseDestinationColor,
                CONSTANT_COLOR or CONSTANT_ALPHA => Blend.BlendFactor,
                ONE_MINUS_CONSTANT_COLOR or ONE_MINUS_CONSTANT_ALPHA => Blend.InverseBlendFactor,
                _ => throw new ArgumentOutOfRangeException($"Blend mode {bf} not supported!"),
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
                        throw new ArgumentException("Function not available, convert your geometry to line primitives and render them using GL_LINES!");
                    }
                case RenderState.CullMode:
                    {
                        switch ((Cull)value)
                        {
                            case Cull.None:
                                if (_isCullEnabled)
                                {
                                    _isCullEnabled = false;
                                    gl2.Disable(CULL_FACE);
                                }
                                gl2.FrontFace(CCW);
                                break;
                            case Cull.Clockwise:
                                if (!_isCullEnabled)
                                {
                                    _isCullEnabled = true;
                                    gl2.Enable(CULL_FACE);
                                }
                                gl2.FrontFace(CW);
                                break;
                            case Cull.Counterclockwise:
                                if (!_isCullEnabled)
                                {
                                    _isCullEnabled = true;
                                    gl2.Enable(CULL_FACE);
                                }
                                gl2.FrontFace(CCW);
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
                        var df = GetDepthCompareFunc((Compare)value);
                        gl2.DepthFunc(df);
                    }
                    break;
                case RenderState.ZEnable:
                    if (value == 0)
                        gl2.Disable(DEPTH_TEST);
                    else
                        gl2.Enable(DEPTH_TEST);
                    break;
                case RenderState.ZWriteEnable:
                    gl2.DepthMask(value != 0);
                    break;
                case RenderState.AlphaBlendEnable:
                    if (value == 0)
                        gl2.Disable(BLEND);
                    else
                        gl2.Enable(BLEND);
                    break;
                case RenderState.BlendOperation:
                    {
                        _blendEquationRgb = BlendOperationToOgl((BlendOperation)value);
                        gl2.BlendEquationSeparate(_blendEquationRgb, _blendEquationAlpha);
                    }
                    break;

                case RenderState.BlendOperationAlpha:
                    {
                        _blendEquationAlpha = BlendOperationToOgl((BlendOperation)value);
                        gl2.BlendEquationSeparate(_blendEquationRgb, _blendEquationAlpha);
                    }
                    break;
                case RenderState.SourceBlend:
                    {
                        _blendSrcRgb = BlendToOgl((Blend)value);
                        gl2.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.DestinationBlend:
                    {
                        _blendDstRgb = BlendToOgl((Blend)value);
                        gl2.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.SourceBlendAlpha:
                    {
                        _blendSrcAlpha = BlendToOgl((Blend)value);
                        gl2.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.DestinationBlendAlpha:
                    {
                        _blendDstAlpha = BlendToOgl((Blend)value);
                        gl2.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.BlendFactor:
                    var rgba = (int)value;
                    var r = rgba >> 32;
                    var g = rgba >> 16 & 0xFF;
                    var b = rgba >> 8 & 0xFF;
                    var a = rgba & 0xFF;
                    gl2.BlendColor(r, g, b, a);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderState));
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
                        return (uint)FillMode.Solid;
                    }
                case RenderState.CullMode:
                    {
                        uint cullFace;
                        cullFace = (uint)(int)gl2.GetParameter(CULL_FACE);
                        if (cullFace == 0)
                            return (int)Cull.None;
                        uint frontFace;
                        frontFace = (uint)(int)gl2.GetParameter(FRONT_FACE);
                        if (frontFace == CW)
                            return (uint)Cull.Clockwise;
                        return (uint)Cull.Counterclockwise;
                    }
                case RenderState.Clipping:
                    // clipping is always on in OpenGL - This state is simply ignored
                    return 1; // == true
                case RenderState.ZFunc:
                    {
                        uint depFunc;
                        depFunc = gl2.GetParameter(DEPTH_FUNC);
                        var ret = depFunc switch
                        {
                            NEVER => Compare.Never,
                            LESS => Compare.Less,
                            EQUAL => Compare.Equal,
                            LEQUAL => Compare.LessEqual,
                            GREATER => Compare.Greater,
                            NOTEQUAL => Compare.NotEqual,
                            GEQUAL => Compare.GreaterEqual,
                            ALWAYS => Compare.Always,
                            _ => throw new ArgumentOutOfRangeException("depFunc", "Value " + depFunc + " not handled"),
                        };
                        return (uint)ret;
                    }
                case RenderState.ZEnable:
                    {
                        uint depTest;
                        depTest = (uint)(int)gl2.GetParameter(DEPTH_TEST);
                        return depTest;
                    }
                case RenderState.ZWriteEnable:
                    {
                        uint depWriteMask;
                        depWriteMask = (uint)(int)gl2.GetParameter(DEPTH_WRITEMASK);
                        return depWriteMask;
                    }
                case RenderState.AlphaBlendEnable:
                    {
                        uint blendEnable;
                        blendEnable = (uint)(int)gl2.GetParameter(BLEND);
                        return blendEnable;
                    }
                case RenderState.BlendOperation:
                    {
                        uint rgbMode;
                        rgbMode = (uint)(int)gl2.GetParameter(BLEND_EQUATION_RGB);
                        return (uint)BlendOperationFromOgl(rgbMode);
                    }
                case RenderState.BlendOperationAlpha:
                    {
                        uint alphaMode;
                        alphaMode = (uint)(int)gl2.GetParameter(BLEND_EQUATION_ALPHA);
                        return (uint)BlendOperationFromOgl(alphaMode);
                    }
                case RenderState.SourceBlend:
                    {
                        uint rgbSrc;
                        rgbSrc = (uint)(int)gl2.GetParameter(BLEND_SRC_RGB);
                        return (uint)BlendFromOgl(rgbSrc);
                    }
                case RenderState.DestinationBlend:
                    {
                        uint rgbDst;
                        rgbDst = (uint)(int)gl2.GetParameter(BLEND_DST_RGB);
                        return (uint)BlendFromOgl(rgbDst);
                    }
                case RenderState.SourceBlendAlpha:
                    {
                        uint alphaSrc;
                        alphaSrc = (uint)(int)gl2.GetParameter(BLEND_SRC_ALPHA);
                        return (uint)BlendFromOgl(alphaSrc);
                    }
                case RenderState.DestinationBlendAlpha:
                    {
                        uint alphaDst;
                        alphaDst = (uint)(int)gl2.GetParameter(BLEND_DST_ALPHA);
                        return (uint)BlendFromOgl(alphaDst);
                    }
                case RenderState.BlendFactor:
                    {
                        uint c = gl2.GetParameter(BLEND_COLOR);
                        ColorUint uintCol = new(c);
                        return (uint)uintCol.ToRgba();
                    }
                default:
                    throw new ArgumentOutOfRangeException("renderState");
            }
        }

        /// <summary>
        /// Renders into the given texture.
        /// </summary>
        /// <param name="tex">The texture.</param>
        /// <param name="texHandle">The texture handle, associated with the given texture. Should be created by the TextureManager in the RenderContext.</param>
        public void SetRenderTarget(IWritableTexture tex, ITextureHandle texHandle)
        {
            WebGLFramebuffer fBuffer;
            if (tex is not WritableTexture)
            {
                throw new NotSupportedException("Blazor has no MultisampleWritableTexture support!");
            }

            if (((TextureHandle)texHandle).FrameBufferHandle == null)
            {
                fBuffer = gl2.CreateFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                gl2.BindFramebuffer(FRAMEBUFFER, fBuffer);
                gl2.BindTexture(TEXTURE_2D, ((TextureHandle)texHandle).TexId);

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    ((TextureHandle)texHandle).DepthRenderBufferHandle = CreateDepthRenderBuffer(tex.Width, tex.Height);
                    gl2.FramebufferTexture2D(FRAMEBUFFER, COLOR_ATTACHMENT0, TEXTURE_2D, ((TextureHandle)texHandle).TexId, 0);
                    gl2.DrawBuffers(new uint[] { COLOR_ATTACHMENT0 });
                }
                else
                {
                    gl2.FramebufferTexture2D(FRAMEBUFFER, DEPTH_ATTACHMENT, TEXTURE_2D, ((TextureHandle)texHandle).TexId, 0);
                    gl2.DrawBuffers(new uint[] { NONE });
                    gl2.ReadBuffer(NONE);
                }
            }
            else
            {
                gl2.BindFramebuffer(FRAMEBUFFER, ((TextureHandle)texHandle).FrameBufferHandle);
            }

            if (gl2.CheckFramebufferStatus(FRAMEBUFFER) != FRAMEBUFFER_COMPLETE)
                throw new Exception($"Error creating RenderTarget: {gl2.GetError()}, {gl2.CheckFramebufferStatus(FRAMEBUFFER)}; color format: {tex.PixelFormat.ColorFormat}");
        }

        /// <summary>
        /// Renders into the given cube map.
        /// </summary>
        /// <param name="tex">The texture.</param>
        /// <param name="texHandle">The texture handle, associated with the given cube map. Should be created by the TextureManager in the RenderContext.</param>
        public void SetRenderTarget(IWritableCubeMap tex, ITextureHandle texHandle)
        {
            if (((TextureHandle)texHandle).FrameBufferHandle == null)
            {
                WebGLFramebuffer fBuffer = gl2.CreateFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                gl2.BindFramebuffer(FRAMEBUFFER, fBuffer);

                gl2.BindTexture(TEXTURE_CUBE_MAP, ((TextureHandle)texHandle).TexId);

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    ((TextureHandle)texHandle).DepthRenderBufferHandle = CreateDepthRenderBuffer(tex.Width, tex.Height);
                    gl2.FramebufferTexture2D(FRAMEBUFFER, COLOR_ATTACHMENT0, TEXTURE_CUBE_MAP, ((TextureHandle)texHandle).TexId, 0);
                    gl2.DrawBuffers(new uint[] { COLOR_ATTACHMENT0 });
                }
                else
                {
                    gl2.FramebufferTexture2D(FRAMEBUFFER, DEPTH_ATTACHMENT, TEXTURE_CUBE_MAP, ((TextureHandle)texHandle).TexId, 0);
                    gl2.DrawBuffers(new uint[] { NONE });
                    gl2.ReadBuffer(NONE);
                }
            }
            else
            {
                gl2.BindFramebuffer(FRAMEBUFFER, ((TextureHandle)texHandle).FrameBufferHandle);
            }

            if (gl2.CheckFramebufferStatus(FRAMEBUFFER) != FRAMEBUFFER_COMPLETE)
                throw new Exception($"Error creating RenderTarget: {gl2.GetError()}, {gl2.CheckFramebufferStatus(FRAMEBUFFER)}; pixel format: {tex.PixelFormat}");
        }

        /// <summary>
        /// Sets an <see cref="IWritableArrayTexture"/> as render target
        /// Not supported in WebGL2 ?
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="layer"></param>
        /// <param name="texHandle"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetRenderTarget(IWritableArrayTexture tex, int layer, ITextureHandle texHandle)
        {
            if (((TextureHandle)texHandle).FrameBufferHandle == null)
            {
                var fBuffer = gl2.CreateFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                gl2.BindFramebuffer(FRAMEBUFFER, fBuffer);

                gl2.BindTexture(TEXTURE_2D_ARRAY, ((TextureHandle)texHandle).TexId);

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    ((TextureHandle)texHandle).DepthRenderBufferHandle = CreateDepthRenderBuffer(tex.Width, tex.Height);
                    gl2.FramebufferTextureLayer(FRAMEBUFFER, COLOR_ATTACHMENT0, ((TextureHandle)texHandle).TexId, 0, layer);
                    gl2.DrawBuffers(new uint[] { COLOR_ATTACHMENT0 });
                }
                else
                {
                    gl2.FramebufferTextureLayer(FRAMEBUFFER, DEPTH_ATTACHMENT, ((TextureHandle)texHandle).TexId, 0, layer);
                    gl2.DrawBuffers(new uint[] { NONE });
                    gl2.ReadBuffer(NONE);
                }
            }
            else
            {
                gl2.BindFramebuffer(FRAMEBUFFER, ((TextureHandle)texHandle).FrameBufferHandle);
                gl2.BindTexture(TEXTURE_2D_ARRAY, ((TextureHandle)texHandle).TexId);
                gl2.FramebufferTextureLayer(FRAMEBUFFER, DEPTH_ATTACHMENT, ((TextureHandle)texHandle).TexId, 0, layer);
            }

            if (gl2.CheckFramebufferStatus(FRAMEBUFFER) != FRAMEBUFFER_COMPLETE)
                throw new Exception($"Error creating RenderTarget IWritableArrayTexture: {gl2.GetError()}, {gl2.CheckFramebufferStatus(FRAMEBUFFER)}");
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
                gl2.BindFramebuffer(FRAMEBUFFER, null);
                return;
            }

            WebGLFramebuffer gBuffer;

            if (renderTarget.GBufferHandle == null)
            {
                renderTarget.GBufferHandle = new FrameBufferHandle();
                gBuffer = CreateFrameBuffer(renderTarget, texHandles);
                ((FrameBufferHandle)renderTarget.GBufferHandle).Handle = gBuffer;
            }
            else
            {
                gBuffer = ((FrameBufferHandle)renderTarget.GBufferHandle).Handle;
                gl2.BindFramebuffer(FRAMEBUFFER, gBuffer);
            }

            if (renderTarget.RenderTextures[(int)RenderTargetTextureTypes.Depth] == null && !renderTarget.IsDepthOnly)
            {
                WebGLRenderbuffer gDepthRenderbufferHandle;
                if (renderTarget.DepthBufferHandle == null)
                {
                    renderTarget.DepthBufferHandle = new RenderBufferHandle();
                    // Create and attach depth-buffer (render-buffer)
                    gDepthRenderbufferHandle = CreateDepthRenderBuffer((int)renderTarget.TextureResolution, (int)renderTarget.TextureResolution);
                    ((RenderBufferHandle)renderTarget.DepthBufferHandle).Handle = gDepthRenderbufferHandle;
                }
                else
                {
                    gDepthRenderbufferHandle = ((RenderBufferHandle)renderTarget.DepthBufferHandle).Handle;
                    gl2.BindRenderbuffer(RENDERBUFFER, gDepthRenderbufferHandle);
                }
            }
#if DEBUG
            if (gl2.CheckFramebufferStatus(FRAMEBUFFER) != FRAMEBUFFER_COMPLETE)
            {
                throw new Exception($"Error creating frame buffer: {gl2.GetError()}, {gl2.CheckFramebufferStatus(FRAMEBUFFER)};" +
                    $"DepthBuffer set? {renderTarget.DepthBufferHandle != null}");
            }
#endif
        }

        /// <summary>
        /// Takes a <see cref="WritableMultisampleTexture"/> and blits the result of all samples into an
        /// existing <see cref="WritableTexture"/> for further use (e. g. bind and use as Albedo texture)
        /// </summary>
        /// <param name="input">WritableMultisampleTexture</param>
        /// <param name="output">WritableTexture</param>
        public void BlitMultisample2DTextureToTexture(IWritableTexture input, IWritableTexture output)
        {
            throw new NotSupportedException("Blazor has no MultisampleWritableTexture support!");
        }

        private WebGLRenderbuffer CreateDepthRenderBuffer(int width, int height)
        {
            gl2.Enable(DEPTH_TEST);

            WebGLRenderbuffer gDepthRenderbuffer = gl2.CreateRenderbuffer();
            gl2.BindRenderbuffer(RENDERBUFFER, gDepthRenderbuffer);
            gl2.RenderbufferStorage(RENDERBUFFER, DEPTH_COMPONENT24, width, height);
            gl2.FramebufferRenderbuffer(FRAMEBUFFER, DEPTH_ATTACHMENT, RENDERBUFFER, gDepthRenderbuffer);
            return gDepthRenderbuffer;
        }

        private WebGLFramebuffer CreateFrameBuffer(IRenderTarget renderTarget, ITextureHandle[] texHandles)
        {
            WebGLFramebuffer gBuffer = gl2.CreateFramebuffer();
            gl2.BindFramebuffer(FRAMEBUFFER, gBuffer);

            int depthCnt = 0;

            int depthTexPos = (int)RenderTargetTextureTypes.Depth;

            if (!renderTarget.IsDepthOnly)
            {
                List<uint> attachments = new();

                //Textures
                for (int i = 0; i < texHandles.Length; i++)
                {
                    attachments.Add(COLOR_ATTACHMENT0 + (uint)i);


                    ITextureHandle texHandle = texHandles[i];
                    if (texHandle == null) continue;


                    if (i == depthTexPos)
                    {
                        gl2.FramebufferTexture2D(FRAMEBUFFER, DEPTH_ATTACHMENT + (uint)depthCnt, TEXTURE_2D, ((TextureHandle)texHandle).TexId, 0);
                        depthCnt++;
                    }
                    else
                    {
                        gl2.FramebufferTexture2D(FRAMEBUFFER, COLOR_ATTACHMENT0 + (uint)(i - depthCnt), TEXTURE_2D, ((TextureHandle)texHandle).TexId, 0);
                    }
                }

                gl2.DrawBuffers(attachments.ToArray());
            }
            else //If a frame-buffer only has a depth texture we don't need draw buffers
            {
                Console.WriteLine("Depth only!");
                ITextureHandle texHandle = texHandles[depthTexPos];

                if (texHandle != null)
                    gl2.FramebufferTexture2D(FRAMEBUFFER, DEPTH_ATTACHMENT, TEXTURE_2D, ((TextureHandle)texHandle).TexId, 0);
                else
                    throw new NullReferenceException("Texture handle is null!");

                gl2.DrawBuffers(new uint[] { NONE });
                gl2.ReadBuffer(NONE);
            }

            return gBuffer;
        }

        /// <summary>
        /// Detaches a texture from the frame buffer object, associated with the given render target.
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="attachment">Number of the fbo attachment. For example: attachment = 1 will detach the texture currently associated with <see cref="COLOR_ATTACHMENT1"/>.</param>
        /// <param name="isDepthTex">Determines if the texture is a depth texture. In this case the texture currently associated with <see cref="DEPTH_ATTACHMENT"/> will be detached.</param>
        public void DetachTextureFromFbo(IRenderTarget renderTarget, bool isDepthTex, int attachment = 0)
        {
            ChangeFramebufferTexture2D(renderTarget, attachment, null, isDepthTex); //TODO: check if "null" is the equivalent to the zero texture (handle = 0) in OpenGL Core
        }


        /// <summary>
        /// Attaches a texture to the frame buffer object, associated with the given render target.
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="attachment">Number of the fbo attachment. For example: attachment = 1 will attach the texture to <see cref="COLOR_ATTACHMENT1"/>.</param>
        /// <param name="isDepthTex">Determines if the texture is a depth texture. In this case the texture is attached to <see cref="DEPTH_ATTACHMENT"/>.</param>
        /// <param name="texHandle">The gpu handle of the texture.</param>
        public void AttacheTextureToFbo(IRenderTarget renderTarget, bool isDepthTex, ITextureHandle texHandle, int attachment = 0)
        {
            ChangeFramebufferTexture2D(renderTarget, attachment, ((TextureHandle)texHandle).TexId, isDepthTex);
        }

        private void ChangeFramebufferTexture2D(IRenderTarget renderTarget, int attachment, WebGLTexture handle, bool isDepth)
        {
            WebGLFramebuffer rtFbo = ((FrameBufferHandle)renderTarget.GBufferHandle).Handle;

            gl2.BindFramebuffer(FRAMEBUFFER, rtFbo);

            if (!isDepth && attachment != NONE)
                gl2.FramebufferTexture2D(FRAMEBUFFER, COLOR_ATTACHMENT0 + (uint)attachment, TEXTURE_2D, handle, 0);
            else
                gl2.FramebufferTexture2D(FRAMEBUFFER, DEPTH_ATTACHMENT, TEXTURE_2D, handle, 0);

            if (gl2.CheckFramebufferStatus(FRAMEBUFFER) != FRAMEBUFFER_COMPLETE)
                throw new Exception($"Error creating RenderTarget: {gl2.GetError()}, {gl2.CheckFramebufferStatus(FRAMEBUFFER)}");

            //if (!isCurrentFbo)
            //    gl2.BindFramebuffer(FRAMEBUFFER, boundFbo);
        }

        /// <summary>
        /// Set the Viewport of the rendering output window by x,y position and width,height parameters.
        /// The Viewport is the portion of the final image window.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>Render
        public void Viewport(int x, int y, int width, int height)
        {
            gl2.Viewport(x, y, width, height);
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
            gl2.ColorMask(red, green, blue, alpha);
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
                HardwareCapability.CanRenderDeferred => 1U,
                HardwareCapability.CanUseGeometryShaders => 0U,
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
            return "";
        }

        /// <summary>
        /// Draws a Debug Line in 3D Space by using a start and end point (float3).
        /// </summary>
        /// <param name="start">The starting point of the DebugLine.</param>
        /// <param name="end">The endpoint of the DebugLine.</param>
        /// <param name="color">The color of the DebugLine.</param>
        public void DebugLine(float3 start, float3 end, float4 color)
        {
            float3[] vertices = new float3[]
            {
                new float3(start.x, start.y, start.z),
                new float3(end.x, end.y, end.z),
            };

            int itemSize = 3;
            int numItems = 2;
            WebGLBuffer posBuffer = gl2.CreateBuffer();

            float[] verticesFlat = new float[vertices.Length * 3];
            int i = 0;
            foreach (float3 v in vertices)
            {
                verticesFlat[i] = v.x;
                verticesFlat[i + 1] = v.y;
                verticesFlat[i + 2] = v.z;
                i += 3;
            }

            gl2.EnableVertexAttribArray((uint)AttributeLocations.VertexAttribLocation);
            gl2.BindBuffer(ARRAY_BUFFER, posBuffer);
            gl2.BufferData(ARRAY_BUFFER, verticesFlat, STATIC_DRAW);
            gl2.VertexAttribPointer((uint)AttributeLocations.VertexAttribLocation, itemSize, FLOAT, false, 0, 0);

            gl2.DrawArrays(LINE_STRIP, 0, numItems);
            gl2.DisableVertexAttribArray((uint)AttributeLocations.VertexAttribLocation);
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

            gl2.ReadPixels(x, y, 1, 1, format.Format, format.PxType, data);

            var err = gl2.GetError();
            if (err != NO_ERROR)
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
            ImageData image = Fusee.Base.Core.ImageData.CreateImage(w, h, ColorUint.Black);
            _ = image.PixelData; // Uint8Array.From(image.PixelData);
                                 // TODO(MR): Check!
            gl2.ReadPixels(x, y, w, h, RGB /* yuk, yuk ??? */, UNSIGNED_BYTE, 0);
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
            //var depthTA = Float32Array.From(depth);
            // TODO(MR): Check!

            gl2.ReadPixels(x, y, 1, 1, DEPTH_COMPONENT, UNSIGNED_BYTE, 0);

            return depth[0];
        }

        /// <summary>
        /// TODO: IMPLEMENT
        /// </summary>
        /// <param name="renderTarget"></param>
        /// <param name="type"></param>
        public void DetachTextureFromFbo(IRenderTarget renderTarget, RenderTargetTextureTypes type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO: IMPLEMENT
        /// </summary>
        /// <param name="renderTarget"></param>
        /// <param name="type"></param>
        /// <param name="texHandle"></param>
        public void ReatatchTextureFromFbo(IRenderTarget renderTarget, RenderTargetTextureTypes type, ITextureHandle texHandle)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the texture filter mode
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="filterMode"></param>
        public void SetTextureFilterMode(ITextureHandle tex, TextureFilterMode filterMode)
        {
            gl2.BindTexture(TEXTURE_2D, ((TextureHandle)tex).TexId);
            Tuple<int, int> glMinMagFilter = GetMinMagFilter(filterMode);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_MIN_FILTER, glMinMagFilter.Item1);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_MAG_FILTER, glMinMagFilter.Item2);
        }

        /// <summary>
        /// Sets the texture wrapping mode
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="wrapMode"></param>
        public void SetTextureWrapMode(ITextureHandle tex, TextureWrapMode wrapMode)
        {
            gl2.BindTexture(TEXTURE_2D, ((TextureHandle)tex).TexId);
            int glWrapMode = GetWrapMode(wrapMode);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_S, glWrapMode);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_T, glWrapMode);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_R, glWrapMode);
        }

        /// <summary>
        /// Creates an writable array texture
        /// Not supported for WebGL2 ?
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ITextureHandle CreateTexture(IWritableArrayTexture img)
        {
            var id = gl2.CreateTexture();
            gl2.BindTexture(TEXTURE_2D_ARRAY, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;
            var glWrapMode = GetWrapMode(img.WrapMode);
            var pxInfo = GetTexturePixelInfo(img.PixelFormat);

            var data = new uint[img.Width * img.Height * img.Layers];

            gl2.TexImage3D(TEXTURE_2D_ARRAY, 0, (int)pxInfo.InternalFormat, img.Width, img.Height, img.Layers, 0, pxInfo.Format, pxInfo.PxType, data);

            gl2.TexParameteri(TEXTURE_2D_ARRAY, TEXTURE_COMPARE_MODE, (int)GetTexCompareMode(img.CompareMode));
            gl2.TexParameteri(TEXTURE_2D_ARRAY, TEXTURE_COMPARE_FUNC, (int)GetDepthCompareFunc(img.CompareFunc));
            gl2.TexParameteri(TEXTURE_2D_ARRAY, TEXTURE_MAG_FILTER, minFilter);
            gl2.TexParameteri(TEXTURE_2D_ARRAY, TEXTURE_MIN_FILTER, magFilter);
            gl2.TexParameteri(TEXTURE_2D_ARRAY, TEXTURE_WRAP_S, glWrapMode);
            gl2.TexParameteri(TEXTURE_2D_ARRAY, TEXTURE_WRAP_T, glWrapMode);


            ITextureHandle texID = new TextureHandle { TexId = id };

            return texID;

        }

        /// <summary>
        /// Sets the vertex array object
        /// </summary>
        /// <param name="mr"></param>
        public void SetVertexArrayObject(IMeshImp mr)
        {
            if (((MeshImp)mr).VertexArrayObject == null)
                ((MeshImp)mr).VertexArrayObject = gl2.CreateVertexArray();

            gl2.BindVertexArray(((MeshImp)mr).VertexArrayObject);
        }

        #endregion


        /// <summary>
        /// Creates a compute shader program.
        /// Not supported in WebGL2 ?
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IShaderHandle CreateShaderProgramCompute(string cs = null)
        {
            var info = string.Empty;
            // Compile compute shader
            var computeObject = new WebGLShader();
            if (!string.IsNullOrEmpty(cs))
            {
                computeObject = gl2.CreateShader(COMPUTE_SHADER);

                gl2.ShaderSource(computeObject, cs);
                gl2.CompileShader(computeObject);
                info = gl2.GetShaderInfoLog(computeObject);
            }

            if (info != string.Empty)
                throw new ApplicationException(info);

            var program = gl2.CreateProgram();

            gl2.AttachShader(program, computeObject);
            gl2.LinkProgram(program); //Must be called AFTER BindAttribLocation
            gl2.DetachShader(program, computeObject);
            gl2.DeleteShader(computeObject);

            return new ShaderHandle { Handle = program };
        }

        /// <summary>
        /// Generates a GBuffer target
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public IRenderTarget CreateGBufferTarget(TexRes res)
        {
            RenderTargetBlazor gBufferRenderTarget = new(res);
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
        /// Returns the shader storage buffer list
        /// Not supported in WebGL2 ?
        /// </summary>
        /// <param name="shaderProgram"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IList<IActiveUniform> GetShaderStorageBufferList(IShaderHandle shaderProgram)
        {
            // compute shader!
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a shader uniform parameter based on a name
        /// Not supported in WebGL2 ?
        /// </summary>
        /// <param name="shaderProgram"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IUniformHandle GetShaderUniformParam(IShaderHandle shaderProgram, string paramName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the shader parameter, casts to float first.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="val"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetShaderParam(IUniformHandle param, double val)
        {
            gl2.Uniform1f(((UniformHandle)param).handle, (float)val);
        }

        /// <summary>
        /// Sets an shader parameter to an image.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="texId"></param>
        /// <param name="texTarget"></param>
        /// <param name="format"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetShaderParamImage(IUniformHandle param, ITextureHandle texId, TextureType texTarget, ImagePixelFormat format)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not supported?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storageBuffer"></param>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void StorageBufferSetData<T>(IStorageBuffer storageBuffer, T[] data) where T : struct
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not supported?
        /// </summary>
        /// <param name="storageBufferHandle"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void DeleteStorageBuffer(IBufferHandle storageBufferHandle)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not supported?
        /// </summary>
        /// <param name="currentProgram"></param>
        /// <param name="buffer"></param>
        /// <param name="ssboName"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void ConnectBufferToShaderStorage(IShaderHandle currentProgram, IStorageBuffer buffer, string ssboName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines a barrier ordering memory transactions. At the moment it will insert all supported barriers.
        /// </summary>
        public void MemoryBarrier()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not supported?
        /// </summary>
        /// <param name="kernelIndex"></param>
        /// <param name="threadGroupsX"></param>
        /// <param name="threadGroupsY"></param>
        /// <param name="threadGroupsZ"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void DispatchCompute(int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ)
        {
            throw new NotImplementedException();
        }
    }
}