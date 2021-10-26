using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// <see cref="SurfaceEffect"/> for Rendering Point Clouds.
    /// </summary>
    public class PointCloudSurfaceEffect : DefaultSurfaceEffect
    {

        /// <summary>
        /// The shader shard containing the projection matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Matrix)]
        public float4x4 FUSEE_P;

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
        /// The depth texture, used for eye dome lighting.
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
        /// Determines how many neighbouring pixels shall be used in the eye dome lighting calculation.
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
        /// Used to lineraize the values form the depth map.
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
        /// The width and height of the render canvas in px.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
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
        /// Shape of the points.
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
        /// The shader shard containing the Point Size.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Uniform)]
        public int PointShape
        {
            get { return _pointShape; }
            set
            {
                _pointShape = value;
                SetFxParam(nameof(PointShape), _pointShape);
            }
        }
        private int _pointShape;


        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public readonly string ViewPosIn = GLSL.CreateIn(GLSL.Type.Vec4, "vViewPos");

        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public readonly string ViewPosOut = GLSL.CreateOut(GLSL.Type.Vec4, "vViewPos");

        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public readonly string WorldSpacePointRadIn = GLSL.CreateIn(GLSL.Type.Float, "vWorldSpacePointRad");

        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public readonly string WorldSpacePointRadOut = GLSL.CreateOut(GLSL.Type.Float, "vWorldSpacePointRad");        

        public static readonly List<string> CalculatePointShapeVaryings = new()
        {
            $"vViewPos = {UniformNameDeclarations.ModelView} * vec4(fuVertex.xyz, 1.0);",
            $"float fov = 2.0 * atan(1.0 / {UniformNameDeclarations.Projection}[1][1]);",
            "float slope = tan(fov / 2.0);",
            $"float projFactor = ((1.0 / slope) / -vViewPos.z) * {nameof(ScreenParams)}.y / 2.0;",
            $"vWorldSpacePointRad = float ({UniformNameDeclarations.PointSize}) / projFactor;"
        };

        /// <summary>
        /// Fragment Shader Shard for linearizing a depth value using the clipping planes of the current camera.
        /// </summary>
        public static readonly List<string> CalculatePointShape = new()
        {
            "vec2 distanceVector = (2.0 * gl_PointCoord) - 1.0; //[-1,1]",
            "float weight;",
            "",
            "switch (PointShape)",
            "{",
            "    case 0: // default = square",
            "    default:",
            "        gl_FragDepth = gl_FragCoord.z;",
            "        break;",
            "    case 1: // circle	",
            "",
            "        float distanceFromCenter = length(2.0 * gl_PointCoord - 1.0);",
            "",
            "        if(distanceFromCenter > 1.0)",
            "            discard;",
            "",
            "        gl_FragDepth = gl_FragCoord.z;",
            "",
            "        break;",
            "    case 2: //paraboloid",
            "",
            "        weight = 1.0 - (pow(distanceVector.x, 2.0) + pow(distanceVector.y, 2.0)); //paraboloid weight function",
            "",
            "        vec4 position = vViewPos;",
            "        position.z += weight * vWorldSpacePointRad;",
            "        position *= FUSEE_P;",
            "        position /= position.w;",
            "        gl_FragDepth = (position.z + 1.0) / 2.0;",
            "",
            "        break;",
            "}"
        };

        /// <summary>
        /// Creates a new instance of type PointCloudSurfaceEffect.
        /// </summary>
        /// <param name="rendererStates">The renderer state set for this effect.</param>
        public PointCloudSurfaceEffect(RenderStateSet rendererStates = null)
            : base(LightingSetupFlags.Edl, new ColorInput() { Albedo = new float4(.5f, 0f, .5f, 1f) }, FragShards.SurfOutBody_VertOrAlbedoColor.Concat(CalculatePointShape).ToList(), VertShards.SufOutBody_Pos.Concat(CalculatePointShapeVaryings).ToList(), rendererStates)
        {
            RendererStates.SetRenderState(RenderState.FillMode, (uint)FillMode.Point);
        }
    }
}