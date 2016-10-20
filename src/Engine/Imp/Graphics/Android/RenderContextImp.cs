using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Fusee.Base.Common;
using Fusee.Base.Core;
using OpenTK;
using Fusee.Math.Core;
using Fusee.Engine.Common;
//using OpenTK.Graphics.ES11;
using OpenTK.Graphics.ES30;
using Path = Fusee.Base.Common.Path;

namespace Fusee.Engine.Imp.Graphics.Android
{
    /// <summary>
    /// Implementation of the <see cref="IRenderContextImp" /> interface for usage with OpenTK framework.
    /// </summary>
    public class RenderContextImp : IRenderContextImp
    {
        #region Fields
        private int _currentAll;
        private readonly Dictionary<int, int> _shaderParam2TexUnit;
        private Context _androidContext;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContextImp"/> class.
        /// </summary>
        /// <param name="renderCanvas">The render canvas interface.</param>
        public RenderContextImp(IRenderCanvasImp renderCanvas, Context androidContext)
        {
            _currentAll = 0;
            _shaderParam2TexUnit = new Dictionary<int, int>();

            _androidContext = androidContext;

            // Due to the right-handed nature of OpenGL and the left-handed design of FUSEE
            // the meaning of what's Front and Back of a face simply flips.
            // TODO - implement this in render states!!!

            GL.CullFace(All.Back);
        }

        #endregion

        #region Image data related Members

