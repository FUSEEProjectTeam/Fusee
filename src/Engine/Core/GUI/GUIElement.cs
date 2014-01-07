using Fusee.Math;

namespace Fusee.Engine
{
    public abstract class GUIElement
    {
        #region Fields

        #region Protected Fields

        protected RenderContext RContext;

        protected bool Dirty;

        protected int PosX;
        protected int PosY;
        protected int PosZ;

        protected int Height;
        protected int Width;

        protected string Text;
        protected IFont Font;

        protected string ImgSrc;
        protected ITexture GUITexture;

        // shader
        protected ShaderEffect GUIShader;
        protected ShaderEffect TextShader;

        protected const string GUIVS = @"
            attribute vec3 fuVertex;
            attribute vec2 fuUV;
            attribute vec4 fuColor;

            varying vec2 vUV;
            varying vec4 vColor;

            void main()
            {
                vUV = fuUV;
                vColor = fuColor;

                gl_Position = vec4(fuVertex, 1);
            }";

        protected const string GUIPS = @"
            #ifdef GL_ES
                precision highp float;
            #endif    
  
            varying vec2 vUV;
            varying vec4 vColor;

            void main(void) {
                gl_FragColor = vColor;
            }";

        protected const string TEXTPS = @"
            #ifdef GL_ES
                precision highp float;
            #endif    
  
            varying vec2 vUV;
            varying vec4 vColor;

            uniform sampler2D tex;

            void main(void) {
                gl_FragColor = vec4(1, 1, 1, texture2D(tex, vUV).a) * vColor;
            }";

        #endregion

        #region Private Fields

        private int _offsetX;
        private int _offsetY;
        private int _offsetZ;

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

