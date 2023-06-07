﻿using Android.Content;
using Fusee.Base.Common;
using System.IO;

namespace Fusee.Base.Imp.Android
{
    /// <summary>
    /// Implementation (platform dependent) for IO related functionality not supported by portable libraries.
    /// </summary>
    public class IOImp : IIOImp
    {
        private readonly Context _androidContext;

        /// <summary>
        /// Android implementation for IO related functionality.
        /// </summary>
        /// <param name="androidContext"></param>
        public IOImp(Context androidContext)
        {
            _androidContext = androidContext;
        }

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
            Stream file;
            string pathCpy = path.ToLower();
            if (pathCpy.StartsWith("assets/") || pathCpy.StartsWith("assets\\"))
            {
                path = path["assets/".Length..];
                file = _androidContext.Assets.Open(path);
            }
            else
            {
                file = _androidContext.OpenFileInput(path);
            }

            if (file == null)
                throw new FileNotFoundException(path);

            return file;
        }
    }
}