using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{

    public struct InstanceData : IInstanceData
    {
        public float3[] Translations { get; }

        public float4[] Colors { get; }

        public int Amount { get; }

        public InstanceData(int amount, float3[] translations, float4[] colors = null)
        {
            Amount = amount;
            if (Amount != translations.Length)
                throw new ArgumentOutOfRangeException();
            Translations = new float3[amount];

            if (colors != null)
            {
                if (Amount != colors.Length)
                    throw new ArgumentOutOfRangeException();
                Colors = new float4[amount];
            }
            else
                Colors = null;
        }
    }

    public class SurfaceEffectInstanced : SurfaceEffectBase
    {
        public InstanceData InstanceData;

        #region Internal/Global Uniforms (set by the Engine)

        /// <summary>
        /// The shader shard containing the model matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.InternalUniform)]
        public float4x4 FUSEE_M;

        /// <summary>
        /// The shader shard containing the view matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.InternalUniform)]
        public float4x4 FUSEE_V;

        /// <summary>
        /// The shader shard containing the projection matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.InternalUniform)]
        public float4x4 FUSEE_P;

        /// <summary>
        /// Used to linearize the values form the depth map.
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
        public SurfaceEffectInstanced(SurfaceEffectInput input, List<string> surfOutVertBody = null, List<string> surfOutFragBody = null, RenderStateSet rendererStates = null)
            : base(input, rendererStates)
        {
            VertIn += VertProperties.InParamsInstanced();

            var inputType = input.GetType();
            if (surfOutFragBody != null)
                SurfOutFragMethod = SurfaceOut.GetChangeSurfFragMethod(surfOutFragBody, inputType);
            else
                SurfOutFragMethod = SurfaceOut.GetChangeSurfFragMethod(FragShards.SurfOutBody(input), inputType);

            if (surfOutVertBody != null)
                SurfOutVertMethod = SurfaceOut.GetChangeSurfVertMethod(surfOutVertBody, input.ShadingModel);
            else
                SurfOutVertMethod = SurfaceOut.GetChangeSurfVertMethod(VertShards.SurfOutBody(input), input.ShadingModel);

            FUSEE_M = float4x4.Identity;
            FUSEE_V = float4x4.Identity;
            FUSEE_P = float4x4.Identity;
            HandleFieldsAndProps();
        }
    }
}
