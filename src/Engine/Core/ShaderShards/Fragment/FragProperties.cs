using Fusee.Base.Common;
using Fusee.Engine.Common;
using Fusee.Serialization;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Fragment
{
    /// <summary>
    /// Collection of shader code strings, describing possible in, out and uniform properties of a fragment shader.
    /// </summary>
    public static class FragProperties
    {
        /// <summary>
        /// The standard name for the fragment shader color output.
        /// </summary>
        public static string OutColorName = "oFragmentColor";

        /// <summary>
        /// Creates a single color (vec4) out parameter.
        /// </summary>       
        public static string ColorOut()
        {
            return GLSL.CreateOut(GLSL.Type.Vec4, OutColorName);
        }

        /// <summary>
        /// Creates the out parameters for rendering into a G-Buffer object.
        /// </summary>
        /// <returns></returns>
        public static string GBufferOut()
        {
            var outs = new List<string>();
            var texCount = 0;

            var ssaoString = RenderTargetTextureTypes.G_SSAO.ToString();
            outs.Add("\n");
            for (int i = 0; i < UniformNameDeclarations.DeferredRenderTextures.Count; i++)
            {
                var texName = UniformNameDeclarations.DeferredRenderTextures[i];

                if (texName == ssaoString) continue;

                outs.Add($"layout (location = {texCount}) out vec4 {texName};\n");
                texCount++;
            }
            return string.Join("\n", outs);
        }

        /// <summary>
        /// Returns the in parameters for a ShaderEffect, depending on the given ShaderEffectProps.
        /// </summary>
        /// <param name="effectProps">The ShaderEffectProps.</param>
        /// <returns></returns>
        public static string InParams(EffectProps effectProps)
        {
            var pxIn = new List<string>
            {
                GLSL.CreateIn(GLSL.Type.Vec4, VaryingNameDeclarations.Position)
            };

            if (effectProps.MeshProbs.HasColors)
                pxIn.Add(GLSL.CreateIn(GLSL.Type.Vec3, VaryingNameDeclarations.Color));

            if (effectProps.MeshProbs.HasNormals)
                pxIn.Add(GLSL.CreateIn(GLSL.Type.Vec3, VaryingNameDeclarations.Normal));

            if (effectProps.MeshProbs.HasTangents && effectProps.MeshProbs.HasBiTangents)
            {
                pxIn.Add(GLSL.CreateIn(GLSL.Type.Vec4, VaryingNameDeclarations.Tangent));
                pxIn.Add(GLSL.CreateIn(GLSL.Type.Vec3, VaryingNameDeclarations.Bitangent));
            }

            if (effectProps.LightingProps.HasNormalMap)
                pxIn.Add(GLSL.CreateIn(GLSL.Type.Mat3, "TBN"));

            if (effectProps.MeshProbs.HasUVs)
                pxIn.Add(GLSL.CreateIn(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates));
            pxIn.Add("\n");
            return string.Join("\n", pxIn);
        }

        /// <summary>
        /// Returns the pre defined Fusee uniform parameters of a fragment shader, depending on the given ShaderEffectProps.
        /// </summary>
        /// <returns></returns>
        public static string FuseeMatrixUniforms()
        {
            var pxFusUniforms = new List<string>
            {
                GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.ModelView),
                GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.IModelView),
                GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.ITView),
                GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.IView),
                GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.View),
                GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.ITModelView),
                "\n"
            };

            return string.Join("\n", pxFusUniforms);
        }

        /// <summary>
        /// Returns all uniforms, as they are given in the <see cref="LightingProps"/> object.
        /// </summary>
        /// <param name="effectProps">The ShaderEffectProps.</param>
        /// <returns></returns>
        public static string MaterialPropsUniforms(EffectProps effectProps)
        {
            var matPropUnifroms = new List<string>();

            if (effectProps.LightingProps.SpecularLighting == SpecularLighting.Pbr)
            {
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Vec4, UniformNameDeclarations.SpecularColor));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.RoughnessValue));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.FresnelReflectance));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.DiffuseFraction));
            }
            else if (effectProps.LightingProps.SpecularLighting == SpecularLighting.Std)
            {
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Vec4, UniformNameDeclarations.SpecularColor));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.SpecularShininess));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.SpecularStrength));
            }

            if (effectProps.LightingProps.DoDiffuseLighting)
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Vec4, UniformNameDeclarations.Albedo));

            if (effectProps.LightingProps.HasEmissive)
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Vec4, UniformNameDeclarations.EmissiveColor));

            //Textures
            if (effectProps.LightingProps.HasNormalMap)
            {
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Sampler2D, UniformNameDeclarations.BumpTexture));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Vec2, UniformNameDeclarations.BumpTextureTiles));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.BumpIntensity));
            }

            if (effectProps.LightingProps.HasDiffuseTexture)
            {
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Sampler2D, UniformNameDeclarations.DiffuseTexture));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.DiffuseMix));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Vec2, UniformNameDeclarations.DiffuseTextureTiles));
            }

            if (effectProps.LightingProps.HasEmissiveTexture)
            {
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Sampler2D, UniformNameDeclarations.EmissiveTexture));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.EmissiveMix));
            }
            matPropUnifroms.Add("\n");
            return string.Join("\n", matPropUnifroms);
        }

        /// <summary>
        /// Creates the uniform texture parameters for the lighting pass, as used in deferred rendering.
        /// </summary>
        /// <returns></returns>
        public static string DeferredTextureUniforms()
        {
            var uniforms = new List<string>();
            var texCount = 0;
            for (int i = 0; i < UniformNameDeclarations.DeferredRenderTextures.Count; i++)
            {
                var texName = UniformNameDeclarations.DeferredRenderTextures[i];

                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Sampler2D, texName));
                texCount++;
            }
            uniforms.Add("\n");
            return string.Join("\n", uniforms);
        }

        /// <summary>
        /// Creates the uniforms for the deferred lighting pass for one light.
        /// </summary>
        /// <param name="lc">The light component, needed to decide if we have a Shadow Cube Map or a standard shadow map.</param>
        /// <param name="isCascaded">If cascaded shadow mapping is used, this should be set to true.</param>
        /// <param name="numberOfCascades">If cascaded shadow mapping is used this is the number of cascades.</param>        
        public static string DeferredLightAndShadowUniforms(LightComponent lc, bool isCascaded, int numberOfCascades)
        {
            var uniforms = new List<string>
            {
                "uniform Light light;"
            };

            if (!isCascaded)
            {
                if (lc.IsCastingShadows)
                {
                    if (lc.Type != LightType.Point)
                        uniforms.Add(GLSL.CreateUniform(GLSL.Type.Sampler2D, UniformNameDeclarations.ShadowMap));
                    else
                        uniforms.Add(GLSL.CreateUniform(GLSL.Type.SamplerCube, UniformNameDeclarations.ShadowCubeMap));
                }
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.LightSpaceMatrix));
            }
            else
            {
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.ArrayTexture, "ShadowMap"));
                //No implementation for GLSL.CreateArrayUniform yet...
                uniforms.Add($"uniform {GLSL.DecodeType(GLSL.Type.Vec2)}[{numberOfCascades}] ClipPlanes;\n");
                uniforms.Add($"uniform {GLSL.DecodeType(GLSL.Type.Mat4)}[{numberOfCascades}] LightSpaceMatrices;\n");
            }

            uniforms.Add(GLSL.CreateUniform(GLSL.Type.Int, UniformNameDeclarations.RenderPassNo));
            uniforms.Add(GLSL.CreateUniform(GLSL.Type.Int, UniformNameDeclarations.SsaoOn));

            uniforms.Add(GLSL.CreateUniform(GLSL.Type.Vec4, UniformNameDeclarations.BackgroundColor));
            return string.Join("\n", uniforms);
        }

        /// <summary>
        /// Creates the "allLights" uniform array, as it is used in forward rendering.
        /// </summary>
        /// <returns></returns>
        public static string FixedNumberLightArray = $"uniform Light allLights[{Lighting.NumberOfLightsForward}];\n";
    }
}
