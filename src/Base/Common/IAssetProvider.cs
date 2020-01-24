using System;
using System.Threading.Tasks;

namespace Fusee.Base.Common
{
    /// <summary>
    /// The state of the asset akquisition process.
    /// </summary>
    public enum GetCallbackState
    {
        /// <summary>
        /// Asset akquisition is in progress. o (<see cref="GetCallbackState"/>)contains an integer specifying the progress in percent (100 == done).
        /// </summary>
        Progress,
        /// <summary>
        /// An error occured while akquiring the asset. o (<see cref="GetCallbackState"/>) contains a string containting the message.
        /// </summary>
        Error,
        /// <summary>
        /// Akquisition ended successfully. o contains the object of the type passed to <see cref="GetCallback"/>.
        /// </summary>
        Done,
    }

    /// <summary>
    /// Used for asynchronous handling. <see cref="IAssetProvider.BeginGetAsset" />.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <param name="o">The asset that was just retrieved.</param>
    /// <param name="type">The type of the asset.</param>
    public delegate void GetCallback(GetCallbackState state, object o, Type type);

    /// <summary>
    /// Type used in <see cref="IAssetProvider.RegisterTypeHandler"/>.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="storage">An opaque object containing the data. The actual type of the storage depends on the AssetProvider.
    /// Many providers use a stream.</param>
    /// <returns>The asset cast to <see cref="object"/></returns>
    public delegate object AssetDecoder(string id, object storage);

    /// <summary>
    /// Type used in <see cref="IAssetProvider.RegisterTypeHandler"/>.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="storage">An opaque object containing the data. The actual type of the storage depends on the AssetProvider.</param>
    /// <returns>The asset cast to <see cref="Task"/></returns>
    public delegate Task<object> AssetDecoderAsync(string id, object storage);

    /// <summary>
    /// Type used in <see cref="IAssetProvider.RegisterTypeHandler"/>.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>True if the corresponding <see cref="AssetDecoder"/> will probably handle the requested asset.</returns>
    public delegate bool AssetChecker(string id);

    /// <summary>
    /// Structure containing an asset handler - typically used by implementation projects knowing how 
    /// to decode an asset of a given type using a serialization format (e.g. a stream or a memory location)
    /// defined by the platform.
    /// </summary>
    public struct AssetHandler
    {
        /// <summary>
        /// The type of asset returned by the decoder
        /// </summary>
        public Type ReturnedType;
        /// <summary>
        /// A method capable of decoding an asset into the returned type from 
        /// the opaque storage.
        /// </summary>
        public AssetDecoder Decoder;

        /// <summary>
        /// A method capable of decoding an asset into the returned type from 
        /// the opaque storage.
        /// </summary>
        public AssetDecoderAsync DecoderAsync;

        /// <summary>
        /// A method delivering a boolean value if an asset with the given id will be 
        /// handled by the decoder.
        /// </summary>
        public AssetChecker Checker;
    }

    /// <summary>
    /// An AssetProvider knows how to akquire assets of certain types from a certain kind of storage.
    /// </summary>
    /// <remarks>
    /// In a normal world, a good design would separate the many aspects of asset akquisition (storage,
    /// serialization, codec, asynchronicity). Unfortunately, in JavaScript-Land, it's all mixed (or should I say messed) up.
    /// You tell the JavaScript API "get me an image" and JavaScript magically loads the raw image 
    /// data, converts it to a two-dimensional pixel array and calls a user-provided callback when its all done. 
    /// No way to replace a single step by something self-provided. 
    /// So this is FUSEE's pitiful approach for an asset akquisition abstraction which is capable of 
    /// being implemented by poorly designed JavaScript APIs.
    /// </remarks>
    public interface IAssetProvider
    {
        /// <summary>
        /// Determines whether this instance can handle assets of the specified type (in general).
        /// </summary>
        /// <param name="type">The asset type  in question (such as typeof(ImageDate, Font, Sound, ...)).</param>
        /// <returns>true if this instance can handle the specified type. false otherwise.</returns>
        bool CanHandleType(Type type);

        /// <summary>
        /// Retrieves the asset identified by the given string.
        /// </summary>
        /// <param name="id">The identifier string.</param>
        /// <param name="type">The type of the asset.</param>
        /// <returns>The asset, if this provider can acquire an asset with the given id and the given type. Otherwise null.</returns>
        object GetAsset(string id, Type type);

        /// <summary>
        /// Retrieves the asset identified by the given string in async.
        /// </summary>
        /// <param name="id">The identifier string.</param>
        /// <param name="type">The type of the asset.</param>
        /// <returns>The asset, if this provider can acquire an asset with the given id and the given type. Otherwise null.</returns>
        Task<object> GetAssetAsync(string id, Type type);

        /// <summary>
        /// Determines whether this asset provider can get the specified asset without actually getting it.
        /// </summary>
        /// <param name="id">The identifier string.</param>
        /// <param name="type">The expected type of the asset.</param>
        /// <returns>
        /// true if this asset will produce a result. Otherwise false.
        /// </returns>
        bool CanGet(string id, Type type);

        /// <summary>
        /// Determines whether this asset provider can get the specified asset without actually getting it.
        /// </summary>
        /// <param name="id">The identifier string.</param>
        /// <param name="type">The expected type of the asset.</param>
        /// <returns>
        /// true if this asset will produce a result. Otherwise false.
        /// </returns>
        Task<bool> CanGetAsync(string id, Type type);

        // TODO: prepare for asynchronous handling
        /// <summary>
        /// Asynchronous get method. 
        /// </summary>
        /// <param name="id">The identifier string.</param>
        /// <param name="getCallback">Code to call when the loading is done.</param>
        /// <remarks>
        /// The design doesn't follow any of the standard .NET asynchronous patterns like APM, EAP, or TAP.
        /// It's close to JavaScript where you just provide an "onLoad" decoder that's called when the object
        /// is retrieved and decoded. This is to enable AssetProviders to be implemented using standard 
        /// JavaScript DOM objects like Image. See the article
        /// <a href="https://msdn.microsoft.com/en-us/library/hh873178(v=vs.110).aspx">Interop with Other Asynchronous Patterns and Types</a>
        /// to get an idea how to map this pattern, which is similar to APM (albeit simpler), to the currently en-vogue TAP (async/await) pattern.
        /// </remarks>
        void BeginGetAsset(string id, GetCallback getCallback);

        /// <summary>
        /// Registers the given asset type decoder.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <remarks>
        /// This method is rather for internal purposes. While an asset provider typically implements access to
        /// a given kind of asset storage, sometimes its hard to implement asset type handlers (decoders) capable of handling
        /// a certain type without knowing much about the contents (like images, etc).
        /// </remarks>
        void RegisterTypeHandler(AssetHandler handler);
    }
}