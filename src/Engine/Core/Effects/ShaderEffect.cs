using System;
using System.Collections.Generic;
using Fusee.Base.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// A ShaderEffect contains a list of render passes with each pass item being a combination of a set of render states, and Shader Programs (the code running on the GPU).
    /// In addition a ShaderEffect contains the actual values for all the shaders' (uniform) variables.
    /// </summary>
    public class ShaderEffect : Effect, IDisposable
    {
        /// <summary>
        /// Vertex shaders of all passes
        /// </summary>
        public string VertexShaderSrc { get; protected set; }

        /// <summary>
        /// Pixel- or fragment shader of all passes
        /// </summary>
        public string PixelShaderSrc { get; protected set; }

        /// <summary>
        /// Geometry of all passes
        /// </summary>
        public string GeometryShaderSrc { get; protected set; }

        //TODO: delete if ShaderEffectProtoPixel is redundant
        public ShaderEffect()
        {

        }

        /// <summary>
        /// The constructor to create a shader effect.
        /// </summary>
        /// <param name="effectPass">See <see cref="FxPassDeclaration"/>.</param>
        /// <param name="effectParameters">A list of (uniform) parameters possibly occurring in one of the shaders in the various passes.
        /// Each array entry consists of the parameter's name and its initial value. The concrete type of the object also indicates the
        /// parameter's type.
        /// </param>
        /// <remarks>Make sure to list any parameter in any of the different passes' shaders you want to change later on in the effectParameters
        /// list. Shaders must not contain parameters with names listed in the effectParameters but declared with different types than those of 
        /// the respective default values given here.</remarks>
        public ShaderEffect(FxPassDeclaration effectPass, IEnumerable<IFxParamDeclaration> effectParameters)
        {
            ParamDecl = new Dictionary<string, IFxParamDeclaration>();

            RendererStates = effectPass.StateSet;
            VertexShaderSrc = effectPass.VS;
            PixelShaderSrc = effectPass.PS;
            GeometryShaderSrc = effectPass.GS;


            if (effectParameters != null)
            {
                foreach (var param in effectParameters)
                {
                    //var val = param.GetType().GetField("Value").GetValue(param);
                    ParamDecl.Add(param.Name, param);
                }
            }

            EffectEventArgs = new EffectEventArgs(this, ChangedEnum.UNCHANGED);
        }

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
                    Diagnostics.Warn("Trying to set unknown parameter! Ignoring change....");
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

        /// <summary>
        /// Returns the value of a given shader effect variable
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// /// <param name="obj">The value. Return null if no parameter was found.</param>
        /// <returns></returns>
        public void GetFxParam<T>(string name, out T obj)
        {
            obj = default;
            if (ParamDecl.TryGetValue(name, out var dcl))
                obj = ((FxParamDeclaration<T>)dcl).Value;
        }

        /// <summary>
        /// Destructor calls <see cref="Dispose"/> in order to fire MeshChanged event.
        /// </summary>
        ~ShaderEffect()
        {
            Dispose();
        }

        /// <summary>
        /// Is called when GC of this shader effect kicks in
        /// </summary>
        public void Dispose()
        {
            EffectChanged?.Invoke(this, new EffectEventArgs(this, ChangedEnum.DISPOSE));
        }
    }
}
