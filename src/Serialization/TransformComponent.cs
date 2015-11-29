using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Transformation (position, orientation and size) of the node.
    /// </summary>
    [ProtoContract]
    public class TransformComponent : SceneComponentContainer
    {
        /// <summary>
        /// The translation (position) of the node.
        /// </summary>
        [ProtoMember(1)]
        public float3 Translation;
        /// <summary>
        /// The rotation (orientation) of the node.
        /// </summary>
        [ProtoMember(2)]
        public float3 Rotation;
        /// <summary>
        /// The scale (size) of the node.
        /// </summary>
        [ProtoMember(3)]
        public float3 Scale;
    }
}
