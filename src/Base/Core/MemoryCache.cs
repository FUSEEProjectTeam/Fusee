using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Fusee.Base.Core
{
    /// <summary>
    /// Extensions for Microsoft.Extensions.Caching.Memory MemoryCache
    /// Source: https://stackoverflow.com/questions/45597057/how-to-retrieve-a-list-of-memory-cache-keys-in-asp-net-core
    /// </summary>
    public static class MemoryCacheExtensions
    {
        #region Microsoft.Extensions.Caching.Memory_6_OR_OLDER

        private static readonly Lazy<Func<MemoryCache, object>> GetEntries6 =
            new(() => (Func<MemoryCache, object>)Delegate.CreateDelegate(
                typeof(Func<MemoryCache, object>),
                typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true),
                throwOnBindFailure: true));

        #endregion

        #region Microsoft.Extensions.Caching.Memory_7_OR_NEWER

        private static readonly Lazy<Func<MemoryCache, object>> GetCoherentState =
            new(() =>
                CreateGetter<MemoryCache, object>(typeof(MemoryCache)
                    .GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance)));

        private static readonly Lazy<Func<object, IDictionary>> GetEntries7 =
            new(() =>
                CreateGetter<object, IDictionary>(typeof(MemoryCache)
                    .GetNestedType("CoherentState", BindingFlags.NonPublic)
                    .GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance)));

        private static Func<TParam, TReturn> CreateGetter<TParam, TReturn>(FieldInfo field)
        {
            var methodName = $"{field.ReflectedType.FullName}.get_{field.Name}";
            var method = new DynamicMethod(methodName, typeof(TReturn), new[] { typeof(TParam) }, typeof(TParam), true);
            var ilGen = method.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, field);
            ilGen.Emit(OpCodes.Ret);
            return (Func<TParam, TReturn>)method.CreateDelegate(typeof(Func<TParam, TReturn>));
        }

        #endregion

        private static readonly Func<MemoryCache, IDictionary> GetEntries =
            Assembly.GetAssembly(typeof(MemoryCache)).GetName().Version.Major < 7
                ? (cache => (IDictionary)GetEntries6.Value(cache))
                : cache => GetEntries7.Value(GetCoherentState.Value(cache));

        /// <summary>
        /// Returns all currently cached keys as <see cref="ICollection"/>.
        /// </summary>
        /// <param name="memoryCache">The source cache.</param>
        public static ICollection GetKeys(this IMemoryCache memoryCache) =>
            GetEntries((MemoryCache)memoryCache).Keys;

        /// <summary>
        /// Returns all currently cached keys as <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="memoryCache">The source cache.</param>
        public static IEnumerable<T> GetKeys<T>(this IMemoryCache memoryCache) =>
            memoryCache.GetKeys().OfType<T>();
    }

    /// <summary>
    /// Generic implementation of <see cref="MemoryCache"/>.
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TItem">The type of the cached item.</typeparam>
    /// </summary>
    public class MemoryCache<TKey, TItem> : IDisposable
    {
        /// <summary>
        /// Snapshot of the caches keys as <see cref="ICollection"/>.
        /// </summary>
        public IEnumerable<TKey> GetKeys => _cache.GetKeys<TKey>();

        /// <summary>
        /// Gets the number of items in the cache for diagnostic purposes.
        /// </summary>
        public int Count => _cache.Count;

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

        private bool _disposed = false;
        private readonly SemaphoreSlim _cacheLock = new(1);

        /// <summary>
        /// Creates a new instance and initializes the internal <see cref="MemoryCache"/>.
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
        /// Adds the given item to the cache or updates it if the item is already present.
        /// </summary>
        /// <param name="key">The key of the cache item.</param>
        /// <param name="cacheEntry">The cache item.</param>
        public async void AddOrUpdate(TKey key, TItem cacheEntry)
        {
            try
            {
                await _cacheLock.WaitAsync();
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
            finally { _cacheLock.Release(); }
        }

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(TKey key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                _cache.Compact(100);
                _cache.Dispose();

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# finalizer syntax for finalization code.
        /// This finalizer will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide finalizer in types derived from this class.
        /// </summary>
        ~MemoryCache()
        {
            Dispose(disposing: false);
        }
    }
}