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
