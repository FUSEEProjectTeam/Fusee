using Fusee.Math.Core;

namespace Fusee.PointCloud.Potree.V2.Data
{
    /// <summary>
    /// This point is used for visualization purposes.
    /// It is read by <see cref="Potree2Reader"/> and converted to mesh data.
    /// </summary>
    public struct VisualizationPoint
    {
        public float3 Position;
        public float4 Color;
        public uint Flags;
    }
}