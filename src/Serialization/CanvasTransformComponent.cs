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
        /// Absolute size of the Canvas. First element in the interface hierarchy .
        /// </summary>
        [ProtoMember(2)]
        public MinMaxRect Size;
    }

    [ProtoContract]
    public enum CanvasRenderMode
    {
        WORLD,
        SCREEN
    }
}
