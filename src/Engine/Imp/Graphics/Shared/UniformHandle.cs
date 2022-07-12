using Fusee.Engine.Common;

#if PLATFORM_DESKTOP
namespace Fusee.Engine.Imp.Graphics.Desktop
#elif PLATFORM_ANDROID
namespace Fusee.Engine.Imp.Graphics.Android
#endif
{
    /// <summary>
    /// Implementation of the <see cref="IUniformHandle" /> interface.
    /// This object is passed to shader programs that are running on the graphics card to modify shader values.
    /// </summary>
    public class UniformHandle : IUniformHandle
    {
        internal int handle;
    }
}