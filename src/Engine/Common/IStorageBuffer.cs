using System;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Type independent abstraction for an Shader Storage Buffer Object.
    /// </summary>
    public interface IStorageBuffer : IDisposable
    {
        /// <summary>
        /// Return the number of buffer elements.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Return the size in byte for one buffer element.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// The handle of the buffer on the GPU.
        /// </summary>
        IBufferHandle BufferHandle { get; set; }

        /// <summary>
        /// The binding index point the SSBO will be bound to.
        /// Caution: the binding point should not be hard coded in the shader code!
        /// </summary>
        uint BindingIndex { get; set; }
    }
}