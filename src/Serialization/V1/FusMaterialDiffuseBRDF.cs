using ProtoBuf;
using System;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Material definition. If contained within a node, the node's (and potentially child node's)
    /// geometry is rendered with the specifies material.
    /// Used to add Lambertian or Oren-Nayar diffuse reflection.
    /// </summary>
    [ProtoContract]
    public class FusMaterialDiffuseBRDF : FusMaterialBase, IEquatable<FusMaterialDiffuseBRDF>
    {
        /// <summary>
        /// Specifies microfacet roughness of the surface for diffuse and specular reflection.
        /// 0.0 gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.
        /// </summary>
        [ProtoMember(1)]
        public float Roughness;

        #region equals

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="lhs">The first instance.</param>
        /// <param name="rhs">The second instance.</param>
        /// <returns>
        /// True, if left does equal right; false otherwise.
        /// </returns>
        public static bool operator ==(FusMaterialDiffuseBRDF lhs, FusMaterialDiffuseBRDF rhs)
        {
            // Check for null on left side.
            if (lhs is null)
            {
                if (rhs is null)
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
        public static bool operator !=(FusMaterialDiffuseBRDF lhs, FusMaterialDiffuseBRDF rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Indicates whether the NormapMapChannel is equal to another one.
        /// </summary>
        /// <param name="other">The NormapMapChannel to compare with this one.</param>
        /// <returns>
        /// true if the current NormapMapChannel is equal to the other; otherwise, false.
        /// </returns>
        public bool Equals(FusMaterialDiffuseBRDF other)
        {
            if (other is null)
                return false;
            return other.Albedo == Albedo
                && other.Emissive == Emissive
                && other.NormalMap == NormalMap
                && other.Roughness == Roughness;
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
            if ((obj is null) || !GetType().Equals(obj.GetType()))
                return false;
            else
                return Equals((FusMaterialBRDF)obj);

        }

        /// <summary>
        /// Returns the hash for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Albedo, Emissive, NormalMap, Roughness);
        }

        #endregion
    }
}
