using System;

namespace Fusee.Engine
{

    /// <summary>
    /// Provides data for the KeyDown or KeyUp event. 
    /// </summary>
    public class KeyEventArgs : EventArgs
    {

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="KeyEventArgs"/> is shift.
        /// </summary>
        /// <value>
        ///   <c>true</c> if shift; otherwise, <c>false</c>.
        /// </value>
        public bool Shift { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="KeyEventArgs"/> is alt.
        /// </summary>
        /// <value>
        ///   <c>true</c> if alt; otherwise, <c>false</c>.
        /// </value>
        public bool Alt { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="KeyEventArgs"/> is control.
        /// </summary>
        /// <value>
        ///   <c>true</c> if control; otherwise, <c>false</c>.
        /// </value>
        public bool Control { get; set; }
        /// <summary>
        /// Gets or sets the key code.
        /// </summary>
        /// <value>
        /// The key code.
        /// </value>
        public KeyCodes KeyCode { get; set; }
    }
}
