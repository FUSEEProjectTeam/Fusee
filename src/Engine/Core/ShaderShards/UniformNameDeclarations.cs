using Fusee.Engine.Common;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// Collection of uniform parameter names, as they should be used in the Shader Shards to make them compatible to each other.
    /// A hash code is assigned to each parameter, which is used to identify it in the <see cref="RenderContext"/>.
    /// </summary>
    public static class UniformNameDeclarations
    {
        /// <summary>
        /// The array postfix as we get it for uniforms of array types, if we call gl.GetShaderParamList().
        /// </summary>
        public const string ArrayPostfix = "[0]";
        /// <summary>
        /// Hash code for the <see cref="ArrayPostfix"/> parameter.
        /// </summary>
        public static int ArrayPostfixHash = ArrayPostfix.GetHashCode();

        #region Fusee internal

        /// <summary>
        /// The vertex attribute name.
        /// </summary>
        public const string Vertex = "fuVertex";
        /// <summary>
        /// Hash code for the <see cref="Vertex"/> parameter.
        /// </summary>
        public static int VertexHash = Vertex.GetHashCode();
        /// <summary>
        /// The color attribute name.
        /// </summary>
        public const string VertexColor = "fuColor";
        /// <summary>
        /// Hash code for the <see cref="VertexColor"/> parameter.
        /// </summary>
        public static int VertexColorHash = VertexColor.GetHashCode();

        /// <summary>
        /// The color attribute name.
        /// </summary>
        public const string VertexColor1 = "fuColor1";
        /// <summary>
        /// Hash code for the <see cref="VertexColor"/> parameter.
        /// </summary>
        public static int VertexColor1Hash = VertexColor1.GetHashCode();

        /// <summary>
        /// The color attribute name.
        /// </summary>
        public const string VertexColor2 = "fuColor2";
        /// <summary>
        /// Hash code for the <see cref="VertexColor"/> parameter.
        /// </summary>
        public static int VertexColor2Hash = VertexColor2.GetHashCode();

        /// <summary>
        /// The normal attribute name.
        /// </summary>
        public const string Normal = "fuNormal";
        /// <summary>
        /// Hash code for the <see cref="Normal"/> parameter.
        /// </summary>
        public static int NormalHash = Normal.GetHashCode();

        /// <summary>
        /// The uv attribute name.
        /// </summary>
        public const string TextureCoordinates = "fuUV";
        /// <summary>
        /// Hash code for the <see cref="TextureCoordinates"/> parameter.
        /// </summary>
        public static int TextureCoordinatesHash = TextureCoordinates.GetHashCode();

        /// <summary>
        /// The tangent attribute name.
        /// </summary>
        public const string Tangent = "fuTangent";
        /// <summary>
        /// Hash code for the <see cref="Tangent"/> parameter.
        /// </summary>
        public static int TangentHash = Tangent.GetHashCode();

        /// <summary>
        /// The bitangent attribute name.
        /// </summary>
        public const string Bitangent = "fuBitangent";
        /// <summary>
        /// Hash code for the <see cref="Bitangent"/> parameter.
        /// </summary>
        public static int BitangentHash = Bitangent.GetHashCode();

        /// <summary>
        /// The bone weight attribute name.
        /// </summary>
        public const string BoneWeight = "fuBoneWeight";
        /// <summary>
        /// Hash code for the <see cref="BoneWeight"/> parameter.
        /// </summary>
        public static int BoneWeightHash = BoneWeight.GetHashCode();

        /// <summary>
        /// The bone index attribute name.
        /// </summary>
        public const string BoneIndex = "fuBoneIndex";
        /// <summary>
        /// Hash code for the <see cref="BoneIndex"/> parameter.
        /// </summary>
        public static int BoneIndexHash = BoneIndex.GetHashCode();

        /// <summary>
        /// The model matrix. Transforms from model into world space.
        /// </summary>
        public const string Model = "FUSEE_M";
        /// <summary>
        /// Hash code for the <see cref="Model"/> parameter.
        /// </summary>
        public static int ModelHash = Model.GetHashCode();

        /// <summary>
        /// The view matrix. Transforms from world into camera space.
        /// </summary>
        public const string View = "FUSEE_V";
        /// <summary>
        /// Hash code for the <see cref="View"/> parameter.
        /// </summary>
        public static int ViewHash = View.GetHashCode();

        /// <summary>
        /// The model view matrix. Transforms from model into camera space.
        /// </summary>
        public const string ModelView = "FUSEE_MV";
        /// <summary>
        /// Hash code for the <see cref="ModelView"/> parameter.
        /// </summary>
        public static int ModelViewHash = ModelView.GetHashCode();

        /// <summary>
        /// The model view matrix. Transforms from view into clip space.
        /// </summary>
        public const string Projection = "FUSEE_P";
        /// <summary>
        /// Hash code for the <see cref="Projection"/> parameter.
        /// </summary>
        public static int ProjectionHash = Projection.GetHashCode();

        /// <summary>
        /// The model view projection matrix. Transforms from model into clip space.
        /// </summary>
        public const string ModelViewProjection = "FUSEE_MVP";
        /// <summary>
        /// Hash code for the <see cref="ModelViewProjection"/> parameter.
        /// </summary>
        public static int ModelViewProjectionHash = ModelViewProjection.GetHashCode();

        /// <summary>
        /// The inverse view matrix.
        /// </summary>
        public const string IView = "FUSEE_IV";
        /// <summary>
        /// Hash code for the <see cref="IView"/> parameter.
        /// </summary>
        public static int IViewHash = IView.GetHashCode();

        /// <summary>
        /// The inverse view matrix.
        /// </summary>
        public const string IModel = "FUSEE_IM";
        /// <summary>
        /// Hash code for the <see cref="IModel"/> parameter.
        /// </summary>
        public static int IModelHash = IModel.GetHashCode();

        /// <summary>
        /// The inverse view matrix.
        /// </summary>
        public const string TModel = "FUSEE_TM";
        /// <summary>
        /// Hash code for the <see cref="TModel"/> parameter.
        /// </summary>
        public static int TModelHash = TModel.GetHashCode();

        /// <summary>
        /// The inverse model view matrix.
        /// </summary>
        public const string IModelView = "FUSEE_IMV";
        /// <summary>
        /// Hash code for the <see cref="IModelView"/> parameter.
        /// </summary>
        public static int IModelViewHash = IModelView.GetHashCode();

        /// <summary>
        /// The inverse transposed view matrix.
        /// </summary>
        public const string TView = "FUSEE_TV";
        /// <summary>
        /// Hash code for the <see cref="TView"/> parameter.
        /// </summary>
        public static int TViewHash = TView.GetHashCode();

        /// <summary>
        /// The inverse transposed view matrix.
        /// </summary>
        public const string ITView = "FUSEE_ITV";
        /// <summary>
        /// Hash code for the <see cref="ITView"/> parameter.
        /// </summary>
        public static int ITViewHash = ITView.GetHashCode();

        /// <summary>
        /// The inverse projection matrix.
        /// </summary>
        public const string IProjection = "FUSEE_IP";
        /// <summary>
        /// Hash code for the <see cref="IProjection"/> parameter.
        /// </summary>
        public static int IProjectionHash = IProjection.GetHashCode();

        /// <summary>
        /// The inverse model view projection matrix.
        /// </summary>
        public const string IModelViewProjection = "FUSEE_IMVP";
        /// <summary>
        /// Hash code for the <see cref="IModelViewProjection"/> parameter.
        /// </summary>
        public static int IModelViewProjectionHash = IModelViewProjection.GetHashCode();

        /// <summary>
        /// The transposed model view matrix.
        /// </summary>
        public const string TModelView = "FUSEE_TMV";
        /// <summary>
        /// Hash code for the <see cref="TModelView"/> parameter.
        /// </summary>
        public static int TModelViewHash = TModelView.GetHashCode();

        /// <summary>
        /// The transposed projection matrix.
        /// </summary>
        public const string TProjection = "FUSEE_TP";
        /// <summary>
        /// Hash code for the <see cref="TProjection"/> parameter.
        /// </summary>
        public static int TProjectionHash = TProjection.GetHashCode();

        /// <summary>
        /// The transposed model view projection matrix.
        /// </summary>
        public const string TModelViewProjection = "FUSEE_TMVP";
        /// <summary>
        /// Hash code for the <see cref="TModelViewProjection"/> parameter.
        /// </summary>
        public static int TModelViewProjectionHash = TModelViewProjection.GetHashCode();

        /// <summary>
        /// The inversed transposed model view matrix.
        /// </summary>
        public const string ITModelView = "FUSEE_ITMV";
        /// <summary>
        /// Hash code for the <see cref="ITModelView"/> parameter.
        /// </summary>
        public static int ITModelViewHash = ITModelView.GetHashCode();

        /// <summary>
        /// The inversed transposed projection matrix.
        /// </summary>
        public const string ITProjection = "FUSEE_ITP";
        /// <summary>
        /// Hash code for the <see cref="ITProjection"/> parameter.
        /// </summary>
        public static int ITProjectionHash = ITProjection.GetHashCode();

        /// <summary>
        /// The inversed transposed model view projection matrix.
        /// </summary>
        public const string ITModelViewProjection = "FUSEE_ITMVP";
        /// <summary>
        /// Hash code for the <see cref="ITModelViewProjection"/> parameter.
        /// </summary>
        public static int ITModelViewProjectionHash = ITModelViewProjection.GetHashCode();

        /// <summary>
        /// The inversed transposed model view projection matrix.
        /// </summary>
        public const string ITModel = "FUSEE_ITM";
        /// <summary>
        /// Hash code for the <see cref="ITModel"/> parameter.
        /// </summary>
        public static int ITModelHash = ITModel.GetHashCode();

        /// <summary>
        /// The bones array.
        /// </summary>
        public const string Bones = "FUSEE_BONES";
        /// <summary>
        /// Hash code for the <see cref="Bones"/> parameter.
        /// </summary>
        public static int BonesHash = Bones.GetHashCode();

        /// <summary>
        /// The bones array including the postfix.
        /// </summary>
        public const string BonesArray = Bones + ArrayPostfix;
        /// <summary>
        /// Hash code for the <see cref="BonesArray"/> parameter.
        /// </summary>
        public static int BonesArrayHash = BonesArray.GetHashCode();

        /// <summary>
        /// The bones array including the postfix.
        /// </summary>
        public const string FuseePlatformId = "FUSEE_PLATFORM_ID";
        /// <summary>
        /// Hash code for the <see cref="FuseePlatformId"/> parameter.
        /// </summary>
        public static int FuseePlatformIdHash = FuseePlatformId.GetHashCode();

        #endregion

        #region SSAO

        /// <summary>
        /// The var name for the uniform SSAOKernel[0] variable.
        /// </summary>
        public static string SSAOKernel { get; } = "SSAOKernel[0]";
        /// <summary>
        /// Hash code for the <see cref="SSAOKernel"/> parameter.
        /// </summary>
        public static int SSAOKernelHash = SSAOKernel.GetHashCode();

        /// <summary>
        /// The var name for the uniform NoiseTex variable, needed to calculate SSAO.
        /// </summary>
        public static string NoiseTex { get; } = "NoiseTex";
        /// <summary>
        /// Hash code for the <see cref="NoiseTex"/> parameter.
        /// </summary>
        public static int NoiseTexHash = NoiseTex.GetHashCode();

        /// <summary>
        /// The var name for the uniform SsaoOn variable.
        /// </summary>
        public static string SsaoOn { get; } = "SsaoOn";
        /// <summary>
        /// Hash code for the <see cref="SsaoOn"/> parameter.
        /// </summary>
        public static int SsaoOnHash = SsaoOn.GetHashCode();

        #endregion

        #region FXAA

        /// <summary>
        /// The texture name for the lighted scene
        /// </summary>
        public const string LightedSceneTexture = "LIGHTED_SCENE_TEX";

        #endregion

        #region Shadow mapping

        /// <summary>
        /// The var name for the uniform LightSpaceMatrix.
        /// </summary>
        public const string LightPos = "LightPos";
        /// <summary>
        /// Hash code for the <see cref="LightPos"/> parameter.
        /// </summary>
        public static int LightPosHash = LightPos.GetHashCode();

        /// <summary>
        /// The var name for the uniform LightMatClipPlanes.
        /// </summary>
        public const string LightMatClipPlanes = "LightMatClipPlanes";
        /// <summary>
        /// Hash code for the <see cref="LightMatClipPlanes"/> parameter.
        /// </summary>
        public static int LightMatClipPlanesHash = LightMatClipPlanes.GetHashCode();

        /// <summary>
        /// The var name for the uniform LightSpaceMatrix.
        /// </summary>
        public const string LightSpaceMatrix = "LightSpaceMatrix";
        /// <summary>
        /// Hash code for the <see cref="LightSpaceMatrix"/> parameter.
        /// </summary>
        public static int LightSpaceMatrixHash = LightSpaceMatrix.GetHashCode();


        /// <summary>
        /// The var name for the uniform LightSpaceMatrix with 6 elements e.g. LightSpaceMatrices[6].
        /// </summary>
        public const string LightSpaceMatrices6 = "LightSpaceMatrices[6]";
        /// <summary>
        /// The var name for the uniform LightSpaceMatrix.
        /// </summary>
        public const string LightSpaceMatrices = "LightSpaceMatrices";
        /// <summary>
        /// Hash code for the <see cref="LightSpaceMatrices"/> parameter.
        /// </summary>
        public static int LightSpaceMatricesHash = (LightSpaceMatrices + ArrayPostfix).GetHashCode();

        /// <summary>
        /// The var name for the uniform ShadowMap.
        /// </summary>
        public const string ShadowMap = "ShadowMap";
        /// <summary>
        /// Hash code for the <see cref="ShadowMap"/> parameter.
        /// </summary>
        public static int ShadowMapHash = ShadowMap.GetHashCode();

        /// <summary>
        /// The var name for the uniform ShadowCubeMap.
        /// </summary>
        public const string ShadowCubeMap = "ShadowCubeMap";
        /// <summary>
        /// Hash code for the <see cref="ShadowCubeMap"/> parameter.
        /// </summary>
        public static int ShadowCubeMapHash = ShadowCubeMap.GetHashCode();

        #endregion

        #region Point Cloud
        /// <summary>
        /// The var name for the uniform PointSize.
        /// </summary>
        public const string PointSize = "PointSize";
        /// <summary>
        /// Hash code for the <see cref="PointSize"/> parameter.
        /// </summary>
        public static int PointSizeHash = PointSize.GetHashCode();

        /// <summary>
        /// The var name for the uniform PointShape.
        /// </summary>
        public const string PointShape = "PointShape";
        /// <summary>
        /// Hash code for the <see cref="PointShape"/> parameter.
        /// </summary>
        public static int PointShapeHash = PointShape.GetHashCode();

        /// <summary>
        /// The var name for the uniform PointSizeMode.
        /// </summary>
        public const string PointSizeMode = "PointSizeMode";
        /// <summary>
        /// Hash code for the <see cref="PointSizeMode"/> parameter.
        /// </summary>
        public static int PointSizeModeHash = PointSizeMode.GetHashCode();

        #endregion


        /// <summary>
        /// The var name for the uniform ClippingPlanes variable.
        /// </summary>
        public const string ClippingPlanes = "FUSEE_ClippingPlanes";
        /// <summary>
        /// Hash code for the <see cref="ClippingPlanes"/> parameter.
        /// </summary>
        public static int ClippingPlanesHash = ClippingPlanes.GetHashCode();

        /// <summary>
        /// The var name for the uniform PassNo variable.
        /// </summary>
        public const string RenderPassNo = "PassNo";
        /// <summary>
        /// Hash code for the <see cref="RenderPassNo"/> parameter.
        /// </summary>
        public static int RenderPassNoHash = RenderPassNo.GetHashCode();

        /// <summary>
        /// The var name for the uniform BackgroundColor.
        /// </summary>
        public const string BackgroundColor = "BackgroundColor";
        /// <summary>
        /// Hash code for the <see cref="BackgroundColor"/> parameter.
        /// </summary>
        public static int BackgroundColorHash = BackgroundColor.GetHashCode();

        /// <summary>
        /// The var name for the uniform ScreenParams (width and height of the window).
        /// </summary>
        public const string ViewportPx = "FUSEE_ViewportPx";
        /// <summary>
        /// Hash code for the <see cref="ViewportPx"/> parameter.
        /// </summary>
        public static int ViewportPxHash = ViewportPx.GetHashCode();

        /// <summary>
        /// The var name for the uniform AmbientStrength variable within the pixel shaders.
        /// </summary>
        public const string AmbientStrength = "AmbientStrength";
        /// <summary>
        /// Hash code for the <see cref="AmbientStrength"/> parameter.
        /// </summary>
        public static int AmbientStrengthHash = AmbientStrength.GetHashCode();

        /// <summary>
        /// The var name for the uniform Albedo variable within the pixel shaders.
        /// </summary>
        public const string Albedo = "Albedo";
        /// <summary>
        /// Hash code for the <see cref="Albedo"/> parameter.
        /// </summary>
        public static int AlbedoHash = Albedo.GetHashCode();

        /// <summary>
        /// The var name for the uniform SpecularColor variable within the pixel shaders.
        /// </summary>
        public const string SpecularColor = "SpecularColor";
        /// <summary>
        /// Hash code for the <see cref="SpecularColor"/> parameter.
        /// </summary>
        public static int SpecularColorHash = SpecularColor.GetHashCode();

        /// <summary>
        /// The var name for the uniform EmissiveColor variable within the pixel shaders.
        /// </summary>
        public const string EmissiveColor = "EmissiveColor";
        /// <summary>
        /// Hash code for the <see cref="EmissiveColor"/> parameter.
        /// </summary>
        public static int EmissiveColorHash = EmissiveColor.GetHashCode();

        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixel shaders.
        /// </summary>
        public const string AlbedoTexture = "AlbedoTexture";
        /// <summary>
        /// Hash code for the <see cref="AlbedoTextureHash"/> parameter.
        /// </summary>
        public static int AlbedoTextureHash = AlbedoTexture.GetHashCode();

        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixel shaders.
        /// </summary>
        public const string DiffuseTextureTiles = "AlbedoTextureTiles";
        /// <summary>
        /// Hash code for the <see cref="DiffuseTextureTiles"/> parameter.
        /// </summary>
        public static int DiffuseTextureTilesHash = DiffuseTextureTiles.GetHashCode();

        /// <summary>
        /// The var name for the uniform SpecularTexture variable within the pixel shaders.
        /// </summary>
        public const string SpecularTexture = "SpecularTexture";
        /// <summary>
        /// Hash code for the <see cref="SpecularTexture"/> parameter.
        /// </summary>
        public static int SpecularTextureHash = SpecularTexture.GetHashCode();

        /// <summary>
        /// The var name for the uniform EmissiveTexture variable within the pixel shaders.
        /// </summary>
        public const string EmissiveTexture = "EmissiveTexture";
        /// <summary>
        /// Hash code for the <see cref="EmissiveTexture"/> parameter.
        /// </summary>
        public static int EmissiveTextureHash = EmissiveTexture.GetHashCode();

        /// <summary>
        /// The var name for the uniform NormalMap variable within the pixel shaders.
        /// </summary>
        public const string NormalMap = "NormalMap";
        /// <summary>
        /// Hash code for the <see cref="NormalMap"/> parameter.
        /// </summary>
        public static int NormalMapHash = NormalMap.GetHashCode();

        /// <summary>
        /// The var name for the uniform NormalTextureTiles variable within the pixel shaders.
        /// </summary>
        public const string NormalTextureTiles = "NormalTextureTiles";
        /// <summary>
        /// Hash code for the <see cref="NormalTextureTiles"/> parameter.
        /// </summary>
        public static int NormalTextureTilesHash = NormalTextureTiles.GetHashCode();

        /// <summary>
        /// The var name for the uniform AlbedoMix variable within the pixel shaders.
        /// </summary>
        public const string AlbedoMix = "AlbedoMix";
        /// <summary>
        /// Hash code for the <see cref="AlbedoMix"/> parameter.
        /// </summary>
        public static int AlbedoMixHash = AlbedoMix.GetHashCode();

        /// <summary>
        /// The var name for the uniform SpecularMix variable within the pixel shaders.
        /// </summary>
        public const string SpecularMix = "SpecularMix";
        /// <summary>
        /// Hash code for the <see cref="SpecularMix"/> parameter.
        /// </summary>
        public static int SpecularMixHash = SpecularMix.GetHashCode();

        /// <summary>
        /// The var name for the uniform EmissiveMix variable within the pixel shaders.
        /// </summary>
        public const string EmissiveMix = "EmissiveMix";
        /// <summary>
        /// Hash code for the <see cref="EmissiveMix"/> parameter.
        /// </summary>
        public static int EmissiveMixHash = EmissiveMix.GetHashCode();

        /// <summary>
        /// The var name for the uniform SpecularShininess variable within the pixel shaders.
        /// </summary>
        public const string SpecularShininess = "SpecularShininess";
        /// <summary>
        /// Hash code for the <see cref="SpecularShininess"/> parameter.
        /// </summary>
        public static int SpecularShininessHash = SpecularShininess.GetHashCode();

        /// <summary>
        /// The var name for the uniform SpecularStrength variable within the pixel shaders.
        /// </summary>
        public const string SpecularStrength = "SpecularStrength";
        /// <summary>
        /// Hash code for the <see cref="SpecularStrength"/> parameter.
        /// </summary>
        public static int SpecularStrengthHash = SpecularStrength.GetHashCode();

        /// <summary>
        /// [PBR (Cook-Torrance) only] Describes the roughness of the material.
        /// </summary>
        public const string RoughnessValue = "RoughnessValue";
        /// <summary>
        /// Hash code for the <see cref="RoughnessValue"/> parameter.
        /// </summary>
        public static int RoughnessValueHash = RoughnessValue.GetHashCode();

        /// <summary>
        /// The var name for the uniform NormalMapIntensity variable within the pixel shaders.
        /// </summary>
        public const string NormalMapIntensity = "NormalMapIntensity";
        /// <summary>
        /// Hash code for the <see cref="NormalMapIntensity"/> parameter.
        /// </summary>
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
            Enum.GetName(typeof(RenderTargetTextureTypes), 7),
        };
    }
}