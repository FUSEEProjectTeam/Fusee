using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ProtoBuf;

namespace Fusee.Math.Core
{

    [StructLayout(LayoutKind.Sequential)]
    [ProtoContract]
    public struct MinMaxRect
    {
        [ProtoMember(1)]
        public float2 Min;

        [ProtoMember(2)]
        public float2 Max;

        public float2 Center
        {
            get
            { // TODO
              throw new NotImplementedException();
            }
        }

        public float2 Size
        {
            get
            { // TODO
                throw new NotImplementedException();
            }
        }

        public MinMaxRect FromCenterSize(float2 center, float size)
        {
            throw new NotImplementedException();
        }
    }
}
