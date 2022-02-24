using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fusee.Base.Core
{
    public class MemoryCache<TKey, TItem> : IDisposable
    {
        public int SlidingExpiration = 5;
        public int ExpirationScanFrequency = 6;

        public delegate Task<TItem> AddItemHandlerAsync(object sender, EventArgs e);
        public event AddItemHandlerAsync AddItemAsync;

        public delegate TItem AddItemHandler(object sender, EventArgs e);
        public event AddItemHandler AddItem;

        public PostEvictionDelegate HandleEvictedItem;

        private readonly MemoryCache _cache;
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _locks = new();

        public MemoryCache()
        {
            _cache = new(new MemoryCacheOptions()
            {
                ExpirationScanFrequency = TimeSpan.FromSeconds(ExpirationScanFrequency)
            });
        }

        public bool TryGetValue(TKey key, out TItem item)
        {
            if (_cache.TryGetValue(key, out item))
                return true;
            return false;
        }

        public void Add(TKey key, TItem cacheEntry)
        {            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.High)
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(SlidingExpiration));

            cacheEntryOptions.RegisterPostEvictionCallback((subkey, subValue, reason, state) =>
            {
                _locks.Remove(subkey, out _);
                HandleEvictedItem?.Invoke(subkey, subValue, reason, state);

            });

            // Key not in cache, so get data.
            _cache.Set(key, cacheEntry, cacheEntryOptions);
            
        }

        public void AddOrUpdate(TKey key, TItem cacheEntry)
        {           
            if (!_cache.TryGetValue(key, out cacheEntry))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetPriority(CacheItemPriority.High)
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(SlidingExpiration));

                cacheEntryOptions.RegisterPostEvictionCallback((subkey, subValue, reason, state) =>
                {
                    _locks.Remove(subkey, out _);
                    HandleEvictedItem?.Invoke(subkey, subValue, reason, state);

                });

                // Key not in cache, so get data.
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }
        }
        
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    _cache.Dispose();
                    
                }
                _disposed = true;
            }
        }

       
        ~MemoryCache()
        {
            Dispose(disposing: false);
        }

    }
}
