using Fusee.Math.Core;
using ProtoBuf;
using System;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Part of the <see cref="FusMaterial"/> defining material settings.
    /// Used in the different lighting calculation components.
    /// </summary>
    [ProtoContract]
    // taken from http://www.codeproject.com/Articles/642677/Protobuf-net-the-unofficial-manual
    // Here is where I start disliking Protobuf...Note-to-self: Write own serialization
    [ProtoInclude(100, typeof(SpecularChannel))]
    public class AlbedoChannel : IEquatable<AlbedoChannel>
    {
        /// <summary>
        /// The color of the light component.
        /// </summary>
        [ProtoMember(1)]
        public float4 Color;

        /// <summary>
        /// If not null, the texture to use as the color.
        /// </summary>
        [ProtoMember(2)]
        public string Texture;

        /// <summary>
        /// The percentage how to mix Color and Texture.
        /// </summary>
        [ProtoMember(3)]
        public float Mix;

        /// <summary>
        /// The diffuse texture tiling.
        /// </summary>
        public float2 Tiles = float2.One;

        #region equals

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="lhs">The first instance.</param>
        /// <param name="rhs">The second instance.</param>
        /// <returns>
        /// True, if left does equal right; false otherwise.
        /// </returns>
        public static bool operator ==(AlbedoChannel lhs, AlbedoChannel rhs)
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
        public static bool operator !=(AlbedoChannel lhs, AlbedoChannel rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Indicates whether the MatChannelContainer is equal to another one.
        /// </summary>
        /// <param name="other">The MatChannelContainer to compare with this one.</param>
        /// <returns>
        /// true if the current MatChannelContainer is equal to the other; otherwise, false.
        /// </returns>
        public bool Equals(AlbedoChannel other)
        {
            if (other == null)
                return false;

            return other.Color == Color && other.Texture == Texture && other.Mix == Mix;
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
                return Equals((AlbedoChannel)obj);
        }

        /// <summary>
        /// Returns the hash for this instance.
        /// </summary>        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 29 + Color.GetHashCode();
                hash = hash * 29 + Mix.GetHashCode();
                hash = Texture == null ? hash * 29 + 0 : hash * 29 + Texture.GetHashCode();
                return hash;
            }
        }

        #endregion
    }

    /// <summary>
    /// The specular channel definition. 
    /// </summary>
    [ProtoContract]
    public class SpecularChannel : AlbedoChannel, IEquatable<SpecularChannel>
    {
        /// <summary>
        /// The material's shininess.
        /// </summary>
        [ProtoMember(1)]
        public float Shininess;

        /// <summary>
        /// The specular intensity.
        /// </summary>
        [ProtoMember(2)]
        public float Strength;

        #region equals

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="lhs">The first instance.</param>
        /// <param name="rhs">The second instance.</param>
        /// <returns>
        /// True, if left does equal right; false otherwise.
        /// </returns>
        public static bool operator ==(SpecularChannel lhs, SpecularChannel rhs)
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
        public static bool operator !=(SpecularChannel lhs, SpecularChannel rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Indicates whether the SpecularChannelContainer is equal to another one.
        /// </summary>
        /// <param name="other">The SpecularChannelContainer to compare with this one.</param>
        /// <returns>
        /// true if the current SpecularChannelContainer is equal to the other; otherwise, false.
        /// </returns>
        public bool Equals(SpecularChannel other)
        {
            if (other is null)
                return false;
            return other.Shininess == Shininess && other.Strength == Strength;
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
                return Equals((SpecularChannel)obj);
        }

        /// <summary>
        /// Returns the hash for this instance.
        /// </summary>        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 29 + Color.GetHashCode();
                hash = hash * 29 + Mix.GetHashCode();
                hash = Texture == null ? hash * 29 + 0 : hash * 29 + Texture.GetHashCode();
                hash = hash * 29 + Shininess.GetHashCode();
                hash = hash * 29 + Strength.GetHashCode();
                return hash;
            }
        }

        #endregion
    }

    /// <summary>
    /// If used, the material shows bumps defined by a normal map (a texture).
    /// </summary>
    [ProtoContract]
    public class NormapMapChannel : IEquatable<NormapMapChannel>
    {
        /// <summary>
        /// The texture to read the normal information from.
        /// </summary>
        [ProtoMember(1)]
        public string Texture;

        /// <summary>
        /// The strength of the normal mapping effect.
        /// </summary>
        [ProtoMember(2)]
        public float Intensity;

        /// <summary>
        /// The normal map texture tiling.
        /// </summary>
        public float2 Tiles = float2.One;

        #region equals

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="lhs">The first instance.</param>
        /// <param name="rhs">The second instance.</param>
        /// <returns>
        /// True, if left does equal right; false otherwise.
        /// </returns>
        public static bool operator ==(NormapMapChannel lhs, NormapMapChannel rhs)
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
        public static bool operator !=(NormapMapChannel lhs, NormapMapChannel rhs)
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
        public bool Equals(NormapMapChannel other)
        {
            if (other is null)
                return false;
            return other.Texture == Texture && other.Intensity == Intensity;
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
                return Equals((NormapMapChannel)obj);

        }

        /// <summary>
        /// Returns the hash for this instance.
        /// </summary>        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = Texture == null ? hash * 29 + 0 : hash * 29 + Texture.GetHashCode();
                hash = hash * 29 + Intensity.GetHashCode();
                return hash;
            }
        }

        #endregion

    }

    /// <summary>
    /// Material definition. If contained within a node, the node's (and potentially child node's)
    /// geometry is rendered with the specified material.
    /// </summary>
    [ProtoContract]
    [ProtoInclude(200, typeof(FusMaterialPBR))]
    public class FusMaterial : FusComponent
    {
        #region Albedo
        /// <summary>
        /// Gets a value indicating whether this instance has an albedo channel.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has an albedo channel; otherwise, <c>false</c>.
        /// </value>
        public bool HasAlbedo { get { return Albedo != null; } }

        /// <summary>
        /// The albedo channel.
        /// </summary>
        [ProtoMember(1)]
        public AlbedoChannel Albedo;
        #endregion

        #region Specular
        /// <summary>
        /// Gets a value indicating whether this instance has a specular channel.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has a specular channel; otherwise, <c>false</c>.
        /// </value>
        public bool HasSpecular { get { return Specular != null; } }

        /// <summary>
        /// The specular channel.
        /// </summary>
        [ProtoMember(2)]
        public SpecularChannel Specular;
        #endregion

        #region Emissive
        /// <summary>
        /// Gets a value indicating whether this instance has an emissive channel.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has an emissive channel; otherwise, <c>false</c>.
        /// </value>
        public bool HasEmissive { get { return Emissive != null; } }

        /// <summary>
        /// The emissive channel.
        /// </summary>
        [ProtoMember(3)]
        public AlbedoChannel Emissive;
        #endregion

        #region NormalMap
        /// <summary>
        /// Gets a value indicating whether this instance has a normal map channel.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has a normal map channel; otherwise, <c>false</c>.
        /// </value>
        public bool HasNormalMap { get { return NormalMap != null; } }

        /// <summary>
        /// The normal map channel.
        /// </summary>
        [ProtoMember(4)]
        public NormapMapChannel NormalMap;
        #endregion
    }

}