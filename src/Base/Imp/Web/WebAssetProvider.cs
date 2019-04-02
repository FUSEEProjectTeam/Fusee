using System;
using System.Collections.Generic;
using Fusee.Base.Common;
using Fusee.Base.Core;
using JSIL.Meta;


namespace Fusee.Base.Imp.Web
{
    /// <summary>
    /// Asset provider implemented by the existing Asset handling provided by JSIL (JSIL.Browser)
    /// </summary>
    public class WebAssetProvider : IAssetProvider
    {
        private readonly Dictionary<Type, AssetHandler> _assetHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamAssetProvider" /> class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        public WebAssetProvider()
        {
            _assetHandlers = new Dictionary<Type, AssetHandler>();

            RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof (ImageData),
                Decoder = delegate(string id, object storage)
                {
                    string ext = Fusee.Base.Common.Path.GetExtension(id).ToLower();
                    switch (ext)
                    {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".bmp":
                            return WrapImage(storage);
                    }
                    return null;
                },
                Checker = delegate(string id)
                {
                    string ext = Fusee.Base.Common.Path.GetExtension(id).ToLower();
                    switch (ext)
                    {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".bmp":
                            return true;
                    }
                    return false;
                }
            });

            // Text file -> String handler. Keep this one the last entry as it doesn't check the extension
            RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof(string),
                Decoder = delegate (string id, object storage)
                {
                    string  sr = WrapString(storage);
                    return sr;
                },
                Checker = id => true // If it's there, we can handle it...
            }
            );
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
        /// Retrieves the asset identified by the given string.
        /// </summary>
        /// <param name="id">The identifier string.</param>
        /// <param name="type">The type of the asset.</param>
        /// <returns>
        /// The asset, if this provider can akquire an asset with the given id and the given type. Ohterwise null.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public object GetAsset(string id, Type type)
        {
            object assetOb = GetRawAsset(id);
            if (assetOb == null)
            {
                return null;
            }

            AssetHandler handler;
            if (_assetHandlers.TryGetValue(type, out handler))
            {
                object ret;
                if (null != (ret = handler.Decoder(id, assetOb)))
                {
                    return ret;
                }
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
                return false;
            }

            AssetHandler handler;
            if (_assetHandlers.TryGetValue(type, out handler))
            {
                if (handler.Checker(id))
                {
                    type = handler.ReturnedType;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks the existance of the identified asset. Implement this on a given platform.
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>Implementors should return true if a stream can be created.</returns>
        [JSExternal]
        private bool CheckExists(string id)
        {
            throw new NotImplementedException("This method is implemented in JavaScript [JSExternal]");
        }

        /// <summary>
        /// Checks the existance of the identified asset. Implement this on a given platform.
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>Implementors should return true if a stream can be created.</returns>
        [JSExternal]
        private object GetRawAsset(string id)
        {
            throw new NotImplementedException("This method is implemented in JavaScript [JSExternal]");
        }

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

        /// <summary>
        /// Loads a new Bitmap-Object from the given stream.
        /// </summary>
        /// <param name="assetOb">JSIL asset object containing the image in a supported format (png, jpg).</param>
        /// <returns>An ImageData object with all necessary information.</returns>
        [JSExternal]
        public static ImageData WrapImage(object assetOb)
        {
            throw new NotImplementedException("This method is implemented in JavaScript [JSExternal]");
        }

        /// <summary>
        /// Wraps a string around the given asset object. The asset must contain text data.
        /// </summary>
        /// <param name="storage">JSIL asset object containing the image in a supported format (png, jpg).</param>
        /// <returns>A string with the asset's contents</returns>
        [JSExternal]
        public static string WrapString(object storage)
        {
            throw new NotImplementedException("This method is implemented in JavaScript [JSExternal]");
        }
    }
}

