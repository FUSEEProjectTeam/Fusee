using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Part of the <see cref="Material"/> defining material settings.
    /// Used in the different lighting calculation components.
    /// </summary>
    public class MatChannel : IEquatable<MatChannel>
    {
        /// <summary>
        /// The color of the light component.
        /// </summary>       
        public float4 Color;

        /// <summary>
        /// If not null, the texture to use as the color.
        /// </summary>      
        public string Texture;

        /// <summary>
        /// The percentage how to mix Color and Texture.
        /// </summary>        
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
        public static bool operator ==(MatChannel lhs, MatChannel rhs)
        {
            // Check for null on left side.
            if (lhs is null)
            {
                return rhs is null;
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
        public static bool operator !=(MatChannel lhs, MatChannel rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Indicates whether the MatChannel is equal to another one.
        /// </summary>
        /// <param name="other">The MatChannel to compare with this one.</param>
        /// <returns>
        /// true if the current MatChannel is equal to the other; otherwise, false.
        /// </returns>
        public bool Equals(MatChannel other)
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
                return Equals((MatChannel)obj);
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
    public class SpecularChannel : MatChannel, IEquatable<SpecularChannel>
    {
        /// <summary>
        /// The material's shininess.
        /// </summary>        
        public float Shininess;

        /// <summary>
        /// The specular intensity.
        /// </summary>        
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
        public static bool operator ==(SpecularChannel lhs, SpecularChannel rhs)
        {
            // Check for null on left side.
            if (lhs is null)
            {
                return rhs is null;
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
        /// Indicates whether the SpecularChannel is equal to another one.
        /// </summary>
        /// <param name="other">The SpecularChannel to compare with this one.</param>
        /// <returns>
        /// true if the current SpecularChannel is equal to the other; otherwise, false.
        /// </returns>
        public bool Equals(SpecularChannel other)
        {
            if (other is null)
                return false;
            return other.Shininess == Shininess && other.Intensity == Intensity;
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
                hash = hash * 29 + Intensity.GetHashCode();
                return hash;
            }
        }

        #endregion
    }

    /// <summary>
    /// If used, the material shows bumps defined by a normal map (a texture).
    /// </summary>
    public class BumpChannel : IEquatable<BumpChannel>
    {
        /// <summary>
        /// The texture to read the normal information from.
        /// </summary>        
        public string Texture;

        /// <summary>
        /// The intensity of the bumps.
        /// </summary>       
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
        public static bool operator ==(BumpChannel lhs, BumpChannel rhs)
        {
            // Check for null on left side.
            if (lhs is null)
            {
                return rhs is null;
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
        public static bool operator !=(BumpChannel lhs, BumpChannel rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Indicates whether the BumpChannel is equal to another one.
        /// </summary>
        /// <param name="other">The BumpChannel to compare with this one.</param>
        /// <returns>
        /// true if the current BumpChannel is equal to the other; otherwise, false.
        /// </returns>
        public bool Equals(BumpChannel other)
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
                return Equals((BumpChannel)obj);

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
    public class Material : SceneComponent, IEquatable<Material>
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
        public MatChannel Diffuse;

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
        public MatChannel Emissive;
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
        public BumpChannel Bump;
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
        public static bool operator ==(Material lhs, Material rhs)
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
        public static bool operator !=(Material lhs, Material rhs)
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
        public bool Equals(Material other)
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
                return Equals((MatChannel)obj);
        }

        #endregion  
    }
}
