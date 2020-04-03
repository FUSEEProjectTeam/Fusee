using ProtoBuf;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Material definition.If contained within a node, the node's (and potentially child node's)
    /// geometry is rendered with the specifies material.
    /// </summary>
    [ProtoContract]
    // ReSharper disable once InconsistentNaming
    public class FusMaterialPBR : FusMaterial
    {
        /// <summary>
        /// This float describes the roughness of the material
        /// </summary>
        [ProtoMember(1)]
        public float RoughnessValue;

        /// <summary>
        /// This float describes the fresnel reflectance of the material
        /// </summary>
        [ProtoMember(2)]
        public float FresnelReflectance;

        /// <summary>
        /// This float describes the diffuse fraction of the material
        /// </summary>
        [ProtoMember(3)]
        public float DiffuseFraction;
    }
}
