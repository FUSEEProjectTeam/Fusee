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
        [ProtoMember(1)]
        public List<JointWeightColumn> WeightMap;

        [ProtoMember(2, AsReference = true)]
        public List<SceneNodeContainer> Joints;

        [ProtoMember(3)]
        public List<float4x4> BindingMatrices;
   
    }

    [ProtoContract]
    public class JointWeightColumn
    {
        [ProtoMember(1)]
        public List<double> JointWeights { get; set; }
    }
}
