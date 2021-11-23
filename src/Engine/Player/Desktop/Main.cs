using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace Fusee.Engine.Player.Desktop
{
    public static class Simple
    {
        public static void TryAddDir(List<string> dirList, string dir)
        {
            if (Directory.Exists(dir))
                dirList.Add(dir);
        }

        public static void Main(string[] args)
        {
            // Inject Fusee.Engine.Base InjectMe dependencies
            IO.IOImp = new Fusee.Base.Imp.Desktop.IOImp();

            Type tApp = null;

            string modelFile = null;
            List<string> assetDirs = new();
            TryAddDir(assetDirs, "Assets");

            string ExeDir = AppContext.BaseDirectory;
            string Cwd = Directory.GetCurrentDirectory();

            if (Cwd != ExeDir)
            {
                TryAddDir(assetDirs, Path.Combine(ExeDir, "Assets"));
            }

            if (args.Length >= 1)
            {
                Diagnostics.Info("File: " + args[0]);

                if (File.Exists(args[0]))
                {
                    var ext = Path.GetExtension(args[0]).ToLower();
                    var filepath = args[0];

                    TryAddDir(assetDirs, Path.GetDirectoryName(filepath));
                    switch (ext)
                    {
                        case ".fus":
                            modelFile = Path.GetFileName(filepath);
                            tApp = typeof(Fusee.Engine.Player.Core.Player);
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

                                // Comparing our version with the version of the referenced Fusee.Serialization
                                var serversion = asm.GetReferencedAssemblies().First(x => x.Name == "Fusee.Serialization").Version;
                                var ourversion = Assembly.GetEntryAssembly().GetName().Version;

                                if (serversion != ourversion)
                                {
                                    Diagnostics.Info("Fusee player and the assembly are on different versions. This can result in unexpected behaviour. Player version: " + ourversion + " Assembly version: " + serversion);
                                }

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
                    Diagnostics.Warn($"Cannot open {args[0]}.");
                }
            }
            else if (File.Exists("Fusee.App.dll"))
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom("Fusee.App.dll");
                    tApp = asm.GetTypes().FirstOrDefault(t => typeof(RenderCanvas).IsAssignableFrom(t));
                }
                catch (Exception e)
                {
                    Diagnostics.Debug("Could not load Fusee.App.dll", e);
                }
            }
            else
            {
                Diagnostics.Info("Fusee test scene. Use 'fusee player <filename/Uri>' to view .fus/.fuz files or Fusee .dlls.");
                tApp = typeof(Fusee.Engine.Player.Core.Player);
            }

            #region FAP

            var fap = new Fusee.Base.Imp.Desktop.FileAssetProvider("Assets");
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(Font),
                    Decoder = (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)) return null;
                        return new Font { _fontImp = new FontImp((Stream)storage) };
                    },
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)) return await Task.FromResult(false).ConfigureAwait(false);
                        return await Task.FromResult(new Font { _fontImp = new FontImp((Stream)storage) }).ConfigureAwait(false);
                    },
                    Checker = id => Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)
                });
            fap.RegisterTypeHandler(

                new AssetHandler
                {
                    ReturnedType = typeof(SceneContainer),
                    Decoder = (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("fus", StringComparison.OrdinalIgnoreCase)) return null;

                        return FusSceneConverter.ConvertFrom(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id);
                    },
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase)) return await Task.FromResult(false).ConfigureAwait(false);
                        return await Task.FromResult(FusSceneConverter.ConvertFrom(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id)).ConfigureAwait(false);
                    },
                    Checker = id => Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase)
                });

            AssetStorage.RegisterProvider(fap);

            #endregion

            // Dynamically instantiate the app because it might live in some external (.NET core) DLL.
            var ctor = tApp.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                Diagnostics.Warn($"Cannot instantiate FUSEE App. {tApp.Name} contains no default constructor");
            }
            else
            {
                // invoke the first public constructor with no parameters.
                RenderCanvas app = (RenderCanvas)ctor.Invoke(Array.Empty<object>());

                if (!string.IsNullOrEmpty(modelFile) && app is Fusee.Engine.Player.Core.Player player)
                    player.ModelFile = modelFile;

                // Inject Fusee.Engine InjectMe dependencies (hard coded)
                app.CanvasImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasImp();
                app.ContextImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderContextImp(app.CanvasImplementor);
                Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(app.CanvasImplementor));
                Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.WindowsSpaceMouseDriverImp(app.CanvasImplementor));
                Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(app.CanvasImplementor));
                // app.InputImplementor = new Fusee.Engine.Imp.Graphics.Desktop.InputImp(app.CanvasImplementor);
                // app.InputDriverImplementor = new Fusee.Engine.Imp.Input.Desktop.InputDriverImp();
                // app.VideoManagerImplementor = ImpFactory.CreateIVideoManagerImp();

                app.InitApp();

                // Start the app
                app.Run();
            }
        }
    }
}