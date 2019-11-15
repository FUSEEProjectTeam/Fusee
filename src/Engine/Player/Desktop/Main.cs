using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Serialization;
using Path = Fusee.Base.Common.Path;


namespace Fusee.Engine.Player.Desktop
{
    public class Simple
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
                if (File.Exists(args[0]))
                {
                    TryAddDir(assetDirs, Path.GetDirectoryName(args[0]));
                    if (Path.GetExtension(args[0]).ToLower().Contains("fus"))
                    {
                        // A .fus file - open it.
                        modelFile = Path.GetFileName(args[0]);
                    }
                    else
                    {
                        // See if the passed argument is an entire Fusee App DLL
                        try
                        {
                            Assembly asm = Assembly.LoadFrom(args[0]);
                            tApp = asm.GetTypes().FirstOrDefault(t => typeof(RenderCanvas).IsAssignableFrom(t));
                            TryAddDir(assetDirs, Path.Combine(Path.GetDirectoryName(args[0]), "Assets"));
                        }
                        catch (Exception e)
                        {
                            Diagnostics.Info("No entire Fusee App DLL recived. Continuing.", e);
                        }
                    }
                }
                else
                {
                    Diagnostics.Error($"Cannot open {args[0]}.");
                }
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
                    Diagnostics.Info("Not in deployment mode", e);
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

                        var scene = ProtoBuf.Serializer.Deserialize<SceneContainer>((Stream) storage) ;

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
                Diagnostics.Fatal($"Cannot instantiate FUSEE App. {tApp.Name} contains no default constructor");
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
