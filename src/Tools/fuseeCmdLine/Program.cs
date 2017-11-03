using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using Assimp;
using CommandLine;
using Fusee.Serialization;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using CommandLine.Infrastructure;
using CommandLine.Text;
using Fusee.Engine.Core;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;
using System.Security;

namespace Fusee.Tools.fuseeCmdLine
{
    enum ErrorCode : int
    {
        CommandLineSyntax = -1,
        InputFile = -2,
        InputFormat = -3,
        OutputFile = -4,
        PlatformNotHandled = -5,
        InsufficentPrivileges = -6,

        InternalError = -42,
    }

    class Program
    {
        static AssimpContext Assimp => _assimpCtx ?? (_assimpCtx = new AssimpContext());
        private static AssimpContext _assimpCtx;
        private static FuseeHttpServer _httpServer;
        private static Thread _httpThread;

        [Verb("scene", HelpText = "Convert 3D scene input into .fus.")]
        public class SceneOptions
        {
            [Value(0,
                HelpText =
                    "Input scene file in a recognized format. Use the 'inputsceneformats' command to retrieve a list of supported formats.",
                MetaName = "Input", Required = true)]
            public string Input { get; set; }

            [Option('o', "output",
                HelpText = "Path of .fus file to be written. \".fus\" extension will be added if not present.")]
            public string Output { get; set; }

            [Option('f', "format",
                HelpText =
                    "Input file format overriding the file extension (if any). For example 'obj' for a Wavefront .obj file."
            )]
            public string Format { get; set; }
        }

        [Verb("inputsceneformats", HelpText = "List the supported input scene formats.")]
        public class InputSceneFormats
        {
        }

        [Verb("protoschema", HelpText = "Output the protobuf schema for the .fus file format.")]
        public class ProtoSchema
        {
            [Option('o', "output",
                HelpText = "Path of .proto file to be written. \".proto\" extension will be added if not present.")]
            public string Output { get; set; }
        }


        public enum Platform
        {
            Desktop,
            Web,
            Android,
        }

        [Verb("publish", HelpText =
            "Packs a FUSEE app together with its dependencies and a player into a folder for deployment to a specific platform.")]
        public class Publish
        {
            [Option('o', "output",
                HelpText = "Path to the directory where to place the deployment package files.")]
            public string Output { get; set; }

            [Option('i', "input",
                HelpText = "Path to the DLL containing the FUSEE app to be deployed (DLL must contain a class derived by RenderCanvas).")]
            public string Input { get; set; }

            [Option('p', "platform", Default = Platform.Desktop,
                HelpText = "Platform the deployment packages is meant to run on. Possible values are: Desktop, Web or Android.")]
            public Platform Platform { get; set; }
        }

        [Verb("server", HelpText = "Launch a minimalistic local webserver and start the default browser.")]
        public class Server
        {
            [Value(0, HelpText = "Directory or File path to be used as WWWRoot. If a file name is given, it will be started in the browser. If not, a default start file will be looked for.", MetaName = "Root")]
            public string Root { get; set; }

            [Option('p', "port", Default = 4655, // HEX FU,
                HelpText = "Port the server should start running on.")]
            public int Port { get; set; }

            [Option('s', "serveronly", Default = false, HelpText ="Launches the server but does not start the default browser.")]
            public bool Serveronly { get; set; }
        }

        public enum InstallationType
        {
            User,
            Machine,
        }

        [Verb("install", HelpText = "Register this instance of FUSEE as the current installation - this invokes three steps: (1) Set the \"FuseeRoot\" environment variable. (2) Add the fusee.exe directory to the \"PATH\" environment variable. (3) Register the 'dotnet new fusee' template.")]
        public class Install
        {
            [Option('t', "type", Default = InstallationType.User, HelpText = "Machine-wide or per-user installation. '-t User' will set \"FuseeRoot\" and \"PATH\" for the current user only. '-t Machine' will set \"FuseeRoot\" and \"PATH\" for all users. Start shell (cmd or powershell) as Administrator for machine-wide installation")]
            public InstallationType InstType { get; set; }

            [Option('u', "uninstall", Default = false, HelpText = "De-Register this FUSEE installation. This will only de-register this FUSEE instance (deregister the 'dotnet new fusee' template, remove fusee.exe from the \"PATH\" and remove the \"FuseeRoot\" environment varable). This will NOT delete the contents of the installation folder.")]
            public bool Uninstall { get; set; }

        }


        [Verb("web", HelpText = "Use an existing .fus-file to start a webserver. Deprectated. Currently used in the FUSEE Blender Export AddIn. To be replaced by the publish verb.")]
        public class WebViewer
        {
            [Value(0, HelpText = "Input .fus-file.", MetaName = "Input", Required = true)]
            public string Input { get; set; }

            [Option('o', "output", HelpText = "Target Directoy")]
            public string Output { get; set; }

            [Option('l', "list", HelpText = "List of paths to texture files")]
            public string List { get; set; }
        }

        [Verb("generate", HelpText = "Generate necessary web export files (Config file, Manifest file, HTML page). Deprecated. Currently used in the FUSEE build process (FuseeBuildActions.target.xml). To be replaced by the publish verb.")]
        public class Generate
        {
            [Value(0, HelpText = "Target Directory", MetaName = "TargetDir", Required = true)]
            public string TargDir { get; set; }

