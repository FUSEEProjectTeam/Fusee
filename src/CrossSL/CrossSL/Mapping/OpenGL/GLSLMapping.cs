using Fusee.Math;
using Fusee.Math.Core;

namespace CrossSL
{
    internal class GLSLMapping : ShaderMapping
    {
        /// <summary>
        ///     Initializes the <see cref="GLSLMapping" /> class.
        /// </summary>
        public GLSLMapping()
        {
            Types.Add(typeof (float2), "vec2");
            Types.Add(typeof (float3), "vec3");
            Types.Add(typeof (float4), "vec4");
            Types.Add(typeof (float3x3), "mat3");
            Types.Add(typeof (float4x4), "mat4");

            UpdateMapping();
        }
    }
}