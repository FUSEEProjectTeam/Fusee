using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fusee.Tools.fuseeCmdLine.Verbs.WebAsm
{
    [Verb("webasm", HelpText = "Create a new WebAssembly project folder including all necessary files (*.csproj, index.html, etc.)")]
    internal class WebAsm
    {
        [Value(0, HelpText = "Namespace of the new project.", MetaName = "Namespace (s)", Required = true)]
        public string Namespace { get; set; }

        [Value(1, HelpText = "Name of the new project.", MetaName = "ProjectName (n)", Required = true)]
        public string ProjectName { get; set; }

        [Value(2, HelpText = "Output folder of the new project. Will be created if not existent.", MetaName = "OutputFolder (o)", Required = true)]
        public string OutputFolder { get; set; }


        public int Run()
        {
            // try to create a file info from given path, if it fails this path is not valid
            FileInfo fi = null;
            try
            {
                fi = new FileInfo(OutputFolder);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"ArgumentException: {e.Message}");
                return 1;
            }
            catch (PathTooLongException e)
            {
                Console.WriteLine($"PathTooLongException: {e.Message}");
                return 1;
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine($"NotSupportedException: {e.Message}");
                return 1;
            }           
                if (fi is null)
                {
                    Console.WriteLine($"{OutputFolder} is no valid output path!");
                    return 1;
                }
           

            if (!Directory.Exists(OutputFolder))
                Directory.CreateDirectory(OutputFolder);

            return CopyAllFiles();
        }

        private void ReplaceContents(ref string content)
        {
            content = content.Replace("__NAMESPACE__", Namespace).Replace("__PROJECTNAME__", ProjectName);
        }

        private int CopyAllFiles()
        {
            try
            {
                foreach (var file in Directory.GetFiles("Verbs\\WebAsm\\_Assets\\", "*.*", SearchOption.AllDirectories))
                {

                    // handle csproj, needs renaming
                    if (file.Contains("Fusee.__NAMESPACE__.__PROJECTNAME__.WebAsm._csproj"))
                    {
                        var fileNameNoExt = Path.GetFileNameWithoutExtension(file);

                        var content = File.ReadAllText(file);
                        ReplaceContents(ref content);
                        ReplaceContents(ref fileNameNoExt);

                        var fileOutputPath = Path.Combine(OutputFolder, fileNameNoExt + ".csproj");

                        File.WriteAllText(fileOutputPath, content);

                        continue;
                    }

                    // handle index.html
                    // handle Main.cs
                    if (file.Contains("index.html") || file.Contains("Main.cs"))
                    {
                        var content = File.ReadAllTextAsync(file).Result;
                        ReplaceContents(ref content);

                        var filename = Path.GetFileName(file);

                        File.WriteAllText(Path.Combine(OutputFolder, filename), content);
                        continue;
                    }

                    var path = Path.GetRelativePath("Verbs\\WebAsm\\_Assets\\", file);

                    // remove the filename from relative path to create this path if directory is missing (e.g. "Scripts" folder)
                    var filenameFromRelativePath = Path.GetFileName(path);
                    var relativePath = path.Replace(filenameFromRelativePath, "");

                    if (!string.IsNullOrEmpty(relativePath) && !Directory.Exists(Path.Combine(OutputFolder, relativePath)))
                        Directory.CreateDirectory(Path.Combine(OutputFolder, relativePath));


                    File.Copy(file, Path.Combine(OutputFolder, path), true);
                }
            }
            catch (FileLoadException e)
            {
                Console.WriteLine($"Error during file write {e.Message}, {e.InnerException}");
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown error during file write {e.Message}, {e.InnerException}");
                return 1;
            }

            return 0;
        }


    }
}