        public float4 TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                Dirty = true;
            }
        }

        public Mesh GUIMesh { get; protected set; }
        public Mesh TextMesh { get; protected set; }

        #endregion

        #endregion

        protected abstract void CreateMesh();

        protected GUIElement(string text, IFont font, int x, int y, int z, int width, int height)
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
            Font = font;

            // shader
            if (Font != null) CreateTextShader();
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
                null);
        }

        protected void CreateTextShader()
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
                new[] {new EffectParameterDeclaration {Name = "tex", Value = Font.TexAtlas}});
        }

        protected internal virtual void AttachToContext(RenderContext rc)
        {
            RContext = rc;

            if (GUIShader != null) GUIShader.AttachToContext(RContext);
            if (TextShader != null) TextShader.AttachToContext(RContext);

            Refresh();
        }

        protected void SetTextMesh(int posX, int posY)
        {
            if (Font == null)
                return;

            // relative coordinates from -1 to +1
            var scaleX = (float) 2/RContext.ViewportWidth;
            var scaleY = (float) 2/RContext.ViewportHeight;

            var x = -1 + posX*scaleX;
            var y = +1 - posY*scaleY;

            // build complete structure
            var vertices = new float3[4*Text.Length];
            var uvs = new float2[4*Text.Length];
            var indices = new ushort[6*Text.Length];
            var colors = new uint[4*Text.Length];


            var charInfo = Font.CharInfo;
            var atlasWidth = Font.Width;
            var atlasHeight = Font.Height;

            var index = 0;
            ushort vertex = 0;

            // now build the mesh
            foreach (var letter in Text)
            {
                var x2 = x + charInfo[letter].BitmapL*scaleX;
                var y2 = -y - charInfo[letter].BitmapT*scaleY;
                var w = charInfo[letter].BitmapW*scaleX;
                var h = charInfo[letter].BitmapH*scaleY;

                x += charInfo[letter].AdvanceX*scaleX;
                y += charInfo[letter].AdvanceY*scaleY;

                // skip glyphs that have no pixels
                if ((w <= MathHelper.EpsilonFloat) || (h <= MathHelper.EpsilonFloat))
                    continue;

                var bitmapW = charInfo[letter].BitmapW;
                var bitmapH = charInfo[letter].BitmapH;
                var texOffsetX = charInfo[letter].TexOffX;
                var texOffsetY = charInfo[letter].TexOffY;

                // vertices
                vertices[vertex] = new float3(x2, -y2 - h, 0);
                vertices[vertex + 1] = new float3(x2, -y2, 0);
                vertices[vertex + 2] = new float3(x2 + w, -y2 - h, 0);
                vertices[vertex + 3] = new float3(x2 + w, -y2, 0);

                // uvs
                uvs[vertex] = new float2(texOffsetX, texOffsetY + bitmapH/atlasHeight);
                uvs[vertex + 1] = new float2(texOffsetX, texOffsetY);
                uvs[vertex + 2] = new float2(texOffsetX + bitmapW/atlasWidth, texOffsetY + bitmapH/atlasHeight);
                uvs[vertex + 3] = new float2(texOffsetX + bitmapW/atlasWidth, texOffsetY);

                // colors
                var colorInt = MathHelper.Float4ToABGR(TextColor);

                colors[vertex] = colorInt;
                colors[vertex + 1] = colorInt;
                colors[vertex + 2] = colorInt;
                colors[vertex + 3] = colorInt;

                // indices
                indices[index++] = (ushort) (vertex + 1);
                indices[index++] = vertex;
                indices[index++] = (ushort) (vertex + 2);

                indices[index++] = (ushort) (vertex + 1);
                indices[index++] = (ushort) (vertex + 2);
                indices[index++] = (ushort) (vertex + 3);

                vertex += 4;
            }

            vertices = RContext.FixTextKerning(Font, vertices, Text, scaleX);

            // create final mesh
            CreateTextMesh(vertices, uvs, indices, colors);
        }

        protected void SetRectangleMesh(float borderWidth, float4 rectColor, float4 borderColor)
        {
            var x = PosX + OffsetX;
            var y = PosY + OffsetY;

            // relative coordinates from -1 to +1
            var scaleX = (float) 2/RContext.ViewportWidth;
            var scaleY = (float) 2/RContext.ViewportHeight;

            var xS = -1 + x*scaleX;
            var yS = +1 - y*scaleY;

            var width = Width*scaleX;
            var height = Height*scaleY;

            var borderX = System.Math.Max(0, borderWidth*scaleX);
            var borderY = System.Math.Max(0, borderWidth*scaleY);

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
            var colorInt = MathHelper.Float4ToABGR(color);

            colors[vtStart + 0] = colorInt;
            colors[vtStart + 1] = colorInt;
            colors[vtStart + 2] = colorInt;
            colors[vtStart + 3] = colorInt;

            // indices
            indices[indStart + 0] = (ushort) (vtStart + 1);
            indices[indStart + 1] = (ushort) (vtStart + 0);
            indices[indStart + 2] = (ushort) (vtStart + 2);

            indices[indStart + 3] = (ushort) (vtStart + 1);
            indices[indStart + 4] = (ushort) (vtStart + 2);
            indices[indStart + 5] = (ushort) (vtStart + 3);
        }

        protected void CreateGUIMesh(float3[] vertices, float2[] uvs, ushort[] indices, uint[] colors)
        {
            if (GUIMesh == null)
                GUIMesh = new Mesh {Vertices = vertices, UVs = uvs, Triangles = indices, Colors = colors};
            else
            {
                GUIMesh.Vertices = vertices;
                GUIMesh.UVs = uvs;
                GUIMesh.Triangles = indices;
                GUIMesh.Colors = colors;
            }
        }

        protected void CreateTextMesh(float3[] vertices, float2[] uvs, ushort[] indices, uint[] colors)
        {
            if (TextMesh == null)
                TextMesh = new Mesh {Vertices = vertices, UVs = uvs, Triangles = indices, Colors = colors};
            else
            {
                TextMesh.Vertices = vertices;
                TextMesh.UVs = uvs;
                TextMesh.Triangles = indices;
                TextMesh.Colors = colors;
            }
        }

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
            if (RContext != rc) AttachToContext(rc);
            if (Dirty) Refresh();
        }

        public void Render(RenderContext rc)
        {
            PreRender(rc);

            if (GUIShader != null && GUIMesh != null)
                GUIShader.RenderMesh(GUIMesh);

            if (TextShader != null && TextMesh != null)
                TextShader.RenderMesh(TextMesh);
        }
    }
}