using System.IO;
using System.Runtime.InteropServices;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Serialization;
using FileMode = Fusee.Base.Common.FileMode;
using Path = Fusee.Base.Common.Path;
using System.Reflection;
using System;
using Fusee.Pointcloud.PointAccessorCollections;
using Fusee.Examples.PcRendering.Core;

namespace Fusee.Examples.PcRendering.Desktop
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
                    Decoder = delegate (string id, object storage)
                    {
                        if (!Path.GetExtension(id).ToLower().Contains("ttf")) return null;
                        return new Font{ _fontImp = new FontImp((Stream)storage) };
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
                        return new ConvertSceneGraph().Convert(ProtoBuf.Serializer.Deserialize<SceneContainer>((Stream)storage));
                    },
                    Checker = id => Path.GetExtension(id).ToLower().Contains("fus")
                });

            AssetStorage.RegisterProvider(fap);

            var ptType = AppSetupHelper.GetPtType(PtRenderingParams.PathToOocFile);
            var ptEnumName = Enum.GetName(typeof(PointType), ptType);

            var genericType = Type.GetType("Fusee.Pointcloud.PointAccessorCollections." + ptEnumName + ", " + "Fusee.Pointcloud.PointAccessorCollections");

            var objectType = typeof(PcRendering<>);
            var objWithGenType = objectType.MakeGenericType(genericType);

            var app = (Pointcloud.Common.IPcRendering)Activator.CreateInstance(objWithGenType);            
            AppSetup.DoSetup(app, ptType, PtRenderingParams.MaxNoOfVisiblePoints, PtRenderingParams.PathToOocFile);

            // Inject Fusee.Engine InjectMe dependencies (hard coded)
            System.Drawing.Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
			app.CanvasImplementor = new Engine.Imp.Graphics.Desktop.RenderCanvasImp(appIcon);            
            app.ContextImplementor = new Engine.Imp.Graphics.Desktop.RenderContextImp(app.CanvasImplementor);
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(app.CanvasImplementor));
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(app.CanvasImplementor));

            // Start the app
            app.Run();
        }
    }
}
