using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Engine;

namespace Fusee.SceneManagement
{
    public class RenderRenderer : RenderJob
    {
        private Renderer _renderer;
        private IShaderParam _uColor;

        public RenderRenderer(Renderer renderer)
        {
            _renderer = renderer;
            _uColor = null;
        }
        public override void SubmitWork(RenderContext renderContext)
        {
            if(_renderer.sp==null)
            {
                _renderer.sp = renderContext.CreateShader(_renderer.material._vs, _renderer.material._ps);
            }
            renderContext.SetShader(_renderer.sp);
            if (_uColor == null)
            {
                _uColor = _renderer.sp.GetShaderParam("uColor");
            }
            renderContext.SetShaderParam(_uColor, _renderer.color);
        }
    }
}
