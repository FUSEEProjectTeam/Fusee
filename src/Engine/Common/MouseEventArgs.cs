using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class MouseEventArgs : EventArgs
    {
        public MouseButtons Button { get; set; }
        public Point Position { get; set; }
    }
}
