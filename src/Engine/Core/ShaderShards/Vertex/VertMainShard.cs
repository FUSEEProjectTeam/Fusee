using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Vertex
{
    /// <summary>
    /// Collection of Shader Shards, describing the Main method of a vertex shader.
    /// </summary>
    public static class VertMainShard
    {
        /// <summary>
        /// Creates the main method for the vertex shader, used in forward rendering.
        /// The naming of the out parameters is the same as in the <see cref="VertPropertiesShard"/>.
        /// </summary>
        /// <param name="effectProps">The <see cref="ShaderEffectProps"/> is the basis to decide, which calculations need to be made. E.g. do we have a weight map? If so, adjust the gl_Position.</param>
        /// <returns></returns>
        public static string VertexMain(ShaderEffectProps effectProps)
        {
            var vertMainBody = new List<string>
            {
                "gl_PointSize = 10.0;"
            };

            if (effectProps.MeshProbs.HasNormals && effectProps.MeshProbs.HasWeightMap)
            {
                vertMainBody.Add("vec4 newVertex;");
                vertMainBody.Add("vec4 newNormal;");
                vertMainBody.Add(
                    $"newVertex = (FUSEE_BONES[int({UniformNameDeclarations.BoneIndex}.x)] * vec4({UniformNameDeclarations.Vertex}, 1.0) ) * {UniformNameDeclarations.BoneWeight}.x ;");
                vertMainBody.Add(
                    $"newNormal = (FUSEE_BONES[int({UniformNameDeclarations.BoneIndex}.x)] * vec4({UniformNameDeclarations.Normal}, 0.0)) * {UniformNameDeclarations.BoneWeight}.x;");
                vertMainBody.Add(
                    $"newVertex = (FUSEE_BONES[int({UniformNameDeclarations.BoneIndex}.y)] * vec4({UniformNameDeclarations.Vertex}, 1.0)) * {UniformNameDeclarations.BoneWeight}.y + newVertex;");
                vertMainBody.Add(
                    $"newNormal = (FUSEE_BONES[int({UniformNameDeclarations.BoneIndex}.y)] * vec4({UniformNameDeclarations.Normal}, 0.0)) * {UniformNameDeclarations.BoneWeight}.y + newNormal;");
                vertMainBody.Add(
                    $"newVertex = (FUSEE_BONES[int({UniformNameDeclarations.BoneIndex}.z)] * vec4({UniformNameDeclarations.Vertex}, 1.0)) * {UniformNameDeclarations.BoneWeight}.z + newVertex;");

                vertMainBody.Add(
                    $"newNormal = (FUSEE_BONES[int({UniformNameDeclarations.BoneIndex}.z)] * vec4({UniformNameDeclarations.Normal}, 0.0)) * {UniformNameDeclarations.BoneWeight}.z + newNormal;");
                vertMainBody.Add(
                    $"newVertex = (FUSEE_BONES[int({UniformNameDeclarations.BoneIndex}.w)] * vec4({UniformNameDeclarations.Vertex}, 1.0)) * {UniformNameDeclarations.BoneWeight}.w + newVertex;");
                vertMainBody.Add(
                    $"newNormal = (FUSEE_BONES[int({UniformNameDeclarations.BoneIndex}.w)] * vec4({UniformNameDeclarations.Normal}, 0.0)) * {UniformNameDeclarations.BoneWeight}.w + newNormal;");

                // At this point the normal is in World space - transform back to model space                
                vertMainBody.Add($"{VaryingNameDeclarations.Normal} = mat3(FUSEE_ITMV) * newNormal.xyz;");
            }

            if (effectProps.MatProbs.HasSpecular)
            {
                vertMainBody.Add("vec3 vCamPos = FUSEE_IMV[3].xyz;");

                vertMainBody.Add(effectProps.MeshProbs.HasWeightMap
                    ? $"{VaryingNameDeclarations.ViewDirection} = normalize({VaryingNameDeclarations.CameraPosition} - vec3(newVertex));"
                    : $"{VaryingNameDeclarations.ViewDirection} = normalize({VaryingNameDeclarations.CameraPosition} - {UniformNameDeclarations.Vertex});");
            }

            if (effectProps.MeshProbs.HasUVs)
                vertMainBody.Add($"{VaryingNameDeclarations.TextureCoordinates} = fuUV;");

            if (effectProps.MeshProbs.HasNormals && !effectProps.MeshProbs.HasWeightMap)
                vertMainBody.Add($"{VaryingNameDeclarations.Normal} = normalize(mat3(FUSEE_ITMV) * {UniformNameDeclarations.Normal});");

            vertMainBody.Add($"{VaryingNameDeclarations.Position} = (FUSEE_MV * vec4({UniformNameDeclarations.Vertex}, 1.0));");

            if (effectProps.MeshProbs.HasTangents && effectProps.MeshProbs.HasBiTangents)
            {
                vertMainBody.Add($"{VaryingNameDeclarations.Tangent} = {UniformNameDeclarations.TangentAttribName};");
                vertMainBody.Add($"{VaryingNameDeclarations.Bitangent} = {UniformNameDeclarations.BitangentAttribName};");
            }

            vertMainBody.Add(effectProps.MeshProbs.HasWeightMap
            ? "gl_Position = FUSEE_MVP * vec4(vec3(newVertex), 1.0);"
            : $"gl_Position = FUSEE_MVP * vec4({UniformNameDeclarations.Vertex}, 1.0);");

            return ShaderShardUtil.MainMethod(vertMainBody);
        }
    }
}
