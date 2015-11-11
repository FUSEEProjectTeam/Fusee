using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class WeightComponent : SceneComponentContainer
    {
        // Contains as many entries as the object containing this component has vertices
        [ProtoMember(1)]
        public List<VertexWeightList> WeightMap;

        [ProtoMember(2, AsReference = true)]
        public List<SceneNodeContainer> Joints;

        [ProtoMember(3)]
        public List<float4x4> BindingMatrices;
   
    }

    [ProtoContract]
    public struct VertexWeight
    {
        [ProtoMember(1)]
        public int JointIndex;

        [ProtoMember(2)]
        public float Weight;
    }

    [ProtoContract]
    public class VertexWeightList
    {
        [ProtoMember(1)]
        public List<VertexWeight> VertexWeights { get; set; }
    }


    // Deprecated/unused
    [ProtoContract]
    public class JointWeightColumn
    {
        [ProtoMember(1)]
        public List<double> JointWeights { get; set; }
    }


}
