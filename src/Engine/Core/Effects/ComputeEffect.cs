using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// Class that contains information on how to build a compute shader program.
    /// </summary>
    public class ComputeEffect : Effect
    {
        /// <summary>
        /// The Compute Shader code.
        /// </summary>
        public string ComputeShaderSrc { get; protected set; }

        private bool disposed;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="shaderCode">The compute shader code.</param>
        /// <param name="effectParameters">The uniform parameters as collections of <see cref="IFxParamDeclaration"/>.</param>
        public ComputeEffect(string shaderCode, IEnumerable<IFxParamDeclaration> effectParameters)
        {
            UniformParameters = new Dictionary<int, IFxParamDeclaration>();
            ComputeShaderSrc = shaderCode;

            if (effectParameters != null)
            {
                foreach (var param in effectParameters)
                    UniformParameters.Add(param.Hash, param);
            }

            RendererStates = RenderStateSet.Default;
            EffectManagerEventArgs = new EffectManagerEventArgs(UniformChangedEnum.Unchanged);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
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
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                }
                EffectChanged?.Invoke(this, new EffectManagerEventArgs(UniformChangedEnum.Dispose));

                disposed = true;
            }
        }

        /// <summary>
        /// Destructor calls <see cref="Dispose()"/> in order to fire MeshChanged event.
        /// </summary>
        ~ComputeEffect()
        {
            Dispose();
        }
    }
}