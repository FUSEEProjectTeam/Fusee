using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Interface to abstract a box shape
    /// </summary>
    public interface IBoxShapeImp : ICollisionShapeImp
    {
        /// <summary>
        /// Gets the half extents.
        /// </summary>
        /// <value>
        /// The half extents (half the length of widht, heigt and depth).
        /// </value>
        float3 HalfExtents { get; }
    }
}
