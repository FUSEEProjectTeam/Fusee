using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class SimpleType
    {
        [ProtoMember(1)]
        public SceneHeaderTest Header;
    }
}


[ProtoContract]
public struct SceneHeaderTest
{
    [ProtoMember(1)]
    public int Version;
    [ProtoMember(2)]
    public string Generator;
    [ProtoMember(3)]
    public string CreatedBy;
    [ProtoMember(4)]
    public string CreationDate;
}
