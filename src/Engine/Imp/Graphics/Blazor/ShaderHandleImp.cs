using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Blazor
{
    /// <summary>
    /// Implementation of the <see cref="IShaderHandle"/>
    /// This object is passed to WebGL and represents a shader program
    /// </summary>
    public class ShaderHandleImp : IShaderHandle
    {
        internal WebGLProgram Handle;
    }
}