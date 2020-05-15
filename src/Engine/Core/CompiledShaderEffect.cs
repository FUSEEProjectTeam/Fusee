using Fusee.Engine.Core.Scene;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// All compiled information of one <see cref="ShaderEffect"/>.
    /// A <see cref="CompiledShaderEffect"/> can have more than one Pass where each pass contains another shader program.
    /// Those are saved as a <see cref="ShaderProgram"/>s.
    /// </summary>
    internal class CompiledShaderEffect
    {
        /// <summary>
        /// The compiled shader programs for every pass.
        /// </summary>
        internal ShaderProgram[] ShaderPrograms;

        /// <summary>
        /// All parameters (as references), saved per pass. See <see cref="EffectParam"/> on the parameter infos that are saved.
        /// </summary>
        internal List<Dictionary<string, EffectParam>> ParamsPerPass = new List<Dictionary<string, EffectParam>>();

        /// <summary>
        /// The shader parameters of all passes. See <see cref="EffectParam"/> on the parameter infos that are saved.
        /// </summary>
        internal Dictionary<string, EffectParam> Parameters = new Dictionary<string, EffectParam>();

    }
}