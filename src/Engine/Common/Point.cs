using System.Runtime.InteropServices;

namespace Fusee.Engine
{
    /// <summary>
    /// Sets a point in 3D space.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Point
    {
        /// <summary>
        /// The x-coordinate
        /// </summary>
        public int x;
        /// <summary>
        /// The y-coordinate
        /// </summary>
        public int y;
        /// <summary>
        /// The z-coordinate
        /// </summary>
        public int z;
    }
}
