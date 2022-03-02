using Fusee.Math.Core;

namespace Fusee.PointCloud.Potree.V2
{
    internal static class Constants
    {
        internal const string HierarchyFileName = "hierarchy.bin";
        internal const string MetadataFileName = "metadata.json";
        internal const string OctreeFileName = "octree.bin";

        internal static readonly double4x4 YZflip = new double4x4()
        {
            M11 = 1,
            M12 = 0,
            M13 = 0,
            M14 = 0,
            M21 = 0,
            M22 = 0,
            M23 = 1,
            M24 = 0,
            M31 = 0,
            M32 = 1,
            M33 = 0,
            M34 = 0,
            M41 = 0,
            M42 = 0,
            M43 = 0,
            M44 = 1
        };
    }
}