using Fusee.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core.ShaderShards.Vertex
{
    /// <summary>
    /// Collection of Shader Shards, describing possible in, out and uniform properties of a vertex shader.
    /// </summary>
    public static class VertPropertiesShard
    {
        /// <summary>
        /// Creates the in (with prefix "fu") and out parameters of the vertex shader, depending on the given ShaderEffectProps.
        /// </summary>
        /// <param name="effectProps">>The ShaderEffectProps.</param>
        /// <returns></returns>
        public static string InAndOutParams(ShaderEffectProps effectProps)
        {
            var vertProps = new List<string>
            {
                GLSL.CreateOut(GLSL.Type.Vec3, "vCamPos"),
                GLSL.CreateOut(GLSL.Type.Vec4, "vPos")
            };
           
            vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec3, "fuVertex"));

            if (effectProps.MeshProbs.HasTangents && effectProps.MeshProbs.HasBiTangents)
            {
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.TangentAttribName));
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec3, UniformNameDeclarations.BitangentAttribName));

                vertProps.Add(GLSL.CreateOut(GLSL.Type.Vec4, "vT"));
                vertProps.Add(GLSL.CreateOut(GLSL.Type.Vec3, "vB"));
            }

            if (effectProps.MatProbs.HasSpecular)
                vertProps.Add(GLSL.CreateOut(GLSL.Type.Vec3, "vViewDir"));

            if (effectProps.MeshProbs.HasWeightMap)
            {
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec4, "fuBoneIndex"));
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec4, "fuBoneWeight"));
            }

            if (effectProps.MeshProbs.HasNormals)
            {
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec3, "fuNormal"));
                vertProps.Add(GLSL.CreateOut(GLSL.Type.Vec3, "vNormal"));
            }

            if (effectProps.MeshProbs.HasUVs)
            {
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec2, "fuUV"));
                vertProps.Add(GLSL.CreateOut(GLSL.Type.Vec2, "vUV"));
            }

            if (effectProps.MeshProbs.HasColors)
            {
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec4, "fuColor"));
                vertProps.Add(GLSL.CreateOut(GLSL.Type.Vec4, "vColor"));
            }

            return string.Join("\n", vertProps);
        }
        
        /// <summary>
        /// Returns the pre defined Fusee uniform parameters of a vertex shader, depending on the given ShaderEffectProps.
        /// </summary>
        /// <param name="effectProps">The ShaderEffectProps.</param>
        /// <returns></returns>
        public static string FuseeUniforms(ShaderEffectProps effectProps)
        {
            var uniforms = new List<string> 
            {
                GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_MV"),
                GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_MVP")
            };

            if (effectProps.MeshProbs.HasNormals)
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_ITMV"));

            if (effectProps.MatProbs.HasSpecular && !effectProps.MeshProbs.HasWeightMap)
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_IMV"));

            if (effectProps.MeshProbs.HasWeightMap)
            {
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_V"));
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_P"));
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_IMV"));
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_BONES[BONES]"));
            }

            return string.Join("\n", uniforms);
        }
    }
}
