using System;

namespace Fusee.Engine.Core.Effects
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class FxShaderAttribute : Attribute
    {
        public readonly ShaderCategory ShaderCategory;

        public FxShaderAttribute(ShaderCategory category)
        {
            ShaderCategory = category;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class FxShardAttribute : Attribute
    {
        public readonly ShardCategory ShardCategory;

        public FxShardAttribute(ShardCategory category)
        {
            ShardCategory = category;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class UniformAttribute : Attribute
    {
        public readonly string ShaderParamName;

        public UniformAttribute(string shaderParamName)
        {
            ShaderParamName = shaderParamName;
        }
    }

    /// <summary>
    /// Used to flag which shader type a <see cref="ShaderEffect"/> property belongs to. 
    /// Supports vertex, pixel and geometry shaders.
    /// </summary>
    [Flags]
    public enum ShaderCategory : ushort
    {
        /// <summary>
        /// The parameter is only used in the vertex shader.
        /// </summary>
        Vertex = 1,

        /// <summary>
        /// The parameter is only used in the pixel shader.
        /// </summary>
        Fragment = 2,

        /// <summary>
        /// The parameter is only used in the geometry shader.
        /// </summary>
        Geometry = 4
    }

    /// <summary>
    /// Used to describe the type of shader shard. A shard is a piece of shader code, usually in form of a static string.
    /// The category is used to sort the shards into the correct order in order to build the complete shader source code from them.
    /// The order is given by the values of the enumeration.
    /// </summary>
    [Flags]
    public enum ShardCategory : ushort
    {
        /// <summary>
        /// The shader shard belongs to the header of the source shader.
        /// </summary>
        Header = 1,

        /// <summary>
        /// The shader shard belongs is a struct of the source shader.
        /// </summary>
        Struct = 2,

        /// <summary>
        /// The shader shard belongs is a property of the source shader.
        /// </summary>
        Uniform = 4,

        /// <summary>
        /// The shader shard belongs is a property of the source shader.
        /// </summary>
        Property = 8,

        /// <summary>
        /// The shader shard is a method of the source shader.
        /// </summary>
        Method = 16,

        /// <summary>
        /// The shader shard is, or is part of, the main method of the source shader.
        /// </summary>
        Main = 31
    }
}
