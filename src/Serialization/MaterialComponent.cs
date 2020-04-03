using Fusee.Math.Core;
using ProtoBuf;
using System;

namespace Fusee.Serialization
{
    /// <summary>
    /// Part of the <see cref="MaterialComponent"/> defining material settings.
    /// Used in the different lighting calculation components.
    /// </summary>
    [ProtoContract]
    // taken from http://www.codeproject.com/Articles/642677/Protobuf-net-the-unofficial-manual
    // Here is where I start disliking Protobuf...Note-to-self: Write own serialization
    [ProtoInclude(100, typeof(SpecularChannelContainer))]
    public class DiffuseChannelContainer : IEquatable<DiffuseChannelContainer>
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
        public static bool operator ==(DiffuseChannelContainer lhs, DiffuseChannelContainer rhs)
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
        public static bool operator !=(DiffuseChannelContainer lhs, DiffuseChannelContainer rhs)
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
        public bool Equals(DiffuseChannelContainer other)
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
                return Equals((DiffuseChannelContainer)obj);
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
    public class SpecularChannelContainer : DiffuseChannelContainer, IEquatable<SpecularChannelContainer>
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
        /// Indicates whether the SpecularChannelContainer is equal to another one.
        /// </summary>
        /// <param name="other">The SpecularChannelContainer to compare with this one.</param>
        /// <returns>
        /// true if the current SpecularChannelContainer is equal to the other; otherwise, false.
        /// </returns>
        public bool Equals(SpecularChannelContainer other)
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
                return Equals((SpecularChannelContainer)obj);
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
    public class BumpChannelContainer : IEquatable<BumpChannelContainer>
    {
        /// <summary>
        /// The texture to read the normal information from.
        /// </summary>
        [ProtoMember(1)]
        public string Texture;

        /// <summary>
        /// The intensity of the bumps.
        /// </summary>
        [ProtoMember(2)]
        public float Intensity;

        /// <summary>
        /// The bump texture tiling.
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
        public static bool operator ==(BumpChannelContainer lhs, BumpChannelContainer rhs)
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
        public static bool operator !=(BumpChannelContainer lhs, BumpChannelContainer rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Indicates whether the BumpChannelContainer is equal to another one.
        /// </summary>
        /// <param name="other">The BumpChannelContainer to compare with this one.</param>
        /// <returns>
        /// true if the current BumpChannelContainer is equal to the other; otherwise, false.
        /// </returns>
        public bool Equals(BumpChannelContainer other)
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
                return Equals((BumpChannelContainer)obj);

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
    [ProtoInclude(201, typeof(MaterialPBRComponent))]
    public class MaterialComponent : SceneComponentContainer, IEquatable<MaterialComponent>
    {
        #region Diffuse
        /// <summary>
        /// Gets a value indicating whether this instance has a diffuse channel.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has a diffuse channel; otherwise, <c>false</c>.
        /// </value>
        public bool HasDiffuse { get { return Diffuse != null; } }

        /// <summary>
        /// The diffuse channel.
        /// </summary>
        [ProtoMember(1)]
        public DiffuseChannelContainer Diffuse;
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
        public DiffuseChannelContainer Emissive;
        #endregion

        #region Bump
        /// <summary>
        /// Gets a value indicating whether this instance has a bump channel.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has a bump channel; otherwise, <c>false</c>.
        /// </value>
        public bool HasBump { get { return Bump != null; } }

        /// <summary>
        /// The bump channel.
        /// </summary>
        [ProtoMember(4)]
        public BumpChannelContainer Bump;
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
        public static bool operator ==(MaterialComponent lhs, MaterialComponent rhs)
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
        public static bool operator !=(MaterialComponent lhs, MaterialComponent rhs)
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

                return hash;
            }
        }

        /// <summary>
        /// Indicates whether the MaterialComponent is equal to another one.
        /// </summary>
        /// <param name="other">The MaterialComponent to compare with this one.</param>
        /// <returns>
        /// true if the current MaterialComponent is equal to the other; otherwise, false.
        /// </returns>
        public bool Equals(MaterialComponent other)
        {
            if (other is null)
                return false;

            return
                HasDiffuse == other.HasDiffuse &&
                Diffuse == other.Diffuse &&
                HasSpecular == other.HasSpecular &&
                Specular == other.Specular &&
                HasEmissive == other.HasEmissive &&
                Emissive == other.Emissive &&
                HasBump == other.HasBump &&
                Bump == other.Bump;
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
                return Equals((DiffuseChannelContainer)obj);
        }

        #endregion  
    }
}
