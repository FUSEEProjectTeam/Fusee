using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// The default <see cref="Effect"/>, that is used if no other Effect is found.
    /// Provides properties to change the Diffuse Color, Specular Color, Specular Intensity and Specular Shininess.
    /// </summary>
    public class SurfaceEffect : SurfaceEffectBase
    {
        #region Matrices

        /// <summary>
        /// The shader shard containing the model view projection matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Matrix)]
        public float4x4 FUSEE_MVP;

        /// <summary>
        /// The shader shard containing the inverse transposed model view matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Matrix)]
        public float4x4 FUSEE_ITMV;

        /// <summary>
        /// The shader shard containing the model view matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Matrix)]
        public float4x4 FUSEE_MV;

        #endregion

        /// <summary>
        /// Creates a new instance of type DefaultSurfaceEffect.
        /// </summary>
        /// <param name="input">See <see cref="SurfaceEffectBase.SurfaceInput"/>.</param>
        /// <param name="surfOutFragBody">The method body for the <see cref="SurfaceEffectBase.SurfOutFragMethod"/></param>
        /// <param name="surfOutVertBody">The method body for the <see cref="SurfaceEffectBase.SurfOutVertMethod"/></param>
        /// <param name="rendererStates">The renderer state set for this effect.</param>
        public SurfaceEffect(SurfaceInput input, List<string> surfOutVertBody = null, List<string> surfOutFragBody = null, RenderStateSet rendererStates = null)
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
            HandleFieldsAndProps();
        }
    }
}