using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace Examples.CoreProto
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
