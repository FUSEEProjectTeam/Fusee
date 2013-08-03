using System;

namespace Fusee.Engine
{
    public interface IRenderCanvasImp
    {
        int Width { get; set; }
        int Height { get; set; }
        string Caption { get; set; }

        double DeltaTime { get; }

        bool VerticalSync { get; set; }
        bool EnableBlending { get; set; }
        bool Fullscreen { get; set; }

        void Present();

        void Run();

        event EventHandler<InitEventArgs> Init;
        event EventHandler<InitEventArgs> UnLoad; 
        event EventHandler<RenderEventArgs> Render;
        event EventHandler<ResizeEventArgs> Resize;
    }
}
