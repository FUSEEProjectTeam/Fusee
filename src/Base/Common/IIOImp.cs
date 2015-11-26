using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fusee.Base.Common
{
    /// <summary>
    /// Contract for all IO related functionality that is not supported by portable libraries.
    /// Implement this interface in a platform dependent implementation library for each
    /// platform that needs to be supported.
    /// </summary>
    public interface IIOImp
    {
        /// <summary>
        /// Opens a file and returns its contents as a <see cref="Stream" />.
        /// </summary>
        /// <param name="path">The path to the file to open.</param>
        /// <param name="mode">The file mode (read, write, append).</param>
        /// <returns>
        /// The stream specified by path.
        /// </returns>
        Stream StreamFromFile(string path, FileMode mode);
    }
}
