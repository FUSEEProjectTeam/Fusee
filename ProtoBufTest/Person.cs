using System;
using ProtoBuf;
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
