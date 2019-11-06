using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using CommandLine;
using Fusee.Serialization;
using Fusee.Engine.Core;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;
using Fusee.Base.Core;
using Fusee.Base.Common;
using Path = System.IO.Path;
using Fusee.Base.Imp.Desktop;
using System.IO.Compression;
using System.Net;

namespace Fusee.Tools.fuseeCmdLine
{
    enum ErrorCode : int
    {
        Success,
        CommandLineSyntax = -1,
        InputFile = -2,
        InputFormat = -3,
        OutputFile = -4,
        PlatformNotHandled = -5,
        InsufficentPrivileges = -6,
        CouldNotDownloadInputFile = -7,

        InternalError = -42,
    }

    class Program
    {
        private static FuseeHttpServer _httpServer;
        private static Thread _httpThread;

        [Verb("protoschema", HelpText = "Output the protobuf schema for the .fus file format.")]
        public class ProtoSchema
        {
            [Option('o', "output",
                HelpText = "Path of .proto file to be written. \".proto\" extension will be added if not present.")]
            public string Output { get; set; }
        }

        [Verb("player", HelpText = "Output the protobuf schema for the .fus file format.")]
        public class Player
        {
            [Value(0, HelpText = "Path or url to .fus/.fuz file or Fusee-App .dll.", MetaName = "Input", Required = false)]
            public string Input { get; set; }
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

        [Verb("install", HelpText = "Install the blender add-on. The -u option undoes the respective step.")]
        public class Install
        {
            [Option('t', "type", Default = InstallationType.User, HelpText = "Machine-wide or per-user installation. '-t User' will set \"FuseeRoot\" and \"PATH\" for the current user only and will install the Blender Add-on below <user>/Appdata/Roaming/Blender Foundation. '-t Machine' will set \"FuseeRoot\" and \"PATH\" for all users and install the Blender Add-on below 'Program Files/Blender Foundation'. Start shell (cmd or powershell) as Administrator for machine-wide installation")]
            public InstallationType InstType { get; set; }

            [Option('u', "uninstall", Default = false, HelpText = "De-Register this FUSEE installation. This will only de-register this FUSEE instance (deregister the 'dotnet new fusee' template, remove fusee.exe from the \"PATH\" and remove the \"FuseeRoot\" environment varable). This will NOT delete the contents of the installation folder.")]
            public bool Uninstall { get; set; }

            [Option('b', "blender", Default = false, HelpText = "Only install/uninstall the Blender FUSEE Add-on.")]
            public bool Blender { get; set; }

            [Option('i', "blenderdir", HelpText = "Manually set the directory where to (un/)install the Blender FUSEE Add-on to/from. If not set, fusee.exe tries to find an appropriate Add-on directory based on the installation type (option '--type' per-user or machine-wide).")]
            public string BlenderDir { get; set; }

        }


        [Verb("web", HelpText = "Use an existing .fus-file to start a webserver. Deprectated. Currently used in the Blender FUSEE Add-on. To be replaced by the publish verb.")]
        public class WebViewer
        {
            [Value(0, HelpText = "Input .fus-file.", MetaName = "Input", Required = true)]
            public string Input { get; set; }

            [Option('o', "output", HelpText = "Target Directoy")]
            public string Output { get; set; }

            [Option('l', "list", HelpText = "List of paths to texture files")]
            public string List { get; set; }
        }

        [Verb("pack", HelpText = "Packs a compiled Fusee-App into one .fuz-File.")]
        public class Pack
        {
            [Value(0, HelpText = "Input Fusee-App .dll", MetaName = "Input", Required = true)]
            public string Input { get; set; }
        }

        // "Globals"
        static string fuseeCmdLineRoot = null;
        static string fuseeRoot = null;
        static string fuseeBuildRoot = null;
        static string fuseeConfiguration = null;

