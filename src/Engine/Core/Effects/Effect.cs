using Fusee.Base.Core;
using Fusee.Serialization;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{
    public interface IEffect
    {
        RenderStateSet RendererStates { get; set; }

        /// <summary>
        /// ShaderEffect event notifies observing ShaderEffectManager about property changes and the ShaderEffects's disposal.
        /// </summary>
        EventHandler<EffectEventArgs> EffectChanged { get; }

        EffectEventArgs EffectEventArgs { get; }

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        Suid SessionUniqueIdentifier { get; }

        void SetFxParam<T>(string name, T value);

        T GetFxParam<T>(string name);

    }

    public abstract class Effect : IEffect
    {
        public Dictionary<string, IFxParamDeclaration> ParamDecl { get; protected set; }

        public RenderStateSet RendererStates { get; set; }

        /// <summary>
        /// ShaderEffect event notifies observing ShaderEffectManager about property changes and the ShaderEffects's disposal.
        /// </summary>
        public EventHandler<EffectEventArgs> EffectChanged { get; internal set; }

        public EffectEventArgs EffectEventArgs { get; internal set; }

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

                    EffectEventArgs.Changed = ChangedEnum.UNIFORM_VAR_UPDATED;
                    EffectEventArgs.ChangedEffectVarName = name;
                    EffectEventArgs.ChangedEffectVarValue = value;

                    EffectChanged?.Invoke(this, EffectEventArgs);
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