        /// <summary>
        /// Updates a texture with images obtained from a Video.
        /// </summary>
        /// <param name="stream">The Video from which the images are taken.</param>
        /// <param name="tex">The texture to which the ImageData is bound to.</param>
        /// <remarks>Look at the VideoTextureExample for further information.</remarks>
        public void UpdateTextureFromVideoStream(IVideoStreamImp stream, ITexture tex)
        {
            throw new NotImplementedException("TODO: VIdeo textures on android.");
            /*
            ImageData img = stream.GetCurrentFrame();
            OpenTK.Graphics.OpenGL.PixelFormat format;
            switch (img.PixelFormat)
            {
                case ImagePixelFormat.RGBA:
                    format = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
                    break;
                case ImagePixelFormat.RGB:
                    format = OpenTK.Graphics.OpenGL.PixelFormat.Bgr;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (img.PixelData != null)
            {
                if (tex == null)
                    tex = CreateTexture(img, false);

                GL.BindTexture(All.Texture2D, ((Texture) tex).handle);
                GL.TexSubImage2D(All.Texture2D, 0, 0, 0, img.Width, img.Height,
                    format, PixelType.UnsignedByte, img.PixelData);
            }
            */
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
        public void UpdateTextureRegion(ITexture tex, ImageData img, int startX, int startY, int width, int height)
        {
            All format;
            switch (img.PixelFormat)
            {
                case ImagePixelFormat.RGBA:
                    format = All.Rgba;
                    break;
                case ImagePixelFormat.RGB:
                    format = All.Rgb;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GL.BindTexture(All.Texture2D, ((Texture) tex).handle);
            GL.TexSubImage2D(All.Texture2D, 0, startX, startY, width, height,
                format, All.UnsignedByte, img.PixelData);
        }

        /// <summary>
        /// Creates a new Image with a specified size and color.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="col">The color of the image. Value must be JS compatible.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing.</returns>
        public ImageData CreateImage(int width, int height, ColorUint col)
        {
            int colorVal = col.ToRgba();
            int nPixels = width * height;
            int nBytes = nPixels*4;
            int[] pxls = new int[nPixels];

            for (int i = 0; i < pxls.Length; i++)
                pxls[i] = colorVal;

            var ret = new ImageData
            {
                PixelData = new byte[nBytes],
                Height = height,
                Width = width,
                PixelFormat = ImagePixelFormat.RGBA,
                Stride = width
            };

            Buffer.BlockCopy(pxls, 0, ret.PixelData, 0, nBytes);
            return ret;
        }

        /// <summary>
        /// Maps a specified text with on an image.
        /// </summary>
        /// <param name="imgData">The ImageData struct with the PixelData from the image.</param>
        /// <param name="fontName">The name of the text-font.</param>
        /// <param name="fontSize">The size of the text-font.</param>
        /// <param name="text">The text that sould be mapped on the iamge.</param>
        /// <param name="textColor">The color of the text-font.</param>
        /// <param name="startPosX">The horizontal start-position of the text on the image.</param>
        /// <param name="startPosY">The vertical start-position of the text on the image.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing</returns>
        public ImageData TextOnImage(ImageData imgData, String fontName, float fontSize, String text, String textColor,
            float startPosX, float startPosY)
        {
            return imgData;
        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <param name="repeat">Indicating if the texture should be clamped or repeated.</param>
        /// <returns>An ITexture that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITexture CreateTexture(ImageData img, bool repeat)
        {
            int internalFormat;
            All format;
            switch (img.PixelFormat)
            {
                case ImagePixelFormat.RGBA:
                    internalFormat = (int) All.Rgba;
                    format = All.Rgba;
                    break;
                case ImagePixelFormat.RGB:
                    internalFormat = (int) All.Rgb;
                    format = All.Rgb;
                    break;
                // TODO: Handle Alpha-only / Intensity-only and AlphaIntensity correctly.
                case ImagePixelFormat.Intensity:
                    internalFormat = (int) All.Alpha;
                    format = All.Alpha;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("CreateTexture: Image pixel format not supported");
            }

            int id;
            GL.GenTextures(1, out id);
            GL.BindTexture(All.Texture2D, id);
            GL.TexImage2D(All.Texture2D, 0, internalFormat, img.Width, img.Height, 0,
                format, All.UnsignedByte, img.PixelData);

            GL.TexParameter(All.Texture2D, All.TextureMinFilter, (int) All.Linear);
            GL.TexParameter(All.Texture2D, All.TextureMagFilter, (int) All.Linear);

            GL.TexParameter(All.Texture2D, All.TextureWrapS, (repeat) ? (int)All.Repeat : (int)All.ClampToEdge);
            GL.TexParameter(All.Texture2D, All.TextureWrapT, (repeat) ? (int)All.Repeat : (int)All.ClampToEdge);

            ITexture texID = new Texture {handle = id};

            return texID;
        }

        #endregion

        #region Text related Members
        /*
        /// <summary>
        /// Loads a font file (*.ttf) and processes it with the given font size.
        /// </summary>
        /// <param name="stream">The stream where to read the font from.</param>
        /// <param name="size">The size.</param>
        /// <returns>An <see cref="IFont"/> containing all necessary information for further processing.</returns>
        public IFont LoadFont(Stream stream, uint size)
        {
            byte[] fileArray;
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                fileArray = ms.ToArray();
            }

            var texAtlas = new Font
            {
                Face = _sharpFont.NewMemoryFace(fileArray, 0),
                FontSize = size,
                UseKerning = false
            };

            texAtlas.Face.SetPixelSizes(0, size);
            return GenerateTextureAtlas(texAtlas);
        }

        private IFont GenerateTextureAtlas(IFont font)
        {
            if (font == null)
                return null;

            var texAtlas = ((Font) font);
            var face = texAtlas.Face;

            // get atlas texture size
            var rowW = 0;
            var rowH = 0;
            var h = 0;

            const int maxWidth = 512;

            for (uint i = 32; i < 256; i++)
            {
                face.LoadChar(i, LoadFlags.Default, LoadTarget.Normal);
                
                if (rowW + ((int) face.Glyph.Advance.X) + 1 >= maxWidth)
                {
                    h += rowH;
                    rowW = 0;
                    rowH = 0;
                }

                rowW += ((int) face.Glyph.Advance.X) + 1;
                rowH = System.Math.Max((int) face.Glyph.Metrics.Height, rowH);
            }

            // for resizing to non-power-of-two
            var potH = (h + rowH) - 1;

            potH |= potH >> 1;
            potH |= potH >> 2;
            potH |= potH >> 4;
            potH |= potH >> 8;
            potH |= potH >> 16;

            texAtlas.Width = maxWidth;
            texAtlas.Height = ++potH;

            // atlas texture
            int tex;
            GL.GenTextures(1, out tex);

            GL.ActiveTexture(All.Texture0);
            GL.BindTexture(All.Texture2D, tex);

            GL.TexImage2D(All.Texture2D, 0, (int)All.Alpha, maxWidth, potH, 0,
                All.Alpha, All.UnsignedByte, IntPtr.Zero);

            // texture settings
            GL.PixelStore(All.UnpackAlignment, 1);

            GL.TexParameter(All.Texture2D, All.TextureWrapS, (int) All.ClampToEdge);
            GL.TexParameter(All.Texture2D, All.TextureWrapT,
                (int)All.ClampToEdge);

            GL.TexParameter(All.Texture2D, All.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(All.Texture2D, All.TextureMagFilter, (int)All.Linear);

            texAtlas.TexAtlas = new Texture {handle = tex};

            // paste the glyph images into the texture atlas
            texAtlas.CharInfo = new CharInfoStruct[256];

            var offX = 0;
            var offY = 0;
            rowH = 0;

            for (uint i = 32; i < 256; i++)
            {
                face.LoadChar(i, LoadFlags.Default, LoadTarget.Normal);
                face.Glyph.RenderGlyph(RenderMode.Normal);

                if (offX + face.Glyph.Bitmap.Width + 1 >= maxWidth)
                {
                    offY += rowH;
                    rowH = 0;
                    offX = 0;
                }

                GL.TexSubImage2D(All.Texture2D, 0, offX, offY, face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows,
                    All.Alpha, All.UnsignedByte, face.Glyph.Bitmap.Buffer);

                // char informations
                texAtlas.CharInfo[i].AdvanceX = (int) face.Glyph.Advance.X;
                texAtlas.CharInfo[i].AdvanceY = (int) face.Glyph.Advance.Y;

                texAtlas.CharInfo[i].BitmapW = face.Glyph.Bitmap.Width;
                texAtlas.CharInfo[i].BitmapH = face.Glyph.Bitmap.Rows;

                texAtlas.CharInfo[i].BitmapL = face.Glyph.BitmapLeft;
                texAtlas.CharInfo[i].BitmapT = face.Glyph.BitmapTop;

                texAtlas.CharInfo[i].TexOffX = offX/(float) maxWidth;
                texAtlas.CharInfo[i].TexOffY = offY/(float) potH;

                rowH = System.Math.Max(rowH, face.Glyph.Bitmap.Rows);
                offX += face.Glyph.Bitmap.Width + 1;
            }

            return texAtlas;
        }

        /// <summary>
        /// Fixes the kerning of a text (if possible).
        /// </summary>
        /// <param name="font">The <see cref="IFont"/> containing information about the font.</param>
        /// <param name="vertices">The vertices.</param>
        /// <param name="text">The text.</param>
        /// <param name="scaleX">The scale x (OpenGL scaling factor).</param>
        /// <returns>The fixed vertices as an array of <see cref="float3"/>.</returns>
        public float3[] FixTextKerning(IFont font, float3[] vertices, string text, float scaleX)
        {
            var texAtlas = ((Font) font);

            if (!texAtlas.UseKerning || !texAtlas.Face.HasKerning)
                return vertices;

            // use kerning -> fix values
            var fixX = 0f;
            var fixVert = 4;

            for (var c = 0; c < text.Length - 1; c++)
            {
                var leftChar = texAtlas.Face.GetCharIndex(text[c]);
                var rightChar = texAtlas.Face.GetCharIndex(text[c + 1]);

                fixX += ((int) texAtlas.Face.GetKerning(leftChar, rightChar, KerningMode.Default).X)*scaleX;

                vertices[fixVert++].x += fixX;
                vertices[fixVert++].x += fixX;
                vertices[fixVert++].x += fixX;
                vertices[fixVert++].x += fixX;
            }

            return vertices;
        }
        */
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
            StringBuilder sbParamName =  new StringBuilder(paramName);
            int h = GL.GetUniformLocation(((ShaderProgramImp) shaderProgram).Program, sbParamName);
            return (h == -1) ? null : new ShaderParam {handle = h};
        }

        /// <summary>
        /// Gets the float parameter value inside a shaderprogram by using a <see cref="IShaderParam" /> as search reference.
        /// Do not use this function in frequent updates as it transfers information from graphics card to the cpu which takes time.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>A float number (default is 0).</returns>
        public float GetParamValue(IShaderProgramImp program, IShaderParam param)
        {
            float f;
            GL.GetUniform(((ShaderProgramImp) program).Program, ((ShaderParam) param).handle, out f);
            return f;
        }

        /// <summary>
        /// Gets the shader parameter list of a specific <see cref="IShaderProgramImp" />. 
        /// </summary>
        /// <param name="shaderProgram">The shader program.</param>
        /// <returns>All Shader parameters of a shaderprogram are returned.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public IList<ShaderParamInfo> GetShaderParamList(IShaderProgramImp shaderProgram)
        {
            var sProg = (ShaderProgramImp) shaderProgram;
            var paramList = new List<ShaderParamInfo>();

            int nParams;
            GL.GetProgram(sProg.Program, All.ActiveUniforms, out nParams);
         

            for (var i = 0; i < nParams; i++)
            {
                All uType;

                var paramInfo = new ShaderParamInfo();
                StringBuilder sbName = new StringBuilder(512);
                int lenWritten;
                GL.GetActiveUniform(sProg.Program, i, 511, out lenWritten, out paramInfo.Size, out uType, sbName);
                paramInfo.Name = sbName.ToString();
                paramInfo.Handle = GetShaderParam(sProg, paramInfo.Name);

                switch (uType)
                {
                    case All.Int:
                        paramInfo.Type = typeof (int);
                        break;

                    case All.Float:
                        paramInfo.Type = typeof (float);
                        break;

                    case All.FloatVec2:
                        paramInfo.Type = typeof (float2);
                        break;

                    case All.FloatVec3:
                        paramInfo.Type = typeof (float3);
                        break;

                    case All.FloatVec4:
                        paramInfo.Type = typeof (float4);
                        break;

                    case All.FloatMat4:
                        paramInfo.Type = typeof (float4x4);
                        break;

                    case All.Sampler2D:
                        paramInfo.Type = typeof (ITexture);
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
            GL.Uniform1(((ShaderParam) param).handle, val);
        }

        /// <summary>
        /// Sets a <see cref="float2" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float2 val)
        {
            GL.Uniform2(((ShaderParam) param).handle, val.x, val.y);
        }

        /// <summary>
        /// Sets a <see cref="float3" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float3 val)
        {
            GL.Uniform3(((ShaderParam) param).handle, val.x, val.y, val.z);
        }

        /// <summary>
        /// Sets a <see cref="float4" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float4 val)
        {
            GL.Uniform4(((ShaderParam) param).handle, val.x, val.y, val.z, val.w);
        }

        // TODO add vector implementations

        /// <summary>
        /// Sets a <see cref="float4x4" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
            unsafe
            {
                var mF = (float*) (&val);
                // Row order notation
                // GL.UniformMatrix4(((ShaderParam) param).handle, 1, false, mF);

                // Column order notation
                GL.UniformMatrix4(((ShaderParam) param).handle, 1, true, mF);
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
                GL.Uniform4(((ShaderParam) param).handle, val.Length, (float*) pFlt);
        }

        /// <summary>
        /// Sets a <see cref="float4x4" /> array shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public unsafe void SetShaderParam(IShaderParam param, float4x4[] val)
        {
            var tmpArray = new float4[val.Length*4];

            for (var i = 0; i < val.Length; i++)
            {
                tmpArray[i*4] = val[i].Column0;
                tmpArray[i*4 + 1] = val[i].Column1;
                tmpArray[i*4 + 2] = val[i].Column2;
                tmpArray[i*4 + 3] = val[i].Column3;
            }

            fixed (float4* pMtx = &tmpArray[0])
                GL.UniformMatrix4(((ShaderParam) param).handle, val.Length, false, (float*) pMtx);
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

        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texId">An ITexture probably returned from CreateTexture method</param>
        public void SetShaderParamTexture(IShaderParam param, ITexture texId)
        {
            int iParam = ((ShaderParam)param).handle;
            int texUnit;
            if (!_shaderParam2TexUnit.TryGetValue(iParam, out texUnit))
            {
                texUnit = _currentAll++;
                _shaderParam2TexUnit[iParam] = texUnit;
            }
            GL.Uniform1(iParam, texUnit);
            GL.ActiveTexture(All.Texture0 + texUnit);
            GL.BindTexture(All.Texture2D, ((Texture)texId).handle);
        }
        #endregion

        #region Clear Fields

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the clear.
        /// </value>
        public float4 ClearColor
        {
            get
            {
                float[] ret = new float[4];
                GL.GetFloat(All.ColorClearValue, ret);
                return new float4(ret[0], ret[1], ret[2], ret[3]);
            }
            set { GL.ClearColor(value.x, value.y, value.z, value.w); }
        }

        /// <summary>
        /// Gets or sets the clear depth value which is used to clear the depth buffer.
        /// </summary>
        /// <value>
        /// Specifies the depth value used when the depth buffer is cleared. The initial value is 1. This value is clamped to the range [0,1].
        /// </value>
        public float ClearDepth
        {
            get
            {
                float ret;
                GL.GetFloat(All.DepthClearValue, out ret);
                return ret;
            }
            set { GL.ClearDepth(value); }
        }

        #endregion

        #region Rendering related Members
        /// <summary>
        /// Creates the shaderprogram by using a valid GLSL vertex and fragment shader code. This code is compiled at runtime.
        /// Do not use this function in frequent updates.
        /// </summary>
        /// <param name="vs">The vertex shader code.</param>
        /// <param name="ps">The pixel(=fragment) shader code.</param>
        /// <returns>An instance of <see cref="IShaderProgramImp" />.</returns>
        /// <exception cref="System.ApplicationException">
        /// </exception>
        public IShaderProgramImp CreateShader(string vs, string ps)
        {
            int statusCode;
            StringBuilder info = new StringBuilder(512);
            int length;

            int vertexObject = GL.CreateShader(All.VertexShader);
            int fragmentObject = GL.CreateShader(All.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(vertexObject, 1, new [] {vs}, new [] {vs.Length});
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, 512, out length, info);
            GL.GetShader(vertexObject, All.CompileStatus, out statusCode);

            if (statusCode != 1)
            {
                var errMsg = info.ToString();
                throw new ApplicationException(info.ToString());
            }

            // Compile pixel shader
            GL.ShaderSource(fragmentObject, 1, new [] {ps}, new [] {ps.Length});
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(vertexObject, 512, out length, info);
            GL.GetShader(fragmentObject, All.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info.ToString());

            int program = GL.CreateProgram();
            GL.AttachShader(program, fragmentObject);
            GL.AttachShader(program, vertexObject);

            // enable GLSL (ES) shaders to use fuVertex, fuColor and fuNormal attributes
            GL.BindAttribLocation(program, Helper.VertexAttribLocation, Helper.VertexAttribName);
            GL.BindAttribLocation(program, Helper.ColorAttribLocation, Helper.ColorAttribName);
            GL.BindAttribLocation(program, Helper.UvAttribLocation, Helper.UvAttribName);
            GL.BindAttribLocation(program, Helper.NormalAttribLocation, Helper.NormalAttribName);
            GL.BindAttribLocation(program, Helper.TangentAttribLocation, Helper.TangentAttribName);
            GL.BindAttribLocation(program, Helper.BoneIndexAttribLocation, Helper.BoneIndexAttribName);
            GL.BindAttribLocation(program, Helper.BoneWeightAttribLocation, Helper.BoneWeightAttribName);
            GL.BindAttribLocation(program, Helper.BitangentAttribLocation, Helper.BitangentAttribName);

            GL.LinkProgram(program); // AAAARRRRRGGGGHHHH!!!! Must be called AFTER BindAttribLocation
            return new ShaderProgramImp {Program = program};
        }

        /// <summary>
        /// Sets the shaderprogram onto the GL Rendercontext.
        /// </summary>
        /// <param name="program">The shaderprogram.</param>
        public void SetShader(IShaderProgramImp program)
        {
            _currentAll = 0;
            _shaderParam2TexUnit.Clear();

            GL.UseProgram(((ShaderProgramImp) program).Program);
        }

    
        /// <summary>
        /// Clears the specified flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
        public void Clear(ClearFlags flags)
        {
            GL.Clear((ClearBufferMask) flags);
        }


        /// <summary>
        /// Binds the vertices onto the GL Rendercontext and assigns an VertexBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="vertices">The vertices.</param>
        /// <exception cref="System.ArgumentException">Vertices must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetVertices(IMeshImp mr, float3[] vertices)
        {
            if (vertices == null || vertices.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            int vboBytes;
            int vertsBytes = vertices.Length*3*sizeof (float);
            if (((MeshImp) mr).VertexBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp) mr).VertexBufferObject);

            GL.BindBuffer(All.ArrayBuffer, ((MeshImp) mr).VertexBufferObject);
            GL.BufferData(All.ArrayBuffer, (IntPtr) (vertsBytes), vertices, All.StaticDraw);
            GL.GetBufferParameter(All.ArrayBuffer, All.BufferSize, out vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);
        }


        
        /// <summary>
        /// Binds the normals onto the GL Rendercontext and assigns an NormalBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="normals">The normals.</param>
        /// <exception cref="System.ArgumentException">Normals must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetNormals(IMeshImp mr, float3[] normals)
        {
            if (normals == null || normals.Length == 0)
            {
                throw new ArgumentException("Normals must not be null or empty");
            }

            int vboBytes;
            int normsBytes = normals.Length*3*sizeof (float);
            if (((MeshImp) mr).NormalBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp) mr).NormalBufferObject);

            GL.BindBuffer(All.ArrayBuffer, ((MeshImp) mr).NormalBufferObject);
            GL.BufferData(All.ArrayBuffer, (IntPtr) (normsBytes), normals, All.StaticDraw);
            GL.GetBufferParameter(All.ArrayBuffer, All.BufferSize, out vboBytes);
            if (vboBytes != normsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.",
                    normsBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);
        }

        /// <summary>
        /// Binds the boneindices onto the GL Rendercontext and assigns an BondeIndexBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneIndices">The boneindices.</param>
        /// <exception cref="System.ArgumentException">BoneIndices must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetBoneIndices(IMeshImp mr, float4[] boneIndices)
        {
            if (boneIndices == null || boneIndices.Length == 0)
            {
                throw new ArgumentException("BoneIndices must not be null or empty");
            }

            int vboBytes;
            int indicesBytes = boneIndices.Length * 4 * sizeof(float);
            if (((MeshImp)mr).BoneIndexBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).BoneIndexBufferObject);

            GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).BoneIndexBufferObject);
            GL.BufferData(All.ArrayBuffer, (IntPtr)(indicesBytes), boneIndices, All.StaticDraw);
            GL.GetBufferParameter(All.ArrayBuffer, All.BufferSize, out vboBytes);
            if (vboBytes != indicesBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading boneindices buffer to VBO (boneindices). Tried to upload {0} bytes, uploaded {1}.",
                    indicesBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);
        }

