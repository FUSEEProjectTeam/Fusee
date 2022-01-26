using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.Structures;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// In addition to the information in  <see cref="OctantD{P}"/> this class provides methods and properties, that are used to render point clouds out of core.
    /// </summary>
    public class PtOctant : OctantD<IPointCloudPoint>
    {
        /// <summary>
        /// The size, projected into screen space. Set with <seealso cref="ComputeScreenProjectedSize(double3, int, float)"/>.
        /// </summary>
        public double ProjectedScreenSize { get; private set; }

        /// <summary>
        /// Number of point cloud points, this node holds.
        /// </summary>
        public int NumberOfPointsInNode { get; set; }

        /// <summary>
        /// Creates a new instance of type <see cref="PtOctant"/>.
        /// </summary>
        /// <param name="center">The center of this octant.</param>
        /// <param name="size">The size (in all three dimensions) of this octant.</param>
        /// <param name="guid"></param>
        /// <param name="children">The octants child octants.</param>
        public PtOctant(double3 center, double size, string guid, PtOctant[] children = null) : base(center, size, guid, children) { }

        /// <summary>
        /// Calculates the size, projected into screen space.
        /// </summary>
        /// <param name="camPos">Position of the camera.</param>
        /// <param name="screenHeight">Hight of the canvas.</param>
        /// <param name="fov">Field of view.</param>
        public void ComputeScreenProjectedSize(double3 camPos, int screenHeight, float fov)
        {
            var distance = (Center - camPos).Length;
            if (Center == camPos)
                distance = 0.0001f;
            var slope = (float)System.Math.Tan(fov / 2d);
            ProjectedScreenSize = screenHeight / 2d * Size / (slope * distance);
        }

        /// <summary>
        /// Creates a child at the given position. 
        /// </summary>
        /// <param name="posInParent">The new octants position in its parent.</param>
        /// <returns></returns>
        public override IOctant<double3, double, IPointCloudPoint> CreateChild(int posInParent)
        {
            var childCenter = CalcChildCenterAtPos(posInParent);

            var childRes = Size / 2d;
            var child = new PtOctant(childCenter, childRes, Guid + posInParent)
            {
                Resolution = Resolution / 2d
            };

            return child;
        }
    }
}