using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Compiled information of one <see cref="ShaderEffect"/>.
    /// </summary>
    internal class CompiledEffect
    {
        /// <summary>
        /// The handle that identifies the shader program on the gpu.
        /// </summary>
        internal IShaderHandle GpuHandle;

        /// <summary>
        /// The shader parameters of all passes. See <see cref="FxParam"/> on the parameter infos that are saved.
        /// </summary>
        internal Dictionary<string, FxParam> ActiveUniforms = new Dictionary<string, FxParam>();

    }
}