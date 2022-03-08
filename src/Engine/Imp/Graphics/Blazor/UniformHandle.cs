using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Blazor
{
    /// <summary>
    /// Implementation of the <see cref="IUniformHandle" /> interface.
    /// This object is passed to shader programs that are running on the graphics card to modify shader values.
    /// </summary>
    public class UniformHandle : IUniformHandle
    {
        internal WebGLUniformLocation handle;
    }
}