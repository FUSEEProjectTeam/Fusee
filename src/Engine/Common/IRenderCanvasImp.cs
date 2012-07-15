using System;

namespace Fusee.Engine
{
    public interface IRenderCanvasImp
    {
        int Width { get ; }
        int Height { get; }

        double DeltaTime { get; }

        void Present();

        void Run();

        event EventHandler<InitEventArgs> Init;
        event EventHandler<RenderEventArgs> Render;
        event EventHandler<ResizeEventArgs> Resize;
    }
}
