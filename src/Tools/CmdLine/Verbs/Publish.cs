using CommandLine;
using Fusee.Engine.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fusee.Tools.CmdLine.Verbs
{
    [Verb("publish", HelpText = "Packs a FUSEE app together with its dependencies and a player into a folder for deployment to a specific platform.")]
    internal class Publish
    {
        [Option('o', "output", HelpText = "Path to the directory where to place the deployment package files.")]
        public string Output { get; set; }

        [Option('i', "input", HelpText = "Path to the DLL containing the FUSEE app to be deployed (DLL must contain a class derived by RenderCanvas).")]
        public string Input { get; set; }

        [Option('p', "platform", Required = true, HelpText = "Platform the deployment packages is meant to run on. Possible values are: Desktop or Web.")]
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
                var ex = File.Exists(input);

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

            string desktopPlayerDir = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            string webPlayerDir = Path.Combine(desktopPlayerDir, "wwwroot");

            switch (Platform)
            {
                case Platform.Desktop:

                    if (!File.Exists(Path.Combine(desktopPlayerDir, "Player.exe")))
                    {
                        Console.Error.WriteLine($"ERROR: FUSEE Desktop Player is not present in {desktopPlayerDir}. Check your FUSEE installation.");
                        Environment.Exit((int)ErrorCode.InternalError);
                    }

                    break;
                case Platform.Web:

                    if (!File.Exists(Path.Combine(webPlayerDir, "index.html")))
                    {
                        Console.Error.WriteLine($"ERROR: FUSEE Web Player is not present in {webPlayerDir}. Check your FUSEE installation.");
                        Environment.Exit((int)ErrorCode.InternalError);
                    }

                    break;
                default:
                    Console.Error.WriteLine($"ERROR: Platform {Platform} is currently not handled by fusee.");
                    Environment.Exit((int)ErrorCode.PlatformNotHandled);
                    break;
            }


            // Do platform dependent stuff to integrate the FUSEE app DLL into the player
            switch (Platform)
            {
                case Platform.Desktop:
                    try
                    {
                        Directory.CreateDirectory(outPath);

                        // Copy the player
                        File.Copy(Path.Combine(desktopPlayerDir, "Player.exe"), Path.Combine(outPath, appName + ".exe"));
                        File.Copy(Path.Combine(desktopPlayerDir, "glfw3.dll"), Path.Combine(outPath, "glfw3.dll"));

                        // Copy the FUSEE App on top of the player.
                        File.Copy(dllFilePath, Path.Combine(outPath, "Fusee.App.dll"));

                        // Copy Assets
                        FileTools.DirectoryCopy(Path.Combine(dllDirPath, "Assets"), Path.Combine(outPath, "Assets"), true, true);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("ERROR: internal error while publishing FUSEE Desktop App: " + ex);
                        Environment.Exit((int)ErrorCode.InternalError);
                    }
                    Console.Error.WriteLine($"SUCCESS: FUSEE Desktop App {appName}.exe generated at {outPath}.");
                    Environment.Exit(0);
                    break;
                case Platform.Web:
                    try
                    {
                        Directory.CreateDirectory(outPath);

                        // Copy the player
                        FileTools.DirectoryCopy(webPlayerDir, outPath, true, true);

                        // Copy the FUSEE App on top of the player.
                        File.Copy(dllFilePath, Path.Combine(outPath, "Fusee.App.dll"));

                        // Copy Assets
                        FileTools.DirectoryCopy(Path.Combine(dllDirPath, "Assets"), Path.Combine(outPath, "Assets"), true, true);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("ERROR: internal error while publishing FUSEE Web App: " + ex);
                        Environment.Exit((int)ErrorCode.InternalError);
                    }


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