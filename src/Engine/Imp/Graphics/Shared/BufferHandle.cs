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
    internal class FrameBufferHandle : IBufferHandle
    {
        internal int Handle = -1;
    }

    internal class StorageBufferHandle : IBufferHandle
    {
        internal int Handle = -1;
    }

    /// <summary>
    /// VertexArrayObject implementation for WebAsm.
    /// </summary>
    public class VertexArrayObject : IBufferHandle
    {
        internal int Handle = -1;
    }

    /// <summary>
    /// VertexArrayObject implementation for WebAsm.
    /// </summary>
    public class VertexBufferObject : IBufferHandle
    {
        internal int Handle = -1;
    }
}