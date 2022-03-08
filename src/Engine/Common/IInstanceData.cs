using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Common
{
    public interface IInstanceData
    {
        public float3[] Translations { get; }

        public float4[] Colors { get; }

        public int Amount { get; }
    }
}