            [Value(1, HelpText = "Target Web", MetaName = "TargetWeb", Required = true)]
            public string TargWeb { get; set; }

            [Value(2, HelpText = "Target Application Path", MetaName = "TargetApp", Required = true)]
            public string TargAppPath { get; set; }

            [Value(3, HelpText = "External (Script) Files", MetaName = "ExternalFiles", Required = true)]
            public string ExternalFiles { get; set; }
        }

        // "Globals"
        static string fuseeCmdLineRoot = null;
        static string fuseeRoot = null;
        static string fuseeBuildRoot = null;

        static void InitFuseeDirectories()
        {
            // Check if player is present
            try
            {
                fuseeCmdLineRoot = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                fuseeBuildRoot = Path.GetFullPath(Path.Combine(fuseeCmdLineRoot, ".."));        // one hop down to remove "Tools" from %FuseeRoot%bin/[Debug|Release]/Tools.
                fuseeRoot = Path.GetFullPath(Path.Combine(fuseeCmdLineRoot, "..", "..", "..")); // three hops from %FuseeRoot%bin/[Debug|Release]/Tools down to the root.
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ERROR: this instance of fusee.exe at {fuseeCmdLineRoot} doesn't seem to be part of a FUSEE installation.\n{ex}");
                Environment.Exit((int)ErrorCode.InternalError);
            }

        }

        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Publish, Server, Install, ProtoSchema, SceneOptions, InputSceneFormats, WebViewer, Generate>(args)

                // Called with the SCENE verb
                .WithParsed<SceneOptions>(opts =>
                {
                    Stream input = null, output = null;

                    // Check and open input file
                    string inputFormat = Path.GetExtension(opts.Input).ToLower();
                    if (String.IsNullOrEmpty(inputFormat))
                    {
                        if (String.IsNullOrEmpty(opts.Format))
                        {
                            Console.Error.WriteLine($"ERROR: No input format specified.");
                            Environment.Exit((int)ErrorCode.InputFormat);
                        }
                        inputFormat = opts.Format.ToLower();
                        if (inputFormat[0] != '.')
                            inputFormat = inputFormat.Insert(0, ".");
                    }
                    if (!Assimp.IsImportFormatSupported(inputFormat))
                    {
                        Console.Error.WriteLine($"ERROR: Unsupported input format {inputFormat}.");
                        Environment.Exit((int)ErrorCode.InputFormat);
                    }
                    try
                    {
                        input = File.Open(opts.Input, FileMode.Open);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR opening input file '{opts.Input}':");
                        Console.Error.WriteLine(ex);
                        Environment.Exit((int)ErrorCode.InputFile);
                    }

                    // Check and open output file
                    if (String.IsNullOrEmpty(opts.Output))
                    {
                        string inp = Path.GetFullPath(opts.Input);
                        opts.Output = Path.Combine(
                            Path.GetPathRoot(inp),
                            Path.GetDirectoryName(inp),
                            Path.GetFileNameWithoutExtension(inp) + ".fus");
                    }
                    if (Path.GetExtension(opts.Output).ToLower() != ".fus")
                    {
                        opts.Output += ".fus";
                    }
                    try
                    {
                        output = File.Open(opts.Output, FileMode.Create);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR creating output file '{opts.Output}':");
                        Console.Error.WriteLine(ex);
                        Environment.Exit((int)ErrorCode.OutputFile);
                    }

                    Console.WriteLine($"Converting from {opts.Input} to {Path.GetFileName(opts.Output)}");
                    var assimpScene = Assimp.ImportFileFromStream(input,
                        PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals |
                        PostProcessSteps.JoinIdenticalVertices, inputFormat);

                    SceneContainer fuseeScene = Assimp2Fusee.FuseefyScene(assimpScene);

                    var ser = new Serializer();
                    ser.Serialize(output, fuseeScene);
                    output.Flush();
                    output.Close();
                })

                // Called with the INPUTSCENEFORMATS verb
                .WithParsed<InputSceneFormats>(opts =>
                {
                    Console.WriteLine("Supported input formats for scene files:");
                    foreach (var inFormat in Assimp.GetSupportedImportFormats())
                    {
                        Console.Write(inFormat + "; ");
                    }
                    Console.WriteLine();
                })

                // Called with the PROTOSCHEMA verb
                .WithParsed<ProtoSchema>(opts =>
                {
                    var schema = ProtoBuf.Serializer.GetProto<SceneContainer>();

                    // Check and open output file
                    if (!String.IsNullOrEmpty(opts.Output))
                    {
                        if (Path.GetExtension(opts.Output).ToLower() != ".proto")
                        {
                            opts.Output += ".proto";
                        }
                        try
                        {
                            using (var output = new StreamWriter(File.Open(opts.Output, FileMode.Create)))
                            {
                                output.WriteLine("syntax = \"proto2\";");
                                output.Write(schema);
                            }
                            Console.Error.WriteLine(
                                $"SUCCESS: Protobuf schema for .fus files written to '{opts.Output}'");
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"ERROR creating output file '{opts.Output}':");
                            Console.Error.WriteLine(ex);
                            Environment.Exit((int)ErrorCode.OutputFile);
                        }
                    }
                    else
                    {
                        Console.WriteLine(schema);
                    }
                })

