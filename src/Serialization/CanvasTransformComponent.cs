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

        /// <summary>
        /// Absolute offset equals the size of the Canvas, because its the first element in the interface hierarchy .
        /// </summary>
        [ProtoMember(1)]
        public MinMaxRect Size;
    }
}
