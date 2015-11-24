using System.Collections.Generic;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class AnimationComponent : SceneComponentContainer
    {
        [ProtoMember(3, AsReference = true)]
        public List<AnimationTrackContainer> AnimationTracks;

    }
}
