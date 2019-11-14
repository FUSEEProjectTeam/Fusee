using Fusee.Serialization;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// This struct describes a collection of properties, which are the basis to build the needed shader.
    /// </summary>
    public struct ShaderEffectProps
    {
        /// <summary>
        /// Do we do a physically based lighting calculation?
        /// </summary>
        public bool DoRenderPhysicallyBased 
        { 
            get 
            { 
                return PBRProps.RoughnessValue == 0 && PBRProps.DiffuseFraction == 0 && PBRProps.FresnelReflectance == 0; 
            } 
            private set { } 
        }

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
        /// Collection of float values, used to do a physically based lighting calculation.
        /// </summary>
        public PBRProps PBRProps;
    }

    /// <summary>
    /// Collection of bools, describing the mesh properties.
    /// </summary>
    public struct MeshProps
    {
        public bool HasVertices;

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
        /// Does this material have a diffuse color?
        /// </summary>
        public bool HasDiffuse;

        /// <summary>
        /// Does this material have a diffuse texture?
        /// </summary>
        public bool HasDiffuseTexture;

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
        /// Does this material have a bump map?
        /// </summary>
        public bool HasBump;        
    }

    /// <summary>
    /// The type of the material.
    /// </summary>
    public enum MaterialType
    {
        /// <summary>
        /// The Standard material.
        /// </summary>
        Material,   
        
        /// <summary>
        /// A material for physically based lighting.
        /// </summary>
        MaterialPbr
    }

    /// <summary>
    /// Collection of float values, used to do a physically based lighting calculation.
    /// </summary>
    public struct PBRProps
    {
        /// <summary>
        /// This float describes the roughness of the material
        /// </summary>       
        public float RoughnessValue;

        /// <summary>
        /// This float describes the fresnel reflectance of the material
        /// </summary>        
        public float FresnelReflectance;

        /// <summary>
        /// This float describes the diffuse fraction of the material
        /// </summary>       
        public float DiffuseFraction;
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

        /// <summary>
        /// Creates a new <see cref="ShaderEffectProps"/> from a MaterialComponent, a WeightComponent and a mesh.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        /// <param name="mc">The material.</param>
        /// <param name="wc">The weights.</param>
        /// <returns></returns>
        public static ShaderEffectProps CollectEffectProps(Mesh mesh, MaterialComponent mc, WeightComponent wc = null)
        {
            var matType = AnalyzeMaterialType(mc);

            PBRProps pbrProps = new PBRProps();

            if (mc.GetType() == typeof(MaterialPBRComponent)) 
            {
                var mpbr = (MaterialPBRComponent)mc;
                pbrProps.DiffuseFraction = mpbr.DiffuseFraction;
                pbrProps.FresnelReflectance = mpbr.FresnelReflectance;
                pbrProps.RoughnessValue = mpbr.RoughnessValue;               
            }

            return new ShaderEffectProps()
            {
                MatType = matType,
                MatProbs = AnalzyeMaterialParams(mc),
                MeshProbs = AnalyzeMesh(mesh, wc),
                PBRProps = pbrProps
            };            
        }

        private static MaterialProps AnalzyeMaterialParams(MaterialComponent mc)
        {
            return new MaterialProps
            {
                HasDiffuse = mc.HasDiffuse,
                HasDiffuseTexture = mc.HasDiffuse && mc.Diffuse.Texture != null,
                HasSpecular = mc.HasSpecular,
                HasSpecularTexture = mc.HasSpecular && mc.Specular.Texture != null,
                HasEmissive = mc.HasEmissive,
                HasEmissiveTexture = mc.HasEmissive && mc.Emissive.Texture != null,
                HasBump = mc.HasBump                
            };
        }

        private static MaterialType AnalyzeMaterialType(MaterialComponent mc)
        {
            if (mc.GetType() == typeof(MaterialPBRComponent))
                return MaterialType.MaterialPbr;

            return MaterialType.Material;
        }

        //TODO: At the moment the ShaderCodebuilder doesn't get meshes and therefor we always have the default values. Do we need (or want this here)? This would mean we have a relation of the ShaderEffect to the Mesh.....
        private static MeshProps AnalyzeMesh(Mesh mesh, WeightComponent wc = null)
        {
            return new MeshProps
            {
                HasVertices = mesh == null || mesh.Vertices != null && mesh.Vertices.Length > 0, // if no mesh => true
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
