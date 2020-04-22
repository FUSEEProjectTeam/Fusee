using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Vertex
{
    /// <summary>
    /// Collection of shader code strings, describing possible in, out and uniform properties of a vertex shader.
    /// </summary>
    public static class VertProperties
    {
        //TODO: don't add all of them....
        /// <summary>
        /// Creates the in (with prefix "fu") and out parameters of the vertex shader, depending on the given ShaderEffectProps.
        /// </summary>
        /// <returns></returns>
        public static string InParams()
        {
            var vertProps = new List<string>
            {
                GLSL.CreateIn(GLSL.Type.Vec3, UniformNameDeclarations.Vertex),
                GLSL.CreateIn(GLSL.Type.Vec3, UniformNameDeclarations.Normal),
                GLSL.CreateIn(GLSL.Type.Vec2, UniformNameDeclarations.TextureCoordinates),

                GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.VertexColor),

                GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.Tangent),
                GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.Bitangent),

                GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.BoneIndex),
                GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.BoneWeight)

            };

            return string.Join("\n", vertProps);
        }
    }
}
