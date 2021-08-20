using Fusee.Base.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Fusee.Base.Core
{

    internal static class Ext
    {
        internal static Dictionary<DateTime, string> _timeAdded = new Dictionary<DateTime, string>();

        /// <summary>
        /// Checks if assets dictionary holds more than 20 element, if so, delete the latest 10 entries
        /// </summary>
        /// <param name="assetBuffer"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static void AddTask(this Dictionary<string, Task> assetBuffer, string key, Task value)
        {
            // check if present
            if (assetBuffer.ContainsKey(key)) return;

            // any memory checks of available memory vs dictionary size, etc. fails here and isn't available
            // for web. Therefore just this 20 element cap.
            if (assetBuffer.Keys.Count > 20)
            {
                var timesSorted = _timeAdded.Keys.ToList();
                timesSorted.Sort(); // this sort from oldest [0] to newest [length]

                for (var i = 0; i < 10; i++)
                {
                    var oldestTime = timesSorted[i];
                    var keyOfOldestTime = _timeAdded[oldestTime];

                    // remove oldest element as well as oldest time from helper dict
                    assetBuffer.Remove(keyOfOldestTime);
                    _timeAdded.Remove(oldestTime);
                }
            }

            _timeAdded.Add(DateTime.Now, key);
            assetBuffer.Add(key, value);
        }

    }

    /// <summary>
    /// A class providing access to Assets. An asset is considered any content to be loaded, deserialized and converted during
    /// an application's lifetime. Often Assets should be loaded up-front and accessed during run-time with no perceivable delay.
    /// AssetStorage is a staticton (a singleton with an additional static interface).
    /// </summary>
    /// <remarks>
    /// The existence of this class is a tribute to the Web-world where a lot of asset types (e.g. images) are JavaScript built-in
    /// functionality with no possibility to separate the many aspects of asset-access (like loading, deserialization, codec,
    /// asynchronicity). Decent programming environments allow to separate these aspects using streams. A decoder is implemented
    /// against a stream. Anything capable of providing streams, synchronously or asynchronously thus can act as an asset store.
    /// If FUSEE had been designed without JavaScript X-compilation in mind, this class would probably not
    /// exist.
    /// </remarks>
    public sealed class AssetStorage
    {
        private readonly List<IAssetProvider> _providers;
        private readonly Dictionary<string, Task> _assetBuffer;

        private static AssetStorage _instance;

        /// <summary>
        /// Returns true if all assets have finished loading, independent of failed or success state
        /// </summary>
        public static bool AllAssetsFinishedLoading =>
            _instance._assetBuffer.Values.All(x => x.IsCompleted);

        private AssetStorage()
        {
            _providers = new List<IAssetProvider>();
            _assetBuffer = new Dictionary<string, Task>();
        }


        /// <summary>
        /// Implements the Singleton pattern.
        /// </summary>
        /// <value>
        /// The (one-and-only) instance of AssetStorage.
        /// </value>
        public static AssetStorage Instance => _instance ??= new AssetStorage();

        /// <summary>
        /// Returns true if an asset is currently loading or already loaded, independent of failed or success state!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsAssetPresent(string id)
        {
            return Instance._assetBuffer.ContainsKey(id);
        }

        /// <summary>
        /// Returns true if an asset has finished loading successfully,
        /// returns false if not present, see <see cref="IsAssetPresent(string)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsAssetLoaded(string id)
        {
            if (Instance._assetBuffer.TryGetValue(id, out var val))
            {
                return val.IsCompletedSuccessfully;
            }
            return false;
        }

        /// <summary>
        /// Staticton implementation of <see cref="GetAsset{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static T Get<T>(string id)
        {
            return Instance.GetAsset<T>(id);
        }

        /// <summary>
        /// Staticton implementation of <see cref="GetAsset{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static Task<T> GetAsync<T>(string id)
        {
            if (Instance._assetBuffer.TryGetValue(id, out var value))
            {
                return (Task<T>)value;
            }
            else
            {
                var task = Instance.GetAssetAsync<T>(id);
                Instance._assetBuffer.AddTask(id, task);
                return task;
            }
        }


        /// <summary>
        /// Retrieves the assets identified by ids in an completely async parallel matter.
        /// </summary>
        /// <typeparam name="T">The expected types of the asset to retrieve.</typeparam>
        /// <param name="ids">The identifiers.</param>
        /// <returns>The assets, if found. Otherwise null.</returns>
        public static Task<T[]> GetAssetsAsync<T>(IEnumerable<string> ids)
        {
            var allTasks = new List<Task<T>>();
            foreach (var id in ids)
            {
                var task = Instance.GetAssetAsync<T>(id);
                allTasks.Add(task);
                Instance._assetBuffer.AddTask(id, task);
            }

            return Task.WhenAll(allTasks);
        }

        /// <summary>
        /// Retrieves the asset identified by id.
        /// </summary>
        /// <typeparam name="T">The expected type of the asset to retrieve.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>The asset, if found. Otherwise null.</returns>
        /// <remarks>Internally, this method queries all of the registered asset providers (<see cref="RegisterAssetProvider"/>.
        /// The first asset provider capable of retrieving the asset "wins". It's up to any application to guarantee
        /// uniqueness of asset identifiers among all assets and asset providers.
        /// </remarks>
        public T GetAsset<T>(string id)
        {
            foreach (var assetProvider in _providers)
            {
                if (assetProvider.CanGet(id, typeof(T)))
                {
                    return (T)assetProvider.GetAsset(id, typeof(T));
                }
            }
            return default;
        }


        /// <summary>
        /// Retrieves a <see cref="Task"/> which loads the asset identified by id, eventually.
        /// </summary>
        /// <typeparam name="T">The expected type of the asset to retrieve.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>The asset, if found. Otherwise null.</returns>
        /// <remarks>Internally, this method queries all of the registered asset providers (<see cref="RegisterAssetProvider"/>.
        /// The first asset provider capable of retrieving the asset "wins". It's up to any application to guarantee
        /// uniqueness of asset identifiers among all assets and asset providers.
        /// </remarks>
        public async Task<T> GetAssetAsync<T>(string id)
        {
            foreach (var assetProvider in _providers)
            {
                if (await assetProvider.CanGetAsync(id, typeof(T)).ConfigureAwait(false))
                {
                    return (T)await assetProvider.GetAssetAsync(id, typeof(T)).ConfigureAwait(false);
                }
            }

            return default;
        }

        /// <summary>
        /// Staticton implementation of <see cref="RegisterAssetProvider"/>.
        /// </summary>
        /// <param name="assetProvider">The asset provider.</param>
        public static void RegisterProvider(IAssetProvider assetProvider)
        {
            Instance.RegisterAssetProvider(assetProvider);
        }

        /// <summary>
        /// Registers the given asset provider. Use this method to register asset providers
        /// for the platform (desktop, mobile, web) your main application should run on.
        /// </summary>
        /// <param name="assetProvider">The asset provider to register.</param>
        public void RegisterAssetProvider(IAssetProvider assetProvider)
        {
            if (_providers.Contains(assetProvider))
                throw new InvalidOperationException("Asset Provider already registered " + assetProvider);
            _providers.Add(assetProvider);
        }

        /// <summary>
        /// Unregisters all asset providers.
        /// </summary>
        public static void UnRegisterAllAssetProviders()
        {
            Instance._providers.Clear();
        }

        /// <summary>
        /// Creates a deep copy of the source object. Only works for source objects with the
        /// <see cref="ProtoBuf.ProtoContractAttribute"/> defined on their class.
        /// </summary>
        /// <typeparam name="T">
        ///   Type of the source and object and the returned clone. Implicitly defined by the source parameter.
        /// </typeparam>
        /// <param name="source">The source object to clone.</param>
        /// <returns>A deep copy of the source object. All objects referenced directly and indirectly from the source
        ///  object are copied, too.</returns>
        public static T DeepCopy<T>(T source) where T : class
        {
            if (source.GetType().GetCustomAttributes(true).OfType<ProtoBuf.ProtoContractAttribute>() == null)
            {
                throw new InvalidOperationException($"DeepCopy: ProtoBuf.ProtoContractAttribute is not defined on '{source.GetType().Name}'!");
            }

            _ = new MemoryStream();

            //ProtoBuf.Serializer.Serialize(stream, source);
            //stream.Position = 0;
            //return ProtoBuf.Serializer.Deserialize<T>(stream) as T;
            return source;
        }
    }
}