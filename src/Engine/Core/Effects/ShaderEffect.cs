using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// A ShaderEffect contains a string for each, the vertex, fragment and geometry shader and a set of render states.
    /// Use this if you want to write the shader code on your own. 
    /// The values of uniform variables you defined (<see cref="Effect.UniformParameters"/>) can be set using <see cref="Effect.SetFxParam{T}(int, T)"/> or <see cref="Effect.SetFxParam{T}(string, T)"/>. 
    /// </summary>
    public class ShaderEffect : Effect, IDisposable
    {
        /// <summary>
        /// The Vertex shader code.
        /// </summary>
        public string VertexShaderSrc { get; protected set; }

        /// <summary>
        /// The Fragment shader code.
        /// </summary>
        public string PixelShaderSrc { get; protected set; }

        /// <summary>
        /// The Geometry shader code.
        /// </summary>
        public string GeometryShaderSrc { get; protected set; }

        private bool disposed;

        /// <summary>
        /// The constructor to create a shader effect.
        /// </summary>
        /// <param name="effectParameters">The list of (uniform) parameters. The concrete type of the object also indicates the parameter's type.
        /// </param>
        /// <param name="rendererStates"></param>
        /// <param name="vs"></param>
        /// <param name="ps"></param>
        /// <param name="gs"></param>
        /// <remarks> Make sure to insert all uniform variable in "effectParameters" that are declared in the shader code.</remarks>
        public ShaderEffect(IEnumerable<IFxParamDeclaration> effectParameters, RenderStateSet rendererStates, string vs, string ps, string gs = null)
        {
            UniformParameters = new Dictionary<int, IFxParamDeclaration>();

            RendererStates = rendererStates;
            VertexShaderSrc = vs;
            PixelShaderSrc = ps;
            GeometryShaderSrc = gs;

            if (effectParameters != null)
            {
                foreach (var param in effectParameters)
                    UniformParameters.Add(param.Hash, param);
            }

            EffectManagerEventArgs = new EffectManagerEventArgs(UniformChangedEnum.Unchanged);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
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
        ~ShaderEffect()
        {
            Dispose();
        }
    }
}