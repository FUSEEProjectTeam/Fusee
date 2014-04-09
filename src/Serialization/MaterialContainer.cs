using System.ComponentModel;
using Fusee.Math;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    // taken from http://www.codeproject.com/Articles/642677/Protobuf-net-the-unofficial-manual
    // Here is where I start disliking Protobuf...Note-to-self: Write own serialization
    [ProtoInclude(100, typeof(SpecularChannelContainer))]
    public class MatChannelContainer
    {
        [ProtoMember(1)]
        public float3 Color;

        [ProtoMember(2)]
        public string Texture;

        [ProtoMember(3)]
        public float Mix;
    }

    [ProtoContract]
    public class SpecularChannelContainer : MatChannelContainer
    {
        [ProtoMember(1)]
        public float Shininess;

        [ProtoMember(2)]
        public float Intensity;
    }

    [ProtoContract]
    public class BumpChannelContainer
    {
        [ProtoMember(1)] 
        public string Texture;

        [ProtoMember(2)] 
        public float Intensity;
    }

    [ProtoContract]
    public class MaterialContainer
    {
        #region Diffuse
        public bool HasDiffuse { get { return Diffuse != null; }}

        [ProtoMember(1)] 
        public MatChannelContainer Diffuse;
        #endregion

        #region Specular
        public bool HasSpecular { get { return Specular != null; } }

        [ProtoMember(2)]
        public SpecularChannelContainer Specular;
        #endregion

        #region Emissive
        public bool HasEmissive { get { return Emissive != null; } }

        [ProtoMember(3)]
        public MatChannelContainer Emissive;
        #endregion

        #region Bump
        public bool HasBump { get { return Bump != null; } }

        [ProtoMember(4)]
        public BumpChannelContainer Bump;
        #endregion
    }
}
