using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    public interface IInstanceData
    {
        public float3[] Translations { get; }

        public float4[] Colors { get; }

        public int Amount { get; }
    }
}
