using Fusee.Math;

namespace Fusee.Engine
{
    public sealed class GUIImage : GUIElement
    {
        #region Private Fields

        private const string IMGPS = @"
            #ifdef GL_ES
                precision highp float;
            #endif    
  
            varying vec2 vUV;
            varying vec4 vColor;

            uniform sampler2D tex;

            void main(void) {
                if (vUV.x == -1.0)
                    gl_FragColor = vColor;
                else
                    gl_FragColor = texture2D(tex, vUV);
            }";

        private float4 _borderColor;
        private int _borderWidth;

        #endregion

        #region Public Fields

        public int BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                _borderWidth = value;
                Dirty = true;
            }
        }

        public float4 BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                Dirty = true;
            }
        }

        #endregion

        public GUIImage(string img, int x, int y, int z, int width, int height)
            : base("", null, x, y, z, width, height)
        {
            // settings
            ImgSrc = img;
            BorderWidth = 0;
        }

        public GUIImage(string img, int x, int y, int width, int height)
            : base("", null, x, y, 0, width, height)
        {
            // settings
            ImgSrc = img;
            BorderWidth = 0;
        }

        protected override void CreateGUIShader()
        {
            GUIShader = new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = GUIVS,
                    PS = IMGPS,
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        SourceBlend = Blend.SourceAlpha,
                        DestinationBlend = Blend.InverseSourceAlpha,
                        ZEnable = true
                    }
                }
            },
                new[] {new EffectParameterDeclaration {Name = "tex", Value = GUITexture}});
        }

        protected internal override void AttachToContext(RenderContext rc)
        {
            if (RContext == rc) return;

            if (ImgSrc != null)
            {
                var imgData = rc.LoadImage(ImgSrc);
                GUITexture = rc.CreateTexture(imgData);

                CreateGUIShader();
            }

            base.AttachToContext(rc);
        }

        protected override void CreateMesh()
        {
            SetRectangleMesh(BorderWidth, new float4(1, 1, 1, 1), BorderColor);

            var uvs = GUIMesh.UVs;

            uvs[0] = new float2(0, 0);
            uvs[1] = new float2(0, 1);
            uvs[2] = new float2(1, 0);
            uvs[3] = new float2(1, 1);

            if (BorderWidth > 0)
                for (var i = 4; i < 8; i++)
                    uvs[i] = new float2(-1, -1);

            GUIMesh.UVs = uvs;
        }
    }
}