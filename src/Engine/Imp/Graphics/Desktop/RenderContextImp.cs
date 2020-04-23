using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using OpenTK;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContextImp"/> class.
        /// </summary>
        /// <param name="renderCanvas">The render canvas interface.</param>
        public RenderContextImp(IRenderCanvasImp renderCanvas)
        {
            _textureCountPerShader = 0;
            _shaderParam2TexUnit = new Dictionary<int, int>();

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


            _blendDstRgb = (BlendingFactorDest)blendDstRgb;
            _blendSrcRgb = (BlendingFactorSrc)blendSrcRgb;
            _blendSrcAlpha = (BlendingFactorSrc)blendSrcAlpha;
            _blendDstAlpha = (BlendingFactorDest)blendDstAlpha;
            _blendEquationAlpha = (BlendEquationMode)blendEqA;
            _blendEquationRgb = (BlendEquationMode)blendEqRgb;

            Diagnostics.Debug(GetHardwareDescription());
        }

        #region Image data related Members

        private OpenTK.Graphics.OpenGL.TextureCompareMode GetTexComapreMode(Common.TextureCompareMode compareMode)
        {
            switch (compareMode)
            {
                case Common.TextureCompareMode.NONE:
                    return OpenTK.Graphics.OpenGL.TextureCompareMode.None;

                case Common.TextureCompareMode.GL_COMPARE_REF_TO_TEXTURE:
                    return OpenTK.Graphics.OpenGL.TextureCompareMode.CompareRefToTexture;

                default:
                    throw new ArgumentException("Invalid compare mode.");
            }
        }

        private Tuple<TextureMinFilter, TextureMagFilter> GetMinMagFilter(TextureFilterMode filterMode)
        {
            TextureMinFilter minFilter;
            TextureMagFilter magFilter;

            switch (filterMode)
            {
                case TextureFilterMode.NEAREST:
                    minFilter = TextureMinFilter.Nearest;
                    magFilter = TextureMagFilter.Nearest;
                    break;
                default:
                case TextureFilterMode.LINEAR:
                    minFilter = TextureMinFilter.Linear;
                    magFilter = TextureMagFilter.Linear;
                    break;
                case TextureFilterMode.NEAREST_MIPMAP_NEAREST:
                    minFilter = TextureMinFilter.NearestMipmapNearest;
                    magFilter = TextureMagFilter.Nearest;
                    break;
                case TextureFilterMode.LINEAR_MIPMAP_NEAREST:
                    minFilter = TextureMinFilter.LinearMipmapNearest;
                    magFilter = TextureMagFilter.Linear;
                    break;
                case TextureFilterMode.NEAREST_MIPMAP_LINEAR:
                    minFilter = TextureMinFilter.NearestMipmapLinear;
                    magFilter = TextureMagFilter.Nearest;
                    break;
                case TextureFilterMode.LINEAR_MIPMAP_LINEAR:
                    minFilter = TextureMinFilter.LinearMipmapLinear;
                    magFilter = TextureMagFilter.Linear;
                    break;
            }

            return new Tuple<TextureMinFilter, TextureMagFilter>(minFilter, magFilter);
        }

        private DepthFunction GetDepthCompareFunc(Compare compareFunc)
        {
            switch (compareFunc)
            {
                case Compare.Never:
                    return DepthFunction.Never;

                case Compare.Less:
                    return DepthFunction.Less;

                case Compare.Equal:
                    return DepthFunction.Equal;

                case Compare.LessEqual:
                    return DepthFunction.Lequal;

                case Compare.Greater:
                    return DepthFunction.Greater;

                case Compare.NotEqual:
                    return DepthFunction.Notequal;

                case Compare.GreaterEqual:
                    return DepthFunction.Gequal;

                case Compare.Always:
                    return DepthFunction.Always;

                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        private OpenTK.Graphics.OpenGL.TextureWrapMode GetWrapMode(Common.TextureWrapMode wrapMode)
        {
            switch (wrapMode)
            {
                default:
                case Common.TextureWrapMode.REPEAT:
                    return OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat;
                case Common.TextureWrapMode.MIRRORED_REPEAT:
                    return OpenTK.Graphics.OpenGL.TextureWrapMode.MirroredRepeat;
                case Common.TextureWrapMode.CLAMP_TO_EDGE:
                    return OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge;
                case Common.TextureWrapMode.CLAMP_TO_BORDER:
                    return OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToBorder;
            }
        }

        private TexturePixelInfo GetTexturePixelInfo(ITextureBase tex)
        {
            PixelInternalFormat internalFormat;
            PixelFormat format;
            PixelType pxType;

            switch (tex.PixelFormat.ColorFormat)
            {
                case ColorFormat.RGBA:
                    internalFormat = PixelInternalFormat.Rgba;
                    format = PixelFormat.Bgra;
                    pxType = PixelType.UnsignedByte;

                    break;
                case ColorFormat.RGB:
                    internalFormat = PixelInternalFormat.Rgb;
                    format = PixelFormat.Bgr;
                    pxType = PixelType.UnsignedByte;

                    break;
                // TODO: Handle Alpha-only / Intensity-only and AlphaIntensity correctly.
                case ColorFormat.Intensity:
                    internalFormat = PixelInternalFormat.Alpha;
                    format = PixelFormat.Alpha;
                    pxType = PixelType.UnsignedByte;

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
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;
            var glWrapMode = GetWrapMode(img.WrapMode);
            var pxInfo = GetTexturePixelInfo(img);

            GL.TexImage3D(TextureTarget.Texture2DArray, 0, pxInfo.InternalFormat, img.Width, img.Height, img.Layers, 0, pxInfo.Format, pxInfo.PxType, IntPtr.Zero);

            if (img.DoGenerateMipMaps)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureCompareMode, (int)GetTexComapreMode(img.CompareMode));
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureCompareFunc, (int)GetDepthCompareFunc(img.CompareFunc));
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = id };

            return texID;
        }

        /// <summary>
        /// Creates a new CubeMap and binds it to the shader.
        /// </summary>
        /// <param name="img">A given IWritableCubeMap object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableCubeMap img)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;

            var glWrapMode = GetWrapMode(img.WrapMode);
            var pxInfo = GetTexturePixelInfo(img);

            for (int i = 0; i < 6; i++)
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, pxInfo.InternalFormat, img.Width, img.Height, 0, pxInfo.Format, pxInfo.PxType, IntPtr.Zero);

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)glWrapMode);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = id };

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ITexture object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(ITexture img)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;

            var glWrapMode = GetWrapMode(img.WrapMode);

            var pxInfo = GetTexturePixelInfo(img);

            GL.TexImage2D(TextureTarget.Texture2D, 0, pxInfo.InternalFormat, img.Width, img.Height, 0, pxInfo.Format, pxInfo.PxType, img.PixelData);

            if (img.DoGenerateMipMaps)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)glWrapMode);

            ITextureHandle texID = new TextureHandle { TexHandle = id };

            return texID;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given IWritableTexture object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureHandle that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureHandle CreateTexture(IWritableTexture img)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            var glMinMagFilter = GetMinMagFilter(img.FilterMode);
            var minFilter = glMinMagFilter.Item1;
            var magFilter = glMinMagFilter.Item2;
            var glWrapMode = GetWrapMode(img.WrapMode);
            var pxInfo = GetTexturePixelInfo(img);

            GL.TexImage2D(TextureTarget.Texture2D, 0, pxInfo.InternalFormat, img.Width, img.Height, 0, pxInfo.Format, pxInfo.PxType, IntPtr.Zero);

            if (img.DoGenerateMipMaps)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)GetTexComapreMode(img.CompareMode));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)GetDepthCompareFunc(img.CompareFunc));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)glWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)glWrapMode);

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

            GL.BindTexture(TextureTarget.Texture2D, ((TextureHandle)tex).TexHandle);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, startX, startY, width, height, format, PixelType.UnsignedByte, bytes);

            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
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

            if (texHandle.TexHandle != -1)
            {
                GL.DeleteTexture(texHandle.TexHandle);
                _textureCountPerShader--;
            }
        }
        #endregion

        #region Shader related Members

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

            if (gs != null)
                GL.AttachShader(program, geometryObject);

            GL.AttachShader(program, vertexObject);

            // enable GLSL (ES) shaders to use fuVertex, fuColor and fuNormal attributes
            GL.BindAttribLocation(program, AttributeLocations.VertexAttribLocation, UniformNameDeclarations.Vertex);
            GL.BindAttribLocation(program, AttributeLocations.ColorAttribLocation, UniformNameDeclarations.VertexColor);
            GL.BindAttribLocation(program, AttributeLocations.UvAttribLocation, UniformNameDeclarations.TextureCoordinates);
            GL.BindAttribLocation(program, AttributeLocations.NormalAttribLocation, UniformNameDeclarations.Normal);
            GL.BindAttribLocation(program, AttributeLocations.TangentAttribLocation, UniformNameDeclarations.Tangent);
            GL.BindAttribLocation(program, AttributeLocations.BoneIndexAttribLocation, UniformNameDeclarations.BoneIndex);
            GL.BindAttribLocation(program, AttributeLocations.BoneWeightAttribLocation, UniformNameDeclarations.BoneWeight);
            GL.BindAttribLocation(program, AttributeLocations.BitangentAttribLocation, UniformNameDeclarations.Bitangent);

            GL.LinkProgram(program); //Must be called AFTER BindAttribLocation

            // mr: Detach Shader & delete
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
            var program = ((ShaderHandleImp)sp).Handle;

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
        public IShaderParam GetShaderParam(IShaderHandle shaderProgram, string paramName)
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
            GL.GetUniform(((ShaderHandleImp)program).Handle, ((ShaderParam)param).handle, out float f);
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

            GL.GetProgram(sProg.Handle, GetProgramParameterName.ActiveUniforms, out int nParams);

            for (var i = 0; i < nParams; i++)
            {
                var paramInfo = new ShaderParamInfo();
                paramInfo.Name = GL.GetActiveUniform(sProg.Handle, i, out paramInfo.Size, out ActiveUniformType uType);
                paramInfo.Handle = GetShaderParam(sProg, paramInfo.Name);

                //Diagnostics.Log($"Active Uniforms: {paramInfo.Name}");

                switch (uType)
                {
                    case ActiveUniformType.Int:
                        paramInfo.Type = typeof(int);
                        break;

                    case ActiveUniformType.Float:
                        paramInfo.Type = typeof(float);
                        break;

                    case ActiveUniformType.FloatVec2:
                        paramInfo.Type = typeof(float2);
                        break;

                    case ActiveUniformType.FloatVec3:
                        paramInfo.Type = typeof(float3);
                        break;

                    case ActiveUniformType.FloatVec4:
                        paramInfo.Type = typeof(float4);
                        break;

                    case ActiveUniformType.FloatMat4:
                        paramInfo.Type = typeof(float4x4);
                        break;
                    case ActiveUniformType.Sampler2D:
                    case ActiveUniformType.UnsignedIntSampler2D:
                    case ActiveUniformType.IntSampler2D:
                    case ActiveUniformType.Sampler2DShadow:
                        paramInfo.Type = typeof(ITextureBase);
                        break;
                    case ActiveUniformType.SamplerCube:
                    case ActiveUniformType.SamplerCubeShadow:
                        paramInfo.Type = typeof(IWritableCubeMap);
                        break;
                    case ActiveUniformType.Sampler2DArray:
                        paramInfo.Type = typeof(IWritableArrayTexture);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"ActiveUniformType {uType} unknown.");
                }

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
                GL.Uniform2(((ShaderParam)param).handle, val.Length, (float*)pFlt);
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
                GL.Uniform3(((ShaderParam)param).handle, val.Length, (float*)pFlt);
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
                GL.Uniform4(((ShaderParam)param).handle, val.Length, (float*)pFlt);
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
                GL.UniformMatrix4(((ShaderParam)param).handle, val.Length, false, (float*)pMtx);
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

        private void BindTextureByTarget(ITextureHandle texId, TextureType texTarget)
        {
            switch (texTarget)
            {
                case TextureType.TEXTURE1D:
                    GL.BindTexture(TextureTarget.Texture1D, ((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.TEXTURE2D:
                    GL.BindTexture(TextureTarget.Texture2D, ((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.TEXTURE3D:
                    GL.BindTexture(TextureTarget.Texture3D, ((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.TEXTURE_CUBE_MAP:
                    GL.BindTexture(TextureTarget.TextureCubeMap, ((TextureHandle)texId).TexHandle);
                    break;
                case TextureType.ARRAY_TEXTURE:
                    GL.BindTexture(TextureTarget.Texture2DArray, ((TextureHandle)texId).TexHandle);
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
                GL.Uniform1(((ShaderParam)param).handle, texUnitArray.Length, pFlt);
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
                GL.GetFloat(GetPName.ColorClearValue, out Vector4 ret);
                return new float4(ret.X, ret.Y, ret.Z, ret.W);
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

        #endregion

        #region Rendering related Members       

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
            GL.GenBuffers(1, out int handle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertsBytes), attributes, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format(
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
                int handle = ((AttributeImp)attribHandle).AttributeBufferObject;
                if (handle != 0)
                {
                    GL.DeleteBuffer(handle);
                    ((AttributeImp)attribHandle).AttributeBufferObject = 0;
                }
            }
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
                GL.GenBuffers(1, out ((MeshImp)mr).VertexBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertsBytes), vertices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format("Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.", vertsBytes, vboBytes));
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
                GL.GenBuffers(1, out ((MeshImp)mr).TangentBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).TangentBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(tangentBytes), tangents, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != tangentBytes)
                throw new ApplicationException(String.Format("Problem uploading vertex buffer to VBO (tangents). Tried to upload {0} bytes, uploaded {1}.", tangentBytes, vboBytes));
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
                GL.GenBuffers(1, out ((MeshImp)mr).BitangentBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).BitangentBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bitangentBytes), bitangents, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != bitangentBytes)
                throw new ApplicationException(String.Format("Problem uploading vertex buffer to VBO (bitangents). Tried to upload {0} bytes, uploaded {1}.", bitangentBytes, vboBytes));
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
                GL.GenBuffers(1, out ((MeshImp)mr).NormalBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normsBytes), normals, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != normsBytes)
                throw new ApplicationException(String.Format("Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.", normsBytes, vboBytes));
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
                GL.GenBuffers(1, out ((MeshImp)mr).BoneIndexBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).BoneIndexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(indicesBytes), boneIndices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != indicesBytes)
                throw new ApplicationException(String.Format("Problem uploading bone indices buffer to VBO (bone indices). Tried to upload {0} bytes, uploaded {1}.", indicesBytes, vboBytes));
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
                GL.GenBuffers(1, out ((MeshImp)mr).BoneWeightBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).BoneWeightBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(weightsBytes), boneWeights, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != weightsBytes)
                throw new ApplicationException(String.Format("Problem uploading bone weights buffer to VBO (bone weights). Tried to upload {0} bytes, uploaded {1}.", weightsBytes, vboBytes));
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
                GL.GenBuffers(1, out ((MeshImp)mr).UVBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).UVBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uvsBytes), uvs, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != uvsBytes)
                throw new ApplicationException(String.Format("Problem uploading uv buffer to VBO (uvs). Tried to upload {0} bytes, uploaded {1}.", uvsBytes, vboBytes));
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
                GL.GenBuffers(1, out ((MeshImp)mr).ColorBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).ColorBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colsBytes), colors, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != colsBytes)
                throw new ApplicationException(String.Format("Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.", colsBytes, vboBytes));
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
                GL.GenBuffers(1, out ((MeshImp)mr).ElementBufferObject);
            // Upload the index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(trisBytes), triangleIndices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out int vboBytes);
            if (vboBytes != trisBytes)
                throw new ApplicationException(String.Format("Problem uploading vertex buffer to VBO (offsets). Tried to upload {0} bytes, uploaded {1}.", trisBytes, vboBytes));
        }

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mr">The mesh which buffer respectively GPU memory should be deleted.</param>
        public void RemoveVertices(IMeshImp mr)
        {
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
        /// Renders the specified <see cref="IMeshImp" />.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        public void Render(IMeshImp mr)
        {
            if (((MeshImp)mr).VertexBufferObject != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.VertexAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
                GL.VertexAttribPointer(AttributeLocations.VertexAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).ColorBufferObject != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.ColorAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).ColorBufferObject);
                GL.VertexAttribPointer(AttributeLocations.ColorAttribLocation, 4, VertexAttribPointerType.UnsignedByte, true, 0, IntPtr.Zero);
            }

            if (((MeshImp)mr).UVBufferObject != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.UvAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).UVBufferObject);
                GL.VertexAttribPointer(AttributeLocations.UvAttribLocation, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).NormalBufferObject != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.NormalAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
                GL.VertexAttribPointer(AttributeLocations.NormalAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).TangentBufferObject != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.TangentAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).TangentBufferObject);
                GL.VertexAttribPointer(AttributeLocations.TangentAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).BitangentBufferObject != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.BitangentAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).BitangentBufferObject);
                GL.VertexAttribPointer(AttributeLocations.BitangentAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).BoneIndexBufferObject != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.BoneIndexAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).BoneIndexBufferObject);
                GL.VertexAttribPointer(AttributeLocations.BoneIndexAttribLocation, 4, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).BoneWeightBufferObject != 0)
            {
                GL.EnableVertexAttribArray(AttributeLocations.BoneWeightAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).BoneWeightBufferObject);
                GL.VertexAttribPointer(AttributeLocations.BoneWeightAttribLocation, 4, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).ElementBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);

                switch (((MeshImp)mr).MeshType)
                {
                    case OpenGLPrimitiveType.TRIANGLES:
                    default:
                        GL.DrawElements(PrimitiveType.Triangles, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.POINT:
                        // enable gl_PointSize to set the point size
                        if (!_isPtRenderingEnabled)
                        {
                            _isPtRenderingEnabled = true;
                            GL.Enable(EnableCap.ProgramPointSize);
                            GL.Enable(EnableCap.PointSprite);
                            GL.Enable(EnableCap.VertexProgramPointSize);
                        }
                        GL.DrawElements(PrimitiveType.Points, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.LINES:
                        if (!_isLineSmoothEnabled)
                        {
                            GL.Enable(EnableCap.LineSmooth);
                            _isLineSmoothEnabled = true;
                        }
                        GL.DrawElements(PrimitiveType.Lines, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.LINE_LOOP:
                        if (!_isLineSmoothEnabled)
                        {
                            GL.Enable(EnableCap.LineSmooth);
                            _isLineSmoothEnabled = true;
                        }
                        GL.DrawElements(PrimitiveType.LineLoop, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.LINE_STRIP:
                        if (!_isLineSmoothEnabled)
                        {
                            GL.Enable(EnableCap.LineSmooth);
                            _isLineSmoothEnabled = true;
                        }
                        GL.DrawElements(PrimitiveType.LineStrip, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.PATCHES:
                        GL.DrawElements(PrimitiveType.Patches, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.QUAD_STRIP:
                        GL.DrawElements(PrimitiveType.QuadStrip, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.TRIANGLE_FAN:
                        GL.DrawElements(PrimitiveType.TriangleFan, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                    case OpenGLPrimitiveType.TRIANGLE_STRIP:
                        GL.DrawElements(PrimitiveType.TriangleStrip, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                        break;
                }
            }

            if (((MeshImp)mr).VertexBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.VertexAttribLocation);
            if (((MeshImp)mr).ColorBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.ColorAttribLocation);
            if (((MeshImp)mr).NormalBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.NormalAttribLocation);
            if (((MeshImp)mr).UVBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.UvAttribLocation);
            if (((MeshImp)mr).TangentBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.TangentAttribLocation);
            if (((MeshImp)mr).BitangentBufferObject != 0)
                GL.DisableVertexAttribArray(AttributeLocations.TangentAttribLocation);
        }

        /// <summary>
        /// Gets the content of the buffer.
        /// </summary>
        /// <param name="quad">The Rectangle where the content is draw into.</param>
        /// <param name="texId">The tex identifier.</param>
        public void GetBufferContent(Common.Rectangle quad, ITextureHandle texId)
        {
            GL.BindTexture(TextureTarget.Texture2D, ((TextureHandle)texId).TexHandle);
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

        internal static BlendEquationMode BlendOperationToOgl(BlendOperation bo)
        {
            switch (bo)
            {
                case BlendOperation.Add:
                    return BlendEquationMode.FuncAdd;
                case BlendOperation.Subtract:
                    return BlendEquationMode.FuncSubtract;
                case BlendOperation.ReverseSubtract:
                    return BlendEquationMode.FuncReverseSubtract;
                case BlendOperation.Minimum:
                    return BlendEquationMode.Min;
                case BlendOperation.Maximum:
                    return BlendEquationMode.Max;
                default:
                    throw new ArgumentOutOfRangeException("bo");
            }
        }

        internal static BlendOperation BlendOperationFromOgl(BlendEquationMode bom)
        {
            switch (bom)
            {
                case BlendEquationMode.FuncAdd:
                    return BlendOperation.Add;
                case BlendEquationMode.Min:
                    return BlendOperation.Minimum;
                case BlendEquationMode.Max:
                    return BlendOperation.Maximum;
                case BlendEquationMode.FuncSubtract:
                    return BlendOperation.Subtract;
                case BlendEquationMode.FuncReverseSubtract:
                    return BlendOperation.ReverseSubtract;
                default:
                    throw new ArgumentOutOfRangeException("bom");
            }
        }

        internal static int BlendToOgl(Blend blend, bool isForBlendFactorAlpha = false)
        {
            switch (blend)
            {
                case Blend.Zero:
                    return (int)BlendingFactorSrc.Zero;
                case Blend.One:
                    return (int)BlendingFactorSrc.One;
                case Blend.SourceColor:
                    return (int)BlendingFactorDest.SrcColor;
                case Blend.InverseSourceColor:
                    return (int)BlendingFactorDest.OneMinusSrcColor;
                case Blend.SourceAlpha:
                    return (int)BlendingFactorSrc.SrcAlpha;
                case Blend.InverseSourceAlpha:
                    return (int)BlendingFactorSrc.OneMinusSrcAlpha;
                case Blend.DestinationAlpha:
                    return (int)BlendingFactorSrc.DstAlpha;
                case Blend.InverseDestinationAlpha:
                    return (int)BlendingFactorSrc.OneMinusDstAlpha;
                case Blend.DestinationColor:
                    return (int)BlendingFactorSrc.DstColor;
                case Blend.InverseDestinationColor:
                    return (int)BlendingFactorSrc.OneMinusDstColor;
                case Blend.BlendFactor:
                    return (int)((isForBlendFactorAlpha) ? BlendingFactorSrc.ConstantAlpha : BlendingFactorSrc.ConstantColor);
                case Blend.InverseBlendFactor:
                    return (int)((isForBlendFactorAlpha) ? BlendingFactorSrc.OneMinusConstantAlpha : BlendingFactorSrc.OneMinusConstantColor);
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

        internal static Blend BlendFromOgl(int bf)
        {
            switch (bf)
            {
                case (int)BlendingFactorSrc.Zero:
                    return Blend.Zero;
                case (int)BlendingFactorSrc.One:
                    return Blend.One;
                case (int)BlendingFactorDest.SrcColor:
                    return Blend.SourceColor;
                case (int)BlendingFactorDest.OneMinusSrcColor:
                    return Blend.InverseSourceColor;
                case (int)BlendingFactorSrc.SrcAlpha:
                    return Blend.SourceAlpha;
                case (int)BlendingFactorSrc.OneMinusSrcAlpha:
                    return Blend.InverseSourceAlpha;
                case (int)BlendingFactorSrc.DstAlpha:
                    return Blend.DestinationAlpha;
                case (int)BlendingFactorSrc.OneMinusDstAlpha:
                    return Blend.InverseDestinationAlpha;
                case (int)BlendingFactorSrc.DstColor:
                    return Blend.DestinationColor;
                case (int)BlendingFactorSrc.OneMinusDstColor:
                    return Blend.InverseDestinationColor;
                case (int)BlendingFactorSrc.ConstantAlpha:
                case (int)BlendingFactorSrc.ConstantColor:
                    return Blend.BlendFactor;
                case (int)BlendingFactorSrc.OneMinusConstantAlpha:
                case (int)BlendingFactorSrc.OneMinusConstantColor:
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
                        PolygonMode pm;
                        switch ((FillMode)value)
                        {
                            case FillMode.Point:
                                pm = PolygonMode.Point;
                                break;
                            case FillMode.Wireframe:
                                pm = PolygonMode.Line;
                                break;
                            case FillMode.Solid:
                                pm = PolygonMode.Fill;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("value");
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
                    GL.BlendColor(Color.FromArgb((int)value));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("renderState");
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
                        FillMode ret;
                        GL.GetInteger(GetPName.PolygonMode, out int pm);
                        switch ((PolygonMode)pm)
                        {
                            case PolygonMode.Point:
                                ret = FillMode.Point;
                                break;
                            case PolygonMode.Line:
                                ret = FillMode.Wireframe;
                                break;
                            case PolygonMode.Fill:
                                ret = FillMode.Solid;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("pm", "Value " + ((PolygonMode)pm) + " not handled");
                        }
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
                        Compare ret;
                        switch ((DepthFunction)depFunc)
                        {
                            case DepthFunction.Never:
                                ret = Compare.Never;
                                break;
                            case DepthFunction.Less:
                                ret = Compare.Less;
                                break;
                            case DepthFunction.Equal:
                                ret = Compare.Equal;
                                break;
                            case DepthFunction.Lequal:
                                ret = Compare.LessEqual;
                                break;
                            case DepthFunction.Greater:
                                ret = Compare.Greater;
                                break;
                            case DepthFunction.Notequal:
                                ret = Compare.NotEqual;
                                break;
                            case DepthFunction.Gequal:
                                ret = Compare.GreaterEqual;
                                break;
                            case DepthFunction.Always:
                                ret = Compare.Always;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("depFunc", "Value " + ((DepthFunction)depFunc) + " not handled");
                        }
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
            if (((TextureHandle)texHandle).FrameBufferHandle == -1)
            {
                var fBuffer = GL.GenFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fBuffer);

                GL.BindTexture(TextureTarget.Texture2D, ((TextureHandle)texHandle).TexHandle);

                if (tex.TextureType != RenderTargetTextureTypes.G_DEPTH)
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

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
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
            if (((TextureHandle)texHandle).FrameBufferHandle == -1)
            {
                var fBuffer = GL.GenFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fBuffer);

                GL.BindTexture(TextureTarget.TextureCubeMap, ((TextureHandle)texHandle).TexHandle);

                if (tex.TextureType != RenderTargetTextureTypes.G_DEPTH)
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

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
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
            if (((TextureHandle)texHandle).FrameBufferHandle == -1)
            {
                var fBuffer = GL.GenFramebuffer();
                ((TextureHandle)texHandle).FrameBufferHandle = fBuffer;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fBuffer);

                GL.BindTexture(TextureTarget.Texture2DArray, ((TextureHandle)texHandle).TexHandle);

                if (tex.TextureType != RenderTargetTextureTypes.G_DEPTH)
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
                GL.BindTexture(TextureTarget.Texture2DArray, ((TextureHandle)texHandle).TexHandle);
                GL.FramebufferTextureLayer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, ((TextureHandle)texHandle).TexHandle, 0, layer);
            }

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
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

            if (renderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_DEPTH] == null && !renderTarget.IsDepthOnly)
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
                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);
                }
            }

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");
            }

            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
        }

        private int CreateDepthRenderBuffer(int width, int height)
        {
            GL.Enable(EnableCap.DepthTest);

            GL.GenRenderbuffers(1, out int gDepthRenderbufferHandle);
            //((FrameBufferHandle)renderTarget.DepthBufferHandle).Handle = gDepthRenderbufferHandle;
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, gDepthRenderbufferHandle);
            return gDepthRenderbufferHandle;
        }

        private int CreateFrameBuffer(IRenderTarget renderTarget, ITextureHandle[] texHandles)
        {
            var gBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);

            int depthCnt = 0;

            var depthTexPos = (int)RenderTargetTextureTypes.G_DEPTH;

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
                        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment + (depthCnt), TextureTarget.Texture2D, ((TextureHandle)texHandle).TexHandle, 0);
                        depthCnt++;
                    }
                    else
                        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + (i - depthCnt), TextureTarget.Texture2D, ((TextureHandle)texHandle).TexHandle, 0);
                }
                GL.DrawBuffers(attachments.Count, attachments.ToArray());
            }
            else //If a frame-buffer only has a depth texture we don't need draw buffers
            {
                var texHandle = texHandles[depthTexPos];

                if (texHandle != null)
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, ((TextureHandle)texHandle).TexHandle, 0);
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
            ChangeFramebufferTexture2D(renderTarget, attachment, ((TextureHandle)texHandle).TexHandle, isDepthTex);
        }

        private void ChangeFramebufferTexture2D(IRenderTarget renderTarget, int attachment, int handle, bool isDepth)
        {
            var boundFbo = GL.GetInteger(GetPName.FramebufferBinding);
            var rtFbo = ((FrameBufferHandle)renderTarget.GBufferHandle).Handle;

            var isCurrentFbo = true;

            if (boundFbo != rtFbo)
            {
                isCurrentFbo = false;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, rtFbo);
            }

            if (!isDepth)
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + attachment, TextureTarget.Texture2D, handle, 0);
            else
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, handle, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception($"Error creating RenderTarget: {GL.GetError()}, {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");

            if (!isCurrentFbo)
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, boundFbo);
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
            switch (capability)
            {
                case HardwareCapability.CAN_RENDER_DEFFERED:
                    return !GL.GetString(StringName.Extensions).Contains("EXT_framebuffer_object") ? 0U : 1U;
                case HardwareCapability.CAN_USE_GEOMETRY_SHADERS:
                    return 1U;
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

            GL.Begin(PrimitiveType.Lines);
            GL.Color4(color.x, color.y, color.z, color.w);
            GL.Vertex3(start.x, start.y, start.z);
            GL.Color4(color.x, color.y, color.z, color.w);
            GL.Vertex3(end.x, end.y, end.z);
            GL.End();
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
            GL.ReadPixels(x, y, w, h, PixelFormat.Bgr /* yuk, yuk ??? */, PixelType.UnsignedByte, image.PixelData);
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