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
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuVertex, 1.0) ) * fuBoneWeight.x ;");
                vertMainBody.Add(
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuNormal, 0.0)) * fuBoneWeight.x;");
                vertMainBody.Add(
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuVertex, 1.0)) * fuBoneWeight.y + newVertex;");
                vertMainBody.Add(
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuNormal, 0.0)) * fuBoneWeight.y + newNormal;");
                vertMainBody.Add(
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuVertex, 1.0)) * fuBoneWeight.z + newVertex;");

                vertMainBody.Add(
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuNormal, 0.0)) * fuBoneWeight.z + newNormal;");
                vertMainBody.Add(
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuVertex, 1.0)) * fuBoneWeight.w + newVertex;");
                vertMainBody.Add(
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuNormal, 0.0)) * fuBoneWeight.w + newNormal;");

                // At this point the normal is in World space - transform back to model space                
                vertMainBody.Add("vNormal = mat3(FUSEE_ITMV) * newNormal.xyz;");
            }

            if (effectProps.MatProbs.HasSpecular)
            {
                vertMainBody.Add("vec3 vCamPos = FUSEE_IMV[3].xyz;");

                vertMainBody.Add(effectProps.MeshProbs.HasWeightMap
                    ? "vViewDir = normalize(vCamPos - vec3(newVertex));"
                    : "vViewDir = normalize(vCamPos - fuVertex);");
            }

            if (effectProps.MeshProbs.HasUVs)
                vertMainBody.Add("vUV = fuUV;");

            if (effectProps.MeshProbs.HasNormals && !effectProps.MeshProbs.HasWeightMap)
                vertMainBody.Add("vNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);");

            vertMainBody.Add("vPos = (FUSEE_MV * vec4(fuVertex, 1.0));");

            if (effectProps.MeshProbs.HasTangents && effectProps.MeshProbs.HasBiTangents)
            {
                vertMainBody.Add($"vT = {UniformNameDeclarations.TangentAttribName};");
                vertMainBody.Add($"vB = {UniformNameDeclarations.BitangentAttribName};");
            }

            vertMainBody.Add(effectProps.MeshProbs.HasWeightMap
            ? "gl_Position = FUSEE_MVP * vec4(vec3(newVertex), 1.0);"
            : "gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);");

            return ShaderShardUtil.MainMethod(vertMainBody);
        }
    }
}
