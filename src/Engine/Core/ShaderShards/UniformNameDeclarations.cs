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

        #region Fusee internal

        /// <summary>
        /// The vertex attribute name.
        /// </summary>
        public const string Vertex = "fuVertex";
        /// <summary>
        /// The color attribute name.
        /// </summary>
        public const string Color = "fuColor";

        /// <summary>
        /// The normal attribute name.
        /// </summary>
        public const string Normal = "fuNormal";

        /// <summary>
        /// The uv attribute name.
        /// </summary>
        public const string TextureCoordinates = "fuUV";

        /// <summary>
        /// The tangent attribute name.
        /// </summary>
        public const string Tangent = "fuTangent";

        /// <summary>
        /// The bitangent attribute name.
        /// </summary>
        public const string Bitangent = "fuBitangent";

        /// <summary>
        /// The bone weight attribute name.
        /// </summary>
        public const string BoneWeight = "fuBoneWeight";

        /// <summary>
        /// The bone index attribute name.
        /// </summary>
        public const string BoneIndex = "fuBoneIndex";

        /// <summary>
        /// The model matrix. Transforms from model into world space.
        /// </summary>
        public const string Model = "FUSEE_M";

        /// <summary>
        /// The view matrix. Transforms from world into camera space.
        /// </summary>
        public const string View = "FUSEE_V";

        /// <summary>
        /// The model view matrix. Transforms from model into camera space.
        /// </summary>
        public const string ModelView = "FUSEE_MV";

        /// <summary>
        /// The model view matrix. Transforms from view into clip space.
        /// </summary>
        public const string Projection = "FUSEE_P";

        /// <summary>
        /// The model view projection matrix. Transforms from model into clip space.
        /// </summary>
        public const string ModelViewProjection = "FUSEE_MVP";

        /// <summary>
        /// The inverse view matrix.
        /// </summary>
        public const string IView = "FUSEE_IV";

        /// <summary>
        /// The inverse view matrix.
        /// </summary>
        public const string IModel = "FUSEE_IM";

        /// <summary>
        /// The inverse view matrix.
        /// </summary>
        public const string TModel = "FUSEE_TM";

        /// <summary>
        /// The inverse model view matrix.
        /// </summary>
        public const string IModelView = "FUSEE_IMV";

        /// <summary>
        /// The inverse transposed view matrix.
        /// </summary>
        public const string TView = "FUSEE_TV";

        /// <summary>
        /// The inverse transposed view matrix.
        /// </summary>
        public const string ITView = "FUSEE_ITV";

        /// <summary>
        /// The inverse projection matrix.
        /// </summary>
        public const string IProjection = "FUSEE_IP";

        /// <summary>
        /// The inverse model view projection matrix.
        /// </summary>
        public const string IModelViewProjection = "FUSEE_IMVP";

        /// <summary>
        /// The transposed model view matrix.
        /// </summary>
        public const string TModelView = "FUSEE_TMV";

        /// <summary>
        /// The transposed projection matrix.
        /// </summary>
        public const string TProjection = "FUSEE_TP";

        /// <summary>
        /// The transposed model view projection matrix.
        /// </summary>
        public const string TModelViewProjection = "FUSEE_TMVP";

        /// <summary>
        /// The inversed transposed model view matrix.
        /// </summary>
        public const string ITModelView = "FUSEE_ITMV";

        /// <summary>
        /// The inversed transposed projection matrix.
        /// </summary>
        public const string ITProjection = "FUSEE_ITP";

        /// <summary>
        /// The inversed transposed model view projection matrix.
        /// </summary>
        public const string ITModelViewProjection = "FUSEE_ITMVP";

        /// <summary>
        /// The inversed transposed model view projection matrix.
        /// </summary>
        public const string ITModel = "FUSEE_ITM";

        /// <summary>
        /// The bones array.
        /// </summary>
        public const string Bones = "FUSEE_BONES";

        /// <summary>
        /// The bones array including the postfix.
        /// </summary>
        public const string BonesArray = Bones + ArrayPostfix;


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
        public const string LightSpaceMatrix = "LightSpaceMatrix";

        /// <summary>
        /// The var name for the uniform ShadowMap.
        /// </summary>
        public const string ShadowMap = "ShadowMap";

        /// <summary>
        /// The var name for the uniform ShadowCubeMap.
        /// </summary>
        public const string ShadowCubeMap = "ShadowCubeMap";

        #endregion


        /// <summary>
        /// The var name for the uniform PassNo variable.
        /// </summary>
        public const string RenderPassNo = "PassNo";

        /// <summary>
        /// The var name for the uniform BackgroundColor.
        /// </summary>
        public const string BackgroundColor = "BackgroundColor";

        /// <summary>
        /// The var name for the uniform ScreenParams (width and height of the window).
        /// </summary>
        public const string ScreenParams = "ScreenParams";

        /// <summary>
        /// The var name for the uniform AmbientStrength variable within the pixel shaders.
        /// </summary>
        public const string AmbientStrength = "AmbientStrength";

        /// <summary>
        /// The var name for the uniform DiffuseColor variable within the pixel shaders.
        /// </summary>
        public const string Albedo = "Albedo";

        /// <summary>
        /// The var name for the uniform SpecularColor variable within the pixel shaders.
        /// </summary>
        public const string SpecularColor = "SpecularColor";

        /// <summary>
        /// The var name for the uniform EmissiveColor variable within the pixel shaders.
        /// </summary>
        public const string EmissiveColor = "EmissiveColor";

        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixel shaders.
        /// </summary>
        public const string AlbedoTexture = "AlbedoTexture";

        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixel shaders.
        /// </summary>
        public const string DiffuseTextureTiles = "AlbedoTextureTiles";

        /// <summary>
        /// The var name for the uniform SpecularTexture variable within the pixel shaders.
        /// </summary>
        public const string SpecularTexture = "SpecularTexture";

        /// <summary>
        /// The var name for the uniform EmissiveTexture variable within the pixel shaders.
        /// </summary>
        public const string EmissiveTexture = "EmissiveTexture";

        /// <summary>
        /// The var name for the uniform NormalMap variable within the pixel shaders.
        /// </summary>
        public const string NormalMap = "NormalMap";

        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixel shaders.
        /// </summary>
        public const string NormalTextureTiles = "NormalTextureTiles";

        /// <summary>
        /// The var name for the uniform DiffuseMix variable within the pixel shaders.
        /// </summary>
        public const string AlbedoMix = "AlbedoMix";

        /// <summary>
        /// The var name for the uniform SpecularMix variable within the pixel shaders.
        /// </summary>
        public const string SpecularMix = "SpecularMix";

        /// <summary>
        /// The var name for the uniform EmissiveMix variable within the pixel shaders.
        /// </summary>
        public const string EmissiveMix = "EmissiveMix";

        /// <summary>
        /// The var name for the uniform SpecularShininess variable within the pixel shaders.
        /// </summary>
        public const string SpecularShininess = "SpecularShininess";

        /// <summary>
        /// The var name for the uniform SpecularIntensity variable within the pixel shaders.
        /// </summary>
        public const string SpecularStrength = "SpecularStrength";

        /// <summary>
        /// [PBR (Cook-Torrance) only] Describes the roughness of the material.
        /// </summary>
        public const string RoughnessValue = "RoughnessValue";

        /// <summary>
        /// [PBR (Cook-Torrance) only] This float describes the fresnel reflectance of the material.
        /// </summary>
        public const string FresnelReflectance = "FresnelReflectance";

        /// <summary>
        /// [PBR (Cook-Torrance) only] This float describes the diffuse fraction of the material.
        /// </summary>
        public const string DiffuseFraction = "DiffuseFraction";

        /// <summary>
        /// The var name for the uniform BumpIntensity variable within the pixel shaders.
        /// </summary>
        public const string NormalMapIntensity = "NormalMapIntensity";


        /// <summary>
        /// List of all possible render texture names.
        /// </summary>
        public static readonly List<string> DeferredRenderTextures = new List<string>()
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
