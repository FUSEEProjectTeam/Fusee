using Fusee.Engine.Common;
using System;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// CPU side implementation for using Shader Storage Buffer Objects.
    /// </summary>
    /// <typeparam name="T">The data type of the payload.</typeparam>
    public class StorageBuffer<T> : IStorageBuffer where T : struct
    {
        /// <summary>
        /// The binding index point the SSBO will be bound to.
        /// Caution: the binding point should not be hard coded in the shader code!
        /// </summary>
        public int BindingIndex { get; set; }

        /// <summary>
        /// Return the number of buffer elements.
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Return the size in byte for one buffer element.
        /// </summary>
        public int Size => _tSize;

        /// <summary>
        /// The handle of the buffer on the GPU.
        /// </summary>
        public IBufferHandle BufferHandle
        {
            get => _bufferHandle;
            set => _bufferHandle = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index] => _data[index];

        private readonly int _count;
        private readonly int _tSize;
        private readonly RenderCanvas _rc;

        private T[] _data;
        private bool _disposed;
        private IBufferHandle _bufferHandle;

        /// <summary>
        /// Creates a new instance of type StorageBuffer.
        /// </summary>
        /// <param name="rc">The RenderContext this object is used with.</param>
        /// <param name="count">The (fixed) count of buffer elements.</param>
        /// <param name="tSize">The size (byte) of one buffer element.</param>
        /// <param name="blockBindingIndex">Int that needs to be unique throughout the shader.</param>
        public StorageBuffer(RenderCanvas rc, int count, int tSize, int blockBindingIndex)
        {
            _count = count;
            _tSize = tSize;
            _rc = rc;
            BindingIndex = blockBindingIndex;
        }

        /// <summary>
        /// Sets the buffer contents on the gpu.
        /// </summary>
        /// <param name="data">The data that needs to be sent the gpu.</param>
        public void SetData(T[] data)
        {
            if (data.Length == _count)
            {
                _data = data;
            }
            else
                throw new ArgumentOutOfRangeException($"Data array has the wrong length. The length has to be {_count}!");
            _rc.ContextImplementor.StorageBufferSetData(this, _data);
        }

        private void Release()
        {
            _rc.ContextImplementor.DeleteStorageBuffer(_bufferHandle);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }

                //Release unmanaged resources
                Release();

                _disposed = true;
            }
        }
    }
}