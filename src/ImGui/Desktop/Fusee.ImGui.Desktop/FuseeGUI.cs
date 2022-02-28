using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.DImGui.Desktop
{
    internal static class FuseeGUI
    {
        #region StaticBindingVars

        private static float _threshold;
        private static float _fuseeViewportMinProj;

        private static int _edlNeighbour;
        private static float _edlStrength;

        private static int _currentPtShape;
        private static int _currentPtSizeMethod;
        private static float _ptSize;

        private static Vector4 _ptColor;
        private static bool _colorPickerOpen;

        private static float _color;

        #endregion

        /// <summary>
        /// Viewport and docking windows are already drawn
        /// Place everything else inside this method
        /// </summary>
        internal static void DrawGUI()
        {
            ImGui.Begin("Settings");
            ImGui.Text("Fusee PointCloud Rendering");
            ImGui.Text($"Application average {1000.0f / ImGui.GetIO().Framerate:0.00} ms/frame ({ImGui.GetIO().Framerate:0} FPS)");
            ImGui.NewLine();
            ImGui.Button("Open File");
            ImGui.SameLine();
            ImGui.Button("Reset Camera");
            ImGui.SameLine();
            ImGui.Button("Show Octree");

            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Visibility");
            ImGui.InputFloat("Threshold", ref _threshold);
            ImGui.SliderFloat("Min. Projection Size Modifier", ref _fuseeViewportMinProj, 0f, 1f);
            ImGui.EndGroup();


            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Lighting");
            ImGui.SliderInt("EDL Neighbor Px", ref _edlNeighbour, 0, 5);
            ImGui.SliderFloat("EDL Strength", ref _edlStrength, -1f, 5f);
            ImGui.EndGroup();

            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Point Shape");
            ImGui.Combo("PointShape", ref _currentPtShape, new string[] { "Paraboloid", "Box", "Square" }, 3);
            ImGui.EndGroup();

            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Point Size Method");
            ImGui.Combo("Point Size Method", ref _currentPtSizeMethod, new string[] { "FixedPixelSize", "Adaptive", "Third" }, 3);
            ImGui.SliderFloat("Point Size", ref _ptSize, 0.2f, 20f);
            ImGui.EndGroup();

            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Color Mode");
            if (ImGui.ColorButton("Toggle Color Picker", _ptColor))
            {
                _colorPickerOpen = !_colorPickerOpen;

            }
            if (_colorPickerOpen)
            {
                ImGui.Begin("Color Picker", ref _colorPickerOpen, ImGuiWindowFlags.AlwaysAutoResize);
                ImGui.ColorPicker4("Color", ref _ptColor);
                ImGui.End();
            }

            ImGui.EndGroup();

            ImGui.BeginGroup();

            ImGui.SliderAngle("Colorpicker", ref _color);

            ImGui.EndGroup();

            ImGui.End();
        }

        /// <summary>
        /// Generate MainMenuBar inside this method
        /// </summary>
        internal static void DrawMainMenuBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Menu"))
                {
                    if (ImGui.MenuItem("Open"))
                    {

                    }
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

            var style = ImGuiNET.ImGui.GetStyle();
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
