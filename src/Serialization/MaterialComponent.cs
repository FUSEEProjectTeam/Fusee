using Fusee.Math.Core;
using ProtoBuf;

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
    public class MatChannelContainer
    {
        /// <summary>
        /// The color of the light componennt.
        /// </summary>
        [ProtoMember(1)]
        public float3 Color;

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
    }

    /// <summary>
    /// The specular channel definition. 
    /// </summary>
    [ProtoContract]
    public class SpecularChannelContainer : MatChannelContainer
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
    }

    /// <summary>
    /// If used, the material shows bumps defined by a normal map (a texture).
    /// </summary>
    [ProtoContract]
    public class BumpChannelContainer
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
    }

    /// <summary>
    /// Material definition. If contained within a node, the node's (and potentially child node's)
    /// geometry is rendered with the speicified material.
    /// </summary>
    [ProtoContract]
    public class MaterialComponent : SceneComponentContainer
    {
        #region Diffuse
        /// <summary>
        /// Gets a value indicating whether this instance has a diffuse channel.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has a diffuse channel; otherwise, <c>false</c>.
        /// </value>
        public bool HasDiffuse { get { return Diffuse != null; }}

        /// <summary>
        /// The diffuse channel.
        /// </summary>
        [ProtoMember(1)] 
        public MatChannelContainer Diffuse;
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
    }
}
