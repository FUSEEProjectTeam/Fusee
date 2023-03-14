using Fusee.Engine.Common;
using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// The position of each instance. This array needs to be as long as <see cref="Amount"/>.
        /// </summary>
        public float3[] Positions
        {
            get => _positions;
            set
            {
                if (Amount != value.Length)
                    throw new ArgumentOutOfRangeException();
                _positions = value;
                DataChanged?.Invoke(this, new InstanceDataChangedEventArgs(this, InstanceDataChangedEnum.Transform));
            }
        }
        private float3[] _positions;

        /// <summary>
        /// The rotation of each instance. This array needs to be as long as <see cref="Amount"/>.
        /// </summary>
        public float3[]? Rotations
        {
            get => _rotations;
            set
            {
                if (Amount != value?.Length)
                    throw new ArgumentOutOfRangeException();
                _rotations = value;
                DataChanged?.Invoke(this, new InstanceDataChangedEventArgs(this, InstanceDataChangedEnum.Transform));
            }
        }
        private float3[]? _rotations;

        /// <summary>
        /// The scale of each instance. This array needs to be as long as <see cref="Amount"/>.
        /// </summary>
        public float3[]? Scales
        {
            get => _scales;
            set
            {
                if (Amount != value?.Length)
                    throw new ArgumentOutOfRangeException();
                _scales = value;
                DataChanged?.Invoke(this, new InstanceDataChangedEventArgs(this, InstanceDataChangedEnum.Transform));
            }
        }
        private float3[]? _scales;

        /// <summary>
        /// The color of each instance. This array needs to be as long as <see cref="Amount"/>.
        /// </summary>
        public float4[]? Colors
        {
            get => _colors;
            set
            {
                if (Amount != value?.Length)
                    throw new ArgumentOutOfRangeException();
                _colors = value;
                DataChanged?.Invoke(this, new InstanceDataChangedEventArgs(this, InstanceDataChangedEnum.Colors));
            }
        }
        private float4[]? _colors;

        /// <summary>
        /// The amount of instances that will be rendered.
        /// </summary>
        public int Amount { get; }

        /// <summary>
        /// The unique id of this object.
        /// </summary>
        public Suid SessionUniqueId { get; } = Suid.GenerateSuid();

        /// <summary>
        /// Creates a new instance of type <see cref="InstanceData"/>. Will fail if the length of a provided array doesn't match <see cref="Amount"/>.
        /// </summary>
        /// <param name="amount">The amount of instances that will be rendered.</param>
        /// <param name="positions">The position of each instance.</param>
        /// <param name="rotations">The rotation of each instance.</param>
        /// <param name="scales">The scale of each instance.</param>
        /// <param name="colors">The color of each instance.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public InstanceData(int amount, float3[] positions, float3[]? rotations = null, float3[]? scales = null, float4[]? colors = null)
        {
            Amount = amount;
            if (Amount != positions.Length)
                throw new ArgumentOutOfRangeException();
            _positions = positions;

            if (scales != null)
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

                }
                DisposeData?.Invoke(this, new InstanceDataChangedEventArgs(this, InstanceDataChangedEnum.Disposed));

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