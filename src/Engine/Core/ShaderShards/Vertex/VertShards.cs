using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Vertex
{
    /// <summary>
    /// Contains pre-defined Shader Shards = 
    /// content of the Vertex Shader's method that lets us change values of the "out"-struct that is used for the position calculation. 
    /// </summary>
    public sealed class VertShards
    {
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static VertShards() { }

        private VertShards() { }

        public static VertShards Instance => _instance;
        private static readonly VertShards _instance = new();

        /// <summary>
        /// Standard shard for storing position and normal information.
        /// </summary>
        public readonly List<string> SufOutBody_PosNorm = new()
        {
            "OUT.position = vec4(fuVertex, 1.0);",
            "OUT.normal = fuNormal;",
        };

        /// <summary>
        /// Standard shard for storing position information.
        /// </summary>
        public readonly List<string> SufOutBody_Pos = new()
        {
            "OUT.position = vec4(fuVertex, 1.0);"
        };
    }
}