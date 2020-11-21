using Fusee.Math.Core;
using ProtoBuf;
using System;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Contains information needed for a physically-based lighting calculation using a bidirectional reflectance distribution function.
    /// </summary>
    [ProtoContract]
    public class BRDFChannel : IEquatable<BRDFChannel>
    {
        /// <summary>
        /// The index of refraction.
        /// </summary>
        [ProtoMember(1)]
        public float IOR;

        /// <summary>
        /// Blends between a non-metallic and metallic material model. Must be a value between 0 and 1.
        /// 0: The material is dielectric: has a diffuse and a non-tinted specular component.
        /// 1: The material is metallic: has no diffuse and a specular component whose color is tinted by the materials base color.
        /// </summary>
        [ProtoMember(2)]
        public float Metallic;

        /// <summary>
        /// Amount of dielectric specular reflection. Must be a value between 0 and 1.
        /// </summary>
        [ProtoMember(3)]
        public float Specular;

        /// <summary>
        /// Specifies microfacet roughness of the surface for diffuse and specular reflection.
        /// </summary>
        [ProtoMember(4)]
        public float Roughness;

        /// <summary>
        /// Mix between diffuse and subsurface scattering.
        /// </summary>
        [ProtoMember(5)]
        public float Subsurface;

        /// <summary>
        /// Mix between diffuse and subsurface scattering.
        /// </summary>
        [ProtoMember(6)]
        public float3 SubsurfaceColor;

        #region equals

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="lhs">The first instance.</param>
        /// <param name="rhs">The second instance.</param>
        /// <returns>
        /// True, if left does equal right; false otherwise.
        /// </returns>
        public static bool operator ==(BRDFChannel lhs, BRDFChannel rhs)
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
        public static bool operator !=(BRDFChannel lhs, BRDFChannel rhs)
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
        public bool Equals(BRDFChannel other)
        {
            if (other is null)
                return false;

            return other.Roughness == Roughness
                && other.Metallic == Metallic
                && other.Specular == Specular
                && other.IOR == IOR
                && other.Subsurface == Subsurface
                && other.SubsurfaceColor == SubsurfaceColor;
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
                return Equals((BRDFChannel)obj);
        }

        /// <summary>
        /// Returns the hash for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Roughness, Metallic, IOR, Specular, Subsurface, SubsurfaceColor);
        }

        #endregion
    }

    /// <summary>
    /// Part of the <see cref="FusMaterialStandard"/> defining material settings.
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
        public string? Texture;

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
                return Equals((AlbedoChannel)obj);
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
            return HashCode.Combine(Color, Mix, Texture, Shininess, Strength);

            //unchecked
            //{
            //    int hash = 17;
            //    hash = hash * 29 + Color.GetHashCode();
            //    hash = hash * 29 + Mix.GetHashCode();
            //    hash = Texture == null ? hash * 29 + 0 : hash * 29 + Texture.GetHashCode();
            //    hash = hash * 29 + Shininess.GetHashCode();
            //    hash = hash * 29 + Strength.GetHashCode();
            //    return hash;
            //}
        }

        #endregion
    }

    /// <summary>
    /// If used, the material shows bumps defined by a normal map (a texture in RGB).
    /// </summary>
    [ProtoContract]
    public class NormapMapChannel : IEquatable<NormapMapChannel>
    {
        /// <summary>
        /// The texture to read the normal information from.
        /// </summary>
        [ProtoMember(1)]
        public string? Texture;

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
            return HashCode.Combine(Texture, Intensity);
        }

        #endregion
    }
}