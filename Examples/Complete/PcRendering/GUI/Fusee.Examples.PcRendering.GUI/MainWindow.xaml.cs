using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Math.Core;
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

namespace Fusee.Examples.PcRendering.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Fusee.Examples.PcRendering.Core.PcRendering app;
        private bool _isAppInizialized = false;

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

                app = new Fusee.Examples.PcRendering.Core.PcRendering();

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

                app.UseWPF = true;

                // Start the app
                app.Run();

            }).Start();

            Lighting.SelectedValue = Core.PtRenderingParams.Lighting;
            PtShape.SelectedValue = Core.PtRenderingParams.Shape;
            PtSizeMode.SelectedValue = Core.PtRenderingParams.PtMode;
            ColorMode.SelectedValue = Core.PtRenderingParams.ColorMode;

            PtSize.Value = Core.PtRenderingParams.Size;

            SSAOCheckbox.IsChecked = Core.PtRenderingParams.CalcSSAO;
            SSAOStrength.Value = Core.PtRenderingParams.SSAOStrength;

            EDLStrengthVal.Content = EDLStrength.Value;
            EDLStrength.Value = Core.PtRenderingParams.EdlStrength;
            EDLNeighbourPxVal.Content = EDLNeighbourPx.Value;
            EDLNeighbourPx.Value = Core.PtRenderingParams.EdlNoOfNeighbourPx;

            var col = Core.PtRenderingParams.SingleColor;
            SingleColor.SelectedColor = System.Windows.Media.Color.FromScRgb(col.a, col.r, col.g, col.b);

            if ((bool)SSAOCheckbox.IsChecked)
                SSAOStrength.IsEnabled = true;
            else
                SSAOStrength.IsEnabled = false;

            if (Core.PtRenderingParams.ColorMode != Pointcloud.Common.ColorMode.SINGLE)
                SingleColor.IsEnabled = false;
            else
                SingleColor.IsEnabled = true;

            InnerGrid.IsEnabled = false;

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

            _isAppInizialized = true;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        private void SSAOCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Core.PtRenderingParams.CalcSSAO = !Core.PtRenderingParams.CalcSSAO;
            if (!(bool)SSAOCheckbox.IsChecked && Core.PtRenderingParams.Lighting == Pointcloud.Common.Lighting.SSAO_ONLY)
            {
                SSAOStrength.IsEnabled = false;
                Lighting.SelectedItem = Pointcloud.Common.Lighting.UNLIT;
            }

            if ((bool)SSAOCheckbox.IsChecked && Core.PtRenderingParams.Lighting == Pointcloud.Common.Lighting.UNLIT)
            {
                SSAOStrength.IsEnabled = true;
                Lighting.SelectedItem = Pointcloud.Common.Lighting.SSAO_ONLY;
            }     
            
            if(Core.PtRenderingParams.Lighting != Pointcloud.Common.Lighting.UNLIT && Core.PtRenderingParams.Lighting != Pointcloud.Common.Lighting.SSAO_ONLY)
            {
                SSAOStrength.IsEnabled = Core.PtRenderingParams.CalcSSAO;
            }

        }

        private void SSAOStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Core.PtRenderingParams.SSAOStrength = (float)e.NewValue;
        }

        private void EDLStrengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            EDLStrengthVal.Content = e.NewValue.ToString("0.000");
            Core.PtRenderingParams.EdlStrength = (float)e.NewValue;            
        }

        private void EDLNeighbourPxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (EDLNeighbourPxVal == null) return;            

            EDLNeighbourPxVal.Content = e.NewValue.ToString("0");
            Core.PtRenderingParams.EdlNoOfNeighbourPx = (int)e.NewValue;
        }

        private void SingleColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {

            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            var col = e.NewValue.Value;
            Core.PtRenderingParams.SingleColor = new float4(col.ScR, col.ScG, col.ScB, col.ScA); 
            
        }        

        private void PtSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PtSizeVal == null) return;
            
            PtSizeVal.Content = e.NewValue.ToString("0");
            Core.PtRenderingParams.Size = (int)e.NewValue;
        }

        private void Lighting_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {            
            Core.PtRenderingParams.Lighting = (Pointcloud.Common.Lighting) e.AddedItems[0];          

            if ((Pointcloud.Common.Lighting)e.AddedItems[0] == Pointcloud.Common.Lighting.SSAO_ONLY && !(bool)SSAOCheckbox.IsChecked)
            {
                SSAOStrength.IsEnabled = true;
                SSAOCheckbox.IsChecked = true;
                Core.PtRenderingParams.CalcSSAO = true;
            }

            if ((Pointcloud.Common.Lighting)e.AddedItems[0] == Pointcloud.Common.Lighting.UNLIT && (bool)SSAOCheckbox.IsChecked)
            {
                SSAOStrength.IsEnabled = false;
                SSAOCheckbox.IsChecked = false;
                Core.PtRenderingParams.CalcSSAO = false;
            }

            if ((Pointcloud.Common.Lighting)e.AddedItems[0] != Pointcloud.Common.Lighting.EDL)
            {
                EDLNeighbourPx.IsEnabled = false;
                EDLStrength.IsEnabled = false;
            }
            else
            {
                EDLNeighbourPx.IsEnabled = true;
                EDLStrength.IsEnabled = true;
            }
        }

        private void PtShape_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Core.PtRenderingParams.Shape = (Pointcloud.Common.PointShape)e.AddedItems[0];
        }

        private void PtSizeMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Core.PtRenderingParams.PtMode = (Pointcloud.Common.PointSizeMode)e.AddedItems[0];
        }

        private void ColorMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Core.PtRenderingParams.ColorMode = (Pointcloud.Common.ColorMode)e.AddedItems[0];

            if (Core.PtRenderingParams.ColorMode != Pointcloud.Common.ColorMode.SINGLE)
                SingleColor.IsEnabled = false;
            else
                SingleColor.IsEnabled = true;
        }

        private void LoadFile_Button_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                if (app.OocLoader.RootNode != null) //if RootNode == null no scene was ever initialized
                {
                    app.DeletePointCloud();

                    while (!app.ReadyToLoadNewFile || !app.OocLoader.WasSceneUpdated)
                    {
                        //app.IsSceneLoaded = false;
                        continue;
                    }
                }

                app.PathToOocFile = fbd.SelectedPath;
                app.LoadPointCloudFromFile();
                InnerGrid.IsEnabled = true;
            }

        }

        private void DeleteFile_Button_Click(object sender, RoutedEventArgs e)
        {
            app.DeletePointCloud();
        }
    }
}
