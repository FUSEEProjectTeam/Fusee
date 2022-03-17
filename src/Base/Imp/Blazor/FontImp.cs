using Fusee.Base.Core;
using SixLabors.Fonts;

namespace Fusee.Base.Imp.Blazor
{
    /// <summary>
    /// Font implementation using SixLabors.Fonts
    /// </summary>
    public class FontImp : FontImpBase
    {
        /// <summary>
        /// Font implementation for WebAsm
        /// </summary>
        /// <param name="stream"></param>
        public FontImp(System.IO.Stream stream)
        {
            _collection = new FontCollection();
            _collection.Add(stream);
        }
    }
}