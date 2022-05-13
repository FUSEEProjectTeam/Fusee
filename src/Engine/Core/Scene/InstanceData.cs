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

        public float3[] Translations {
            get => _translations;
            set
            {
                _translations = value;
                DataChanged?.Invoke(this, new InstanceDataChangedEventArgs(this, InstanceDataChangedEnum.Transform));
            }
        }
        private float3[] _translations;

        public float3[] Rotations {
            get => _rotations;
            set
            {
                _rotations = value;
                DataChanged?.Invoke(this, new InstanceDataChangedEventArgs(this, InstanceDataChangedEnum.Transform));
            }
        }
        private float3[] _rotations;

        public float3[] Scales {
            get => _scales;
            set
            {
                _scales = value;
                DataChanged?.Invoke(this, new InstanceDataChangedEventArgs(this, InstanceDataChangedEnum.Transform));
            }
        }
        private float3[] _scales;

        public float4[] Colors {
            get => _colors;
            set
            {
                _colors = value;
                DataChanged?.Invoke(this, new InstanceDataChangedEventArgs(this, InstanceDataChangedEnum.Colors));
            }
        }
        private float4[] _colors;

        public int Amount { get; }

        public Suid SessionUniqueId { get; } = Suid.GenerateSuid();

        public InstanceData(int amount, float3[] translations, float3[] rotations = null, float3[] scales = null, float4[] colors = null)
        {
            Amount = amount;
            if (Amount != translations.Length)
                throw new ArgumentOutOfRangeException();
            _translations = translations;

            if(scales != null)
            {
                if (Amount != scales.Length)
                    throw new ArgumentOutOfRangeException();
                _scales = scales;
            }

            if (rotations != null)
            {
                if (Amount != rotations.Length)
                    throw new ArgumentOutOfRangeException();
                _rotations = rotations;
            }

            if (colors != null)
            {
                if (Amount != colors.Length)
                    throw new ArgumentOutOfRangeException();
                _colors = colors;
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
