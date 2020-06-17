using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Examples.PcRendering.Core;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Fusee.Pointcloud.PointAccessorCollections;
using Fusee.Serialization;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace Fusee.Examples.PcRendering.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IPcRendering app;

        private bool _isAppInizialized = false;
        private bool _areOctantsShown;

        private bool _ptSizeDragStarted;
        private bool _projSizeModDragStarted;
        private bool _edlStrengthDragStarted;
        private bool _edlNeighbourPxDragStarted;
        private bool _ssaoStrengthDragStarted;
        private bool _specularStrengthPxDragStarted;

        private Thread _fusThread;

        private static readonly Regex numRegex = new Regex("[^0-9.-]+");

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
            SSAOStrength.IsEnabled = PtRenderingParams.CalcSSAO;

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
            SSAOStrength.IsEnabled = PtRenderingParams.CalcSSAO;
        }

        #region ssao strength
        private void SSAOStrength_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            _ssaoStrengthDragStarted = true;
        }

        private void SSAOStrength_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            PtRenderingParams.SSAOStrength = (float)((Slider)sender).Value;
            _ssaoStrengthDragStarted = false;
        }

        private void SSAOStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            if (_ssaoStrengthDragStarted) return;
            PtRenderingParams.SSAOStrength = (float)e.NewValue;
        }
        #endregion

        #region edl strength
        private void EDLStrength_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            _edlStrengthDragStarted = true;
        }

        private void EDLStrength_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            PtRenderingParams.EdlStrength = (float)((Slider)sender).Value;
            _edlStrengthDragStarted = false;
        }

        private void EDLStrengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            EDLStrengthVal.Content = e.NewValue.ToString("0.000");

            if (_edlStrengthDragStarted) return;
            PtRenderingParams.EdlStrength = (float)e.NewValue;
        }
        #endregion

        #region edl neighbor px

        private void EDLNeighbourPx_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            _edlNeighbourPxDragStarted = true;
        }

        private void EDLNeighbourPx_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            if (EDLNeighbourPxVal == null) return;

            EDLNeighbourPxVal.Content = ((Slider)sender).Value.ToString("0");
            PtRenderingParams.EdlNoOfNeighbourPx = (int)((Slider)sender).Value;

            _edlNeighbourPxDragStarted = false;
        }

        private void EDLNeighbourPxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            if (EDLNeighbourPxVal == null) return;

            EDLNeighbourPxVal.Content = e.NewValue.ToString("0");

            if (_edlNeighbourPxDragStarted) return;
            PtRenderingParams.EdlNoOfNeighbourPx = (int)e.NewValue;
        }

        #endregion       

        #region point size
        private void PtSize_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _ptSizeDragStarted = true;
        }

        private void PtSize_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (PtSizeVal == null) return;

            PtSizeVal.Content = ((Slider)sender).Value.ToString("0");
            PtRenderingParams.Size = (int)((Slider)sender).Value;
        }

        private void PtSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            if (PtSizeVal == null) return;
            PtSizeVal.Content = e.NewValue.ToString("0");

            if (_ptSizeDragStarted) return;
            PtRenderingParams.Size = (int)e.NewValue;
        }

        #endregion

        #region specular strength
        private void SpecStrength_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            _specularStrengthPxDragStarted = true;
        }

        private void SpecStrength_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            PtRenderingParams.SpecularStrength = (float)((Slider)sender).Value;
            _specularStrengthPxDragStarted = false;
        }

        private void SpecStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            if (_specularStrengthPxDragStarted) return;
            PtRenderingParams.SpecularStrength = (float)e.NewValue;
        }
        #endregion

        #region min. proj. size modifier

        private void MinProjSize_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _projSizeModDragStarted = true;
        }

        private void MinProjSize_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (_isAppInizialized)
                MinProjSizeVal.Content = MinProjSize.Value.ToString("0.00");
            app?.SetOocLoaderMinProjSizeMod((float)MinProjSize.Value);

            _projSizeModDragStarted = false;
        }

        private void MinProjSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isAppInizialized)
                MinProjSizeVal.Content = MinProjSize.Value.ToString("0.00");

            if (_projSizeModDragStarted) return;
            app?.SetOocLoaderMinProjSizeMod((float)MinProjSize.Value);
        }
        #endregion

        private void SingleColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            var col = e.NewValue.Value;

            PtRenderingParams.SingleColor = new float4(col.ScR, col.ScG, col.ScB, col.ScA);
        }

        private void Lighting_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;

            PtRenderingParams.Lighting = (Lighting)e.AddedItems[0];

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
                SSAOStrength.IsEnabled = PtRenderingParams.CalcSSAO;
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
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            PtRenderingParams.Shape = (PointShape)e.AddedItems[0];
        }

        private void PtSizeMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            PtRenderingParams.PtMode = (PointSizeMode)e.AddedItems[0];
        }

        private void ColorMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!_isAppInizialized || !app.IsSceneLoaded) return;
            PtRenderingParams.ColorMode = (ColorMode)e.AddedItems[0];

            if (PtRenderingParams.ColorMode != Pointcloud.Common.ColorMode.SINGLE)
                SingleColor.IsEnabled = false;
            else
                SingleColor.IsEnabled = true;
        }

        private async void LoadFile_Button_Click(object sender, RoutedEventArgs e)
        {
            string fullPath;
            string path;
            var ofd = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Meta json (*.json)|*.json"
            };

            var dialogResult = ofd.ShowDialog();

            Console.WriteLine(dialogResult);

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (!ofd.SafeFileName.Contains("meta.json"))
                {
                    MessageBox.Show("Invalid file selected", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                fullPath = ofd.FileName;
                path = fullPath.Replace(ofd.SafeFileName, "");

                await OpenFusThread(path);

                MinProjSize.Value = app.GetOocLoaderMinProjSizeMod();
                MinProjSizeVal.Content = MinProjSize.Value.ToString("0.00");

                if (app.GetOocLoaderRootNode() != null) //if RootNode == null no scene was ever initialized
                {
                    app.DeletePointCloud();

                    SpinWait.SpinUntil(() => app.ReadyToLoadNewFile && app.GetOocLoaderWasSceneUpdated() && _isAppInizialized);
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
            app?.ResetCamera();
        }

        private void VisPoints_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!_isAppInizialized) return;
            e.Handled = !IsTextAllowed(PtThreshold.Text);

            if (!e.Handled)
            {
                if (!int.TryParse(PtThreshold.Text, out var ptThreshold)) return;
                if (ptThreshold < 0)
                {
                    PtThreshold.Text = app.GetOocLoaderPointThreshold().ToString();
                    return;
                }
                app.SetOocLoaderPointThreshold(ptThreshold);
            }
            else
            {
                PtThreshold.Text = app.GetOocLoaderPointThreshold().ToString();
            }
        }

        private void ShininessVal_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!_isAppInizialized) return;
            e.Handled = !IsTextAllowed(ShininessVal.Text);

            if (!e.Handled)
            {
                if (!int.TryParse(ShininessVal.Text, out var shininess)) return;
                if (shininess < 0)
                {
                    ShininessVal.Text = Core.PtRenderingParams.Shininess.ToString();
                    return;
                }

                PtRenderingParams.Shininess = shininess;
            }
            else
                ShininessVal.Text = PtRenderingParams.Shininess.ToString();
        }

        private void ShininessVal_LostFocus(object sender, RoutedEventArgs e)
        {
            ShininessVal.Text = PtRenderingParams.Shininess.ToString();
        }

        private void VisPoints_LostFocus(object sender, RoutedEventArgs e)
        {
            PtThreshold.Text = app.GetOocLoaderPointThreshold().ToString();
        }

        private void ShowOctants_Button_Click(object sender, RoutedEventArgs e)
        {
            while (!app.ReadyToLoadNewFile || !app.GetOocLoaderWasSceneUpdated() || !app.IsSceneLoaded)
                continue;

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

        private static bool IsTextAllowed(string text)
        {
            return !numRegex.IsMatch(text);
        }

        private async Task OpenFusThread(string pathToFile)
        {
            InnerGrid.IsEnabled = false;
            _isAppInizialized = false;

            if (app != null && !app.IsAlive)
                app = null;

            if (_fusThread != null && _fusThread.IsAlive)
            {
                try
                {
                    app?.CloseGameWindow(); //UI Thread
                    app = null;
                }
                catch (NullReferenceException) { }

                _fusThread.Join();
            }

            await Task.Run(() =>
            {
                _fusThread = new Thread(() =>
                {
                    // Inject Fusee.Engine.Base InjectMe dependencies
                    IO.IOImp = new IOImp();

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

                    int th = 0;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        int.TryParse(PtThreshold.Text, out th);
                    });

                    var ptType = AppSetupHelper.GetPtType(pathToFile);
                    var ptEnumName = Enum.GetName(typeof(PointType), ptType);

                    var genericType = Type.GetType("Fusee.Pointcloud.PointAccessorCollections." + ptEnumName + ", " + "Fusee.Pointcloud.PointAccessorCollections");

                    var objectType = typeof(PcRendering<>);
                    var objWithGenType = objectType.MakeGenericType(genericType);

                    app = (IPcRendering)Activator.CreateInstance(objWithGenType);
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

                _fusThread.Start();

                SpinWait.SpinUntil(() => app != null && app.IsInitialized);

                Closed += (s, e) => app?.CloseGameWindow();

            });

            _isAppInizialized = true;
            InnerGrid.IsEnabled = true;
        }
    }
}
