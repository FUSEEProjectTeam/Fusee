using Fusee.Math.Core;

namespace Fusee.PointCloud.Common.Accessors
{
    /// <summary>
    /// Declares valid data types for a point cloud's position data.
    /// </summary>
    public enum PointPositionType
    {
        /// <summary>
        /// The position of this point is undefined - renders the point unusable.
        /// </summary>
        Undefined,

        /// <summary>
        /// A point cloud point has a position value of type <see cref="float3"/>.
        /// </summary>
        Float3,
        /// <summary>
        /// A point cloud point has a position value of type <see cref="double3"/>.
        /// </summary>
        Double3
    }
}