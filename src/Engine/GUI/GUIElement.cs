using System;
using Fusee.Base.Common;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.GUI
{
    /// <summary>
    /// This abstact class allows for creating custom <see cref="GUIElement"/>s.
    /// </summary>
    public abstract class GUIElement
    {
        #region Fields

        #region Protected Fields

        protected RenderContext RContext;

        protected bool Dirty;

        protected int Height;
        protected int Width;

        protected int PosZ;

        // protected IFont Font;
        protected FontMap FontMap;

        protected IImageData ImgSrc;
        protected Texture GUITexture;

        // shader
        protected ShaderEffect GUIShader;
        protected ShaderEffect TextShader;

        protected IShaderParam ColorParam;

        protected readonly string GUIVS = @"#version 300 es
            uniform mat4 guiXForm;
            in vec3 fuVertex;
            in vec2 fuUV;
            in vec4 fuColor;

            out vec2 vUV;
            out vec4 vColor;

            void main()
            {
                vUV = fuUV;
                vColor = fuColor;

                gl_Position = guiXForm * vec4(fuVertex, 1);
            }";

        protected readonly string GUIPS = @"#version 300 es
                precision highp float; 
  
            in vec2 vUV;
            in vec4 vColor;

            out vec4 fragColor;

            void main(void) {
                fragColor = vColor;
            }";

        protected readonly string TEXTPS = @"#version 300 es
                precision highp float;
  
            in vec2 vUV;
            in vec4 vColor;

            uniform sampler2D tex;
            uniform vec4 uColor;

            out vec4 fragColor;

            void main(void) {
                fragColor = vec4(1.0, 1.0, 1.0, texture(tex, vUV).a) * uColor;
            }";

        #endregion

        #region Private Fields

        private int _posX;
        private int _posY;
        private float4x4 _rotation;

        private int _offsetX;
        private int _offsetY;
        private int _offsetZ;

        private string _text;
        private float4 _textColor;

        #endregion

        #region Internal Fields

        protected internal int OffsetX
        {
            get { return _offsetX; }
            set
            {
                if (value != _offsetX)
                    Dirty = true;

                _offsetX = value;
            }
        }

        protected internal int OffsetY
        {
            get { return _offsetY; }
            set
            {
                if (value != _offsetY)
                    Dirty = true;

                _offsetY = value;
            }
        }

        protected internal int OffsetZ
        {
            get { return _offsetZ; }
            set
            {
                if (value != _offsetZ)
                    Dirty = true;

                _offsetZ = value;
            }
        }

        protected internal int ZIndex
        {
            get { return PosZ + _offsetZ; }
        }

        #endregion

        #region Public Fields

        /// <summary>
        ///     Gets or sets the color of this element's text.
        /// </summary>
        /// <value>
        ///     The color of the text.
        /// </value>
        public float4 TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        /// <summary>
        ///     Gets or sets this element's text.
        /// </summary>
        /// <value>
        ///     The text.
        /// </value>
        public String Text
        {
            get { return _text; }
            set
            {
                if (value != _text)
                    Dirty = true;

                _text = value;
            }
        }

        /// <summary>
        ///     Gets or sets this element's x-coordinate.
        /// </summary>
        /// <value>
        ///     The x-coordinate.
        /// </value>
        public int PosX
        {
            get { return _posX; }
            set
            {
                if (value != _posX)
                    Dirty = true;

                _posX = value;
            }
        }

        /// <summary>
        ///     Gets or sets this element's y-coordinate.
        /// </summary>
        /// <value>
        ///     The y-coordinate.
        /// </value>
        public int PosY
        {
            get { return _posY; }
            set
            {
                if (value != _posY)
                    Dirty = true;

                _posY = value;
            }
        }

        /// <summary>
        ///     Gets or sets this element's rotation about the z-axis.
        /// </summary>
        /// <value>
        ///     The z-rotation.
        /// </value>
        public float ZRot { set; get; }

        /// <summary>
        ///     Gets or sets this element's pivot's point x-coordinate.
        /// </summary>
        /// <value>
        ///     The x-coordinate of the pivot point.
        /// </value>
        public int XPivot { set; get; }

        /// <summary>
        ///     Gets or sets this element's pivot's point y-coordinate.
        /// </summary>
        /// <value>
        ///     The y-coordinate of the pivot point.
        /// </value>
        public int YPivot { set; get; }

        /// <summary>
        ///     Gets or sets the tag.
        /// </summary>
        /// <value>
        ///     The tag.
        /// </value>
        /// <remarks>
        ///     The tag can be used to store information about other GUIElements
        ///     or any other kind of information like numbers or strings.
        /// </remarks>
        public object Tag { get; set; }

        /// <summary>
        ///     Gets the automatically generated GUI mesh.
        /// </summary>
        /// <value>
        ///     The GUI mesh.
        /// </value>
        public Mesh GUIMesh { get; protected set; }

        /// <summary>
        ///     Gets the automatically generated text mesh.
        /// </summary>
        /// <value>
        ///     The text mesh.
        /// </value>
        public Mesh TextMesh { get; protected set; }

        #endregion

        #endregion

        protected abstract void CreateMesh();

        protected GUIElement(string text, FontMap fontMap, int x, int y, int z, int width, int height, float zRot = 0, int xPivot = 0, int yPivot = 0)
        {
            Dirty = false;

            // x, y, width, height
            PosX = x;
            PosY = y;
            PosZ = z;

            OffsetX = 0;
            OffsetY = 0;
            OffsetZ = 0;

            Width = width;
            Height = height;

            // settings
            Text = text;
            FontMap = fontMap;

            ZRot = zRot;
            XPivot = xPivot;
            YPivot = yPivot;
            // shader
            //if (FontMap != null) CreateTextShader(RContext.CreateTexture(FontMap.Image));


        }

        protected virtual void CreateGUIShader()
        {
            GUIShader = new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = GUIVS,
                    PS = GUIPS,
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        SourceBlend = Blend.SourceAlpha,
                        DestinationBlend = Blend.InverseSourceAlpha,
                        ZEnable = true
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration { Name = "guiXForm", Value = float4x4.Identity },
            }
            );
        }

        protected void CreateTextShader(Texture textAtlas)
        {
            TextShader = new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = GUIVS,
                    PS = TEXTPS,
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        SourceBlend = Blend.SourceAlpha,
                        DestinationBlend = Blend.InverseSourceAlpha,
                        ZEnable = false
                    }
                }
            },
                new[]
                {
                    new EffectParameterDeclaration {Name = "tex", Value = textAtlas},
                    new EffectParameterDeclaration {Name = "uColor", Value = _textColor},
                    new EffectParameterDeclaration {Name = "guiXForm", Value = float4x4.Identity},
                });
        }

        protected internal virtual void AttachToContext(RenderContext rc)
        {
            if (RContext == rc)
                return;

            if (RContext != null)
            {
                TextShader = null;
            }

            RContext = rc;


            if (FontMap != null)
            {
                CreateTextShader(new Texture(FontMap.Image));
            }


            Refresh();
        }

        protected internal virtual void DetachFromContext()
        {
            RContext = null;
        }

        protected void SetTextMesh(int posX, int posY)
        {
            if (FontMap == null)
                return;

            // relative coordinates from -1 to +1
            var scaleX = (float)2 / RContext.ViewportWidth;
            var scaleY = (float)2 / RContext.ViewportHeight;

            var x = -1 + posX * scaleX;
            var y = +1 - posY * scaleY;

            // build complete structure
            var vertices = new float3[4 * Text.Length];
            var uvs = new float2[4 * Text.Length];
            var indices = new ushort[6 * Text.Length];

            // var charInfo = Font.CharInfo;
            var atlasWidth = FontMap.Image.Width;
            var atlasHeight = FontMap.Image.Height;

            var index = 0;
            ushort vertex = 0;

            // now build the mesh
            foreach (var letter in Text)
            {
                GlyphOnMap glyphOnMap = FontMap.GetGlyphOnMap(letter);
                GlyphInfo glyphInfo = FontMap.Font.GetGlyphInfo(letter);

                var x2 = x + glyphOnMap.BitmapL * scaleX;
                var y2 = -y - glyphOnMap.BitmapT * scaleY;
                var w = glyphOnMap.BitmapW * scaleX;
                var h = glyphOnMap.BitmapH * scaleY;

                x += glyphInfo.AdvanceX * scaleX;
                y += glyphInfo.AdvanceY * scaleY;

                // skip glyphs that have no pixels
                if ((w <= M.EpsilonFloat) || (h <= M.EpsilonFloat))
                    continue;

                var bitmapW = glyphOnMap.BitmapW;
                var bitmapH = glyphOnMap.BitmapH;
                var texOffsetX = glyphOnMap.TexOffX;
                var texOffsetY = glyphOnMap.TexOffY;

                // vertices
                vertices[vertex] = new float3(x2, -y2 - h, 0);
                vertices[vertex + 1] = new float3(x2, -y2, 0);
                vertices[vertex + 2] = new float3(x2 + w, -y2 - h, 0);
                vertices[vertex + 3] = new float3(x2 + w, -y2, 0);

                // uvs
                uvs[vertex] = new float2(texOffsetX, texOffsetY + bitmapH / atlasHeight);
                uvs[vertex + 1] = new float2(texOffsetX, texOffsetY);
                uvs[vertex + 2] = new float2(texOffsetX + bitmapW / atlasWidth, texOffsetY + bitmapH / atlasHeight);
                uvs[vertex + 3] = new float2(texOffsetX + bitmapW / atlasWidth, texOffsetY);

                // indices
                indices[index++] = (ushort)(vertex + 1);
                indices[index++] = vertex;
                indices[index++] = (ushort)(vertex + 2);

                indices[index++] = (ushort)(vertex + 1);
                indices[index++] = (ushort)(vertex + 2);
                indices[index++] = (ushort)(vertex + 3);

                vertex += 4;
            }

            vertices = FontMap.FixTextKerning(vertices, Text, scaleX);

            // create final mesh
            CreateTextMesh(vertices, uvs, indices);
        }

        protected void SetRectangleMesh(float borderWidth, float4 rectColor, float4 borderColor)
        {
            var x = PosX + OffsetX;
            var y = PosY + OffsetY;

            // relative coordinates from -1 to +1
            var scaleX = (float)2 / RContext.ViewportWidth;
            var scaleY = (float)2 / RContext.ViewportHeight;

            var xS = -1 + x * scaleX;
            var yS = +1 - y * scaleY;

            var width = Width * scaleX;
            var height = Height * scaleY;

            var borderX = System.Math.Max(0, borderWidth * scaleX);
            var borderY = System.Math.Max(0, borderWidth * scaleY);

            // build complete structure
            var vertices = new float3[(borderWidth > 0) ? 8 : 4];
            var uvs = new float2[(borderWidth > 0) ? 8 : 4];
            var indices = new ushort[(borderWidth > 0) ? 12 : 6];
            var colors = new uint[(borderWidth > 0) ? 8 : 4];

            var c1 = xS + borderX;
            var c2 = xS - borderX + width;
            var c3 = yS - height + borderY;
            var c4 = yS - borderY;

            DrawRectangle(c1, c2, c3, c4, 0, 0, rectColor, ref vertices, ref indices, ref colors);

            // border
            if (borderWidth > 0)
            {
                c1 = xS;
                c2 = xS + width;
                c3 = yS - height;
                c4 = yS;

                DrawRectangle(c1, c2, c3, c4, 4, 6, borderColor, ref vertices, ref indices, ref colors);
            }

            CreateGUIMesh(vertices, uvs, indices, colors);
        }

        protected void DrawRectangle(float c1, float c2, float c3, float c4, int vtStart, int indStart, float4 color,
            ref float3[] vertices, ref ushort[] indices, ref uint[] colors)
        {
            // vertices
            vertices[vtStart + 0] = new float3(c1, c3, 0);
            vertices[vtStart + 1] = new float3(c1, c4, 0);
            vertices[vtStart + 2] = new float3(c2, c3, 0);
            vertices[vtStart + 3] = new float3(c2, c4, 0);

            // colors
            var colorInt = M.Float4ToABGR(color);

            colors[vtStart + 0] = colorInt;
            colors[vtStart + 1] = colorInt;
            colors[vtStart + 2] = colorInt;
            colors[vtStart + 3] = colorInt;

            // indices
            indices[indStart + 0] = (ushort)(vtStart + 1);
            indices[indStart + 1] = (ushort)(vtStart + 0);
            indices[indStart + 2] = (ushort)(vtStart + 2);

            indices[indStart + 3] = (ushort)(vtStart + 1);
            indices[indStart + 4] = (ushort)(vtStart + 2);
            indices[indStart + 5] = (ushort)(vtStart + 3);
        }

        protected void CreateGUIMesh(float3[] vertices, float2[] uvs, ushort[] indices, uint[] colors)
        {
            if (GUIMesh == null)
                GUIMesh = new Mesh { Vertices = vertices, UVs = uvs, Triangles = indices, Colors = colors };
            else
            {
                GUIMesh.Vertices = vertices;
                GUIMesh.UVs = uvs;
                GUIMesh.Triangles = indices;
                GUIMesh.Colors = colors;
            }
        }

        protected void CreateTextMesh(float3[] vertices, float2[] uvs, ushort[] indices)
        {
            if (TextMesh == null)
                TextMesh = new Mesh { Vertices = vertices, UVs = uvs, Triangles = indices };
            else
            {
                TextMesh.Vertices = vertices;
                TextMesh.UVs = uvs;
                TextMesh.Triangles = indices;
            }
        }

        /// <summary>
        ///     Refreshes this element (is called when the properties of this element have been changed).
        /// </summary>
        /// <remarks>
        ///     This should be called after the viewport / the windows has been resized. It's also possible to call the Refresh
        ///     method of a <see cref="GUIHandler" /> object."
        /// </remarks>
        public virtual void Refresh()
        {
            if (RContext != null)
            {
                Dirty = false;
                CreateMesh();
            }
        }


        protected virtual void PreRender(RenderContext rc)
        {

            if (RContext != rc)
                AttachToContext(rc);
            if (Dirty)
                Refresh();
        }

        /// <summary>
        ///     Renders this element onto a specifie <see cref="RenderContext" />.
        /// </summary>
        /// <param name="rc">The <see cref="RenderContext" />.</param>
        public void Render(RenderContext rc)
        {
            PreRender(rc);

            float3 clipPivot = new float3(
                                 XPivot * 2.0f / RContext.ViewportWidth - 1.0f, 1.0f - YPivot * 2.0f / RContext.ViewportHeight, 0);

            float4x4 guiXForm = float4x4.CreateTranslation(clipPivot)
                                    * float4x4.CreateScale(1.0f, (float)RContext.ViewportWidth / (float)RContext.ViewportHeight, 1) *
                                         float4x4.CreateRotationZ(ZRot)
                                    * float4x4.CreateScale(1.0f, (float)RContext.ViewportHeight / (float)RContext.ViewportWidth, 1) *
                                float4x4.CreateTranslation(-clipPivot);

            if (GUIShader != null)
            {
                RContext.SetShaderEffect(GUIShader);

                GUIShader.SetEffectParam("guiXForm", guiXForm);

                if (GUIShader != null && GUIMesh != null)
                    RContext.Render(GUIMesh);
            }

            if (FontMap != null)
            {
                //
                RContext.SetShaderEffect(TextShader);

                TextShader.SetEffectParam("guiXForm", guiXForm);

                if (TextShader != null && TextMesh != null)
                {
                    TextShader.SetEffectParam("uColor", _textColor);
                    RContext.Render(TextMesh);
                }
            }


        }
    }
}