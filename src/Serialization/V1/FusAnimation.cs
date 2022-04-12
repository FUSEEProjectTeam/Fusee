using ProtoBuf;
using System.Collections.Generic;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Contains animation data. Generally, a list of animation tracks
    /// </summary>
    [ProtoContract]
    public class FusAnimation : FusComponent
    {
        /// <summary>
        /// The animation tracks making up this animation data. 
        /// Each animation track controls a single value.
        /// </summary>
        [ProtoMember(3)]
        public List<FusAnimationTrack> AnimationTracks = new();
    }
}