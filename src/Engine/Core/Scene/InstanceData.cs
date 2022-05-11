using Fusee.Math.Core;
using Fusee.Engine.Common;
using System;

namespace Fusee.Engine.Core.Scene
{
    public class InstanceData : SceneComponent, IManagedInstanceData
    {
        /// <summary>
        /// MeshChanged event notifies observing MeshManager about property changes and the Mesh's disposal.
        /// </summary>
        public event EventHandler<InstanceDataChangedEventArgs> DataChanged;

        /// <summary>
        /// MeshChanged event notifies observing MeshManager about property changes and the Mesh's disposal.
        /// </summary>
        public event EventHandler<InstanceDataChangedEventArgs> DisposeData;

        public float3[] Translations { get; }

        public float3[] Rotations { get; }

        public float3[] Scales { get; }

        public float4[] Colors { get; }

        public int Amount { get; }

        public Suid SessionUniqueId { get; internal set; }

        public InstanceData(int amount, float3[] translations, float3[] rotations = null, float3[] scales = null, float4[] colors = null)
        {
            Amount = amount;
            if (Amount != translations.Length)
                throw new ArgumentOutOfRangeException();
            Translations = translations;

            if(scales != null)
            {
                if (Amount != scales.Length)
                    throw new ArgumentOutOfRangeException();
                Scales = scales;
            }

            if (rotations != null)
            {
                if (Amount != rotations.Length)
                    throw new ArgumentOutOfRangeException();
                Rotations = rotations;
            }

            if (colors != null)
            {
                if (Amount != colors.Length)
                    throw new ArgumentOutOfRangeException();
                Colors = colors;
            }
        }

    }
}
