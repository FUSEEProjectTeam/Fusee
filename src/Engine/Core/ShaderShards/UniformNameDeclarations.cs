using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core.ShaderShards
{
    public static class UniformNameDeclarations
    {
        #region Fusee internal

        /// <summary>
        /// The vertex attribute name.
        /// </summary>
        public static readonly string VertexAttribName = "fuVertex";
        /// <summary>
        /// The color attribute name.
        /// </summary>
        public static readonly string ColorAttribName = "fuColor";
        /// <summary>
        /// The normal attribute name.
        /// </summary>
        public static readonly string NormalAttribName = "fuNormal";
        /// <summary>
        /// The uv attribute name.
        /// </summary>
        public static readonly string UvAttribName = "fuUV";
        /// <summary>
        /// The tangent attribute name.
        /// </summary>
        public static readonly string TangentAttribName = "fuTangent";
        /// <summary>
        /// The bitangent attribute name.
        /// </summary>
        public static readonly string BitangentAttribName = "fuBitangent";
        /// <summary>
        /// The boneweight attribute name.
        /// </summary>
        public static readonly string BoneWeightAttribName = "fuBoneWeight";
        /// <summary>
        /// The boneindex attribute name.
        /// </summary>
        public static readonly string BoneIndexAttribName = "fuBoneIndex";

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
        /// The var name for the uniform BumpIntensity variable within the pixel shaders
        /// </summary>
        public static string BumpIntensityName { get; } = "BumpIntensity";

        public static string LightIntensityName { get; } = "LightIntensity";
        
    }
}
