using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Math;
using Fusee.Engine;
namespace Fusee.SceneManagement
{
    /// <summary>
    /// Creates a float4x4 matrix that will be passed to the RenderContext.
    /// </summary>
    public class RenderMatrix : RenderJob
    {
        #region Fields
        private float4x4 _matrix;
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderMatrix"/> class. Needs to be provided with a matrix that needs to be rendered.
        /// </summary>
        /// <param name="matrix">The matrix that need's to be rendered e.g. the matrix of a object in scene.</param>
        public RenderMatrix(float4x4 matrix)
        {
            _matrix = matrix;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// The SubmitWork method will be overwritten by a visited Component that "want's" to be rendered.
        /// Therefore a RenderContext is needed.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        public override void SubmitWork(RenderContext renderContext)
        {
            renderContext.Model = _matrix;
        }
        #endregion
    }
}
