using System;
using ProtoBuf;
using System.Collections.Generic;

namespace ProtoBufTest
{
    [ProtoContract]
    public class Person
    {
        [ProtoMember(1)]
        public int Id;
        [ProtoMember(2)]
        public string Name;
        [ProtoMember(3)]
        public int Age;
    }
}
