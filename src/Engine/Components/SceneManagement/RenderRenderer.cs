using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Engine;

namespace Fusee.SceneManagement
{
    public class RenderRenderer : RenderJob
    {
        private Renderer _renderer;
        
        public RenderRenderer(Renderer renderer)
        {
            _renderer = renderer;
        }
        public override void SubmitWork(RenderContext renderContext)
        {
            if(_renderer.sp==null)
            {
                _renderer.sp = renderContext.CreateShader(_renderer.material._vs, _renderer.material._ps);
            }
            IShaderParam vcolor = _renderer.sp.GetShaderParam("vColor");
            renderContext.SetShaderParam(vcolor, _renderer.color);
            renderContext.SetShader(_renderer.sp);
        }
    }
}
