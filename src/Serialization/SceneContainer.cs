using System.Collections.Generic;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public struct SceneHeader
    {
        [ProtoMember(1)] 
        public int Version;

        [ProtoMember(2)] 
        public string Generator;

        [ProtoMember(3)] 
        public string CreatedBy;

        [ProtoMember(4)]
        public string CreationDate; 
    }

    [ProtoContract]
    public class SceneContainer
    {
        [ProtoMember(1)]
        public SceneHeader Header;
        
        [ProtoMember(2, AsReference = true)]
        public List<SceneObjectContainer> Children;
    }
}
