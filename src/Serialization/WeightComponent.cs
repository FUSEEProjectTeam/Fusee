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
        public List<JointWeightWrapper> Weights;

        [ProtoMember(2, AsReference = true)]
        public List<SceneNodeContainer> Joints;

   
    }

    [ProtoContract]
    public class JointWeightWrapper
    {
        [ProtoMember(1)]
        public List<double> JointWeights { get; set; }
    }
}
