using System.Collections.Generic;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// This struct describes a collection of properties, which are the basis to build the needed shader.
    /// </summary>
    public struct ShaderEffectProps
    {
        /// <summary>
        /// Collection of bools, describing the mesh properties.
        /// </summary>
        public MeshProps MeshProbs;

        /// <summary>
        /// Collection of bools, describing the material properties.
        /// </summary>
        public MaterialProps MatProbs;

        /// <summary>
        /// The type of the material.
        /// </summary>
        public MaterialType MatType;

        /// <summary>
        /// Collection of values of one material 
        /// </summary>
        public MaterialValues MatValues;
    }

    /// <summary>
    /// The values one material could carry (e.g.: AlbedoColor or SpecularIntensity)
    /// </summary>
    public struct MaterialValues
    {
#pragma warning disable CS1591 // missing XML-comment for public type
        public float4 AlbedoColor;
        public string AlbedoTexture;
        public float AlbedoMix;

        public float4 SpecularColor;
        public float SpecularShininess;
        public float SpecularIntensity;
        public float SpecularMix;
        public string SpecularTexture;

        public float RoughnessValue;
        public float FresnelReflectance;
        public float DiffuseFraction;

        public float4 EmissiveColor;
        public float EmissiveMix;
        public string EmissiveTexture;

        public float NormalMapIntensity;
        public string NormalMap;
#pragma warning restore CS1591 // missing XML-comment for public type
    }

    /// <summary>
    /// Collection of bools, describing the mesh properties.
    /// </summary>
    public struct MeshProps
    {
        /// <summary>
        /// Does this mesh have normals?
        /// </summary>
        public bool HasNormals;

        /// <summary>
        /// Does this mesh have uv coordinates?
        /// </summary>
        public bool HasUVs;

        /// <summary>
        /// Does this mesh have vertex colors?
        /// </summary>
        public bool HasColors;

        /// <summary>
        /// Does this mesh have a weight map?
        /// </summary>
        public bool HasWeightMap;

        /// <summary>
        /// Does this mesh have tangents?
        /// </summary>
        public bool HasTangents;

        /// <summary>
        /// Does this mesh have bitangents?
        /// </summary>
        public bool HasBiTangents;
    }

    /// <summary>
    /// Collection of bools, describing the material properties.
    /// </summary>
    public struct MaterialProps
    {
        /// <summary>
        /// Does this material have an albedo color?
        /// </summary>
        public bool HasAlbedo;

        /// <summary>
        /// Does this material have an albedo texture?
        /// </summary>
        public bool HasAlbedoTexture;

        /// <summary>
        /// Does this material have a specular reflection?
        /// </summary>
        public bool HasSpecular;

        /// <summary>
        /// Does this material have a specular texture?
        /// </summary>
        public bool HasSpecularTexture;

        /// <summary>
        /// Does this material have a emissive component?
        /// </summary>
        public bool HasEmissive;

        /// <summary>
        /// Does this material have a emissive texture?
        /// </summary>
        public bool HasEmissiveTexture;

        /// <summary>
        /// Does this material have a normal map?
        /// </summary>
        public bool HasNormalMap;
    }

    /// <summary>
    /// The type of the material.
    /// </summary>
    public enum MaterialType
    {
        /// <summary>
        /// The Standard material.
        /// </summary>
        Standard,

        /// <summary>
        /// A material for physically based lighting.
        /// </summary>
        MaterialPbr
    }

    /// <summary>
    /// Provides utility methods to write and use Shader Shards.
    /// </summary>
    public static class ShaderShardUtil
    {
        /// <summary>
        /// Creates a main method with the given method body.
        /// </summary>
        /// <param name="methodBody">The content of the method.</param>
        /// <returns></returns>
        public static string MainMethod(IList<string> methodBody)
        {
            return GLSL.CreateMethod(GLSL.Type.Void, "main",
                new[] { "" }, methodBody);
        }
    }
}
