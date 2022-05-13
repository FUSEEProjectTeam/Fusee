using Fusee.Math.Core;
using Fusee.Engine.Common;
using System;

namespace Fusee.Engine.Core.Scene
{
    public class InstanceData : SceneComponent, IDisposable, IManagedInstanceData
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

        public Suid SessionUniqueId { get; } = new();

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


        #region IDisposable Support

        private bool disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    DisposeData?.Invoke(this, new InstanceDataChangedEventArgs(this, InstanceDataChangedEnum.Disposed));
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Finalizers (historically referred to as destructors) are used to perform any necessary final clean-up when a class instance is being collected by the garbage collector.
        /// </summary>
        ~InstanceData()
        {
            Dispose(false);
        }

        #endregion


    }
}
