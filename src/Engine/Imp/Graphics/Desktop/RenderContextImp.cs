using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using System.Runtime.InteropServices;
using Fusee.Base.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Fusee.Math.Core;
using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IRenderContextImp" /> interface for usage with OpenTK framework.
    /// </summary>
    public class RenderContextImp : IRenderContextImp
    {
        #region Fields
        private int _currentTextureUnit;
        private readonly Dictionary<int, int> _shaderParam2TexUnit;
        // private readonly Library _sharpFont;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContextImp"/> class.
        /// </summary>
        /// <param name="renderCanvas">The render canvas interface.</param>
        public RenderContextImp(IRenderCanvasImp renderCanvas)
        {
            _currentTextureUnit = 0;
            _shaderParam2TexUnit = new Dictionary<int, int>();

            // Due to the right-handed nature of OpenGL and the left-handed design of FUSEE
            // the meaning of what's Front and Back of a face simply flips.
            // TODO - implement this in render states!!!

            GL.CullFace(CullFaceMode.Back);

            // _sharpFont = new Library();
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

                GL.BindTexture(TextureTarget.Texture2D, ((Texture) tex).handle);
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, img.Width, img.Height,
                    format, PixelType.UnsignedByte, img.PixelData);
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
        public void UpdateTextureRegion(ITexture tex, ImageData img, int startX, int startY, int width, int height)
        {
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

            GL.BindTexture(TextureTarget.Texture2D, ((Texture) tex).handle);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, startX, startY, width, height,
                format, PixelType.UnsignedByte, img.PixelData);
        }


        /* OBSOLETE
        /// <summary>
        /// Creates a new Image with a specified size and color.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="color">The color of the image. Value must be JS compatible.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing.</returns>
        public ImageData CreateImage(int width, int height, ColorUint color)
        {
            var bmp = new Bitmap(width, height);
            System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(bmp);
            Color col = Color.FromArgb(color.A, color.R, color.G, color.B);
            gfx.Clear(col);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int strideAbs = (bmpData.Stride < 0) ? -bmpData.Stride : bmpData.Stride;
            int bytes = (strideAbs)*bmp.Height;

            var ret = new ImageData
            {
                PixelData = new byte[bytes],
                Height = bmpData.Height,
                Width = bmpData.Width,
                PixelFormat = ImagePixelFormat.RGBA,
                Stride = bmpData.Stride
            };

            Marshal.Copy(bmpData.Scan0, ret.PixelData, 0, bytes);

            bmp.UnlockBits(bmpData);
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
            var imgDataNew = imgData;

            GCHandle arrayHandle = GCHandle.Alloc(imgDataNew.PixelData,
                GCHandleType.Pinned);
            var bmp = new Bitmap(imgDataNew.Width, imgDataNew.Height, imgDataNew.Stride, PixelFormat.Format32bppArgb,
                arrayHandle.AddrOfPinnedObject());

            // Flip before writing text on bmp
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            Color color = Color.FromName(textColor);
            var font = new System.Drawing.Font(fontName, fontSize, FontStyle.Regular, GraphicsUnit.World);

            System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(bmp);
            gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
            gfx.DrawString(text, font, new SolidBrush(color), startPosX, startPosY);

            // Flip after writing text on bmp
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int strideAbs = (bmpData.Stride < 0) ? -bmpData.Stride : bmpData.Stride;
            int bytes = (strideAbs)*bmp.Height;

            imgDataNew.PixelData = new byte[bytes];
            imgDataNew.Height = bmpData.Height;
            imgDataNew.Width = bmpData.Width;
            imgDataNew.Stride = bmpData.Stride;

            Marshal.Copy(bmpData.Scan0, imgDataNew.PixelData, 0, bytes);

            bmp.UnlockBits(bmpData);
            return imgDataNew;
        }
        */

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <param name="repeat">Indicating if the texture should be clamped or repeated.</param>
        /// <returns>An ITexture that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITexture CreateTexture(ImageData img, bool repeat)
        {
            PixelInternalFormat internalFormat;
            OpenTK.Graphics.OpenGL.PixelFormat format;
            switch (img.PixelFormat)
            {
                case ImagePixelFormat.RGBA:
                    internalFormat = PixelInternalFormat.Rgba;
                    format = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
                    break;
                case ImagePixelFormat.RGB:
                    internalFormat = PixelInternalFormat.Rgb;
                    format = OpenTK.Graphics.OpenGL.PixelFormat.Bgr;
                    break;
                // TODO: Handle Alpha-only / Intensity-only and AlphaIntensity correctly.
                case ImagePixelFormat.Intensity:
                    internalFormat = PixelInternalFormat.Alpha;
                    format = OpenTK.Graphics.OpenGL.PixelFormat.Alpha;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("CreateTexture: Image pixel format not supported");
            }

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, img.Width, img.Height, 0,
                format, PixelType.UnsignedByte, img.PixelData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMinFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (repeat) ? (int) TextureWrapMode.Repeat : (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (repeat) ? (int) TextureWrapMode.Repeat : (int) TextureWrapMode.ClampToEdge);

            ITexture texID = new Texture {handle = id};

            return texID;
        }

        #endregion

        #region Text related Members
        /*
        public AssetHandler FontAssetHandler
        {
            get
            {
                return new AssetHandler
                {
                    Decoder = delegate(string id, object storage)
                    {
                        string ext = Base.Common.Path.GetExtension(id).ToLower();
                        switch (ext)
                        {
                            case "ttf":
                            case "otf":
                                return LoadFont((Stream) storage);
                        }
                        return null;
                    },
                    Checker = delegate(string id)
                    {
                        string ext = Base.Common.Path.GetExtension(id).ToLower();
                        switch (ext)
                        {
                            case "ttf":
                            case "otf":
                                return true;
                        }
                        return false;
                    }
                };
            }
        }


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
        private static IFont GenerateTextureAtlas(IFont font)
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
            var tex = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Alpha, maxWidth, potH, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Alpha, PixelType.UnsignedByte, IntPtr.Zero);

            // texture settings
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int) TextureWrapMode.ClampToEdge);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);

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

                GL.TexSubImage2D(TextureTarget.Texture2D, 0, offX, offY, face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows,
                    OpenTK.Graphics.OpenGL.PixelFormat.Alpha, PixelType.UnsignedByte, face.Glyph.Bitmap.Buffer);

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
            int h = GL.GetUniformLocation(((ShaderProgramImp) shaderProgram).Program, paramName);
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
            GL.GetProgram(sProg.Program, GetProgramParameterName.ActiveUniforms, out nParams);

            for (var i = 0; i < nParams; i++)
            {
                ActiveUniformType uType;

                var paramInfo = new ShaderParamInfo();
                paramInfo.Name = GL.GetActiveUniform(sProg.Program, i, out paramInfo.Size, out uType);
                paramInfo.Handle = GetShaderParam(sProg, paramInfo.Name);

                switch (uType)
                {
                    case ActiveUniformType.Int:
                        paramInfo.Type = typeof (int);
                        break;

                    case ActiveUniformType.Float:
                        paramInfo.Type = typeof (float);
                        break;

                    case ActiveUniformType.FloatVec2:
                        paramInfo.Type = typeof (float2);
                        break;

                    case ActiveUniformType.FloatVec3:
                        paramInfo.Type = typeof (float3);
                        break;

                    case ActiveUniformType.FloatVec4:
                        paramInfo.Type = typeof (float4);
                        break;

                    case ActiveUniformType.FloatMat4:
                        paramInfo.Type = typeof (float4x4);
                        break;

                    case ActiveUniformType.Sampler2D:
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
                texUnit = _currentTextureUnit++;
                _shaderParam2TexUnit[iParam] = texUnit;
            }
            GL.Uniform1(iParam, texUnit);
            GL.ActiveTexture(TextureUnit.Texture0 + texUnit);
            GL.BindTexture(TextureTarget.Texture2D, ((Texture)texId).handle);
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
                Vector4 ret;
                GL.GetFloat(GetPName.ColorClearValue, out ret);
                return new float4(ret.X, ret.Y, ret.Z, ret.W);
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
                GL.GetFloat(GetPName.DepthClearValue, out ret);
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
            string info;

            int vertexObject = GL.CreateShader(ShaderType.VertexShader);
            int fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(vertexObject, vs);
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, out info);
            GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out statusCode);

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
            _currentTextureUnit = 0;
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

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp) mr).VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (vertsBytes), vertices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp) mr).NormalBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (normsBytes), normals, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != normsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.",
                    normsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).BoneIndexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(indicesBytes), boneIndices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != indicesBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading boneindices buffer to VBO (boneindices). Tried to upload {0} bytes, uploaded {1}.",
                    indicesBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).BoneWeightBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(weightsBytes), boneWeights, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != weightsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading boneweights buffer to VBO (boneweights). Tried to upload {0} bytes, uploaded {1}.",
                    weightsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp) mr).UVBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (uvsBytes), uvs, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != uvsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading uv buffer to VBO (uvs). Tried to upload {0} bytes, uploaded {1}.",
                    uvsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertsBytes), vertices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


            // normals
            if (normals == null || normals.Length == 0)
            {
                throw new ArgumentException("Normals must not be null or empty");
            }

            int normsBytes = normals.Length * 3 * sizeof(float);
            if (((MeshImp)mr).NormalBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).NormalBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normsBytes), normals, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != normsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.",
                    normsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            
            // UVs
            if (uvs == null || uvs.Length == 0)
            {
                throw new ArgumentException("UVs must not be null or empty");
            }

            int uvsBytes = uvs.Length * 2 * sizeof(float);
            if (((MeshImp)mr).UVBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).UVBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).UVBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uvsBytes), uvs, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != uvsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading uv buffer to VBO (uvs). Tried to upload {0} bytes, uploaded {1}.",
                    uvsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

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

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp) mr).ColorBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (colsBytes), colors, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != colsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.",
                    colsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((MeshImp) mr).ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr) (trisBytes), triangleIndices,
                BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != trisBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (offsets). Tried to upload {0} bytes, uploaded {1}.",
                    trisBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp) mr).VertexBufferObject);
                GL.VertexAttribPointer(Helper.VertexAttribLocation, 3, VertexAttribPointerType.Float, false, 0,
                    IntPtr.Zero);
            }
            if (((MeshImp) mr).ColorBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.ColorAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp) mr).ColorBufferObject);
                GL.VertexAttribPointer(Helper.ColorAttribLocation, 4, VertexAttribPointerType.UnsignedByte, true, 0,
                    IntPtr.Zero);
            }

            if (((MeshImp) mr).UVBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.UvAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp) mr).UVBufferObject);
                GL.VertexAttribPointer(Helper.UvAttribLocation, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }

            if (((MeshImp) mr).NormalBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.NormalAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp) mr).NormalBufferObject);
                GL.VertexAttribPointer(Helper.NormalAttribLocation, 3, VertexAttribPointerType.Float, false, 0,
                    IntPtr.Zero);
            }
            if (((MeshImp)mr).BoneIndexBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.BoneIndexAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).BoneIndexBufferObject);
                GL.VertexAttribPointer(Helper.BoneIndexAttribLocation, 4, VertexAttribPointerType.Float, false, 0,
                    IntPtr.Zero);
            }
            if (((MeshImp)mr).BoneWeightBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.BoneWeightAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).BoneWeightBufferObject);
                GL.VertexAttribPointer(Helper.BoneWeightAttribLocation, 4, VertexAttribPointerType.Float, false, 0,
                    IntPtr.Zero);
            }
            if (((MeshImp) mr).ElementBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((MeshImp) mr).ElementBufferObject);
                GL.DrawElements(BeginMode.Triangles, ((MeshImp) mr).NElements, DrawElementsType.UnsignedShort,
                    IntPtr.Zero);
                //GL.DrawArrays(GL.Enums.BeginMode.POINTS, 0, shape.Vertices.Length);
            }
            if (((MeshImp) mr).VertexBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.VertexAttribLocation);
            }
            if (((MeshImp) mr).ColorBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.ColorAttribLocation);
            }
            if (((MeshImp) mr).NormalBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.NormalAttribLocation);
            }
            if (((MeshImp) mr).UVBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(start.x, start.y, start.z);
            GL.Vertex3(end.x, end.y, end.z);
            GL.End();
        }

        /// <summary>
        /// Gets the content of the buffer.
        /// </summary>
        /// <param name="quad">The Rectangle where the content is draw into.</param>
        /// <param name="texId">The tex identifier.</param>
        public void GetBufferContent(Common.Rectangle quad, ITexture texId)
        {
            GL.BindTexture(TextureTarget.Texture2D, ((Texture) texId).handle);
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

        internal static int BlendToOgl(Blend blend, bool isForAlpha = false)
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
                    return (int)((isForAlpha) ? BlendingFactorSrc.ConstantAlpha : BlendingFactorSrc.ConstantColor);
                case Blend.InverseBlendFactor:
                    return (int)((isForAlpha) ? BlendingFactorSrc.OneMinusConstantAlpha : BlendingFactorSrc.OneMinusConstantColor);
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
                                GL.Disable(EnableCap.CullFace);
                                GL.FrontFace(FrontFaceDirection.Ccw);
                                break;
                            case Cull.Clockwise:
                                GL.Enable(EnableCap.CullFace);
                                GL.FrontFace(FrontFaceDirection.Cw);
                                break;
                            case Cull.Counterclockwise:
                                GL.Enable(EnableCap.CullFace);
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
                        DepthFunction df;
                        switch ((Compare)value)
                        {
                            case Compare.Never:
                                df = DepthFunction.Never;
                                break;
                            case Compare.Less:
                                df = DepthFunction.Less;
                                break;
                            case Compare.Equal:
                                df = DepthFunction.Equal;
                                break;
                            case Compare.LessEqual:
                                df = DepthFunction.Lequal;
                                break;
                            case Compare.Greater:
                                df = DepthFunction.Greater;
                                break;
                            case Compare.NotEqual:
                                df = DepthFunction.Notequal;
                                break;
                            case Compare.GreaterEqual:
                                df = DepthFunction.Gequal;
                                break;
                            case Compare.Always:
                                df = DepthFunction.Always;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("value");
                        }
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
                    int alphaMode;
                    GL.GetInteger(GetPName.BlendEquationAlpha, out alphaMode);
                    GL.BlendEquationSeparate(BlendOperationToOgl((BlendOperation)value), (BlendEquationMode)alphaMode);
                    break;
                case RenderState.BlendOperationAlpha:
                    int rgbMode;
                    GL.GetInteger(GetPName.BlendEquationRgb, out rgbMode);
                    GL.BlendEquationSeparate((BlendEquationMode)rgbMode, BlendOperationToOgl((BlendOperation)value));
                    break;
                case RenderState.SourceBlend:
                    {
                        int rgbDst, alphaSrc, alphaDst;
                        GL.GetInteger(GetPName.BlendDstRgb, out rgbDst);
                        GL.GetInteger(GetPName.BlendSrcAlpha, out alphaSrc);
                        GL.GetInteger(GetPName.BlendDstAlpha, out alphaDst);
                        GL.BlendFuncSeparate((BlendingFactorSrc)BlendToOgl((Blend)value),
                                             (BlendingFactorDest)rgbDst,
                                             (BlendingFactorSrc)alphaSrc,
                                             (BlendingFactorDest)alphaDst);
                    }
                    break;
                case RenderState.DestinationBlend:
                    {
                        int rgbSrc, alphaSrc, alphaDst;
                        GL.GetInteger(GetPName.BlendSrcRgb, out rgbSrc);
                        GL.GetInteger(GetPName.BlendSrcAlpha, out alphaSrc);
                        GL.GetInteger(GetPName.BlendDstAlpha, out alphaDst);
                        GL.BlendFuncSeparate((BlendingFactorSrc)rgbSrc,
                                             (BlendingFactorDest)BlendToOgl((Blend)value),
                                             (BlendingFactorSrc)alphaSrc,
                                             (BlendingFactorDest)alphaDst);
                    }
                    break;
                case RenderState.SourceBlendAlpha:
                    {
                        int rgbSrc, rgbDst, alphaDst;
                        GL.GetInteger(GetPName.BlendSrcRgb, out rgbSrc);
                        GL.GetInteger(GetPName.BlendDstRgb, out rgbDst);
                        GL.GetInteger(GetPName.BlendDstAlpha, out alphaDst);
                        GL.BlendFuncSeparate((BlendingFactorSrc)rgbSrc,
                                             (BlendingFactorDest)rgbDst,
                                             (BlendingFactorSrc)BlendToOgl((Blend)value, true),
                                             (BlendingFactorDest)alphaDst);
                    }
                    break;
                case RenderState.DestinationBlendAlpha:
                    {
                        int rgbSrc, rgbDst, alphaSrc;
                        GL.GetInteger(GetPName.BlendSrcRgb, out rgbSrc);
                        GL.GetInteger(GetPName.BlendDstRgb, out rgbDst);
                        GL.GetInteger(GetPName.BlendSrcAlpha, out alphaSrc);
                        GL.BlendFuncSeparate((BlendingFactorSrc)rgbSrc,
                                             (BlendingFactorDest)rgbDst,
                                             (BlendingFactorSrc)alphaSrc,
                                             (BlendingFactorDest)BlendToOgl((Blend)value, true));
                    }
                    break;
                case RenderState.BlendFactor:
                    GL.BlendColor(Color.FromArgb((int)value));
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
                    throw new ArgumentOutOfRangeException("renderState");
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
                        int pm;
                        FillMode ret;
                        GL.GetInteger(GetPName.PolygonMode, out pm);
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
                        int cullFace;
                        GL.GetInteger(GetPName.CullFace, out cullFace);
                        if (cullFace == 0)
                            return (uint)Cull.None;
                        int frontFace;
                        GL.GetInteger(GetPName.FrontFace, out frontFace);
                        if (frontFace == (int)FrontFaceDirection.Cw)
                            return (uint)Cull.Clockwise;
                        return (uint)Cull.Counterclockwise;
                    }
                case RenderState.Clipping:
                    // clipping is always on in OpenGL - This state is simply ignored
                    return 1; // == true
                case RenderState.ZFunc:
                    {
                        int depFunc;
                        GL.GetInteger(GetPName.DepthFunc, out depFunc);
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
                        int depTest;
                        GL.GetInteger(GetPName.DepthTest, out depTest);
                        return (uint)(depTest);
                    }
                case RenderState.ZWriteEnable:
                    {
                        int depWriteMask;
                        GL.GetInteger(GetPName.DepthWritemask, out depWriteMask);
                        return (uint)(depWriteMask);
                    }
                case RenderState.AlphaBlendEnable:
                    {
                        int blendEnable;
                        GL.GetInteger(GetPName.Blend, out blendEnable);
                        return (uint)(blendEnable);
                    }
                case RenderState.BlendOperation:
                    {
                        int rgbMode;
                        GL.GetInteger(GetPName.BlendEquationRgb, out rgbMode);
                        return (uint)BlendOperationFromOgl((BlendEquationMode)rgbMode);
                    }
                case RenderState.BlendOperationAlpha:
                    {
                        int alphaMode;
                        GL.GetInteger(GetPName.BlendEquationAlpha, out alphaMode);
                        return (uint)BlendOperationFromOgl((BlendEquationMode)alphaMode);
                    }
                case RenderState.SourceBlend:
                    {
                        int rgbSrc;
                        GL.GetInteger(GetPName.BlendSrcRgb, out rgbSrc);
                        return (uint)BlendFromOgl(rgbSrc);
                    }
                case RenderState.DestinationBlend:
                    {
                        int rgbDst;
                        GL.GetInteger(GetPName.BlendSrcRgb, out rgbDst);
                        return (uint)BlendFromOgl(rgbDst);
                    }
                case RenderState.SourceBlendAlpha:
                    {
                        int alphaSrc;
                        GL.GetInteger(GetPName.BlendSrcAlpha, out alphaSrc);
                        return (uint)BlendFromOgl(alphaSrc);
                    }
                case RenderState.DestinationBlendAlpha:
                    {
                        int alphaDst;
                        GL.GetInteger(GetPName.BlendDstAlpha, out alphaDst);
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
        public ImageData GetPixelColor(int x, int y, int w = 1, int h = 1)
        {
            ImageData image = ImageData.CreateImage(w, h, ColorUint.Black);
            GL.ReadPixels(x, y, w, h, OpenTK.Graphics.OpenGL.PixelFormat.Bgr /* yuk, yuk ??? */, PixelType.UnsignedByte, image.PixelData);
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
            GL.ReadPixels(x, y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.UnsignedByte, ref depth);

            return depth;
        }

        #endregion
    }
}