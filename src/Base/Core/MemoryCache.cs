using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fusee.Base.Core
{
    public class MemoryCache<TItem>
    {
        public int SlidingExpiration = 5;
        public int ExpirationScanFrequency = 6;

        public delegate Task<TItem> AddItemHandler(object sender, EventArgs e);
        public event AddItemHandler AddItem;

        public PostEvictionDelegate HandleEvictedItem;

        private readonly MemoryCache _cache;
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _meshLocks = new();

        public MemoryCache()
        {
            _cache = new(new MemoryCacheOptions()
            {
                ExpirationScanFrequency = TimeSpan.FromSeconds(ExpirationScanFrequency)
            });
        }

        public bool TryGetValue(Guid key, out TItem item)
        {
            if (_cache.TryGetValue(key, out item))
                return true;
            return false;
        }

        public async Task AddOrUpdate(Guid key, EventArgs args)
        {
            if (!_cache.TryGetValue(key, out TItem cacheEntry))// Look for cache key.
            {
                SemaphoreSlim chacheLock = _meshLocks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));

                await chacheLock.WaitAsync();
                try
                {
                    if (!_cache.TryGetValue(key, out cacheEntry))
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetPriority(CacheItemPriority.High)
                            // Keep in cache for this time, reset time if accessed.
                            .SetSlidingExpiration(TimeSpan.FromSeconds(SlidingExpiration));
                        cacheEntryOptions.RegisterPostEvictionCallback((subkey, subValue, reason, state) =>
                        {
                            _meshLocks.Remove(subkey, out _);                            
                            HandleEvictedItem?.Invoke(subkey, subValue, reason, state);
                            
                        });

                        // Key not in cache, so get data.
                        cacheEntry = await AddItem?.Invoke(this, args);//createItem(node, points);
                        _cache.Set(key, cacheEntry, cacheEntryOptions);
                    }
                }
                finally
                {
                    chacheLock.Release();
                }
            }
        }
    }
}
