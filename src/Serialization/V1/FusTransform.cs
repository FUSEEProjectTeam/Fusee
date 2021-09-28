using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Transformation (position, orientation and size) of the node.
    /// </summary>
    [ProtoContract]
    public class FusTransform : FusComponent
    {
        #region Payload
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
        /// Dummy
        /// </summary>
        [ProtoMember(3)]
        public float Dummy;

        /// <summary>
        /// The scale (size) of the node.
        /// </summary>
        [ProtoMember(4)]
        public float3 Scale = float3.One;
        #endregion
    }
}