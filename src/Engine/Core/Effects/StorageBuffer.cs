using Fusee.Engine.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Fusee.Engine.Core.Effects
{
    public class StorageBuffer<T> : IStorageBuffer where T : struct
    {
        private readonly int _count;
        private readonly int _tSize;

        private T[] _data;

        private bool _disposed;
        private IBufferHandle _bufferHandle;

        private IntPtr _dataMem;

        public IntPtr DataMem { get { return _dataMem; } }

        private RenderCanvas _rc;

        public int BindingIndex { get; set; }

        /// <summary>
        /// Return the number of buffer elements.
        /// </summary>
        public int Count { get { return _count; } }

        /// <summary>
        /// Return the size in byte for one buffer element.
        /// </summary>
        public int Size { get { return _tSize; } }

        public IBufferHandle BufferHandle
        {
            get { return _bufferHandle; }
            set { _bufferHandle = value; }
        }

        public T this[int index]
        {
            get { return _data[index]; }
        }

        /// <summary>
        /// Creates a new instance of type StorageBuffer.
        /// </summary>
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

        /// <summary>
        /// Returns the buffer contents.
        /// </summary>
        /// <returns></returns>
        public T[] GetData()
        {
            _data = _rc.ContextImplementor.StorageBufferGetData<T>(_bufferHandle);
            return _data;
        }

        private void Release()
        {
            _rc.ContextImplementor.DeleteStorageBuffer(_bufferHandle);
            Marshal.FreeCoTaskMem(_dataMem);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                //Release unmanaged resources
                Release();

                // Note disposing has been done.
                _disposed = true;
            }
        }
    }
}
