using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Building block to set up User Interface hierarchies.
    /// Use this as your first element in the Interface hierarchy.
    /// </summary>
    [ProtoContract]
    public class FusCanvasTransform : FusComponent
    {
        /// <summary>
        /// The canvas render mode.  Is the UI on this canvas placed in the 3D world or overlaid onto the 2D screen. 
        /// </summary>
        [ProtoMember(1)]
        public CanvasRenderMode CanvasRenderMode;

        /// <summary>
        /// Absolute size of the Canvas. First element in the interface hierarchy.
        /// </summary>
        [ProtoMember(2)]
        public MinMaxRect Size;

        /// <summary>
        /// Absolute size of the Canvas. First element in the interface hierarchy.
        /// </summary>
        [ProtoMember(3)]
        public MinMaxRect ScreenSpaceSize;

        /// <summary>
        /// Scale factor for the user given offsets that define the sizes if the canvas' child elements. This becomes important when rendering in SCREEN mode.        
        /// </summary>
        [ProtoMember(4)] public float2 Scale;

        /// <summary>
        /// Initializes a new instance of the <see cref="FusCanvasTransform"/> class.
        /// </summary>
        /// <param name="canvasRenderMode">The canvas render mode. Is the UI on this canvas placed in the 3D world or overlaid onto the 2D screen.</param>
        public FusCanvasTransform(CanvasRenderMode canvasRenderMode)
        {
            CanvasRenderMode = canvasRenderMode;
            Size = ScreenSpaceSize;
        }
    }
    ///<summary>
    ///The available render modes.
    ///</summary>
    [ProtoContract]
    public enum CanvasRenderMode
    {
        /// <summary>
        /// The UI is embedded into the 3D world.
        /// </summary>
        World,
        /// <summary>
        /// The UI should be overlaid onto the 2D screen.
        /// </summary>
        Screen
    }
}
