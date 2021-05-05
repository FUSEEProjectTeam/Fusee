using System;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Provides data for the MouseUp, MouseDown, and MouseMove events.
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        /// <summary>
        /// Gets and sets the button.
        /// </summary>
        /// <value>
        /// The button.
        /// </value>
        public MouseButtons Button { get; set; }

        /// <summary>
        /// Gets and sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Point Position { get; set; }
    }
}