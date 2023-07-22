using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Imp.Graphics.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using System;

namespace Fusee.ImGuiImp.Desktop
{
    struct UniformFieldInfo
    {
        public int Location;
        public string Name;
        public int Size;
        public ActiveUniformType Type;
    }

    /// <summary>
    /// Implementation of the cross-platform abstraction of the window handle.
    /// </summary>
    public class WindowHandle : IWindowHandle
    {
        /// <summary>
        /// The Window Handle as IntPtr
        /// </summary>
        public IntPtr Handle { get; internal set; }
    }

    public class ImGuiRenderCanvasImp : RenderCanvasImp
    {
        private static bool _initialized = false;

        private ImGuiController _controller;
        private bool _isShuttingDown;

        public ImGuiRenderCanvasImp(ImageData? icon = null, int width = 1280, int height = 720, int minWidth = 360, int minHeight = 640) : base(icon, true, width, height, minWidth, minHeight)
        {
            _controller = new ImGuiController(_gameWindow);
        }

        public override void Run()
        {
            if (_gameWindow != null)
            {
                //MUST be 0, is handled internally by ImGui. Other values will lead to AccessViolation Exception.
                _gameWindow.UpdateFrequency = 0;
                _gameWindow.Run();
            }
        }

        /// <summary>
        /// Do nothing, pipe all Present() calls inside OnRenderFrame() to /dev/null
        /// We call present ourselves
        /// </summary>
        public override void Present()
        {

        }

        public override void DoInit()
        {
            _controller.InitImGUI(14, "Assets/Lato-Black.ttf");
            base.DoInit();
            _initialized = true;
        }

        public override void DoUpdate()
        {
            if (!_initialized) return;
            if (_isShuttingDown) return;

            // HACK(mr): Fixme, don't know why
            //Input.Instance.PreUpdate();

            base.DoUpdate();
            _controller.UpdateImGui(DeltaTimeUpdate);

            //Input.Instance.PostUpdate();
        }

        public override void DoRender()
        {
            if (!_initialized) return;
            if (_controller.GameWindowWidth <= 0) return;
            if (_isShuttingDown) return;

            Input.Instance.PreUpdate();

            base.DoRender();

            if (_isShuttingDown) return;

            _controller.RenderImGui();
            Input.Instance.PostUpdate();
        }

        public override void DoUnLoad()
        {
            base.DoUnLoad();
        }

        public override void DoResize(int width, int height)
        {
            base.DoResize(width, height);
            _controller?.WindowResized(width, height);
        }

        public override void CloseGameWindow()
        {
            _isShuttingDown = true;
            NativeWindow.ProcessWindowEvents(true);
            _controller.Dispose();
            _gameWindow.Context.MakeCurrent();
            _gameWindow.Close();
            _gameWindow.Dispose();
        }
    }
}