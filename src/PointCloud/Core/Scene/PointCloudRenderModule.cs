// Ignore Spelling: fov

using Fusee.Base.Core;
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
        private RenderLayers _renderLayer;

        /// <summary>
        /// The RenderLayer this renderer should render.
        /// </summary>
        private Camera _camera;

        private readonly bool _isForwardModule;

        /// <summary>
        /// Sets the render context for the given scene.
        /// </summary>
        /// <param name="rc"></param>
        public void UpdateContext(RenderContext rc)
        {
            if (rc == null)
                throw new ArgumentNullException(nameof(rc));

            _rc = rc;

            // prevent rendering with <see cref="Mesh.HasDirtyIndices"/>
            _rc.AllowDirtyMeshs = false;
        }

        /// <summary>
        /// Sets the render context for the given scene.
        /// </summary>
        /// <param name="state"></param>
        public void UpdateState(RendererState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            _state = state;
        }

        /// <summary>
        /// Sets the currently used <see cref="RenderLayer"/>.
        /// </summary>
        /// <param name="renderLayer"></param>
        public void UpdateRenderLayer(RenderLayers renderLayer)
        {
            _renderLayer = renderLayer;
        }

        /// <summary>
        /// Sets the currently used <see cref="Camera"/>.
        /// </summary>
        /// <param name="cam"></param>
        public void UpdateCamera(Camera cam)
        {
            _camera = cam;
        }

        private readonly Plane quad = new();

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
            if (!_renderLayer.HasFlag(_state.RenderLayer.Layer) && !_state.RenderLayer.Layer.HasFlag(_renderLayer) || _state.RenderLayer.Layer.HasFlag(RenderLayers.None))
                return;

            if (pointCloud.Camera == null)
            {
                Diagnostics.Warn("Point Cloud Render Camera is null!");
                return;
            }
            else if (pointCloud.Camera == _camera)
            {
                var fov = (float)_rc.ViewportWidth / _rc.ViewportHeight;
                pointCloud.PointCloudImp.Update(fov, _rc.ViewportHeight, _rc.RenderFrustum, _rc.InvView.Column4.xyz, _rc.Model);
            }

            switch (pointCloud.RenderMode)
            {
                case RenderMode.StaticMesh:
                    foreach (var mesh in ((IPointCloudImp<GpuMesh, VisualizationPoint>)pointCloud.PointCloudImp).GpuDataToRender)
                    {
                        _rc.Render(mesh, _isForwardModule);
                    }
                    break;
                case RenderMode.Instanced:
                    foreach (var instanceData in ((IPointCloudImp<InstanceData, VisualizationPoint>)pointCloud.PointCloudImp).GpuDataToRender)
                    {
                        _rc.Render(quad, instanceData, _isForwardModule);
                    }
                    break;
                case RenderMode.DynamicMesh:
                    foreach (var mesh in ((IPointCloudImp<Mesh, VisualizationPoint>)pointCloud.PointCloudImp).GpuDataToRender)
                    {
                        _rc.Render(mesh, null, _isForwardModule);
                    }
                    break;
            }
        }
    }
}