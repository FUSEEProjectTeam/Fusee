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
        /// All parameters (as references), saved per pass. See <see cref="FxParam"/> on the parameter infos that are saved.
        /// </summary>
        internal List<Dictionary<string, FxParam>> ParamsPerPass = new List<Dictionary<string, FxParam>>();

        /// <summary>
        /// The shader parameters of all passes. See <see cref="FxParam"/> on the parameter infos that are saved.
        /// </summary>
        internal Dictionary<string, FxParam> Parameters = new Dictionary<string, FxParam>();

    }
}