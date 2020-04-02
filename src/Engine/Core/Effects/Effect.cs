using Fusee.Base.Core;
using Fusee.Serialization;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// Abstract class that provides input for <see cref="ShaderEffect"/> and <see cref="SurfaceEffect"/>.
    /// </summary>
    public abstract class Effect
    {
        /// <summary>
        /// Collection of all uniform parameters of this effect. See <see cref="IFxParamDeclaration"/>.
        /// </summary>
        public Dictionary<string, IFxParamDeclaration> ParamDecl { get; protected set; }

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
                if (ParamDecl.ContainsKey(name))
                {
                    if (ParamDecl[name] != null)
                        if (ParamDecl[name].Equals(value)) return;

                    //Implemented using reflections and not "(FxParamDeclaration<T>)ParamDecl[name]" because 
                    //we get a InvalidCast exception when coming from the RC (Render(Mesh)) and T is of type "object" but ParamDecl[name] "T" isn't.                    
                    ParamDecl[name].GetType().GetField("Value").SetValue(ParamDecl[name], value);

                    EffectManagerEventArgs.Changed = ChangedEnum.UNIFORM_VAR_UPDATED;
                    EffectManagerEventArgs.ChangedEffectVarName = name;
                    EffectManagerEventArgs.ChangedEffectVarValue = value;

                    EffectChanged?.Invoke(this, EffectManagerEventArgs);
                }
                else
                {
                    Diagnostics.Warn($"Trying to set unknown parameter {name}! Ignoring change....");
                }
            }
        }

        /// <summary>
        /// Returns the value of a given shader effect variable
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// <returns></returns>
        public T GetFxParam<T>(string name)
        {
            if (ParamDecl.TryGetValue(name, out var dcl))
            {
                return ((FxParamDeclaration<T>)dcl).Value;
            }
            return default;
        }
    }
}
