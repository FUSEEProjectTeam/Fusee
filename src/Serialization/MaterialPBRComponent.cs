using ProtoBuf;
using System;

namespace Fusee.Serialization
{
    /// <summary>
    /// Material definition.If contained within a node, the node's (and potentially child node's)
    /// geometry is rendered with the specified material.
    /// </summary>
    [ProtoContract]
    // ReSharper disable once InconsistentNaming
    public class MaterialPBRComponent : MaterialComponent, IEquatable<MaterialPBRComponent>
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

        #region equals

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="lhs">The first instance.</param>
        /// <param name="rhs">The second instance.</param>
        /// <returns>
        /// True, if left does equal right; false otherwise.
        /// </returns>
        public static bool operator ==(MaterialPBRComponent lhs, MaterialPBRComponent rhs)
        {
            // Check for null on left side.
            if (lhs is null)
            {
                if (lhs is null)
                    return true;

                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }


        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="lhs">The first instance.</param>
        /// <param name="rhs">The second instance.</param>
        /// <returns>
        /// True, if left does not equal right; false otherwise.
        /// </returns>
        public static bool operator !=(MaterialPBRComponent lhs, MaterialPBRComponent rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Returns the hash for this instance.
        /// </summary>        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 29 + HasDiffuse.GetHashCode();
                hash = Diffuse == null ? hash * 29 + 0 : hash * 29 + Diffuse.GetHashCode();

                hash = hash * 29 + HasSpecular.GetHashCode();
                hash = Specular == null ? hash * 29 + 0 : hash * 29 + Specular.GetHashCode();

                hash = hash * 29 + HasEmissive.GetHashCode();
                hash = Emissive == null ? hash * 29 + 0 : hash * 29 + Emissive.GetHashCode();

                hash = hash * 29 + HasBump.GetHashCode();
                hash = Bump == null ? hash * 29 + 0 : hash * 29 + Bump.GetHashCode();

                hash = hash * 29 + RoughnessValue.GetHashCode();
                hash = hash * 29 + FresnelReflectance.GetHashCode();
                hash = hash * 29 + DiffuseFraction.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Indicates whether the MaterialPBRComponent is equal to another one.
        /// </summary>
        /// <param name="other">The MaterialPBRComponent to compare with this one.</param>
        /// <returns>
        /// true if the current MaterialPBRComponent is equal to the other; otherwise, false.
        /// </returns>
        public bool Equals(MaterialPBRComponent other)
        {
            if (other == null)
                return false;

            return
                HasDiffuse == other.HasDiffuse &&
                Diffuse == other.Diffuse &&
                HasSpecular == other.HasSpecular &&
                Specular == other.Specular &&
                HasEmissive == other.HasEmissive &&
                Emissive == other.Emissive &&
                HasBump == other.HasBump &&
                Bump == other.Bump &&
                RoughnessValue == other.RoughnessValue &&
                FresnelReflectance == other.FresnelReflectance &&
                DiffuseFraction == other.DiffuseFraction;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>
        /// True if the instances are equal; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
                return false;
            else
                return Equals((MaterialPBRComponent)obj);
        }

        #endregion
    }
}