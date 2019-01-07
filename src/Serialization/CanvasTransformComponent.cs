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
        /// Scale factor for the user given offsets that define the sizes if the canvas' child elements. This becomes important when rendering in SCREEN mode.
        /// By default Scale in SCREEN mode is set to 0.1.
        /// </summary>
        [ProtoMember(3)] public float Scale;

        public CanvasTransformComponent(CanvasRenderMode canvasRenderMode, float scale = 0.1f)
        {
            CanvasRenderMode = canvasRenderMode;
            Scale = CanvasRenderMode == CanvasRenderMode.SCREEN ? scale : 1;
        }
    }

    [ProtoContract]
    public enum CanvasRenderMode
    {
        WORLD,
        SCREEN
    }
}
