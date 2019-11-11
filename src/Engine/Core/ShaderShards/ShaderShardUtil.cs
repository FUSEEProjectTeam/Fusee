using Fusee.Serialization;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    public struct ShaderEffectProps
    {
        public MeshProps MeshProbs;
        public MaterialProps MatProbs;
        public MaterialType MatType;
    }

    public struct MeshProps
    {
        public bool HasVertices;
        public bool HasNormals;
        public bool HasUVs;
        public bool HasColors;
        public bool HasWeightMap;
        public bool HasTangents;
        public bool HasBiTangents;
    }

    public struct MaterialProps
    {
        public bool HasDiffuse;
        public bool HasDiffuseTexture;
        public bool HasSpecular;
        public bool HasSpecularTexture;
        public bool HasEmissive;
        public bool HasEmissiveTexture;
        public bool HasBump;
        public bool HasApplyLightString;
    }

    public enum MaterialType
    {
        Material,
        MaterialLightComponent,
        MaterialPbrComponent
    }

    public static class ShaderShardUtil
    {
        public static string MainMethod(IList<string> methodBody)
        {
            return GLSL.CreateMethod(GLSL.Type.Void, "main",
                new[] { "" }, methodBody);
        }

        public static ShaderEffectProps CollectEffectProps(Mesh mesh, MaterialComponent mc, WeightComponent wc = null)
        {
            var matType = AnalyzeMaterialType(mc);
            
            //TODO: obsolete with shader shard system
            var hasApplyLightString = matType == MaterialType.MaterialLightComponent && (string.IsNullOrEmpty((mc as MaterialLightComponent)?.ApplyLightString));

            return new ShaderEffectProps()
            {
                MatType = matType,
                MatProbs = AnalzyeMaterialParams(mc, hasApplyLightString),
                MeshProbs = AnalyzeMesh(mesh, wc)
            };            
        }

        private static MaterialProps AnalzyeMaterialParams(MaterialComponent mc, bool hasApplyLightString)
        {
            return new MaterialProps
            {
                HasDiffuse = mc.HasDiffuse,
                HasDiffuseTexture = mc.HasDiffuse && mc.Diffuse.Texture != null,
                HasSpecular = mc.HasSpecular,
                HasSpecularTexture = mc.HasSpecular && mc.Specular.Texture != null,
                HasEmissive = mc.HasEmissive,
                HasEmissiveTexture = mc.HasEmissive && mc.Emissive.Texture != null,
                HasBump = mc.HasBump,
                HasApplyLightString = hasApplyLightString
            };
        }

        private static MaterialType AnalyzeMaterialType(MaterialComponent mc)
        {
            if (mc.GetType() == typeof(MaterialPBRComponent))
                return MaterialType.MaterialPbrComponent;

            if (mc.GetType() == typeof(MaterialLightComponent))
                return MaterialType.MaterialLightComponent;

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
