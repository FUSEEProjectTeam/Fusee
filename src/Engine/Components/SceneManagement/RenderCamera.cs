using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// RenderCamera is derived from Renderjob and is responible for passing the Camera towards the RenderContext.
    /// </summary>
    public class RenderCamera : RenderJob
    {
        private float4x4 _matrix;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCamera"/> class. Needs the ViewMatrix of a Camera.
        /// </summary>
        /// <param name="matrix">The matrix that will be the new ViewMatrix.</param>
        public RenderCamera(float4x4 matrix)
        {
            _matrix = matrix;
        }

        /// <summary>
        /// Sets the ViewMatrix in the RenderContext.
        /// </summary>
        /// <param name="renderContext">The RenderContext that handles the rendering.</param>
        public override void SubmitWork(RenderContext renderContext)
        {
            renderContext.View=_matrix;
        }
    }
}
