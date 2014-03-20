using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class MatChannelContainer
    {
        [ProtoMember(0)]
        public float3 Color;

        [ProtoMember(1)]
        public string Texure;

        [ProtoMember(2)]
        public float Mix;
    }

    public class SpecularChannelContainer : MatChannelContainer
    {
        [ProtoMember(0)]
        public float Shininess;
    }

    [ProtoContract]
    public class MaterialContainer
    {
        #region Diffuse
        public bool HasDiffuse { get { return Diffuse != null; }}

        [ProtoMember(0)] 
        public MatChannelContainer Diffuse;
        #endregion

        #region Specular
        public bool HasSpecular { get { return Diffuse != null; } }

        [ProtoMember(0)]
        public SpecularChannelContainer Specular;
        #endregion


    }
}
