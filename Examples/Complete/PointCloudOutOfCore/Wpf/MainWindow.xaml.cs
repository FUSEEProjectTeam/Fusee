using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Examples.PointCloudOutOfCore.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.PotreeReader.V1;
using Fusee.Serialization;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace Fusee.Examples.PointCloudOutOfCore.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IPointCloudOutOfCore App;

        private bool _isAppInizialized = false;
        private bool _areOctantsShown;

        //private bool _ptSizeDragStarted;
        private bool _projSizeModDragStarted;
        private bool _edlStrengthDragStarted;
        private bool _edlNeighbourPxDragStarted;

        private Task _fusTask;

        private static readonly Regex numRegex = new("[^0-9.-]+");

        public MainWindow()
        {
            InitializeComponent();

            PtShape.SelectedValue = PtRenderingParams.Instance.Shape;
            PtSizeMode.SelectedValue = PtRenderingParams.Instance.PtMode;
            ColorMode.SelectedValue = PtRenderingParams.Instance.ColorMode;

            PtSize.Value = PtRenderingParams.Instance.Size;
            PtSizeVal.Content = PtSize.Value;

            EDLStrength.Value = PtRenderingParams.Instance.EdlStrength;
            EDLStrengthVal.Content = EDLStrength.Value.ToString("0.000");
            EDLNeighbourPx.Value = PtRenderingParams.Instance.EdlNoOfNeighbourPx;
            EDLNeighbourPxVal.Content = EDLNeighbourPx.Value;
            MinProjSize.Value = PtRenderingParams.Instance.ProjectedSizeModifier;
            MinProjSizeVal.Content = MinProjSize.Value;

            InnerGrid.IsEnabled = false;
        }

        #region UI Handler

        #region edl strength
        private void EDLStrength_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (!_isAppInizialized) return;
            _edlStrengthDragStarted = true;
        }

        private void EDLStrength_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!_isAppInizialized) return;
            PtRenderingParams.Instance.EdlStrength = (float)((Slider)sender).Value;
            _edlStrengthDragStarted = false;
        }

        private void EDLStrengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isAppInizialized) return;
            EDLStrengthVal.Content = e.NewValue.ToString("0.000");

            if (_edlStrengthDragStarted) return;
            PtRenderingParams.Instance.EdlStrength = (float)e.NewValue;
        }
        #endregion

        #region edl neighbor px

        private void EDLNeighbourPx_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (!_isAppInizialized) return;
            _edlNeighbourPxDragStarted = true;
        }

        private void EDLNeighbourPx_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!_isAppInizialized) return;
            if (EDLNeighbourPxVal == null) return;

            EDLNeighbourPxVal.Content = ((Slider)sender).Value.ToString("0");
            PtRenderingParams.Instance.EdlNoOfNeighbourPx = (int)((Slider)sender).Value;

            _edlNeighbourPxDragStarted = false;
        }

        private void EDLNeighbourPxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isAppInizialized) return;
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
            if (!_isAppInizialized) return;
            if (PtSizeVal == null) return;
            PtSizeVal.Content = e.NewValue.ToString("0");

            // if (_ptSizeDragStarted) return;
            PtRenderingParams.Instance.Size = (int)e.NewValue;
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

        private void PtShape_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isAppInizialized) return;
            PtRenderingParams.Instance.Shape = (PointShape)e.AddedItems[0];
        }

        private void PtSizeMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isAppInizialized) return;
            PtRenderingParams.Instance.PtMode = (PointSizeMode)e.AddedItems[0];
        }

        private void ColorMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isAppInizialized) return;
            PtRenderingParams.Instance.ColorMode = (PointColorMode)e.AddedItems[0];

            //ColorPicker is unavailable right now
            //if (PtRenderingParams.Instance.ColorMode != PointCloud.Common.ColorMode.Single)
            //    SingleColor.IsEnabled = false;
            //else
            //    SingleColor.IsEnabled = true;
        }

        private void LoadFile_Button_Click(object sender, RoutedEventArgs e)
        {
            string fullPath;
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
                PtRenderingParams.Instance.PathToOocFile = fullPath.Replace(ofd.SafeFileName, "");

                if (App != null)
                {
                    App.ClosingRequested = true;
                    SpinWait.SpinUntil(() => App.ReadyToLoadNewFile); //End of frame                    
                    App.CloseGameWindow();
                    SpinWait.SpinUntil(() => !App.IsAlive);
                    AssetStorage.UnRegisterAllAssetProviders();
                    GC.Collect();
                }

                CreateApp(PtRenderingParams.Instance.PathToOocFile);
                RunApp();

                MinProjSize.Value = App.GetOocLoaderMinProjSizeMod();
                MinProjSizeVal.Content = MinProjSize.Value.ToString("0.00");

                //TODO: add null/initialization check?
                App.DeletePointCloud();
                SpinWait.SpinUntil(() => App.ReadyToLoadNewFile && _isAppInizialized);

                App.ResetCamera();
                
                InnerGrid.IsEnabled = true;
                //ShowOctants_Button.IsEnabled = true;
                //ShowOctants_Img.Source = new BitmapImage(new Uri("Assets/octants.png", UriKind.Relative));
                inactiveBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void ResetCam_Button_Click(object sender, RoutedEventArgs e)
        {
            App?.ResetCamera();
        }

        private void VisPoints_TextChanged(object sender, TextChangedEventArgs e)
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

        private void VisPoints_LostFocus(object sender, RoutedEventArgs e)
        {
            PtThreshold.Text = App.GetOocLoaderPointThreshold().ToString();
        }

        private void ShowOctants_Button_Click(object sender, RoutedEventArgs e)
        {
            while (!App.ReadyToLoadNewFile)
                continue;

            if (!_areOctantsShown)
            {
                App.DoShowOctants = true;
                _areOctantsShown = true;
                ShowOctants_Img.Source = new BitmapImage(new Uri("Assets/octants_on.png", UriKind.Relative));
            }
            else
            {
                App.DoShowOctants = false;
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

        private void CreateApp(string pathToFile)
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
                        if (!Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)) return null;
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

            var ptType = ReadPotreeMetadata.GetPtTypeFromMetaJson(pathToFile);
            var ptEnumName = Enum.GetName(typeof(PointType), ptType);

            var genericType = Type.GetType("Fusee.PointCloud.Common." + ptEnumName + ", " + "Fusee.PointCloud.Common");

            var objectType = typeof(PointCloudOutOfCore<>);
            var objWithGenType = objectType.MakeGenericType(genericType);

            AppSetup.DoSetup(out App, ptType, pathToFile);
            App.UseWPF = true;

            //Inject Fusee.Engine InjectMe dependencies(hard coded)
            System.Drawing.Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            App.CanvasImplementor = new Engine.Imp.Graphics.Desktop.RenderCanvasImp(appIcon, true);
            App.ContextImplementor = new Engine.Imp.Graphics.Desktop.RenderContextImp(App.CanvasImplementor);
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(App.CanvasImplementor));
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(App.CanvasImplementor));
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.WindowsSpaceMouseDriverImp(App.CanvasImplementor));

            App.InitApp();
        }
    }
}