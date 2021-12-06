using Fusee.Engine.Core.Scene;
using Fusee.PointCloud.Core;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fusee.Engine.Core
{
    public class PointCloudOutOfCoreCache<TPoint>
    {
        public int SlidingExpiration = 5;
        public int ExpirationScanFrequency = 6;

        public ConcurrentBag<Mesh> DisposeQueue;

        private readonly MemoryCache _cache;
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _locks = new();

        public PointCloudOutOfCoreCache()
        {
            DisposeQueue = new ConcurrentBag<Mesh>();
            _cache = new(new MemoryCacheOptions() 
            { 
                ExpirationScanFrequency = TimeSpan.FromSeconds(ExpirationScanFrequency) 
            });
        }

        public bool TryGetValue(Guid key, out IEnumerable<Mesh> item)
        {
            if (_cache.TryGetValue(key, out item))
                return true;
            return false;
        }

        public async Task AddOrUpdate(Guid key, PtOctantRead<TPoint> node, Func<PtOctantRead<TPoint>, Task<IEnumerable<Mesh>>> createItem)
        {
            if (!_cache.TryGetValue(key, out IEnumerable<Mesh> cacheEntry))// Look for cache key.
            {
                SemaphoreSlim chacheLock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));

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
                            _locks.Remove(subkey, out _);
                            foreach (var mesh in (IEnumerable<Mesh>)subValue)
                            {
                                DisposeQueue.Add(mesh);
                                //mesh.Dispose();
                            }
                        });

                        // Key not in cache, so get data.
                        cacheEntry = await createItem(node);
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
