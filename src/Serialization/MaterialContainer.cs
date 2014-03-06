using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class MaterialContainer
    {
        #region Diffuse
        [ProtoMember(0)] 
        public bool HasDiffuse;

        [ProtoMember(1)]
        public float3 DiffuseColor;

        [ProtoMember(2)]
        public string DiffuseTexure;
        #endregion

        #region Specular
        [ProtoMember(3)]
        public bool HasSpecular;

        [ProtoMember(4)]
        public float3 SpecularColor;

        [ProtoMember(5)]
        public float SpecularIntensity;

        [ProtoMember(6)]
        public float SpecularShininess;
        #endregion
    }
}
