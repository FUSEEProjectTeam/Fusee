using System;
using System.Linq;

namespace Fusee.Engine.Core.ShaderEffects
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class FxParamAttribute : Attribute
    {
        public ShaderCategory ShaderCategory;

        public FxParamAttribute()
        {
            // Default - used in all categories
            ShaderCategory = 0;
            foreach (var cat in Enum.GetValues(typeof(ShaderCategory)).Cast<ShaderCategory>())
                ShaderCategory |= cat;
        }
        public FxParamAttribute(ShaderCategory usedInShards)
        {
            ShaderCategory = usedInShards;
        }
    }

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
        /// The parameter is used in the pixel and in the vertex shader.
        /// </summary>
        Vertex_Pixel = 3,


        /// <summary>
        /// The parameter is only used in the geometry shader.
        /// </summary>
        Geometry = 4,


        /// <summary>
        /// The parameter is used in the vertex and the geometry shader.
        /// </summary>
        Vertex_Geometry = 5,


        /// <summary>
        /// The parameter is used in the geometry and the pixel shader.
        /// </summary>
        Geometry_Pixel = 6,


        /// <summary>
        /// The parameter is used in the vertex, geometry and pixel shader.
        /// </summary>
        Vertex_Geometry_Pixel = 7

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
        /// The shader shard belongs is a property of the source shader.
        /// </summary>
        Property = 2,

        /// <summary>
        /// The shader shard belongs is a struct of the source shader.
        /// </summary>
        Struct = 3,

        /// <summary>
        /// The shader shard is a method of the source shader.
        /// </summary>
        Method = 4,

        /// <summary>
        /// The shader shard is, or is part of, the main method of the source shader.
        /// </summary>
        Main = 5
    }
}
