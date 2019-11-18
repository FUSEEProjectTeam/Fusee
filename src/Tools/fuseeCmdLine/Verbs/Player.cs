using CommandLine;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using Path = System.IO.Path;

namespace Fusee.Tools.fuseeCmdLine
{
    [Verb("player", HelpText = "Output the protobuf schema for the .fus file format.")]
    internal class Player
    {
        [Value(0, HelpText = "Path or url to .fus/.fuz file or Fusee-App .dll.", MetaName = "Input", Required = false)]
        public string InputArgs { get; set; }

        public int Run()
        {
            var input = InputArgs;

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
                                Diagnostics.Error("Error opening assembly", e);
                            }
                            break;
                    }
                }
                else
                {
                    Diagnostics.Warn($"Cannot open {input}.");
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
                    Diagnostics.Warn("Not in deployed mode", e);
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
                Diagnostics.Warn($"Cannot instantiate FUSEE App. {tApp.Name} contains no default constructor");
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
            return 0;
        }

        private void TryAddDir(List<string> dirList, string dir)
        {
            if (Directory.Exists(dir))
                dirList.Add(dir);
        }

        private bool DownloadFile(string uri, string localfile)
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
    }
}
