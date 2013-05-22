using System;

namespace Fusee.Engine
{
    public interface IRenderCanvasImp
    {
        int Width { get ; }
        int Height { get; }
        string Caption { get; set; }

        double DeltaTime { get; }

        bool VerticalSync { get; set; }

        void Present();

        void Run();

        event EventHandler<InitEventArgs> Init;
        event EventHandler<InitEventArgs> UnLoad; 
        event EventHandler<RenderEventArgs> Render;
        event EventHandler<ResizeEventArgs> Resize;
    }
}
