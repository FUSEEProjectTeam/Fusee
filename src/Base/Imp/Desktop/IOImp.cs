using System.IO;
using Fusee.Base.Common;

namespace Fusee.Base.Imp.Desktop
{
    /// <summary>
    /// Implementation (platform dependent) for IO related functionality not supported by portable libraries.
    /// </summary>
    public class IOImp : IIOImp
    {
        /// <summary>
        /// Opens a file and returns its contents as a <see cref="Stream" />.
        /// </summary>
        /// <param name="path">The path to the file to open.</param>
        /// <param name="mode">The file mode (read, write, append).</param>
        /// <returns>
        /// The stream specified by path.
        /// </returns>
        public Stream StreamFromFile(string path, Common.FileMode mode)
        {
            return new FileStream(path, (System.IO.FileMode) mode);
        }
    }
}
