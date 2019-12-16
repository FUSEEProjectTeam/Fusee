using Fusee.Engine.Common;

#if PLATFORM_DESKTOP
namespace Fusee.Engine.Imp.Graphics.Desktop
#elif PLATFORM_ANDROID
namespace Fusee.Engine.Imp.Graphics.Android
#endif
{
    /// <summary>
    /// Implementation of <see cref="IShaderProgramImp" /> for usage with OpenTK framework.
    /// </summary>
    public class ShaderProgramImp : IShaderProgramImp
    {
        internal int Program;
    }
}