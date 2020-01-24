using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Fusee.Base.Imp.Desktop
{
    /// <summary>
    /// A class to handle with extracting and loading embedded resources.
    /// </summary>
    public static class EmbeddedResourcesDllHandler
    {
        private static List<string> loadedDlls = new List<string>();

        /// <summary>
        /// Extract a dll from resources to temporary folder and loads it
        /// </summary>
        /// <param name="dllName">name of DLL file to create (including dll suffix)</param>
        /// <param name="resourceName">The resource name (fully qualified)</param>
        public static void LoadEmbeddedDll(string dllName, string resourceName)
        {
            if (!loadedDlls.Contains(dllName))
            {
                loadedDlls.Add(dllName);

                Assembly assem = Assembly.GetExecutingAssembly();
                string[] names = assem.GetManifestResourceNames();
                AssemblyName an = assem.GetName();

                var resourceStream = assem.GetManifestResourceStream(resourceName);

                // The temporary folder holds one or more of the temporary DLLs
                // It is made "unique" to avoid different versions of the DLL or architectures.
                var tempFolder = String.Format("{0}.{1}.{2}", an.Name, an.ProcessorArchitecture, an.Version);

                string dirName = Path.Combine(Path.GetTempPath(), tempFolder);
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }

                // See if the file exists, avoid rewriting it if not necessary
                string dllPath = Path.Combine(dirName, dllName);

                using (var fileStream = File.Create(dllPath))
                {
                    resourceStream.Seek(0, SeekOrigin.Begin);
                    resourceStream.CopyTo(fileStream);
                }

                IntPtr h = LoadLibrary(dllPath);
                if (h == IntPtr.Zero)
                {
                    throw new DllNotFoundException("Unable to load library: " + dllName + " from " + tempFolder);
                }
            }
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);
    }
}
