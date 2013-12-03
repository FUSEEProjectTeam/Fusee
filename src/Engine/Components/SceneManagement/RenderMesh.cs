using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Engine;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// Creates a mesh that will be passed to the RenderContext.
    /// </summary>
    public class RenderMesh : RenderJob
    {
        #region Fields
        private Mesh _mesh;
        #endregion
        #region Members
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderMesh"/> class. Needs to be provided with a mesh that needs to be rendered.
        /// </summary>
        /// <param name="mesh">The mesh that need's to be rendered e.g. the mesh of a ingame object.</param>
        public RenderMesh(Mesh mesh)
        {
            _mesh = mesh;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// Overwrites the SubmitWork method of RenderJob class. The mesh will be provided with everything it need's and passed to
        /// RenderContextImplementation.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        public override void SubmitWork(RenderContext renderContext)
        {
            renderContext.Render(_mesh);
        }
        #endregion
    }
}
