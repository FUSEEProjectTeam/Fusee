using Fusee.Base.Core;
using System;
using System.Collections.Generic;

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
        public static string InAndOutParams(EffectProps effectProps)
        {
            var vertProps = new List<string>
            {
                GLSL.CreateOut(GLSL.Type.Vec3, VaryingNameDeclarations.CameraPosition),
                GLSL.CreateOut(GLSL.Type.Vec4, VaryingNameDeclarations.Position)
            };

            vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec3, UniformNameDeclarations.Vertex));

            if (effectProps.MatProbs.HasBump)
            {
                if (!effectProps.MeshProbs.HasTangents || !effectProps.MeshProbs.HasBiTangents)
                    Diagnostics.Error(effectProps, new ArgumentException("The effect props state the material has a bump map but is missing tangents and/or bitangents!"));

                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.Tangent));
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec3, UniformNameDeclarations.Bitangent));

                vertProps.Add(GLSL.CreateOut(GLSL.Type.Vec4, VaryingNameDeclarations.Tangent));
                vertProps.Add(GLSL.CreateOut(GLSL.Type.Vec3, VaryingNameDeclarations.Bitangent));

                vertProps.Add(GLSL.CreateOut(GLSL.Type.Mat3, "TBN"));

            }

            if (effectProps.MatProbs.HasSpecular)
                vertProps.Add(GLSL.CreateOut(GLSL.Type.Vec3, VaryingNameDeclarations.ViewDirection));

            if (effectProps.MeshProbs.HasWeightMap)
            {
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.BoneIndex));
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.BoneWeight));
            }

            if (effectProps.MeshProbs.HasNormals)
            {
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec3, UniformNameDeclarations.Normal));
                vertProps.Add(GLSL.CreateOut(GLSL.Type.Vec3, VaryingNameDeclarations.Normal));
            }

            if (effectProps.MeshProbs.HasUVs)
            {
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec2, UniformNameDeclarations.TextureCoordinates));
                vertProps.Add(GLSL.CreateOut(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates));
            }

            if (effectProps.MeshProbs.HasColors)
            {
                vertProps.Add(GLSL.CreateIn(GLSL.Type.Vec4, UniformNameDeclarations.Color));
                vertProps.Add(GLSL.CreateOut(GLSL.Type.Vec4, VaryingNameDeclarations.Color));
            }

            return string.Join("\n", vertProps);
        }

        /// <summary>
        /// Returns the pre defined Fusee uniform parameters of a vertex shader, depending on the given ShaderEffectProps.
        /// </summary>
        /// <param name="effectProps">The ShaderEffectProps.</param>
        /// <returns></returns>
        public static string FuseeMatUniforms(EffectProps effectProps)
        {
            var uniforms = new List<string>
            {
                GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.ModelView),
                GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.ModelViewProjection)
            };

            if (effectProps.MeshProbs.HasNormals)
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.ITModelView));

            if (effectProps.MatProbs.HasSpecular && !effectProps.MeshProbs.HasWeightMap)
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.IModelView));

            if (effectProps.MeshProbs.HasWeightMap)
            {
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.View));
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.Projection));
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.IModelView));
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.Bones + "[" + HeaderShard.BoneDefineVar + "]"));
            }

            return string.Join("\n", uniforms);
        }

        /// <summary>
        /// Returns the pre defined Fusee uniform parameters of a vertex shader, depending on the given ShaderEffectProps.
        /// </summary>       
        /// <returns></returns>
        public static string FuseeUniformsDefault()
        {
            var uniforms = new List<string>
            {
                GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.ModelView),
                GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.ModelViewProjection),
                GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.ITModelView),
                GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.IModelView)
            };
            return string.Join("\n", uniforms);
        }
    }
}
