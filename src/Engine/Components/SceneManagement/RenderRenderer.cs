using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Engine;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// Creates a RendererRenderer Component that will be passed to the RenderContext as the current shaderprogram. 
    /// </summary>
    public class RenderRenderer : RenderJob
    {

        private Renderer _renderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderRenderer"/> class. Needs to be provided with a Renderer Component.
        /// </summary>
        public RenderRenderer(Renderer renderer)
        {
            _renderer = renderer;
            //_uColor = null;
        }
        /// <summary>
        /// Overwrites the SubmitWork method of RenderJob class. Render Component will get shaders and materials and passed to the RenderContextImplementation.
        /// </summary>
        public override void SubmitWork(RenderContext renderContext)
        {
 
            _renderer.material.Update(renderContext);
            SceneManager.Manager.UpdateLights();
        }
    }
}
