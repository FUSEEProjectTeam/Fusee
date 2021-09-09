using Fusee.Engine.Common;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// Collection of uniform parameter names, as they should be used in the Shader Shards to make them compatible to each other.
    /// </summary>
    public static class UniformNameDeclarations
    {
        /// <summary>
        /// The array postfix as we get it for uniforms of array types, if we call gl.GetShaderParamList().
        /// </summary>
        public const string ArrayPostfix = "[0]";
        public static int ArrayPostfixHash = ArrayPostfix.GetHashCode();

        #region Fusee internal

        /// <summary>
        /// The vertex attribute name.
        /// </summary>
        public const string Vertex = "fuVertex";
        public static int VertexHash = Vertex.GetHashCode();
        /// <summary>
        /// The color attribute name.
        /// </summary>
        public const string VertexColor = "fuColor";
        public static int VertexColorHash = VertexColor.GetHashCode();

        /// <summary>
        /// The normal attribute name.
        /// </summary>
        public const string Normal = "fuNormal";
        public static int NormalHash = Normal.GetHashCode();

        /// <summary>
        /// The uv attribute name.
        /// </summary>
        public const string TextureCoordinates = "fuUV";
        public static int TextureCoordinatesHash = TextureCoordinates.GetHashCode();

        /// <summary>
        /// The tangent attribute name.
        /// </summary>
        public const string Tangent = "fuTangent";
        public static int TangentHash = Tangent.GetHashCode();

        /// <summary>
        /// The bitangent attribute name.
        /// </summary>
        public const string Bitangent = "fuBitangent";
        public static int BitangentHash = Bitangent.GetHashCode();

        /// <summary>
        /// The bone weight attribute name.
        /// </summary>
        public const string BoneWeight = "fuBoneWeight";
        public static int BoneWeightHash = BoneWeight.GetHashCode();

        /// <summary>
        /// The bone index attribute name.
        /// </summary>
        public const string BoneIndex = "fuBoneIndex";
        public static int BoneIndexHash = BoneIndex.GetHashCode();

        /// <summary>
        /// The model matrix. Transforms from model into world space.
        /// </summary>
        public const string Model = "FUSEE_M";
        public static int ModelHash = Model.GetHashCode();

        /// <summary>
        /// The view matrix. Transforms from world into camera space.
        /// </summary>
        public const string View = "FUSEE_V";
        public static int ViewHash = View.GetHashCode();

        /// <summary>
        /// The model view matrix. Transforms from model into camera space.
        /// </summary>
        public const string ModelView = "FUSEE_MV";
        public static int ModelViewHash = ModelView.GetHashCode();

        /// <summary>
        /// The model view matrix. Transforms from view into clip space.
        /// </summary>
        public const string Projection = "FUSEE_P";
        public static int ProjectionHash = Projection.GetHashCode();

        /// <summary>
        /// The model view projection matrix. Transforms from model into clip space.
        /// </summary>
        public const string ModelViewProjection = "FUSEE_MVP";
        public static int ModelViewProjectionHash = ModelViewProjection.GetHashCode();

        /// <summary>
        /// The inverse view matrix.
        /// </summary>
        public const string IView = "FUSEE_IV";
        public static int IViewHash = IView.GetHashCode();

        /// <summary>
        /// The inverse view matrix.
        /// </summary>
        public const string IModel = "FUSEE_IM";
        public static int IModelHash = IModel.GetHashCode();

        /// <summary>
        /// The inverse view matrix.
        /// </summary>
        public const string TModel = "FUSEE_TM";
        public static int TModelHash = TModel.GetHashCode();

        /// <summary>
        /// The inverse model view matrix.
        /// </summary>
        public const string IModelView = "FUSEE_IMV";
        public static int IModelViewHash = IModelView.GetHashCode();

        /// <summary>
        /// The inverse transposed view matrix.
        /// </summary>
        public const string TView = "FUSEE_TV";
        public static int TViewHash = TView.GetHashCode();

        /// <summary>
        /// The inverse transposed view matrix.
        /// </summary>
        public const string ITView = "FUSEE_ITV";
        public static int ITViewHash = ITView.GetHashCode();

        /// <summary>
        /// The inverse projection matrix.
        /// </summary>
        public const string IProjection = "FUSEE_IP";
        public static int IProjectionHash = IProjection.GetHashCode();

        /// <summary>
        /// The inverse model view projection matrix.
        /// </summary>
        public const string IModelViewProjection = "FUSEE_IMVP";
        public static int IModelViewProjectionHash = IModelViewProjection.GetHashCode();

        /// <summary>
        /// The transposed model view matrix.
        /// </summary>
        public const string TModelView = "FUSEE_TMV";
        public static int TModelViewHash = TModelView.GetHashCode();

        /// <summary>
        /// The transposed projection matrix.
        /// </summary>
        public const string TProjection = "FUSEE_TP";
        public static int TProjectionHash = TProjection.GetHashCode();

        /// <summary>
        /// The transposed model view projection matrix.
        /// </summary>
        public const string TModelViewProjection = "FUSEE_TMVP";
        public static int TModelViewProjectionHash = TModelViewProjection.GetHashCode();

        /// <summary>
        /// The inversed transposed model view matrix.
        /// </summary>
        public const string ITModelView = "FUSEE_ITMV";
        public static int ITModelViewHash = ITModelView.GetHashCode();

        /// <summary>
        /// The inversed transposed projection matrix.
        /// </summary>
        public const string ITProjection = "FUSEE_ITP";
        public static int ITProjectionHash = ITProjection.GetHashCode();

        /// <summary>
        /// The inversed transposed model view projection matrix.
        /// </summary>
        public const string ITModelViewProjection = "FUSEE_ITMVP";
        public static int ITModelViewProjectionHash = ITModelViewProjection.GetHashCode();

        /// <summary>
        /// The inversed transposed model view projection matrix.
        /// </summary>
        public const string ITModel = "FUSEE_ITM";
        public static int ITModelHash = ITModel.GetHashCode();

        /// <summary>
        /// The bones array.
        /// </summary>
        public const string Bones = "FUSEE_BONES";
        public static int BonesHash = Bones.GetHashCode();

        /// <summary>
        /// The bones array including the postfix.
        /// </summary>
        public const string BonesArray = Bones + ArrayPostfix;
        public static int BonesArrayHash = BonesArray.GetHashCode();

        /// <summary>
        /// The bones array including the postfix.
        /// </summary>
        public const string FuseePlatformId = "FUSEE_PLATFORM_ID";
        public static int FuseePlatformIdHash = FuseePlatformId.GetHashCode();

        #endregion

        #region SSAO

        /// <summary>
        /// The var name for the uniform SSAOKernel[0] variable.
        /// </summary>
        public static string SSAOKernel { get; } = "SSAOKernel[0]";
        public static int SSAOKernelHash = SSAOKernel.GetHashCode();

        /// <summary>
        /// The var name for the uniform NoiseTex variable, needed to calculate SSAO.
        /// </summary>
        public static string NoiseTex { get; } = "NoiseTex";
        public static int NoiseTexHash = NoiseTex.GetHashCode();

        /// <summary>
        /// The var name for the uniform SsaoOn variable.
        /// </summary>
        public static string SsaoOn { get; } = "SsaoOn";
        public static int SsaoOnHash = SsaoOn.GetHashCode();

        #endregion

        #region Shadow mapping

        /// <summary>
        /// The var name for the uniform LightSpaceMatrix.
        /// </summary>
        public const string LightPos = "LightPos";
        public static int LightPosHash = LightPos.GetHashCode();

        /// <summary>
        /// The var name for the uniform LightSpaceMatrix.
        /// </summary>
        public const string LightMatClipPlanes = "LightMatClipPlanes";
        public static int LightMatClipPlanesHash = LightMatClipPlanes.GetHashCode();

        /// <summary>
        /// The var name for the uniform LightSpaceMatrix.
        /// </summary>
        public const string LightSpaceMatrix = "LightSpaceMatrix";
        public static int LightSpaceMatrixHash = LightSpaceMatrix.GetHashCode();

        /// <summary>
        /// The var name for the uniform LightSpaceMatrix.
        /// </summary>
        public const string LightSpaceMatrices = "LightSpaceMatrices" + ArrayPostfix;
        public static int LightSpaceMatricesHash = LightSpaceMatrices.GetHashCode();

        /// <summary>
        /// The var name for the uniform ShadowMap.
        /// </summary>
        public const string ShadowMap = "ShadowMap";
        public static int ShadowMapHash = ShadowMap.GetHashCode();

        /// <summary>
        /// The var name for the uniform ShadowCubeMap.
        /// </summary>
        public const string ShadowCubeMap = "ShadowCubeMap";
        public static int ShadowCubeMapHash = ShadowCubeMap.GetHashCode();

        #endregion

        #region Point Cloud

        public const string PointSize = "PointSize";
        public static int PointSizeHash = PointSize.GetHashCode();

        #endregion

        /// <summary>
        /// The var name for the uniform PassNo variable.
        /// </summary>
        public const string RenderPassNo = "PassNo";
        public static int RenderPassNoHash = RenderPassNo.GetHashCode();

        /// <summary>
        /// The var name for the uniform BackgroundColor.
        /// </summary>
        public const string BackgroundColor = "BackgroundColor";
        public static int BackgroundColorHash = BackgroundColor.GetHashCode();

        /// <summary>
        /// The var name for the uniform ScreenParams (width and height of the window).
        /// </summary>
        public const string ScreenParams = "ScreenParams";
        public static int ScreenParamsHash = ScreenParams.GetHashCode();

        /// <summary>
        /// The var name for the uniform AmbientStrength variable within the pixel shaders.
        /// </summary>
        public const string AmbientStrength = "AmbientStrength";
        public static int AmbientStrengthHash = AmbientStrength.GetHashCode();

        /// <summary>
        /// The var name for the uniform DiffuseColor variable within the pixel shaders.
        /// </summary>
        public const string Albedo = "Albedo";
        public static int AlbedoHash = Albedo.GetHashCode();

        /// <summary>
        /// The var name for the uniform SpecularColor variable within the pixel shaders.
        /// </summary>
        public const string SpecularColor = "SpecularColor";
        public static int SpecularColorHash = SpecularColor.GetHashCode();

        /// <summary>
        /// The var name for the uniform EmissiveColor variable within the pixel shaders.
        /// </summary>
        public const string EmissiveColor = "EmissiveColor";
        public static int EmissiveColorHash = EmissiveColor.GetHashCode();

        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixel shaders.
        /// </summary>
        public const string AlbedoTexture = "AlbedoTexture";
        public static int AlbedoTextureHash = AlbedoTexture.GetHashCode();

        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixel shaders.
        /// </summary>
        public const string DiffuseTextureTiles = "AlbedoTextureTiles";
        public static int DiffuseTextureTilesHash = DiffuseTextureTiles.GetHashCode();

        /// <summary>
        /// The var name for the uniform SpecularTexture variable within the pixel shaders.
        /// </summary>
        public const string SpecularTexture = "SpecularTexture";
        public static int SpecularTextureHash = SpecularTexture.GetHashCode();

        /// <summary>
        /// The var name for the uniform EmissiveTexture variable within the pixel shaders.
        /// </summary>
        public const string EmissiveTexture = "EmissiveTexture";
        public static int EmissiveTextureHash = EmissiveTexture.GetHashCode();

        /// <summary>
        /// The var name for the uniform NormalMap variable within the pixel shaders.
        /// </summary>
        public const string NormalMap = "NormalMap";
        public static int NormalMapHash = NormalMap.GetHashCode();

        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixel shaders.
        /// </summary>
        public const string NormalTextureTiles = "NormalTextureTiles";
        public static int NormalTextureTilesHash = NormalTextureTiles.GetHashCode();

        /// <summary>
        /// The var name for the uniform DiffuseMix variable within the pixel shaders.
        /// </summary>
        public const string AlbedoMix = "AlbedoMix";
        public static int AlbedoMixHash = AlbedoMix.GetHashCode();

        /// <summary>
        /// The var name for the uniform SpecularMix variable within the pixel shaders.
        /// </summary>
        public const string SpecularMix = "SpecularMix";
        public static int SpecularMixHash = SpecularMix.GetHashCode();

        /// <summary>
        /// The var name for the uniform EmissiveMix variable within the pixel shaders.
        /// </summary>
        public const string EmissiveMix = "EmissiveMix";
        public static int EmissiveMixHash = EmissiveMix.GetHashCode();

        /// <summary>
        /// The var name for the uniform SpecularShininess variable within the pixel shaders.
        /// </summary>
        public const string SpecularShininess = "SpecularShininess";
        public static int SpecularShininessHash = SpecularShininess.GetHashCode();

        /// <summary>
        /// The var name for the uniform SpecularIntensity variable within the pixel shaders.
        /// </summary>
        public const string SpecularStrength = "SpecularStrength";
        public static int SpecularStrengthHash = SpecularStrength.GetHashCode();

        /// <summary>
        /// [PBR (Cook-Torrance) only] Describes the roughness of the material.
        /// </summary>
        public const string RoughnessValue = "RoughnessValue";
        public static int RoughnessValueHash = RoughnessValue.GetHashCode();

        /// <summary>
        /// The var name for the uniform NormalMapIntensity variable within the pixel shaders.
        /// </summary>
        public const string NormalMapIntensity = "NormalMapIntensity";
        public static int NormalMapIntensityHash = NormalMapIntensity.GetHashCode();


        /// <summary>
        /// List of all possible render texture names.
        /// </summary>
        public static readonly List<string> DeferredRenderTextures = new()
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