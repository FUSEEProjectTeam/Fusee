using Fusee.Engine.Common;
using System;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
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
}