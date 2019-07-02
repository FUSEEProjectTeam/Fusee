using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Samples;
using FileMode = System.IO.FileMode;
using Path = Fusee.Base.Common.Path;

namespace Fusee.Base.Imp.WebAsm
{
    /// <summary>
    /// Asset provider for direct file access. Typically used in desktop builds where assets are simply contained within
    /// a subdirectory of the application.
    /// </summary>
    public class AssetProvider : StreamAssetProvider
    {
        private string _baseDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAssetProvider"/> class.
        /// </summary>
        /// <param name="baseDir">The base directory where assets should be looked for.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public AssetProvider(string baseDir = null) : base()
        {

            _baseDir = (string.IsNullOrEmpty(baseDir)) ? "Assets" : baseDir;

            if (_baseDir[_baseDir.Length - 1] != '/')
                _baseDir += '/';
  
            // Image handler
            RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof(ImageData),
                Decoder = delegate (string id, object storage)
                {
                    string ext = Path.GetExtension(id).ToLower();
                    switch (ext)
                    {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".bmp":
                            return FileDecoder.LoadImage((Stream)storage);
                    }
                    return null;
                },
                Checker = delegate (string id)
                {
                    string ext = Path.GetExtension(id).ToLower();
                    switch (ext)
                    {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".bmp":
                            return true;
                    }
                    return false;
                }
            });

            // Text file -> String handler. Keep this one the last entry as it doesn't check the extension
            RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof(string),
                Decoder = delegate (string id, object storage)
                {
                    var result = WasmResourceLoader.LoadAsync("Assets/" + id, WasmResourceLoader.GetLocalAddress());

                    return result.ToString();
                    //string ret;
                    //using (var sr = new StreamReader((Stream)storage, System.Text.Encoding.Default, true))
                    //{
                    //    ret = sr.ReadToEnd();
                    //}
                    //return ret;
                    // return IO.StreamFromFile("Assets/" + id, Fusee.Base.Common.FileMode.Open);
                },
                Checker = id => true // If it's there, we can handle it...
            });
        }

        /// <summary>
        /// Creates a stream for the asset identified by id using <see cref="FileStream"/>
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>
        /// A valid stream for reading if the asset ca be retrieved. null otherwise.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected override Stream GetStream(string id)
        {
            var task = WasmResourceLoader.LoadAsync(_baseDir + id, WasmResourceLoader.GetLocalAddress());
            task.Wait();
            return task.Result; 
        }
        
        
        /// <summary>
        /// Checks the existence of the identified asset using <see cref="File.Exists"/>
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>
        /// true if a stream can be created.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected override bool CheckExists(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            
            return true;
            /*
            // If it is an absolute path (e.g. C:\SomeDir\AnAssetFile.ext) directly check its presence
            if (Path.IsPathRooted(id))
                return File.Exists(id);

            // Path seems relative. First see if the file exists at the current working directory
            if (File.Exists(id))
                return true;

            foreach (var baseDir in _baseDirs)
            {
                string path = Path.Combine(baseDir, id);
                if (File.Exists(path))
                    return true;
            }
            return false;
            */
        }
    }
}
