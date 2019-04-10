using Fusee.Base.Core;
using JSIL.Meta;
using System;

namespace Fusee.Base.Imp.Web
{
    /// <summary>
    /// Provides methods for platform specific conversions of file content.
    /// </summary>
    public static class FileDecoder
    {
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
