using System;

namespace Fusee.Engine.Core.Effects
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class NoUniformAttribute : Attribute
    {

    }

    /// <summary>
    /// The FxShaderAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class FxShaderAttribute : Attribute
    {
        /// <summary>
        /// Specify shader category aka vertex, fragment, etc.
        /// </summary>
        public readonly ShaderCategory ShaderCategory;

        /// <summary>
        /// Generate FxShaderAttribute with given shader category
        /// </summary>
        /// <param name="category"></param>
        public FxShaderAttribute(ShaderCategory category)
        {
            ShaderCategory = category;
        }
    }

    /// <summary>
    /// The FxShardAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class FxShardAttribute : Attribute
    {
        /// <summary>
        ///  Specify shader category aka vertex, fragment, etc.
        /// </summary>
        public readonly ShardCategory ShardCategory;

        /// <summary>
        /// Generate FxShardAttribute with given shader category
        /// </summary>
        /// <param name="category"></param>
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
        /// The parameter is only used in the geometry shader.
        /// </summary>
        Geometry = 4
    }

    /// <summary>
    /// Used to describe the type of shader shard. A shard is a piece of shader code, usually in form of a static string.
    /// The category is used to sort the shards in order to build the complete shader source code from them.
    /// Furthermore this it can be used to describe special behavior of a shader shard in the <see cref="SurfaceEffectBase"/>.
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
        /// The shader shard belongs is a struct of the source shader.
        /// </summary>
        SurfOutStruct = 4,

        /// <summary>
        /// Those are uniforms in the shader code but should not be properties of a <see cref="SurfaceEffectBase"/> because they will be updated by the SceneRenderer.
        /// </summary>
        InternalUniform = 8,

        /// <summary>
        /// The shader shard belongs is a property of the source shader.
        /// </summary>
        Uniform = 16,

        /// <summary>
        /// The shader shard belongs is a property of the source shader.
        /// </summary>
        Property = 32,

        /// <summary>
        /// The shader shard is a method of the source shader.
        /// </summary>
        Method = 64,

        /// <summary>
        /// The shader shard the surface output method of the source shader.
        /// </summary>
        SurfOut = 128,

        /// <summary>
        /// The shader shard is, or is part of, the main method of the source shader.
        /// </summary>
        Main = 256,

    }
}