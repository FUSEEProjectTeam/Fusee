using System.Runtime.InteropServices;

namespace Fusee.Engine
{
    /// <summary>
    /// Sets the bounding box of a rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Rectangle
    {
        /// <summary>
        /// The left x-coordinate
        /// </summary>
        public int Left;

        /// <summary>
        /// The right x-coordinate
        /// </summary>
        public int Right;

        /// <summary>
        /// The upper y-coordinate
        /// </summary>
        public int Top;

        /// <summary>
        /// The lower y-coordinate
        /// </summary>
        public int Bottom;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        public Rectangle(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Gets the width of the rectangle.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get { return Right - Left; } }
        /// <summary>
        /// Gets the height of the rectangle.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get { return Bottom - Top; } }
    }
}
