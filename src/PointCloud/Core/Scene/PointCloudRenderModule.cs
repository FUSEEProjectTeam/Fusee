using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Xene;
using System;

namespace Fusee.PointCloud.Core.Scene
{
    /// <summary>
    /// Allows to add the visit/render method for the <see cref="PointCloudComponent"/> to a <see cref="SceneRendererForward"/> or <see cref="SceneRendererDeferred"/>.
    /// </summary>
    public class PointCloudRenderModule : IRendererModule
    {
        RenderContext _rc;

        /// <summary>
        /// Holds the status of the model matrices and other information we need while traversing up and down the scene graph.
        /// </summary>
        private RendererState _state;

        /// <summary>
        /// The RenderLayer this renderer should render.
        /// </summary>
        public RenderLayers RenderLayer { get; set; }

        /// <summary>
        /// Sets the render context for the given scene.
        /// </summary>
        /// <param name="rc"></param>
        public void SetContext(RenderContext rc)
        {
            if (rc == null)
                throw new ArgumentNullException(nameof(rc));

            if (rc != _rc)
            {
                _rc = rc;
            }
        }

        /// <summary>
        /// Sets the render context for the given scene.
        /// </summary>
        /// <param name="state"></param>
        public void SetState(RendererState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            if (state != _state)
            {
                _state = state;
            }
        }

        /// <summary>
        /// Determines visible points of a point cloud (using the components <see cref="VisibilityTester"/>) and renders them.
        /// </summary>
        /// <param name="pointCloud">The point cloud component.</param>
        [VisitMethod]
        public void RenderPointCloud(PointCloudComponent pointCloud)
        {
            if (!pointCloud.Active) return;
            if (!RenderLayer.HasFlag(_state.RenderLayer.Layer) && !_state.RenderLayer.Layer.HasFlag(RenderLayer) || _state.RenderLayer.Layer.HasFlag(RenderLayers.None))
                return;
            //if (_rc.InvView == float4x4.Identity) return;

            var fov = (float)_rc.ViewportWidth / _rc.ViewportHeight;
            pointCloud.PointCloudImp.Update(fov, _rc.ViewportHeight, _rc.RenderFrustum, _rc.InvView.Column4.xyz);

            foreach (var mesh in pointCloud.PointCloudImp.MeshesToRender)
            {
                var renderStatesBefore = _rc.CurrentRenderState.Copy();
                _rc.Render(mesh, true);
                _state.RenderUndoStates = renderStatesBefore.Merge(_rc.CurrentRenderState);
            }
        }
    }
}