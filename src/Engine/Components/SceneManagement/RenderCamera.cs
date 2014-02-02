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
        #region Fields
        private float4x4 _matrix;
        private bool _dirty;
        private float4x4 _projection;
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCamera"/> class. Needs the ViewMatrix of a Camera.
        /// </summary>
        /// <param name="matrix">The matrix that will be the new ViewMatrix.</param>
        /// <param name="projection">The matrix that will be the new projection.</param>
        /// <param name="dirty">True if values have changed; otherwise, false.</param>
        public RenderCamera(float4x4 matrix, float4x4 projection, bool dirty)
        {
            _matrix = matrix;
            _projection = projection;
            _dirty = dirty;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// Sets the ViewMatrix in the RenderContext.
        /// </summary>
        /// <param name="renderContext">The RenderContext that handles the rendering.</param>
        public override void SubmitWork(RenderContext renderContext)
        {
            if(_dirty)
            {
                renderContext.Projection = _projection;
            }

            renderContext.View=_matrix;
        }
        #endregion
    }
}
