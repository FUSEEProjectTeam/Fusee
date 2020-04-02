using Fusee.Serialization;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// This struct describes a collection of properties, which are the basis to build the needed shader.
    /// </summary>    
    public struct EffectProps
    {
        /// <summary>
        /// Collection of bools, describing the mesh properties.
        /// </summary>
        public MeshProps MeshProbs;

        /// <summary>
        /// Collection of bools, describing the material properties.
        /// </summary>
        public LightingProps LightingProps;
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
    /// Specifies the type of the specular lighting calculation.
    /// </summary>
    public enum SpecularLighting
    {
        /// <summary>
        /// Standard specular calculation with strength and shininess.
        /// </summary>
        Std,

        /// <summary>
        /// Physically based specular calculation with fresnel and roughness value.
        /// </summary>
        Pbr,

        /// <summary>
        /// No specular calculation.
        /// </summary>
        None
    }

    /// <summary>
    /// Collection of bools, describing the material properties.
    /// </summary>
    public struct LightingProps
    {
        /// <summary>
        /// Do we need to perform diffuse lighting for this material?
        /// </summary>
        public bool DoDiffuseLighting;

        /// <summary>
        /// Does this material have a diffuse texture?
        /// </summary>
        public bool HasDiffuseTexture;

        /// <summary>
        /// Do we need to perform diffuse lighting for this material?
        /// </summary>
        public SpecularLighting SpecularLighting;

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
        /// Does this material have a bump map?
        /// </summary>
        public bool HasNormalMap;
    }

    /// <summary>
    /// Provides utility methods to write and use Shader Shards.
    /// </summary>
    public static class Utility
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

        /// <summary>
        /// Creates a new <see cref="EffectProps"/> from a MaterialComponent, a WeightComponent and a mesh.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        /// <param name="mc">The material.</param>
        /// <param name="wc">The weights.</param>
        /// <returns></returns>
        public static EffectProps CollectEffectProps(Mesh mesh, MaterialComponent mc, WeightComponent wc = null)
        {
            return new EffectProps()
            {
                LightingProps = AnalzyeMaterialParams(mc),
                MeshProbs = AnalyzeMesh(mesh, wc),
            };
        }

        private static LightingProps AnalzyeMaterialParams(MaterialComponent mc)
        {
            var lProps = new LightingProps
            {
                DoDiffuseLighting = mc.HasDiffuse,
                HasDiffuseTexture = mc.HasDiffuse && mc.Diffuse.Texture != null,

                HasSpecularTexture = mc.HasSpecular && mc.Specular.Texture != null,
                HasEmissive = mc.HasEmissive,
                HasEmissiveTexture = mc.HasEmissive && mc.Emissive.Texture != null,
                HasNormalMap = mc.HasBump
            };

            if (mc.HasSpecular)
            {
                if (mc.GetType() == typeof(MaterialPBRComponent))
                    lProps.SpecularLighting = SpecularLighting.Pbr;
                else
                    lProps.SpecularLighting = SpecularLighting.Std;
            }
            else
                lProps.SpecularLighting = SpecularLighting.None;

            return lProps;
        }

        //TODO: At the moment the ShaderCodebuilder doesn't get meshes and therefor we always have the default values. Do we need (or want this here)? This would mean we have a relation of the ShaderEffect to the Mesh.....
        private static MeshProps AnalyzeMesh(Mesh mesh, WeightComponent wc = null)
        {
            return new MeshProps
            {
                HasNormals = mesh == null || mesh.Normals != null && mesh.Normals.Length > 0,
                HasUVs = mesh == null || mesh.UVs != null && mesh.UVs.Length > 0,
                HasColors = false,
                HasWeightMap = wc != null,
                HasTangents = mesh == null || (mesh.Tangents != null && mesh.Tangents.Length > 1),
                HasBiTangents = mesh == null || (mesh.BiTangents != null && mesh.BiTangents.Length > 1)
            };
        }
    }
}
