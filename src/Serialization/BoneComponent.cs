using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Marker component (contains no data). If contained within a node, the node 
    /// serves as a bone in a bone animation.
    /// </summary>
    [ProtoContract]
    public class BoneComponent : SceneComponentContainer
    {
    }
}
