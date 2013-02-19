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
            if(_renderer.material.sp==null)
            {
                _renderer.material.sp = renderContext.CreateShader(_renderer.material._vs, _renderer.material._ps);
                _renderer.material.InitValues(renderContext);
            }
          /* if(renderContext.CurrentShader != _renderer.material.sp)
            {
            
                renderContext.SetShader(_renderer.material.sp);
            }else
           {
               _renderer.material.UpdateValues(renderContext); 
           }*/
            _renderer.material.UpdateValues(renderContext); 
            renderContext.SetShader(_renderer.material.sp);
            /*if (_uColor == null)
            {
                _uColor = _renderer.sp.GetShaderParam("vColor");
            }
            renderContext.SetShaderParam(_uColor, _renderer.color);
             */
        }
    }
}
