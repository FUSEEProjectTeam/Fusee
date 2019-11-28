using Fusee.Base.Core;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Fusee.Base.Imp.Desktop
{
    public static class EmbeddedResourcesDllHandler
    {
        /// <summary>
        /// Extract a dll from resources to temporary folder and loads it
        /// </summary>
        /// <param name="dllName">name of DLL file to create (including dll suffix)</param>
        /// <param name="resourceName">The resource name (fully qualified)</param>
        public static void LoadEmbeddedDlls(string dllName, string resourceName)
        {
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

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);
    }
}
