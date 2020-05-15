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
using Fusee.Engine.Core.ShaderShards;

namespace Fusee.Engine.Imp.Graphics.WebAsm
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContextImp"/> class.
        /// </summary>
        /// <param name="renderCanvasImp">The platform specific render canvas implementation.</param>
        public RenderContextImp(IRenderCanvasImp renderCanvasImp)
        {
            _textureCountPerShader = 0;
            _shaderParam2TexUnit = new Dictionary<WebGLUniformLocation, int>();

            gl = ((RenderCanvasImp)renderCanvasImp)._gl;

            // Due to the right-handed nature of OpenGL and the left-handed design of FUSEE
            // the meaning of what's Front and Back of a face simply flips.
            // TODO - implement this in render states!!!
            gl.CullFace(BACK);

            _blendSrcAlpha = (uint)gl.GetParameter(BLEND_SRC_ALPHA);
            _blendDstAlpha = (uint)gl.GetParameter(BLEND_DST_ALPHA);
            _blendDstRgb = (uint)gl.GetParameter(BLEND_DST_RGB);
            _blendSrcRgb = (uint)gl.GetParameter(BLEND_SRC_RGB);
            _blendEquationAlpha = (uint)gl.GetParameter(BLEND_EQUATION_ALPHA);
            _blendEquationRgb = (uint)gl.GetParameter(BLEND_EQUATION_RGB);
        }

        #region Image data related Members

        private uint GetTexComapreMode(TextureCompareMode compareMode)
        {
            switch (compareMode)
            {
                case TextureCompareMode.None:
                    return (int)NONE;

                case TextureCompareMode.CompareRefToTexture:
                    return (int)COMPARE_REF_TO_TEXTURE;

                default:
                    throw new ArgumentException("Invalid compare mode.");
            }
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
                default:
                case TextureFilterMode.Linear:
                    minFilter = (int)LINEAR;
                    magFilter = (int)LINEAR;
                    break;
                case TextureFilterMode.NearestMipmapNearest:
                    minFilter = (int)NEAREST_MIPMAP_NEAREST;
                    magFilter = (int)NEAREST_MIPMAP_NEAREST;
                    break;
                case TextureFilterMode.LinearMipmapNearest:
                    minFilter = (int)LINEAR_MIPMAP_NEAREST;
                    magFilter = (int)LINEAR_MIPMAP_NEAREST;
                    break;
                case TextureFilterMode.NearestMipmapLinear:
                    minFilter = (int)NEAREST_MIPMAP_LINEAR;
                    magFilter = (int)NEAREST_MIPMAP_LINEAR;
                    break;
                case TextureFilterMode.LinearMipmapLinear:
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
                case TextureWrapMode.Repeat:
                    return (int)REPEAT;
                case TextureWrapMode.MirroredRepeat:
                    return (int)MIRRORED_REPEAT;
                case TextureWrapMode.ClampToEdge:
                    return (int)CLAMP_TO_EDGE;
                case TextureWrapMode.ClampToBorder:
                    {
#warning TextureWrapMode.CLAMP_TO_BORDER is not supported on Android. CLAMP_TO_EDGE is set instead.
                        return (int)CLAMP_TO_EDGE;
                    }
            }
        }

        private uint GetDepthCompareFunc(Compare compareFunc)
        {
            switch (compareFunc)
            {
                case Compare.Never:
                    return NEVER;

                case Compare.Less:
                    return LESS;

                case Compare.Equal:
                    return EQUAL;

                case Compare.LessEqual:
                    return LEQUAL;

                case Compare.Greater:
                    return GREATER;

                case Compare.NotEqual:
                    return NOTEQUAL;

                case Compare.GreaterEqual:
                    return GEQUAL;

                case Compare.Always:
                    return ALWAYS;

                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        private TexturePixelInfo GetTexturePixelInfo(ITextureBase tex)
        {
            uint internalFormat;
            uint format;
            uint pxType;

            switch (tex.PixelFormat.ColorFormat)
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
                    break;
                // TODO: Handle Alpha-only / Intensity-only and AlphaIntensity correctly.
                case ColorFormat.Intensity:
                    internalFormat = ALPHA;
                    format = ALPHA;
                    pxType = UNSIGNED_BYTE;
                    break;
                case ColorFormat.Depth24:
                    internalFormat = DEPTH_COMPONENT24;
                    format = DEPTH_COMPONENT;
                    pxType = FLOAT;
                    break;
                case ColorFormat.Depth16:
                    internalFormat = DEPTH_COMPONENT16;
                    format = DEPTH_COMPONENT;
                    pxType = FLOAT;
                    break;
                case ColorFormat.uiRgb8:
                    internalFormat = RGBA8UI;
                    format = RGBA;
                    pxType = UNSIGNED_BYTE;

                    break;
                case ColorFormat.fRGB32:
                    internalFormat = RGB32F;
                    format = RGB;
                    pxType = FLOAT;

                    break;
                case ColorFormat.fRGB16:
                    internalFormat = RGB16F;
                    format = RGB;
                    pxType = FLOAT;

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
        /// Creates a new CubeMap and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableCubeMap img)
        {
            WebGLTexture id = gl2.CreateTexture();
            gl2.BindTexture(TEXTURE_CUBE_MAP, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;

            var glWrapMode = GetWrapMode(img.WrapMode);
            var pxInfo = GetTexturePixelInfo(img);

            for (int i = 0; i < 6; i++)
            {
                gl2.TexImage2D(TEXTURE_CUBE_MAP_POSITIVE_X + (uint)i, 0, (int)pxInfo.InternalFormat, img.Width, img.Height, 0, pxInfo.Format, pxInfo.PxType, IntPtr.Zero);
            }

            gl2.TexParameteri(TEXTURE_CUBE_MAP, TEXTURE_MAG_FILTER, (int)magFilter);
            gl2.TexParameteri(TEXTURE_CUBE_MAP, TEXTURE_MIN_FILTER, (int)minFilter);
            gl2.TexParameteri(TEXTURE_CUBE_MAP, TEXTURE_WRAP_S, (int)glWrapMode);
            gl2.TexParameteri(TEXTURE_CUBE_MAP, TEXTURE_WRAP_T, (int)glWrapMode);
            gl2.TexParameteri(TEXTURE_CUBE_MAP, TEXTURE_WRAP_R, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = id };

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param> 
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(ITexture img)
        {
            WebGLTexture id = gl2.CreateTexture();
            gl2.BindTexture(TEXTURE_CUBE_MAP, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;

            var glWrapMode = GetWrapMode(img.WrapMode);
            var pxInfo = GetTexturePixelInfo(img);

            var imageData = gl.CastNativeArray(img.PixelData);
            gl.TexImage2D(TEXTURE_2D, 0, pxInfo.InternalFormat, img.Width, img.Height, 0, pxInfo.Format, pxInfo.PxType, imageData);

            if (img.DoGenerateMipMaps)
                gl.GenerateMipmap(TEXTURE_2D);

            gl2.TexParameteri(TEXTURE_2D, TEXTURE_MAG_FILTER, (int)magFilter);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_MIN_FILTER, (int)minFilter);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_S, (int)glWrapMode);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_T, (int)glWrapMode);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_R, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = id };

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param> 
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableTexture img)
        {
            WebGLTexture id = gl2.CreateTexture();
            gl2.BindTexture(TEXTURE_CUBE_MAP, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;

            var glWrapMode = GetWrapMode(img.WrapMode);
            var pxInfo = GetTexturePixelInfo(img);

            var imgData = gl.CastNativeArray(new byte[0]); //TODO: how to create an empty(?) ITypedArray?
            gl.TexImage2D(TEXTURE_2D, 0, pxInfo.InternalFormat, img.Width, img.Height, 0, pxInfo.Format, pxInfo.PxType, imgData);

            if (img.DoGenerateMipMaps)
                gl.GenerateMipmap(TEXTURE_2D);

            gl2.TexParameteri(TEXTURE_2D, TEXTURE_COMPARE_MODE, (int)GetTexComapreMode(img.CompareMode));
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_COMPARE_FUNC, (int)GetDepthCompareFunc(img.CompareFunc));
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_MAG_FILTER, (int)magFilter);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_MIN_FILTER, (int)minFilter);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_S, (int)glWrapMode);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_T, (int)glWrapMode);
            gl2.TexParameteri(TEXTURE_2D, TEXTURE_WRAP_R, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = id };

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
            uint pixelFormat = GetTexturePixelInfo(img).Format;

            // copy the bytes from image to GPU texture
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

            gl.BindTexture(TEXTURE_2D, ((TextureHandle)tex).TexHandle);
            var imageData = gl.CastNativeArray(bytes);
            gl.TexSubImage2D(TEXTURE_2D, 0, startX, startY, width, height, pixelFormat, UNSIGNED_BYTE, imageData);

            gl.GenerateMipmap(TEXTURE_2D);

            gl.TexParameteri(TEXTURE_2D, TEXTURE_MIN_FILTER, (int)LINEAR_MIPMAP_LINEAR);
            gl.TexParameteri(TEXTURE_2D, TEXTURE_MAG_FILTER, (int)LINEAR_MIPMAP_LINEAR);
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
                gl.DeleteFramebuffer(texHandle.FrameBufferHandle);
            }

            if (texHandle.DepthRenderBufferHandle != null)
            {
                gl.DeleteRenderbuffer(texHandle.DepthRenderBufferHandle);
            }

            if (texHandle.TexHandle != null)
            {
                gl.DeleteTexture(texHandle.TexHandle);
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
            statusCode = (bool)gl.GetShaderParameter(fragmentObject, COMPILE_STATUS);

            if (!statusCode)
                throw new ApplicationException(info);

            WebGLProgram program = gl.CreateProgram();
            gl.AttachShader(program, fragmentObject);
            gl.AttachShader(program, vertexObject);

            // enable GLSL (ES) shaders to use fuVertex, fuColor and fuNormal attributes
            gl.BindAttribLocation(program, (uint)AttributeLocations.VertexAttribLocation, UniformNameDeclarations.Vertex);
            gl.BindAttribLocation(program, (uint)AttributeLocations.ColorAttribLocation, UniformNameDeclarations.Color);
            gl.BindAttribLocation(program, (uint)AttributeLocations.UvAttribLocation, UniformNameDeclarations.TextureCoordinates);
            gl.BindAttribLocation(program, (uint)AttributeLocations.NormalAttribLocation, UniformNameDeclarations.Normal);
            gl.BindAttribLocation(program, (uint)AttributeLocations.TangentAttribLocation, UniformNameDeclarations.TangentAttribName);
            gl.BindAttribLocation(program, (uint)AttributeLocations.BoneIndexAttribLocation, UniformNameDeclarations.BoneIndex);
            gl.BindAttribLocation(program, (uint)AttributeLocations.BoneWeightAttribLocation, UniformNameDeclarations.BoneWeight);
            gl.BindAttribLocation(program, (uint)AttributeLocations.BitangentAttribLocation, UniformNameDeclarations.BitangentAttribName);

            gl.LinkProgram(program);

            return new ShaderHandleImp { Handle = program };
        }

        /// <inheritdoc />
        /// <summary>
        /// Removes shader from the GPU
        /// </summary>
        /// <param name="sp"></param>
        public void RemoveShader(IShaderHandle sp)
        {
            if (sp == null) return;

            var program = ((ShaderHandleImp)sp).Handle;

            // wait for all threads to be finished
            gl.Finish();
            gl.Flush();

            // cleanup
            // gl.DeleteShader(program);
            gl.DeleteProgram(program);
        }

        /// <summary>
        /// Sets the shader program onto the GL render context.
        /// </summary>
        /// <param name="program">The shader program.</param>
        public void SetShader(IShaderHandle program)
        {
            _textureCountPerShader = 0;
            _shaderParam2TexUnit.Clear();

            gl.UseProgram(((ShaderHandleImp)program).Handle);
        }

        /// <summary>
        /// Set the width of line primitives.
        /// </summary>
        /// <param name="width"></param>
        public void SetLineWidth(float width)
        {
            gl.LineWidth(width);
        }

        /// <summary>
        /// Gets the shader parameter.
        /// The Shader parameter is used to bind values inside of shader programs that run on the graphics card.
        /// Do not use this function in frequent updates as it transfers information from graphics card to the cpu which takes time.
        /// </summary>
        /// <param name="shaderProgram">The shader program.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>The Shader parameter is returned if the name is found, otherwise null.</returns>
        public IShaderParam GetShaderParam(IShaderHandle shaderProgram, string paramName)
        {
            WebGLUniformLocation h = gl.GetUniformLocation(((ShaderHandleImp)shaderProgram).Handle, paramName);
            return (h == null) ? null : new ShaderParam { handle = h };
        }

        /// <summary>
        /// Gets the float parameter value inside a shader program by using a <see cref="IShaderParam" /> as search reference.
        /// Do not use this function in frequent updates as it transfers information from the graphics card to the cpu which takes time.
        /// </summary>
        /// <param name="program">The shader program.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>The current parameter's value.</returns>
        public float GetParamValue(IShaderHandle program, IShaderParam param)
        {
            float f = (float)gl.GetUniform(((ShaderHandleImp)program).Handle, ((ShaderParam)param).handle);
            return f;
        }


        /// <summary>
        /// Gets the shader parameter list of a specific <see cref="IShaderHandle" />. 
        /// </summary>
        /// <param name="shaderProgram">The shader program.</param>
        /// <returns>All Shader parameters of a shader program are returned.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IList<ShaderParamInfo> GetShaderParamList(IShaderHandle shaderProgram)
        {
            var sProg = (ShaderHandleImp)shaderProgram;
            var paramList = new List<ShaderParamInfo>();

            int nParams;
            nParams = (int)gl.GetProgramParameter(sProg.Handle, ACTIVE_UNIFORMS);

            for (uint i = 0; i < nParams; i++)
            {
                WebGLActiveInfo activeInfo = gl.GetActiveUniform(sProg.Handle, i);

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
                    case UNSIGNED_INT_SAMPLER_2D:
                    case INT_SAMPLER_2D:
                    case SAMPLER_2D_SHADOW:
                        paramInfo.Type = typeof(ITextureBase);
                        break;
                    case SAMPLER_CUBE_SHADOW:
                    case SAMPLER_CUBE:
                        paramInfo.Type = typeof(ITextureBase);
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
        /// Sets a <see cref="float2" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IShaderParam param, float2[] val)
        {
            fixed (float2* pFlt = &val[0])
                gl.Uniform2fv(((ShaderParam)param).handle, new Span<float>((float*)pFlt, val.Length * 2));
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
                gl.Uniform4fv(((ShaderParam)param).handle, new Span<float>((float*)pFlt, val.Length * 4));
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
                gl.UniformMatrix4fv(((ShaderParam)param).handle, true, new Span<float>((float*)pMtx, val.Length * 16));
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

        private void BindTextureByTarget(ITextureHandle texId, TextureType texTarget)
        {
            switch (texTarget)
            {
                case TextureType.Texture1D:
                    Diagnostics.Error("OpenTK ES31 does not support Texture1D.");
                    break;
                case TextureType.Texture2D:
                    gl.BindTexture(TEXTURE_2D, ((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.Texture3D:
                    gl.BindTexture(TEXTURE_3D, ((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.TextureCubeMap:
                    gl.BindTexture(TEXTURE_CUBE_MAP, ((TextureHandle)texId).TexHandle);
                    break;
                default:
                    break;
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
            var iParam = ((ShaderParam)param).handle;
            if (!_shaderParam2TexUnit.TryGetValue(iParam, out int texUnit))
            {
                _textureCountPerShader++;
                texUnit = _textureCountPerShader;
                _shaderParam2TexUnit[iParam] = texUnit;
            }

            gl.ActiveTexture((uint)(TEXTURE0 + texUnit));
            BindTextureByTarget(texId, texTarget);
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
            var iParam = ((ShaderParam)param).handle;
            if (!_shaderParam2TexUnit.TryGetValue(iParam, out texUnit))
            {
                _textureCountPerShader++;
                texUnit = _textureCountPerShader;
                _shaderParam2TexUnit[iParam] = texUnit;
            }

            gl.ActiveTexture((uint)(TEXTURE0 + texUnit));
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
            var iParam = ((ShaderParam)param).handle;
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

                gl.ActiveTexture((uint)(TEXTURE0 + firstTexUnit + i));
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
            var iParam = ((ShaderParam)param).handle;
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

                gl.ActiveTexture((uint)(TEXTURE0 + firstTexUnit + i));
                BindTextureByTarget(texIds[i], texTarget);
            }
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
            gl.Uniform1i(((ShaderParam)param).handle, texUnit);
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
            gl.Uniform1i(((ShaderParam)param).handle, texUnitArray[0]);
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
                float[] ret = (float[])gl.GetParameter(COLOR_CLEAR_VALUE);
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
                return (float)gl.GetParameter(DEPTH_CLEAR_VALUE);
            }
            set { gl.ClearDepth(value); }
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
            gl.Scissor(x, y, width, height);
        }

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
            vboBytes = (int)gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
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
        /// <param name="attribHandle">The attribute handle.</param>
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
        /// Binds the vertices onto the GL render context and assigns an VertexBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="vertices">The vertices.</param>
        /// <exception cref="ArgumentException">Vertices must not be null or empty</exception>
        /// <exception cref="ApplicationException"></exception>
        public void SetVertices(IMeshImp mr, float3[] vertices)
        {
            Diagnostics.Debug("[SetVertices]");
            if (vertices == null || vertices.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            int vboBytes;
            int vertsBytes = vertices.Length * 3 * sizeof(float);
            if (((MeshImp)mr).VertexBufferObject == null)
                ((MeshImp)mr).VertexBufferObject = gl.CreateBuffer();

            gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).VertexBufferObject);
            Diagnostics.Debug("[Before gl.BufferData]");

            var verticesFlat = new float[vertices.Length * 3];
            unsafe
            {
                fixed (float3* pBytes = &vertices[0])
                {
                    Marshal.Copy((IntPtr)(pBytes), verticesFlat, 0, verticesFlat.Length);
                }
            }
            gl.BufferData(ARRAY_BUFFER, verticesFlat, STATIC_DRAW);

            Diagnostics.Debug("[After gl.BufferData]");
            vboBytes = (int)gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format("Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.", vertsBytes, vboBytes));

        }

        /// <summary>
        /// Binds the tangents onto the GL render context and assigns an TangentBuffer index to the passed <see cref="IMeshImp" /> instance.
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
            vboBytes = (int)gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != tangentBytes)
                throw new ApplicationException(String.Format("Problem uploading vertex buffer to VBO (tangents). Tried to upload {0} bytes, uploaded {1}.", tangentBytes, vboBytes));

        }

        /// <summary>
        /// Binds the bitangents onto the GL render context and assigns an BiTangentBuffer index to the passed <see cref="IMeshImp" /> instance.
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
            vboBytes = (int)gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != bitangentBytes)
                throw new ApplicationException(String.Format("Problem uploading vertex buffer to VBO (bitangents). Tried to upload {0} bytes, uploaded {1}.", bitangentBytes, vboBytes));
        }

        /// <summary>
        /// Binds the normals onto the GL render context and assigns an NormalBuffer index to the passed <see cref="IMeshImp" /> instance.
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

            vboBytes = (int)gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != normsBytes)
                throw new ApplicationException(String.Format("Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.", normsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the bone indices onto the GL render context and assigns an BondeIndexBuffer index to the passed <see cref="IMeshImp" /> instance.
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
            vboBytes = (int)gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != indicesBytes)
                throw new ApplicationException(String.Format("Problem uploading bone indices buffer to VBO (bone indices). Tried to upload {0} bytes, uploaded {1}.", indicesBytes, vboBytes));
        }

        /// <summary>
        /// Binds the bone weights onto the GL render context and assigns an BondeWeightBuffer index to the passed <see cref="IMeshImp" /> instance.
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
            vboBytes = (int)gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != weightsBytes)
                throw new ApplicationException(String.Format("Problem uploading bone weights buffer to VBO (bone weights). Tried to upload {0} bytes, uploaded {1}.", weightsBytes, vboBytes));

        }

        /// <summary>
        /// Binds the UV coordinates onto the GL render context and assigns an UVBuffer index to the passed <see cref="IMeshImp" /> instance.
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
            vboBytes = (int)gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != uvsBytes)
                throw new ApplicationException(String.Format("Problem uploading uv buffer to VBO (uvs). Tried to upload {0} bytes, uploaded {1}.", uvsBytes, vboBytes));

        }

        /// <summary>
        /// Binds the colors onto the GL render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
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
            vboBytes = (int)gl.GetBufferParameter(ARRAY_BUFFER, BUFFER_SIZE);
            if (vboBytes != colsBytes)
                throw new ApplicationException(String.Format("Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.", colsBytes, vboBytes));
        }

        /// <summary>
        /// Binds the triangles onto the GL render context and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
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
                gl.EnableVertexAttribArray((uint)AttributeLocations.VertexAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).VertexBufferObject);
                gl.VertexAttribPointer((uint)AttributeLocations.VertexAttribLocation, 3, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).ColorBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)AttributeLocations.ColorAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).ColorBufferObject);
                gl.VertexAttribPointer((uint)AttributeLocations.ColorAttribLocation, 4, UNSIGNED_BYTE, true, 0, 0);
            }

            if (((MeshImp)mr).UVBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)AttributeLocations.UvAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).UVBufferObject);
                gl.VertexAttribPointer((uint)AttributeLocations.UvAttribLocation, 2, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).NormalBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)AttributeLocations.NormalAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).NormalBufferObject);
                gl.VertexAttribPointer((uint)AttributeLocations.NormalAttribLocation, 3, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).TangentBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)AttributeLocations.TangentAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).TangentBufferObject);
                gl.VertexAttribPointer((uint)AttributeLocations.TangentAttribLocation, 3, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).BitangentBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)AttributeLocations.BitangentAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).BitangentBufferObject);
                gl.VertexAttribPointer((uint)AttributeLocations.BitangentAttribLocation, 3, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).BoneIndexBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)AttributeLocations.BoneIndexAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).BoneIndexBufferObject);
                gl.VertexAttribPointer((uint)AttributeLocations.BoneIndexAttribLocation, 4, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).BoneWeightBufferObject != null)
            {
                gl.EnableVertexAttribArray((uint)AttributeLocations.BoneWeightAttribLocation);
                gl.BindBuffer(ARRAY_BUFFER, ((MeshImp)mr).BoneWeightBufferObject);
                gl.VertexAttribPointer((uint)AttributeLocations.BoneWeightAttribLocation, 4, FLOAT, false, 0, 0);
            }
            if (((MeshImp)mr).ElementBufferObject != null)
            {
                gl.BindBuffer(ELEMENT_ARRAY_BUFFER, ((MeshImp)mr).ElementBufferObject);
                gl.DrawElements(TRIANGLES, ((MeshImp)mr).NElements, UNSIGNED_SHORT, 0);
                //gl.DrawArrays(gl.Enums.BeginMode.POINTS, 0, shape.Vertices.Length);
            }
            if (((MeshImp)mr).ElementBufferObject != null)
            {
                gl.BindBuffer(ELEMENT_ARRAY_BUFFER, ((MeshImp)mr).ElementBufferObject);

                switch (((MeshImp)mr).MeshType)
                {
                    case OpenGLPrimitiveType.Triangles:
                    default:
                        gl.DrawElements(TRIANGLES, ((MeshImp)mr).NElements, UNSIGNED_SHORT, 0);
                        break;
                    case OpenGLPrimitiveType.Points:                        
                        gl.DrawElements(POINTS, ((MeshImp)mr).NElements, UNSIGNED_SHORT, 0);
                        break;
                    case OpenGLPrimitiveType.Lines:                       
                        gl.DrawElements(LINES, ((MeshImp)mr).NElements, UNSIGNED_SHORT, 0);
                        break;
                    case OpenGLPrimitiveType.LineLoop:                        
                        gl.DrawElements(LINE_LOOP, ((MeshImp)mr).NElements, UNSIGNED_SHORT, 0);
                        break;
                    case OpenGLPrimitiveType.LineStrip:                        
                        gl.DrawElements(LINE_STRIP, ((MeshImp)mr).NElements, UNSIGNED_SHORT, 0);
                        break;
                    case OpenGLPrimitiveType.Patches:
                        gl.DrawElements(TRIANGLES, ((MeshImp)mr).NElements, UNSIGNED_SHORT, 0);
                        Diagnostics.Warn("Mesh type set to triangles due to unavailability of PATCHES");
                        break;
                    case OpenGLPrimitiveType.QuadStrip:
                        gl.DrawElements(TRIANGLES, ((MeshImp)mr).NElements, UNSIGNED_SHORT, 0);
                        Diagnostics.Warn("Mesh type set to triangles due to unavailability of QUAD_STRIP");
                        break;
                    case OpenGLPrimitiveType.TriangleFan:
                        gl.DrawElements(TRIANGLE_FAN, ((MeshImp)mr).NElements, UNSIGNED_SHORT, 0);
                        break;
                    case OpenGLPrimitiveType.TriangleStrip:
                        gl.DrawElements(TRIANGLE_STRIP, ((MeshImp)mr).NElements, UNSIGNED_SHORT, 0);
                        break;
                }
            }


            if (((MeshImp)mr).VertexBufferObject != null)
            {
                gl.BindBuffer(ARRAY_BUFFER, null);
                gl.DisableVertexAttribArray((uint)AttributeLocations.VertexAttribLocation);
            }
            if (((MeshImp)mr).ColorBufferObject != null)
            {
                gl.BindBuffer(ARRAY_BUFFER, null);
                gl.DisableVertexAttribArray((uint)AttributeLocations.ColorAttribLocation);
            }
            if (((MeshImp)mr).NormalBufferObject != null)
            {
                gl.BindBuffer(ARRAY_BUFFER, null);
                gl.DisableVertexAttribArray((uint)AttributeLocations.NormalAttribLocation);
            }
            if (((MeshImp)mr).UVBufferObject != null)
            {
                gl.BindBuffer(ARRAY_BUFFER, null);
                gl.DisableVertexAttribArray((uint)AttributeLocations.UvAttribLocation);
            }
            if (((MeshImp)mr).TangentBufferObject != null)
            {
                gl.BindBuffer(ARRAY_BUFFER, null);
                gl.DisableVertexAttribArray((uint)AttributeLocations.TangentAttribLocation);
            }
            if (((MeshImp)mr).BitangentBufferObject != null)
            {
                gl.BindBuffer(ARRAY_BUFFER, null);
                gl.DisableVertexAttribArray((uint)AttributeLocations.TangentAttribLocation);
            }
        }

        /// <summary>
        /// Gets the content of the buffer.
        /// </summary>
        /// <param name="quad">The Rectangle where the content is draw into.</param>
        /// <param name="texId">The texture identifier.</param>
        public void GetBufferContent(Rectangle quad, ITextureHandle texId)
        {
            gl.BindTexture(TEXTURE_2D, ((TextureHandle)texId).TexHandle);
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
                    throw new ArgumentOutOfRangeException($"Invalid argument: {bo}");
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
                    throw new ArgumentOutOfRangeException($"Invalid argument: {bom}");
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
            gl.Enable(SCISSOR_TEST);

            switch (renderState)
            {
                case RenderState.FillMode:
                    {
                        if (value != (uint)FillMode.Solid)
                            throw new NotSupportedException("Line or Point fill mode (glPolygonMode) not supported in WebGL!");
                    }
                    break;
                case RenderState.CullMode:
                    {
                        switch ((Cull)value)
                        {
                            case Cull.None:
                                if (_isCullEnabled)
                                {
                                    _isCullEnabled = false;
                                    gl.Disable(CULL_FACE);
                                }
                                gl.FrontFace(NONE);
                                break;
                            case Cull.Clockwise:
                                if (!_isCullEnabled)
                                {
                                    _isCullEnabled = true;
                                    gl.Enable(CULL_FACE);
                                }
                                gl.FrontFace(CW);
                                break;
                            case Cull.Counterclockwise:
                                if (!_isCullEnabled)
                                {
                                    _isCullEnabled = true;
                                    gl.Enable(CULL_FACE);
                                }
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
                        uint df = GetDepthCompareFunc((Compare)value);
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
                    _blendEquationRgb = BlendOperationToOgl((BlendOperation)value);
                    gl.BlendEquationSeparate(_blendEquationRgb, _blendEquationAlpha);
                    break;
                case RenderState.BlendOperationAlpha:
                    _blendEquationAlpha = BlendOperationToOgl((BlendOperation)value);
                    gl.BlendEquationSeparate(_blendEquationRgb, _blendEquationAlpha);
                    break;
                case RenderState.SourceBlend:
                    {
                        _blendSrcRgb = BlendToOgl((Blend)value);
                        gl.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.DestinationBlend:
                    {
                        _blendDstRgb = BlendToOgl((Blend)value);
                        gl.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.SourceBlendAlpha:
                    {
                        _blendSrcAlpha = BlendToOgl((Blend)value);
                        gl.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.DestinationBlendAlpha:
                    {
                        _blendDstAlpha = BlendToOgl((Blend)value);
                        gl.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
                    }
                    break;
                case RenderState.BlendFactor:
                    var blendcolor = ColorUint.Tofloat4((ColorUint)value);
                    gl.BlendColor(blendcolor.r, blendcolor.g, blendcolor.b, blendcolor.a);
                    break;
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
                        return (uint)FillMode.Solid;
                    }
                case RenderState.CullMode:
                    {
                        uint cullFace;
                        cullFace = (uint)gl.GetParameter(CULL_FACE);
                        if (cullFace == 0)
                            return (uint)Cull.None;
                        uint frontFace;
                        frontFace = (uint)gl.GetParameter(FRONT_FACE);
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
                        depFunc = (uint)gl.GetParameter(DEPTH_FUNC);
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
                        return (uint)ret;
                    }
                case RenderState.ZEnable:
                    {
                        uint depTest;
                        depTest = (uint)gl.GetParameter(DEPTH_TEST);
                        return (uint)(depTest);
                    }
                case RenderState.ZWriteEnable:
                    {
                        uint depWriteMask;
                        depWriteMask = (uint)gl.GetParameter(DEPTH_WRITEMASK);
                        return (uint)(depWriteMask);
                    }
                case RenderState.AlphaBlendEnable:
                    {
                        uint blendEnable;
                        blendEnable = (uint)gl.GetParameter(BLEND);
                        return (uint)(blendEnable);
                    }
                case RenderState.BlendOperation:
                    {
                        uint rgbMode;
                        rgbMode = (uint)gl.GetParameter(BLEND_EQUATION_RGB);
                        return (uint)BlendOperationFromOgl(rgbMode);
                    }
                case RenderState.BlendOperationAlpha:
                    {
                        uint alphaMode;
                        alphaMode = (uint)gl.GetParameter(BLEND_EQUATION_ALPHA);
                        return (uint)BlendOperationFromOgl(alphaMode);
                    }
                case RenderState.SourceBlend:
                    {
                        uint rgbSrc;
                        rgbSrc = (uint)gl.GetParameter(BLEND_SRC_RGB);
                        return (uint)BlendFromOgl(rgbSrc);
                    }
                case RenderState.DestinationBlend:
                    {
                        uint rgbDst;
                        rgbDst = (uint)gl.GetParameter(BLEND_DST_RGB);
                        return (uint)BlendFromOgl(rgbDst);
                    }
                case RenderState.SourceBlendAlpha:
                    {
                        uint alphaSrc;
                        alphaSrc = (uint)gl.GetParameter(BLEND_SRC_ALPHA);
                        return (uint)BlendFromOgl(alphaSrc);
                    }
                case RenderState.DestinationBlendAlpha:
                    {
                        uint alphaDst;
                        alphaDst = (uint)gl.GetParameter(BLEND_DST_ALPHA);
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
        /// Renders into the given texture.
        /// </summary>
        /// <param name="tex">The texture.</param>
        /// <param name="texHandle">The texture handle, associated with the given texture. Should be created by the TextureManager in the RenderContext.</param>
        public void SetRenderTarget(IWritableTexture tex, ITextureHandle texHandle)
        {
            if (((TextureHandle)texHandle).FrameBufferHandle == null)
            {
                var fBuffer = gl2.CreateFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                gl2.BindFramebuffer(FRAMEBUFFER, fBuffer);

                gl2.BindTexture(TEXTURE_2D, ((TextureHandle)texHandle).TexHandle);

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    CreateDepthRenderBuffer(tex.Width, tex.Height);
                    gl2.FramebufferTexture2D(FRAMEBUFFER, COLOR_ATTACHMENT0, TEXTURE_2D, ((TextureHandle)texHandle).TexHandle, 0);
                    gl2.DrawBuffers(new uint[] { COLOR_ATTACHMENT0 });
                }
                else
                {
                    gl2.FramebufferTexture2D(FRAMEBUFFER, DEPTH_ATTACHMENT, TEXTURE_2D, ((TextureHandle)texHandle).TexHandle, 0);
                    gl2.DrawBuffers(new uint[] { NONE });
                    gl2.ReadBuffer(NONE);
                }
            }
            else
                gl2.BindFramebuffer(FRAMEBUFFER, ((TextureHandle)texHandle).FrameBufferHandle);

            if (gl2.CheckFramebufferStatus(FRAMEBUFFER) != FRAMEBUFFER_COMPLETE)
                throw new Exception($"Error creating RenderTarget: {gl2.GetError()}, {gl2.CheckFramebufferStatus(FRAMEBUFFER)}");

            gl2.Clear(DEPTH_BUFFER_BIT | COLOR_BUFFER_BIT);
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
                var fBuffer = gl2.CreateFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                gl2.BindFramebuffer(FRAMEBUFFER, fBuffer);

                gl2.BindTexture(TEXTURE_CUBE_MAP, ((TextureHandle)texHandle).TexHandle);

                if (tex.TextureType != RenderTargetTextureTypes.Depth)
                {
                    CreateDepthRenderBuffer(tex.Width, tex.Height);
                    gl2.FramebufferTexture2D(FRAMEBUFFER, COLOR_ATTACHMENT0, TEXTURE_CUBE_MAP, ((TextureHandle)texHandle).TexHandle, 0);
                    gl2.DrawBuffers(new uint[] { COLOR_ATTACHMENT0 });
                }
                else
                {
                    gl2.FramebufferTexture2D(FRAMEBUFFER, DEPTH_ATTACHMENT, TEXTURE_CUBE_MAP, ((TextureHandle)texHandle).TexHandle, 0);
                    gl2.DrawBuffers(new uint[] { NONE });
                    gl2.ReadBuffer(NONE);
                }
            }
            else
                gl2.BindFramebuffer(FRAMEBUFFER, ((TextureHandle)texHandle).FrameBufferHandle);

            if (gl2.CheckFramebufferStatus(FRAMEBUFFER) != FRAMEBUFFER_COMPLETE)
                throw new Exception($"Error creating RenderTarget: {gl2.GetError()}, {gl2.CheckFramebufferStatus(FRAMEBUFFER)}");


            gl2.Clear(DEPTH_BUFFER_BIT | COLOR_BUFFER_BIT);
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

            if (gl2.CheckFramebufferStatus(FRAMEBUFFER) != FRAMEBUFFER_COMPLETE)
            {
                throw new Exception($"Error creating RenderTarget: {gl2.GetError()}, {gl2.CheckFramebufferStatus(FRAMEBUFFER)}");
            }

            gl2.Clear(DEPTH_CLEAR_VALUE | COLOR_CLEAR_VALUE);
        }

        private WebGLRenderbuffer CreateDepthRenderBuffer(int width, int height)
        {
            gl2.Enable(DEPTH_TEST);

            var gDepthRenderbuffer = gl2.CreateRenderbuffer();
            gl2.BindRenderbuffer(RENDERBUFFER, gDepthRenderbuffer);
            gl2.RenderbufferStorage(RENDERBUFFER, DEPTH_COMPONENT24, width, height);
            gl2.FramebufferRenderbuffer(FRAMEBUFFER, DEPTH_ATTACHMENT, RENDERBUFFER, gDepthRenderbuffer);
            return gDepthRenderbuffer;
        }

        private WebGLFramebuffer CreateFrameBuffer(IRenderTarget renderTarget, ITextureHandle[] texHandles)
        {
            var gBuffer = gl2.CreateFramebuffer();
            gl2.BindFramebuffer(FRAMEBUFFER, gBuffer);

            int depthCnt = 0;

            var depthTexPos = (int)RenderTargetTextureTypes.Depth;

            if (!renderTarget.IsDepthOnly)
            {
                var attachments = new List<uint>();

                //Textures
                for (int i = 0; i < texHandles.Length; i++)
                {
                    attachments.Add(COLOR_ATTACHMENT0 + (uint)i);

                    var texHandle = texHandles[i];
                    if (texHandle == null) continue;

                    if (i == depthTexPos)
                    {
                        gl2.FramebufferTexture2D(FRAMEBUFFER, DEPTH_ATTACHMENT + (uint)depthCnt, TEXTURE_2D, ((TextureHandle)texHandle).TexHandle, 0);
                        depthCnt++;
                    }
                    else
                        gl2.FramebufferTexture2D(FRAMEBUFFER, COLOR_ATTACHMENT0 + (uint)(i - depthCnt), TEXTURE_2D, ((TextureHandle)texHandle).TexHandle, 0);
                }
                gl2.DrawBuffers(attachments.ToArray());
            }
            else //If a frame-buffer only has a depth texture we don't need draw buffers
            {
                var texHandle = texHandles[depthTexPos];

                if (texHandle != null)
                    gl2.FramebufferTexture2D(FRAMEBUFFER, DEPTH_ATTACHMENT, TEXTURE_2D, ((TextureHandle)texHandle).TexHandle, 0);
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
            ChangeFramebufferTexture2D(renderTarget, attachment, ((TextureHandle)texHandle).TexHandle, isDepthTex);
        }

        private void ChangeFramebufferTexture2D(IRenderTarget renderTarget, int attachment, WebGLTexture handle, bool isDepth)
        {
            var boundFbo = (WebGLFramebuffer)gl2.GetParameter(FRAMEBUFFER_BINDING);
            var rtFbo = ((FrameBufferHandle)renderTarget.GBufferHandle).Handle;

            var isCurrentFbo = true;

            if (boundFbo != rtFbo)
            {
                isCurrentFbo = false;
                gl2.BindFramebuffer(FRAMEBUFFER, rtFbo);
            }

            if (!isDepth)
                gl2.FramebufferTexture2D(FRAMEBUFFER, COLOR_ATTACHMENT0 + (uint)attachment, TEXTURE_2D, handle, 0);
            else
                gl2.FramebufferTexture2D(FRAMEBUFFER, DEPTH_ATTACHMENT, TEXTURE_2D, handle, 0);

            if (gl2.CheckFramebufferStatus(FRAMEBUFFER) != FRAMEBUFFER_COMPLETE)
                throw new Exception($"Error creating RenderTarget: {gl2.GetError()}, {gl2.CheckFramebufferStatus(FRAMEBUFFER)}");

            if (!isCurrentFbo)
                gl2.BindFramebuffer(FRAMEBUFFER, boundFbo);
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
                case HardwareCapability.CanRenderDeferred:
                    return 0U;
                case HardwareCapability.CanUseGeometryShaders:
                    return 0U; //WASM uses OpenGL es, where no geometry shaders can be used.
                default:
                    throw new ArgumentOutOfRangeException(nameof(capability), capability, null);
            }
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
            var vertices = new float3[]
            {
                new float3(start.x, start.y, start.z),
                new float3(end.x, end.y, end.z),
            };

            var itemSize = 3;
            var numItems = 2;
            var posBuffer = gl.CreateBuffer();

            gl.EnableVertexAttribArray((uint)AttributeLocations.VertexAttribLocation);
            gl.BindBuffer(ARRAY_BUFFER, posBuffer);
            gl.BufferData(ARRAY_BUFFER, vertices, STATIC_DRAW);
            gl.VertexAttribPointer((uint)AttributeLocations.VertexAttribLocation, itemSize, FLOAT, false, 0, 0);

            gl.DrawArrays(LINE_STRIP, 0, numItems);
            gl.DisableVertexAttribArray((uint)AttributeLocations.VertexAttribLocation);
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