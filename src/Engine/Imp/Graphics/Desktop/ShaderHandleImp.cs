using Fusee.Engine.Common;
using OpenTK.Graphics;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    /// <summary>
    /// Implementation of <see cref="IShaderHandle" /> for usage with OpenTK framework.
    /// </summary>
    public class ShaderHandleImp : IShaderHandle
    {
        internal ProgramHandle Handle;
    }
}