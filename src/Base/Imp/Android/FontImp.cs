using Fusee.Base.Core;
using System.IO;
using System.Linq;
using SixLabors.Fonts;


namespace Fusee.Base.Imp.Android
{
    /// <summary>
    /// Font implementation using SixLabors.Fonts
    /// </summary>
    public class FontImp : FontCore
    {
        /// <summary>
        /// Font ctor implementation for Android
        /// Copy file stream to new memory stream first, otherwise the read operation fails
        /// </summary>
        /// <param name="stream"></param>
        public FontImp(Stream stream)
        {
            _collection = new FontCollection();
            using MemoryStream memoryStream = new();
            stream.CopyTo(memoryStream);
            stream.Flush();
            memoryStream.Position = 0;
            _collection.Install(memoryStream);
            _font = _collection.Families.AsEnumerable().First().CreateFont(PixelHeight);
        }
    }
}