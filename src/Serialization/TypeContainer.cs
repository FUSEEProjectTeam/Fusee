using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Base class to hold all sorts of data type. Derived types specify the type of the value.
    /// </summary>
    [ProtoContract]
    [ProtoInclude(100, typeof(TypeContainerDouble))]
    [ProtoInclude(101, typeof(TypeContainerInt))]
    [ProtoInclude(102, typeof(TypeContainerFloat))]
    [ProtoInclude(103, typeof(TypeContainerFloat2))]
    [ProtoInclude(104, typeof(TypeContainerFloat3))]
    [ProtoInclude(105, typeof(TypeContainerFloat4))]
    public class TypeContainer
    {
        /// <summary>
        /// The Name
        /// </summary>
        [ProtoMember(1)]
        public string Name;
    }

    /// <summary>
    /// TypeContainer storing double values.
    /// </summary>
    [ProtoContract]
    public class TypeContainerDouble : TypeContainer
    {
        /// <summary>
        /// The key value combined with a <see cref="TypeContainer.Name"/>.
        /// </summary>
        [ProtoMember(1)]
        public double Value;
    }

    /// <summary>
    /// TypeContainer storing int values.
    /// </summary>
    [ProtoContract]
    public class TypeContainerInt : TypeContainer
    {
        /// <summary>
        /// The key value combined with a <see cref="TypeContainer.Name"/>.
        /// </summary>
        [ProtoMember(1)]
        public int Value;
    }

    /// <summary>
    /// TypeContainer storing float values.
    /// </summary>
    [ProtoContract]
    public class TypeContainerFloat : TypeContainer
    {
        /// <summary>
        /// The key value combined with a <see cref="TypeContainer.Name"/>.
        /// </summary>
        [ProtoMember(1)]
        public float Value;
    }

    /// <summary>
    /// TypeContainer storing float2 values.
    /// </summary>
    [ProtoContract]
    public class TypeContainerFloat2 : TypeContainer
    {
        /// <summary>
        /// The key value combined with a <see cref="TypeContainer.Name"/>.
        /// </summary>
        [ProtoMember(1)]
        public float2 Value;
    }

    /// <summary>
    /// TypeContainer storing float3 values.
    /// </summary>
    [ProtoContract]
    public class TypeContainerFloat3 : TypeContainer
    {
        /// <summary>
        /// The key value combined with a <see cref="TypeContainer.Name"/>.
        /// </summary>
        [ProtoMember(1)]
        public float3 Value;
    }

    /// <summary>
    /// TypeContainer storing float4 values.
    /// </summary>
    [ProtoContract]
    public class TypeContainerFloat4 : TypeContainer
    {
        /// <summary>
        /// The key value combined with a <see cref="TypeContainer.Name"/>.
        /// </summary>
        [ProtoMember(1)]
        public float4 Value;
    }
}