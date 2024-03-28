using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using Fusee.Math.Core;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// <see cref="SurfaceEffectBase"/> for Rendering Point Clouds.
    /// </summary>
    public class SurfaceEffectPointCloud : SurfaceEffect
    {
        /// <summary>
        /// The depth texture, used for eye dome lighting.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Uniform)]
        public WritableTexture? DepthTex
        {
            get { return _depthTexture; }
            set
            {
                _depthTexture = value;
                SetFxParam(nameof(DepthTex), _depthTexture);
            }
        }
        private WritableTexture? _depthTexture;

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
        /// Determines how many neighboring pixels shall be used in the eye dome lighting calculation.
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
        /// Shape of the points.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Uniform)]
        public float PointSize
        {
            get { return _pointSize; }
            set
            {
                _pointSize = value;
                SetFxParam(nameof(PointSize), _pointSize);
            }
        }
        private float _pointSize;

        /// <summary>
        /// Shape of the points.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Uniform)]
        public int PointSizeMode
        {
            get { return _pointSizeMode; }
            set
            {
                _pointSizeMode = value;
                SetFxParam(nameof(PointSizeMode), _pointSizeMode);
            }
        }
        private int _pointSizeMode;

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

        /// <summary>
        /// The in variable for the view position in the fragment shader.
        /// Used for paraboloid shaped points.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public readonly string ViewPosIn = GLSL.CreateIn(GLSL.Type.Vec4, $"{VaryingNameDeclarations.ViewPos}");

        /// <summary>
        /// The out variable for the view position in the vertex shader.
        /// Used for paraboloid shaped points.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public readonly string ViewPosOut = GLSL.CreateOut(GLSL.Type.Vec4, $"{VaryingNameDeclarations.ViewPos}");

        /// <summary>
        /// Alternative to gl_PointSize
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public readonly string PointCoordIn = GLSL.CreateIn(GLSL.Type.Vec2, $"{VaryingNameDeclarations.PointCoord}");

        /// <summary>
        /// Alternative to gl_PointSize
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public readonly string PointCoordOut = GLSL.CreateOut(GLSL.Type.Vec2, $"{VaryingNameDeclarations.PointCoord}");

        /// <summary>
        /// The in variable for the point radius in the fragment shader.
        /// Used for paraboloid shaped points.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public readonly string WorldSpacePointRadIn = GLSL.CreateIn(GLSL.Type.Float, "vWorldSpacePointRad");

        /// <summary>
        /// The out variable for the point radius in the vertex shader.
        /// Used for paraboloid shaped points.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public readonly string WorldSpacePointRadOut = GLSL.CreateOut(GLSL.Type.Float, "vWorldSpacePointRad");

        /// <summary>
        /// Generate the varying variables
        /// </summary>
        /// <param name="doRenderInstanced"></param>
        /// <returns></returns>
        protected static List<string> CalculateVaryings(bool doRenderInstanced)
        {
            if (!doRenderInstanced)
            {
                return new List<string>() {
                    $"{VaryingNameDeclarations.ViewPos} = {UniformNameDeclarations.ModelView} * vec4({UniformNameDeclarations.Vertex}.xyz, 1.0);",
                    $"float fov = 2.0 * atan(1.0 / {UniformNameDeclarations.Projection}[1][1]);",
                    "float slope = tan(fov / 2.0);",
                    $"float projFactor = ((1.0 / slope) / -{VaryingNameDeclarations.ViewPos}.z) * float({UniformNameDeclarations.ViewportPx}.y) / 2.0;",
                    $"vWorldSpacePointRad = float ({UniformNameDeclarations.PointSize}) / projFactor;",

                    "switch(PointSizeMode)",
                    "{",
                    "   // Fixed pixel size",
                    "   default:",
                    "   case 0:",
                    "   {",
                    $"       gl_PointSize = {UniformNameDeclarations.PointSize};",
                    "       break;",
                    "   }",
                    "   //Fixed world size",
                    "   case 1:",
                    "   {",
                    "       //In this scenario the PointSize is the given point radius in world space - the point size in pixel will shrink if the camera moves farther away",
                    "",
                    "       //Formula as given (without division at the end) in Schuetz' thesis - produces points that are to big without the division!",
                    $"      gl_PointSize =  (float({UniformNameDeclarations.ViewportPx}.y) / 2.0) * (float({UniformNameDeclarations.PointSize}) / ( slope * {VaryingNameDeclarations.ViewPos}.z)) / 100.0;",
                    "       break;",
                    "   }",
                    "}"
                    };
            }
            else
            {
                return new List<string>() {
                    $"mat4 mv = FUSEE_V * FUSEE_M * {UniformNameDeclarations.InstanceModelMat};",

                    //assumption: position x and y are in range [-0.5, 0.5].
                    $"{VaryingNameDeclarations.PointCoord} = vec2(0.5) / {UniformNameDeclarations.Vertex}.xy;",
                    $"float z = mv[3][2]; //disctance from rect to cam",
                    $"float fov = 2.0 * atan(1.0 / {UniformNameDeclarations.Projection}[1][1]) * 180.0 / PI;",

                    "float slope = tan(fov / 2.0);",
                    $"float projFactor = ((1.0 / slope) / - z) * float({UniformNameDeclarations.ViewportPx}.y) / 2.0;",
                    $"vWorldSpacePointRad = float ({UniformNameDeclarations.PointSize}) / projFactor;",

                    $"float sizeInPx = 1.0;",
                    "float billboardHeight = 1.0;",
                    "switch(PointSizeMode)",
                    "{",
                    "   // Fixed pixel size",
                    "   case 0:",
                    "   {",
                    $"      sizeInPx = (billboardHeight / (2.0 * slope * z)) * float({UniformNameDeclarations.ViewportPx}.y);",
                    "       break;",
                    "   }",
                    "   //Fixed world size",
                    "   case 1:",
                    "   {",
                    "       //In this scenario the PointSize is the given point radius in world space - the point size in pixel will shrink if the camera moves farther away",                    "",
                    $"      sizeInPx = (billboardHeight / (2.0 * slope)) * float({UniformNameDeclarations.ViewportPx}.y);",
                    "       break;",
                    "   }",
                    "}",

                    $"float scaleFactor = {UniformNameDeclarations.PointSize} / sizeInPx;",

                    $"{VaryingNameDeclarations.ViewPos} = mv * vec4(0.0, 0.0, 0.0, 1.0)",
                    $"         + vec4({UniformNameDeclarations.Vertex}.x, {UniformNameDeclarations.Vertex}.y, 0.0, 0.0)",
                    $"         * vec4(scaleFactor, scaleFactor, 1.0, 1.0);",

                };
            }
        }

        /// <summary>
        /// Fragment Shader Shard for linearizing a depth value using the clipping planes of the current camera.
        /// </summary>
        protected static List<string> CalculatePointShape(bool doRenderInstanced)
        {
            if (!doRenderInstanced)
            {
                return new List<string>() {
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
                $"       vec4 position = {VaryingNameDeclarations.ViewPos};",
                "        position.z += weight * vWorldSpacePointRad;",
                $"       position = {UniformNameDeclarations.Projection} * position;",
                "        position /= position.w;",
                "        gl_FragDepth = (position.z + 1.0) / 2.0;",
                "",
                "        break;",
                "}"
                };
            }
            else
            {
                return new List<string>() {

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
                "        float distanceFromCenter = length(2.0 * vPointCoord - 1.0);",
                "",
                "        if(distanceFromCenter > 1.0)",
                "            discard;",
                "",
                "        gl_FragDepth = gl_FragCoord.z;",
                "",
                "        break;",
                "    case 2: //paraboloid",
                "",
                "        weight = 1.0 - (pow(vPointCoord.x, 2.0) + pow(vPointCoord.y, 2.0)); //paraboloid weight function",
                "",
                $"       vec4 position = {VaryingNameDeclarations.ViewPos};",
                "        position.z += weight * vWorldSpacePointRad;",
                $"       position = {UniformNameDeclarations.Projection} * position;",
                "        position /= position.w;",
                "        gl_FragDepth = (position.z + 1.0) / 2.0;",
                "",
                "        break;",
                "}"
                };
            }
        }

        /// <summary>
        /// Creates a new instance of type PointCloudSurfaceEffect.
        /// </summary>
        /// <param name="rendererStates">The renderer state set for this effect.</param>
        /// <param name="doRenderInstanced">Use instanced rendering for visualizing the point cloud?</param>
        public SurfaceEffectPointCloud(RenderStateSet? rendererStates = null, bool doRenderInstanced = false)
            : base(new EdlInput() { Albedo = new float4(.5f, 0f, .5f, 1f) },
                  RenderFlags.PointCloud | (doRenderInstanced ? RenderFlags.Instanced : RenderFlags.None),
                  CalculateVaryings(doRenderInstanced).Concat(VertShards.SurfOutBody(ShadingModel.Edl, doRenderInstanced)).ToList(),
                  FragShards.SurfOutBody(ShadingModel.Edl, TextureSetup.NoTextures).Concat(CalculatePointShape(doRenderInstanced)).ToList(),
                  rendererStates)
        {
            if (!doRenderInstanced)
            {
                RendererStates.SetRenderState(RenderState.FillMode, (uint)FillMode.Point);
            }
            else
            {
                RendererStates.SetRenderState(RenderState.FillMode, (uint)FillMode.Solid);
            }
        }

        /// <summary>
        /// Creates a new instance of type PointCloudSurfaceEffect.
        /// </summary>
        /// <param name="surfOutVertBody">List of shader code lines that make up the vertex shader "Change Surf" method body.</param>
        /// <param name="surfOutFragBody">List of shader code lines that make up the fragment shader "Change Surf" method body.</param>
        /// <param name="rendererStates">The renderer state set for this effect.</param>
        /// <param name="doRenderInstanced">Use instanced rendering for visualizing the point cloud?</param>
        public SurfaceEffectPointCloud(List<string>? surfOutVertBody, List<string>? surfOutFragBody = null, RenderStateSet? rendererStates = null, bool doRenderInstanced = false)
            : base(new EdlInput() { Albedo = new float4(.5f, 0f, .5f, 1f) },
                  RenderFlags.PointCloud | (doRenderInstanced ? RenderFlags.Instanced : RenderFlags.None),
                  surfOutVertBody,
                  surfOutFragBody,
                  rendererStates)
        {
            if (!doRenderInstanced)
            {
                RendererStates.SetRenderState(RenderState.FillMode, (uint)FillMode.Point);
            }
            else
            {
                RendererStates.SetRenderState(RenderState.FillMode, (uint)FillMode.Solid);
            }
        }
    }
}