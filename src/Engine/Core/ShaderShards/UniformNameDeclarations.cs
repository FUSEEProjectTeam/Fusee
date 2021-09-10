using Fusee.Engine.Common;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// Collection of uniform parameter names, as they should be used in the Shader Shards to make them compatible to each other.
    /// </summary>
    public sealed class UniformNameDeclarations
    {
        // Explicit static readonlyructor to tell C# compiler
        // not to mark type as beforefieldinit
        static UniformNameDeclarations() { }

        private UniformNameDeclarations() 
        {
            LightSpaceMatrices = "LightSpaceMatrices" + ArrayPostfix;
            BonesArray = Bones + ArrayPostfix;

            ModelHash = Model.GetHashCode();
            ViewHash = View.GetHashCode();
            ModelViewHash = ModelView.GetHashCode();
            ProjectionHash= Projection.GetHashCode();
            ModelViewProjectionHash = ModelViewProjection.GetHashCode();
            IViewHash = IView.GetHashCode();
            IModelHash = IModel.GetHashCode();
            TModelHash = TModel.GetHashCode();
            IModelViewHash = IModelView.GetHashCode();
            TViewHash = TView.GetHashCode();
            ITViewHash = ITView.GetHashCode();
            ITModelHash = ITModel.GetHashCode();
            IProjectionHash = IProjection.GetHashCode();
            TModelViewHash = TModelView.GetHashCode();
            TProjectionHash = TProjection.GetHashCode();
            TModelViewProjectionHash = TModelViewProjection.GetHashCode();
            ITModelViewHash = ITModelView.GetHashCode();
            ITProjectionHash = ITProjection.GetHashCode();
            ITModelViewProjectionHash = ITModelViewProjection.GetHashCode();
            IModelViewProjectionHash = IModelViewProjection.GetHashCode();
            BonesHash = Bones.GetHashCode();
            BonesArrayHash = BonesArray.GetHashCode();
            FuseePlatformIdHash = FuseePlatformId.GetHashCode();
            RenderPassNoHash = RenderPassNo.GetHashCode();
            BackgroundColorHash = BackgroundColor.GetHashCode();
            SsaoOnHash = SsaoOn.GetHashCode();
            LightSpaceMatrixHash = LightSpaceMatrix.GetHashCode();
            LightSpaceMatricesHash = LightSpaceMatrices.GetHashCode();
            LightMatClipPlanesHash = LightMatClipPlanes.GetHashCode();
            LightPosHash= LightPos.GetHashCode();
            ShadowMapHash = ShadowMap.GetHashCode();
            ShadowCubeMapHash = ShadowCubeMap.GetHashCode();
        }

        public static UniformNameDeclarations Instance => _instance;
        private static readonly UniformNameDeclarations _instance = new();

        /// <summary>
        /// The standard name for the fragment shader color output.
        /// </summary>
        public string OutColorName = "oFragmentColor";

        /// <summary>
        /// The array postfix as we get it for uniforms of array types, if we call gl.GetShaderParamList().
        /// </summary>
        public readonly string ArrayPostfix = "[0]";

        /// <summary>
        /// Struct, that describes a Light object in the shader code./>
        /// </summary>
        /// <returns></returns>
        public readonly string LightStructDeclaration = string.Join("\n", new List<string>() {

        "struct Light",
        "{",
        "   vec3 position;",
        "   vec4 intensities;",
        "   vec3 direction;",
        "   float maxDistance;",
        "   float strength;",
        "   float outerConeAngle;",
        "   float innerConeAngle;",
        "   int lightType;",
        "   int isActive;",
        "   int isCastingShadows;",
        "   float bias;",
        "};\n",
        });

        /// <summary>
        /// Creates the "allLights" uniform array, as it is used in forward rendering.
        /// </summary>
        /// <returns></returns>
        public readonly string AllLights = "allLights";

        #region Fusee internal

        /// <summary>
        /// The vertex attribute name.
        /// </summary>
        public readonly string Vertex = "fuVertex";

        /// <summary>
        /// The color attribute name.
        /// </summary>
        public readonly string VertexColor = "fuColor";

        /// <summary>
        /// The normal attribute name.
        /// </summary>
        public readonly string Normal = "fuNormal";

        /// <summary>
        /// The uv attribute name.
        /// </summary>
        public readonly string TextureCoordinates = "fuUV";

        /// <summary>
        /// The tangent attribute name.
        /// </summary>
        public readonly string Tangent = "fuTangent";

        /// <summary>
        /// The bitangent attribute name.
        /// </summary>
        public readonly string Bitangent = "fuBitangent";

        /// <summary>
        /// The bone weight attribute name.
        /// </summary>
        public readonly string BoneWeight = "fuBoneWeight";

        /// <summary>
        /// The bone index attribute name.
        /// </summary>
        public readonly string BoneIndex = "fuBoneIndex";

        /// <summary>
        /// The model matrix. Transforms from model into world space.
        /// </summary>
        public readonly string Model = "FUSEE_M";

        /// <summary>
        /// Hash for the model matrix parameter name.
        /// </summary>
        internal readonly int ModelHash;

        /// <summary>
        /// The view matrix. Transforms from world into camera space.
        /// </summary>
        public readonly string View = "FUSEE_V";

        /// <summary>
        /// Hash for the view matrix parameter name.
        /// </summary>
        internal readonly int ViewHash;

        /// <summary>
        /// The model view matrix. Transforms from model into camera space.
        /// </summary>
        public readonly string ModelView = "FUSEE_MV";

        /// <summary>
        /// Hash for the model view matrix parameter name.
        /// </summary>
        internal readonly int ModelViewHash;

        /// <summary>
        /// The projection matrix. Transforms from view into clip space.
        /// </summary>
        public readonly string Projection = "FUSEE_P";

        /// <summary>
        /// Hash for the projection matrix parameter name.
        /// </summary>
        internal readonly int ProjectionHash;

        /// <summary>
        /// The model view projection matrix. Transforms from model into clip space.
        /// </summary>
        public readonly string ModelViewProjection = "FUSEE_MVP";

        /// <summary>
        /// Hash for the model view projection matrix parameter name.
        /// </summary>
        internal readonly int ModelViewProjectionHash;

        /// <summary>
        /// The inverse view matrix.
        /// </summary>
        public readonly string IView = "FUSEE_IV";

        /// <summary>
        /// Hash for the inverse view matrix parameter name.
        /// </summary>
        internal readonly int IViewHash;

        /// <summary>
        /// The inverse model matrix.
        /// </summary>
        public readonly string IModel = "FUSEE_IM";

        /// <summary>
        /// Hash for the inverse model matrix parameter name.
        /// </summary>
        internal readonly int IModelHash;

        /// <summary>
        /// The transposed model matrix.
        /// </summary>
        public readonly string TModel = "FUSEE_TM";

        /// <summary>
        /// Hash for the transposed model matrix parameter name.
        /// </summary>
        internal readonly int TModelHash;

        /// <summary>
        /// The inverse model view matrix.
        /// </summary>
        public readonly string IModelView = "FUSEE_IMV";

        /// <summary>
        /// Hash for the inverse model view matrix parameter name.
        /// </summary>
        internal readonly int IModelViewHash;

        /// <summary>
        /// The inverse transposed view matrix.
        /// </summary>
        public readonly string TView = "FUSEE_TV";

        /// <summary>
        /// Hash for the transposed view matrix parameter name.
        /// </summary>
        internal readonly int TViewHash;

        /// <summary>
        /// The inverse transposed view matrix.
        /// </summary>
        public readonly string ITView = "FUSEE_ITV";

        /// <summary>
        /// Hash for inverse transposed view matrix parameter name.
        /// </summary>
        internal readonly int ITViewHash;

        /// <summary>
        /// The inverse projection matrix.
        /// </summary>
        public readonly string IProjection = "FUSEE_IP";

        /// <summary>
        /// Hash for the inverse projection matrix parameter name.
        /// </summary>
        internal readonly int IProjectionHash;

        /// <summary>
        /// The inverse model view projection matrix.
        /// </summary>
        public readonly string IModelViewProjection = "FUSEE_IMVP";

        /// <summary>
        /// Hash for the inverse model view projection matrix parameter name.
        /// </summary>
        internal readonly int IModelViewProjectionHash;

        /// <summary>
        /// The transposed model view matrix.
        /// </summary>
        public readonly string TModelView = "FUSEE_TMV";

        /// <summary>
        /// Hash for the transposed model view matrix parameter name.
        /// </summary>
        internal readonly int TModelViewHash;

        /// <summary>
        /// The transposed projection matrix.
        /// </summary>
        public readonly string TProjection = "FUSEE_TP";

        /// <summary>
        /// Hash for the transposed projection matrix parameter name.
        /// </summary>
        internal readonly int TProjectionHash;

        /// <summary>
        /// The transposed model view projection matrix.
        /// </summary>
        public readonly string TModelViewProjection = "FUSEE_TMVP";

        /// <summary>
        /// Hash for the transposed model view projection matrix parameter name.
        /// </summary>
        internal readonly int TModelViewProjectionHash;

        /// <summary>
        /// The inversed transposed model view matrix.
        /// </summary>
        public readonly string ITModelView = "FUSEE_ITMV";

        /// <summary>
        /// Hash for the transposed model view matrix parameter name.
        /// </summary>
        internal readonly int ITModelViewHash;

        /// <summary>
        /// The inversed transposed projection matrix.
        /// </summary>
        public readonly string ITProjection = "FUSEE_ITP";

        /// <summary>
        /// Hash for the nversed transposed projection matrix parameter name.
        /// </summary>
        internal readonly int ITProjectionHash;

        /// <summary>
        /// The inversed transposed model view projection matrix.
        /// </summary>
        public readonly string ITModelViewProjection = "FUSEE_ITMVP";

        /// <summary>
        /// Hash for the transposed model view projection matrix parameter name.
        /// </summary>
        internal readonly int ITModelViewProjectionHash;

        /// <summary>
        /// The inversed transposed model view matrix.
        /// </summary>
        public readonly string ITModel = "FUSEE_ITM";

        /// <summary>
        /// Hash for the transposed model view matrix parameter name.
        /// </summary>
        internal readonly int ITModelHash;

        /// <summary>
        /// The bones array.
        /// </summary>
        public readonly string Bones = "FUSEE_BONES";

        /// <summary>
        /// Hash for the bones array parameter name.
        /// </summary>
        internal readonly int BonesHash;

        /// <summary>
        /// The bones array including the postfix.
        /// </summary>
        public readonly string BonesArray;

        /// <summary>
        /// Hash for the bones array parameter name.
        /// </summary>
        internal readonly int BonesArrayHash;

        /// <summary>
        /// The platform id.
        /// </summary>
        public readonly string FuseePlatformId = "FUSEE_PLATFORM_ID";

        /// <summary>
        /// Hash for the platform id parameter name.
        /// </summary>
        internal readonly int FuseePlatformIdHash;

        #endregion

        #region SSAO

        /// <summary>
        /// The var name for the uniform SSAOKernel[0] variable.
        /// </summary>
        public readonly string SSAOKernel = "SSAOKernel[0]";

        /// <summary>
        /// The var name for the uniform NoiseTex variable, needed to calculate SSAO.
        /// </summary>
        public readonly string NoiseTex = "NoiseTex";

        /// <summary>
        /// The var name for the uniform SsaoOn variable.
        /// </summary>
        public readonly string SsaoOn = "SsaoOn";
        internal readonly int SsaoOnHash;

        #endregion

        #region Shadow mapping

        /// <summary>
        /// The var name for the uniform LightSpaceMatrix.
        /// </summary>
        public readonly string LightPos = "LightPos";
        internal readonly int LightPosHash;

        /// <summary>
        /// The var name for the uniform LightSpaceMatrix.
        /// </summary>
        public readonly string LightMatClipPlanes = "LightMatClipPlanes";
        internal readonly int LightMatClipPlanesHash;

        /// <summary>
        /// The var name for the uniform LightSpaceMatrix.
        /// </summary>
        public readonly string LightSpaceMatrix = "LightSpaceMatrix";
        internal readonly int LightSpaceMatrixHash;

        /// <summary>
        /// The var name for the uniform LightSpaceMatrix.
        /// </summary>
        public readonly string LightSpaceMatrices;
        internal readonly int LightSpaceMatricesHash;

        /// <summary>
        /// The var name for the uniform ShadowMap.
        /// </summary>
        public readonly string ShadowMap = "ShadowMap";
        internal readonly int ShadowMapHash;

        /// <summary>
        /// The var name for the uniform ShadowCubeMap.
        /// </summary>
        public readonly string ShadowCubeMap = "ShadowCubeMap";
        internal readonly int ShadowCubeMapHash;

        #endregion

        #region Point Cloud

        /// <summary>
        /// The name for the uniform variable PointSize.
        /// </summary>
        public readonly string PointSize = "PointSize";

        #endregion

        /// <summary>
        /// The var name for the uniform PassNo variable.
        /// </summary>
        public readonly string RenderPassNo = "PassNo";
        internal readonly int RenderPassNoHash;

        /// <summary>
        /// The var name for the uniform BackgroundColor.
        /// </summary>
        public readonly string BackgroundColor = "BackgroundColor";
        internal readonly int BackgroundColorHash;

        /// <summary>
        /// The var name for the uniform ScreenParams (width and height of the window).
        /// </summary>
        public readonly string ScreenParams = "ScreenParams";

        /// <summary>
        /// The var name for the uniform AmbientStrength variable within the pixel shaders.
        /// </summary>
        public readonly string AmbientStrength = "AmbientStrength";

        /// <summary>
        /// The var name for the uniform DiffuseColor variable within the pixel shaders.
        /// </summary>
        public readonly string Albedo = "Albedo";

        /// <summary>
        /// The var name for the uniform SpecularColor variable within the pixel shaders.
        /// </summary>
        public readonly string SpecularColor = "SpecularColor";

        /// <summary>
        /// The var name for the uniform EmissiveColor variable within the pixel shaders.
        /// </summary>
        public readonly string EmissiveColor = "EmissiveColor";

        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixel shaders.
        /// </summary>
        public readonly string AlbedoTexture = "AlbedoTexture";

        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixel shaders.
        /// </summary>
        public readonly string DiffuseTextureTiles = "AlbedoTextureTiles";

        /// <summary>
        /// The var name for the uniform SpecularTexture variable within the pixel shaders.
        /// </summary>
        public readonly string SpecularTexture = "SpecularTexture";

        /// <summary>
        /// The var name for the uniform EmissiveTexture variable within the pixel shaders.
        /// </summary>
        public readonly string EmissiveTexture = "EmissiveTexture";

        /// <summary>
        /// The var name for the uniform NormalMap variable within the pixel shaders.
        /// </summary>
        public readonly string NormalMap = "NormalMap";

        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixel shaders.
        /// </summary>
        public readonly string NormalTextureTiles = "NormalTextureTiles";

        /// <summary>
        /// The var name for the uniform DiffuseMix variable within the pixel shaders.
        /// </summary>
        public readonly string AlbedoMix = "AlbedoMix";

        /// <summary>
        /// The var name for the uniform SpecularMix variable within the pixel shaders.
        /// </summary>
        public readonly string SpecularMix = "SpecularMix";

        /// <summary>
        /// The var name for the uniform EmissiveMix variable within the pixel shaders.
        /// </summary>
        public readonly string EmissiveMix = "EmissiveMix";

        /// <summary>
        /// The var name for the uniform SpecularShininess variable within the pixel shaders.
        /// </summary>
        public readonly string SpecularShininess = "SpecularShininess";

        /// <summary>
        /// The var name for the uniform SpecularIntensity variable within the pixel shaders.
        /// </summary>
        public readonly string SpecularStrength = "SpecularStrength";

        /// <summary>
        /// [PBR (Cook-Torrance) only] Describes the roughness of the material.
        /// </summary>
        public readonly string RoughnessValue = "RoughnessValue";

        /// <summary>
        /// The var name for the uniform NormalMapIntensity variable within the pixel shaders.
        /// </summary>
        public readonly string NormalMapIntensity = "NormalMapIntensity";

        /// <summary>
        /// List of all possible render texture names.
        /// </summary>
        public readonly List<string> DeferredRenderTextures = new()
        {
            Enum.GetName(typeof(RenderTargetTextureTypes), 0),
            Enum.GetName(typeof(RenderTargetTextureTypes), 1),
            Enum.GetName(typeof(RenderTargetTextureTypes), 2),
            Enum.GetName(typeof(RenderTargetTextureTypes), 3),
            Enum.GetName(typeof(RenderTargetTextureTypes), 4),
            Enum.GetName(typeof(RenderTargetTextureTypes), 5),
            Enum.GetName(typeof(RenderTargetTextureTypes), 6),
        };
    }
}