        static void InitFuseeDirectories()
        {
            // Check if player is present
            try
            {
                fuseeCmdLineRoot = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                fuseeBuildRoot = Path.GetFullPath(Path.Combine(fuseeCmdLineRoot, ".."));        // one hop down to remove "Tools" from %FuseeRoot%/bin/[Debug|Release]/Tools.
                fuseeRoot = Path.GetFullPath(Path.Combine(fuseeCmdLineRoot, "..", "..", "..")); // three hops from %FuseeRoot%/bin/[Debug|Release]/Tools down to the root.
                fuseeCmdLineRoot = FileTools.PathAddTrailingSeperator(fuseeCmdLineRoot);
                fuseeRoot = FileTools.PathAddTrailingSeperator(fuseeRoot);
                fuseeBuildRoot = FileTools.PathAddTrailingSeperator(fuseeBuildRoot);
                if (fuseeCmdLineRoot.Contains("Debug"))
                {
                    fuseeConfiguration = "Debug";
                }
                else
                {
                    fuseeConfiguration = fuseeCmdLineRoot.Contains("Release") ? "Release" : "Unknown";
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ERROR: this instance of fusee.exe at {fuseeCmdLineRoot} doesn't seem to be part of a FUSEE installation.\n{ex}");
                Environment.Exit((int)ErrorCode.InternalError);
            }

        }

        [STAThread]
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Pack, Player, Publish, Server, Install, ProtoSchema, WebViewer>(args)
            #region Pack
                .WithParsed<Pack>(opts =>
                {
                    if (!string.IsNullOrWhiteSpace(opts.Input) && Path.GetExtension(opts.Input).ToLower().Equals(".dll"))
                    {
                        var path = Path.GetDirectoryName(opts.Input);
                        var appname = Path.GetFileNameWithoutExtension(opts.Input);

                        var zipfile = appname + ".fuz";

                        var temppath = Path.GetTempPath();

                        var zipfilepath = Path.Combine(path, zipfile);
                        var zipfiletemppath = Path.Combine(temppath, zipfile);

                        if (File.Exists(Path.Combine(zipfilepath)))
                            File.Delete(Path.Combine(zipfilepath));

                        if (File.Exists(Path.Combine(zipfiletemppath)))
                            File.Delete(Path.Combine(zipfiletemppath));

                        ZipFile.CreateFromDirectory(path, zipfiletemppath, CompressionLevel.Optimal, false);
                        File.Move(zipfiletemppath, zipfilepath);

                        Console.WriteLine("Packed to: " + zipfilepath);
                    }
                })
            #endregion

            #region Player
                // Called with the Player verb
                .WithParsed<Player>(opts =>
                {
                    var input = opts.Input;

                    if (Uri.IsWellFormedUriString(input, UriKind.Absolute))
                    {
                        var uri = new Uri(input);

                        if (uri.Scheme.Equals("fusee") || uri.Scheme.Equals("http") || uri.Scheme.Equals("https"))
                        {
                            var filename = Path.GetFileName(uri.LocalPath);

                            if (!string.IsNullOrWhiteSpace(filename))
                            {
                                var tempfilepath = Path.Combine(Path.GetTempPath(), filename);

                                if (File.Exists(tempfilepath))
                                    File.Delete(tempfilepath);

                                if (uri.Scheme.Equals("fusee"))
                                {
                                    string uriWithoutScheme = uri.Host + uri.PathAndQuery + uri.Fragment;
                                    bool status = false;
                                    Console.WriteLine("Trying to download via https");
                                    status = DownloadFile("https://" + uriWithoutScheme, tempfilepath);
                                    if (!status)
                                    {
                                        Console.WriteLine("Trying to download via http");
                                        status = DownloadFile("http://" + uriWithoutScheme, tempfilepath);
                                    }
                                    if (!status)
                                    {
                                        Environment.Exit((int)ErrorCode.CouldNotDownloadInputFile);
                                    }
                                    else
                                    {
                                        input = tempfilepath;
                                    }
                                }
                                else
                                {
                                    bool status = DownloadFile(uri.ToString(), tempfilepath);
                                    if (!status)
                                    {
                                        Environment.Exit((int)ErrorCode.CouldNotDownloadInputFile);
                                    }
                                    else
                                    {
                                        input = tempfilepath;
                                    }
                                }
                            }
                        }
                    }

                    Console.WriteLine("Starting player ...");

                    // Inject Fusee.Engine.Base InjectMe dependencies
                    IO.IOImp = new Fusee.Base.Imp.Desktop.IOImp();

                    Type tApp = null;

                    string modelFile = null;
                    List<string> assetDirs = new List<string>();
                    TryAddDir(assetDirs, "Assets");

                    string ExeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string Cwd = Directory.GetCurrentDirectory();
                    if (Cwd != ExeDir)
                    {
                        TryAddDir(assetDirs, Path.Combine(ExeDir, "Assets"));
                    }

                    if (!string.IsNullOrEmpty(input))
                    {
                        Console.WriteLine("File: " + input);

                        if (File.Exists(input))
                        {
                            var ext = Path.GetExtension(input).ToLower();
                            var filepath = input;

                            TryAddDir(assetDirs, Path.GetDirectoryName(filepath));
                            switch (ext)
                            {
                                case ".fus":
                                    modelFile = Path.GetFileName(filepath);
                                    break;

                                case ".fuz":
                                    var appname = Path.GetFileNameWithoutExtension(filepath);
                                    var tmppath = Path.GetTempPath();

                                    var apppath = Path.Combine(tmppath, "FuseeApp_" + appname);

                                    if (Directory.Exists(apppath))
                                        Directory.Delete(apppath, true);

                                    ZipFile.ExtractToDirectory(filepath, apppath);

                                    filepath = Path.Combine(apppath, appname + ".dll");
                                    goto default;

                                default:
                                    try
                                    {
                                        Assembly asm = Assembly.LoadFrom(filepath);
                                        tApp = asm.GetTypes().FirstOrDefault(t => typeof(RenderCanvas).IsAssignableFrom(t));
                                        TryAddDir(assetDirs, Path.Combine(Path.GetDirectoryName(filepath), "Assets"));
                                    }
                                    catch (Exception e)
                                    {
                                        Diagnostics.Log(e.ToString());
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            Diagnostics.Log($"Cannot open {opts.Input}.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Fusee test scene. Use 'fusee player <filename/Uri>' to view .fus/.fuz files or Fusee .dlls.");
                    }

                    if (tApp == null)
                    {
                        // See if we are in "Deployed mode". That is: A Fusee.App.dll is lying next to us.
                        try
                        {
                            Assembly asm = Assembly.LoadFrom(Path.Combine(ExeDir, "Fusee.App.dll"));
                            tApp = asm.GetTypes().FirstOrDefault(t => typeof(RenderCanvas).IsAssignableFrom(t));
                        }
                        catch (Exception e)
                        {
                            Diagnostics.Log(e.ToString());
                        }
                        // No App was specified and we're not in Deplyed mode. Simply use the default App (== Viewer)
                        if (tApp == null)
                        {
                            tApp = typeof(Fusee.Engine.Player.Core.Player);
                        }
                    }

                    var fap = new Fusee.Base.Imp.Desktop.FileAssetProvider(assetDirs);
                    fap.RegisterTypeHandler(
                        new AssetHandler
                        {
                            ReturnedType = typeof(Font),
                            Decoder = delegate (string id, object storage)
                            {
                                if (!Path.GetExtension(id).ToLower().Contains("ttf")) return null;
                                return new Font { _fontImp = new FontImp((Stream)storage) };
                            },
                            Checker = id => Path.GetExtension(id).ToLower().Contains("ttf")
                        });
                    fap.RegisterTypeHandler(
                        new AssetHandler
                        {
                            ReturnedType = typeof(SceneContainer),
                            Decoder = delegate (string id, object storage)
                            {
                                if (!Path.GetExtension(id).ToLower().Contains("fus")) return null;

                                var scene = ProtoBuf.Serializer.Deserialize<SceneContainer>((Stream)storage);

                                var container = scene;

                                return new ConvertSceneGraph().Convert(container);
                            },
                            Checker = id => Path.GetExtension(id).ToLower().Contains("fus")
                        });

                    AssetStorage.RegisterProvider(fap);

                    // Dynamically instantiate the app because it might live in some external (.NET core) DLL.
                    var ctor = tApp.GetConstructor(Type.EmptyTypes);
                    if (ctor == null)
                    {
                        Diagnostics.Log($"Cannot instantiate FUSEE App. {tApp.Name} contains no default constructor");
                    }
                    else
                    {
                        // invoke the first public constructor with no parameters.
                        RenderCanvas app = (RenderCanvas)ctor.Invoke(new object[] { });

                        if (!string.IsNullOrEmpty(modelFile) && app is Fusee.Engine.Player.Core.Player)
                            ((Fusee.Engine.Player.Core.Player)app).ModelFile = modelFile;

                        // Inject Fusee.Engine InjectMe dependencies (hard coded)
                        System.Drawing.Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
                        app.CanvasImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasImp(appIcon);
                        app.ContextImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderContextImp(app.CanvasImplementor);
                        Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(app.CanvasImplementor));
                        Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.WindowsSpaceMouseDriverImp(app.CanvasImplementor));
                        Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(app.CanvasImplementor));
                        // app.InputImplementor = new Fusee.Engine.Imp.Graphics.Desktop.InputImp(app.CanvasImplementor);
                        // app.AudioImplementor = new Fusee.Engine.Imp.Sound.Desktop.AudioImp();
                        // app.NetworkImplementor = new Fusee.Engine.Imp.Network.Desktop.NetworkImp();
                        // app.InputDriverImplementor = new Fusee.Engine.Imp.Input.Desktop.InputDriverImp();
                        // app.VideoManagerImplementor = ImpFactory.CreateIVideoManagerImp();

                        // Start the app
                        app.Run();
                    }
                })
            #endregion

            #region ProtoShema
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
                            using (var output = new StreamWriter(File.Open(opts.Output, System.IO.FileMode.Create)))
                            {
                                // Is added with Protobuf 2.4.0: output.WriteLine("syntax = \"proto2\";");
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
            #endregion

            #region Publish
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
                            // 2.cd %FuseeRoot%/bin/Debug/Player/Desktop   // to have reference assemblies at hand in JSILc
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

                            // Call JSILc on the App DLL with cwd set to %FuseeRoot%/bin/Debug/Player/Desktop to have reference assemblies at hand in JS

                            string jsilRoot = Path.GetFullPath(Path.Combine(fuseeRoot, "ext", "JSIL"));
                            // Special JSIL configuration switching off Dead Code Elimination (optimizes too much away when combined with --nodeps)
                            string distroWebbuildConfigFilePath = Path.Combine(jsilRoot, "distrowebbuild.jsilconfig");
                            string jsilc = Path.Combine(jsilRoot, "Compiler", "JSILc.exe");
                            string temp = Path.Combine(outPath, "tmp");
                            Directory.CreateDirectory(temp);

                            using (Process cmd = Process.Start(new ProcessStartInfo
                            {
                                FileName = jsilc,
                                Arguments = $"--nodeps \"{distroWebbuildConfigFilePath}\" \"{dllFilePath}\" -o \"{temp}\"",
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
            #endregion

            #region Server
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
            #endregion

            #region Install
                // Called with the INSTALL verb
                .WithParsed<Install>(opts =>
                {
                    // Find FuseeRoot from this assembly
                    InitFuseeDirectories();

                    // Set the individual installation steps (currently four). If NONE of them is set, select ALL OF THEM.
                    bool instBlender = opts.Blender;
                    if (!(instBlender))
                    {
                        instBlender = true;
                    }

                    ErrorCode exitCode = ErrorCode.Success;

                    // Install or uninstall ? 
                    if (!opts.Uninstall)
                    {
                        // Install the Blender AddOn
                        if (instBlender)
                        {
                            string blenderAddOnDstDir = "";
                            string blenderAddOnSrcDir = "";

                            try
                            {
                                IEnumerable<string> possibleDirs;
                                if (!string.IsNullOrEmpty(opts.BlenderDir))
                                {
                                    if (!Directory.Exists(opts.BlenderDir))
                                    {
                                        throw new ArgumentException($"ERROR: The specified path doesn't exist {opts.BlenderDir}.");
                                    }
                                    string allLower = opts.BlenderDir.ToLower();
                                    if (!allLower.Contains("addon") && !allLower.Contains("blender"))
                                    {
                                        Console.Error.WriteLine($"WARNING: The specified path doesn't look like a typical Blender Add-on folder.");
                                    }
                                    blenderAddOnDstDir = opts.BlenderDir;
                                }
                                else
                                {
                                    possibleDirs = GetBlenderAddOnDir(opts);
                                    blenderAddOnDstDir = possibleDirs.FirstOrDefault();
                                    if (!string.IsNullOrEmpty(blenderAddOnDstDir) && !Directory.Exists(blenderAddOnDstDir))
                                        Directory.CreateDirectory(blenderAddOnDstDir);
                                }
                                
                                if (!string.IsNullOrEmpty(blenderAddOnDstDir))
                                {
                                    blenderAddOnSrcDir = Path.Combine(fuseeCmdLineRoot, "BlenderScripts", "addons");
                                    foreach (var addOnSrcDir in Directory.EnumerateDirectories(blenderAddOnSrcDir, "*", SearchOption.TopDirectoryOnly))
                                    {
                                        string addOnName = new DirectoryInfo(addOnSrcDir).Name;
                                        string addOnDstDir = Path.Combine(blenderAddOnDstDir, addOnName);
                                        FileTools.DirectoryCopy(addOnSrcDir, addOnDstDir, true, true);
                                        Console.Error.WriteLine($"SUCCESS: Installed the {addOnName} Blender Add-on to {blenderAddOnDstDir}.");
                                    }
                                }
                                else
                                {
                                    Console.Error.WriteLine($"ERROR: Could not find a suitable Blender installation to install the Add-on to. Consider using the '-i' option to specifiy the Blender Add-on installation directory.");
                                    exitCode = ErrorCode.OutputFile;
                                }
                            }
                            catch (UnauthorizedAccessException /* ex */)
                            {
                                Console.Error.WriteLine($"ERROR: Could not install FUSEE Blender Add-on to {blenderAddOnDstDir} due to access restrictions. Try running the shell as Administrator before calling fusee.exe.");
                                exitCode = ErrorCode.OutputFile;
                            }
                            catch (Exception ex)
                            {
                                Console.Error.WriteLine($"ERROR: Unable to install the FUSEE Blender Add-on.\n{ex}");
                                exitCode = ErrorCode.InternalError;
                            }
                        }
                    }
                    else
                    {
                        // UNINSTALL

                        // Uninstall the Blender AddOn
                        if (instBlender)
                        {
                            string blenderAddOnDstDir = "";
                            string blenderAddOnSrcDir = "";

                            try
                            {
                                IEnumerable<string> possibleDirs;
                                if (!string.IsNullOrEmpty(opts.BlenderDir))
                                {
                                    if (!Directory.Exists(opts.BlenderDir))
                                    {
                                        throw new ArgumentException($"ERROR: The specified path doesn't exist {opts.BlenderDir}.");
                                    }
                                    string allLower = opts.BlenderDir.ToLower();
                                    if (!allLower.Contains("addon") && !allLower.Contains("blender"))
                                    {
                                        Console.Error.WriteLine($"WARNING: The specified path doesn't look like a typical Blender Add-on folder.");
                                    }
                                    blenderAddOnDstDir = opts.BlenderDir;
                                }
                                else
                                {
                                    possibleDirs = GetBlenderAddOnDir(opts);
                                    blenderAddOnDstDir = possibleDirs.First();
                                }

                                if (!string.IsNullOrEmpty(blenderAddOnDstDir))
                                {
                                    blenderAddOnSrcDir = Path.Combine(fuseeCmdLineRoot, "BlenderScripts", "addons");
                                    foreach (var addOnSrcDir in Directory.EnumerateDirectories(blenderAddOnSrcDir, "*", SearchOption.TopDirectoryOnly))
                                    {
                                        string addOnName = new DirectoryInfo(addOnSrcDir).Name;
                                        string addOnDstDir = Path.Combine(blenderAddOnDstDir, addOnName);
                                        if (Directory.Exists(addOnDstDir))
                                        {
                                            Directory.Delete(addOnDstDir, true);
                                            Console.Error.WriteLine($"SUCCESS: Removed the the {addOnName} Blender Add-on from {blenderAddOnDstDir}.");
                                        }
                                        else
                                        {
                                            Console.Error.WriteLine($"WARNING: Could not remove the {addOnName} Blender Add-on from {blenderAddOnDstDir}. Either the Add-on was already removed, or this FUSEE instance installed the Add-on to a previous Blender version, or a different installation type (Machine/User) was specified at installation time.");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.Error.WriteLine($"ERROR: Could not find a suitable Blender installation where to remove the FUSEE Add-on from. Consider using the `-i` option to specifiy the Blender Add-on installation directory.");
                                    exitCode = ErrorCode.OutputFile;
                                }
                            }
                            catch (UnauthorizedAccessException /* ex */)
                            {
                                Console.Error.WriteLine($"ERROR: Could not uninstall FUSEE Blender Add-on from {blenderAddOnDstDir} due to access restrictions. Try running the shell as Administrator before calling fusee.exe.");
                                exitCode = ErrorCode.OutputFile;
                            }
                            catch (Exception ex)
                            {
                                Console.Error.WriteLine($"ERROR: Unable to uninstall the FUSEE Blender Add-on.\n{ex}");
                                exitCode = ErrorCode.InternalError;
                            }
                        }
                    }

                    if (exitCode == ErrorCode.Success)
                        Console.Error.WriteLine($"SUCCESS: All required FUSEE installation tasks succeeded.");
                    else
                        Console.Error.WriteLine($"WARNING: One or more required FUSEE installation tasks failed. See error messages above.");

                    Environment.Exit((int) exitCode);
                })
            #endregion

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

        private static void TryAddDir(List<string> dirList, string dir)
        {
            if (Directory.Exists(dir))
                dirList.Add(dir);
        }

        private static IEnumerable<string> GetBlenderAddOnDir(Install opts)
        {
             // Start with some possible start directories (e.g. "C:\Program Files\" and C:\Program Files (x86)\"
            List<string> baseDirs = new List<string>();
            baseDirs.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            baseDirs.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

            // Find [B|b]lender sub-subdirectories
            List<string> blenderDirs = new List<string>();
            foreach(var baseDir in baseDirs)
            {
                try
                {
                    blenderDirs.AddRange(Directory.EnumerateDirectories(baseDir, "?lender*", SearchOption.TopDirectoryOnly));
                }
                catch (UnauthorizedAccessException /* ex */)
                {
                    // Do nothing - simply ignore paths we cannot access.
                }
            }

            /* Removed - no FolderBrowserDialog in DotNet Core. Plus, opening a user dialog makes fusee.exe batch-incompatible
            // No Blender Installations found! Let the user pick Blender Path with file dialog!
            if (!blenderDirs.Any())
            {
                Console.WriteLine("WARNING: Blender Installation not found!\nINPUT REQUIRED: Please select Blender installation folder to proceed.");
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine($"OK! Selected Blender installation path: {fbd.SelectedPath}");
                    blenderDirs.Add(fbd.SelectedPath);
                } // ERROR message will be shown @909 if no blender addon could be installed... 
            }
            */

            // Find addon sub-subdirectories
            List<string> addonDirs = new List<string>();
            foreach (var blenderDir in blenderDirs)
            {
                addonDirs.AddRange(Directory.EnumerateDirectories(blenderDir, "addon?", SearchOption.AllDirectories));
            }

            // 
            List<string> realAddonDirs;
            if (opts.InstType == InstallationType.User)
            {
                // PER-USER Installation. Assume the respective Add-on-Directory below AppData\Roaming
                realAddonDirs = new List<string>();
                var appDataRoamingDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // This is "<user>\AppData\Roaming"
                foreach (var addonDir in addonDirs)
                {
                    string blVer = GetBlenderVersionFromPath(addonDir);
                    if (!string.IsNullOrEmpty(blVer))
                    {
                        realAddonDirs.Add(Path.Combine(appDataRoamingDir, "Blender Foundation", "Blender", blVer, "scripts", "addons"));
                    }
                }
            }
            else
            {
                // PER-MACHINE installation. We already have the addonDirs at hand
                realAddonDirs = addonDirs;
            }

            // Reverse-Sort directories according to some sub-dir name containting the blender version (e.g. 2.79 in ...\blender\2.79\addons\) 
            // (Highest version number first)
            return realAddonDirs.OrderByDescending(dir =>
            {
                int sortNumber = 0;
                foreach (var dirName in dir.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }).Reverse())
                {
                    // Build a sortable number from the (maximum) first four version digits. Typically Blender versions contain only two digits.
                    // Assume that digits are not bigger than 999 (e.g. Blender 2.999.111) 
                    var digits = dirName.Split('.');
                    var digitCount = digits.Length > 4 ? 4 : digits.Length;
                    for (int iDigit = 0; iDigit < digitCount; iDigit++)
                    {
                        if (int.TryParse(digits[iDigit], out int digit))
                            sortNumber += digit * (int) System.Math.Pow(1000, (3 - iDigit));
                    }
                    if (sortNumber > 0)
                        break;
                }
                return sortNumber;
            });
        }

        public static string GetBlenderVersionFromPath(string path)
        {
            foreach (var dirName in path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }).Reverse())
            {
                bool isVersion = true;
                var digits = dirName.Split('.');
                foreach (var digit in digits)
                {
                    if (!int.TryParse(digit, out int digiNum))
                    {
                        isVersion = false;
                    }
                }
                if (isVersion)
                    return dirName;
            }
            return "";
        }

        private static bool DownloadFile(string uri, string localfile)
        {
            bool status = false;

            using (var client = new WebClient())
            {
                try
                {
                    Console.Write("Downloading: " + uri);

                    client.DownloadFile(uri, localfile);
                    status = true;

                    Console.WriteLine(" - SUCCESS");
                }
                catch (Exception e)
                {
                    Console.WriteLine(" - FAILD");
                    status = false;
                }
            }

            return status;
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

