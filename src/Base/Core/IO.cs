using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fusee.Base.Common;

namespace Fusee.Base.Core
{
    /// <summary>
    /// Contains static methods not supported by portable libraries
    /// dealing with input/output.
    /// </summary>
    public static class IO
    {
        /// <summary>
        /// The platform dependent implementation of all IO functionality.
        /// </summary>
        /// <value>
        /// The io implementation.
        /// </value>
        [InjectMe]
        public static IIOImp IOImp { set; get; }

        /// <summary>
        /// Opens a file and returns its contents as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="path">The path to the file to open.</param>
        /// <returns>A stream</returns>
        /// <exception>
        /// Exceptions thrown by this method depend on the underlying 
        /// platform dependent implementation.
        /// </exception>
        public static Stream StreamFromFile(string path, FileMode mode)
        {
            return IOImp.StreamFromFile(path, mode);
        }
    }
}
