using System;
using System.Diagnostics;
using System.IO;
using Fusee.Math;
using JSIL.Meta;

namespace Fusee.Engine
{
    partial class RenderContext
    {
        private readonly ShaderProgram _textShader;
        private readonly IShaderParam _textTextureParam;

        public IFont LoadFont(string filename, uint size)
        {
            if (!File.Exists(filename))
                throw new Exception("Font not found: " + filename);

            return _rci.LoadFont(filename, size);
        }

        [JSExternal]
        public IFont LoadSystemFont(string fontname, uint size)
        {
            var fontsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            var pathToFont = Path.Combine(fontsFolder, fontname + ".ttf");

            return LoadFont(pathToFont, size);
        }

        public void TextOut(Mesh textMesh, IFont font)
        {
            var curShader = _currentShader;

            if (_currentShader != _textShader)
                SetShader(_textShader);

            SetShaderParamTexture(_textTextureParam, font.TexAtlas);

            _rci.PrepareTextRendering(true);
            Render(textMesh);
            _rci.PrepareTextRendering(false);

            if (curShader != null && curShader != _textShader)
                SetShader(curShader);
        }

        public void TextOut(string text, IFont font, float4 color, float x, float y)
        {
           // TextOut(GetTextMesh(text, font, x, y, color), font);
        }

        internal float3[] FixTextKerning(IFont font, float3[] vertices, string text, float scaleX)
        {
            return _rci.FixTextKerning(font, vertices, text, scaleX);
        }
    }
}
