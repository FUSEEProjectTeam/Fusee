using Android.Views;
using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Android
{
    /// <summary>
    /// Implementation of the cross-platform abstraction of the window handle.
    /// </summary>
    public class WindowHandle : IWindowHandle
    {
        /// <summary>
        /// The window handle
        /// </summary>
        public WindowId WinId { get; internal set; }
    }
}