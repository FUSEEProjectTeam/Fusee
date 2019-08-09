using System;
using System.Collections.Generic;
using System.Dynamic;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// An effect pass declaration contains the vertex and pixel shader source code as well as a <see cref="RenderStateSet"/>
    /// declaration for the rendering pass declared by this instance.
    /// </summary>
    public struct EffectPassDeclaration
    {
        /// <summary>
        /// The  <see cref="RenderStateSet"/> of the current effect pass.  
        /// </summary>
        public RenderStateSet StateSet;
        /// <summary>
        /// Vertexshader as string
        /// </summary>
        // ReSharper disable InconsistentNaming
        public string VS;
        /// <summary>
        /// Pixel- or fragmentshader as string
        /// </summary>
        public string PS;
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// A datatype for the list of (uniform) parameters possibliy occurring in one of the shaders in the various passes.
    /// Each of this array entry consists of the parameter's name and its initial value. The concrete type of the object also indicates the
    /// parameter's type.
    /// </summary>
    public struct EffectParameterDeclaration
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name;
        /// <summary>
        /// Value
        /// </summary>
        public object Value;
    }


    /// <summary>
    /// A ShaderEffect contains a list of render passes with each pass item being a combination of a set of render states, and a
    /// pair of Pixel and Vertex Shader Programs (the code running on the GPU).
    /// In addition a ShaderEffect contains the actual values for all the shaders' (uniform) variables.
    /// </summary>
    public class ShaderEffect : DynamicObject, IDisposable
    {
        /// <summary>
        /// The uniform parameter and value of a <see cref="ShaderEffect"/>
        /// </summary>
        public readonly Dictionary<string, object> ParamDecl;

        /// <summary>
        /// List of <see cref="RenderStateSet"/>
        /// </summary>
        public readonly RenderStateSet[] States;

        /// <summary>
        /// Vertexshaders of all passes
        /// </summary>
        public readonly string[] VertexShaderSrc;
        /// <summary>
        /// Pixel- or fragmentshader of all passes
        /// </summary>
        public readonly string[] PixelShaderSrc;

        // Event ShaderEffect changes
        /// <summary>
        /// ShaderEffect event notifies observing ShaderEffectManager about property changes and the ShaderEffects's disposal.
        /// </summary>
        internal event EventHandler<ShaderEffectEventArgs> ShaderEffectChanged;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        internal readonly Suid SessionUniqueIdentifier = Suid.GenerateSuid();

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
                throw new ArgumentNullException(nameof(effectPasses), "must not be null and must contain at least one pass");

            var nPasses = effectPasses.Length;

            States = new RenderStateSet[nPasses];
            
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

        /// <summary>
        /// Is called when GC of this shadereffect kicks in
        /// </summary>
        public void Dispose()
        {
            ShaderEffectChanged?.Invoke(this, new ShaderEffectEventArgs(this, ShaderEffectChangedEnum.DISPOSE));
        }

        /// <summary>
        /// Set effect parameter
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// <param name="value">Value of the uniform variable</param>
        public void SetEffectParam(string name, object value)
        {
            object param;

            if (ParamDecl != null)
            {
                if (ParamDecl.TryGetValue(name, out param))
                {
                    if (param != null)
                    {// do nothing if new value = old value
                        if (param.Equals(value)) return; // TODO: Write a better compare method! 
                    }

                    ParamDecl[name] = value;
                    ShaderEffectChanged?.Invoke(this, new ShaderEffectEventArgs(this, ShaderEffectChangedEnum.UNIFORM_VAR_UPDATED, name, value));
                }
                else
                {
                    // not in Parameters yet, add it and call uniform_var_changed!
                    ParamDecl.Add(name, value);
                    ShaderEffectChanged?.Invoke(this, new ShaderEffectEventArgs(this, ShaderEffectChangedEnum.UNIFORM_VAR_ADDED));
                }
            }
        }


        /// <summary>
        /// Returns the value of a given shadereffect variable
        /// <remarks>THIS IS NOT THE ACTUAL UNIFORM VALUE</remarks>
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// <returns></returns>
        public object GetEffectParam(string name)
        {
                object pa;
                if (ParamDecl.TryGetValue(name, out pa))
                {
                    return pa;
                }
                return null;
        }
     
        // This property returns the number of elements
        // in the inner dictionary.
        /// <summary>
        /// Size of <see cref="ParamDecl"/>
        /// Needed for <see cref="DynamicObject"/>
        /// </summary>
        public int Count => ParamDecl.Count;

        // If you try to get a value of a property 
        // not defined in the class, this method is called.
        /// <summary>
        /// Returns value of property <see cref="ParamDecl"/>.
        /// Needed for <see cref="DynamicObject"/>
        /// </summary>
        /// <param name="binder">Name</param>
        /// <param name="result">Result, in this case the value of one <see cref="ParamDecl"/> object.</param>
        /// <returns></returns>
        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = binder.Name;

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.

            return ParamDecl.TryGetValue(name, out result);
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        /// <summary>
        /// Set a uniform variable
        /// Needed for <see cref="DynamicObject"/>
        /// </summary>
        /// <param name="binder">Name of the uniform variable</param>
        /// <param name="value">Value of the uniform variable as <see cref="EffectParam"/>EffectParam</param>
        /// <returns>Element found and from type EffectParam</returns>
        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            object result;

            if (!ParamDecl.TryGetValue(binder.Name, out result))
                return false;
            
            SetEffectParam(binder.Name, value);

            return true;
        }

        internal class ShaderEffectEventArgs : EventArgs
    {
        internal ShaderEffect Effect { get; }
        internal ShaderEffectChangedEnum Changed { get; }
        internal EffectParam EffectParameter { get; }
        internal string ChangedEffectName { get;  }
        internal object ChangedEffectValue { get; }

        internal ShaderEffectEventArgs(ShaderEffect effect, ShaderEffectChangedEnum changed, string changedName = null, object changedValue = null)
        {
            Effect = effect;
            Changed = changed;

            if (changedName == null || changedValue == null) return;

            ChangedEffectName = changedName;
            ChangedEffectValue = changedValue;
        }
    }

    internal enum ShaderEffectChangedEnum
    {
        // ReSharper disable InconsistentNaming
        DISPOSE = 0,
        UNIFORM_VAR_UPDATED,
        UNIFORM_VAR_ADDED
        // ReSharper restore InconsistentNaming
    }

    }
}
