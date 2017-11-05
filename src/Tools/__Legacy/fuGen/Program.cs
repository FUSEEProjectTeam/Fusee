using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fusee.Tools.fuGen
{
    internal class Program
    {
        private static bool _debugMode;

        private static void DebugMode(string msg)
        {
            if (_debugMode)
                Console.WriteLine("// " + msg);
        }

        private static void CreateDirectories(string targWeb)
        {
            if (!Directory.Exists(Path.Combine(targWeb, "Assets")))
                Directory.CreateDirectory(Path.Combine(targWeb, "Assets"));

            if (!Directory.Exists(Path.Combine(targWeb, "Assets", "Scripts")))
                Directory.CreateDirectory(Path.Combine(targWeb, "Assets", "Scripts"));

            if (!Directory.Exists(Path.Combine(targWeb, "Assets", "Styles")))
                Directory.CreateDirectory(Path.Combine(targWeb, "Assets", "Styles"));

            if (!Directory.Exists(Path.Combine(targWeb, "Assets", "Config")))
                Directory.CreateDirectory(Path.Combine(targWeb, "Assets", "Config"));
        }


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

        private static int Main(string[] args)
        {
            if (args.Length < 3) return 1;

            var targDir = args[0];
            var targWeb = args[1];
            var targApp = args[2];
            var externalFiles = args[3].Split(';');

            //if (args.Length == 4)
            //    _debugMode = (args[3] == "-d");
            _debugMode = true;

            string fileName = Path.GetFileNameWithoutExtension(targApp);

            // Create directories
            CreateDirectories(targWeb);

            // Does HTML already exists?
            var newHTML = !File.Exists(targWeb + fileName + ".html");

            Console.WriteLine(newHTML
                ? "// No HTML file found - generating a simple HTML file"
                : "// HTML file already exists - delete it to create a new one");

            // Collecting all files
            var customManifest = Directory.Exists(Path.Combine(targDir, "Assets"));
            var customCSS = "";

            Console.WriteLine(customManifest
                ? "// Found an Assets folder - collecting all and write manifest"
                : "// No Assets folder - no additional files will be added");

            List<string> filePaths;

            if (customManifest)
            {
                filePaths = Directory.GetFiles(Path.Combine(targDir, "Assets"), "*.*", SearchOption.AllDirectories).ToList();
                filePaths.Sort(string.Compare);
            }
            else
                filePaths = new List<string>();

            // Load custom implementations first
            var fileCount = 0;

            /*
            var externalFiles = new[]
            {
                "Fusee.Engine.Imp.WebAudio", "Fusee.Engine.Imp.WebNet", "Fusee.Engine.Imp.WebGL",
                "Fusee.Engine.Imp.WebInput", "XirkitScript", "WebSimpleScene"
            };
            */

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
                    DebugMode("Couldn't find " + extFile + ".js");
                    return 1;
                }
            }

            List<string> destRelativePaths = new List<string>(filePaths.Count);
            for (int inx = 0; inx < filePaths.Count; inx++)
                destRelativePaths.Add("");

            if (customManifest)
            {
                // Copy to output folder
                for (var ct = filePaths.Count - 1; ct > fileCount - 1; ct--)
                {
                    bool remove = false;
                    string pathExt = "";
                    string filePath = filePaths.ElementAt(ct);

                    // style or config
                    if (Path.GetExtension(filePath) == ".css")
                    {
                        customCSS = Path.GetFileName(filePath);
                        pathExt = "Styles";
                        remove = true;
                    }

                    if (Path.GetFileName(filePath) == "fusee_config.xml")
                    {
                        pathExt = "Config";
                        remove = true;
                    }

                    var srcAssetFolder = Path.Combine(targDir, "Assets");
                    if (!srcAssetFolder.EndsWith("\\")) srcAssetFolder += "\\";

                    var srcAssetDirPath = Path.GetDirectoryName(filePath);
                    if (!srcAssetDirPath.EndsWith("\\")) srcAssetDirPath += "\\";

                    var srcRelativeToAssetsDir = MakeRelativePath(srcAssetFolder, srcAssetDirPath);
                    pathExt = srcRelativeToAssetsDir;
                    // DebugMode("MakeRelativePath(" + srcAssetFolder + ", " + srcAssetDirPath + "); yields: " + srcRelativeToAssetsDir);

                    // Copy files to output if they don't exist yet
                    var tmpFileName = Path.GetFileName(filePath);
                    var dstFilePath = Path.Combine(targWeb, "Assets", pathExt, tmpFileName);

                    if (tmpFileName != null && !File.Exists(dstFilePath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(dstFilePath));
                        File.Copy(filePath, dstFilePath);
                    }

                    destRelativePaths[ct] = pathExt;

                    if (remove)
                    {
                        filePaths.RemoveAt(ct);
                        destRelativePaths.RemoveAt(ct);
                    }
                }
            }

            // Create manifest
            var manifest = new ManifestFile(fileName, filePaths, destRelativePaths, fileCount);
            string manifestContent = manifest.TransformText();

            File.WriteAllText(Path.Combine(targWeb, "Assets", "Scripts", fileName + ".contentproj.manifest.js"),
                manifestContent);

            // Create HTML file
            if (newHTML)
            {
                Console.WriteLine(customCSS == ""
                    ? "// No additional .css file found in Assets folder - using only default one"
                    : "// Found an additional .css file in Assets folder - adding to HTML file");

                var page = new WebPage(targApp, customCSS);
                string pageContent = page.TransformText();

                File.WriteAllText(Path.Combine(targWeb, fileName + ".html"), pageContent);
            }

            // Create config file
            var customConf = File.Exists(Path.Combine(targDir, "Assets", "fusee_config.xml"));

            Console.WriteLine(!customConf
                ? "// No custom config file ('fusee_config.xml') found in Assets folder - using default settings"
                : "// Found an custom config file in Assets folder - applying settings to webbuild");

            var conf = new JsilConfig(targApp, targDir, customConf);
            string confContent = conf.TransformText();

            File.WriteAllText(Path.Combine(targWeb, "Assets", "Config", "jsil_config.js"), confContent);

            // Done
            Console.WriteLine("// Finished all tasks");

            return 0;
        }
    }
}