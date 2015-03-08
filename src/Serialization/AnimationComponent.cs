using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
