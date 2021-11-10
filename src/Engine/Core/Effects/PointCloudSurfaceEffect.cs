using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using Fusee.Math.Core;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// <see cref="SurfaceEffectBase"/> for Rendering Point Clouds.
    /// </summary>
    public class PointCloudSurfaceEffect : SurfaceEffect
    {
        /// <summary>
        /// The shader shard containing an value that is used to change the lighting calculation.
        /// For now only Eye Dome Lighting or Unlit are supported.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Uniform)]
        public bool DoEyeDomeLighting
        {
            get { return _doEyeDomeLighting; }
            set
            {
                _doEyeDomeLighting = value;
                SetFxParam(nameof(DoEyeDomeLighting), _doEyeDomeLighting);
            }
        }
        private bool _doEyeDomeLighting;

        /// <summary>
        /// The shader shard containing the strength of the eye dome lighting.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Uniform)]
        public WritableTexture DepthTex
        {
            get { return _depthTexture; }
            set
            {
                _depthTexture = value;
                SetFxParam(nameof(DepthTex), _depthTexture);
            }
        }
        private WritableTexture _depthTexture;

        /// <summary>
        /// The shader shard containing the strength of the eye dome lighting.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Uniform)]
        public float EDLStrength
        {
            get { return _edlStrength; }
            set
            {
                _edlStrength = value;
                SetFxParam(nameof(EDLStrength), _edlStrength);
            }
        }
        private float _edlStrength;

        /// <summary>
        /// The shader shard containing the strength of the eye dome lighting.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Uniform)]
        public int EDLNeighbourPixels
        {
            get { return _edlNeighbourPixels; }
            set
            {
                _edlNeighbourPixels = value;
                SetFxParam(nameof(EDLNeighbourPixels), _edlNeighbourPixels);
            }
        }
        private int _edlNeighbourPixels;

        /// <summary>
        /// The shader shard containing the strength of the eye dome lighting.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Uniform)]
        public float2 ClippingPlanes
        {
            get { return _clippingPlanes; }
            set
            {
                _clippingPlanes = value;
                SetFxParam(nameof(ClippingPlanes), _clippingPlanes);
            }
        }
        private float2 _clippingPlanes;

        /// <summary>
        /// The shader shard containing the strength of the eye dome lighting.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Uniform)]
        public float2 ScreenParams
        {
            get { return _screenParams; }
            set
            {
                _screenParams = value;
                SetFxParam(nameof(ScreenParams), _screenParams);
            }
        }
        private float2 _screenParams;

        /// <summary>
        /// The shader shard containing the Color Mode.
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
        private int _colorMode;

        /// <summary>
        /// The shader shard containing the Point Size.
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
        private int _pointSize;

        /// <summary>
        /// Creates a new instance of type DefaultSurfaceEffect.
        /// </summary>
        /// <param name="rendererStates">The renderer state set for this effect.</param>
        public PointCloudSurfaceEffect(RenderStateSet rendererStates = null)
            : base(new EdlInput() { Albedo = new float4(.5f, 0f, .5f, 1f) }, null, null, rendererStates)
        {
            RendererStates.SetRenderState(RenderState.FillMode, (uint)FillMode.Point);
        }
    }
}