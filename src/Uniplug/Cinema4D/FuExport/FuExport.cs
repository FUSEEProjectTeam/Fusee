using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Threading;
using C4d;
using Fusee.Math;
using Fusee.Serialization;

namespace FuExport
{
    [SceneSaverPlugin(1000048,
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
            var root = FusConverter.FuseefyScene(doc);

            var ser = new Serializer();
            using (var file = File.Create(name.GetString()))
            {
                ser.Serialize(file, root);
            }

            return FILEERROR.FILEERROR_NONE;
        }

    }


    [SceneSaverPlugin(1000049,
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
                Thread thread = new Thread(new ThreadStart(_httpServer.listen));
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
            string htmlFilePath = name.GetString();
            string htmlFileDir = Path.GetDirectoryName(htmlFilePath);
            string fuseePlayerDir = Path.Combine(GetThisPluginPath(), "Viewer");
            
            DirCopy.DirectoryCopy(fuseePlayerDir, htmlFileDir, true, true);


            string origHtmlFilePath = Path.Combine(htmlFileDir, "Examples.SceneViewer.html");
            if (File.Exists(htmlFilePath))
                File.Delete(htmlFilePath);

            File.Move(origHtmlFilePath, htmlFilePath);

            string sceneFilePath = Path.Combine(htmlFileDir, "Assets", "Model.fus");

            var root = FusConverter.FuseefyScene(doc);
            
            var ser = new Serializer();
            using (var file = File.Create(sceneFilePath))
            {
                ser.Serialize(file, root);
            }

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

    }

}
