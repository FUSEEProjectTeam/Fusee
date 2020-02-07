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
        #region Fusee internal

        /// <summary>
        /// The vertex attribute name.
        /// </summary>
        public static readonly string Vertex = "fuVertex";
        /// <summary>
        /// The color attribute name.
        /// </summary>
        public static readonly string Color = "fuColor";

        /// <summary>
        /// The normal attribute name.
        /// </summary>
        public static readonly string Normal = "fuNormal";

        /// <summary>
        /// The uv attribute name.
        /// </summary>
        public static readonly string TextureCoordinates = "fuUV";

        /// <summary>
        /// The tangent attribute name.
        /// </summary>
        public static readonly string TangentAttribName = "fuTangent";

        /// <summary>
        /// The bitangent attribute name.
        /// </summary>
        public static readonly string BitangentAttribName = "fuBitangent";

        /// <summary>
        /// The bone weight attribute name.
        /// </summary>
        public static readonly string BoneWeight = "fuBoneWeight";

        /// <summary>
        /// The bone index attribute name.
        /// </summary>
        public static readonly string BoneIndex = "fuBoneIndex";

        /// <summary>
        /// The model matrix. Transforms from model into world space.
        /// </summary>
        public static readonly string Model = "FUSEE_M";

        /// <summary>
        /// The view matrix. Transforms from world into camera space.
        /// </summary>
        public static readonly string View = "FUSEE_V";

        /// <summary>
        /// The model view matrix. Transforms from model into camera space.
        /// </summary>
        public static readonly string ModelView = "FUSEE_MV";

        /// <summary>
        /// The model view matrix. Transforms from view into clip space.
        /// </summary>
        public static readonly string Projection = "FUSEE_P";

        /// <summary>
        /// The model view projection matrix. Transforms from model into clip space.
        /// </summary>
        public static readonly string ModelViewProjection = "FUSEE_MVP";

        /// <summary>
        /// The inverse view matrix.
        /// </summary>
        public static readonly string IView = "FUSEE_IV";

        /// <summary>
        /// The inverse view matrix.
        /// </summary>
        public static readonly string IModel = "FUSEE_IM";

        /// <summary>
        /// The inverse view matrix.
        /// </summary>
        public static readonly string TModel = "FUSEE_TM";

        /// <summary>
        /// The inverse model view matrix.
        /// </summary>
        public static readonly string IModelView = "FUSEE_IMV";

        /// <summary>
        /// The inverse transposed view matrix.
        /// </summary>
        public static readonly string TView = "FUSEE_TV";

        /// <summary>
        /// The inverse transposed view matrix.
        /// </summary>
        public static readonly string ITView = "FUSEE_ITV";

        /// <summary>
        /// The inverse projection matrix.
        /// </summary>
        public static readonly string IProjection = "FUSEE_IP";

        /// <summary>
        /// The inverse model view projection matrix.
        /// </summary>
        public static readonly string IModelViewProjection = "FUSEE_IMVP";

        /// <summary>
        /// The transposed model view matrix.
        /// </summary>
        public static readonly string TModelView = "FUSEE_TMV";

        /// <summary>
        /// The transposed projection matrix.
        /// </summary>
        public static readonly string TProjection = "FUSEE_TP";

        /// <summary>
        /// The transposed model view projection matrix.
        /// </summary>
        public static readonly string TModelViewProjection = "FUSEE_TMVP";

        /// <summary>
        /// The inversed transposed model view matrix.
        /// </summary>
        public static readonly string ITModelView = "FUSEE_ITMV";

        /// <summary>
        /// The inversed transposed projection matrix.
        /// </summary>
        public static readonly string ITProjection = "FUSEE_ITP";

        /// <summary>
        /// The inversed transposed model view projection matrix.
        /// </summary>
        public static readonly string ITModelViewProjection = "FUSEE_ITMVP";

        /// <summary>
        /// The inversed transposed model view projection matrix.
        /// </summary>
        public static readonly string ITModel = "FUSEE_ITM";

        /// <summary>
        /// The bones array.
        /// </summary>
        public static readonly string Bones = "FUSEE_BONES";

        #endregion

        #region SSAO

        /// <summary>
        /// The var name for the uniform SSAOKernel[0] variable.
        /// </summary>
        public static string SSAOKernel { get; } = "SSAOKernel[0]";

        /// <summary>
        /// The var name for the uniform NoiseTex variable, needed to calculate SSAO.
        /// </summary>
        public static string NoiseTex { get; } = "NoiseTex";

        /// <summary>
        /// The var name for the uniform SsaoOn variable.
        /// </summary>
        public static string SsaoOn { get; } = "SsaoOn";

        #endregion

        #region Shadow mapping

        /// <summary>
        /// The var name for the uniform LightSpaceMatrix.
        /// </summary>
        public static string LightSpaceMatrix { get; } = "LightSpaceMatrix";

        /// <summary>
        /// The var name for the uniform ShadowMap.
        /// </summary>
        public static string ShadowMap { get; } = "ShadowMap";

        /// <summary>
        /// The var name for the uniform ShadowCubeMap.
        /// </summary>
        public static string ShadowCubeMap { get; } = "ShadowCubeMap";

        #endregion


        /// <summary>
        /// The var name for the uniform PassNo variable.
        /// </summary>
        public static string RenderPassNo { get; } = "PassNo";

        /// <summary>
        /// The var name for the uniform BackgroundColor.
        /// </summary>
        public static string BackgroundColor { get; } = "BackgroundColor";

        /// <summary>
        /// The var name for the uniform ScreenParams (width and height of the window).
        /// </summary>
        public static string ScreenParams { get; } = "ScreenParams";

        /// <summary>
        /// The var name for the uniform DiffuseColor variable within the pixel shaders.
        /// </summary>
        public static string AmbientStrengthName { get; } = "AmbientStrength";

        /// <summary>
        /// The var name for the uniform DiffuseColor variable within the pixel shaders.
        /// </summary>
        public static string DiffuseColor { get; } = "DiffuseColor";

        /// <summary>
        /// The var name for the uniform SpecularColor variable within the pixel shaders.
        /// </summary>
        public static string SpecularColor { get; } = "SpecularColor";

        /// <summary>
        /// The var name for the uniform EmissiveColor variable within the pixel shaders.
        /// </summary>
        public static string EmissiveColorName { get; } = "EmissiveColor";

        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixel shaders.
        /// </summary>
        public static string DiffuseTexture { get; } = "DiffuseTexture";

        /// <summary>
        /// The var name for the uniform SpecularTexture variable within the pixel shaders.
        /// </summary>
        public static string SpecularTextureName { get; } = "SpecularTexture";

        /// <summary>
        /// The var name for the uniform EmissiveTexture variable within the pixel shaders.
        /// </summary>
        public static string EmissiveTextureName { get; } = "EmissiveTexture";

        /// <summary>
        /// The var name for the uniform BumpTexture variable within the pixel shaders.
        /// </summary>
        public static string BumpTextureName { get; } = "BumpTexture";

        /// <summary>
        /// The var name for the uniform DiffuseMix variable within the pixel shaders.
        /// </summary>
        public static string DiffuseMix { get; } = "DiffuseMix";

        /// <summary>
        /// The var name for the uniform SpecularMix variable within the pixel shaders.
        /// </summary>
        public static string SpecularMixName { get; } = "SpecularMix";

        /// <summary>
        /// The var name for the uniform EmissiveMix variable within the pixel shaders.
        /// </summary>
        public static string EmissiveMixName { get; } = "EmissiveMix";

        /// <summary>
        /// The var name for the uniform SpecularShininess variable within the pixel shaders.
        /// </summary>
        public static string SpecularShininessName { get; } = "SpecularShininess";

        /// <summary>
        /// The var name for the uniform SpecularIntensity variable within the pixel shaders.
        /// </summary>
        public static string SpecularStrength { get; } = "SpecularIntensity";

        /// <summary>
        /// [PBR (Cook-Torrance) only] Describes the roughness of the material.
        /// </summary>       
        public static string RoughnessValue { get; } = "RoughnessValue";

        /// <summary>
        /// [PBR (Cook-Torrance) only] This float describes the fresnel reflectance of the material.
        /// </summary>        
        public static string FresnelReflectance { get; } = "FresnelReflectance";

        /// <summary>
        /// [PBR (Cook-Torrance) only] This float describes the diffuse fraction of the material.
        /// </summary>       
        public static string DiffuseFraction { get; } = "DiffuseFraction";

        /// <summary>
        /// The var name for the uniform BumpIntensity variable within the pixel shaders.
        /// </summary>
        public static string BumpIntensityName { get; } = "BumpIntensityName";

        /// <summary>
        /// List of all possible render texture names.
        /// </summary>
        public static List<string> DeferredRenderTextures { get; } = new List<string>()
        {
            Enum.GetName(typeof(RenderTargetTextureTypes), 0),
            Enum.GetName(typeof(RenderTargetTextureTypes), 1),
            Enum.GetName(typeof(RenderTargetTextureTypes), 2),
            Enum.GetName(typeof(RenderTargetTextureTypes), 3),
            Enum.GetName(typeof(RenderTargetTextureTypes), 4),
            Enum.GetName(typeof(RenderTargetTextureTypes), 5),
        };        
    }
}
