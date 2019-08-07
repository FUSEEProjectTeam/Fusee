using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Serialization;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Path = System.IO.Path;

namespace Fusee.Examples.Simple.GUI.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Fusee.Examples.Simple.Core.Simple app;

        public MainWindow()
        {
            InitializeComponent();

            new Thread(() =>
            {
                // Inject Fusee.Engine.Base InjectMe dependencies
                IO.IOImp = new Fusee.Base.Imp.Desktop.IOImp();

                var fap = new Fusee.Base.Imp.Desktop.FileAssetProvider("Assets");
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
                            if (!System.IO.Path.GetExtension(id).ToLower().Contains("fus")) return null;
                            var ser = new Serializer();
                            return new ConvertSceneGraph().Convert(ser.Deserialize((Stream)storage, null, typeof(SceneContainer)) as SceneContainer);
                            return null;
                        },
                        Checker = id => System.IO.Path.GetExtension(id).ToLower().Contains("fus")
                    });

                AssetStorage.RegisterProvider(fap);

                app = new Fusee.Examples.Simple.Core.Simple();

                // Inject Fusee.Engine InjectMe dependencies (hard coded)
                System.Drawing.Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
                app.CanvasImplementor = new Engine.Imp.Graphics.Desktop.RenderCanvasImp(appIcon);
                app.ContextImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderContextImp(app.CanvasImplementor);
                Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(app.CanvasImplementor));
                Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(app.CanvasImplementor));
                // app.InputImplementor = new Fusee.Engine.Imp.Graphics.Desktop.InputImp(app.CanvasImplementor);
                // app.AudioImplementor = new Fusee.Engine.Imp.Sound.Desktop.AudioImp();
                // app.NetworkImplementor = new Fusee.Engine.Imp.Network.Desktop.NetworkImp();
                // app.InputDriverImplementor = new Fusee.Engine.Imp.Input.Desktop.InputDriverImp();
                // app.VideoManagerImplementor = ImpFactory.CreateIVideoManagerImp();

                app.CanvasImplementor.Init += MainWindow_Initialized;

                // Start the app
                app.Run();
                
            }).Start();
        }

        private void MainWindow_Initialized(object sender, System.EventArgs e)
        {
            // find window handle of the Fusee window
            var fuseeWinHandle = FindWindow(null, app.CanvasImplementor.Caption);
            IntPtr wpfHandle = IntPtr.Zero;
            App.Current.Dispatcher.Invoke(() =>
            {
                wpfHandle = FindWindow(null, Name);
            });
           
            if (fuseeWinHandle == IntPtr.Zero)
                throw new Exception("Error: Fusee window not found!");

            if (wpfHandle == IntPtr.Zero)
                throw new Exception("Error: WPF window not found!");

            // sets the Fusee window as a child of the given parent form
            App.Current.Dispatcher.Invoke(() =>
            {
                SetParent(fuseeWinHandle, wpfHandle);
            });
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    }
}
