using Fusee.Engine.Common;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    //TODO: define names for "standard" varying parameters?
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

        #endregion

        /// <summary>
        /// The var name for the uniform DiffuseColor variable within the pixel shaders
        /// </summary>
        public static string AmbientStrengthName { get; } = "AmbientStrength";

        /// <summary>
        /// The var name for the uniform DiffuseColor variable within the pixel shaders
        /// </summary>
        public static string DiffuseColorName { get; } = "DiffuseColor";

        /// <summary>
        /// The var name for the uniform SpecularColor variable within the pixel shaders
        /// </summary>
        public static string SpecularColorName { get; } = "SpecularColor";

        /// <summary>
        /// The var name for the uniform EmissiveColor variable within the pixel shaders
        /// </summary>
        public static string EmissiveColorName { get; } = "EmissiveColor";

        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixel shaders
        /// </summary>
        public static string DiffuseTextureName { get; } = "DiffuseTexture";

        /// <summary>
        /// The var name for the uniform SpecularTexture variable within the pixel shaders
        /// </summary>
        public static string SpecularTextureName { get; } = "SpecularTexture";

        /// <summary>
        /// The var name for the uniform EmissiveTexture variable within the pixel shaders
        /// </summary>
        public static string EmissiveTextureName { get; } = "EmissiveTexture";

        /// <summary>
        /// The var name for the uniform BumpTexture variable within the pixel shaders
        /// </summary>
        public static string BumpTextureName { get; } = "BumpTexture";

        /// <summary>
        /// The var name for the uniform DiffuseMix variable within the pixel shaders
        /// </summary>
        public static string DiffuseMixName { get; } = "DiffuseMix";

        /// <summary>
        /// The var name for the uniform SpecularMix variable within the pixel shaders
        /// </summary>
        public static string SpecularMixName { get; } = "SpecularMix";

        /// <summary>
        /// The var name for the uniform EmissiveMix variable within the pixel shaders
        /// </summary>
        public static string EmissiveMixName { get; } = "EmissiveMix";

        /// <summary>
        /// The var name for the uniform SpecularShininess variable within the pixel shaders
        /// </summary>
        public static string SpecularShininessName { get; } = "SpecularShininess";

        /// <summary>
        /// The var name for the uniform SpecularIntensity variable within the pixel shaders
        /// </summary>
        public static string SpecularIntensityName { get; } = "SpecularIntensity";

        /// <summary>
        /// [PBR (Cook-Torrance) only] Describes the roughness of the material
        /// </summary>       
        public static string RoughnessValue { get; } = "RoughnessValue";

        /// <summary>
        /// [PBR (Cook-Torrance) only] This float describes the fresnel reflectance of the material
        /// </summary>        
        public static string FresnelReflectance { get; } = "FresnelReflectance";

        /// <summary>
        /// [PBR (Cook-Torrance) only] This float describes the diffuse fraction of the material
        /// </summary>       
        public static string DiffuseFraction { get; } = "DiffuseFraction";

        /// <summary>
        /// The var name for the uniform BumpIntensity variable within the pixel shaders
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
