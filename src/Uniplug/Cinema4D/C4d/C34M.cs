using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace C4d
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct C34M
    {
        public fixed double m[12];
    };
}
