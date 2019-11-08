using Fusee.Engine.Common;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Fragment
{
    public static class FragPropertiesShard
    {
        public static string InParams(ShaderEffectProps effectProps)
        {
            var pxIn = new List<string>
            {
                GLSL.CreateIn(GLSL.Type.Vec3, "vViewDir"),
                GLSL.CreateIn(GLSL.Type.Vec3, "vViewPos"),
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

        public static string MatPropsUniforms(ShaderEffectProps effectProps)
        {
            var matPropUnifroms = new List<string>();

            if (effectProps.MatProbs.HasSpecular)
            {
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.SpecularShininessName));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Float, UniformNameDeclarations.SpecularIntensityName));
                matPropUnifroms.Add(GLSL.CreateUniform(GLSL.Type.Vec4, UniformNameDeclarations.SpecularColorName));
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

        public static string ColorOut()
        {
            return GLSL.CreateOut(GLSL.Type.Vec4, "oFragmentColor");
        }

        public static string GBufferOut(RenderTarget rt)
        {
            var outs = new List<string>();
            var texCount = 0;
            for (int i = 0; i < rt.RenderTextures.Length; i++)
            {
                var tex = rt.RenderTextures[i];
                if (tex == null) continue;

                outs.Add($"layout (location = {texCount}) out vec4 {Enum.GetName(typeof(RenderTargetTextureTypes), i)};\n");
                texCount++;
            }
            return string.Join("\n", outs);
        }
    }
}
