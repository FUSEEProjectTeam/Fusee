#pragma warning disable 1591 //disables the warning about missing XML-comments

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
        event EventHandler<InitEventArgs> UnLoad; 
        event EventHandler<RenderEventArgs> Render;
        event EventHandler<ResizeEventArgs> Resize;
    }
}

#pragma warning restore 1591
