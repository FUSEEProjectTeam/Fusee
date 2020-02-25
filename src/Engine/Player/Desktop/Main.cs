﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Serialization;
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
            List<string> assetDirs = new List<string>();
            TryAddDir(assetDirs, "Assets");

            string ExeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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

            var fap = new Fusee.Base.Imp.Desktop.FileAssetProvider(assetDirs);
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(Font),
                    Decoder = (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("ttf", StringComparison.OrdinalIgnoreCase)) return null;
                        return new Font { _fontImp = new FontImp((Stream)storage) };
                    },
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("ttf", StringComparison.OrdinalIgnoreCase)) return null;
                        return await Task.Factory.StartNew(() => new Font { _fontImp = new FontImp((Stream)storage) });
                    },
                    Checker = id => Path.GetExtension(id).Contains("ttf", StringComparison.OrdinalIgnoreCase)
                });
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(SceneContainer),
                    Decoder = (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("fus", StringComparison.OrdinalIgnoreCase)) return null;

                        return Serializer.DeserializeSceneContainer((Stream)storage);
                    },
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("fus", StringComparison.OrdinalIgnoreCase)) return null;

                        return await Task.Factory.StartNew(() => Serializer.DeserializeSceneContainer((Stream)storage));
                    },
                    Checker = id => Path.GetExtension(id).Contains("fus", StringComparison.OrdinalIgnoreCase)
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
        }
    }
}
