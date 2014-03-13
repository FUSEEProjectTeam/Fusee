using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var ser = new Serializer();
            using (var file = File.Create(fileName))
            {
                ser.Serialize(file, root);
            }
            
            return FILEERROR.FILEERROR_NONE;
        }

    }


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

        }

        private static FuseeHttpServer _httpServer;


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
            return true;
        }


        public override FILEERROR Save(BaseSceneSaver node, Filename name, BaseDocument doc, SCENEFILTER filterflags)
        {
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
            string sceneFilePath = Path.Combine(sceneFileDir,  "Model.fus");

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
            List<string> filePaths;

            filePaths = textureList.ToList(); // Directory.GetFiles(Path.Combine(targDir, "Assets")).ToList();
            filePaths.Add("Assets/Model.fus");
            filePaths.Add("Assets/FuseeLogo150.png");
            filePaths.Add("Assets/Lato-Black.ttf");
            filePaths.Sort(string.Compare);

            // Load custom implementations first
            var fileCount = 0;

            var externalFiles = new[]
            {
                "Fusee.Engine.Imp.WebAudio", "Fusee.Engine.Imp.WebNet", "Fusee.Engine.Imp.WebGL",
                "Fusee.Engine.Imp.WebInput", "XirkitScript", 
            };

            foreach (var extFile in externalFiles)
            {
                var exists = File.Exists(Path.Combine(targWeb, "Assets", "Scripts", extFile + ".js"));

                if (exists)
                {
                    filePaths.Insert(fileCount, Path.Combine(targWeb, "Assets", "Scripts", extFile + ".js"));
                    fileCount++;
                }
                else
                {
                    Logger.Error("Couldn't find " + extFile + ".js");
                    return;
                }
            }

            // Create manifest
            string fileName = "Examples.SceneViewer";
            var manifest = new ManifestFile(fileName, filePaths, fileCount);
            string manifestContent = manifest.TransformText();

            File.WriteAllText(Path.Combine(targWeb, "Assets", "Scripts", fileName + ".contentproj.manifest.js"),
                manifestContent);
        }

    }

}
