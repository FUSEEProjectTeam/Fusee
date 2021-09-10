using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Vertex
{
    /// <summary>
    /// Collection of shader code strings, describing possible in, out and uniform properties of a vertex shader.
    /// </summary>
    public static class VertProperties
    {
        //TODO: don't add all of them.... do we need to add some mesh property flags?
        /// <summary>
        /// Creates the in (with prefix "fu") and out parameters of the vertex shader, depending on the given ShaderEffectProps.
        /// </summary>
        /// <returns></returns>
        public static string InParams(LightingSetupFlags setup)
        {
            var vertProps = new List<string>
            {
                GLSL.CreateIn(GLSL.Type.Vec3, UniformNameDeclarations.Instance.Vertex),
                GLSL.CreateIn(GLSL.Type.Vec3, UniformNameDeclarations.Instance.Normal),
                GLSL.CreateIn(GLSL.Type.Vec2, UniformNameDeclarations.Instance.TextureCoordinates),

                GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.Instance.VertexColor),

                GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.Instance.BoneIndex),
                GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.Instance.BoneWeight)

            };

            if (setup.HasFlag(LightingSetupFlags.NormalMap))
            {
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.Instance.Tangent));
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.Instance.Bitangent));
            }

            return string.Join("\n", vertProps);
        }
    }
}