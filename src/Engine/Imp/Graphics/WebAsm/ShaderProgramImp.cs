using Fusee.Engine.Common;
using Fusee.Engine.Imp.Graphics.WebAsm;

namespace Fusee.Engine.Imp.WebAsm
{
    /// <summary>
    /// Implementation of <see cref="IShaderProgramImp" /> within WebAsm for WebGL.
    /// </summary>
    public class ShaderProgramImp : IShaderProgramImp
    {
        internal WebGLProgram Program;
    }
}
