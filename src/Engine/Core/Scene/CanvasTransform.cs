using Fusee.Math.Core;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Building block to set up User Interface hierarchies.
    /// Use this as your first element in the Interface hierarchy.
    /// </summary>
    public class CanvasTransform : SceneComponent
    {
        /// <summary>
        /// The canvas render mode.  Is the UI on this canvas placed in the 3D world or overlaid onto the 2D screen. 
        /// </summary>
        public CanvasRenderMode CanvasRenderMode;

        /// <summary>
        /// Absolute size of the Canvas. First element in the interface hierarchy.
        /// </summary>
        public MinMaxRect Size;

        /// <summary>
        /// Absolute size of the Canvas. First element in the interface hierarchy.
        /// </summary>
        public MinMaxRect ScreenSpaceSize;

        /// <summary>
        /// Scale factor for the user given offsets that define the sizes if the canvas' child elements. This becomes important when rendering in SCREEN mode.        
        /// </summary>
        public float2 Scale;

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasTransform"/> class.
        /// </summary>
        /// <param name="canvasRenderMode">The canvas render mode. Is the UI on this canvas placed in the 3D world or overlaid onto the 2D screen.</param>
        public CanvasTransform(CanvasRenderMode canvasRenderMode)
        {
            CanvasRenderMode = canvasRenderMode;
            Size = ScreenSpaceSize;
        }
    }

    ///<summary>
    ///The available render modes.
    ///</summary>
    public enum CanvasRenderMode
    {
        /// <summary>
        /// The UI is embedded into the 3D world.
        /// </summary>
        WORLD,
        /// <summary>
        /// The UI should be overlaid onto the 2D screen.
        /// </summary>
        SCREEN
    }
}
