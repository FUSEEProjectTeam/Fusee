using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Examples.Integrations.Wpf.Model;
using Fusee.Examples.Integrations.Wpf.ViewModel;
using Fusee.Serialization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Fusee.Examples.Integrations.Wpf.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;
        private Core.Main _fuseeApp;

        public MainWindow()
        {
            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;

            _mainViewModel.Position.PropertyChanged += Position_PropertyChanged;

            InitializeComponent();

            //Needs to be done on the main thread  - else glfw will throw an error.
            CreateFuseeApp();

            Task.Run(() =>
            {
                _fuseeApp.Run();
            });
        }
        private void Position_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is PositionModel p)
            {
                if (e.PropertyName == "X")
                {
                    _fuseeApp?.ChangeRocketX(p.X);
                }
                else if (e.PropertyName == "Y")
                {
                    _fuseeApp?.ChangeRocketY(p.Y);
                }
                else if (e.PropertyName == "Z")
                {
                    _fuseeApp?.ChangeRocketZ(p.Z);
                }
            }
        }

        private void CreateFuseeApp()
        {
            IO.IOImp = new Fusee.Base.Imp.Desktop.IOImp();

            var fap = new Fusee.Base.Imp.Desktop.FileAssetProvider("Assets");
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

            _fuseeApp = new Core.Main();

            _fuseeApp.FusToWpfEvents += FusToWpfEvents;

            //Inject Fusee.Engine InjectMe dependencies(hard coded)
            var icon = AssetStorage.Get<ImageData>("FuseeIconTop32.png");
            _fuseeApp.CanvasImplementor = new Engine.Imp.Graphics.Desktop.RenderCanvasImp(icon, true);
            _fuseeApp.ContextImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderContextImp(_fuseeApp.CanvasImplementor);
            Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(_fuseeApp.CanvasImplementor));
            Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(_fuseeApp.CanvasImplementor));
            // app.InputImplementor = new Fusee.Engine.Imp.Graphics.Desktop.InputImp(app.CanvasImplementor);
            // app.InputDriverImplementor = new Fusee.Engine.Imp.Input.Desktop.InputDriverImp();
            // app.VideoManagerImplementor = ImpFactory.CreateIVideoManagerImp();

            _fuseeApp.InitApp();
        }

        private void FusToWpfEvents(object sender, Core.FusEvent e)
        {
            if (e is Core.FpsEvent f)
            {
                _mainViewModel.Fps.Fps = f.Fps;
            }
            else if (e is Core.StartupInfoEvent si)
            {
                _mainViewModel.VSync.VSync = si.VSync;
            }
        }
    }
}