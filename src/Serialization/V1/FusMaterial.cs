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
    [ProtoInclude(100, typeof(SpecularChannelContainer))]
    public class MatChannelContainer : IEquatable<MatChannelContainer>
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

        #region equals

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="lhs">The first instance.</param>
        /// <param name="rhs">The second instance.</param>
        /// <returns>
        /// True, if left does equal right; false otherwise.
        /// </returns>
        public static bool operator ==(MatChannelContainer lhs, MatChannelContainer rhs)
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
        public static bool operator !=(MatChannelContainer lhs, MatChannelContainer rhs)
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
        public bool Equals(MatChannelContainer other)
        {
            if (other is null)
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
                return Equals((MatChannelContainer)obj);
        }

        /// <summary>
        /// Returns the hash for this instance.
        /// </summary>        
        public override int GetHashCode()
        {
            return HashCode.Combine(Color, Mix, Texture);
        }

        #endregion
    }

    /// <summary>
    /// The specular channel definition. 
    /// </summary>
    [ProtoContract]
    public class SpecularChannelContainer : MatChannelContainer, IEquatable<SpecularChannelContainer>
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
        public float Intensity;

        #region equals

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="lhs">The first instance.</param>
        /// <param name="rhs">The second instance.</param>
        /// <returns>
        /// True, if left does equal right; false otherwise.
        /// </returns>
        public static bool operator ==(SpecularChannelContainer lhs, SpecularChannelContainer rhs)
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
        public static bool operator !=(SpecularChannelContainer lhs, SpecularChannelContainer rhs)
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
        public bool Equals(SpecularChannelContainer other)
        {
            if (other is null)
                return false;

            return other.Color == Color && other.Texture == Texture && other.Mix == Mix
                && other.Shininess == Shininess && other.Intensity == Intensity;
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
                return Equals((SpecularChannelContainer)obj);
        }

        /// <summary>
        /// Returns the hash for this instance.
        /// </summary>        
        public override int GetHashCode()
        {
            return HashCode.Combine(Color, Mix, Texture, Shininess, Intensity);
        }

        #endregion
    }

    /// <summary>
    /// If used, the material shows bumps defined by a normal map (a texture in RGB).
    /// </summary>
    [ProtoContract]
    public class NormapMapChannelContainer : IEquatable<NormapMapChannelContainer>
    {
        /// <summary>
        /// The texture to read the normal information from.
        /// </summary>
        [ProtoMember(1)]
        public string Texture;

        /// <summary>
        /// The intensity of the normal map..
        /// </summary>
        [ProtoMember(2)]
        public float Intensity;

        #region equals

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="lhs">The first instance.</param>
        /// <param name="rhs">The second instance.</param>
        /// <returns>
        /// True, if left does equal right; false otherwise.
        /// </returns>
        public static bool operator ==(NormapMapChannelContainer lhs, NormapMapChannelContainer rhs)
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
        public static bool operator !=(NormapMapChannelContainer lhs, NormapMapChannelContainer rhs)
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
        public bool Equals(NormapMapChannelContainer other)
        {
            if (other is null)
                return false;

            return other.Texture == Texture && other.Texture == Texture && other.Intensity == Intensity;
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
                return Equals((NormapMapChannelContainer)obj);
        }

        /// <summary>
        /// Returns the hash for this instance.
        /// </summary>        
        public override int GetHashCode()
        {
            return HashCode.Combine(Texture, Intensity);
        }

        #endregion
    }

    /// <summary>
    /// Material definition. If contained within a node, the node's (and potentially child node's)
    /// geometry is rendered with the specified material.
    /// </summary>
    [ProtoContract]
    [ProtoInclude(200, typeof(FusMaterialPBR))]
    public class FusMaterial : FusComponent, IEquatable<FusMaterial>
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
        public MatChannelContainer Albedo;
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
        public SpecularChannelContainer Specular;
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
        public MatChannelContainer Emissive;
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
        public NormapMapChannelContainer NormalMap;
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
        public static bool operator ==(FusMaterial lhs, FusMaterial rhs)
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
        public static bool operator !=(FusMaterial lhs, FusMaterial rhs)
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
        public bool Equals(FusMaterial other)
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
                return Equals((FusMaterial)obj);

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