using Fusee.Base.Common;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fusee.Base.Imp.Blazor
{
    /// <summary>
    /// Implementation (platform dependent) for IO related functionality not supported by portable libraries.
    /// </summary>
    public class IO : IIOImp
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
            return new FileStream(path, (System.IO.FileMode)mode);
        }
    }

    /// <summary>
    /// EmbeddedResourceHelper
    /// </summary>
    public static class EmbeddedResourceHelper
    {
        /// <summary>
        /// Tries to load an embedded resource from given assembly as a stream
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="assembly"></param>
        /// <returns>The stream specified by path.</returns>
        public static Stream Load(string resourceName, Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetExecutingAssembly();
            }

            string fullResourceName = assembly
                .GetManifestResourceNames()
                .First(resource => resource.EndsWith(resourceName));

            return assembly.GetManifestResourceStream(fullResourceName);
        }
    }
}