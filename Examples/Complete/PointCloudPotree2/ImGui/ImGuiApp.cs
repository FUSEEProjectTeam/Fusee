using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Examples.PointCloudPotree2.Core;
using Fusee.ImGuiImp.Desktop;
using Fusee.ImGuiImp.Desktop.Templates;
using Fusee.PointCloud.Common;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace Fusee.Examples.PointCloudPotree2.Gui
{
    [FuseeApplication(Name = "FUSEE ImGui Example",
        Description = "A very simple example how to use ImGui within a Fusee application.")]
    public class ImGuiApp : RenderCanvas
    {
        #region StaticBindingVars

        private static bool _dockspaceOpen = true;

        private static int _threshold = 1000000;
        private static float _fuseeViewportMinProj;

        private static int _edlNeighbour = 0;
        private static float _edlStrength = .5f;

        private static int _currentPtShape;
        private static int _currentPtSizeMethod;
        private static int _ptSize = 1;

        private static int _currentColorMode = 1;

        private static Vector4 _ptColor = new(0, 0, 0, 1);
        private static bool _colorPickerOpen;

        private static bool _isMouseInsideFuControl;

        private bool _spwanOpenFilePopup = false;
        private bool _wantsToShutdown;


        private PointCloudRenderingControl _fuControl;
        private ImGuiFilePicker _picker;

        #endregion

        public override async Task InitAsync()
        {
            SetImGuiDesign();

            _fuControl = new PointCloudRenderingControl(RC);
            ApplicationIsShuttingDown += OnShuttingDown;
            EndOfFrame += _fuControl.OnLoadNewFile;
            _fuControl.Init();
            await base.InitAsync();

            _picker = new ImGuiFilePicker(new DirectoryInfo(Environment.CurrentDirectory), ".json");
            _picker.OnPicked += (s, file) =>
            {
                if (file == null || !file.Exists) return;

                PointRenderingParams.Instance.PathToOocFile = file.DirectoryName;

                if (_fuControl != null)
                {
                    _fuControl.RequestedNewFile = true;
                    _fuControl.UpdateOriginalGameWindowDimensions(Width, Height);
                    _fuControl.ResetCamera();
                    _currentColorMode = (int)PointRenderingParams.Instance.ColorMode;
                }
            };
        }

        public override void Update()
        {
            _fuControl?.Update(_isMouseInsideFuControl);
        }

        public override void Resize(ResizeEventArgs e)
        {
            _fuControl?.UpdateOriginalGameWindowDimensions(e.Width, e.Height);
        }

        public override void RenderAFrame()
        {
            // Enable Dockspace
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            // Set Window flags for Dockspace
            var wndDockspaceFlags =
                    ImGuiWindowFlags.NoDocking
                    | ImGuiWindowFlags.NoTitleBar
                    | ImGuiWindowFlags.NoCollapse
                    | ImGuiWindowFlags.NoResize
                    | ImGuiWindowFlags.NoMove
                    | ImGuiWindowFlags.NoBringToFrontOnFocus
                    | ImGuiWindowFlags.NoFocusOnAppearing;

            var dockspaceFlags = ImGuiDockNodeFlags.PassthruCentralNode /*| ImGuiDockNodeFlags.AutoHideTabBar*/;

            var viewport = ImGui.GetMainViewport();

            // Set the parent window's position, size, and viewport to match that of the main viewport. This is so the parent window
            // completely covers the main viewport, giving it a "full-screen" feel.
            ImGui.SetNextWindowPos(viewport.WorkPos);
            ImGui.SetNextWindowSize(viewport.WorkSize);
            ImGui.SetNextWindowViewport(viewport.ID);

            // Set the parent window's styles to match that of the main viewport:
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f); // No corner rounding on the window
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f); // No border around the window
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

            // Create Dockspace
            ImGui.Begin("DockSpace", ref _dockspaceOpen, wndDockspaceFlags);

            var dockspace_id = ImGui.GetID("DockSpace");
            ImGui.DockSpace(dockspace_id, Vector2.Zero, dockspaceFlags);

            ImGui.PopStyleVar(3);

            // Titlebar
            DrawMainMenuBar();

            // Fusee Viewport
            ImGui.Begin("Viewport",
              ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse);

            var parentMin = ImGui.GetWindowContentRegionMin();
            var parentMax = ImGui.GetWindowContentRegionMax();
            var size = parentMax - parentMin;

            // Using a Child allow to fill all the space of the window.
            // It also allows customization
            ImGui.BeginChild("GameRender", size, true, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);

            var fuseeViewportMin = ImGui.GetWindowContentRegionMin();
            var fuseeViewportMax = ImGui.GetWindowContentRegionMax();
            var fuseeViewportSize = fuseeViewportMax - fuseeViewportMin;
            var fuseeViewportPos = ImGui.GetWindowPos();

            var hndl = _fuControl.RenderToTexture((int)fuseeViewportSize.X, (int)fuseeViewportSize.Y);


            ImGui.Image(hndl, fuseeViewportSize,
                new Vector2(0, 1),
                new Vector2(1, 0));

            // check if mouse is inside window, if true, accept update() inputs
            _isMouseInsideFuControl = ImGui.IsItemHovered();

            ImGui.EndChild();
            ImGui.End();

            Draw();
            DrawFilePickerDialog();

            if (_wantsToShutdown)
                CloseGameWindow();
        }

        private void Draw()
        {
            ImGui.Begin("Settings");
            ImGui.Text("Fusee PointCloud Rendering");
            ImGui.Text($"Application average {1000.0f / ImGui.GetIO().Framerate:0.00} ms/frame ({ImGui.GetIO().Framerate:0} FPS)");
            ImGui.NewLine();
            if (ImGui.Button("Open File"))
            {
                _spwanOpenFilePopup = true;
            }
            ImGui.SameLine();

            if (ImGui.Button("Reset Camera"))
            {
                _fuControl.ResetCamera();
            }
            ImGui.SameLine();

            if (ImGui.Button("Show Octree"))
            {
                // not implemented
            }

            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Visibility");
            ImGui.InputInt("Point threshold", ref _threshold, 1000, 10000);
            ImGui.SliderFloat("Min. Projection Size Modifier", ref _fuseeViewportMinProj, 0f, 1f);


            PointRenderingParams.Instance.PointThreshold = _threshold;
            PointRenderingParams.Instance.ProjectedSizeModifier = _fuseeViewportMinProj;

            ImGui.EndGroup();


            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Lighting");
            ImGui.SliderInt("EDL Neighbor Px", ref _edlNeighbour, 0, 5);
            ImGui.SliderFloat("EDL Strength", ref _edlStrength, 0.0f, 5f);

            PointRenderingParams.Instance.EdlStrength = _edlStrength;
            PointRenderingParams.Instance.EdlNoOfNeighbourPx = _edlNeighbour;

            ImGui.EndGroup();

            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Point Shape");
            ImGui.Combo("PointShape", ref _currentPtShape, new string[] { "Paraboloid", "Rect", "Circle" }, 3);

            PointRenderingParams.Instance.Shape = _currentPtShape switch
            {
                0 => PointShape.Paraboloid,
                1 => PointShape.Rect,
                2 => PointShape.Circle,
                _ => PointShape.Paraboloid
            };

            ImGui.EndGroup();

            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Point Size Method");
            ImGui.Combo("Point Size Method", ref _currentPtSizeMethod, new string[] { "FixedPixelSize", "FixedWorldSize" }, 2);
            ImGui.SliderInt("Point Size", ref _ptSize, 1, 20);

            PointRenderingParams.Instance.Size = _ptSize;
            PointRenderingParams.Instance.PtMode = _currentPtSizeMethod switch
            {
                0 => PointSizeMode.FixedPixelSize,
                1 => PointSizeMode.FixedWorldSize,
                _ => PointSizeMode.FixedPixelSize
            };

            ImGui.EndGroup();

            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Color Mode");

            ImGui.Combo("Color mode", ref _currentColorMode, new string[] { "BaseColor", "VertexColor0", "VertexColor1", "VertexColor2" }, 4);

            PointRenderingParams.Instance.ColorMode = _currentColorMode switch
            {
                0 => ColorMode.BaseColor,
                1 => ColorMode.VertexColor0,
                2 => ColorMode.VertexColor1,
                3 => ColorMode.VertexColor2,
                _ => ColorMode.VertexColor0
            };

            if (_currentColorMode == (int)ColorMode.BaseColor)
            {
                ImGui.Spacing();
                ImGui.BeginGroup();
                ImGui.Text("Color picker");

                if (ImGui.ColorButton("Toggle Color Picker", _ptColor, ImGuiColorEditFlags.DefaultOptions, Vector2.One * 50))
                {
                    _colorPickerOpen = !_colorPickerOpen;
                }
                if (_colorPickerOpen)
                {
                    ImGui.Begin("Color Picker", ref _colorPickerOpen, ImGuiWindowFlags.AlwaysAutoResize);
                    ImGui.ColorPicker4("Color", ref _ptColor);
                    ImGui.End();

                    PointRenderingParams.Instance.ColorPassEf.SurfaceInput.Albedo = _ptColor.ToFuseeVector();
                }
                ImGui.EndGroup();
            }

            ImGui.EndGroup();
            ImGui.End();
        }

        private void DrawMainMenuBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Menu"))
                {
                    if (ImGui.MenuItem("Open"))
                    {
                        _spwanOpenFilePopup = true;
                    }
                    if (ImGui.MenuItem("Exit"))
                    {
                        _wantsToShutdown = true;
                    }
                    ImGui.EndMenu();
                }
            }
            ImGui.EndMainMenuBar();
        }

        private void DrawFilePickerDialog()
        {
            _picker.Draw(ref _spwanOpenFilePopup);
        }

        private void OnShuttingDown(object sender, EventArgs e)
        {
            _fuControl.ClosingRequested = true;
        }

        /// <summary>
        /// Place all design/styles inside this method
        /// </summary>
        private static void SetImGuiDesign()
        {
            var style = ImGui.GetStyle();
            var colors = style.Colors;

            style.WindowRounding = 2.0f;             // Radius of window corners rounding. Set to 0.0f to have rectangular windows
            style.ScrollbarRounding = 3.0f;             // Radius of grab corners rounding for scrollbar
            style.GrabRounding = 2.0f;             // Radius of grabs corners rounding. Set to 0.0f to have rectangular slider grabs.
            style.AntiAliasedLines = true;
            style.AntiAliasedFill = true;
            style.WindowRounding = 2;
            style.ChildRounding = 2;
            style.ScrollbarSize = 16;
            style.ScrollbarRounding = 3;
            style.GrabRounding = 2;
            style.ItemSpacing.X = 10;
            style.ItemSpacing.Y = 4;
            style.IndentSpacing = 22;
            style.FramePadding.X = 6;
            style.FramePadding.Y = 4;
            style.Alpha = 1.0f;
            style.FrameRounding = 3.0f;


            colors[(int)ImGuiCol.Text] = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.60f, 0.60f, 0.60f, 1.00f);
            colors[(int)ImGuiCol.WindowBg] = new Vector4(0.86f, 0.86f, 0.86f, 1.00f);
            //color(int)s[ImGuiCol_ChildWindowBg]         = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.ChildBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.PopupBg] = new Vector4(0.93f, 0.93f, 0.93f, 0.98f);
            colors[(int)ImGuiCol.Border] = new Vector4(0.71f, 0.71f, 0.71f, 0.08f);
            colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.04f);
            colors[(int)ImGuiCol.FrameBg] = new Vector4(0.71f, 0.71f, 0.71f, 0.55f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.94f, 0.94f, 0.94f, 0.55f);
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.71f, 0.78f, 0.69f, 0.98f);
            colors[(int)ImGuiCol.TitleBg] = new Vector4(0.85f, 0.85f, 0.85f, 1.00f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.82f, 0.78f, 0.78f, 0.51f);
            colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.78f, 0.78f, 0.78f, 1.00f);
            colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.86f, 0.86f, 0.86f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.20f, 0.25f, 0.30f, 0.61f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.90f, 0.90f, 0.90f, 0.30f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.92f, 0.92f, 0.92f, 0.78f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            colors[(int)ImGuiCol.CheckMark] = new Vector4(0.184f, 0.407f, 0.193f, 1.00f);
            colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.26f, 0.59f, 0.98f, 0.78f);
            colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);
            colors[(int)ImGuiCol.Button] = new Vector4(0.71f, 0.78f, 0.69f, 0.40f);
            colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.725f, 0.805f, 0.702f, 1.00f);
            colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.793f, 0.900f, 0.836f, 1.00f);
            colors[(int)ImGuiCol.Header] = new Vector4(0.71f, 0.78f, 0.69f, 0.31f);
            colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.71f, 0.78f, 0.69f, 0.80f);
            colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.71f, 0.78f, 0.69f, 1.00f);
            colors[(int)ImGuiCol.Tab] = new Vector4(0.39f, 0.39f, 0.39f, 1.00f);
            colors[(int)ImGuiCol.TabHovered] = new Vector4(0.26f, 0.59f, 0.98f, 0.78f);
            colors[(int)ImGuiCol.TabActive] = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);
            colors[(int)ImGuiCol.Separator] = new Vector4(0.39f, 0.39f, 0.39f, 1.00f);
            colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.14f, 0.44f, 0.80f, 0.78f);
            colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.14f, 0.44f, 0.80f, 1.00f);
            colors[(int)ImGuiCol.ResizeGrip] = new Vector4(1.00f, 1.00f, 1.00f, 0.00f);
            colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.26f, 0.59f, 0.98f, 0.45f);
            colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.26f, 0.59f, 0.98f, 0.78f);
            colors[(int)ImGuiCol.PlotLines] = new Vector4(0.39f, 0.39f, 0.39f, 1.00f);
            colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(1.00f, 0.43f, 0.35f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.90f, 0.70f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(1.00f, 0.60f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.26f, 0.59f, 0.98f, 0.35f);
            //colors[(int)ImGuiCol.ModalWindowDarkening] = new Vector4(0.20f, 0.20f, 0.20f, 0.35f);
            colors[(int)ImGuiCol.DragDropTarget] = new Vector4(0.26f, 0.59f, 0.98f, 0.95f);
            colors[(int)ImGuiCol.NavHighlight] = colors[(int)ImGuiCol.HeaderHovered];
            colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(0.70f, 0.70f, 0.70f, 0.70f);
        }
    }
}