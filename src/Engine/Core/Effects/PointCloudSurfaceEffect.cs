using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using Fusee.Math.Core;

namespace Fusee.Engine.Core.Effects
{
    public class PointCloudSurfaceEffect : DefaultSurfaceEffect
    {

        /// <summary>
        /// The shader shard containing the model view projection matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Uniform)]
        public int ColorMode
        {
            get { return _colorMode; }
            set
            {
                _colorMode = value;
                SetFxParam(nameof(ColorMode), _colorMode);
            }
        }
        private static int _colorMode;

        /// <summary>
        /// The shader shard containing the model view projection matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Uniform)]
        public int PointSize
        {
            get { return _pointSize; }
            set
            {
                _pointSize = value;
                SetFxParam(nameof(PointSize), _pointSize);
            }
        }
        private static int _pointSize;

        /// <summary>
        /// Creates a new instance of type DefaultSurfaceEffect.
        /// </summary>
        /// <param name="rendererStates">The renderer state set for this effect.</param>
        public PointCloudSurfaceEffect(RenderStateSet rendererStates = null)
            : base(LightingSetupFlags.Unlit, new ColorInput() {Albedo = new float4(.5f, 0f, .5f, 1f) }, FragShards.SurfOutBody_VertOrAlbedoColor, VertShards.SufOutBody_Pos, rendererStates)
        {
            RendererStates.SetRenderState(RenderState.FillMode, (uint)FillMode.Point);
        }
    }
}