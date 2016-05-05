using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// This class holds the shader definition. If contained within a node, the node's (and potentially child node's)
    /// geometry is rendered with the specified shader.
    /// </summary>
    [ProtoContract]
    public class ShaderComponent : SceneComponentContainer
    {
        /// <summary>
        /// VS
        /// </summary>
        [ProtoMember(1)]
        public string VS;

        /// <summary>
        /// PS
        /// </summary>
        [ProtoMember(2)]
        public string PS;

        /// <summary>
        /// The RenderStates
        /// </summary>
        [ProtoMember(3)]
        public Dictionary<uint, uint> RenderStateContainer;

        /// <summary>
        /// The EffectParameters
        /// </summary>
        [ProtoMember(3, AsReference = true)]
        public List<TypeContainer> EffectParameter;
    }
}