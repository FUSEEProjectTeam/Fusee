using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine
{
    /// <summary>
    /// Provides data for the MouseUp, MouseDown, and MouseMove events.
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the button.
        /// </summary>
        /// <value>
        /// The button.
        /// </value>
        public MouseButtons Button { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Point Position { get; set; }
    }
}
