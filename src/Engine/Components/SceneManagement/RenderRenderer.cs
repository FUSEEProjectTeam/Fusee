using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Engine;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// Creates a Renderer Component that will be passed to the RenderContext.
    /// </summary>
    public class RenderRenderer : RenderJob
    {

        private Renderer _renderer;
        private IShaderParam _uColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderRenderer"/> class. Needs to be provided with a Renderer Component.
        /// </summary>
        public RenderRenderer(Renderer renderer)
        {
            _renderer = renderer;
            _uColor = null;
        }
        /// <summary>
        /// Overwrites the SubmitWork method of RenderJob class. Render Component will get shaders and materials and passed to the RenderContextImplementation.
        /// </summary>
        public override void SubmitWork(RenderContext renderContext)
        {
            if(_renderer.material.sp==null)
            {
                //_renderer.material.sp = MoreShaders.GetShader("diffuse", renderContext);
                _renderer.material.sp = renderContext.CreateShader(_renderer.material._vs, _renderer.material._ps);
                _renderer.material.InitValues(renderContext);
            }

            _renderer.material.UpdateValues(renderContext); 
            renderContext.SetShader(_renderer.material.sp);

        }
    }
}
