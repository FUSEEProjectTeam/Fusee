using Fusee.Base.Core;
using SixLabors.Fonts;
using System.IO;
using System.Linq;

namespace Fusee.Base.Imp.Desktop
{
    /// <summary>
    /// Font implementation using SixLabors.Fonts
    /// </summary>
    public class FontImp : FontCore
    {
        /// <summary>
        /// Font ctor implementation for Desktop
        /// </summary>
        /// <param name="stream"></param>
        public FontImp(Stream stream)
        {
            _collection = new FontCollection();
            _collection.Install(stream);
            _font = _collection.Families.AsEnumerable().First().CreateFont(PixelHeight);
        }
    }
}