using Fusee.Engine.Core.ShaderShards;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// The default <see cref="ShaderEffect"/>, that is used if no other ShaderEffect is found.
    /// Provides properties to change the Diffuse Color, Specular Color, Specular Intensity and Specular Shininess.
    /// </summary>
    public class DefaultSurfaceEffect : SurfaceEffect
    {
        /// <summary>
        /// A <see cref="EffectProps"/> helps to use pre-defined shard strings. 
        /// The namespace ShaderShards provides helper methods, 
        /// that create static strings containing the appropriate shader code for the material described by the ShaderEffectProps.
        /// Note that this is not mandatory, one can write their own shader shards directly in the ShaderEffect.
        /// </summary>
        private static EffectProps _effectProps = new EffectProps()
        {
            LightingProps = new LightingProps()
            {
                SpecularLighting = SpecularLighting.Std,
                DoDiffuseLighting = true
            },
            MeshProbs = new MeshProps()
            {
                HasNormals = true,
                HasUVs = true
            }
        };

        #region struct declaration

        #endregion

        #region Uniform Declarations

        //This region contains all user-defined uniform parameters
        //They will be added to the shader defined by "ShaderCategory" in the form of "uniform <type> <name>"        

        #endregion

        #region Common

        //This region contains shards that are common through all shader types.
        //Those will likely be the things defined in the header of the shaders, like the version and the precision.
        //These are marked with all the "ShaderCategory" attributes as well as the"ShardCategory.Header" attribute.
        //"ShardCategory" enables the ShaderEffect to generate the glsl source code from the shards in the appropriate order.

        #endregion

        #region Vertex

        //This region contains shards that belong to the vertex shader.

        /// <summary>
        /// The shader shard containing the matrix uniform parameters which should NOT be settable via property because they get updated internally.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public static string FuseeVertUniforms = ShaderShards.Vertex.VertProperties.FuseeMatUniforms(_effectProps);

        /// <summary>
        /// The shader shard containing "fu" variables (in and out parameters) like fuVertex, fuNormal etc.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public static string VertInOut = ShaderShards.Vertex.VertProperties.InAndOutParams(_effectProps);

        /// <summary>
        /// The shader shard containing the main method of the vertex shader.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Main)]
        public static string VertMain = ShaderShards.Vertex.VertMain.VertexMain(_effectProps);

        #endregion

        #region Fragment

        //This region contains shards that belong to the pixel shader.
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public static string FuseeMat = ShaderShards.Fragment.FragProperties.FuseeMatrixUniforms();

        

        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public static string PxInOut = ShaderShards.Fragment.FragProperties.InParams(_effectProps);//"fu" variables like fuVertex, fuNormal

        // -------------- Needs to go into the RenderContext if this effect is to be used for forward and deferred -------

        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public static string LightStruct = ShaderShards.Fragment.Lighting.LightStructDeclaration;

        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public static string PxLightArray = ShaderShards.Fragment.FragProperties.FixedNumberLightArray;

        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public static string PxColorOut = ShaderShards.Fragment.FragProperties.ColorOut();

        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Main)]
        public static string PxMain = ShaderShards.Fragment.FragMain.ForwardLighting(typeof(SpecularInput), "SurfIn", ShaderSurfaceOut.GetLightingSetupShards(LightingSetup.SpecularStd).Name);

        //Note that "AssembleLightingMethods" contains more than the main method BUT in the correct order. Therefor we do not more than one shard here.
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Method)]
        public static string PxLightingMethods;

        //----------------------------------------------------------------------------------------------------------------

        #endregion

        /// <summary>
        /// Create a new instance of type ShaderEffectDefault
        /// </summary>
        public DefaultSurfaceEffect(LightingSetup lightingSetup, SpecularInput specularIn, RenderStateSet rendererStates = null) 
            : base(lightingSetup, specularIn, rendererStates) 
        {

            PxLightingMethods = ShaderShards.Fragment.Lighting.AssembleLightingMethods(LightingSetup);

            var surfOutBody = new List<string>()
            {
                "OUT.albedo = IN.Albedo;",
                "OUT.specularStrength = IN.SpecularStrength;",
                "OUT.shininess = IN.Shininess;",
                $"OUT.normal = {VaryingNameDeclarations.Normal};",
                $"OUT.position = {VaryingNameDeclarations.Position};",
            };

            SurfOutMethodBody.InsertRange(1, surfOutBody);

            SurfOutMethod = ShaderShards.Fragment.FragShards.GetChangeSurfFragMethod(LightingSetup, SurfOutMethodBody, typeof(SpecularInput));

            HandleFieldsAndProps();
        }
    }
}
