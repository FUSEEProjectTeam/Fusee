using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// Class that contains information on how to build a compute shader program.
    /// </summary>
    public class ComputeShader : Effect, IDisposable
    {
        /// <summary>
        /// The Compute Shader code.
        /// </summary>
        public string ComputeShaderSrc { get; protected set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="shaderCode">The compute shader code.</param>
        /// <param name="effectParameters">The uniform parameters as collections of <see cref="IFxParamDeclaration"/>.</param>
        public ComputeShader(string shaderCode, IEnumerable<IFxParamDeclaration> effectParameters)
        {
            ParamDecl = new Dictionary<int, IFxParamDeclaration>();
            ComputeShaderSrc = shaderCode;

            if (effectParameters != null)
            {
                foreach (var param in effectParameters)
                    ParamDecl.Add(param.Hash, param);
            }

            RendererStates = RenderStateSet.Default;
            EffectManagerEventArgs = new EffectManagerEventArgs(UniformChangedEnum.Unchanged);
        }

        /// <summary>
        /// Destructor calls <see cref="Dispose"/> in order to fire MeshChanged event.
        /// </summary>
        ~ComputeShader()
        {
            Dispose();
        }

        /// <summary>
        /// Is called when GC of this shader effect kicks in
        /// </summary>
        public void Dispose()
        {
            EffectChanged?.Invoke(this, new EffectManagerEventArgs(UniformChangedEnum.Dispose));
        }
    }
}