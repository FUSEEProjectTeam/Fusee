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
        /// The standard name for the fragment shader output.
        /// </summary>
        public static string OutColorName = "oFragmentColor";

        /// <summary>
        /// Returns the in parameters for a ShaderEffect, depending on the given ShaderEffectProps.
        /// </summary>
        /// <param name="effectProps">The ShaderEffectProps.</param>
        /// <returns></returns>
        public static string InParams(ShaderEffectProps effectProps)
        {
            var pxIn = new List<string>
            {
                GLSL.CreateIn(GLSL.Type.Vec3, VaryingNameDeclarations.ViewDirection),
                GLSL.CreateIn(GLSL.Type.Vec4, VaryingNameDeclarations.Position),
                GLSL.CreateIn(GLSL.Type.Vec3, VaryingNameDeclarations.CameraPosition)
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

            if (effectProps.MeshProbs.HasUVs)
                pxIn.Add(GLSL.CreateIn(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates));

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
                GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_MV"),
                GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_IMV"),
                GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_ITV"),
                GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_IV"),
                GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_V"),
                GLSL.CreateUniform(GLSL.Type.Mat4, "FUSEE_ITMV")
            };

            return string.Join("\n", pxFusUniforms);
        }

        /// <summary>
        /// Returns all uniforms, as they are given in the <see cref="MaterialProps"/> object.
        /// </summary>
        /// <param name="effectProps">The ShaderEffectProps.</param>
        /// <returns></returns>
        public static string MatPropsUniforms(ShaderEffectProps effectProps)
        {
            var matPropUnifroms = new List<string>();

            if (effectProps.MatProbs.HasSpecular)
            {
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Vec4, UniformNameDeclarations.SpecularColorName));

                if (effectProps.MatType == MaterialType.MaterialPbr)
                {
                    matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.RoughnessValue));
                    matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.FresnelReflectance));
                    matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.DiffuseFraction));
                }
                else if(effectProps.MatType == MaterialType.Standard)
                { 
                    matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.SpecularShininessName));
                    matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.SpecularIntensityName));                    
                }
            }

            if (effectProps.MatProbs.HasDiffuse)
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Vec4, UniformNameDeclarations.DiffuseColorName));

            if (effectProps.MatProbs.HasEmissive)
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Vec4, UniformNameDeclarations.EmissiveColorName));

            //Textures
            if (effectProps.MatProbs.HasBump)
            {
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Sampler2D, UniformNameDeclarations.BumpTextureName));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.BumpIntensityName));
            }

            if (effectProps.MatProbs.HasDiffuseTexture)
            {
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Sampler2D, UniformNameDeclarations.DiffuseTextureName));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.DiffuseMixName));
            }

            if (effectProps.MatProbs.HasEmissiveTexture)
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
            return GLSL.CreateOut(GLSL.Type.Vec4, OutColorName);
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

            var ssaoString = RenderTargetTextureTypes.G_SSAO.ToString();
            for (int i = 0; i < UniformNameDeclarations.DeferredRenderTextures.Count; i++)
            {
                var texName = UniformNameDeclarations.DeferredRenderTextures[i];

                if (texName == ssaoString) continue;

                outs.Add($"layout (location = {texCount}) out vec4 {texName};\n");
                texCount++;
            }
            return string.Join("\n", outs);
        }        
    }
}
