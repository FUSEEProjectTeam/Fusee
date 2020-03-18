using Fusee.Base.Core;
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
        /// <param name="effectProps">The <see cref="EffectProps"/> is the basis to decide, which calculations need to be made. E.g. do we have a weight map? If so, adjust the gl_Position.</param>
        /// <returns></returns>
        public static string VertexMain(EffectProps effectProps)
        {
            var vertMainBody = new List<string>
            {
                "gl_PointSize = 10.0;"
            };

            if (effectProps.MatProbs.HasSpecular)
            {
                vertMainBody.Add($"vec3 vCamPos = {UniformNameDeclarations.IModelView}[3].xyz;");

                vertMainBody.Add(effectProps.MeshProbs.HasWeightMap
                    ? $"{VaryingNameDeclarations.ViewDirection} = normalize({VaryingNameDeclarations.CameraPosition} - vec3(newVertex));"
                    : $"{VaryingNameDeclarations.ViewDirection} = normalize({VaryingNameDeclarations.CameraPosition} - {UniformNameDeclarations.Vertex});");
            }

            if (effectProps.MeshProbs.HasUVs)
                vertMainBody.Add($"{VaryingNameDeclarations.TextureCoordinates} = fuUV;");

            if (effectProps.MeshProbs.HasNormals && effectProps.MeshProbs.HasWeightMap)
            {
                vertMainBody.Add("vec4 newVertex;");
                vertMainBody.Add("vec4 newNormal;");
                vertMainBody.Add(
                    $"newVertex = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.x)] * vec4({UniformNameDeclarations.Vertex}, 1.0) ) * {UniformNameDeclarations.BoneWeight}.x ;");
                vertMainBody.Add(
                    $"newNormal = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.x)] * vec4({UniformNameDeclarations.Normal}, 0.0)) * {UniformNameDeclarations.BoneWeight}.x;");
                vertMainBody.Add(
                    $"newVertex = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.y)] * vec4({UniformNameDeclarations.Vertex}, 1.0)) * {UniformNameDeclarations.BoneWeight}.y + newVertex;");
                vertMainBody.Add(
                    $"newNormal = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.y)] * vec4({UniformNameDeclarations.Normal}, 0.0)) * {UniformNameDeclarations.BoneWeight}.y + newNormal;");
                vertMainBody.Add(
                    $"newVertex = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.z)] * vec4({UniformNameDeclarations.Vertex}, 1.0)) * {UniformNameDeclarations.BoneWeight}.z + newVertex;");

                vertMainBody.Add(
                    $"newNormal = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.z)] * vec4({UniformNameDeclarations.Normal}, 0.0)) * {UniformNameDeclarations.BoneWeight}.z + newNormal;");
                vertMainBody.Add(
                    $"newVertex = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.w)] * vec4({UniformNameDeclarations.Vertex}, 1.0)) * {UniformNameDeclarations.BoneWeight}.w + newVertex;");
                vertMainBody.Add(
                    $"newNormal = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.w)] * vec4({UniformNameDeclarations.Normal}, 0.0)) * {UniformNameDeclarations.BoneWeight}.w + newNormal;");

                // At this point the normal is in World space - transform back to model space                
                vertMainBody.Add($"{VaryingNameDeclarations.Normal} = mat3({UniformNameDeclarations.ITModelView}) * newNormal.xyz;");
            }
            else if (effectProps.MeshProbs.HasNormals && !effectProps.MeshProbs.HasWeightMap)
                vertMainBody.Add($"{VaryingNameDeclarations.Normal} = normalize(vec3({ UniformNameDeclarations.ITModelView}* vec4({ UniformNameDeclarations.Normal}, 0.0)));");

            vertMainBody.Add($"{VaryingNameDeclarations.Position} = ({UniformNameDeclarations.ModelView} * vec4({UniformNameDeclarations.Vertex}, 1.0));");

            if (effectProps.MatProbs.HasBump)
            {
                if (!effectProps.MeshProbs.HasTangents || !effectProps.MeshProbs.HasBiTangents)
                    Diagnostics.Error(effectProps, new ArgumentException("The effect props state the material has a bump map but is missing tangents and/or bitangents!"));

                vertMainBody.Add($" vec3 T = normalize(vec3({UniformNameDeclarations.ITModelView} * vec4({ UniformNameDeclarations.Tangent}.xyz, 0.0)));");
                vertMainBody.Add($"vec3 B = normalize(vec3({ UniformNameDeclarations.ITModelView} * vec4({ UniformNameDeclarations.Bitangent}, 0.0)));");
                
                vertMainBody.Add($"{VaryingNameDeclarations.Tangent} = vec4(T, {UniformNameDeclarations.Tangent}.w);");
                vertMainBody.Add($"{VaryingNameDeclarations.Bitangent} = B;");

                vertMainBody.Add($"TBN = mat3(T,B,{VaryingNameDeclarations.Normal});");
            }

            vertMainBody.Add(effectProps.MeshProbs.HasWeightMap
            ? $"gl_Position = {UniformNameDeclarations.ModelViewProjection} * vec4(vec3(newVertex), 1.0);"
            : $"gl_Position = {UniformNameDeclarations.ModelViewProjection} * vec4({UniformNameDeclarations.Vertex}, 1.0);");

            return ShaderShardUtil.MainMethod(vertMainBody);
        }
    }
}
