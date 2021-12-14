using Fusee.Engine.Common;
using OpenTK.Graphics;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    /// <summary>
    /// RenderBuffer for OpenTK, an integer value is used as a handle
    /// </summary>
    public class RenderBufferHandle : IBufferHandle
    {
        internal RenderbufferHandle Handle = new(-1);
    }

    /// <summary>
    /// FrameBuffer for OpenTK, an integer value is used as a handle
    /// </summary>
    class FrameBufferHandle : IBufferHandle
    {
        internal FramebufferHandle Handle = new(-1);
    }

    class StorageBufferHandle : IBufferHandle
    {
        internal BufferHandle Handle = new(-1);
    }
}