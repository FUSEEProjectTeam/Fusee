using Fusee.Engine.Common;
using Fusee.Engine.Imp.Graphics.WebAsm;

namespace Fusee.Engine.Imp.WebAsm
{
    /// <summary>
    /// Implementation of <see cref="IShaderHandle" /> within WebAsm for WebGL.
    /// </summary>
    public class ShaderHandleImp : IShaderHandle
    {
        internal WebGLProgram Handle;
    }
}
