using Fusee.Math.Core;
using Fusee.Structures;
using System.Collections.Generic;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Node/Bucket data structure that is used in a <see cref="PtOctreeRead{TPoint}"/>. Needed to save a point cloud into a file format that can be used for out of core rendering.
    /// </summary>
    /// <typeparam name="TPoint">The type pf the point cloud points.</typeparam>
    public class PtOctantRead<TPoint> : OctantD<TPoint>
    {
        /// <summary>
        /// The size, projected into screen space. Set with <seealso cref="ComputeScreenProjectedSize(double3, int, float)"/>.
        /// </summary>
        public double ProjectedScreenSize { get; private set; }

        /// <summary>
        /// Number of point cloud points, this node holds.
        /// </summary>
        public int NumberOfPointsInNode;

        /// <summary>
        /// Creates a new instance of type <see cref="PtOctantRead{TPoint}"/>.
        /// </summary>
        /// <param name="center">The center of this octant.</param>
        /// <param name="size">The size (in all three dimensions) of this octant.</param>
        /// <param name="children">The octants child octants.</param>
        public PtOctantRead(double3 center, double size, IOctant<double3, double, TPoint>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new IOctant<double3, double, TPoint>[8];
            else
                Children = children;
            
            Payload = new List<TPoint>(NumberOfPointsInNode);
        }

        /// <summary>
        /// Calculates the size, projected into screen space.
        /// </summary>
        /// <param name="camPos">Position of the camera.</param>
        /// <param name="screenHeight">Hight of the canvas.</param>
        /// <param name="fov">Field of view.</param>
        public void ComputeScreenProjectedSize(double3 camPos, int screenHeight, float fov)
        {
            var distance = (Center - camPos).Length;
            var slope = (float)System.Math.Tan(fov / 2d);
            ProjectedScreenSize = screenHeight / 2d * Size / (slope * distance);
        }

        /// <summary>
        /// Creates a child at the given position. 
        /// </summary>
        /// <param name="posInParent">The new octants position in its parent.</param>
        /// <returns></returns>
        public override IOctant<double3, double, TPoint> CreateChild(int posInParent)
        {
            var childCenter = CalcChildCenterAtPos(posInParent);

            var childRes = Size / 2d;
            var child = new PtOctantRead<TPoint>(childCenter, childRes)
            {
                Resolution = Resolution / 2d,
                Level = Level + 1
            };

            return child;
        }
    }
}