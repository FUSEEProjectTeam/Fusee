using System;

namespace Fusee.Engine
{
    public class KeyEventArgs : EventArgs
    {
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public bool Control { get; set; }
        public KeyCodes KeyCode { get; set; }
    }
}
