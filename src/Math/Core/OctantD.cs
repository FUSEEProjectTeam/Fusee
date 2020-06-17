
namespace Fusee.Math.Core
{
    public class OctantD
    {
        /// <summary>
        /// Defines the position in the parent octant.
        /// </summary>
        public int PosInParent;

        /// <summary>
        /// The level of the octree the node lies in.
        /// </summary>
        public int Level;

        /// <summary>
        /// Center in world space.
        /// </summary>
        public double3 Center;

        /// <summary>
        /// Length of on side of the cubical node.
        /// </summary>
        public double Size;

        /// <summary>
        /// Defines if the node is a leaf node.
        /// </summary>
        public bool IsLeaf;

        /// <summary>
        /// The size, projected into screen space. Set with <seealso cref="ComputeScreenProjectedSize(double3, int, float)"/>.
        /// </summary>
        public double ProjectedScreenSize { get; private set; }

        /// <summary>
        /// Calculates the size, projected into screen space.
        /// </summary>
        /// <param name="camPos">Position of the camera.</param>
        /// <param name="screenHeight">Hight of the canvas.</param>
        /// <param name="fov">Field of view.</param>
        public void ComputeScreenProjectedSize(double3 camPos, int screenHeight, float fov)
        {
            var distance = (Center - camPos).Length;
            var slope = (float)System.Math.Tan(fov / 2f);
            ProjectedScreenSize = screenHeight / 2d * Size / (slope * distance);
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this AABB.      
        /// </summary>
        /// <param name="plane">The plane to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingPlane(PlaneD plane)
        {
            return plane.InsideOrIntersecting(this);
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this AABB.      
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumD frustum)
        {
            if (!frustum.Near.InsideOrIntersecting(this))
                return false;
            if (!frustum.Far.InsideOrIntersecting(this))
                return false;
            if (!frustum.Left.InsideOrIntersecting(this))
                return false;
            if (!frustum.Right.InsideOrIntersecting(this))
                return false;
            if (!frustum.Top.InsideOrIntersecting(this))
                return false;
            if (!frustum.Bottom.InsideOrIntersecting(this))
                return false;

            return true;
        }
    }
}
