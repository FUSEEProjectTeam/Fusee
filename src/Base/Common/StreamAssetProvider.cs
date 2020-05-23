using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Fusee.Base.Common
{
    /// <summary>
    /// Asset provider base class for implementing asset providers based on streams. 
    /// Used to implement FileAssetProvider and Android ApkAssetProviders.
    /// </summary>
    public abstract class StreamAssetProvider : IAssetProvider
    {
        private readonly Dictionary<Type, AssetHandler> _assetHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamAssetProvider" /> class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected StreamAssetProvider()
        {
            _assetHandlers = new Dictionary<Type, AssetHandler>();
        }


        /// <summary>
        /// Determines whether this instance can handle assets of the specified type (in general).
        /// </summary>
        /// <param name="type">The asset type  in question (such as typeof(ImageDate, Font, Sound, ...)).</param>
        /// <returns>
        /// true if this instance can handle the specified type. false otherwise.
        /// </returns>
        public bool CanHandleType(Type type)
        {
            if (_assetHandlers.ContainsKey(type))
                return true;

            return false;
        }


        /// <summary>
        /// Implement this on a given platform to create a stream for the asset identified by id.
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>Implementors should return null if the asset cannot be retrieved.</returns>
        protected abstract Stream GetStream(string id);

        /// <summary>
        /// Implement this on a given platform to create an async stream for the asset identified by id.
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>Implementors should return null if the asset cannot be retrieved.</returns>
        protected abstract Task<Stream> GetStreamAsync(string id);

        /// <summary>
        /// Retrieves the asset identified by the given string.
        /// </summary>
        /// <param name="id">The identifier string.</param>
        /// <param name="type">The type of the asset.</param>
        /// <returns>
        /// The asset, if this provider can acquire an asset with the given id and the given type. Otherwise null.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public object GetAsset(string id, Type type)
        {
            using (Stream stream = GetStream(id))
            {
                if (stream == null)
                {
                    return null;
                }

                AssetHandler handler;
                if (_assetHandlers.TryGetValue(type, out handler))
                {
                    object ret;
                    if (null != (ret = handler.Decoder(id, stream)))
                    {
                        // Return in using will dispose the used object, which is what we want...
                        return ret;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves the asset identified by the given string with an <see langword="async"/> method.
        /// </summary>
        /// <param name="id">The identifier string.</param>
        /// <param name="type">The type of the asset.</param>
        /// <returns>
        /// The asset, if this provider can acquire an asset with the given id and the given type. Otherwise null.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async Task<object> GetAssetAsync(string id, Type type)
        {
            var stream = await GetStreamAsync(id).ConfigureAwait(false);

            if (stream == null)
            {
                return null;
            }

            if (_assetHandlers.TryGetValue(type, out var handler))
            {
                // Return in using will dispose the used object, which is what we want...
                return await handler.DecoderAsync(id, stream).ConfigureAwait(false);
            }

            return null;
        }


        /// <summary>
        /// Determines whether this asset provider can get the specified asset without actually getting it.
        /// </summary>
        /// <param name="id">The identifier string.</param>
        /// <param name="type">The expected type of the asset.</param>
        /// <returns>
        /// true if this asset will produce a result. Otherwise false.
        /// </returns>
        public bool CanGet(string id, Type type)
        {
            if (!CheckExists(id))
            {
                type = null;
                return false;
            }

            AssetHandler handler;
            if (_assetHandlers.TryGetValue(type, out handler))
            {
                if (handler.Checker(id))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether this asset provider can get the specified asset without actually getting it.
        /// </summary>
        /// <param name="id">The identifier string.</param>
        /// <param name="type">The expected type of the asset.</param>
        /// <returns>
        /// true if this asset will produce a result. Otherwise false.
        /// </returns>
        public async Task<bool> CanGetAsync(string id, Type type)
        {
            if (!await CheckExistsAsync(id))
            {
                type = null;
                return false;
            }

            AssetHandler handler;
            if (_assetHandlers.TryGetValue(type, out handler))
            {
                if (handler.Checker(id))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Checks the existence of the identified asset. Implement this on a given platform.
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>Implementors should return true if a stream can be created.</returns>
        protected abstract bool CheckExists(string id);

        /// <summary>
        /// Checks the existence of the identified asset as an async method. Implement this on a given platform.
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>Implementors should return true if a stream can be created.</returns>
        protected abstract Task<bool> CheckExistsAsync(string id);

        /// <summary>
        /// Asynchronous get method.
        /// </summary>
        /// <param name="id">The identifier string.</param>
        /// <param name="getCallback">Code to call when the loading is done.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>
        /// The design doesn't follow any of the standard .NET asynchronous patterns like APM, EAP, or TAP.
        /// It's close to JavaScript where you just provide an "onLoad" handler that's called when the object
        /// is retrieved and decoded. This is to enable AssetProviders to be implemented using standard
        /// JavaScript DOM objects like Image. See the article
        /// <a href="https://msdn.microsoft.com/en-us/library/hh873178(v=vs.110).aspx">Interop with Other Asynchronous Patterns and Types</a>
        /// to get an idea how to map this pattern, which is similar to APM (albeit simpler), to the currently en-vogue TAP (async/await) pattern.
        /// </remarks>
        public void BeginGetAsset(string id, GetCallback getCallback)
        {
            // TODO: Implement Asynchronous loading
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers the given asset type handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <remarks>
        /// This method is rather for internal purposes. While an asset provider typically implements access to
        /// a given kind of asset storage, sometimes its hard to implement asset type handlers (decoders) capable of handling
        /// a certain type without knowing much about the contents (like images, etc).
        /// </remarks>
        public void RegisterTypeHandler(AssetHandler handler)
        {
            _assetHandlers.Add(handler.ReturnedType, handler);
        }

    }
}