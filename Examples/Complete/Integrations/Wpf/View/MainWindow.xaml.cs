using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Examples.Integrations.Wpf.Model;
using Fusee.Examples.Integrations.Wpf.ViewModel;
using Fusee.Serialization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Fusee.Examples.Integrations.Wpf.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel mainViewModel;
        private Core.Main fuseeApp;

        public MainWindow()
        {
            mainViewModel = new MainViewModel();
            DataContext = mainViewModel;

            mainViewModel.Position.PropertyChanged += Position_PropertyChanged;

            InitializeComponent();

            OpenFusee();
        }

        private void Position_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is PositionModel p)
            {
                if (e.PropertyName == "X")
                {
                    fuseeApp?.ChangeRocketX(p.X);
                }
                else if (e.PropertyName == "Y")
                {
                    fuseeApp?.ChangeRocketY(p.Y);
                }
                else if (e.PropertyName == "Z")
                {
                    fuseeApp?.ChangeRocketZ(p.Z);
                }
            }
        }

        private void OpenFusee()
        {
            Task.Run(() =>
            {
                IO.IOImp = new Fusee.Base.Imp.Desktop.IOImp();

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
                        Checker = id => Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)
                    });
                fap.RegisterTypeHandler(
                    new AssetHandler
                    {
                        ReturnedType = typeof(SceneContainer),
                        Decoder = (string id, object storage) =>
                        {
                            if (!Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase)) return null;
                            return FusSceneConverter.ConvertFrom(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage));
                        },
                        Checker = id => Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase)
                    });

                AssetStorage.RegisterProvider(fap);

                fuseeApp = new Core.Main();

                fuseeApp.FusToWpfEvents += FusToWpfEvents;

                // Inject Fusee.Engine InjectMe dependencies (hard coded)
                System.Drawing.Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
                fuseeApp.CanvasImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasImp(appIcon);
                fuseeApp.ContextImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderContextImp(fuseeApp.CanvasImplementor);
                Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(fuseeApp.CanvasImplementor));
                Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(fuseeApp.CanvasImplementor));
                // app.InputImplementor = new Fusee.Engine.Imp.Graphics.Desktop.InputImp(app.CanvasImplementor);
                // app.InputDriverImplementor = new Fusee.Engine.Imp.Input.Desktop.InputDriverImp();
                // app.VideoManagerImplementor = ImpFactory.CreateIVideoManagerImp();

                // Start the app
                fuseeApp.Run();
            });
        }

        private void FusToWpfEvents(object sender, Core.FusEvent e)
        {
            if (e is Core.FpsEvent f)
            {
                mainViewModel.Fps.Fps = f.Fps;
            }
            else if (e is Core.StartupInfoEvent si)
            {
                mainViewModel.VSync.VSync = si.VSync;
            }
        }
    }
}