using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Blazor
{
    /// <summary>
    /// Implementation of the <see cref="IShaderParam" /> interface.
    /// This object is passed to shader programs that are running on the graphics card to modify shader values.
    /// </summary>
    public class ShaderParam : IShaderParam
    {
        internal WebGLUniformLocation handle;
    }

    /// <summary>
    /// Implementation of the <see cref="IShaderHandle"/>
    /// This object is passed to WebGL and represents a shader program
    /// </summary>
    public class ShaderHandleImp : IShaderHandle
    {
        internal WebGLProgram Handle;
    }
}