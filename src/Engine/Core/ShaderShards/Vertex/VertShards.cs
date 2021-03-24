﻿using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Vertex
{
    /// <summary>
    /// Contains pre-defined Shader Shards = 
    /// content of the Vertex Shader's method that lets us change values of the "out"-struct that is used for the position calculation. 
    /// </summary>
    public static class VertShards
    {
        /// <summary>
        /// Standard shard for storing position and normal information.
        /// </summary>
        public static List<string> SufOutBody_PosNorm = new List<string>()
        {
            "OUT.position = vec4(fuVertex, 1.0);",
            "OUT.normal = fuNormal;",
        };

        /// <summary>
        /// Additional shard for storing second position and normal information.
        /// </summary>
        public static List<string> SufOutBody_PosNormAnimation = new List<string>()
        {
            "OUT.position = vec4(fuVertex * PercentPerVertex + fuVertex1 * PercentPerVertex1, 1.0);",
            "OUT.normal = fuNormal * PercentPerVertex + fuNormal1 * PercentPerVertex1;",
        };

        /// <summary>
        /// Standard shard for storing position information.
        /// </summary>
        public static List<string> SufOutBody_Pos = new List<string>()
        {
            "OUT.position = vec4(fuVertex, 1.0);"
        };

        /// <summary>
        /// Additional shard for storing second position information.
        /// </summary>
        public static List<string> SufOutBody_PosAnimation = new List<string>()
        {
            "OUT.position = vec4(fuVertex * PercentPerVertex + fuVertex1 * PercentPerVertex1, 1.0);"
        };
    }
}