using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;

namespace Fusee.Engine.Core.ShaderEffects
{
    /// <summary>
    /// The default <see cref="ShaderEffect"/>, that is used if no other ShaderEffect is found.
    /// Provides properties to change the Diffuse Color, Specular Color, Specular Intensity and Specular Shininess.
    /// </summary>
    public class ShaderEffectDefault : ShaderEffect
    {
        /// <summary>
        /// A <see cref="ShaderEffectProps"/> helps to use pre-defined shard strings. 
        /// The namespace ShaderShards provides helper methods, 
        /// that create static strings containing the appropriate shader code for the material described by the ShaderEffectProps.
        /// Note that this is not mandatory, one can write their own shader shards directly in the ShaderEffect.
        /// </summary>
        private static ShaderEffectProps _effectProps = new ShaderEffectProps()
        {
            MatProbs = new MaterialProps()
            {
                HasSpecular = true,
                HasDiffuse = true
            },
            MatType = MaterialType.Standard,
            MeshProbs = new MeshProps()
            {
                HasNormals = true,
                HasUVs = true
            }
        };

        #region Uniform Declarations

        #region Matrices

        //Matrices that are used by the shaders, keep in mind that they will be set by the SceneRenderer

        [FxParam(ShaderCategory.Vertex)]
        public float4x4 FUSEE_MV
        {
            set { SetFxParam(nameof(FUSEE_MV), value); }
            get { return GetFxParam<float4x4>(nameof(FUSEE_MV)); }
        }

        [FxParam(ShaderCategory.Vertex)]
        public float4x4 FUSEE_MVP
        {
            set { SetFxParam(nameof(FUSEE_MVP), value); }
            get { return GetFxParam<float4x4>(nameof(FUSEE_MVP)); }
        }

        [FxParam(ShaderCategory.Vertex)]
        public float4x4 FUSEE_ITMV
        {
            set { SetFxParam(nameof(FUSEE_ITMV), value); }
            get { return GetFxParam<float4x4>(nameof(FUSEE_ITMV)); }
        }

        [FxParam(ShaderCategory.Vertex)]
        public float4x4 FUSEE_IMV
        {
            set { SetFxParam(nameof(FUSEE_IMV), value); }
            get { return GetFxParam<float4x4>(nameof(FUSEE_IMV)); }
        }

        #endregion

        #region Color / Lighting

        //This region contains all user-defined uniform parameters
        //They will be added to the shader defined by "ShaderCategory" in the form of "uniform <type> <name>"

        /// <summary>
        /// The diffuse color of the this shader effect.
        /// </summary>
        [FxParam(ShaderCategory.Fragment)]
        public float4 DiffuseColor
        {
            set { SetFxParam(nameof(DiffuseColor), value); }
            get { return GetFxParam<float4>(nameof(DiffuseColor)); }
        }

        /// <summary>
        /// The specular color of this shader effect.
        /// </summary>
        [FxParam(ShaderCategory.Fragment)]
        public float4 SpecularColor
        {
            set { SetFxParam(nameof(SpecularColor), value); }
            get { return GetFxParam<float4>(nameof(SpecularColor)); }
        }

        /// <summary>
        /// The specular intensity of the shader effect.
        /// </summary>
        [FxParam(ShaderCategory.Fragment)]
        public float SpecularIntensity
        {
            set { SetFxParam(nameof(SpecularIntensity), value); }
            get { return GetFxParam<float>(nameof(SpecularIntensity)); }
        }

        /// <summary>
        /// The specular shininess of this shader effect.
        /// </summary>
        [FxParam(ShaderCategory.Fragment)]
        public float SpecularShininess
        {
            set { SetFxParam(nameof(SpecularShininess), value); }
            get { return GetFxParam<float>(nameof(SpecularShininess)); }
        }
        #endregion

        #endregion

        #region Common

        //This region contains shards that are common through all shader types.
        //Those will likely be the things defined in the header of the shaders, like the version and the precision.
        //These are marked with all the "ShaderCategory" attributes as well as the"ShardCategory.Header" attribute.
        //"ShardCategory" enables the ShaderEffect to generate the glsl source code from the shards in the appropriate order.

        /// <summary>
        /// The shader shard containing the shader version.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Header)]
        public static string Version = HeaderShard.Version300Es;

        /// <summary>
        /// The shader shard containing the float precision.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Header)]
        public static string Precision = HeaderShard.EsPrecisionHighpFloat;

        #endregion

        #region Vertex

        //This region contains shards that belong to the vertex shader.
       
        /// <summary>
        /// The shader shard containing "fu" variables (in and out parameters) like fuVertex, fuNormal etc.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public static string VertInOut = ShaderShards.Vertex.VertPropertiesShard.InAndOutParams(_effectProps);

        /// <summary>
        /// The shader shard containing the main method of the vertex shader.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Main)]
        public static string VertMain = ShaderShards.Vertex.VertMainShard.VertexMain(_effectProps);

        #endregion

        #region Fragment        

        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public static string LightStruct = ShaderShards.Fragment.LightingShard.LightStructDeclaration;

        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public static string PxLightArray = ShaderShards.Fragment.FragPropertiesShard.FixedNumberLightArray;

        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public static string PxInOut = ShaderShards.Fragment.FragPropertiesShard.InParams(_effectProps);//"fu" variables like fuVertex, fuNormal        

        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public static string PxColorOut = ShaderShards.Fragment.FragPropertiesShard.ColorOut();

        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Main)]
        public static string PxMain = ShaderShards.Fragment.FragMainShard.ForwardLighting(_effectProps);

        //Note that "AssembleLightingMethods" contains more than the main method BUT in the correct order. Therefor we do not more than one shard here.
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Method)]
        public static string PxLightingMethods = ShaderShards.Fragment.LightingShard.AssembleLightingMethods(_effectProps);

        #endregion

        /// <summary>
        /// Create a new instance of type ShaderEffectDefault
        /// </summary>
        public ShaderEffectDefault() : base()
        {
            DiffuseColor = new float4(0.5f, 0.5f, 0.5f, 1.0f);
            SpecularColor = new float4(1, 1, 1, 1);
            SpecularIntensity = 0.5f;
            SpecularShininess = 22;
        }
    }
}
