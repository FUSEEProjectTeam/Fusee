using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Implementation agnostic representation of a cylinder shape.
    /// </summary>
    public interface ICylinderShapeImp : ICollisionShapeImp
    {
        /// <summary>
        /// Gets the half extents.
        /// </summary>
        /// <value>
        /// The half extents.
        /// </value>
        float3 HalfExtents { get; }
        /// <summary>
        /// Gets the radius.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        float Radius { get; }
        /// <summary>
        /// Gets up axis.
        /// </summary>
        /// <value>
        /// Up axis.
        /// </value>
        int UpAxis { get; }
    }
}