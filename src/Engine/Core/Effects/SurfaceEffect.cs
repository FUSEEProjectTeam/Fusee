using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// The default <see cref="Effect"/>, that is used if no other Effect is found.
    /// Provides properties to change the Diffuse Color, Specular Color, Specular Intensity and Specular Shininess.
    /// </summary>
    public class SurfaceEffect : SurfaceEffectBase
    {
        #region Internal/Global Uniforms (set by the Engine)

        /// <summary>
        /// The shader shard containing the model view projection matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.InternalUniform)]
        public float4x4 FUSEE_MVP;

        /// <summary>
        /// The shader shard containing the inverse transposed model view matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.InternalUniform)]
        public float4x4 FUSEE_ITMV;

        /// <summary>
        /// The shader shard containing the model view matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.InternalUniform)]
        public float4x4 FUSEE_MV;

        /// <summary>
        /// The shader shard containing the projection matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.InternalUniform)]
        public float4x4 FUSEE_P;

        /// <summary>
        /// Used to lineraize the values form the depth map.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.InternalUniform)]
        public float2 FUSEE_ClippingPlanes;

        /// <summary>
        /// The width and height of the render canvas in px.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.InternalUniform)]
        public float2 FUSEE_ViewportPx;

        #endregion

        /// <summary>
        /// Creates a new instance of type DefaultSurfaceEffect.
        /// </summary>
        /// <param name="input">See <see cref="SurfaceEffectBase.SurfaceInput"/>.</param>
        /// <param name="surfOutFragBody">The method body for the <see cref="SurfaceEffectBase.SurfOutFragMethod"/></param>
        /// <param name="surfOutVertBody">The method body for the <see cref="SurfaceEffectBase.SurfOutVertMethod"/></param>
        /// <param name="rendererStates">The renderer state set for this effect.</param>
        public SurfaceEffect(SurfaceEffectInput input, List<string> surfOutVertBody = null, List<string> surfOutFragBody = null, RenderStateSet rendererStates = null)
            : base(input, rendererStates)
        {
            var inputType = input.GetType();
            if (surfOutFragBody != null)
                SurfOutFragMethod = SurfaceOut.GetChangeSurfFragMethod(surfOutFragBody, inputType);
            else
                SurfOutFragMethod = SurfaceOut.GetChangeSurfFragMethod(FragShards.SurfOutBody(input), inputType);

            if (surfOutVertBody != null)
                SurfOutVertMethod = SurfaceOut.GetChangeSurfVertMethod(surfOutVertBody, input.ShadingModel);
            else
                SurfOutVertMethod = SurfaceOut.GetChangeSurfVertMethod(VertShards.SurfOutBody(input), input.ShadingModel);

            FUSEE_MVP = float4x4.Identity;
            FUSEE_ITMV = float4x4.Identity;
            FUSEE_MVP = float4x4.Identity;

            //TODO: try to suppress adding these parameters if the effect is used only for deferred rendering.
            //May be difficult because we'd need to remove or add them (and only them) depending on the render method
            VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Main, VertMain.VertexMain(SurfaceInput.ShadingModel, SurfaceInput.TextureSetup)));
            foreach (var dcl in CreateForwardLightingParamDecls(Lighting.NumberOfLightsForward))
                ParamDecl.Add(dcl.Hash, dcl);

            HandleFieldsAndProps();
        }
    }
}