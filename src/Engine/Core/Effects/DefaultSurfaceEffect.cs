using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// The default <see cref="ShaderEffect"/>, that is used if no other ShaderEffect is found.
    /// Provides properties to change the Diffuse Color, Specular Color, Specular Intensity and Specular Shininess.
    /// </summary>
    public class DefaultSurfaceEffect : SurfaceEffect
    {
        #region Matrices

        /// <summary>
        /// The shader shard containing the model view projection matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Matrix)]
        public static float4x4 FUSEE_MVP = float4x4.Identity;

        /// <summary>
        /// The shader shard containing the inverse transposed model view matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Matrix)]
        public static float4x4 FUSEE_ITMV = float4x4.Identity;

        /// <summary>
        /// The shader shard containing the model view matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Matrix)]
        public static float4x4 FUSEE_MV = float4x4.Identity;

        #endregion

        /// <summary>
        /// Creates a new instance of type DefaultSurfaceEffect.
        /// </summary>
        /// <param name="lightingSetup">See <see cref="SurfaceEffect.LightingSetup"/>.</param>
        /// <param name="input">See <see cref="SurfaceEffect.SurfaceInput"/>.</param>
        /// <param name="surfOutBody"></param>
        /// <param name="rendererStates"></param>
        public DefaultSurfaceEffect(LightingSetupFlags lightingSetup, ColorInput input, List<string> surfOutBody, RenderStateSet rendererStates = null)
            : base(lightingSetup, input, rendererStates)
        {
            SurfOutMethod = SurfaceOut.GetChangeSurfFragMethod(surfOutBody, input.GetType());
            HandleFieldsAndProps();
        }
    }
}
