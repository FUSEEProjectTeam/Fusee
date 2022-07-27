using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Blazor
{
    /// <summary>
    /// RenderBuffer implementation for WebAsm.
    /// </summary>
    public class RenderBufferHandle : IBufferHandle
    {
        internal WebGLRenderbuffer Handle = null;
    }

    /// <summary>
    /// RenderBuffer implementation for WebAsm.
    /// </summary>
    public class FrameBufferHandle : IBufferHandle
    {
        internal WebGLFramebuffer Handle = null;
    }

    /// <summary>
    /// VertexArrayObject implementation for WebAsm.
    /// </summary>
    public class VertexArrayObject : IBufferHandle
    {
        internal WebGLVertexArrayObject Handle = null;
    }

    /// <summary>
    /// VertexArrayObject implementation for WebAsm.
    /// </summary>
    public class VertexBufferObject : IBufferHandle
    {
        internal WebGLBuffer Handle = null;
    }
}