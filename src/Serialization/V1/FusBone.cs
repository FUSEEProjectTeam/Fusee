using ProtoBuf;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Marker component (contains no data). If contained within a node, the node 
    /// serves as a bone in a bone animation.
    /// </summary>
    [ProtoContract]
    public class FusBone : FusComponent
    {
        /// <summary>
        /// The name of this component.
        /// Needed for blender exporter
        /// </summary>
        [ProtoMember(1)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public string? Name;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    }
}
