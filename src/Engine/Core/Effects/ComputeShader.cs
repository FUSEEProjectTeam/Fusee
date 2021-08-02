using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core.Effects
{
    public class ComputeShader : Effect, IDisposable
    {
        //TODO: has RenderStates but doesn't need them...


        /// <summary>
        /// The Compute Shader code.
        /// </summary>
        public string ComputeShaderSrc { get; protected set; }

        public ComputeShader(string shaderCode, IEnumerable<IFxParamDeclaration> effectParameters)
        {
            ParamDecl = new Dictionary<string, IFxParamDeclaration>();
            ComputeShaderSrc = shaderCode;

            if (effectParameters != null)
            {
                foreach (var param in effectParameters)
                    ParamDecl.Add(param.Name, param);
            }

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
