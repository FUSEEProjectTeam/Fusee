using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Building block to set up User Interface hierarchies.
    /// </summary>
    [ProtoContract]
    public class FusRectTransform : FusComponent
    {
        /// <summary>
        /// Per-cent setting where to place the anchor point in respect to 
        /// the area defined by the parent node.
        /// </summary>
        [ProtoMember(1)]
        public MinMaxRect Anchors;
        /// <summary>
        /// Absolute offset values added to the anchor points.
        /// </summary>
        [ProtoMember(2)]
        public MinMaxRect Offsets;
    }
}