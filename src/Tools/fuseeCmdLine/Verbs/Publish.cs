using CommandLine;
using Fusee.Engine.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fusee.Tools.fuseeCmdLine
{
    [Verb("publish", HelpText = "Packs a FUSEE app together with its dependencies and a player into a folder for deployment to a specific platform.")]
    internal class Publish
    {
        [Option('o', "output", HelpText = "Path to the directory where to place the deployment package files.")]
        public string Output { get; set; }

        [Option('i', "input", HelpText = "Path to the DLL containing the FUSEE app to be deployed (DLL must contain a class derived by RenderCanvas).")]
        public string Input { get; set; }

        [Option('p', "platform", Default = Platform.Desktop, HelpText = "Platform the deployment packages is meant to run on. Possible values are: Desktop, Web or Android.")]
        public Platform Platform { get; set; }

        public int Run()
        {
            // INPUT
            string input = Input;
            string appName = null;
            string dllFilePath = null;
            string dllDirPath = null;
            string dir = null;
            // See if anything is specified at all
            if (string.IsNullOrEmpty(input) || Directory.Exists(input))
            {
                dir = string.IsNullOrEmpty(input) ? Directory.GetCurrentDirectory() : Path.GetFullPath(input);
                // See if we are inside a .Net Core project (is there a .csproj file)?
                var csprojFile = Directory.EnumerateFiles(dir, "*.csproj").FirstOrDefault();
                if (!string.IsNullOrEmpty(csprojFile))
                {
                    // We are at the root of a DotNet project.
                    appName = Path.GetFileNameWithoutExtension(csprojFile);
                    try
                    {
                        // Find dlls with the .csproj file's name below the bin subdir
                        // Orderby.Reverse will make Release builds appear prior to Debug builds
                        dllFilePath = Directory
                            .EnumerateFiles(Path.Combine(dir, "bin"),
                                appName + ".dll", SearchOption.AllDirectories).OrderBy(s => s).Reverse().First();
                    }
                    catch (Exception /* ex */)
                    {
                    }
                }
                else
                {
                    Console.Error.WriteLine($"ERROR: {dir} does not contain any FUSEE App project (.csproj file).");
                    Environment.Exit((int)ErrorCode.InputFile);
                }
                if (string.IsNullOrEmpty(dllFilePath))
                {
                    Console.Error.WriteLine($"ERROR: {appName}.dll could not be found below bin subdirectory. Build {appName}.csproj before publishing.");
                    Environment.Exit((int)ErrorCode.InputFile);
                }
            }
            else
            {
                // A dll was explicitely mentioned. See if it exists
                if (File.Exists(input) && Path.GetExtension(input).ToLower() == ".dll")
                {
                    try
                    {
                        var fullPath = Path.GetFullPath(input);
                        appName = Path.GetFileNameWithoutExtension(fullPath);
                        dllFilePath = fullPath;
                        dir = Path.Combine(Path.GetPathRoot(fullPath), Path.GetDirectoryName(fullPath));
                    }
                    catch (Exception /* ex */)
                    {
                    }
                }
                if (string.IsNullOrEmpty(dllFilePath))
                {
                    Console.Error.WriteLine($"ERROR: {input} does not exist or is not a DLL file.");
                    Environment.Exit((int)ErrorCode.InputFile);
                }
            }

            // Check if the specified infile really contains a FUSEE App
            Type tApp = null;
            try
            {
                Assembly asm = Assembly.LoadFrom(dllFilePath);
                tApp = asm.GetTypes().FirstOrDefault(t => typeof(RenderCanvas).IsAssignableFrom(t));
                if (tApp == null)
                {
                    Console.Error.WriteLine($"ERROR: {input} does not contain a FUSEE app (a class derived from RenderCanvas).");
                    Environment.Exit((int)ErrorCode.InputFile);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ERROR: Cannot read contents of {dllFilePath}: {ex}");
                Environment.Exit((int)ErrorCode.InternalError);
            }
            dllDirPath = Path.Combine(Path.GetPathRoot(dllFilePath), Path.GetDirectoryName(dllFilePath));
            // End of Input handling

            // OUTPUT
            string output = Output;
            string outPath = null;
            if (string.IsNullOrEmpty(output))
            {
                outPath = Path.Combine(dir, "pub", Platform.ToString());
            }
            else
            {
                outPath = output;
            }
            // End of output handling.

            // Empty the given directory (if present)
            if (Directory.Exists(outPath))
            {
                try
                {
                    Directory.Delete(outPath, true);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"ERROR: deleting {outPath}. Make sure that fusee can delete its contents or remove it before publishing.\n{ex}");
                    Environment.Exit((int)ErrorCode.OutputFile);
                }
            }

            var fuseePaths = Helper.InitFuseeDirectories();

            string playerFile = null;
            string desktopPlayerDir = Path.GetFullPath(Path.Combine(fuseePaths.fuseeBuildRoot, "Player", "Desktop")); // need this in web build as well.
            switch (Platform)
            {
                case Platform.Desktop:
                    playerFile = Path.GetFullPath(Path.Combine(desktopPlayerDir, "Fusee.Engine.Player.Desktop.exe"));
                    break;
                case Platform.Web:
                    playerFile = Path.GetFullPath(Path.Combine(
                        fuseePaths.fuseeBuildRoot, "Player", "Web", "Fusee.Engine.Player.Web.html"));
                    break;
                default:
                    Console.Error.WriteLine($"ERROR: Platform {Platform.ToString()} is currently not handled by fusee.");
                    Environment.Exit((int)ErrorCode.PlatformNotHandled);
                    break;
            }
            if (!File.Exists(playerFile))
            {
                Console.Error.WriteLine($"ERROR: FUSEE Player {playerFile} is not present. Check your FUSEE installation.");
                Environment.Exit((int)ErrorCode.InternalError);
            }
            string playerDir = Path.Combine(Path.GetPathRoot(playerFile), Path.GetDirectoryName(playerFile));

            // Copy the player
            FileTools.DirectoryCopy(playerDir, outPath, true, true);

            // Do platform dependent stuff to integrate the FUSEE app DLL into the player
            switch (Platform)
            {
                case Platform.Desktop:
                    try
                    {
                        // Copy the FUSEE App on top of the player.
                        FileTools.DirectoryCopy(dllDirPath, outPath, true, true);

                        // Rename Player.exe to App Name
                        File.Move(Path.Combine(outPath, "Fusee.Engine.Player.Desktop.exe"), Path.Combine(outPath, appName + ".exe"));

                        // Rename App DLL to "Fusee.App.dll"
                        File.Move(Path.Combine(outPath, appName + ".dll"), Path.Combine(outPath, "Fusee.App.dll"));

                        // Delete all pdb and xml files
                        var directory = new DirectoryInfo(outPath);
                        foreach (var file in directory.EnumerateFiles("*.pdb"))
                            file.Delete();
                        foreach (var file in directory.EnumerateFiles("*.xml"))
                            file.Delete();
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("ERROR: internal error while publishing FUSEE Desktop App: " + ex);
                        Environment.Exit((int)ErrorCode.InternalError);
                    }
                    Console.Error.WriteLine($"SUCCESS: FUSEE Desktop App {appName}.exe generated at {outPath}.");
                    Environment.Exit(0);
                    break;
            }
            return 0;
        }
    }

    public enum Platform
    {
        Desktop,
        Web,
        Android,
    }
}
