using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// Abstract class that provides input for <see cref="ShaderEffect"/> and <see cref="SurfaceEffect"/>.
    /// </summary>
    public abstract class Effect : SceneComponent
    {
        /// <summary>
        /// Collection of all uniform parameters of this effect. See <see cref="IFxParamDeclaration"/>.
        /// </summary>
        public Dictionary<int, IFxParamDeclaration> ParamDecl { get; protected set; }

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
        public Suid SessionUniqueIdentifier { get; } = Suid.GenerateSuid();

        /// <summary>
        /// Set effect parameter
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// <param name="value">Value of the uniform variable</param>
        public void SetFxParam<T>(string name, T value)
        {
            if (ParamDecl != null)
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
            if (ParamDecl.ContainsKey(hash))
            {
                var dcl = ParamDecl[hash];
                
                if(!ParamDecl[hash].SetValue(value)) return;

                EffectManagerEventArgs.Changed = UniformChangedEnum.Update;
                EffectManagerEventArgs.ChangedUniformHash = hash;
                EffectManagerEventArgs.ChangedUniformValue = value;

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
            if (ParamDecl.TryGetValue(hash, out var dcl))
            {
                return ((FxParamDeclaration<T>)dcl).Value;
            }
            return default;
        }

        public override bool Equals(object obj)
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
            return (SessionUniqueIdentifier == p.SessionUniqueIdentifier);
        }

        public bool Equals(Effect p)
        {
            // If parameter is null return false:
            if (p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (SessionUniqueIdentifier == p.SessionUniqueIdentifier);
        }

        public override int GetHashCode()
        {
            return SessionUniqueIdentifier.GetHashCode();
        }
    }
}