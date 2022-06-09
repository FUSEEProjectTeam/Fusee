using Fusee.Engine.Core;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.PointCloud.Common;
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

        private readonly bool _isForwardModule;

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

        private Plane quad = new();

        /// <summary>
        /// Creates a new instance of type <see cref="PointCloudRenderModule"/>.
        /// </summary>
        /// <param name="doRenderForward">Propagated the render type (forward or deferred) to the <see cref="RenderContext"/>.</param>
        public PointCloudRenderModule(bool doRenderForward)
        {
            _isForwardModule = doRenderForward;
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


            if (!pointCloud.DoRenderInstanced)
            {
                foreach (var mesh in ((IPointCloudImp<GpuMesh>)pointCloud.PointCloudImp).GpuDataToRender)
                {
                    _rc.Render(mesh, _isForwardModule);
                }
            }
            else
            {
                foreach (var instanceData in ((IPointCloudImp<InstanceData>)pointCloud.PointCloudImp).GpuDataToRender)
                {
                    _rc.Render(quad, instanceData, _isForwardModule);
                }
            }
        }
    }
}