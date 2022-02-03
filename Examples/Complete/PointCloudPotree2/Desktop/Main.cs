using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Examples.PointCloudPotree2.Core;
using Fusee.Serialization;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Fusee.Examples.PointCloudPotree2.Desktop
{
    public class PcRendering
    {
        public static void Main()
        {
            // Inject Fusee.Engine.Base InjectMe dependencies
            IO.IOImp = new IOImp();

            var fap = new FileAssetProvider("Assets");
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(Font),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("ttf", StringComparison.OrdinalIgnoreCase)) return null;
                        return await Task.FromResult(new Font { _fontImp = new FontImp((Stream)storage) });
                    },
                    Decoder = (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("ttf", StringComparison.OrdinalIgnoreCase)) return null;
                        return new Font { _fontImp = new FontImp((Stream)storage) };
                    },
                    Checker = id => Path.GetExtension(id).Contains("ttf", StringComparison.OrdinalIgnoreCase)
                });
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(SceneContainer),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("fus", StringComparison.OrdinalIgnoreCase)) return null;
                        return await FusSceneConverter.ConvertFromAsync(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id);
                    },
                    Decoder = (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("fus", StringComparison.OrdinalIgnoreCase)) return null;
                        return FusSceneConverter.ConvertFrom(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id);
                    },
                    Checker = id => Path.GetExtension(id).Contains("fus", StringComparison.OrdinalIgnoreCase)
                });

            AssetStorage.RegisterProvider(fap);

            var app = new PointCloudOutOfCore();

            // Inject Fusee.Engine InjectMe dependencies (hard coded)
            System.Drawing.Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            app.CanvasImplementor = new Engine.Imp.Graphics.Desktop.RenderCanvasImp(appIcon);
            app.ContextImplementor = new Engine.Imp.Graphics.Desktop.RenderContextImp(app.CanvasImplementor);
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(app.CanvasImplementor));
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(app.CanvasImplementor));
            app.InitApp();
            PointCloudImplementor.CreateGpuMesh = app.RC.CreateGpuMesh;

            // Start the app
            app.Run();
        }
    }
}