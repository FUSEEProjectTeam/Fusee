using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using Fusee.Math;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpFont;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Fusee.Engine
{
    /// <summary>
    /// Implementation of the <see cref="IRenderContextImp" /> interface for usage with OpenTK framework.
    /// </summary>
    public class RenderContextImp : IRenderContextImp
    {
        #region Fields

        private int _currentTextureUnit;
        private readonly Dictionary<int, int> _shaderParam2TexUnit;

        private readonly Library _sharpFont;

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

            // TODO: dispose at the end
            _sharpFont = new Library();
        }

        #endregion

        #region Image data related Members

        /// <summary>
        /// Creates a new Bitmap-Object from an image file,
        /// locks the bits in the memory and makes them available
        /// for furher action (e.g. creating a texture).
        /// Method must be called before creating a texture to get the necessary
        /// ImageData struct.
        /// </summary>
        /// <param name="filename">Path to the image file you would like to use as texture.</param>
        /// <returns>An ImageData object with all necessary information for the texture-binding process.</returns>
        public ImageData LoadImage(String filename)
        {
            var bmp = new Bitmap(filename);

            //Flip y-axis, otherwise texture would be upside down
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
                Stride = bmpData.Stride
            };

            Marshal.Copy(bmpData.Scan0, ret.PixelData, 0, bytes);

            bmp.UnlockBits(bmpData);
            return ret;
        }

        /// <summary>
        /// Creates a new Image with a specified size and color.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bgColor">The color of the image. Value must be JS compatible.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing.</returns>
        public ImageData CreateImage(int width, int height, String bgColor)
        {
            var bmp = new Bitmap(width, height);
            Graphics gfx = Graphics.FromImage(bmp);
            Color color = Color.FromName(bgColor);
            gfx.Clear(color);

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

            Graphics gfx = Graphics.FromImage(bmp);
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

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITexture that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITexture CreateTexture(ImageData img)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, img.Width, img.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, img.PixelData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMinFilter.Linear);

            ITexture texID = new Texture {handle = id};
            return texID;
        }

        #endregion

        #region Text related Members

        public IFont LoadFont(string filename, uint size)
        {
            var texAtlas = new Font
            {
                Face = _sharpFont.NewFace(filename, 0),
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

            var texAtlas = ((Font)font);
            var face = texAtlas.Face;

            // get atlas texture size
            var rowW = 0;
            var rowH = 0;
            var h = 0;

            const int maxWidth = 512;

            for (uint i = 32; i < 256; i++)
            {
                face.LoadChar(i, LoadFlags.Default, LoadTarget.Normal);

                if (rowW + (face.Glyph.Advance.X >> 6) + 1 >= maxWidth)
                {
                    h += rowH;
                    rowW = 0;
                    rowH = 0;
                }

                rowW += (face.Glyph.Advance.X >> 6) + 1;
                rowH = System.Math.Max(face.Glyph.Metrics.Height >> 6, rowH);
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
                (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int)TextureWrapMode.ClampToEdge);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            texAtlas.TexAtlas = new Texture { handle = tex };

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
                texAtlas.CharInfo[i].AdvanceX = face.Glyph.Advance.X >> 6;
                texAtlas.CharInfo[i].AdvanceY = face.Glyph.Advance.Y >> 6;

                texAtlas.CharInfo[i].BitmapW = face.Glyph.Bitmap.Width;
                texAtlas.CharInfo[i].BitmapH = face.Glyph.Bitmap.Rows;

                texAtlas.CharInfo[i].BitmapL = face.Glyph.BitmapLeft;
                texAtlas.CharInfo[i].BitmapT = face.Glyph.BitmapTop;

                texAtlas.CharInfo[i].TexOffX = offX / (float)maxWidth;
                texAtlas.CharInfo[i].TexOffY = offY / (float)potH;

                rowH = System.Math.Max(rowH, face.Glyph.Bitmap.Rows);
                offX += face.Glyph.Bitmap.Width + 1;
            }

            return texAtlas;
        }

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

                fixX += (texAtlas.Face.GetKerning(leftChar, rightChar, KerningMode.Default).X >> 6)*scaleX;

                vertices[fixVert++].x += fixX;
                vertices[fixVert++].x += fixX;
                vertices[fixVert++].x += fixX;
                vertices[fixVert++].x += fixX;
            }

            return vertices;
        }

        public void PrepareTextRendering(bool active)
        {
            if (active)
            {
                // save current state
                GL.PushAttrib(AttribMask.EnableBit | AttribMask.ColorBufferBit);

                // set state for text rendering
                GL.Disable(EnableCap.DepthTest);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            }
            else
            {
                GL.PopAttrib();
            }
        }

        #endregion

        #region Matrix Fields

        /// <summary>
        /// Gets or sets the model view.
        /// </summary>
        /// <value>
        /// The model view.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public float4x4 ModelView
        {
            get { throw new NotImplementedException(); }
            set
            {
                GL.MatrixMode(MatrixMode.Modelview);
                unsafe
                {
                    GL.LoadMatrix((float*) (&value));
                }
            }
        }

        /// <summary>
        /// The projection matrix used by the rendering pipeline
        /// </summary>
        /// <value>
        /// The 4x4 projection matrix applied to view coordinates yielding clip space coordinates.
        /// </value>
        /// <remarks>
        /// View coordinates are the result of the ModelView matrix multiplied to the geometry (<see cref="Fusee.Engine.RenderContext.ModelView"/>).
        /// The coordinate system of the view space has its origin in the camera center with the z axis aligned to the viewing direction, and the x- and
        /// y axes aligned to the viewing plane. Still, no projection from 3d space to the viewing plane has been performed. This is done by multiplying
        /// view coordinate geometry wihth the projection matrix. Typically, the projection matrix either performs a parallel projection or a perspective
        /// projection.
        /// </remarks>
        public float4x4 Projection
        {
            get { throw new NotImplementedException(); }
            set
            {
                GL.MatrixMode(MatrixMode.Projection);
                unsafe
                {
                    GL.LoadMatrix((float*) (&value));
                }
            }
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
            int h = GL.GetUniformLocation(((ShaderProgramImp)shaderProgram).Program, paramName);
            return (h == -1) ? null : new ShaderParam { handle = h };
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
            GL.GetUniform(((ShaderProgramImp)program).Program, ((ShaderParam)param).handle, out f);
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
            var sp = (ShaderProgramImp)shaderProgram;
            int nParams;
            GL.GetProgram(sp.Program, ProgramParameter.ActiveUniforms, out nParams);
            var list = new List<ShaderParamInfo>();
            for (int i = 0; i < nParams; i++)
            {
                ActiveUniformType t;
                var ret = new ShaderParamInfo();
                ret.Name = GL.GetActiveUniform(sp.Program, i, out ret.Size, out t);
                ret.Handle = GetShaderParam(sp, ret.Name);
                switch (t)
                {
                    case ActiveUniformType.Int:
                        ret.Type = typeof(int);
                        break;
                    case ActiveUniformType.Float:
                        ret.Type = typeof(float);
                        break;
                    case ActiveUniformType.FloatVec2:
                        ret.Type = typeof(float2);
                        break;
                    case ActiveUniformType.FloatVec3:
                        ret.Type = typeof(float3);
                        break;
                    case ActiveUniformType.FloatVec4:
                        ret.Type = typeof(float4);
                        break;
                    case ActiveUniformType.FloatMat4:
                        ret.Type = typeof(float4x4);
                        break;
                    case ActiveUniformType.Sampler2D:
                        //TODO ret.Type = typeof (sampler?);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                list.Add(ret);
            }
            return list;
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
        /// Sets a <see cref="float3" /> shader parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="val">The value.</param>
        public void SetShaderParam(IShaderParam param, float3 val)
        {
            GL.Uniform3(((ShaderParam)param).handle, val.x, val.y, val.z);
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
                var mF = (float*)(&val);
                GL.UniformMatrix4(((ShaderParam)param).handle, 1, false, mF);
            }
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

        #endregion

        #region Rendering related Members

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
        public void GetBufferContent(Rectangle quad, ITexture texId)
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
        /// Specify the View Frustum by settings its left,right,bottom,top,near and far planes. 
        /// Image the View frustum as a cubical form that determines the Cameras 3D view along its far plane. 
        /// </summary>
        /// <param name="left">The left plane.</param>
        /// <param name="right">The right plane.</param>
        /// <param name="bottom">The bottom plane.</param>
        /// <param name="top">The top plane.</param>
        /// <param name="zNear">The z near plane.</param>
        /// <param name="zFar">The z far plane.</param>
        public void Frustum(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            GL.Frustum(left, right, bottom, top, zNear, zFar);
        }

        #endregion
    }
}