using Fusee.Math.Core;


namespace Fusee.Examples.PcRendering.Core
{
    /// <summary>
    /// User-given class that defines the data a point consists of.
    /// </summary>
    public class LAZPointType
    {
        public double3 Position;
        public float3 Color;
        public ushort Intensity;
        public int3 GridIndex;
    }
}
