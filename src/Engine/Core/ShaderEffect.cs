using System;
using System.Collections.Generic;
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
    public class ShaderEffect: IDisposable
    {
        public readonly RenderStateSet[] States;
        public ShaderProgram[] CompiledShaders;
        public readonly string[] VertexShaderSrc;
        public readonly string[] PixelShaderSrc;
        public Dictionary<string, EffectParam> Parameters;
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
            EffectParam param;

            if (Parameters != null)
                if (Parameters.TryGetValue(name, out param))
                {
                    param.Value = value;
                    ShaderEffectChanged?.Invoke(this, new ShaderEffectEventArgs(this, ShaderEffectChangedEnum.CHANGED_EFFECT_PARAM));
                }
        }

        public object GetEffectParam(string name)
        {
            EffectParam param;
            if (Parameters.TryGetValue(name, out param))
            {
                return param.Value;
            }
            return null;
        }
    }

    public class ShaderEffectEventArgs : EventArgs
    {
        public ShaderEffect Effect { get; }
        public ShaderEffectChangedEnum Changed { get; }

        public ShaderEffectEventArgs(ShaderEffect effect, ShaderEffectChangedEnum changed)
        {
            Effect = effect;
            Changed = changed;
        }
    }

    public enum ShaderEffectChangedEnum
    {
        DISPOSE = 0,
        CHANGED_EFFECT_PARAM

    }

}
