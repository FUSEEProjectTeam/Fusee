using System;
using System.Collections.Generic;
using System.Dynamic;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// An effect pass declaration contains the vertex and pixel shader source code as well as a <see cref="RenderStateSet"/>
    /// declaration for the rendering pass declared by this instance.
    /// </summary>
    public struct EffectPassDeclaration
    {
        public RenderStateSet StateSet;
        public string VS;
        public string PS;
    }

    public struct EffectParameterDeclaration
    {
        public string Name;
        public object Value;
    }

    public sealed class EffectParam
    {
        public ShaderParamInfo Info;
        public Object Value;
        public List<int> ShaderInxs;
    }

    /// <summary>
    /// A ShaderEffect contains a list of render passes with each pass item being a combination of a set of render states, and a
    /// pair of Pixel and Vertex Shader Programs (the code running on the GPU).
    /// In addition a ShaderEffect contains the actual values for all the shaders' (uniform) variables.
    /// </summary>
    public class ShaderEffect : DynamicObject, IDisposable
    {
        public readonly RenderStateSet[] States;
        public ShaderProgram[] CompiledShaders;
        public readonly string[] VertexShaderSrc;
        public readonly string[] PixelShaderSrc;
        public Dictionary<string, object> Parameters;
        public List<List<EffectParam>> ParamsPerPass;
        public Dictionary<string, object> ParamDecl;

        // Event ShaderEffect changes
        /// <summary>
        /// ShaderEffect event notifies observing ShaderEffectManager about property changes and the ShaderEffects's disposal.
        /// </summary>
        public event EventHandler<ShaderEffectEventArgs> ShaderEffectChanged;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        public readonly Suid SessionUniqueIdentifier = Suid.GenerateSuid();

        /// <summary>
        /// The constructor to create a shader effect.
        /// </summary>
        /// <param name="effectPasses">The ordered array of <see cref="EffectPassDeclaration"/> items. The first item
        /// in the array is the first pass applied to rendered geometry, and so on.</param>
        /// <param name="effectParameters">A list of (uniform) parameters possibliy occurring in one of the shaders in the various passes.
        /// Each array entry consists of the parameter's name and its initial value. The concrete type of the object also indicates the
        /// parameter's type.
        /// </param>
        /// <remarks>Make sure to list any parameter in any of the different passes' shaders you want to change later on in the effectParameters
        /// list. Shaders must not contain paramaeters with names listed in the effectParameters but declared with different types than those of 
        /// the respective default values given here.</remarks>
        public ShaderEffect(EffectPassDeclaration[] effectPasses, IEnumerable<EffectParameterDeclaration> effectParameters)
        {
            if (effectPasses == null || effectPasses.Length == 0)
                throw new ArgumentNullException("effectPasses", "must not be null and must contain at least one pass");

            int nPasses = effectPasses.Length;

            States = new RenderStateSet[nPasses];
            CompiledShaders = new ShaderProgram[nPasses];
            VertexShaderSrc = new string[nPasses];
            PixelShaderSrc = new string[nPasses];

            for (int i = 0; i < nPasses; i++)
            {
                States[i] = effectPasses[i].StateSet;
                VertexShaderSrc[i] = effectPasses[i].VS;
                PixelShaderSrc[i] = effectPasses[i].PS;
            }

            ParamDecl = new Dictionary<string, object>();

            if (effectParameters != null)
            {
                foreach (var param in effectParameters)
                {
                    ParamDecl.Add(param.Name, param.Value);
                }
            }
        }

        /// <summary>
        /// Destructor calls <see cref="Dispose"/> in order to fire MeshChanged event.
        /// </summary>
        ~ShaderEffect()
        {
            Dispose();
        }

        public void Dispose()
        {
            ShaderEffectChanged?.Invoke(this, new ShaderEffectEventArgs(this, ShaderEffectChangedEnum.DISPOSE));
        }

            public void SetEffectParam(string name, object value)
            {
                object pa;

                if (Parameters != null)
                {
                    if (Parameters.TryGetValue(name, out pa))
                    {
                        var param = (EffectParam) pa;
                        // do nothing if new value = old value
                        if (param.Value.Equals(value)) return; // TODO: Write a better compare method

                        param.Value = value;
                        ShaderEffectChanged?.Invoke(this,
                            new ShaderEffectEventArgs(this, ShaderEffectChangedEnum.CHANGED_EFFECT_PARAM, param));
                    }
                    else
                    {
                        if(name != null && value != null)
                            // not in Parameters, try to get it anyway through ShaderProgram
                            ShaderEffectChanged?.Invoke(this, new ShaderEffectEventArgs(this, ShaderEffectChangedEnum.CHANGED_UNKNOWN_EFFECT_PARAM, null, name, value));
                }
                      
                }
                   
            }

            public object GetEffectParam(string name)
            {
                object pa;
                if (Parameters.TryGetValue(name, out pa))
                {
                    var param = (EffectParam) pa;
                    return param.Value;
                }
                return null;
            }

        // This property returns the number of elements
        // in the inner dictionary.
        public int Count => Parameters.Count;

        // If you try to get a value of a property 
        // not defined in the class, this method is called.
        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = binder.Name.ToLower();

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return Parameters.TryGetValue(name, out result);
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            Parameters[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
    }

    public class ShaderEffectEventArgs : EventArgs
    {
        public ShaderEffect Effect { get; }
        public ShaderEffectChangedEnum Changed { get; }
        public EffectParam EffectParameter { get; }

        public string UnknownUniformName { get; }
        public object UnknownUniformObject { get; }

        public ShaderEffectEventArgs(ShaderEffect effect, ShaderEffectChangedEnum changed, EffectParam effectParam = null, string unknownUniformName = null, object unknownUniformObject = null)
        {
            Effect = effect;
            Changed = changed;
            if(effectParam != null)
                EffectParameter = effectParam;

            if (unknownUniformName != null && unknownUniformObject != null)
            {
                UnknownUniformName = unknownUniformName;
                UnknownUniformObject = unknownUniformObject;
            }
               
        }
    }

    public enum ShaderEffectChangedEnum
    {
        DISPOSE = 0,
        CHANGED_EFFECT_PARAM,
        CHANGED_UNKNOWN_EFFECT_PARAM

    }

}