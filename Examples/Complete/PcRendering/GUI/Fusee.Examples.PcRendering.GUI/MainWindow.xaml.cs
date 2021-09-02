using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Examples.PcRendering.Core;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.PointAccessorCollections;
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
        public IPcRendering App;

        private bool _isAppInizialized = false;
        private bool _areOctantsShown;

        //private bool _ptSizeDragStarted;
        private bool _projSizeModDragStarted;
        private bool _edlStrengthDragStarted;
        private bool _edlNeighbourPxDragStarted;
        private bool _ssaoStrengthDragStarted;
        private bool _specularStrengthPxDragStarted;

        private Task _fusTask;

        private static readonly Regex numRegex = new("[^0-9.-]+");

        public MainWindow()
        {
            InitializeComponent();

            ThreadPool.GetMaxThreads(out int t1, out int t2);
            Console.WriteLine(t1 + " " + t2);
            ThreadPool.SetMaxThreads(2, 2);

            Lighting.SelectedValue = PtRenderingParams.Instance.Lighting;
            PtShape.SelectedValue = PtRenderingParams.Instance.Shape;
            PtSizeMode.SelectedValue = PtRenderingParams.Instance.PtMode;
            ColorMode.SelectedValue = PtRenderingParams.Instance.ColorMode;

            PtSize.Value = PtRenderingParams.Instance.Size;

            SSAOCheckbox.IsChecked = PtRenderingParams.Instance.CalcSSAO;
            SSAOStrength.Value = PtRenderingParams.Instance.SSAOStrength;

            EDLStrengthVal.Content = EDLStrength.Value;
            EDLStrength.Value = PtRenderingParams.Instance.EdlStrength;
            EDLNeighbourPxVal.Content = EDLNeighbourPx.Value;
            EDLNeighbourPx.Value = PtRenderingParams.Instance.EdlNoOfNeighbourPx;

            ShininessVal.Text = PtRenderingParams.Instance.Shininess.ToString();
            SpecStrength.Value = PtRenderingParams.Instance.SpecularStrength;

            var col = PtRenderingParams.Instance.SingleColor;
            SingleColor.SelectedColor = System.Windows.Media.Color.FromScRgb(col.a, col.r, col.g, col.b);
            SSAOStrength.IsEnabled = PtRenderingParams.Instance.CalcSSAO;

            if (PtRenderingParams.Instance.ColorMode != PointCloud.Common.ColorMode.Single)
                SingleColor.IsEnabled = false;
            else
                SingleColor.IsEnabled = true;

            InnerGrid.IsEnabled = false;
        }

        #region UI Handler

        private void SSAOCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PtRenderingParams.Instance.CalcSSAO = !PtRenderingParams.Instance.CalcSSAO;
            SSAOStrength.IsEnabled = PtRenderingParams.Instance.CalcSSAO;
        }

        #region ssao strength
        private void SSAOStrength_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            _ssaoStrengthDragStarted = true;
        }

        private void SSAOStrength_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            PtRenderingParams.Instance.SSAOStrength = (float)((Slider)sender).Value;
            _ssaoStrengthDragStarted = false;
        }

        private void SSAOStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            if (_ssaoStrengthDragStarted) return;
            PtRenderingParams.Instance.SSAOStrength = (float)e.NewValue;
        }
        #endregion

        #region edl strength
        private void EDLStrength_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            _edlStrengthDragStarted = true;
        }

        private void EDLStrength_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            PtRenderingParams.Instance.EdlStrength = (float)((Slider)sender).Value;
            _edlStrengthDragStarted = false;
        }

        private void EDLStrengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            EDLStrengthVal.Content = e.NewValue.ToString("0.000");

            if (_edlStrengthDragStarted) return;
            PtRenderingParams.Instance.EdlStrength = (float)e.NewValue;
        }
        #endregion

        #region edl neighbor px

        private void EDLNeighbourPx_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            _edlNeighbourPxDragStarted = true;
        }

        private void EDLNeighbourPx_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            if (EDLNeighbourPxVal == null) return;

            EDLNeighbourPxVal.Content = ((Slider)sender).Value.ToString("0");
            PtRenderingParams.Instance.EdlNoOfNeighbourPx = (int)((Slider)sender).Value;

            _edlNeighbourPxDragStarted = false;
        }

        private void EDLNeighbourPxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            if (EDLNeighbourPxVal == null) return;

            EDLNeighbourPxVal.Content = e.NewValue.ToString("0");

            if (_edlNeighbourPxDragStarted) return;
            PtRenderingParams.Instance.EdlNoOfNeighbourPx = (int)e.NewValue;
        }

        #endregion       

        #region point size
        private void PtSize_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            //_ptSizeDragStarted = true;
        }

        private void PtSize_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (PtSizeVal == null) return;

            PtSizeVal.Content = ((Slider)sender).Value.ToString("0");
            PtRenderingParams.Instance.Size = (int)((Slider)sender).Value;
        }

        private void PtSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            if (PtSizeVal == null) return;
            PtSizeVal.Content = e.NewValue.ToString("0");

            // if (_ptSizeDragStarted) return;
            PtRenderingParams.Instance.Size = (int)e.NewValue;
        }

        #endregion

        #region specular strength
        private void SpecStrength_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            _specularStrengthPxDragStarted = true;
        }

        private void SpecStrength_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            PtRenderingParams.Instance.SpecularStrength = (float)((Slider)sender).Value;
            _specularStrengthPxDragStarted = false;
        }

        private void SpecStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            if (_specularStrengthPxDragStarted) return;
            PtRenderingParams.Instance.SpecularStrength = (float)e.NewValue;
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
            App?.SetOocLoaderMinProjSizeMod((float)MinProjSize.Value);

            _projSizeModDragStarted = false;
        }

        private void MinProjSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isAppInizialized)
                MinProjSizeVal.Content = MinProjSize.Value.ToString("0.00");

            if (_projSizeModDragStarted) return;
            App?.SetOocLoaderMinProjSizeMod((float)MinProjSize.Value);
        }
        #endregion

        private void SingleColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            var col = e.NewValue.Value;

            PtRenderingParams.Instance.SingleColor = new float4(col.ScR, col.ScG, col.ScB, col.ScA);
        }

        private void Lighting_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;

            PtRenderingParams.Instance.Lighting = (Lighting)e.AddedItems[0];

            if (PtRenderingParams.Instance.Lighting == PointCloud.Common.Lighting.SsaoOnly || PtRenderingParams.Instance.Lighting == PointCloud.Common.Lighting.Unlit)
            {
                SSAOCheckbox.IsEnabled = false;
                SSAOStrengthLabel.IsEnabled = false;
                SSAOStrength.IsEnabled = false;
            }
            else
            {
                SSAOCheckbox.IsEnabled = true;
                SSAOStrengthLabel.IsEnabled = true;
                SSAOStrength.IsEnabled = PtRenderingParams.Instance.CalcSSAO;
            }

            if (PtRenderingParams.Instance.Lighting == PointCloud.Common.Lighting.SsaoOnly)
                SSAOCheckbox.IsChecked = PtRenderingParams.Instance.CalcSSAO = true;
            if (PtRenderingParams.Instance.Lighting == PointCloud.Common.Lighting.Unlit)
                SSAOCheckbox.IsChecked = PtRenderingParams.Instance.CalcSSAO = false;

            if (PtRenderingParams.Instance.Lighting != PointCloud.Common.Lighting.BlinnPhong)
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

            if (PtRenderingParams.Instance.Lighting != PointCloud.Common.Lighting.Edl)
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
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            PtRenderingParams.Instance.Shape = (PointShape)e.AddedItems[0];
        }

        private void PtSizeMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            PtRenderingParams.Instance.PtMode = (PointSizeMode)e.AddedItems[0];
        }

        private void ColorMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!_isAppInizialized || !App.IsSceneLoaded) return;
            PtRenderingParams.Instance.ColorMode = (ColorMode)e.AddedItems[0];

            if (PtRenderingParams.Instance.ColorMode != PointCloud.Common.ColorMode.Single)
                SingleColor.IsEnabled = false;
            else
                SingleColor.IsEnabled = true;
        }

        private void LoadFile_Button_Click(object sender, RoutedEventArgs e)
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

                if (App != null)
                {
                    App.ClosingRequested = true;
                    SpinWait.SpinUntil(() => App.ReadyToLoadNewFile); //End of frame                    
                    App.CloseGameWindow();
                    SpinWait.SpinUntil(() => !App.IsAlive);
                    AssetStorage.UnRegisterAllAssetProviders();
                    GC.Collect();
                }

                _ = int.TryParse(PtThreshold.Text, out int th);

                CreateApp(path, th);
                RunApp();

                MinProjSize.Value = App.GetOocLoaderMinProjSizeMod();
                MinProjSizeVal.Content = MinProjSize.Value.ToString("0.00");

                if (App.GetOocLoaderRootNode() != null) //if RootNode == null no scene was ever initialized
                {
                    App.DeletePointCloud();
                    SpinWait.SpinUntil(() => App.ReadyToLoadNewFile && App.GetOocLoaderWasSceneUpdated() && _isAppInizialized);
                }

                PtRenderingParams.Instance.PathToOocFile = path;
                App.ResetCamera();
                App.LoadPointCloudFromFile();
                InnerGrid.IsEnabled = true;
                ShowOctants_Button.IsEnabled = true;
                ShowOctants_Img.Source = new BitmapImage(new Uri("Assets/octants.png", UriKind.Relative));
                inactiveBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void ResetCam_Button_Click(object sender, RoutedEventArgs e)
        {
            App?.ResetCamera();
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
                    PtThreshold.Text = App.GetOocLoaderPointThreshold().ToString();
                    return;
                }
                App.SetOocLoaderPointThreshold(ptThreshold);
            }
            else
            {
                PtThreshold.Text = App.GetOocLoaderPointThreshold().ToString();
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
                    ShininessVal.Text = PtRenderingParams.Instance.Shininess.ToString();
                    return;
                }

                PtRenderingParams.Instance.Shininess = shininess;
            }
            else
                ShininessVal.Text = PtRenderingParams.Instance.Shininess.ToString();
        }

        private void ShininessVal_LostFocus(object sender, RoutedEventArgs e)
        {
            ShininessVal.Text = PtRenderingParams.Instance.Shininess.ToString();
        }

        private void VisPoints_LostFocus(object sender, RoutedEventArgs e)
        {
            PtThreshold.Text = App.GetOocLoaderPointThreshold().ToString();
        }

        private void ShowOctants_Button_Click(object sender, RoutedEventArgs e)
        {
            while (!App.ReadyToLoadNewFile || !App.GetOocLoaderWasSceneUpdated() || !App.IsSceneLoaded)
                continue;

            if (!_areOctantsShown)
            {
                App.DoShowOctants = true;
                _areOctantsShown = true;
                ShowOctants_Img.Source = new BitmapImage(new Uri("Assets/octants_on.png", UriKind.Relative));
            }
            else
            {
                App.DeleteOctants();
                _areOctantsShown = false;
                ShowOctants_Img.Source = new BitmapImage(new Uri("Assets/octants.png", UriKind.Relative));
            }
        }

        #endregion

        private static bool IsTextAllowed(string text)
        {
            return !numRegex.IsMatch(text);
        }

        private void RunApp()
        {
            InnerGrid.IsEnabled = false;
            _isAppInizialized = false;

            _fusTask = new Task(() =>
            {
                Thread.CurrentThread.Name = "FusAppRunner";
                App.Run();
            });

            _fusTask.Start();

            SpinWait.SpinUntil(() => App != null && App.IsInitialized);

            Closed += (s, e) => App?.CloseGameWindow();

            _isAppInizialized = true;
            InnerGrid.IsEnabled = true;
        }

        private void CreateApp(string pathToFile, int th)
        {
            // Inject Fusee.Engine.Base InjectMe dependencies
            IO.IOImp = new IOImp();

            var fap = new FileAssetProvider("Assets");
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

            var ptType = AppSetupHelper.GetPtType(pathToFile);
            var ptEnumName = Enum.GetName(typeof(PointType), ptType);

            var genericType = Type.GetType("Fusee.PointCloud.PointAccessorCollections." + ptEnumName + ", " + "Fusee.PointCloud.PointAccessorCollections");

            var objectType = typeof(PcRendering<>);
            var objWithGenType = objectType.MakeGenericType(genericType);

            AppSetup.DoSetup(out App, AppSetupHelper.GetPtType(pathToFile), th, pathToFile);
            App.UseWPF = true;

            //Inject Fusee.Engine InjectMe dependencies(hard coded)
            System.Drawing.Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            App.CanvasImplementor = new Engine.Imp.Graphics.Desktop.RenderCanvasImp(appIcon, true);
            App.ContextImplementor = new Engine.Imp.Graphics.Desktop.RenderContextImp(App.CanvasImplementor);
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(App.CanvasImplementor));
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(App.CanvasImplementor));
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.WindowsSpaceMouseDriverImp(App.CanvasImplementor));

            App.InitCanvas();
        }
    }
}