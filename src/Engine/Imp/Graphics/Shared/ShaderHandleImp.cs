using Fusee.Engine.Common;
using System.Runtime.CompilerServices;

#if PLATFORM_DESKTOP
[assembly: InternalsVisibleTo("Fusee.ImGuiDesktop")]
namespace Fusee.Engine.Imp.Graphics.Desktop
#elif PLATFORM_ANDROID
namespace Fusee.Engine.Imp.Graphics.Android
#endif
{
    /// <summary>
    /// Implementation of <see cref="IShaderHandle" /> for usage with OpenTK framework.
    /// </summary>
    public class ShaderHandleImp : IShaderHandle
    {
        internal int Handle;
    }
}