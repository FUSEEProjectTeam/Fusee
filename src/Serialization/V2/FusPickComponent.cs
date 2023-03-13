using ProtoBuf;

namespace Fusee.Serialization.V2
{
    /// <summary>
    /// If <see cref="V1.FusComponent.Active"/> is <see langword="false"/>, the picking is being skiped for this and all successors.
    /// </summary>
    [ProtoContract]
    public class FusPickComponent : V1.FusComponent
    {
        /// <summary>
        /// If there is more than one PickComponent in one scene, the rendered output of the picker with a higher layer will be picked first above the output of a pick result with a lower layer.
        /// </summary>
        [ProtoMember(1)]
        public int PickLayer;
    }
}