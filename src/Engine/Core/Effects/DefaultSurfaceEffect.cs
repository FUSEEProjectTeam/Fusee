using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// The default <see cref="Effect"/>, that is used if no other Effect is found.
    /// Provides properties to change the Diffuse Color, Specular Color, Specular Intensity and Specular Shininess.
    /// </summary>
    public class DefaultSurfaceEffect : SurfaceEffect, IDisposable
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

        private bool _disposed;

        /// <summary>
        /// Creates a new instance of type DefaultSurfaceEffect.
        /// </summary>
        /// <param name="lightingSetup">See <see cref="SurfaceEffect.LightingSetup"/>.</param>
        /// <param name="input">See <see cref="SurfaceEffect.SurfaceInput"/>.</param>
        /// <param name="surfOutFragBody">The method body for the <see cref="SurfaceEffect.SurfOutFragMethod"/></param>
        /// <param name="surfOutVertBody">The method body for the <see cref="SurfaceEffect.SurfOutVertMethod"/></param>
        /// <param name="rendererStates">The renderer state set for this effect.</param>
        public DefaultSurfaceEffect(LightingSetupFlags lightingSetup, ColorInput input, List<string> surfOutFragBody, List<string> surfOutVertBody, RenderStateSet rendererStates = null)
            : base(lightingSetup, input, rendererStates)
        {
            SurfOutFragMethod = SurfaceOut.GetChangeSurfFragMethod(surfOutFragBody, input.GetType());
            SurfOutVertMethod = SurfaceOut.GetChangeSurfVertMethod(surfOutVertBody, lightingSetup);
            FUSEE_MVP = float4x4.Identity;
            FUSEE_ITMV = float4x4.Identity;
            FUSEE_MVP = float4x4.Identity;
            HandleFieldsAndProps();
        }
    }
}