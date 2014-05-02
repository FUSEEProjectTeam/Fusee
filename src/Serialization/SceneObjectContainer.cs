using System.Collections.Generic;
using Fusee.Math;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class SceneObjectContainer
    {
        [ProtoMember(1)]
        public string Name;

        [ProtoMember(2)]
        public TransformContainer Transform;

        [ProtoMember(3, AsReference = true)] 
        public MaterialContainer Material;

        [ProtoMember(4, AsReference = true)]
        public MeshContainer Mesh;

        [ProtoMember(5, AsReference = true)]
        public List<SceneObjectContainer> Children;
    }
}
