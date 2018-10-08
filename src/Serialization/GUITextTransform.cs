using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class GUITextTransform : TransformComponent
    {
        public uint FontSize;
    }
}
