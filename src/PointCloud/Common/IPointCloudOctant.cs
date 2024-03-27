using Fusee.Math.Core;
using Fusee.Structures;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Used in <see cref="IPointCloudOctree"/>. Allows the use in non-generic context, e.g. in <see cref="IPointReader"/>s.
    /// </summary>
    public interface IPointCloudOctant : IEmptyOctant<double3, double3>
    {
        /// <summary>
        /// The number of points that are in this octant.
        /// </summary>
        public int NumberOfPointsInNode { get; set; }

        /// <summary>
        /// The number of points this octant can hold.
        /// </summary>
        public int PointCapacity { get; }

        /// <summary>
        /// Calculates the size, projected into screen space.
        /// </summary>
        /// <param name="camPos">Position of the camera.</param>
        /// <param name="screenHeight">Hight of the canvas.</param>
        /// <param name="fov">Field of view.</param>
        /// <param name="translation">Translation of the octant.</param>
        /// <param name="scale">Scale of the octant.</param>
        public void ComputeScreenProjectedSize(double3 camPos, int screenHeight, float fov, float3 translation, float3 scale);

        /// <summary>
        /// The size, projected into screen space. Set with <seealso cref="ComputeScreenProjectedSize(double3, int, float, float3, float3)"/>.
        /// </summary>
        public double ProjectedScreenSize { get; }

        /// <summary>
        /// Checks if this Octant lies within or intersects a Frustum.
        /// Returns true if one of the Frustum planes is intersecting this octant.
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <param name="translation">Additional translation.</param>
        /// <param name="scale">Additional scale.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumF frustum, float3 translation, float3 scale);

        /// <summary>
        /// True if octant is currently visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// The globally unique identifier for this octant.
        /// </summary>
        public OctantId OctId { get; }

        /// <summary>
        /// True, if this octant is marked as proxy/helper node.
        /// Proxy nodes can have children but don't necessarily have any payload.
        /// </summary>
        public bool IsProxy { get; }

        /// <summary>
        /// Support lazy loading.
        /// </summary>
        public bool Initialized { get; }
    }
}