                // Called with the PUBLISH verb
                .WithParsed<Publish>(opts =>
                {
                // INPUT
                string input = opts.Input;
                string appName = null;
                string dllFilePath = null;
                string dllDirPath = null;
                string dir = null;
                // See if anything is specified at all
                if (string.IsNullOrEmpty(input) || Directory.Exists(input))
                {
                    dir = string.IsNullOrEmpty(input) ? Directory.GetCurrentDirectory() : Path.GetFullPath(input);
                    // See if we are inside a .Net Core project (is there a .csproj file)?
                    var csprojFile = Directory.EnumerateFiles(dir, "*.csproj").First();
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
                        catch (Exception ex)
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
                        catch (Exception ex)
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
                string output = opts.Output;
                string outPath = null;
                if (string.IsNullOrEmpty(output))
                {
                    outPath = Path.Combine(dir, "pub", opts.Platform.ToString());
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


                InitFuseeDirectories();

                string playerFile = null;
                string desktopPlayerDir = Path.GetFullPath(Path.Combine(fuseeBuildRoot, "Player", "Desktop")); // need this in web build as well.
                switch (opts.Platform)
                {
                    case Platform.Desktop:
                        playerFile = Path.GetFullPath(Path.Combine(desktopPlayerDir, "Fusee.Engine.Player.Desktop.exe"));
                        break;
                    case Platform.Web:
                        playerFile = Path.GetFullPath(Path.Combine(
                            fuseeBuildRoot, "Player", "Web", "Fusee.Engine.Player.Web.html"));
                        break;
                    default:
                        Console.Error.WriteLine($"ERROR: Platform {opts.Platform.ToString()} is currently not handled by fusee.");
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
                switch (opts.Platform)
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
                    // END Publish Desktop

                    case Platform.Web:
                        try
                        {
                            // Steps taken for Web publishing
                            // 1.Copy Original Compiled Web Player to Pub/Web
                            // 2.cd %FuseeRoot%bin/Debug/Player/Desktop   // to have reference assemblies at hand in JSILc
                            // 3. > PathToExt\JSILc.exe --nd
                            //        "CoreLibraryRoot>\bin\Debug\netstandard2.0\MyFuseeApp.dll"
                            //        - o "<Assembled>Assets/Scripts"
                            //
                            //     ->Will generate "MyFuseeAppBlaBla.js" and MyFuseeApp.dll.manifest.js
                            //
                            //4.Match $asmNN variables in MyFuseeApp.manifest.js and Fusee.Engine.Player.Web.exe.manifest.js. If 
                            //  $asmNN from MyFuseApp are not referenced in WebPlayer, assign new $asmMMs to them. This will automatically
                            //  handle the next point
                            //5.In Fusee.Engine.Player.Web.exe.manifest: 
                            //	Add MyFuseeAppBlabla as last entry of $asmNN
                            //    Add["Script", "MyFuseeApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null.js", { "sizeBytes": xxx }]
                            //6.In the newly generated MyFuseeAppBlaBla.js, replace all asmNN with NNs in MyFuseeApp.dll.manifest.js
                            //  with their respective MMs from Fusee.Engine.Player.Web.exe.manifest.
                            //  then replace tututu to asm.
                            //7.In Fusee.Engine.Player.Web...js, change 
                            //    $T0A = JSIL.Memoize($asm06.Fusee.Engine.Player.Core.Player)   to
                            //	$T0A = JSIL.Memoize($asm45.FuseeApp.MyFuseeApp)
                            //8.Copy MyFuseeApp's assets to the Pub/Web and insert entries for each of them into.contentproj file (similar to fuConv...)
                            //9.Rename the Player's main .html file to MyFuseeApp.html

                            // Call JSILc on the App DLL with cwd set to %FuseeRoot%bin/Debug/Player/Desktop to have reference assemblies at hand in JS
                            string jsilc = Path.GetFullPath(Path.Combine(fuseeRoot, "ext", "JSIL", "Compiler", "JSILc.exe"));
                            string temp = Path.Combine(outPath, "tmp");
                            Directory.CreateDirectory(temp);
                            using (Process cmd = Process.Start(new ProcessStartInfo
                            {
                                FileName = jsilc,
                                Arguments = $"--nodeps {dllFilePath} -o {temp}",
                                UseShellExecute = false,
                                WorkingDirectory = desktopPlayerDir,
                            }))
                            {
                                cmd.WaitForExit();
                            }

                            // Open Fusee.Engine.Player.Web.exe.manifest.js and the manifest.js of the just compiled output (in tmp)
                            string playerManifest = Path.Combine(outPath, "Assets", "Scripts", "Fusee.Engine.Player.Web.exe.manifest.js");
                            string appManifest = Path.Combine(temp, $"{appName}.dll.manifest.js");

                            string playerManifestContents = File.ReadAllText(playerManifest); 
                            var playerDict = ReadAssemblyDict(playerManifestContents, out int maxPlayerRef);
                            string appManifestContents = File.ReadAllText(appManifest);
                            var appDict = ReadAssemblyDict(appManifestContents, out int maxAppRef);
                            var appSizes = ReadAssemblySizes(appManifestContents);

                            // Build XLation table: $asm<KEY> needs to be replaced by $asm<VALUE> in all js files below tmp
                            var xlation = new Dictionary<int, int>();
                            var newRefDict = new Dictionary<string, int>();
                            int nextNewRef = maxPlayerRef + 1;
                            int appRefNumber = -1;
                            foreach (var refApp in appDict)
                            {
                                // See if the app's reference is already present in the original player's reference
                                if (playerDict.TryGetValue(refApp.Key, out int asmRef))
                                {
                                    // Yes, the orignal player's references already contains the current app's reference
                                    // Put an entry into the translation table
                                    xlation.Add(refApp.Value, asmRef);
                                }
                                else
                                {
                                    // No, this $asm needs to be added to the original player manifest. Note that for later.
                                    xlation.Add(refApp.Value, nextNewRef);
                                    newRefDict[refApp.Key] = nextNewRef;

                                    // See if this is the $asm reference to the main app module and store its asm number for later use
                                    if (refApp.Key.Contains(appName))
                                        appRefNumber = nextNewRef;

                                    nextNewRef++;
                                }
                            }

                            if (appRefNumber < 0)
                            {
                                Console.Error.WriteLine($"ERROR: Could not find {appName} generated java script file");
                                Environment.Exit((int)ErrorCode.InternalError);
                            }

                            // Do a GREP on the contents of tmp using the xlation table
                            foreach (string filename in Directory.EnumerateFiles(temp, "*.js"))
                            {
                                string contents = File.ReadAllText(filename);
                                string replacement = ReplaceAsmRefs(contents, xlation);
                                File.WriteAllText(filename, replacement);
                            }

                            // Add the missing references to the player's manifest
                            StringBuilder missingAsmDecls = new StringBuilder();
                            StringBuilder missingAsmRefs = new StringBuilder();
                            foreach (var missingRef in newRefDict)
                            {
                                missingAsmDecls.AppendLine($"var $asm{missingRef.Value.ToString("X2")} = JSIL.GetAssembly(\"{missingRef.Key}\");");
                                missingAsmRefs.AppendLine($"    [\"Script\", \"{missingRef.Key}.js\", {{ \"sizeBytes\": {appSizes[missingRef.Key]} }}],");
                            }
                            Match m = Regex.Match(playerManifestContents, @"if \(typeof \(contentManifest\)");
                            playerManifestContents = playerManifestContents.Insert(m.Index - 1, missingAsmDecls.ToString());

                            m = Regex.Match(playerManifestContents, @"    \[""Script""");
                            playerManifestContents = playerManifestContents.Insert(m.Index - 1, missingAsmRefs.ToString());

                            File.WriteAllText(playerManifest, playerManifestContents);

                            string assetDstDir = Path.Combine(outPath, "Assets");
                            string scriptsDstDir = Path.Combine(assetDstDir, "Scripts");

                            // Fiddle in the instantiation of the app
                            string mainExeJsFile = Directory.EnumerateFiles(scriptsDstDir, "Fusee.Engine.Player.Web, Version*.js").First();
                            string mainExeJsContents = File.ReadAllText(mainExeJsFile);
                            mainExeJsContents = Regex.Replace(mainExeJsContents, @"\$asm(..)\.Fusee\.Engine\.Player\.Core\.Player", $"$asm{appRefNumber.ToString("X2")}.{tApp.Namespace}.{tApp.Name}");
                            File.WriteAllText(mainExeJsFile, mainExeJsContents);

                            // Copy app.js from tmp to outdir
                            FileTools.DirectoryCopy(temp, scriptsDstDir, true, true);

                            // Copy the app's assets
                            string assetSrcDir = Path.Combine(dllDirPath, "Assets");
                            if (Directory.Exists(assetSrcDir))
                                FileTools.DirectoryCopy(assetSrcDir, assetDstDir, true, true);

                            // add app's assets to the listing in player's contentproj.manifest
                            AssetManifest.AdjustAssetManifest(dllDirPath, outPath, Path.Combine(scriptsDstDir, "Fusee.Engine.Player.Web.contentproj.manifest.js"));

                            // Rename main HTML file to <app>.html
                            File.Move(Path.Combine(outPath, "Fusee.Engine.Player.Web.html"), Path.Combine(outPath, $"{appName}.html"));

                            // Remove tmp
                            Directory.Delete(temp, true);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine("ERROR: internal error while publishing FUSEE Web App:\n" + ex);
                            Environment.Exit((int)ErrorCode.InternalError);
                        }
                        Console.Error.WriteLine($"SUCCESS: FUSEE Web App {appName}.html generated at {outPath}.");
                        Environment.Exit(0);
                        break;
                        // END Publish Web

                    }
                })

                // Called with the SERVER verb
                .WithParsed<Server>(opts =>
                {
                    string wwwRoot = null;
                    string htmlFile = null;

                    // If no root is given assume the current working directory
                    if (string.IsNullOrEmpty(opts.Root))
                    {
                        opts.Root = Directory.GetCurrentDirectory();
                    }

                    // See if a file or a directory is specified
                    if (File.Exists(opts.Root))
                    {
                        htmlFile = Path.GetFileName(opts.Root);
                        wwwRoot = Path.Combine(Path.GetPathRoot(opts.Root), Path.GetDirectoryName(opts.Root));
                    }
                    else
                    {
                        wwwRoot = opts.Root;
                    }

                    if (!Directory.Exists(wwwRoot))
                    {
                        Console.Error.WriteLine($"ERROR: Root directory {wwwRoot} not present or not accessible.");
                        Environment.Exit((int)ErrorCode.InputFile);
                    }

                    // If no file is specified, try to find index.htm[l], default.htm[l] or any other html
                    if (string.IsNullOrEmpty(htmlFile))
                    {
                        string[] htmlFiles = Directory.GetFiles(wwwRoot, "*.htm?", SearchOption.TopDirectoryOnly);
                        htmlFile = htmlFiles.FirstOrDefault(s => s.ToLower().Contains("index")) ?? htmlFiles.FirstOrDefault(s => s.ToLower().Contains("default")) ?? htmlFiles.FirstOrDefault();
                        if (string.IsNullOrEmpty(htmlFile))
                        {
                            opts.Serveronly = true;
                        }
                        else
                        {
                            htmlFile = Path.GetFileName(htmlFile);
                        }
                    }

                    // Fire up the http server
                    try
                    {
                        if (_httpThread != null)
                        {
                            _httpThread.Abort();
                            _httpServer = null;
                        }

                        _httpServer = new FuseeHttpServer(wwwRoot, opts.Port);
                        _httpThread = new Thread(_httpServer.listen);
                        _httpThread.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR: starting local HTTP server at {wwwRoot} on port {opts.Port}.\n{ex}");
                        Environment.Exit((int)ErrorCode.InternalError);
                    }
                    Console.Error.WriteLine($"SUCCESS: Local HTTP server running at {wwwRoot} on port {opts.Port}.");

                    if (!opts.Serveronly)
                        Process.Start($"http://localhost:{opts.Port}/" + Path.GetFileName(htmlFile));
                    
                    // Environment.Exit(0);
                })


                // Called with the INSTALL verb
                .WithParsed<Install>(opts =>
                {
                    // Find FuseeRoot from this assembly
                    string fuseeCmdLineRoot = null;
                    string fuseeRoot = null;
                    try
                    {
                        fuseeCmdLineRoot = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                        fuseeRoot = Path.GetFullPath(Path.Combine(fuseeCmdLineRoot, "..", "..", "..")); // three hops from %FuseeRoot%bin/[Debug|Release]/Tools down to the root.
                        fuseeCmdLineRoot = FileTools.PathAddTrailingSeperator(fuseeCmdLineRoot);
                        fuseeRoot = FileTools.PathAddTrailingSeperator(fuseeRoot);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR: this instance of fusee.exe at {fuseeCmdLineRoot} doesn't seem to be part of a FUSEE installation.\n{ex}");
                        Environment.Exit((int)ErrorCode.InternalError);
                    }
                    string templateDir = Path.Combine(fuseeRoot, "dis", "DnTemplate"); ;

                    // Install or uninstall ? 
                    if (!opts.Uninstall)
                    {
                        // INSTALL
                        // Set the FuseeRoot Environment variable
                        try
                        {
                            Environment.SetEnvironmentVariable("FuseeRoot", fuseeRoot, (opts.InstType == InstallationType.User) ? EnvironmentVariableTarget.User : EnvironmentVariableTarget.Machine);
                        }
                        catch (SecurityException ex)
                        {
                            Console.Error.WriteLine($"ERROR: Insufficient privileges to set the \"FuseeRoot\" environment variable. Run the shell as Administrator before calling fusee.exe\n");
                            Environment.Exit((int)ErrorCode.InsufficentPrivileges);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"ERROR: Unable to set the \"FuseeRoot\" environment variable.\n{ex}");
                            Environment.Exit((int)ErrorCode.InternalError);
                        }

                        // Add us to the PATH so fusee.exe can be called from anywhere. Be careful with the PATH. Check if fusee is already registered there.
                        try
                        {
                            var target = (opts.InstType == InstallationType.User) ? EnvironmentVariableTarget.User : EnvironmentVariableTarget.Machine;
                            string pathVariable = Environment.GetEnvironmentVariable("PATH", target);

                            int alreadyRegistered = 0;
                            var pathContents = pathVariable.Split(new char[] { ';' });
                            List<string> fuseePaths = new List<string>();
                            foreach (var path in pathContents)
                            {
                                var pathProcessed = FileTools.PathAddTrailingSeperator(path);
                                if (pathProcessed == fuseeCmdLineRoot)
                                    alreadyRegistered++;
                                else if (pathProcessed.ToLower().Contains("fusee"))
                                    fuseePaths.Add(pathProcessed);
                            }

                            if (fuseePaths.Count > 0)
                            {
                                Console.Error.WriteLine($"WARNING: the \"PATH\" Variable already references the following FUSEE instances:");
                                foreach (var fuseePath in fuseePaths)
                                {
                                    Console.Error.WriteLine($"\t{fuseePath}");
                                }
                                Console.Error.WriteLine("\tfusee.exe will NOT alter the \"PATH\" variable. Please check the contents of your \"PATH\" manually or de-install the other fusee instance(s) first.");
                            }
                            else if (alreadyRegistered >= 1)
                            {
                                if (alreadyRegistered == 1)
                                    Console.Error.WriteLine($"The \"PATH\" Variable already contains this FUSEE instance. \"PATH\" will not be altered\n");
                                else
                                    Console.Error.WriteLine($"WARNING: The \"PATH\" Variable already contains this FUSEE instance MULTIPLE TIMES. Please check the contents of your \"PATH\" manually. \"PATH\" will not be altered\n");
                            }
                            else
                            {
                                pathVariable += $";{fuseeCmdLineRoot}";
                                Environment.SetEnvironmentVariable("PATH", pathVariable, target);
                            }
                        }
                        catch (SecurityException ex)
                        {
                            Console.Error.WriteLine($"ERROR: Insufficient privileges to alter the \"PATH\" environment variable. Run the shell as Administrator before calling fusee.exe\n");
                            Environment.Exit((int)ErrorCode.InsufficentPrivileges);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"ERROR: Unable to set the \"PATH\" environment variable.\n{ex}");
                            Environment.Exit((int)ErrorCode.InternalError);
                        }

                        // Install the dotnet new fusee template
                        try
                        {
                            using (Process cmd = Process.Start(new ProcessStartInfo
                            {
                                FileName = "dotnet",
                                Arguments = $"new --install {templateDir}",
                                UseShellExecute = false,
                                WorkingDirectory = Directory.GetCurrentDirectory(),
                            }))
                            {
                                cmd.WaitForExit();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"ERROR: Unable to install the dotnet new fusee template from {templateDir}.\n{ex}");
                            Environment.Exit((int)ErrorCode.InternalError);
                        }

                        // Install the Blender AddOn
                        try
                        {
                            string blenderAddOnDir = GetBlenderAddOnDir(opts.InstType);
                        }
                    }
                    else
                    {
                        // UNINSTALL
                        // Uninstall the dotnet new fusee template
                        try
                        {
                            using (Process cmd = Process.Start(new ProcessStartInfo
                            {
                                FileName = "dotnet",
                                Arguments = $"new --uninstall {templateDir}",
                                UseShellExecute = false,
                                WorkingDirectory = Directory.GetCurrentDirectory(),
                            }))
                            {
                                cmd.WaitForExit();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"ERROR: Unable to uninstall the dotnet new fusee template at {templateDir}.\n{ex}");
                            Environment.Exit((int)ErrorCode.InternalError);
                        }

                        // Remove us from the PATH. Be careful with the PATH. 
                        try
                        {
                            var target = (opts.InstType == InstallationType.User) ? EnvironmentVariableTarget.User : EnvironmentVariableTarget.Machine;
                            string pathVariable = Environment.GetEnvironmentVariable("PATH", target);

                            int alreadyRegistered = 0;
                            var pathContents = pathVariable.Split(new char[] { ';' });
                            List<string> remainingPaths = new List<string>();
                            List<string> fuseePaths = new List<string>();
                            foreach (var path in pathContents)
                            {
                                var pathProcessed = FileTools.PathAddTrailingSeperator(path);
                                if (pathProcessed == fuseeCmdLineRoot)
                                    alreadyRegistered++;
                                else
                                {
                                    if (pathProcessed.ToLower().Contains("fusee"))
                                        fuseePaths.Add(pathProcessed);

                                    remainingPaths.Add(path);
                                }
                            }

                            if (fuseePaths.Count > 0)
                            {
                                Console.Error.WriteLine($"WARNING: the \"PATH\" Variable references the following FUSEE instances:");
                                foreach (var fuseePath in fuseePaths)
                                {
                                    Console.Error.WriteLine($"\t{fuseePath}");
                                }
                                Console.Error.WriteLine("\tfusee.exe will NOT remove these additional FUSEE references from the \"PATH\" variable. Please check the contents of your \"PATH\" manually or de-install the other fusee instances.");
                            }

                            if (alreadyRegistered == 0)
                            {
                                Console.Error.WriteLine($"WARNING: The \"PATH\" Variable does not contain this FUSEE instance. \"PATH\" will not be altered\n");
                            }
                            else
                            {
                                Console.Error.WriteLine($"Removing this FUSEE instance from the \"PATH\" variable");
                                var newPathVariable = "";
                                for (int i = 0; ; i++)
                                {
                                    newPathVariable += remainingPaths[i];
                                    if (i == remainingPaths.Count - 1)
                                        break;
                                    newPathVariable += ";";
                                }
                                Environment.SetEnvironmentVariable("PATH", newPathVariable, target);
                            }
                        }
                        catch (SecurityException ex)
                        {
                            Console.Error.WriteLine($"ERROR: Insufficient privileges to alter the \"PATH\" environment variable. Run the shell as Administrator before calling fusee.exe\n");
                            Environment.Exit((int)ErrorCode.InsufficentPrivileges);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"ERROR: Unable to alter the \"PATH\" environment variable.\n{ex}");
                            Environment.Exit((int)ErrorCode.InternalError);
                        }


                        // Remove the FuseeRoot variable
                        try
                        {
                            Environment.SetEnvironmentVariable("FuseeRoot", null, (opts.InstType == InstallationType.User) ? EnvironmentVariableTarget.User : EnvironmentVariableTarget.Machine);
                        }
                        catch (SecurityException ex)
                        {
                            Console.Error.WriteLine($"ERROR: Insufficient privileges to delete the \"FuseeRoot\" environment variable. Run the shell as Administrator before calling fusee.exe\n");
                            Environment.Exit((int)ErrorCode.InsufficentPrivileges);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"ERROR: Unable to delete the \"FuseeRoot\" environment variable.\n{ex}");
                            Environment.Exit((int)ErrorCode.InternalError);
                        }

                    }
                })

                // Called with the GENERATE verb (fuGen legacy code)
                .WithParsed<Generate>(opts =>
                {
                    var targDir = opts.TargDir;
                    var targWeb = opts.TargWeb;
                    var targApp = opts.TargAppPath;
                    var externalFiles = opts.ExternalFiles.Split(';');

                    string fileName = Path.GetFileNameWithoutExtension(targApp);

                    // Create directories
                    if (!Directory.Exists(Path.Combine(targWeb, "Assets")))
                        Directory.CreateDirectory(Path.Combine(targWeb, "Assets"));

                    if (!Directory.Exists(Path.Combine(targWeb, "Assets", "Scripts")))
                        Directory.CreateDirectory(Path.Combine(targWeb, "Assets", "Scripts"));

                    if (!Directory.Exists(Path.Combine(targWeb, "Assets", "Styles")))
                        Directory.CreateDirectory(Path.Combine(targWeb, "Assets", "Styles"));

                    if (!Directory.Exists(Path.Combine(targWeb, "Assets", "Config")))
                        Directory.CreateDirectory(Path.Combine(targWeb, "Assets", "Config"));
                    
                    // Does HTML already exists?
                    var newHTML = !File.Exists(targWeb + fileName + ".html");

                    Console.Error.WriteLine(newHTML
                        ? "No HTML file found - generating a simple HTML file"
                        : "HTML file already exists - delete it to create a new one");

                    // Collecting all files
                    var customManifest = Directory.Exists(Path.Combine(targDir, "Assets"));
                    var customCSS = "";

                    Console.Error.WriteLine(customManifest
                        ? "Found an Assets folder - collecting all and write manifest"
                        : "No Assets folder - no additional files will be added");

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
                            Console.Error.WriteLine("ERROR Couldn't find " + extFile + ".js");
                            Environment.Exit((int)ErrorCode.InputFile);
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

                            var srcAssetFolder = FileTools.PathAddTrailingSeperator(Path.Combine(targDir, "Assets"));
                            var srcAssetDirPath = FileTools.PathAddTrailingSeperator(Path.GetDirectoryName(filePath));

                            var srcRelativeToAssetsDir = FileTools.MakeRelativePath(srcAssetFolder, srcAssetDirPath);
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
                    var fileNamesList = new List<string>();
                    var fileSizeList = new List<long>();
                    var fileTypesList = new List<string>();
                    var fileFormatsList = new List<string>();

                    AssetManifest.GenerateAssetManifestEntryItems(filePaths, destRelativePaths, fileCount, fileNamesList, fileSizeList, fileTypesList, fileFormatsList);
                    var manifest = new ManifestFile("Fusee.Engine.Player.Web", fileNamesList, fileSizeList, fileTypesList, fileFormatsList);
                    string manifestContent = manifest.TransformText();

                    File.WriteAllText(Path.Combine(targWeb, "Assets", "Scripts", fileName + ".contentproj.manifest.js"),
                        manifestContent);

                    // Create HTML file
                    if (newHTML)
                    {
                        Console.WriteLine(customCSS == ""
                            ? "No additional .css file found in Assets folder - using only default one"
                            : "Found an additional .css file in Assets folder - adding to HTML file");

                        var page = new WebPage(targApp, customCSS);
                        string pageContent = page.TransformText();

                        File.WriteAllText(Path.Combine(targWeb, fileName + ".html"), pageContent);
                    }

                    // Create config file
                    var customConf = File.Exists(Path.Combine(targDir, "Assets", "fusee_config.xml"));

                    Console.WriteLine(!customConf
                        ? "No custom config file ('fusee_config.xml') found in Assets folder - using default settings"
                        : "Found an custom config file in Assets folder - applying settings to webbuild");

                    var conf = new JsilConfig(targApp, targDir, customConf);
                    string confContent = conf.TransformText();

                    File.WriteAllText(Path.Combine(targWeb, "Assets", "Config", "jsil_config.js"), confContent);

                    // Done
                    Console.Error.WriteLine($"SUCCESS: Generated Web Build at {targWeb}.");
                    Environment.Exit(0);
                })

                // Called with the WEB verb (fuConv legacy code)
                .WithParsed<WebViewer>(opts =>
                {
                    List<string> textureFiles = new List<string>();
                    try
                    {
                        // Get list of paths to texturefiles
                        string fileList = opts.List;
                        Console.WriteLine($"FILELIST: {opts.List}");
                        textureFiles = fileList.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        Console.WriteLine($"FILES: {textureFiles}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"No texture filepaths set");
                    }

                    string htmlFileDir = opts.Output;

                    InitFuseeDirectories();
                    string fuseePlayerDir = Path.Combine(fuseeBuildRoot, "Player", "Web");

                    Stream input = null, output = null;
                    string sceneFileDir = Path.Combine(htmlFileDir, "Assets");
                    if (File.Exists(sceneFileDir))
                    {
                        File.Delete(sceneFileDir);
                    }
                    string sceneFilePath = Path.Combine(sceneFileDir, "Model.fus");
                    // string origHtmlFilePath = Path.Combine(htmlFileDir, "Fusee.Engine.SceneViewer.Web.html");
                    string origHtmlFilePath = Path.Combine(htmlFileDir, "Fusee.Engine.Player.Web.html");
                    if (File.Exists(origHtmlFilePath))
                        File.Delete(origHtmlFilePath);
                    string targetHtmlFilePath =
                        Path.Combine(htmlFileDir, Path.GetFileNameWithoutExtension(opts.Input) + ".html");
                    if (File.Exists(targetHtmlFilePath))
                        File.Delete(targetHtmlFilePath);

                    //Copy
                    FileTools.DirectoryCopy(fuseePlayerDir, htmlFileDir, true, true);
                    File.Move(origHtmlFilePath, targetHtmlFilePath);

                    // Rename 

                    // Check and open input file
                    string inputFormat = Path.GetExtension(opts.Input).ToLower();
                    if (!String.Equals(inputFormat, ".fus"))
                    {
                        Console.Error.WriteLine($"Input ('{opts.Input}') is not a .fus file.");
                        Environment.Exit((int)ErrorCode.InputFile);
                    }

                    try
                    {
                        input = File.Open(opts.Input, FileMode.Open);
                        input.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR opening input file '{opts.Input}':");
                        Console.Error.WriteLine(ex);
                        Environment.Exit((int)ErrorCode.InputFile);
                    }

                    // Check and open output file
                    if (String.IsNullOrEmpty(opts.Output))
                    {
                        Console.Error.WriteLine("No outputpath specified");
                        Environment.Exit((int)ErrorCode.OutputFile);
                    }
                    try
                    {
                        if (File.Exists(sceneFilePath))
                            File.Delete(sceneFilePath);
                        File.Move(opts.Input, sceneFilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR creating output file '{sceneFilePath}':");
                        Console.Error.WriteLine(ex);
                        Environment.Exit((int)ErrorCode.OutputFile);
                    }

                    //Manifest File + Textures
                    Console.WriteLine($"Moving File from {opts.Input} to {sceneFilePath}");
                    for (int i = 0; i < textureFiles.Count; i++)
                    {
                        string textureFile = Path.GetFileName(textureFiles[i]);
                        string texturePath = Path.Combine(sceneFileDir, textureFile);
                        if (!File.Exists(texturePath))
                        {
                            File.Move(textureFiles[i], texturePath);
                        }
                        textureFiles[i] = Path.Combine("Assets", textureFile);
                        Console.WriteLine($"TEXTUREFILES {textureFiles[i]}");
                    }
                    if (textureFiles != null)
                        AssetManifest.CreateAssetManifest(htmlFileDir, textureFiles);

                    //WebServer
                    if (_httpServer == null)
                    {
                        _httpServer = new FuseeHttpServer(htmlFileDir, 4655); // HEX: FU
                        Thread thread = new Thread(_httpServer.listen);
                        thread.Start();
                    }
                    else
                    {
                        _httpServer.HtDocsRoot = htmlFileDir;
                    }
                    Console.WriteLine($"Server running");
                    Process.Start("http://localhost:4655/" + Path.GetFileName(targetHtmlFilePath));
                })

                // ERROR on the command line
                .WithNotParsed(errs =>
                {
                    /*foreach (var error in errs)
                    {
                        Console.Error.WriteLine(error);
                    }
                    */
                    Environment.Exit((int)ErrorCode.CommandLineSyntax);
                });
        }

        private static string GetBlenderAddOnDir(InstallationType instType)
        {
            switch (instType)
            {
                case InstallationType.Machine:

                    break;

                case InstallationType.User:
                    break;

                default:
                    throw new ArgumentException("Unknown installation type.");
            }
        }

        private static Dictionary<string, int> ReadAssemblySizes(string fileContents)
        {
            var dict = new Dictionary<string, int>();
            Regex r = new Regex(@"\[""Script""\s*,\s*""(.*)\.js"",\s*\{\s*""sizeBytes""\s*:\s*(\d+)\s*\}\]");
            for (var m = r.Match(fileContents); m.Success; m = m.NextMatch())
            {
                int curAsmInx = int.Parse(m.Groups[2].ToString());
                dict[m.Groups[1].ToString()] = curAsmInx;
            }

            return dict;
        }

        private static string ReplaceAsmRefs(string contents, Dictionary<int, int> xlation)
        {
            Regex r = new Regex(@"\$asm(..)");
            string ret = r.Replace(contents, m =>
            {
                int curAsmInx = int.Parse(m.Groups[1].ToString(), NumberStyles.HexNumber);
                return $"$asm{xlation[curAsmInx].ToString("X2")}";
            });
            return ret;
        }

        static Dictionary<string, int> ReadAssemblyDict(string fileContents, out int maxAsmInx)
        {
            var dict = new Dictionary<string, int>();
            maxAsmInx = 0;
            Regex r = new Regex(@"var \$asm(..)\s*=\s*JSIL.GetAssembly\(""(.*)""\);");
            for (var m = r.Match(fileContents); m.Success; m = m.NextMatch())
            {
                int curAsmInx = int.Parse(m.Groups[1].ToString(), NumberStyles.HexNumber);
                if (curAsmInx > maxAsmInx)
                    maxAsmInx = curAsmInx;
                // dict[$"asm{m.Groups[1].ToString()}"] = m.Groups[2].ToString();
                dict[m.Groups[2].ToString()] = curAsmInx;
            }

            return dict;
        }
    }
}

