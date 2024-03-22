using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// Abstract base class for <see cref="ShaderEffect"/>, <see cref="SurfaceEffectBase"/> and <see cref="ComputeEffect"/>.
    /// </summary>
    public abstract class Effect : SceneComponent, IDisposable
    {
        /// <summary>
        /// Collection of all uniform parameters of this effect. See <see cref="IFxParamDeclaration"/>.
        /// </summary>
        public Dictionary<int, IFxParamDeclaration> UniformParameters { get; protected set; }

        /// <summary>
        /// The renderer states that are applied for this effect, e.g. the blend and alpha mode.
        /// </summary>
        public RenderStateSet RendererStates { get; set; }

        /// <summary>
        /// ShaderEffect event notifies observing ShaderEffectManager about property changes and the ShaderEffects's disposal.
        /// </summary>
        public EventHandler<EffectManagerEventArgs> EffectChanged { get; internal set; }

        /// <summary>
        /// Event arguments that are used in the <see cref="EffectManager"/>.
        /// </summary>
        public EffectManagerEventArgs EffectManagerEventArgs { get; internal set; }

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        public Guid UniqueIdentifier { get; } = Guid.NewGuid();

        private bool _disposed;

        /// <summary>
        /// Set effect parameter
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// <param name="value">Value of the uniform variable</param>
        public void SetFxParam<T>(string name, T value)
        {
            if (UniformParameters != null)
            {
                var hash = name.GetHashCode();
                SetFxParam(hash, value);
            }
        }

        /// <summary>
        /// Set effect parameter
        /// </summary>
        /// <param name="hash">Hash of the uniform variable. Retrieved by name.GetHashCode().</param>
        /// <param name="value">Value of the uniform variable</param>
        public void SetFxParam<T>(int hash, T value)
        {
            if (UniformParameters.ContainsKey(hash))
            {
                if (!UniformParameters[hash].SetValue(value)) return;

                EffectManagerEventArgs.Changed = UniformChangedEnum.Update;
                EffectManagerEventArgs.ChangedUniformHash = hash;

                EffectChanged?.Invoke(this, EffectManagerEventArgs);
            }
            else
            {
                Diagnostics.Warn($"Trying to set unknown parameter! Ignoring change....");
            }
        }

        /// <summary>
        /// Returns the value of a given shader effect variable
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// <returns></returns>
        public T GetFxParam<T>(string name)
        {
            var hash = name.GetHashCode();
            if (UniformParameters.TryGetValue(hash, out var dcl))
            {
                return ((FxParamDeclaration<T>)dcl).Value;
            }
            return default;
        }

        /// <summary>
        /// Determines whether two Effect instances are equal.
        /// </summary>
        /// <param name="obj">The other Effect.</param>
        public override bool Equals(object? obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (obj is not Effect p)
            {
                return false;
            }

            // Return true if the fields match:
            return (UniqueIdentifier == p.UniqueIdentifier);
        }

        /// <summary>
        /// Determines whether two Effect instances are equal.
        /// </summary>
        /// <param name="p">The other Effect.</param>
        public bool Equals(Effect p)
        {
            // If parameter is null return false:
            if (p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (UniqueIdentifier == p.UniqueIdentifier);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return UniqueIdentifier.GetHashCode();
        }

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
            if (!_disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                }
                EffectChanged?.Invoke(this, new EffectManagerEventArgs(UniformChangedEnum.Dispose));

                _disposed = true;
            }
        }

        /// <summary>
        /// Destructor calls <see cref="Dispose()"/> in order to fire MeshChanged event.
        /// </summary>
        ~Effect()
        {
            Dispose(false);
        }
    }
}