        /// <summary>
        /// Binds the boneweights onto the GL Rendercontext and assigns an BondeWeightBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneWeights">The boneweights.</param>
        /// <exception cref="System.ArgumentException">BoneWeights must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetBoneWeights(IMeshImp mr, float4[] boneWeights)
        {
            if (boneWeights == null || boneWeights.Length == 0)
            {
                throw new ArgumentException("BoneWeights must not be null or empty");
            }

            int vboBytes;
            int weightsBytes = boneWeights.Length * 4 * sizeof(float);
            if (((MeshImp)mr).BoneWeightBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).BoneWeightBufferObject);

            GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).BoneWeightBufferObject);
            GL.BufferData(All.ArrayBuffer, (IntPtr)(weightsBytes), boneWeights, All.StaticDraw);
            GL.GetBufferParameter(All.ArrayBuffer, All.BufferSize, out vboBytes);
            if (vboBytes != weightsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading boneweights buffer to VBO (boneweights). Tried to upload {0} bytes, uploaded {1}.",
                    weightsBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);
        }

        /// <summary>
        /// Binds the UV coordinates onto the GL Rendercontext and assigns an UVBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="uvs">The UV's.</param>
        /// <exception cref="System.ArgumentException">UVs must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetUVs(IMeshImp mr, float2[] uvs)
        {
            if (uvs == null || uvs.Length == 0)
            {
                throw new ArgumentException("UVs must not be null or empty");
            }

            int vboBytes;
            int uvsBytes = uvs.Length*2*sizeof (float);
            if (((MeshImp) mr).UVBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp) mr).UVBufferObject);

            GL.BindBuffer(All.ArrayBuffer, ((MeshImp) mr).UVBufferObject);
            GL.BufferData(All.ArrayBuffer, (IntPtr) (uvsBytes), uvs, All.StaticDraw);
            GL.GetBufferParameter(All.ArrayBuffer, All.BufferSize, out vboBytes);
            if (vboBytes != uvsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading uv buffer to VBO (uvs). Tried to upload {0} bytes, uploaded {1}.",
                    uvsBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);
        }

        /* Not using tangent space normal maps at the moment
        public void SetVertexData(IMeshImp mr, float3[] vertices, float2[] uvs, float3[] normals)
        {
            if (vertices == null || vertices.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            int vboBytes;
            int vertsBytes = vertices.Length * 3 * sizeof(float);
            if (((MeshImp)mr).VertexBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).VertexBufferObject);

            GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
            GL.BufferData(All.ArrayBuffer, (IntPtr)(vertsBytes), vertices, All.StaticDraw);
            GL.GetBufferParameter(All.ArrayBuffer, All.BufferSize, out vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);


            // normals
            if (normals == null || normals.Length == 0)
            {
                throw new ArgumentException("Normals must not be null or empty");
            }

            int normsBytes = normals.Length * 3 * sizeof(float);
            if (((MeshImp)mr).NormalBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).NormalBufferObject);

            GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
            GL.BufferData(All.ArrayBuffer, (IntPtr)(normsBytes), normals, All.StaticDraw);
            GL.GetBufferParameter(All.ArrayBuffer, All.BufferSize, out vboBytes);
            if (vboBytes != normsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.",
                    normsBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);

            
            // UVs
            if (uvs == null || uvs.Length == 0)
            {
                throw new ArgumentException("UVs must not be null or empty");
            }

            int uvsBytes = uvs.Length * 2 * sizeof(float);
            if (((MeshImp)mr).UVBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).UVBufferObject);

            GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).UVBufferObject);
            GL.BufferData(All.ArrayBuffer, (IntPtr)(uvsBytes), uvs, All.StaticDraw);
            GL.GetBufferParameter(All.ArrayBuffer, All.BufferSize, out vboBytes);
            if (vboBytes != uvsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading uv buffer to VBO (uvs). Tried to upload {0} bytes, uploaded {1}.",
                    uvsBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);

            // Generate Tangents and Bitangents

        }
        */

        /// <summary>
        /// Binds the colors onto the GL Rendercontext and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="System.ArgumentException">colors must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetColors(IMeshImp mr, uint[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("colors must not be null or empty");
            }

            int vboBytes;
            int colsBytes = colors.Length*sizeof (uint);
            if (((MeshImp) mr).ColorBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp) mr).ColorBufferObject);

            GL.BindBuffer(All.ArrayBuffer, ((MeshImp) mr).ColorBufferObject);
            GL.BufferData(All.ArrayBuffer, (IntPtr) (colsBytes), colors, All.StaticDraw);
            GL.GetBufferParameter(All.ArrayBuffer, All.BufferSize, out vboBytes);
            if (vboBytes != colsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.",
                    colsBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);
        }

        /// <summary>
        /// Binds the triangles onto the GL Rendercontext and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="triangleIndices">The triangle indices.</param>
        /// <exception cref="System.ArgumentException">triangleIndices must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetTriangles(IMeshImp mr, ushort[] triangleIndices)
        {
            if (triangleIndices == null || triangleIndices.Length == 0)
            {
                throw new ArgumentException("triangleIndices must not be null or empty");
            }
            ((MeshImp) mr).NElements = triangleIndices.Length;
            int vboBytes;
            int trisBytes = triangleIndices.Length*sizeof (short);

            if (((MeshImp) mr).ElementBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp) mr).ElementBufferObject);
            // Upload the index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
            GL.BindBuffer(All.ElementArrayBuffer, ((MeshImp) mr).ElementBufferObject);
            GL.BufferData(All.ElementArrayBuffer, (IntPtr) (trisBytes), triangleIndices,
                All.StaticDraw);
            GL.GetBufferParameter(All.ElementArrayBuffer, All.BufferSize, out vboBytes);
            if (vboBytes != trisBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (offsets). Tried to upload {0} bytes, uploaded {1}.",
                    trisBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);
        }

        /// <summary>
        /// Renders the specified <see cref="IMeshImp" />.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        public void RenderDeferred(IMeshImp mr)
        {
            if (((MeshImp)mr).VertexBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.VertexAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
                GL.VertexAttribPointer(Helper.VertexAttribLocation, 3, All.Float, false, 0,
                    IntPtr.Zero);
            }
            if (((MeshImp)mr).ColorBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.ColorAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).ColorBufferObject);
                GL.VertexAttribPointer(Helper.ColorAttribLocation, 4, All.UnsignedByte, true, 0,
                    IntPtr.Zero);
            }

            if (((MeshImp)mr).UVBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.UvAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).UVBufferObject);
                GL.VertexAttribPointer(Helper.UvAttribLocation, 2, All.Float, false, 0, IntPtr.Zero);
            }

            if (((MeshImp)mr).NormalBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.NormalAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
                GL.VertexAttribPointer(Helper.NormalAttribLocation, 3, All.Float, false, 0,
                    IntPtr.Zero);
            }
            if (((MeshImp)mr).BoneIndexBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.BoneIndexAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).BoneIndexBufferObject);
                GL.VertexAttribPointer(Helper.BoneIndexAttribLocation, 4, All.Float, false, 0,
                    IntPtr.Zero);
            }
            if (((MeshImp)mr).BoneWeightBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.BoneWeightAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).BoneWeightBufferObject);
                GL.VertexAttribPointer(Helper.BoneWeightAttribLocation, 4, All.Float, false, 0,
                    IntPtr.Zero);
            }
            if (((MeshImp)mr).ElementBufferObject != 0)
            {
                GL.BindBuffer(All.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);
                GL.DrawElements(All.Triangles, ((MeshImp)mr).NElements, All.UnsignedShort,
                    IntPtr.Zero);
                //GL.DrawArrays(GL.Enums.All.POINTS, 0, shape.Vertices.Length);
            }
            if (((MeshImp)mr).VertexBufferObject != 0)
            {
                GL.BindBuffer(All.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.VertexAttribLocation);
            }
            if (((MeshImp)mr).ColorBufferObject != 0)
            {
                GL.BindBuffer(All.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.ColorAttribLocation);
            }
            if (((MeshImp)mr).NormalBufferObject != 0)
            {
                GL.BindBuffer(All.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.NormalAttribLocation);
            }
            if (((MeshImp)mr).UVBufferObject != 0)
            {
                GL.BindBuffer(All.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.UvAttribLocation);
            }
        }

        /// <summary>
        /// Renders the specified <see cref="IMeshImp" />.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        public void Render(IMeshImp mr)
        {
            if (((MeshImp) mr).VertexBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.VertexAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp) mr).VertexBufferObject);
                GL.VertexAttribPointer(Helper.VertexAttribLocation, 3, All.Float, false, 0,
                    IntPtr.Zero);
            }
            if (((MeshImp) mr).ColorBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.ColorAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp) mr).ColorBufferObject);
                GL.VertexAttribPointer(Helper.ColorAttribLocation, 4, All.UnsignedByte, true, 0,
                    IntPtr.Zero);
            }

            if (((MeshImp) mr).UVBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.UvAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp) mr).UVBufferObject);
                GL.VertexAttribPointer(Helper.UvAttribLocation, 2, All.Float, false, 0, IntPtr.Zero);
            }

            if (((MeshImp) mr).NormalBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.NormalAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp) mr).NormalBufferObject);
                GL.VertexAttribPointer(Helper.NormalAttribLocation, 3, All.Float, false, 0,
                    IntPtr.Zero);
            }
            if (((MeshImp)mr).BoneIndexBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.BoneIndexAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).BoneIndexBufferObject);
                GL.VertexAttribPointer(Helper.BoneIndexAttribLocation, 4, All.Float, false, 0,
                    IntPtr.Zero);
            }
            if (((MeshImp)mr).BoneWeightBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.BoneWeightAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).BoneWeightBufferObject);
                GL.VertexAttribPointer(Helper.BoneWeightAttribLocation, 4, All.Float, false, 0,
                    IntPtr.Zero);
            }
            if (((MeshImp) mr).ElementBufferObject != 0)
            {
                GL.BindBuffer(All.ElementArrayBuffer, ((MeshImp) mr).ElementBufferObject);
                GL.DrawElements(All.Triangles, ((MeshImp) mr).NElements, All.UnsignedShort,
                    IntPtr.Zero);
                //GL.DrawArrays(GL.Enums.All.POINTS, 0, shape.Vertices.Length);
            }
            if (((MeshImp) mr).VertexBufferObject != 0)
            {
                GL.BindBuffer(All.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.VertexAttribLocation);
            }
            if (((MeshImp) mr).ColorBufferObject != 0)
            {
                GL.BindBuffer(All.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.ColorAttribLocation);
            }
            if (((MeshImp) mr).NormalBufferObject != 0)
            {
                GL.BindBuffer(All.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.NormalAttribLocation);
            }
            if (((MeshImp) mr).UVBufferObject != 0)
            {
                GL.BindBuffer(All.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.UvAttribLocation);
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
            throw new NotImplementedException("TODO: Implement RenderContextImp.DebugLine on Android");
            /*
            GL.Begin(All.Lines);
            GL.Vertex3(start.x, start.y, start.z);
            GL.Vertex3(end.x, end.y, end.z);
            GL.End();
            */
        }

        /// <summary>
        /// Gets the content of the buffer.
        /// </summary>
        /// <param name="quad">The Rectangle where the content is draw into.</param>
        /// <param name="texId">The tex identifier.</param>
        public void GetBufferContent(Common.Rectangle quad, ITexture texId)
        {
            GL.BindTexture(All.Texture2D, ((Texture) texId).handle);
            GL.CopyTexImage2D(All.Texture2D, 0, All.Rgba, quad.Left, quad.Top, quad.Width,
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

        internal static All BlendOperationToOgl(BlendOperation bo)
        {
            switch (bo)
            {
                case BlendOperation.Add:
                    return All.FuncAdd;
                case BlendOperation.Subtract:
                    return All.FuncSubtract;
                case BlendOperation.ReverseSubtract:
                    return All.FuncReverseSubtract;
                case BlendOperation.Minimum:
                    return All.Min;
                case BlendOperation.Maximum:
                    return All.Max;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bo));
            }
        }

        internal static BlendOperation BlendOperationFromOgl(All bom)
        {
            switch (bom)
            {
                case All.FuncAdd:
                    return BlendOperation.Add;
                case All.Min:
                    return BlendOperation.Minimum;
                case All.Max:
                    return BlendOperation.Maximum;
                case All.FuncSubtract:
                    return BlendOperation.Subtract;
                case All.FuncReverseSubtract:
                    return BlendOperation.ReverseSubtract;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bom));
            }
        }

        internal static int BlendToOgl(Blend blend, bool isForAlpha = false)
        {
            switch (blend)
            {
                case Blend.Zero:
                    return (int)All.Zero;
                case Blend.One:
                    return (int)All.One;
                case Blend.SourceColor:
                    return (int)All.SrcColor;
                case Blend.InverseSourceColor:
                    return (int)All.OneMinusSrcColor;
                case Blend.SourceAlpha:
                    return (int)All.SrcAlpha;
                case Blend.InverseSourceAlpha:
                    return (int)All.OneMinusSrcAlpha;
                case Blend.DestinationAlpha:
                    return (int)All.DstAlpha;
                case Blend.InverseDestinationAlpha:
                    return (int)All.OneMinusDstAlpha;
                case Blend.DestinationColor:
                    return (int)All.DstColor;
                case Blend.InverseDestinationColor:
                    return (int)All.OneMinusDstColor;
                case Blend.BlendFactor:
                    return (int)((isForAlpha) ? All.ConstantAlpha : All.ConstantColor);
                case Blend.InverseBlendFactor:
                    return (int)((isForAlpha) ? All.OneMinusConstantAlpha : All.OneMinusConstantColor);
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
                    throw new ArgumentOutOfRangeException(nameof(blend));
            }
        }

        internal static Blend BlendFromOgl(int bf)
        {
            switch (bf)
            {
                case (int)All.Zero:
                    return Blend.Zero;
                case (int)All.One:
                    return Blend.One;
                case (int)All.SrcColor:
                    return Blend.SourceColor;
                case (int)All.OneMinusSrcColor:
                    return Blend.InverseSourceColor;
                case (int)All.SrcAlpha:
                    return Blend.SourceAlpha;
                case (int)All.OneMinusSrcAlpha:
                    return Blend.InverseSourceAlpha;
                case (int)All.DstAlpha:
                    return Blend.DestinationAlpha;
                case (int)All.OneMinusDstAlpha:
                    return Blend.InverseDestinationAlpha;
                case (int)All.DstColor:
                    return Blend.DestinationColor;
                case (int)All.OneMinusDstColor:
                    return Blend.InverseDestinationColor;
                case (int)All.ConstantAlpha:
                case (int)All.ConstantColor:
                    return Blend.BlendFactor;
                case (int)All.OneMinusConstantAlpha:
                case (int)All.OneMinusConstantColor:
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
            switch (renderState)
            {
                case RenderState.FillMode:
                    {
                        switch ((FillMode)value)
                        {
                            case FillMode.Point:
                            case FillMode.Wireframe:
                                Diagnostics.Log("SetRenderState(RenderState.FillMode): Trying to set unsopported FillMode (PolygonMode) on Android. Not supported by OpenGL ES 3.0.");
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
                                GL.Disable(All.CullFace);
                                GL.FrontFace(All.Ccw);
                                break;
                            case Cull.Clockwise:
                                GL.Enable(All.CullFace);
                                GL.FrontFace(All.Cw);
                                break;
                            case Cull.Counterclockwise:
                                GL.Enable(All.CullFace);
                                GL.FrontFace(All.Ccw);
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
                        All df;
                        switch ((Compare)value)
                        {
                            case Compare.Never:
                                df = All.Never;
                                break;
                            case Compare.Less:
                                df = All.Less;
                                break;
                            case Compare.Equal:
                                df = All.Equal;
                                break;
                            case Compare.LessEqual:
                                df = All.Lequal;
                                break;
                            case Compare.Greater:
                                df = All.Greater;
                                break;
                            case Compare.NotEqual:
                                df = All.Notequal;
                                break;
                            case Compare.GreaterEqual:
                                df = All.Gequal;
                                break;
                            case Compare.Always:
                                df = All.Always;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(value));
                        }
                        GL.DepthFunc(df);
                    }
                    break;
                case RenderState.ZEnable:
                    if (value == 0)
                        GL.Disable(All.DepthTest);
                    else
                        GL.Enable(All.DepthTest);
                    break;
                case RenderState.AlphaBlendEnable:
                    if (value == 0)
                        GL.Disable(All.Blend);
                    else
                        GL.Enable(All.Blend);
                    break;
                case RenderState.BlendOperation:
                    int alphaMode;
                    GL.GetInteger(All.BlendEquationAlpha, out alphaMode);
                    GL.BlendEquationSeparate(BlendOperationToOgl((BlendOperation)value), (All)alphaMode);
                    break;
                case RenderState.BlendOperationAlpha:
                    int rgbMode;
                    GL.GetInteger(All.BlendEquation, out rgbMode);
                    GL.BlendEquationSeparate((All)rgbMode, BlendOperationToOgl((BlendOperation)value));
                    break;
                case RenderState.SourceBlend:
                    {
                        int rgbDst, alphaSrc, alphaDst;
                        GL.GetInteger(All.BlendDstRgb, out rgbDst);
                        GL.GetInteger(All.BlendSrcAlpha, out alphaSrc);
                        GL.GetInteger(All.BlendDstAlpha, out alphaDst);
                        GL.BlendFuncSeparate((All)BlendToOgl((Blend)value),
                                             (All)rgbDst,
                                             (All)alphaSrc,
                                             (All)alphaDst);
                    }
                    break;
                case RenderState.DestinationBlend:
                    {
                        int rgbSrc, alphaSrc, alphaDst;
                        GL.GetInteger(All.BlendSrcRgb, out rgbSrc);
                        GL.GetInteger(All.BlendSrcAlpha, out alphaSrc);
                        GL.GetInteger(All.BlendDstAlpha, out alphaDst);
                        GL.BlendFuncSeparate((All)rgbSrc,
                                             (All)BlendToOgl((Blend)value),
                                             (All)alphaSrc,
                                             (All)alphaDst);
                    }
                    break;
                case RenderState.SourceBlendAlpha:
                    {
                        int rgbSrc, rgbDst, alphaDst;
                        GL.GetInteger(All.BlendSrcRgb, out rgbSrc);
                        GL.GetInteger(All.BlendDstRgb, out rgbDst);
                        GL.GetInteger(All.BlendDstAlpha, out alphaDst);
                        GL.BlendFuncSeparate((All)rgbSrc,
                                             (All)rgbDst,
                                             (All)BlendToOgl((Blend)value, true),
                                             (All)alphaDst);
                    }
                    break;
                case RenderState.DestinationBlendAlpha:
                    {
                        int rgbSrc, rgbDst, alphaSrc;
                        GL.GetInteger(All.BlendSrcRgb, out rgbSrc);
                        GL.GetInteger(All.BlendDstRgb, out rgbDst);
                        GL.GetInteger(All.BlendSrcAlpha, out alphaSrc);
                        GL.BlendFuncSeparate((All)rgbSrc,
                                             (All)rgbDst,
                                             (All)alphaSrc,
                                             (All)BlendToOgl((Blend)value, true));
                    }
                    break;
                case RenderState.BlendFactor:
                    float4 col = ColorUint.FromRgba(value).Tofloat4();
                    GL.BlendColor(col.r, col.g, col.b, col.a);
                    break;
                /* TODO: Implement texture wrapping rahter as a texture property than a "global" render state. This is most
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
                        Diagnostics.Log("GetRenderState(RenderState.FillMode): FillMode (PolygonMode) on Android is not supported by OpenGL ES 3.0. Returning FillMode.Solid.");
                        return (uint)FillMode.Solid;
                    }
                case RenderState.CullMode:
                    {
                        int cullFace;
                        GL.GetInteger(All.CullFace, out cullFace);
                        if (cullFace == 0)
                            return (uint)Cull.None;
                        int frontFace;
                        GL.GetInteger(All.FrontFace, out frontFace);
                        if (frontFace == (int)All.Cw)
                            return (uint)Cull.Clockwise;
                        return (uint)Cull.Counterclockwise;
                    }
                case RenderState.Clipping:
                    // clipping is always on in OpenGL - This state is simply ignored
                    return 1; // == true
                case RenderState.ZFunc:
                    {
                        int depFunc;
                        GL.GetInteger(All.DepthFunc, out depFunc);
                        Compare ret;
                        switch ((All)depFunc)
                        {
                            case All.Never:
                                ret = Compare.Never;
                                break;
                            case All.Less:
                                ret = Compare.Less;
                                break;
                            case All.Equal:
                                ret = Compare.Equal;
                                break;
                            case All.Lequal:
                                ret = Compare.LessEqual;
                                break;
                            case All.Greater:
                                ret = Compare.Greater;
                                break;
                            case All.Notequal:
                                ret = Compare.NotEqual;
                                break;
                            case All.Gequal:
                                ret = Compare.GreaterEqual;
                                break;
                            case All.Always:
                                ret = Compare.Always;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("depFunc", "Value " + ((All)depFunc) + " not handled");
                        }
                        return (uint)ret;
                    }
                case RenderState.ZEnable:
                    {
                        int depTest;
                        GL.GetInteger(All.DepthTest, out depTest);
                        return (uint)(depTest);
                    }
                case RenderState.AlphaBlendEnable:
                    {
                        int blendEnable;
                        GL.GetInteger(All.Blend, out blendEnable);
                        return (uint)(blendEnable);
                    }
                case RenderState.BlendOperation:
                    {
                        int rgbMode;
                        GL.GetInteger(All.BlendEquation, out rgbMode);
                        return (uint)BlendOperationFromOgl((All)rgbMode);
                    }
                case RenderState.BlendOperationAlpha:
                    {
                        int alphaMode;
                        GL.GetInteger(All.BlendEquationAlpha, out alphaMode);
                        return (uint)BlendOperationFromOgl((All)alphaMode);
                    }
                case RenderState.SourceBlend:
                    {
                        int rgbSrc;
                        GL.GetInteger(All.BlendSrcRgb, out rgbSrc);
                        return (uint)BlendFromOgl(rgbSrc);
                    }
                case RenderState.DestinationBlend:
                    {
                        int rgbDst;
                        GL.GetInteger(All.BlendSrcRgb, out rgbDst);
                        return (uint)BlendFromOgl(rgbDst);
                    }
                case RenderState.SourceBlendAlpha:
                    {
                        int alphaSrc;
                        GL.GetInteger(All.BlendSrcAlpha, out alphaSrc);
                        return (uint)BlendFromOgl(alphaSrc);
                    }
                case RenderState.DestinationBlendAlpha:
                    {
                        int alphaDst;
                        GL.GetInteger(All.BlendDstAlpha, out alphaDst);
                        return (uint)BlendFromOgl(alphaDst);
                    }
                case RenderState.BlendFactor:
                    int col;
                    GL.GetInteger(All.BlendColor, out col);
                    return (uint)col;
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderState));
            }
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
            float outVar;
            switch (capability)
            {
                case HardwareCapability.DEFFERED_POSSIBLE:
                    return !GL.GetString(All.Extensions).Contains("EXT_framebuffer_object") ? 0U : 1U;
                case HardwareCapability.BUFFERSIZE:
                    GL.GetFloat(All.BufferSize, out outVar);
                    return BitConverter.ToUInt32(BitConverter.GetBytes(outVar), 0);
                default:
                    throw new ArgumentOutOfRangeException(nameof(capability), capability, null);
            }
        }


        public bool CreateFBO()
        {
            return true;
        }

        #endregion

        #region Picking related Members

        /// <summary>
        /// Retrieves a sub-image of the giben region.
        /// </summary>
        /// <param name="x">The x value of the start of the region.</param>
        /// <param name="y">The y value of the start of the region.</param>
        /// <param name="w">The width to copy.</param>
        /// <param name="h">The height to copy.</param>
        /// <returns>The specified sub-image</returns>
        public ImageData GetPixelColor(int x, int y, int w = 1, int h = 1)
        {
            ImageData image = CreateImage(w, h, ColorUint.Black);
            GL.ReadPixels(x, y, w, h, All.Rgb, All.UnsignedByte, image.PixelData);
            return image;
            
            /*
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, w, h);
            Bitmap bmp = new Bitmap(w, h);
            BitmapData data = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            GL.ReadPixels(x, y, w, h, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);

            bmp.UnlockBits(data);
            return bmp;
            */
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
            GL.ReadPixels(x, y, 1, 1, All.DepthComponent, All.UnsignedByte, ref depth);

            return depth;
        }

        #endregion
    }
}