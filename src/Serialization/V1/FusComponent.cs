using Fusee.Xene;
using ProtoBuf;
using System;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Base class for components. Each node (<see cref="FusNode"/>) contains a list of components of various types.
    /// </summary>
    [ProtoContract]

    [ProtoInclude(100, typeof(FusTransform))]
    [ProtoInclude(101, typeof(FusMesh))]
    [ProtoInclude(102, typeof(FusMaterialBase))]
    [ProtoInclude(103, typeof(FusLight))]
    [ProtoInclude(104, typeof(FusWeight))]
    [ProtoInclude(105, typeof(FusAnimation))]
    [ProtoInclude(106, typeof(FusBone))]
    [ProtoInclude(107, typeof(FusCamera))]
    public class FusComponent : IComponent
    {
        /// <summary>
        /// The name of this component.
        /// </summary>
        [ProtoMember(1)]
        public string Name;

        /// <summary>
        /// Determines if this Component contributes to the render result.
        /// </summary>
        [ProtoMember(2)]
        public bool Active { get; set; } = true;
    }

}