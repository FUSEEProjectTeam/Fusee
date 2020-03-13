using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderEffects;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Compiled information of one <see cref="ShaderEffect"/>.   
    /// </summary>
    internal class CompiledShaderEffect
    {
        /// The handle that identifies the shader program on the gpu.
        /// </summary>
        internal IShaderHandle GpuHandle;

        /// <summary>
        /// All parameters of this ShaderProgramm as a Dictionary of type "string, ShaderParamInfo".
        /// </summary>
        internal Dictionary<string, ShaderParamInfo> ShaderParamInfos;

        /// <summary>
        /// The shader parameters of all passes. See <see cref="FxParam"/> on the parameter infos that are saved.
        /// </summary>
        internal Dictionary<string, FxParam> ActiveUniforms = new Dictionary<string, FxParam>();

    }
}