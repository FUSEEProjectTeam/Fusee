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

        IBufferHandle BufferHandle { get; set; }

        IntPtr DataMem { get; }

        int BindingIndex { get; set; }
    }
}
