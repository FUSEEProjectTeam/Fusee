using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Implement this to extend the SceneRenderer with visit methods for modules that contain <see cref="SceneComponent"/>s.
    /// </summary>
    public interface IRendererModule : IVisitorModule
    {
        /// <summary>
        /// Sets the currently used <see cref="RenderLayer"/>.
        /// </summary>
        /// <param name="renderLayer"></param>
        public void UpdateRenderLayer(RenderLayers renderLayer);

        /// <summary>
        /// Sets the currently used <see cref="Camera"/>.
        /// </summary>
        /// <param name="cam"></param>
        public void UpdateCamera(Camera cam);

        /// <summary>
        /// Sets the render context for the given scene.
        /// </summary>
        /// <param name="rc"></param>
        public void UpdateContext(RenderContext rc);

        /// <summary>
        /// Sets the <see cref="RendererState"/> for this module. Pass the state from the base renderer.
        /// </summary>
        /// <param name="state">The state to set.</param>
        public void UpdateState(RendererState state);
    }
}