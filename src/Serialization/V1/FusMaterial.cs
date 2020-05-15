using Fusee.Math.Core;
using ProtoBuf;

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
    public class MatChannelContainer
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
    /// If used, the material shows bumps defined by a normal map (a texture in RGB).
    /// </summary>
    [ProtoContract]
    public class NormapMapChannelContainer
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
    }

}