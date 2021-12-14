using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Android
{
    /// <summary>
    /// RenderBuffer for OpenGL, an integer value is used as a handle
    /// </summary>
    internal class RenderBufferHandle : IBufferHandle
    {
        internal int Handle = -1;
    }

    /// <summary>
    /// FrameBuffer for OpenGL, an integer value is used as a handle
    /// </summary>
    internal class FrameBufferHandle : IBufferHandle
    {
        internal int Handle = -1;
    }

    /// <summary>
    /// StorageBuffer for OpenGL, an integer value is used as a handle
    /// </summary>
    class StorageBufferHandle : IBufferHandle
    {
        internal int Handle = -1;
    }
}