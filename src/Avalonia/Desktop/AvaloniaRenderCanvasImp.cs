using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Imp.Graphics.Desktop;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;

namespace Fusee.Avalonia.Desktop
{
    public class AvaloniaRenderCanvasImp : RenderCanvasImpBase, IRenderCanvasImp
    {
        private readonly FuseeWindowControl _control;
        /// <summary>
        /// Used to track delta time
        /// </summary>
        private readonly Stopwatch _sw = new();

        public AvaloniaRenderCanvasImp(FuseeWindowControl control) => (_control) = (control);

        public IWindowHandle WindowHandle => throw new NotImplementedException("WindowHandlePtr hidden within Avalonia!");

        public int Width
        {
            get => _control.GetPixelSize().Width;
            set => Diagnostics.Warn("Do not try to set the width! This property is managed by Avalonia!");
        }
        public int Height
        {
            get => _control.GetPixelSize().Height;
            set => Diagnostics.Warn("Do not try to set the height! This property is managed by Avalonia!");
        }
        public string Caption
        {
            // TODO(mr): implement properly
            get => _control?.Parent?.Name;
            set { }
        }

        public float DeltaTime { get; set; }
        public float DeltaTimeUpdate { get; set; }

        // TODO(mr): implement vsync & fullscreen
        public bool VerticalSync { get { return true; } set { } }
        public bool Fullscreen { get { return false; } set { } }

        public void CloseGameWindow()
        {
            // nothing to do, handled by Avalonia
        }

        public void OpenLink(string link)
        {
            if (link.StartsWith("http://") || link.StartsWith("https://"))
            {
                //UseShellExecute needs to be set to true in .net 3.0. See:https://github.com/dotnet/corefx/issues/33714
                ProcessStartInfo psi = new()
                {
                    FileName = link,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }

        public void Present()
        {
            // nothing to do, handled by Avalonia
        }

        public void Run()
        {
            DoInit();

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front); // needed because we flip the y-axis

            _control.OnRender += (s, e) =>
            {
                DeltaTimeUpdate = (float)(_sw.ElapsedMilliseconds / 1000.0);
                DoUpdate();
                DeltaTime = (float)(_sw.ElapsedMilliseconds / 1000.0);
                DoRender();

                _sw.Restart();

            };
        }

        public void SetCursor(CursorType cursorType)
        {
            Diagnostics.Warn("Do not try to set the cursor, this behavior is managed by Avalonia!");

        }

        public void SetWindowSize(int width, int height, int posx = -1, int posy = -1, bool borderHidden = false)
        {
            Diagnostics.Warn("Do not try to set the window size, this behavior is managed by Avalonia!");
        }

    }
}
