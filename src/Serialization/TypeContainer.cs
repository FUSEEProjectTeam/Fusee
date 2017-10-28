using System;
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
    [ProtoInclude(106, typeof(TypeContainerBool))]
    public class TypeContainer
    {
        /// <summary>
        /// The Name
        /// </summary>
        [ProtoMember(1)]
        public string Name;

        /// <summary>
        /// The type of the key-values stored in this ShaderComponent.
        /// </summary>
        [ProtoMember(2)]
        public TypeId TypeId;
    }

    /// <summary>
    /// TypeContainer storing a double value.
    /// </summary>
    [ProtoContract]
    public class TypeContainerDouble : TypeContainer
    {
        /// <summary>
        /// The key value combined with a <see cref="TypeContainer.Name"/>.
        /// </summary>
        [ProtoMember(1)]
        public double Value;

        /// <summary>
        /// Standard constructor. Initializes the TypeId fiels appropriately.
        /// </summary>
        public TypeContainerDouble() { TypeId = TypeId.Double; }
    }

    /// <summary>
    /// TypeContainer storing a int value.
    /// </summary>
    [ProtoContract]
    public class TypeContainerInt : TypeContainer
    {
        /// <summary>
        /// The key value combined with a <see cref="TypeContainer.Name"/>.
        /// </summary>
        [ProtoMember(1)]
        public int Value;

        /// <summary>
        /// Standard constructor. Initializes the TypeId fiels appropriately.
        /// </summary>
        public TypeContainerInt() { TypeId = TypeId.Int; }
    }

    /// <summary>
    /// TypeContainer storing a float value.
    /// </summary>
    [ProtoContract]
    public class TypeContainerFloat : TypeContainer
    {
        /// <summary>
        /// The key value combined with a <see cref="TypeContainer.Name"/>.
        /// </summary>
        [ProtoMember(1)]
        public float Value;

        /// <summary>
        /// Standard constructor. Initializes the TypeId fiels appropriately.
        /// </summary>
        public TypeContainerFloat() { TypeId = TypeId.Float; }
    }

    /// <summary>
    /// TypeContainer storing a float2 value.
    /// </summary>
    [ProtoContract]
    public class TypeContainerFloat2 : TypeContainer
    {
        /// <summary>
        /// The key value combined with a <see cref="TypeContainer.Name"/>.
        /// </summary>
        [ProtoMember(1)]
        public float2 Value;

        /// <summary>
        /// Standard constructor. Initializes the TypeId fiels appropriately.
        /// </summary>
        public TypeContainerFloat2() { TypeId = TypeId.Float2; }
    }

    /// <summary>
    /// TypeContainer storing a float3 value.
    /// </summary>
    [ProtoContract]
    public class TypeContainerFloat3 : TypeContainer
    {
        /// <summary>
        /// The key value combined with a <see cref="TypeContainer.Name"/>.
        /// </summary>
        [ProtoMember(1)]
        public float3 Value;

        /// <summary>
        /// Standard constructor. Initializes the TypeId fiels appropriately.
        /// </summary>
        public TypeContainerFloat3() { TypeId = TypeId.Float3; }
    }

    /// <summary>
    /// TypeContainer storing a float4 value.
    /// </summary>
    [ProtoContract]
    public class TypeContainerFloat4 : TypeContainer
    {
        /// <summary>
        /// The key value combined with a <see cref="TypeContainer.Name"/>.
        /// </summary>
        [ProtoMember(1)]
        public float4 Value;

        /// <summary>
        /// Standard constructor. Initializes the TypeId fiels appropriately.
        /// </summary>
        public TypeContainerFloat4() { TypeId = TypeId.Float4; }
    }

    /// <summary>
    /// TypeContainer storing a bool value.
    /// </summary>
    [ProtoContract]
    public class TypeContainerBool : TypeContainer
    {
        /// <summary>
        /// The key value combined with a <see cref="TypeContainer.Name"/>.
        /// </summary>
        [ProtoMember(1)]
        public bool Value;

        /// <summary>
        /// Standard constructor. Initializes the TypeId fiels appropriately.
        /// </summary>
        public TypeContainerBool() { TypeId = TypeId.Bool; }
    }
}