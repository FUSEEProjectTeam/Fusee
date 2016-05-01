using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Threading;
using C4d;
using Fusee.Math;
using Fusee.Serialization;

namespace FuExport
{
    // Your Plugin Number for "FUSEE Scene Format Exporter" is: 1031843 
    [SceneSaverPlugin(1031843,
        Name = "FUSEE 3D (*.fus)",
        Suffix = "fus")
    ]
    public class FusFileExporter : SceneSaverData
    {
        // call the base class' constructor with false!!! This keeps the garbage collector from collecting us
        public FusFileExporter() : base(false)
        {

        }

        public override bool Init(GeListNode node)
        {
            return true;
        }


        public override FILEERROR Save(BaseSceneSaver node, Filename name, BaseDocument doc, SCENEFILTER filterflags)
        {
            List<string> textureFiles;
            string fileName = name.GetString();
            string sceneRoot = Path.GetDirectoryName(fileName);
            var root = new FusConverter().FuseefyScene(doc, sceneRoot, out textureFiles);

            var ser = new Fusee.Serialization.Serializer();
            using (var file = File.Create(fileName))
            {
                ser.Serialize(file, root);
            }

            return FILEERROR.FILEERROR_NONE;
        }

    }


    // Your Plugin Number for "FUSEE Android APK Exporter" is: 1037175 

    // Your Plugin Number for "FUSEE HTML Exporter" is: 1031842 
    [SceneSaverPlugin(1031842,
        Name = "FUSEE Web Viewer (*.html)",
        Suffix = "html")
    ]
    public class FusWebExporter : SceneSaverData
    {
        // call the base class' constructor with false!!! This keeps the garbage collector from collecting us
        public FusWebExporter()
            : base(false)
        {
            _newVersionExists = false;
        }

