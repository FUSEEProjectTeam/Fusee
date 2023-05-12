using System;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Token for invalidating the cached gpu data .
    /// </summary>
    public class InvalidateGpuDataCache
    {
        /// <summary>
        /// Called when the value of <see cref="IsDirty"/> changes.
        /// </summary>
        public Action<bool>? IsDirtyPropertyChanged;

        /// <summary>
        /// Set this to true if the Data Handler should invalidate the gpu data cache.
        /// Is set to false internally.
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                _isDirty = value;
                IsDirtyPropertyChanged?.Invoke(_isDirty);
            }

        }
        private bool _isDirty;
    }
}