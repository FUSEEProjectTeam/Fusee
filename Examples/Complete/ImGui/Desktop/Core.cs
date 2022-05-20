using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.ImGuiDesktop;
using Fusee.Math.Core;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.Examples.FuseeImGui.Desktop
{
    [FuseeApplication(Name = "FUSEE ImGui Example",
        Description = "A very simple example how to use ImGui within a Fusee application.")]
    public class Core : RenderCanvas
    {

#region StaticBindingVars

        private static bool _dockspaceOpen = true;
        private static bool _isMouseInsideFuControl;

        private CoreControl _fuControl;

        private Vector4 _rocketColor = Vector4.UnitW; // alpha 255
        private bool _colorPickerOpen;

        #endregion


        private void Load()
        {
            SetImGuiDesign();

            _fuControl = new CoreControl(RC);
            _fuControl.Init();

            // reload last used cfg
            if(File.Exists(Path.Combine("Assets/MyImGuiSettings.ini")))
            {
                ImGui.LoadIniSettingsFromDisk(Path.Combine("Assets/MyImGuiSettings.ini"));
            }
        }

        public override async Task InitAsync()
        {
            Load();
            await base.InitAsync();

        }

        public override void Update()
        {
            _fuControl.Update(_isMouseInsideFuControl);
        }

        public override void Resize(ResizeEventArgs e)
        {
            _fuControl.UpdateOriginalGameWindowDimensions(e.Width, e.Height);

        }

        public override void RenderAFrame()
        {
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

            ImGui.Image(_fuControl.RenderToTexture((int)size.X, (int)size.Y), fuseeViewportSize,
                new Vector2(0, 1),
                new Vector2(1, 0));

            // check if mouse is inside window, if true, accept update() inputs
            _isMouseInsideFuControl = ImGui.IsItemHovered();

            ImGui.EndChild();
            ImGui.End();

            DrawGUI();
        }


        internal void DrawGUI()
        {
            ImGui.Begin("Sidebar");

            ImGui.BeginGroup();
            ImGui.Text("Fusee Simple Example");
            ImGui.Text($"Application average {1000.0f / ImGui.GetIO().Framerate:0.00} ms/frame ({ImGui.GetIO().Framerate:0} FPS)");
            ImGui.EndGroup();
          
            ImGui.Spacing();

            var allSceneElements = _fuControl.GetSceneGraphRePresentation();

            ImGui.BeginTable("SceneGraph", 3, ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingMask | ImGuiTableFlags.Borders);

            ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.DefaultSort, 50);
            ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.DefaultSort, 100);
            ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.DefaultSort, 500);

            ImGui.TableHeadersRow();

            foreach (var element in allSceneElements)
            {                
                ImGui.TableNextColumn();
                ImGui.Text(element.Name);
                ImGui.TableNextColumn();
                ImGui.Text(element.Type.FullName);
                ImGui.TableNextColumn();
                ImGui.Text(element.Value);
                ImGui.TableNextRow();
            }
            ImGui.EndTable();

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Separator();


            ImGui.BeginGroup();

            ImGui.Text("Color Rocket");

            if (ImGui.ColorButton("Toggle Color Picker", _rocketColor, ImGuiColorEditFlags.DefaultOptions, Vector2.One * 50))
            {
                _colorPickerOpen = !_colorPickerOpen;
            }
            if (_colorPickerOpen)
            {
                ImGui.Begin("Color Picker", ref _colorPickerOpen, ImGuiWindowFlags.AlwaysAutoResize);
                ImGui.ColorPicker4("Color", ref _rocketColor);
                ImGui.End();

                _fuControl.ColorRocket(_rocketColor.ToFuseeVector());
            }

            ImGui.EndGroup();

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Separator();


            ImGui.BeginGroup();
            ImGui.Spacing();

            if (ImGui.Button("Load layout from *.ini file"))
            {
                ImGui.LoadIniSettingsFromDisk(Path.Combine("Assets/MyImGuiSettings.ini"));
            }

            if (ImGui.Button("Save layout to *.ini file"))
            {
                ImGui.SaveIniSettingsToDisk(Path.Combine("Assets/MyImGuiSettings.ini"));
            }

            ImGui.EndGroup();

            ImGui.End();


        }

     

        internal static void DrawMainMenuBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Menu"))
                {
                    if (ImGui.MenuItem("Exit"))
                    {
                        Environment.Exit(0);
                    }
                    ImGui.EndMenu();
                }
            }
            ImGui.EndMainMenuBar();
        }

        /// <summary>
        /// Place all design/styles inside this method
        /// </summary>
        internal static void SetImGuiDesign()
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

            colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.70f, 0.70f, 0.70f, 0.70f);
            colors[(int)ImGuiCol.TableBorderStrong] = Vector4.Zero;
            colors[(int)ImGuiCol.TableRowBg] = new Vector4(0.71f, 0.78f, 0.69f, 0.40f);
            colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(0.725f, 0.805f, 0.702f, 1.00f);
            colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.66f, 0.66f, 0.66f, 1.00f);
        }
    }
}
