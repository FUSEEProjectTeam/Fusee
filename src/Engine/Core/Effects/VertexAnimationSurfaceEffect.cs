using Fusee.Engine.Core.ShaderShards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// The Surface effect used for Interpolation between 2 sets of vertices.
    /// </summary>
    public class VertexAnimationSurfaceEffect : DefaultSurfaceEffect
    {
        private float _percentPerVertex;
        private float _percentPerVertex1;
        /// <summary>
        /// The shader shard containing the PercentPerVertex uniform.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Uniform)]
        public float PercentPerVertex
        {
            get
            {
                return _percentPerVertex;
            }
            set
            {
                _percentPerVertex = value;
                SetFxParam(nameof(PercentPerVertex), _percentPerVertex);
            }
        }

        /// <summary>
        ///  The shader shard containing the PercentPerVertex1 uniform.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Uniform)]
        public float PercentPerVertex1
        {
            get
            {
                return _percentPerVertex1;
            }
            set
            {
                _percentPerVertex1 = value;
                SetFxParam(nameof(PercentPerVertex1), _percentPerVertex1);
            }
        }


        /// <summary>
        /// Creates a new instance of type VertexAnimationSurfaceEffect.
        /// </summary>
        /// <param name="lightingSetup">See <see cref="SurfaceEffect.LightingSetup"/>.</param>
        /// <param name="input">See <see cref="SurfaceEffect.SurfaceInput"/>.</param>
        /// <param name="surfOutFragBody">The method body for the <see cref="SurfaceEffect.SurfOutFragMethod"/></param>
        /// <param name="SufOutBody_PosNormAnimation">The method body for the <see cref="ShaderShards.Vertex.VertShards.SufOutBody_PosNormAnimation"/></param>
        /// <param name="rendererStates">The renderer state set for this effect.</param>
        public VertexAnimationSurfaceEffect(LightingSetupFlags lightingSetup, ColorInput input, List<string> surfOutFragBody, List<string> SufOutBody_PosNormAnimation, RenderStateSet rendererStates = null)
            : base(lightingSetup, input, surfOutFragBody, SufOutBody_PosNormAnimation, rendererStates)
        {

        }
    }
}