using System;
using Fusee.Math;

namespace Fusee.Engine
{
    public abstract class GUIElement
    {
        protected RenderContext RContext;
        
        protected float PosX;
        protected float PosY;

        protected float Height;
        protected float Width;

        protected string Text;
        protected IFont Font;

        public float4 TextColor { get; set; }
        public Mesh GUIMesh { get; protected set; }

        // shader
        public ShaderEffect GUIShader;

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

        protected abstract void CreateMesh();

        protected GUIElement(RenderContext rc, string text, IFont font, float x, float y, float width, float height)
        {
            RContext = rc;

            // x, y, width, height
            PosX = x;
            PosY = y;

            Width = width;
            Height = height;

            // settings
            Text = text;
            Font = font;

            // shader
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
                        ZEnable = false
                    }
                }
            },
                new[] {new EffectParameterDeclaration {Name = "tex", Value = Font.TexAtlas}});

            GUIShader.AttachToContext(RContext);
        }

        public void Refresh()
        {
            CreateMesh();
        }
    }
}