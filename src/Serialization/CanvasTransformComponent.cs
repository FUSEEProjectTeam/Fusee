using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Building block to set up User Interface hierarchies.
    /// Use this as your first element in the Interface hierarchy.
    /// </summary>
    [ProtoContract]
    public class CanvasTransformComponent : SceneComponentContainer
    {
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

        public CanvasTransformComponent(CanvasRenderMode canvasRenderMode)
        {
            CanvasRenderMode = canvasRenderMode;
            Size = ScreenSpaceSize;
        }
    }

    [ProtoContract]
    public enum CanvasRenderMode
    {
        WORLD,
        SCREEN
    }
}
