using System;
using System.Collections.Generic;
using System.IO;

namespace Fusee.Tools.fuConv
{
    public class AssetManifest
    {
        public static void CreateAssetManifest(string targWeb, IEnumerable<string> textureList)
        {
            List<string> filePaths = new List<string>();

            if (textureList != null)
                foreach (string texture in textureList)
                    filePaths.Add(Path.Combine(targWeb, texture));

            // filePaths = textureList.ToList(); // Directory.GetFiles(Path.Combine(targDir, "Assets")).ToList();
            filePaths.Add(Path.Combine(targWeb, "Assets", "Model.fus"));
            filePaths.Add(Path.Combine(targWeb, "Assets", "FuseeLogo150.png"));
            filePaths.Add(Path.Combine(targWeb, "Assets", "Lato-Black.ttf"));
            filePaths.Sort(String.Compare);

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
                    Console.Error.WriteLine($"Couldn't find " + extFile + ".js");
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

        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme)
            {
                return toPath;
            } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.ToUpperInvariant() == "FILE")
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }
    }
}