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
        /// The shader shard containing a matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Matrix)]
        public static float4x4 FUSEE_MVP = float4x4.Identity;

        /// <summary>
        /// The shader shard containing a matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Matrix)]
        public static float4x4 FUSEE_ITMV = float4x4.Identity;

        /// <summary>
        /// The shader shard containing a matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Matrix)]
        public static float4x4 FUSEE_MV = float4x4.Identity;

        /// <summary>
        /// The shader shard containing a matrix uniform which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Matrix)]
        public static float4x4 FUSEE_IMV = float4x4.Identity;

        #endregion

        /// <summary>
        /// The shader shard containing the main method of the vertex shader.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Main)]
        public static string VertMain;

        /// <summary>
        /// Create a new instance of type ShaderEffectDefault
        /// </summary>
        public DefaultSurfaceEffect(LightingSetup lightingSetup, ColorInput input, List<string> surfOutBody, RenderStateSet rendererStates = null)
            : base(lightingSetup, input, rendererStates)
        {
            VertMain = ShaderShards.Vertex.VertMain.VertexMain(lightingSetup);
            SurfOutMethodBody = new List<string>()
            {
               $"{SurfaceOut.StructName} OUT = {SurfaceOut.SurfOutVaryingName};"
            };
            SurfOutMethodBody.InsertRange(1, surfOutBody);
            SurfOutMethod = ShaderShards.Fragment.FragShards.GetChangeSurfFragMethod(SurfOutMethodBody, input.GetType());
            HandleFieldsAndProps();
        }
    }
}
