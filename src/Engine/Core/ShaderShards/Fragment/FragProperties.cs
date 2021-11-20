using Fusee.Base.Common;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
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

            var ssaoString = RenderTargetTextureTypes.Ssao.ToString();
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
        public static string DeferredLightAndShadowUniforms(Light lc, bool isCascaded, int numberOfCascades)
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
                        uniforms.Add(GLSL.CreateUniform(GLSL.Type.Sampler2DShadow, UniformNameDeclarations.ShadowMap));
                    else
                        uniforms.Add(GLSL.CreateUniform(GLSL.Type.SamplerCube, UniformNameDeclarations.ShadowCubeMap));
                }
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.LightSpaceMatrix));
            }
            else
            {
                uniforms.Add(GLSL.CreateUniform(GLSL.Type.ArrayTextureShadow, UniformNameDeclarations.ShadowMap));
                //No implementation for GLSL.CreateArrayUniform yet...
                uniforms.Add($"uniform {GLSL.DecodeType(GLSL.Type.Vec2)}[{numberOfCascades}] {UniformNameDeclarations.LightMatClipPlanes};\n");
                uniforms.Add($"uniform {GLSL.DecodeType(GLSL.Type.Mat4)}[{numberOfCascades}] {UniformNameDeclarations.LightSpaceMatrices};\n");
            }
            uniforms.Add(GLSL.CreateUniform(GLSL.Type.Vec2, UniformNameDeclarations.ClippingPlanes));
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