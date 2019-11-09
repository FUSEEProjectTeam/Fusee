using System;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Argument Container for Resize event of <see cref="IRenderCanvasImp"/>.
    /// </summary>
    public class ResizeEventArgs : EventArgs
    {
        /// <summary>
        /// The width held for the Resize event.
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// The height held for the Resize event.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Constructs a new ResizeEventArgs.
        /// </summary>
        /// <param name="width">The width held for the Resize event.</param>
        /// <param name="height">The height held for the Resize event.</param>
        public ResizeEventArgs(int width, int height)
        {
            Width = width;
            Height = height;
        }

    }
}
