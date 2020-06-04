using ProtoBuf;
using System;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Material definition. If contained within a node, the node's (and potentially child node's)
    /// geometry is rendered with the specified material.
    /// The standard material contains information needed to calculate specular reflection based on Shininess and Intensity (= strength).
    /// </summary>
    [ProtoContract]
    public class FusMaterialStandard : FusMaterialBase, IEquatable<FusMaterialStandard>
    {
        #region Specular
        /// <summary>
        /// Gets a value indicating whether this instance has a specular channel.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has a specular channel; otherwise, <c>false</c>.
        /// </value>
        public bool HasSpecularChannel => Specular != null;

        /// <summary>
        /// The specular channel.
        /// </summary>
        [ProtoMember(2)]
        public SpecularChannel? Specular;
        #endregion

        #region equals

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="lhs">The first instance.</param>
        /// <param name="rhs">The second instance.</param>
        /// <returns>
        /// True, if left does equal right; false otherwise.
        /// </returns>
        public static bool operator ==(FusMaterialStandard lhs, FusMaterialStandard rhs)
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
        public static bool operator !=(FusMaterialStandard lhs, FusMaterialStandard rhs)
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
        public bool Equals(FusMaterialStandard other)
        {
            if (other is null)
                return false;
            return other.Albedo == Albedo
                && other.Emissive == Emissive
                && other.NormalMap == NormalMap
                && other.Specular == Specular;
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
                return Equals((FusMaterialStandard)obj);

        }

        /// <summary>
        /// Returns the hash for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Albedo, Emissive, NormalMap, Specular);
        }

        #endregion

    }

}