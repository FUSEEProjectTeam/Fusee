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

        protected int Height;
        protected int Width;

        protected string Text;
        protected IFont Font;

        #endregion

        #region Private Fields

        private int _offsetX;
        private int _offsetY;

        private float4 _textColor;

        // shader
        private readonly ShaderEffect _guiShader;
        private readonly ShaderEffect _textShader;

        private const string GUIVS = @"
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

        private const string GUIPS = @"
            #ifdef GL_ES
                precision highp float;
            #endif    
  
            varying vec2 vUV;
            varying vec4 vColor;

            void main(void) {
                gl_FragColor = vColor;
            }";

        private const string TEXTPS = @"
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

        #region Internal Fields

        protected internal int OffsetX
        {
            get { return _offsetX; }
            set
            {
                _offsetX = value;
                Dirty = true;
            }
        }

        protected internal int OffsetY
        {
            get { return _offsetY; }
            set
            {
                _offsetY = value;
                Dirty = true;
            }
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

        protected GUIElement(RenderContext rc, string text, IFont font, int x, int y, int width, int height)
        {
            RContext = rc;
            Dirty = false;

            // x, y, width, height
            PosX = x;
            PosY = y;

            OffsetX = 0;
            OffsetY = 0;

            Width = width;
            Height = height;

            // settings
            Text = text;
            Font = font;

            // shader
            _guiShader = new ShaderEffect(new[]
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

            _textShader = new ShaderEffect(new[]
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

            _guiShader.AttachToContext(RContext);
            _textShader.AttachToContext(RContext);
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

        protected void DrawRectangle(float c1, float c2, float c3, float c4, int vtStart, int indStart, float4 color, ref float3[] vertices, ref ushort[] indices, ref uint[] colors)
        {
            // vertices
            vertices[vtStart+0] = new float3(c1, c3, 0);
            vertices[vtStart+1] = new float3(c1, c4, 0);
            vertices[vtStart+2] = new float3(c2, c3, 0);
            vertices[vtStart+3] = new float3(c2, c4, 0);

            // colors
            var colorInt = MathHelper.Float4ToABGR(color);

            colors[vtStart+0] = colorInt;
            colors[vtStart+1] = colorInt;
            colors[vtStart+2] = colorInt;
            colors[vtStart+3] = colorInt;

            // indices
            indices[indStart+0] = (ushort)(vtStart + 1);
            indices[indStart+1] = (ushort)(vtStart + 0);
            indices[indStart+2] = (ushort)(vtStart + 2);

            indices[indStart+3] = (ushort)(vtStart + 1);
            indices[indStart+4] = (ushort)(vtStart + 2);
            indices[indStart+5] = (ushort)(vtStart + 3);
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

        protected void CreateTextMesh(float3[] vertices, float2[] uvs, ushort[] indices, uint[] colors)
        {
            if (TextMesh == null)
                TextMesh = new Mesh { Vertices = vertices, UVs = uvs, Triangles = indices, Colors = colors };
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
            Dirty = false;
            CreateMesh();
        }

        protected virtual void PreRender()
        {
            if (Dirty)
                Refresh();
        }

        public void Render()
        {
            PreRender();

            if (GUIMesh != null) _guiShader.RenderMesh(GUIMesh);
            if (TextMesh != null) _textShader.RenderMesh(TextMesh);
        }
    }
}