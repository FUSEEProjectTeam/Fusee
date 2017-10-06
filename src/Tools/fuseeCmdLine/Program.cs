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

namespace Fusee.Tools.fuseeCmdLine
{
    enum ErrorCode : int
    {
        CommandLineSyntax = -1,
        InputFile = -2,
        InputFormat = -3,
        OutputFile = -4,
        PlatformNotHandled = -5,

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
                HelpText = "Platform the deployment packages is meant to run on. Possible values are: Desktop, Web or Android")]
            public Platform Platform { get; set; }
        }

        [Verb("server", HelpText = "Launch a minimalistic local webserver and start the default browser")]
        public class Server
        {
            [Value(0, HelpText = "Directory or File path. to be used as WWWRoot. If a file name is given, it will be started in the browser. If not, a default start file will be looked for.", MetaName = "Root")]
            public string Root { get; set; }

            [Option('p', "port", Default = 4655, // HEX FU,
                HelpText = "Port the server should start running on")]
            public int Port { get; set; }

            [Option('s', "serveronly", Default = false, HelpText ="Launches the server but does not start the default browser.")]
            public bool Serveronly { get; set; }
        }


        [Verb("web", HelpText = "Use an existing .fus-file to start a webserver.")]
        public class WebViewer
        {
            [Value(0, HelpText = "Input .fus-file.", MetaName = "Input", Required = true)]
            public string Input { get; set; }

            [Option('o', "output", HelpText = "Target Directoy")]
            public string Output { get; set; }

            [Option('l', "list", HelpText = "List of paths to texture files")]
            public string List { get; set; }
        }


        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<SceneOptions, InputSceneFormats, ProtoSchema, WebViewer, Publish, Server>(args)

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
                                output.Write(schema);
                            Console.Error.WriteLine(
                                $"Protobuf schema for .fus files successfully written to '{opts.Output}'");
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


                // Check if player is present
                string fuseeCmdLineRoot = null;
                string fuseeRoot = null;
                try
                {
                    fuseeCmdLineRoot = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    fuseeRoot = Path.GetFullPath(Path.Combine(fuseeCmdLineRoot, "..", "..", "..")); // three hops from %FuseeRoot%bin/[Debug|Release]/Tools down to the root.
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"ERROR: this instance of fusee.exe at {fuseeCmdLineRoot} doesn't seem to be part of a FUSEE installation.\n{ex}");
                    Environment.Exit((int)ErrorCode.InternalError);
                }

                string playerFile = null;
                string desktopPlayerDir = Path.GetFullPath(Path.Combine(fuseeCmdLineRoot, "Player", "Desktop")); // need this in web build as well.
                switch (opts.Platform)
                {
                    case Platform.Desktop:
                        playerFile = Path.GetFullPath(Path.Combine(desktopPlayerDir, "Fusee.Engine.Player.Desktop.exe"));
                        break;
                    case Platform.Web:
                        playerFile = Path.GetFullPath(Path.Combine(
                            fuseeCmdLineRoot, "Player", "Web", "Fusee.Engine.Player.Web.html"));
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
                DirCopy.DirectoryCopy(playerDir, outPath, true, true);

                // Do platform dependent stuff to integrate the FUSEE app DLL into the player
                switch (opts.Platform)
                {
                    case Platform.Desktop:
                        try
                        {
                            // Copy the FUSEE App on top of the player.
                            DirCopy.DirectoryCopy(dllDirPath, outPath, true, true);

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
                            // Call JSILc on the App DLL with cwd set to %FuseeRoot%bin/Debug/Player/Desktop to have reference assemblies at hand in JS
                            string jsilc = Path.GetFullPath(Path.Combine(fuseeRoot, "ext", "JSIL", "Compiler", "JSILc.exe"));
                            string temp = Path.Combine(outPath, "tmp");
                            Directory.CreateDirectory(temp);
                            string jsilargs = $"--nodeps {dllFilePath} -o {temp}";
                            ProcessStartInfo cmdsi = new ProcessStartInfo(jsilc, jsilargs)
                            {
                                UseShellExecute = false,
                                WorkingDirectory = desktopPlayerDir,
                            };
                            Process cmd = Process.Start(cmdsi);
                            cmd.WaitForExit();

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
                                // See if the app's reference is already present in the original players reference
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
                            DirCopy.DirectoryCopy(temp, scriptsDstDir, true, true);

                            // Copy the app's assets
                            string assetSrcDir = Path.Combine(dllDirPath, "Assets");
                            if (Directory.Exists(assetSrcDir))
                                DirCopy.DirectoryCopy(assetSrcDir, assetDstDir, true, true);

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

                // Called with the WEB verb
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
                    string thisPath = Assembly.GetExecutingAssembly().Location;
                    thisPath = thisPath.Remove(thisPath.LastIndexOf(Path.DirectorySeparatorChar));
                    string fuseePlayerDir = Path.Combine(thisPath, "Viewer");
                    Stream input = null, output = null;
                    string sceneFileDir = Path.Combine(htmlFileDir, "Assets");
                    if (File.Exists(sceneFileDir))
                    {
                        File.Delete(sceneFileDir);
                    }
                    string sceneFilePath = Path.Combine(sceneFileDir, "Model.fus");
                    string origHtmlFilePath = Path.Combine(htmlFileDir, "Fusee.Engine.SceneViewer.Web.html");
                    if (File.Exists(origHtmlFilePath))
                        File.Delete(origHtmlFilePath);
                    string targetHtmlFilePath =
                        Path.Combine(htmlFileDir, Path.GetFileNameWithoutExtension(opts.Input) + ".html");
                    if (File.Exists(targetHtmlFilePath))
                        File.Delete(targetHtmlFilePath);

                    //Copy
                    DirCopy.DirectoryCopy(fuseePlayerDir, htmlFileDir, true, true);
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

