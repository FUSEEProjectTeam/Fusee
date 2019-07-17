using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusee.Base.Common;
using Fusee.Serialization;

namespace Fusee.Base.Core
{
    /// <summary>
    /// A class providing access to Assets. An asset is considered any content to be loaded, de-serialized and converted during
    /// an application's lifetime. Often Assets should be loaded up-front and accessed during run-time with no perceivable delay.
    /// AssetStorage is a staticton (a singleton with an additional static interface).
    /// </summary>
    /// <remarks>
    /// The existance of this class is a tribute to the Web-world where a lot of asset types (e.g. images) are JavaScript built-in
    /// functionality with no possibility to separate the many aspects of asset-access (like loading, deserialization, codec,
    /// asynchronicity). Decent programming environments allow to separate these aspects using streams. A decoder is implemented
    /// against a stream. Anything capable of providing streams, synchronously or asynchronousliy thus can act as an asset store.
    /// If FUSEE had been designed without JavaScript X-compilation in mind, this class would probably not
    /// exist.
    /// </remarks>
    public class AssetStorage
    {
        private readonly List<IAssetProvider> _providers;
        private static AssetStorage _instance;

        private AssetStorage()
        {
            _providers = new List<IAssetProvider>();
        }

        /// <summary>
        /// Implements the Singleton pattern.
        /// </summary>
        /// <value>
        /// The (one-and-only) instance of AssetStorage.
        /// </value>
        public static AssetStorage Instance => _instance ?? (_instance = new AssetStorage());

        /// <summary>
        /// Staticton implementation of <see cref="GetAsset{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static T Get<T>(string id) => Instance.GetAsset<T>(id);

        /// <summary>
        /// Staticton implementation of <see cref="GetAsset{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string id) => await Instance.GetAssetAsync<T>(id);

        /// <summary>
        /// Retrieves the asset identified by id.
        /// </summary>
        /// <typeparam name="T">The expected type of the asset to retrieve.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>The asset, if found. Otherwise null.</returns>
        /// <remarks>Internally, this method queries all of the registerd asset providers (<see cref="RegisterAssetProvider"/>.
        /// The first asset provider capable of retrieving the asset "wins". It's up to any appliacation to guarantee
        /// uniquenesss of asset identifiers among all assets and asset providers.
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
        /// Retrieves the asset identified by id.
        /// </summary>
        /// <typeparam name="T">The expected type of the asset to retrieve.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>The asset, if found. Otherwise null.</returns>
        /// <remarks>Internally, this method queries all of the registerd asset providers (<see cref="RegisterAssetProvider"/>.
        /// The first asset provider capable of retrieving the asset "wins". It's up to any appliacation to guarantee
        /// uniquenesss of asset identifiers among all assets and asset providers.
        /// </remarks>
        public async Task<T> GetAssetAsync<T>(string id)
        {
            Console.WriteLine("Trying to get asset async");
            Console.WriteLine($"My prodivers {_providers.Count}");

            foreach (var assetProvider in _providers)
            {
                if (await assetProvider.CanGetAsync(id, typeof(T)))
                {
                    Console.WriteLine("Checking for asset providers");
                    return (T)await assetProvider.GetAssetAsync(id, typeof(T));
                }
            }
            Console.WriteLine("Nothing found!");

            return default;
        }

        /// <summary>
        /// Staticton implementation of <see cref="RegisterAssetProvider"/>.
        /// </summary>
        /// <param name="assetProvider">The asset provider.</param>
        public static void RegisterProvider(IAssetProvider assetProvider)
                    => Instance.RegisterAssetProvider(assetProvider);

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
        /// Creates a deep copy of the source object. Only works for source objects with the 
        /// <see cref="ProtoBuf.ProtoContractAttribute"/> defined on their class.
        /// </summary>
        /// <typeparam name="T">
        ///   Type of the source and object and the returnded clone. Implicitely defined by the source parameter.
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
            var ser = new Serializer();
            var stream = new MemoryStream();

            ser.Serialize(stream, source);
            stream.Position = 0;
            return ser.Deserialize(stream, null, typeof(T)) as T;
        }
    }
}
