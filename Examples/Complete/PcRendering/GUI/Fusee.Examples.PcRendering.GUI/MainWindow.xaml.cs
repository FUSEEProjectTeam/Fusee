using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Examples.PcRendering.Core;
using Fusee.Math.Core;
using Fusee.Pointcloud.PointAccessorCollections;
using Fusee.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace Fusee.Examples.PcRendering.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isAppInizialized = false;

        private Thread FUSThread;

        public Pointcloud.Common.IPcRendering app;

        public MainWindow()
        {
            InitializeComponent();

            Lighting.SelectedValue = PtRenderingParams.Lighting;
            PtShape.SelectedValue = PtRenderingParams.Shape;
            PtSizeMode.SelectedValue = PtRenderingParams.PtMode;
            ColorMode.SelectedValue = PtRenderingParams.ColorMode;

            PtSize.Value = PtRenderingParams.Size;

            SSAOCheckbox.IsChecked = PtRenderingParams.CalcSSAO;
            SSAOStrength.Value = PtRenderingParams.SSAOStrength;

            EDLStrengthVal.Content = EDLStrength.Value;
            EDLStrength.Value = PtRenderingParams.EdlStrength;
            EDLNeighbourPxVal.Content = EDLNeighbourPx.Value;
            EDLNeighbourPx.Value = PtRenderingParams.EdlNoOfNeighbourPx;

            ShininessVal.Text = PtRenderingParams.Shininess.ToString();
            SpecStrength.Value = PtRenderingParams.SpecularStrength;

            var col = PtRenderingParams.SingleColor;
            SingleColor.SelectedColor = System.Windows.Media.Color.FromScRgb(col.a, col.r, col.g, col.b);
                     

            if (PtRenderingParams.ColorMode != Pointcloud.Common.ColorMode.SINGLE)
                SingleColor.IsEnabled = false;
            else
                SingleColor.IsEnabled = true;

            InnerGrid.IsEnabled = false;

        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        #region UI Handler

        private void SSAOCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PtRenderingParams.CalcSSAO = !PtRenderingParams.CalcSSAO;
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

        private void SpecStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Core.PtRenderingParams.SpecularStrength = (float)e.NewValue;
        }
        private void Lighting_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PtRenderingParams.Lighting = (Pointcloud.Common.Lighting)e.AddedItems[0];

            if (PtRenderingParams.Lighting == Pointcloud.Common.Lighting.SSAO_ONLY || PtRenderingParams.Lighting == Pointcloud.Common.Lighting.UNLIT)
            {
                SSAOCheckbox.IsEnabled = false;
                SSAOStrengthLabel.IsEnabled = false;
                SSAOStrength.IsEnabled = false;
            }
            else
            {
                SSAOCheckbox.IsEnabled = true;
                SSAOStrengthLabel.IsEnabled = true;
                SSAOStrength.IsEnabled = true;
            }

            if (PtRenderingParams.Lighting == Pointcloud.Common.Lighting.SSAO_ONLY)           
                SSAOCheckbox.IsChecked = PtRenderingParams.CalcSSAO = true; 
            if (PtRenderingParams.Lighting == Pointcloud.Common.Lighting.UNLIT)
                SSAOCheckbox.IsChecked = PtRenderingParams.CalcSSAO = false;

            if (PtRenderingParams.Lighting != Pointcloud.Common.Lighting.BLINN_PHONG)
            {
                SpecStrength.IsEnabled = false;
                SpecStrengthLabel.IsEnabled = false;
                Shininess.IsEnabled = false;
                ShininessVal.IsEnabled = false;
            }
            else
            {
                SpecStrength.IsEnabled = true;
                SpecStrengthLabel.IsEnabled = true;
                Shininess.IsEnabled = true;
                ShininessVal.IsEnabled = true;
            }

            if (PtRenderingParams.Lighting != Pointcloud.Common.Lighting.EDL)
            {
                EDLNeighbourPx.IsEnabled = false;
                EDLNeighbourPxLabel.IsEnabled = false;
                EDLStrength.IsEnabled = false;
                EDLStrengthLabel.IsEnabled = false;
            }
            else
            {
                EDLNeighbourPx.IsEnabled = true;
                EDLNeighbourPxLabel.IsEnabled = true;
                EDLStrength.IsEnabled = true;
                EDLStrengthLabel.IsEnabled = true;
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

        private async void LoadFile_Button_Click(object sender, RoutedEventArgs e)
        {
            string fullPath;
            string path;
            var ofd = new OpenFileDialog
            {
                Filter = "Meta json (*.json)|*.json"
            };

            var sd = ofd.ShowDialog();
            if (sd == null) return;

            if ((bool)sd)
            {
                if (!ofd.SafeFileName.Contains("meta.json"))
                {
                    MessageBox.Show("Invalid file selected", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                fullPath = ofd.FileName;
                path = fullPath.Replace(ofd.SafeFileName, "");

                app?.CloseGameWindow();
                app = null;
                await OpenFusThread(path);

                if (app.GetOocLoaderRootNode() != null) //if RootNode == null no scene was ever initialized
                {
                    app.DeletePointCloud();

                    while (!app.ReadyToLoadNewFile || !app.GetOocLoaderWasSceneUpdated() || !_isAppInizialized)
                    {
                        app.IsSceneLoaded = false;
                        continue;
                    }
                }

                PtRenderingParams.PathToOocFile = path;
                app.ResetCamera();
                app.LoadPointCloudFromFile();
                InnerGrid.IsEnabled = true;
                ShowOctants_Button.IsEnabled = true;
                ShowOctants_Img.Source = new BitmapImage(new Uri("Assets/octants.png", UriKind.Relative));
                inactiveBorder.Visibility = Visibility.Collapsed;
            }

        }

        private void DeleteFile_Button_Click(object sender, RoutedEventArgs e)
        {
            app.DeletePointCloud();
        }

        private void ResetCam_Button_Click(object sender, RoutedEventArgs e)
        {
            app.ResetCamera();
        }

        private void VisPoints_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!_isAppInizialized) return;
            e.Handled = !IsTextAllowed(VisPoints.Text);

            if (!e.Handled)
            {
                if (!Int32.TryParse(VisPoints.Text, out var ptThreshold)) return;
                if (ptThreshold < 0)
                {
                    VisPoints.Text = app.GetOocLoaderPointThreshold().ToString();
                    return;
                }
                app.SetOocLoaderPointThreshold(ptThreshold);
            }
            else
            {
                VisPoints.Text = app.GetOocLoaderPointThreshold().ToString();
            }
        }

        private static readonly Regex numRegex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !numRegex.IsMatch(text);
        }

        private void ShininessVal_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!_isAppInizialized) return;
            e.Handled = !IsTextAllowed(ShininessVal.Text);

            if (!e.Handled)
            {
                if (!Int32.TryParse(ShininessVal.Text, out var shininess)) return;
                if (shininess < 0)
                {
                    ShininessVal.Text = Core.PtRenderingParams.Shininess.ToString();
                    return;
                }
                Core.PtRenderingParams.Shininess = shininess;
            }
            else
            {
                ShininessVal.Text = Core.PtRenderingParams.Shininess.ToString();
            }
        }

        private void ShininessVal_LostFocus(object sender, RoutedEventArgs e)
        {
            ShininessVal.Text = Core.PtRenderingParams.Shininess.ToString();
        }

        private void VisPoints_LostFocus(object sender, RoutedEventArgs e)
        {
            VisPoints.Text = app.GetOocLoaderPointThreshold().ToString();
        }

        private bool _areOctantsShown;

        private void ShowOctants_Button_Click(object sender, RoutedEventArgs e)
        {
            while (!app.ReadyToLoadNewFile || !app.GetOocLoaderWasSceneUpdated() || !app.IsSceneLoaded)
            {
                //app.IsSceneLoaded = false;
                continue;
            }

            if (!_areOctantsShown)
            {
                app.DoShowOctants = true;
                _areOctantsShown = true;
                ShowOctants_Img.Source = new BitmapImage(new Uri("Assets/octants_on.png", UriKind.Relative));
            }
            else
            {
                app.DeleteOctants();
                _areOctantsShown = false;
                ShowOctants_Img.Source = new BitmapImage(new Uri("Assets/octants.png", UriKind.Relative));
            }
        }

        #endregion 

        private async Task OpenFusThread(string pathToFile)
        {
            InnerGrid.IsEnabled = false;
            _isAppInizialized = false;

            if (app != null)
                while (!app.ReadyToLoadNewFile || !app.IsSceneLoaded) continue;

            if (FUSThread != null && FUSThread.IsAlive)
            {
                //At a few occasions the fusee app will throw a null reference at different stages, e.g. Keyboard in RenderAFrame or _networkImp in Network.OnUpdateFrame.
                //Flaw in Fusee shut down process or multi threading in this wpf app? (in a normal app the debugger will be terminated on shutdown...)
                try
                {
                    app?.CloseGameWindow(); //UI Thread
                    app = null;
                }
                catch (NullReferenceException nre)
                {

                }

                FUSThread.Join();
            }

            await Task.Run(() =>
            {
                FUSThread = new Thread(() =>
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
                                var ser = new Serializer();
                                return new ConvertSceneGraph().Convert(ser.Deserialize((Stream)storage, null, typeof(SceneContainer)) as SceneContainer);
                            },
                            Checker = id => Path.GetExtension(id).ToLower().Contains("fus")
                        });

                    AssetStorage.RegisterProvider(fap);

                    int th = 0;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        int.TryParse(VisPoints.Text, out th);
                    });                   

                    var ptType = AppSetupHelper.GetPtType(pathToFile);
                    var ptEnumName = Enum.GetName(typeof(PointType), ptType);

                    var genericType = Type.GetType("Fusee.Pointcloud.PointAccessorCollections." + ptEnumName + ", " + "Fusee.Pointcloud.PointAccessorCollections");

                    var objectType = typeof(PcRendering<>);
                    var objWithGenType = objectType.MakeGenericType(genericType);

                    app = (Pointcloud.Common.IPcRendering)Activator.CreateInstance(objWithGenType);
                    app.UseWPF = true;
                    AppSetup.DoSetup(app, AppSetupHelper.GetPtType(pathToFile), th, pathToFile);

                    // Inject Fusee.Engine InjectMe dependencies (hard coded)
                    System.Drawing.Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
                    app.CanvasImplementor = new Engine.Imp.Graphics.Desktop.RenderCanvasImp(appIcon);
                    app.ContextImplementor = new Engine.Imp.Graphics.Desktop.RenderContextImp(app.CanvasImplementor);
                    Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(app.CanvasImplementor));
                    Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(app.CanvasImplementor));

                    app.Run();

                });

                FUSThread.Start();

                while (app == null || app.IsInitialized == false) continue;
                Closed += (s, e) => app?.CloseGameWindow();

            });

            _isAppInizialized = true;
            InnerGrid.IsEnabled = true;
        }       
    }
}