        private static FuseeHttpServer _httpServer;
        private bool _newVersionExists;


        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.ToUpperInvariant() == "FILE")
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }


        private void StartServer(string root)
        {
            if (_httpServer == null)
            {
                _httpServer = new FuseeHttpServer(root, 4655); // HEX: FU
                Thread thread = new Thread(_httpServer.listen);
                thread.Start();
            }
            else
            {
                _httpServer.HtDocsRoot = root;
            }
        }

        public override bool Init(GeListNode node)
        {
            Thread thread = new Thread(CheckForNewVersion);
            thread.Start();

            return true;
        }

        public void CheckForNewVersion()
        {
            try
            {
                /*
                 * A word on deployment: This code reads the file LastChange.txt. It should contain a date/time text in US-notation, e.g. 24-Dec-2014 20:15
                 * 
                 * This time is compared against the lastChanged file time of FuExport.dll. 
                 * 
                 * Make sure that the lastChanged file time of FuExport.dll is listed correctly in the deployed zip file. When updating the zip file contents
                 * wie Explorer the new date is listed in explorer but when unzipping aftwrwards with 7zip the original (old) date is restored. Always zip from 
                 * new with 7zip.
                 * 
                 * When deploying, make sure the new date within LastChange.txt is slightly older than the file date of the new FuExport.dll (but of course
                 * later than the old FuExport.dll). Otherwise, this plugin will report updated versions forever.
                 * 
                HttpWebRequest gameFile = (HttpWebRequest)WebRequest.Create("http://fusee3d.org/C4DPluginDownload/LastChanged.txt");
                HttpWebResponse gameFileResponse = (HttpWebResponse)gameFile.GetResponse();

                Assembly thisDll = Assembly.GetExecutingAssembly();
                DateTime localFileModifiedTime = File.GetLastWriteTime(thisDll.Location);
                DateTime onlineFileModifiedTime = gameFileResponse.LastModified;
                */
                WebClient client = new WebClient();
                Stream stream = client.OpenRead("http://fusee3d.org/C4DPluginDownload/LastChanged.txt");
                StreamReader reader = new StreamReader(stream);
                String content = reader.ReadToEnd();
                DateTime onlineFileTime = DateTime.Parse(content, CultureInfo.CreateSpecificCulture("en-US"));

                Assembly thisDll = Assembly.GetExecutingAssembly();
                DateTime localFileModifiedTime = File.GetLastWriteTime(thisDll.Location);


                _newVersionExists = (localFileModifiedTime < onlineFileTime);
            }
            catch (Exception ex) // Catch anything - this should in no case stop C4D from working
            {
                Logger.Error(ex.ToString());
                // swallow throw;
            }
        }

        public override FILEERROR Save(BaseSceneSaver node, Filename name, BaseDocument doc, SCENEFILTER filterflags)
        {
            if (_newVersionExists)
            {
                if (!C4dApi.QuestionDialog(
                    "A newer version of the FUSEE export plugin is available at fusee3d.org/c4dexporter.\n\n" +
                    "Would you like to \n" +
                    " - complete this export operation using the existing plugin [Yes]\n" +
                    "or\n" +
                    " - cancel this export operation and download the newer version [No]?"))
                {
                    C4dApi.GeOpenHTML("http://fusee3d.org/c4dexporter");
                    return FILEERROR.FILEERROR_NONE;
                }
                _newVersionExists = false; // stop annoying the user...
            }
            List<string> textureFiles;
            string htmlFilePath = name.GetString();
            string htmlFileDir = Path.GetDirectoryName(htmlFilePath);
            string sceneRoot = Path.Combine(htmlFileDir, "Assets");
            string fuseePlayerDir = Path.Combine(GetThisPluginPath(), "Viewer");

            DirCopy.DirectoryCopy(fuseePlayerDir, htmlFileDir, true, true);


            string origHtmlFilePath = Path.Combine(htmlFileDir, "SceneViewer.html");
            if (File.Exists(htmlFilePath))
                File.Delete(htmlFilePath);

            File.Move(origHtmlFilePath, htmlFilePath);

            string sceneFileDir = Path.Combine(htmlFileDir, "Assets");
            string sceneFilePath = Path.Combine(sceneFileDir, "Model.fus");

            var root = new FusConverter().FuseefyScene(doc, sceneFileDir, out textureFiles);

            var ser = new Serializer();
            using (var file = File.Create(sceneFilePath))
            {
                ser.Serialize(file, root);
            }

            for (int i = 0; i < textureFiles.Count; i++)
            {
                textureFiles[i] = Path.Combine("Assets", textureFiles[i]);
            }
            if (textureFiles != null)
                CreateAssetManifest(htmlFileDir, textureFiles);


            StartServer(htmlFileDir);
            C4dApi.GeOpenHTML("http://localhost:4655/" + Path.GetFileName(htmlFilePath));
            //Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "http://localhost:4655/" + Path.GetFileName(htmlFilePath));

            return FILEERROR.FILEERROR_NONE;
        }

        private static string GetThisPluginPath()
        {
            const string plugins = "Plugins";
            string path = Assembly.GetExecutingAssembly().Location;
            path = path.Remove(path.LastIndexOf(Path.DirectorySeparatorChar));
            return path;
        }

        private void CreateAssetManifest(string targWeb, IEnumerable<string> textureList)
        {
            List<string> filePaths = new List<string>();

            if (textureList != null)
                foreach (string texture in textureList)
                    filePaths.Add(Path.Combine(targWeb, texture));

            // filePaths = textureList.ToList(); // Directory.GetFiles(Path.Combine(targDir, "Assets")).ToList();
            filePaths.Add(Path.Combine(targWeb, "Assets", "Model.fus"));
            filePaths.Add(Path.Combine(targWeb, "Assets", "FuseeLogo150.png"));
            filePaths.Add(Path.Combine(targWeb, "Assets", "Lato-Black.ttf"));
            filePaths.Sort(string.Compare);

            // Load custom implementations first
            var fileCount = 0;

            var externalFiles = new[]
            {
                "Fusee.Base.Core.Ext",
                "Fusee.Base.Imp.Web.Ext",
                "opentype",
                "Fusee.Xene.Ext", 
                "Fusee.Xirkit.Ext",
                "Fusee.Engine.Imp.Graphics.Web.Ext",
                "SystemExternals",

                //"Fusee.Engine.Imp.WebAudio", "Fusee.Engine.Imp.WebNet", "Fusee.Engine.Imp.WebGL",
                //"Fusee.Engine.Imp.WebInput", "XirkitScript", "WebSimpleScene"
            };

            foreach (var extFile in externalFiles)
            {
                var exists = File.Exists(Path.Combine(targWeb, "Assets", "Scripts", extFile + ".js"));

                if (exists)
                {
                    var filePath = Path.Combine(targWeb, "Assets", "Scripts", extFile + ".js");
                    filePaths.Insert(fileCount, filePath);
                    fileCount++;
                }
                else
                {
                    Logger.Error("Couldn't find " + extFile + ".js");
                    return;
                }
            }

            var destRelativePaths = MakeAssetRelativePaths(filePaths, targWeb);
            // Create manifest
            string fileName = "Fusee.Engine.SceneViewer.Web";
            var manifest = new ManifestFile(fileName, filePaths, destRelativePaths, fileCount);
            string manifestContent = manifest.TransformText();

            File.WriteAllText(Path.Combine(targWeb, "Assets", "Scripts", fileName + ".contentproj.manifest.js"),
                manifestContent);
        }

        public static List<string> MakeAssetRelativePaths(IEnumerable<string> filePaths, string targWeb)
        {
            List<string> destRelativePaths = new List<string>();

            var srcAssetFolder = Path.Combine(targWeb, "Assets");
            if (!srcAssetFolder.EndsWith("\\")) srcAssetFolder += "\\";

            foreach (var filePath in filePaths)
            {

                var srcAssetDirPath = Path.GetDirectoryName(filePath);
                if (!srcAssetDirPath.EndsWith("\\")) srcAssetDirPath += "\\";

                var srcRelativeToAssetsDir = MakeRelativePath(srcAssetFolder, srcAssetDirPath);
                destRelativePaths.Add(srcRelativeToAssetsDir);
            }
            return destRelativePaths;
        }

    }

    // Your Plugin Number for "FUSEE Android APK Exporter" is: 1037175 
    //[SceneSaverPlugin(1037175,
    //    Name = "FUSEE Android App (*.apk)",
    //    Suffix = "apk")
    //]
    //public class FusApkExporter : SceneSaverData
    //{
    //    public FusApkExporter() : base(false)
    //    {
    //    }

    //    public override FILEERROR Save(BaseSceneSaver node, Filename name, BaseDocument doc, SCENEFILTER filterflags)
    //    {

    //        return FILEERROR.FILEERROR_NONE;
    //    }
    //}
}
