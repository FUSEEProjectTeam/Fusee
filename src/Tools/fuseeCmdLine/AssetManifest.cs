using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Fusee.Tools.fuseeCmdLine
{
    public static class AssetManifest
    {
        static IEnumerable<string> GetFiles(string path)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i];
                    }
                }
            }
        }

        // Copy the assets from the build-output folder (bin/Debug/Assets) to the publication-output assets folder (pub/Web/Assets = publicationOutputDir + "/Assets")
        // Create an entry for each found asset file within the existing asset manifest file 
        public static void AdjustAssetManifest(string buildOutputDir, string publicationOutputDir, string assetManifestFile)
        {
            List<string> filePaths = new List<string>();
            List<string> destRelativePaths = new List<string>();

            string buildOutputAssetDir = Path.Combine(buildOutputDir, "Assets");
            string buildOutputAssetDirT = PathAddTrailingSeperator(buildOutputAssetDir);
            string publicationOutputAssetDir = Path.Combine(publicationOutputDir, "Assets");
            foreach (string filePath in GetFiles(buildOutputAssetDir))
            {
                filePaths.Add(filePath);

                var srcAssetDirPath = Path.GetDirectoryName(filePath);
                srcAssetDirPath = PathAddTrailingSeperator(srcAssetDirPath);

                destRelativePaths.Add(MakeRelativePath(buildOutputAssetDirT, srcAssetDirPath));
            }

            var fileNamesList = new List<string>();
            var fileSizeList = new List<long>();
            var fileTypesList = new List<string>();
            var fileFormatsList = new List<string>();
            GenerateAssetManifestEntryItems(filePaths, destRelativePaths, 0, fileNamesList, fileSizeList, fileTypesList, fileFormatsList);
            StringBuilder sb = new StringBuilder();
            for (int i= 0; i < fileNamesList.Count; i++)
            {
                sb.AppendLine($"    [\"{fileTypesList[i]}\",    \"{fileNamesList[i]}\", {{{fileFormatsList[i]} \"sizeBytes\": {fileSizeList[i]} }}],");
            }
            string assetContents = File.ReadAllText(assetManifestFile);
            var m = Regex.Match(assetContents, @"    \];");
            assetContents = assetContents.Insert(m.Index - 1, sb.ToString());
            File.WriteAllText(assetManifestFile, assetContents);
        }


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
            string projName = "Fusee.Engine.SceneViewer.Web";

            var fileNamesList = new List<string>();
            var fileSizeList = new List<long>();
            var fileTypesList = new List<string>();
            var fileFormatsList = new List<string>();

            GenerateAssetManifestEntryItems(filePaths, destRelativePaths, fileCount, fileNamesList, fileSizeList, fileTypesList, fileFormatsList);
            var manifest = new ManifestFile(projName, fileNamesList, fileSizeList, fileTypesList, fileFormatsList);

            string manifestContent = manifest.TransformText();
            File.WriteAllText(Path.Combine(targWeb, "Assets", "Scripts", projName + ".contentproj.manifest.js"),
                manifestContent);
        }

        public static List<string> MakeAssetRelativePaths(IEnumerable<string> filePaths, string targWeb)
        {
            List<string> destRelativePaths = new List<string>();

            var srcAssetFolder = Path.Combine(targWeb, "Assets");
            srcAssetFolder = PathAddTrailingSeperator(srcAssetFolder);

            foreach (var filePath in filePaths)
            {
                var srcAssetDirPath = Path.GetDirectoryName(filePath);
                srcAssetDirPath = PathAddTrailingSeperator(srcAssetDirPath);
                
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

        static string PathAddTrailingSeperator(string path)
        {
            string separator1 = Path.DirectorySeparatorChar.ToString();
            string separator2 = Path.AltDirectorySeparatorChar.ToString();

            path = path.TrimEnd();
            if (path.EndsWith(separator1) || path.EndsWith(separator2))
                return path;
            if (path.Contains(separator2))
                return path + separator2;
            return path + separator1;
        }

        private static void GenerateAssetManifestEntryItems(List<string> filePaths, List<string> dstRelPaths, int specFiles, List<string> fileNamesList, List<long> fileSizeList, List<string> fileTypesList, List<string> fileFormatsList)
        {
            for (var ct = 0; ct <= filePaths.Count - 1; ct++)
            {
                string filePath = filePaths.ElementAt(ct);

                var fInfo = new FileInfo(filePath);
                string fType = FileTypes.GetFileType(filePath);

                // size, type
                fileSizeList.Add(fInfo.Length);
                fileTypesList.Add(fType);

                // special files
                if (ct < specFiles)
                {
                    fileNamesList.Add(Path.GetFileName(filePath));
                    fileFormatsList.Add(" ");
                    continue;
                }

                var pathExt = dstRelPaths[ct].Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (pathExt != "" && !pathExt.EndsWith("/"))
                    pathExt += "/";

                // Sound files in more than one format
                var doubleExt = false;

                if ((fType == "Sound") && (ct < filePaths.Count - 1))
                {
                    string fileName1 = Path.GetFileNameWithoutExtension(filePath);
                    string fileName2 = Path.GetFileNameWithoutExtension(filePaths.ElementAt(ct + 1));

                    if (fileName1 == fileName2)
                    {
                        string ext1 = Path.GetExtension(filePath);
                        string ext2 = Path.GetExtension(filePaths.ElementAt(ct + 1));

                        fileFormatsList.Add(" \"formats\": [\"" + ext1 + "\", \"" + ext2 + "\"],	");
                        fileNamesList.Add("Assets/" + pathExt + fileName1);

                        ct++;
                        doubleExt = true;
                    }
                }

                if (!doubleExt)
                {
                    if ((fType == "Sound"))
                    {
                        string fileName1 = Path.GetFileNameWithoutExtension(filePath);
                        string ext1 = Path.GetExtension(filePath);
                        fileNamesList.Add("Assets/" + pathExt + fileName1);
                        fileFormatsList.Add(" \"formats\": [\"" + ext1 + "\"],	");
                    }
                    else
                    {
                        fileNamesList.Add("Assets/" + pathExt + Path.GetFileName(filePath));
                        fileFormatsList.Add(" ");
                    }
                }
            }
        }
    }
}