using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Examples.PointCloudPotree2.Core;
using Fusee.PointCloud.Common;
using Fusee.Serialization;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace Fusee.Examples.PointCloudPotree2.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Core.PointCloudPotree2 app;
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

            InnerGrid.IsEnabled = false;
        }

        #region UI Handler

        #region edl strength
        private void EDLStrength_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (app == null || !app.IsInitialized) return;
            _edlStrengthDragStarted = true;
        }

        private void EDLStrength_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (app == null || !app.IsInitialized) return;
            PtRenderingParams.Instance.EdlStrength = (float)((Slider)sender).Value;
            _edlStrengthDragStarted = false;
        }

        private void EDLStrengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (app == null || !app.IsInitialized) return;
            EDLStrengthVal.Content = e.NewValue.ToString("0.00");

            if (_edlStrengthDragStarted) return;
            PtRenderingParams.Instance.EdlStrength = (float)e.NewValue;
        }
        #endregion

        #region edl neighbor px

        private void EDLNeighbourPx_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (app == null || !app.IsInitialized) return;
            _edlNeighbourPxDragStarted = true;
        }

        private void EDLNeighbourPx_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (app == null || !app.IsInitialized) return;
            if (EDLNeighbourPxVal == null) return;

            EDLNeighbourPxVal.Content = ((Slider)sender).Value.ToString("0");
            PtRenderingParams.Instance.EdlNoOfNeighbourPx = (int)((Slider)sender).Value;

            _edlNeighbourPxDragStarted = false;
        }

        private void EDLNeighbourPxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (app == null || !app.IsInitialized) return;
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
            if (app == null || !app.IsInitialized) return;
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
            if (app == null || !app.IsInitialized) return;
            MinProjSizeVal.Content = MinProjSize.Value.ToString("0.0000");
            PtRenderingParams.Instance.ProjectedSizeModifier = (float)MinProjSize.Value;
            _projSizeModDragStarted = false;
        }

        private void MinProjSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (app == null || !app.IsInitialized) return;
            MinProjSizeVal.Content = MinProjSize.Value.ToString("0.0000");

            if (_projSizeModDragStarted) return;
            PtRenderingParams.Instance.ProjectedSizeModifier = (float)MinProjSize.Value;
        }
        #endregion

        private void PtShape_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (app == null || !app.IsInitialized) return;
            PtRenderingParams.Instance.Shape = (PointShape)e.AddedItems[0];
        }

        private void PtSizeMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (app == null || !app.IsInitialized) return;
            PtRenderingParams.Instance.PtMode = (PointSizeMode)e.AddedItems[0];
        }

        private void ColorMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (app == null || !app.IsInitialized) return;
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
                if (!ofd.SafeFileName.Contains("metadata.json"))
                {
                    MessageBox.Show("Invalid file selected", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                fullPath = ofd.FileName;
                PtRenderingParams.Instance.PathToOocFile = fullPath.Replace(ofd.SafeFileName, "");

                if (app != null)
                {
                    app.ClosingRequested = true;
                    SpinWait.SpinUntil(() => app.ReadyToLoadNewFile); //End of frame
                    app.CloseGameWindow();
                    SpinWait.SpinUntil(() => !app.IsAlive);
                    AssetStorage.UnRegisterAllAssetProviders();
                    GC.Collect();
                }

                CreateApp();
                RunApp();

                MinProjSize.Value = PtRenderingParams.Instance.ProjectedSizeModifier;
                MinProjSizeVal.Content = MinProjSize.Value.ToString("0.0000");

                SpinWait.SpinUntil(() => app.ReadyToLoadNewFile && app.IsInitialized);

                app.ResetCamera();

                InnerGrid.IsEnabled = true;
                //ShowOctants_Button.IsEnabled = true;
                //ShowOctants_Img.Source = new BitmapImage(new Uri("Assets/octants.png", UriKind.Relative));
                inactiveBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void ResetCam_Button_Click(object sender, RoutedEventArgs e)
        {
            app?.ResetCamera();
        }

        private void VisPoints_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (app == null || !app.IsInitialized) return;
            e.Handled = !IsTextAllowed(PtThreshold.Text);

            if (!e.Handled)
            {
                if (!int.TryParse(PtThreshold.Text, out var ptThreshold)) return;
                if (ptThreshold < 0)
                {
                    PtThreshold.Text = PtRenderingParams.Instance.PointThreshold.ToString();
                    return;
                }
                PtRenderingParams.Instance.PointThreshold = ptThreshold;
            }
            else
            {
                PtThreshold.Text = PtRenderingParams.Instance.PointThreshold.ToString();
            }
        }

        private void VisPoints_LostFocus(object sender, RoutedEventArgs e)
        {
            PtThreshold.Text = PtRenderingParams.Instance.PointThreshold.ToString();
        }

        private void ShowOctants_Button_Click(object sender, RoutedEventArgs e)
        {
            while (!app.ReadyToLoadNewFile)
                continue;

            if (!_areOctantsShown)
            {
                //app.DoShowOctants = true;
                _areOctantsShown = true;
                ShowOctants_Img.Source = new BitmapImage(new Uri("Assets/octants_on.png", UriKind.Relative));
            }
            else
            {
                //app.DoShowOctants = false;
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

            _fusTask = new Task(() =>
            {
                Thread.CurrentThread.Name = "FusAppRunner";
                app.Run();
            });

            _fusTask.Start();

            SpinWait.SpinUntil(() => app != null && app.IsInitialized);

            Closed += (s, e) => app?.CloseGameWindow();

            InnerGrid.IsEnabled = true;
        }

        private void CreateApp()
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

            app = new Core.PointCloudPotree2();

            PtShape.SelectedValue = PtRenderingParams.Instance.Shape;
            PtSizeMode.SelectedValue = PtRenderingParams.Instance.PtMode;
            ColorMode.SelectedValue = PtRenderingParams.Instance.ColorMode;

            PtSize.Value = PtRenderingParams.Instance.Size;
            PtSizeVal.Content = PtRenderingParams.Instance.Size;
            PtThreshold.Text = PtRenderingParams.Instance.PointThreshold.ToString();

            EDLStrength.Value = PtRenderingParams.Instance.EdlStrength;
            EDLStrengthVal.Content = PtRenderingParams.Instance.EdlStrength.ToString("0.00");
            EDLNeighbourPx.Value = PtRenderingParams.Instance.EdlNoOfNeighbourPx;
            EDLNeighbourPxVal.Content = PtRenderingParams.Instance.EdlNoOfNeighbourPx;
            MinProjSize.Value = PtRenderingParams.Instance.ProjectedSizeModifier;
            MinProjSizeVal.Content = PtRenderingParams.Instance.ProjectedSizeModifier;

            app.UseWPF = true;

            //Inject Fusee.Engine InjectMe dependencies(hard coded)
            var icon = AssetStorage.Get<ImageData>("FuseeIconTop32.png");
            app.CanvasImplementor = new Engine.Imp.Graphics.Desktop.RenderCanvasImp(icon, true);
            app.ContextImplementor = new Engine.Imp.Graphics.Desktop.RenderContextImp(app.CanvasImplementor);
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(app.CanvasImplementor));
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(app.CanvasImplementor));
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.WindowsSpaceMouseDriverImp(app.CanvasImplementor));

            app.InitApp();
        }
    }
}