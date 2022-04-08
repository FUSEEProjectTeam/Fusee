using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core.Scene
{
    internal class InstanceData : SceneComponent
    {
        public float3[] Translations { get; }

        public float4[] Colors { get; }

        public int Amount { get; }

        public InstanceData(int amount, float3[] translations, float4[] colors = null)
        {
            Amount = amount;
            if (Amount != translations.Length)
                throw new ArgumentOutOfRangeException();
            Translations = new float3[amount];

            if (colors != null)
            {
                if (Amount != colors.Length)
                    throw new ArgumentOutOfRangeException();
                Colors = new float4[amount];
            }
            else
                Colors = null;
        }

    }
}
