using System.Runtime.InteropServices;

namespace Fusee.Engine
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Point
    {
        public int x;
        public int y;
    }
}
