using System.Collections.Generic;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Contains animation data. Generally, a list of animation tracks
    /// </summary>
    [ProtoContract]
    public class AnimationComponent : SceneComponentContainer
    {
        /// <summary>
        /// The animation tracks making up this animation data. 
        /// Each animation track controls a single value.
        /// </summary>
        [ProtoMember(3, AsReference = true)]
        public List<AnimationTrackContainer> AnimationTracks;

    }
}
