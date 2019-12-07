using Fusee.Engine.Common;

#if PLATFORM_DESKTOP
namespace Fusee.Engine.Imp.Graphics.Desktop
#elif PLATFORM_ANDROID
namespace Fusee.Engine.Imp.Graphics.Android
#endif
{
    /// <summary>
    /// RenderBuffer for OpenTK, an integer value is used as a handle
    /// </summary>
    public class RenderBufferHandle : IBufferHandle
    {
        internal int Handle = -1;
    }

    /// <summary>
    /// FrameBuffer for OpenTK, an integer value is used as a handle
    /// </summary>
    class FrameBufferHandle : IBufferHandle
    {
        internal int Handle = -1;
    }
}