using System;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Argument Container for Resize event of <see cref="IRenderCanvasImp"/>.
    /// </summary>
    public class ResizeEventArgs : EventArgs
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public ResizeEventArgs(int width, int height)
        {
            Width = width;
            Height = height;
        }

    }
}
