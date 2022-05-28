using Fusee.Math.Core;
using ProtoBuf;
using System.Collections.Generic;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Contains animation data. Generally, a list of animation tracks
    /// </summary>
    [ProtoContract]
    public class FusBoneTransform : FusComponent
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
        public float4 Rotation;
        #endregion
    }
}