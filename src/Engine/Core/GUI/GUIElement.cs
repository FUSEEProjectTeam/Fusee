using System;
using System.Diagnostics;
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

            uniform sampler2D tex;

            void main(void) {
                if (vUV.x == -1.0 && vUV.y == -1.0)
                    gl_FragColor = vColor;
                else
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
                        ZEnable = false
                    }
                }
            },
                new[] {new EffectParameterDeclaration {Name = "tex", Value = Font.TexAtlas}});

            _guiShader.AttachToContext(RContext);
        }

        public void Refresh()
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
            _guiShader.RenderMesh(GUIMesh);
            PostRender();
        }

        protected virtual void PostRender()
        {
            // nothing to do here
        }
    }
}