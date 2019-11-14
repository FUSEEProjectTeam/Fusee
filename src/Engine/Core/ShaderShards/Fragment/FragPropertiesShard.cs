using Fusee.Engine.Common;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Fragment
{
    /// <summary>
    /// Collection of Shader Shards, describing possible in, out and uniform properties of a fragment shader.
    /// </summary>
    public static class FragPropertiesShard
    {
        /// <summary>
        /// Returns the in parameters for a ShaderEffect, depending on the given ShaderEffectProps.
        /// </summary>
        /// <param name="effectProps">The ShaderEffectProps.</param>
        /// <returns></returns>
        public static string InParams(ShaderEffectProps effectProps)
        {
            var pxIn = new List<string>
            {
                GLSL.CreateIn(GLSL.Type.Vec3, "vViewDir"),
                GLSL.CreateIn(GLSL.Type.Vec4, "vPos"),
                GLSL.CreateIn(GLSL.Type.Vec3, "vCamPos")
            };

            if (effectProps.MeshProbs.HasColors)
                pxIn.Add(GLSL.CreateIn(GLSL.Type.Vec3, "vColor"));

            if (effectProps.MeshProbs.HasNormals)
                pxIn.Add(GLSL.CreateIn(GLSL.Type.Vec3, "vNormal"));

            if (effectProps.MeshProbs.HasTangents && effectProps.MeshProbs.HasBiTangents)
            {
                pxIn.Add(GLSL.CreateIn(GLSL.Type.Vec4, "vT"));
                pxIn.Add(GLSL.CreateIn(GLSL.Type.Vec3, "vB"));
            }

            if (effectProps.MeshProbs.HasUVs)
                pxIn.Add(GLSL.CreateIn(GLSL.Type.Vec2, "vUV"));

            return string.Join("\n", pxIn);
        }

        /// <summary>
        /// Returns the pre defined Fusee uniform parameters of a fragment shader, depending on the given ShaderEffectProps.
        /// </summary>
        /// <param name="effectProps">The ShaderEffectProps.</param>
        /// <returns></returns>
        public static string FuseeUniforms(ShaderEffectProps effectProps)
        {
            var pxFusUniforms = new List<string>
            {
                GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_MV"),
                GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_IMV"),
                GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_IV"),
                GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_V")
            };

            if (effectProps.MatProbs.HasBump)
                pxFusUniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_ITMV"));

            // Multipass
            pxFusUniforms.Add(GLSL.CreateUniform(GLSL.Type.Sampler2D, "firstPassTex"));

            return string.Join("\n", pxFusUniforms);
        }

        /// <summary>
        /// Returns all uniforms, as they are given in the <see cref="MaterialProps"/> object.
        /// </summary>
        /// <param name="matProps">The MaterialProps.</param>
        /// <returns></returns>
        public static string MatPropsUniforms(MaterialProps matProps)
        {
            var matPropUnifroms = new List<string>();

            if (matProps.HasSpecular)
            {
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.SpecularShininessName));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.SpecularIntensityName));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Vec4, UniformNameDeclarations.SpecularColorName));
            }

            if (matProps.HasDiffuse)
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Vec4, UniformNameDeclarations.DiffuseColorName));

            if (matProps.HasEmissive)
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Vec4, UniformNameDeclarations.EmissiveColorName));

            //Textures
            if (matProps.HasBump)
            {
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Sampler2D, UniformNameDeclarations.BumpTextureName));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.BumpIntensityName));
            }

            if (matProps.HasDiffuseTexture)
            {
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Sampler2D, UniformNameDeclarations.DiffuseTextureName));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.DiffuseMixName));
            }

            if (matProps.HasEmissiveTexture)
            {
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Sampler2D, UniformNameDeclarations.EmissiveTextureName));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.EmissiveMixName));
            }

            return string.Join("\n", matPropUnifroms);
        }

        /// <summary>
        /// Creates the uniform texture parameters for the lighting pass, as used in deferred rendering.
        /// </summary>
        /// <returns></returns>
        public static string DeferredUniforms()
        {
            var outs = new List<string>();
            var texCount = 0;
            for (int i = 0; i < UniformNameDeclarations.DeferredRenderTextures.Count; i++)
            {
                var texName = UniformNameDeclarations.DeferredRenderTextures[i];

                outs.Add($"uniform sampler2D {texName};\n");
                texCount++;
            }
            return string.Join("\n", outs);
        }

        /// <summary>
        /// Creates a single color (vec4) out parameter.
        /// </summary>       
        public static string ColorOut()
        {
            return GLSL.CreateOut(GLSL.Type.Vec4, "oFragmentColor");
        }

        /// <summary>
        /// Creates the "allLights" uniform array, as it is used in forward rendering.
        /// </summary>
        /// <returns></returns>
        public static string FixedNumberLightArray()
        {
            return $"uniform Light allLights[{LightingShard.NumberOfLightsForward}];";
        }

        /// <summary>
        /// Creates the out parameters for rendering into a G-Buffer object.
        /// </summary>
        /// <returns></returns>
        public static string GBufferOut()
        {
            var outs = new List<string>();
            var texCount = 0;
            for (int i = 0; i < UniformNameDeclarations.DeferredRenderTextures.Count - 1; i++)
            {
                var texName = UniformNameDeclarations.DeferredRenderTextures[i];               

                outs.Add($"layout (location = {texCount}) out vec4 {texName};\n");
                texCount++;
            }
            return string.Join("\n", outs);
        }

        
    }
}
