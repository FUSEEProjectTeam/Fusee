using Microsoft.Extensions.Caching.Memory;
using System;

namespace Fusee.Base.Core
{
    /// <summary>
    /// Generic implementation of <see cref="MemoryCache"/>.
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TItem">The type of the cached item.</typeparam>
    /// </summary>
    public class MemoryCache<TKey, TItem>
    {
        /// <summary>
        /// Sets how long a cache entry can be inactive (not accessed) before it will be removed.
        /// </summary>
        public int SlidingExpiration = 5;

        /// <summary>
        /// Time between successive scans for expired items.
        /// </summary>
        public int ExpirationScanFrequency = 6;

        /// <summary>
        /// Method that is called when an item was evicted from the cache.
        /// </summary>
        public PostEvictionDelegate HandleEvictedItem;

        private readonly MemoryCache _cache;

        /// <summary>
        /// Creates a new instance and initializes the internal <see cref="MemoryCache"/> with the gi
        /// </summary>
        public MemoryCache()
        {
            _cache = new(new MemoryCacheOptions()
            {
                ExpirationScanFrequency = TimeSpan.FromSeconds(ExpirationScanFrequency)
            });
        }

        /// <summary>
        /// Tries to get an item from the cache.
        /// </summary>
        /// <param name="key">The key of the cached item.</param>
        /// <param name="item">The received cache item.</param>
        /// <returns>True if the item was in the cache, false otherwise.</returns>
        public bool TryGetValue(TKey key, out TItem item)
        {
            if (_cache.TryGetValue(key, out item))
                return true;
            return false;
        }

        /// <summary>
        /// Adds the given item to the cache. Will not check if the item is already in the cache!
        /// </summary>
        /// <param name="key">The key of the cache item.</param>
        /// <param name="cacheEntry">The cache item.</param>
        public void Add(TKey key, TItem cacheEntry)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.High)
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(SlidingExpiration));

            cacheEntryOptions.RegisterPostEvictionCallback((subkey, subValue, reason, state) =>
            {
                HandleEvictedItem?.Invoke(subkey, subValue, reason, state);
            });

            // Key not in cache, so get data.
            _cache.Set(key, cacheEntry, cacheEntryOptions);
        }

        /// <summary>
        /// If the item isn't in the cache, add it, otherwise override the value in the cache.
        /// </summary>
        /// <param name="key">The key of the cache item.</param>
        /// <param name="cacheEntry">The cache item.</param>
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
                    HandleEvictedItem?.Invoke(subkey, subValue, reason, state);
                });

                // Key not in cache, so get data.
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }
        }
    }
}