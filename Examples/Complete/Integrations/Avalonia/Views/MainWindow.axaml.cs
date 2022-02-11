using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Fusee.Base.Core;
using Fusee.Base.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Avalonia.Desktop;
using Fusee.Examples.Integrations.Core;
using System.IO;
using Fusee.Serialization;
using Fusee.Base.Imp.Desktop;
using System.Threading.Tasks;
using System;

namespace Fusee.Example.Integrations.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            InitializeFusee();
        }

        public static Main? FuseeApp { get; private set; }

        private void InitializeFusee()
        {
            IO.IOImp = new IOImp();

            var fap = new FileAssetProvider("Assets");
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(Font),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)) return null;
                        return await Task.FromResult(new Font { _fontImp = new FontImp((Stream)storage) });
                    },
                    Decoder = (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)) return null;
                        return new Font { _fontImp = new FontImp((Stream)storage) };
                    },
                    Checker = id => Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)
                });
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(SceneContainer),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase)) return null;
                        return await FusSceneConverter.ConvertFromAsync(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id);
                    },
                    Decoder = (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase)) return null;
                        return FusSceneConverter.ConvertFrom(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id);
                    },
                    Checker = id => Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase)
                });

            AssetStorage.RegisterProvider(fap);

            FuseeApp = new Main();

            // retrive WindowControl instance from axaml
            var fuseeWindow = this.FindControl<FuseeWindowControl>("FuseeWindowControl");

            if (fuseeWindow == null)
                throw new Exception("FuseeWindowControl not found in *.axaml file.");

            fuseeWindow.OnInit += (s, e) =>
            {
                FuseeApp.CanvasImplementor = new AvaloniaRenderCanvasImp(fuseeWindow);
                FuseeApp.ContextImplementor = new AvaloniaRenderContextImp(FuseeApp.CanvasImplementor);

                Input.AddDriverImp(new AvaloniaRenderCanvasInputDriverImp(FuseeApp.CanvasImplementor, fuseeWindow));

                // pass custom RenderContext which flips coordinate axis
                FuseeApp.InitApp(new AvaloniaRenderContext(FuseeApp.ContextImplementor));
                FuseeApp.Run();
            };
        }
    }
}
